using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGT.BackgroundWorkers.Utils;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 86400000)]

    public class Alerta : LongRunningProcessBase<Alerta>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            ProcessarAlertaRegraICMS(unitOfWork, _tipoServicoMultisoftware);
            ProcessarAlertaVeiculo(unitOfWork, _tipoServicoMultisoftware);
            ProcessarAlertaPessoa(unitOfWork, _tipoServicoMultisoftware);
            ProcessarAlertaMotorista(unitOfWork, _tipoServicoMultisoftware);
            ProcessarAlertaTabelaFrete(unitOfWork, _tipoServicoMultisoftware);
            ProcessarAlertaManutencao(unitOfWork, _tipoServicoMultisoftware);
            ProcessarAlertaControleArquivo(unitOfWork);
            ProcessarAlertaEstoqueMinimo(unitOfWork, _tipoServicoMultisoftware);
            ProcessarAlertaOrdemServicoInterna(unitOfWork, _tipoServicoMultisoftware);
            ProcessarAlertaOrdemServicoExterna(unitOfWork, _tipoServicoMultisoftware);
            ProcessarAlertaChecklist(unitOfWork, _tipoServicoMultisoftware);
        }

        #region Métodos Privados

        private void ProcessarAlertaRegraICMS(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracoes.ControleAlerta repControleAlerta = new Repositorio.Embarcador.Configuracoes.ControleAlerta(unitOfWork);
            Repositorio.Embarcador.ICMS.RegraICMS repRegraICMS = new Repositorio.Embarcador.ICMS.RegraICMS(unitOfWork);

            List<Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta> controlesRegraICMS = repControleAlerta.BuscarControlesPorTela(ControleAlertaTela.RegraICMS);

            if (controlesRegraICMS == null || controlesRegraICMS.Count == 0)
                return;

            try
            {
                List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> regrasICMS = repRegraICMS.BuscarRegrasParaAlerta();
                foreach (Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMS in regrasICMS)
                {
                    foreach (Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta controle in controlesRegraICMS)
                    {
                        if (regraICMS.VigenciaFim.HasValue && regraICMS.VigenciaFim.Value.Date.AddDays(-controle.QuantidadeDias) <= DateTime.Now.Date)
                        {
                            string descricaoAlerta = string.Empty;
                            if (regraICMS.VigenciaFim.HasValue && regraICMS.VigenciaFim.Value.Date <= DateTime.Now.Date)
                                descricaoAlerta = string.Format(Localization.Resources.Threads.Alerta.RegraICMSDescricaoVencida, regraICMS.Descricao, regraICMS.DescricaoRegra);
                            else if (regraICMS.VigenciaFim.HasValue)
                                descricaoAlerta = string.Format(Localization.Resources.Threads.Alerta.RegraICMSDescricaoAVencer, regraICMS.Descricao, regraICMS.DescricaoRegra, (regraICMS.VigenciaFim.Value.Date - DateTime.Now.Date).TotalDays);

                            SalvarGerarEnviarAlerta(controle, descricaoAlerta, regraICMS.Codigo, regraICMS.Codigo, "ICMS/RegraICMS", "Alerta de Regra de ICMS", ControleAlertaTela.RegraICMS, unitOfWork, tipoServicoMultisoftware);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void ProcessarAlertaVeiculo(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracoes.ControleAlerta repControleAlerta = new Repositorio.Embarcador.Configuracoes.ControleAlerta(unitOfWork);
            Repositorio.Embarcador.Veiculos.LicencaVeiculo repLicencaVeiculo = new Repositorio.Embarcador.Veiculos.LicencaVeiculo(unitOfWork);

            List<Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta> controlesVeiculos = repControleAlerta.BuscarControlesPorTela(ControleAlertaTela.Veiculo);

            if (controlesVeiculos == null || controlesVeiculos.Count == 0)
                return;

            try
            {
                List<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo> licencasVeiculos = repLicencaVeiculo.BuscarLicencasParaAlerta();
                foreach (Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo licenca in licencasVeiculos)
                {
                    foreach (Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta controle in controlesVeiculos)
                    {
                        if (licenca.DataVencimento.Value.Date.AddDays(-controle.QuantidadeDias) <= DateTime.Now.Date)
                        {
                            string descricaoAlerta = string.Empty;
                            if (licenca.DataVencimento.Value.Date <= DateTime.Now.Date)
                                descricaoAlerta = string.Format(Localization.Resources.Threads.Alerta.LicencaVeiculoVencida, licenca.Descricao, licenca.Veiculo.Placa_Formatada);
                            else
                                descricaoAlerta = string.Format(Localization.Resources.Threads.Alerta.LicencaVeiculoVencida, licenca.Descricao, licenca.Veiculo.Placa_Formatada, (licenca.DataVencimento.Value.Date - DateTime.Now.Date).TotalDays);

                            SalvarGerarEnviarAlerta(controle, descricaoAlerta, licenca.Codigo, licenca.Veiculo.Codigo, "Veiculos/Veiculo", "Alerta de Licença de Veículo", ControleAlertaTela.Veiculo,
                                unitOfWork, tipoServicoMultisoftware, licenca.Licenca?.Email);
                        }
                    }

                    if (licenca.DataVencimento.Value <= DateTime.Now.Date)
                    {
                        licenca.Status = StatusLicenca.Vencido;
                        repLicencaVeiculo.Atualizar(licenca);
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void ProcessarAlertaPessoa(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracoes.ControleAlerta repControleAlerta = new Repositorio.Embarcador.Configuracoes.ControleAlerta(unitOfWork);
            Repositorio.Embarcador.Pessoas.PessoaLicenca repPessoaLicenca = new Repositorio.Embarcador.Pessoas.PessoaLicenca(unitOfWork);

            List<Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta> controlesPessoas = repControleAlerta.BuscarControlesPorTela(ControleAlertaTela.Pessoa);

            if (controlesPessoas == null || controlesPessoas.Count == 0)
                return;

            try
            {
                List<Dominio.Entidades.Embarcador.Pessoas.PessoaLicenca> licencasPessoas = repPessoaLicenca.BuscarLicencasParaAlerta();
                foreach (Dominio.Entidades.Embarcador.Pessoas.PessoaLicenca licenca in licencasPessoas)
                {
                    foreach (Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta controle in controlesPessoas)
                    {
                        if (licenca.DataVencimento.Value.Date.AddDays(-controle.QuantidadeDias) <= DateTime.Now.Date)
                        {
                            string descricaoAlerta = string.Empty;
                            if (licenca.DataVencimento.Value.Date <= DateTime.Now.Date)
                                descricaoAlerta = string.Format(Localization.Resources.Threads.Alerta.LicencaPessoaVencida, licenca.Descricao, licenca.Pessoa.Descricao);
                            else
                                descricaoAlerta += string.Format(Localization.Resources.Threads.Alerta.LicencaPessoaVencida, licenca.Descricao, licenca.Pessoa.Descricao, (licenca.DataVencimento.Value.Date - DateTime.Now.Date).TotalDays);

                            SalvarGerarEnviarAlerta(controle, descricaoAlerta, licenca.Codigo, 0, "Pessoas/Pessoa", "Alerta de Licença de Pessoa", ControleAlertaTela.Pessoa,
                                unitOfWork, tipoServicoMultisoftware, licenca.Licenca?.Email);
                        }
                    }

                    if (licenca.DataVencimento.Value <= DateTime.Now.Date)
                    {
                        licenca.Status = StatusLicenca.Vencido;
                        repPessoaLicenca.Atualizar(licenca);
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void ProcessarAlertaMotorista(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracoes.ControleAlerta repControleAlerta = new Repositorio.Embarcador.Configuracoes.ControleAlerta(unitOfWork);
            Repositorio.Embarcador.Transportadores.MotoristaLicenca repMotoristaLicenca = new Repositorio.Embarcador.Transportadores.MotoristaLicenca(unitOfWork);

            List<Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta> controlesMotorista = repControleAlerta.BuscarControlesPorTela(ControleAlertaTela.Motorista);

            if (controlesMotorista == null || controlesMotorista.Count == 0)
                return;

            try
            {
                List<Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca> licencasMotoristas = repMotoristaLicenca.BuscarLicencasParaAlerta();
                foreach (Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca licenca in licencasMotoristas)
                {
                    foreach (Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta controle in controlesMotorista)
                    {
                        if (licenca.DataVencimento.Value.Date.AddDays(-controle.QuantidadeDias) <= DateTime.Now.Date)
                        {
                            string descricaoAlerta = string.Empty;
                            if (licenca.DataVencimento.Value.Date <= DateTime.Now.Date)
                                descricaoAlerta = string.Format(Localization.Resources.Threads.Alerta.LicencaMotoristaVencida, licenca.Descricao, licenca.Motorista.Descricao);
                            else
                                descricaoAlerta = string.Format(Localization.Resources.Threads.Alerta.LicencaMotoristaVencida, licenca.Descricao, licenca.Motorista.Descricao, (licenca.DataVencimento.Value.Date - DateTime.Now.Date).TotalDays);

                            SalvarGerarEnviarAlerta(controle, descricaoAlerta, licenca.Codigo, licenca.Motorista.Codigo, "Transportadores/Motorista", "Alerta de Licença de Motorista", ControleAlertaTela.Motorista,
                                unitOfWork, tipoServicoMultisoftware, licenca.Licenca?.Email);
                        }
                    }

                    if (licenca.DataVencimento.Value <= DateTime.Now.Date)
                    {
                        licenca.Status = StatusLicenca.Vencido;
                        repMotoristaLicenca.Atualizar(licenca);
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void ProcessarAlertaTabelaFrete(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracoes.ControleAlerta repControleAlerta = new Repositorio.Embarcador.Configuracoes.ControleAlerta(unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFreteCliente repTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);

            List<Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta> controlesTabelaFrete = repControleAlerta.BuscarControlesPorTela(ControleAlertaTela.TabelaFrete);

            if (controlesTabelaFrete == null || controlesTabelaFrete.Count == 0)
                return;

            try
            {
                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> vigenciasTabelas = repTabelaFreteCliente.BuscarTabelasParaAlerta();
                foreach (Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente licenca in vigenciasTabelas)
                {
                    foreach (Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta controle in controlesTabelaFrete)
                    {
                        if (licenca.Vigencia != null && licenca.Vigencia.DataFinal.HasValue && licenca.Vigencia.DataFinal.Value.Date.AddDays(-controle.QuantidadeDias) <= DateTime.Now.Date)
                        {
                            string descricaoAlerta = string.Empty;
                            if (licenca.Vigencia.DataFinal.Value.Date <= DateTime.Now.Date)
                                descricaoAlerta = string.Format(Localization.Resources.Threads.Alerta.TabelaFreteVencida, licenca.Descricao);
                            else
                                descricaoAlerta = string.Format(Localization.Resources.Threads.Alerta.TabelaFreteAVencer, licenca.Descricao, (licenca.Vigencia.DataFinal.Value.Date - DateTime.Now.Date).TotalDays);

                            SalvarGerarEnviarAlerta(controle, descricaoAlerta, licenca.Codigo, licenca.Codigo, "Fretes/TabelaFreteCliente", "Alerta de Tabela de Frete", ControleAlertaTela.TabelaFrete, unitOfWork, tipoServicoMultisoftware);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void ProcessarAlertaManutencao(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracoes.ControleAlerta repControleAlerta = new Repositorio.Embarcador.Configuracoes.ControleAlerta(unitOfWork);
            Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServicoFrota = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            List<Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta> controlesManutencoes = repControleAlerta.BuscarControlesPorTela(ControleAlertaTela.Manutencao);

            if (controlesManutencoes == null || controlesManutencoes.Count == 0)
                return;

            try
            {
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioManutencaoVeiculo filtroRelatorioManutencao = new Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioManutencaoVeiculo()
                {
                    VisualizarServicosPendentesManutencao = true,
                    PropriedadeVeiculo = "P",
                    TiposManutencao = new List<TipoManutencaoServicoVeiculo> { TipoManutencaoServicoVeiculo.Preventiva },
                    UtilizarValidadeServicoPeloGrupoServicoOrdemServico = configuracao.UtilizarValidadeServicoPeloGrupoServicoOrdemServico
                };
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta();
                IList<Dominio.Relatorios.Embarcador.DataSource.Frota.ManutencaoVeiculo> manutencaoVeiculos = repOrdemServicoFrota.RelatorioManutencaoVeiculo(filtroRelatorioManutencao, parametrosConsulta);
                foreach (Dominio.Relatorios.Embarcador.DataSource.Frota.ManutencaoVeiculo manutencao in manutencaoVeiculos)
                {
                    foreach (Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta controle in controlesManutencoes)
                    {
                        string descricaoAlerta = string.Format(Localization.Resources.Threads.Alerta.ServicoManutencaoVeiculoPendente, manutencao.DescricaoServico, manutencao.PlacaVeiculo);

                        SalvarGerarEnviarAlerta(controle, descricaoAlerta, manutencao.CodigoVeiculo, manutencao.CodigoVeiculo, "Veiculos/Veiculo", "Alerta de Manutenção de Veículo", ControleAlertaTela.Manutencao, unitOfWork, tipoServicoMultisoftware);
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void ProcessarAlertaControleArquivo(Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Repositorio.Embarcador.Anexo.ControleArquivo repControleArquivo = new Repositorio.Embarcador.Anexo.ControleArquivo(unitOfWork);

                List<Dominio.Entidades.Embarcador.Anexo.ControleArquivo> controlesArquivos = repControleArquivo.BuscarControlesPendentesBaixar();
                Servicos.Embarcador.Anexo.ControleArquivo servicoControleArquivo = new Servicos.Embarcador.Anexo.ControleArquivo(unitOfWork);

                if (controlesArquivos == null || controlesArquivos.Count == 0)
                    return;

                foreach (Dominio.Entidades.Embarcador.Anexo.ControleArquivo controleArquivo in controlesArquivos)
                {
                    if (controleArquivo.DataVencimento.HasValue && (controleArquivo.DataVencimento.Value.AddDays(-5) <= DateTime.Now.Date || controleArquivo.DataVencimento.Value.Date == DateTime.Now.Date.AddDays(-1)))
                    {
                        servicoControleArquivo.EnviarEmailAlerta(controleArquivo);
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void ProcessarAlertaEstoqueMinimo(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracoes.ControleAlerta repControleAlerta = new Repositorio.Embarcador.Configuracoes.ControleAlerta(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.ProdutoEstoque repProdutoEstoque = new Repositorio.Embarcador.NotaFiscal.ProdutoEstoque(unitOfWork);

            List<Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta> controlesEstoqueMinimo = repControleAlerta.BuscarControlesPorTela(ControleAlertaTela.EstoqueMinimo);

            if (controlesEstoqueMinimo == null || controlesEstoqueMinimo.Count == 0)
                return;

            try
            {
                List<Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque> estoquesMinimos = repProdutoEstoque.BuscarEstoquesMinimosParaAlerta();
                foreach (Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque estoque in estoquesMinimos)
                {
                    foreach (Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta controle in controlesEstoqueMinimo)
                    {
                        string descricaoAlerta = string.Format(Localization.Resources.Threads.Alerta.ProdutoEstoqueMinimoQuantidadeAtual, estoque.Produto.Descricao, estoque.EstoqueMinimo.ToString("n2"), estoque.Quantidade.ToString("n2"));

                        SalvarGerarEnviarAlerta(controle, descricaoAlerta, estoque.Codigo, estoque.Codigo, "Produtos/Produto", "Alerta de Estoque Mínimo de Produto", ControleAlertaTela.EstoqueMinimo, unitOfWork, tipoServicoMultisoftware);
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void ProcessarAlertaOrdemServicoInterna(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracoes.ControleAlerta repControleAlerta = new Repositorio.Embarcador.Configuracoes.ControleAlerta(unitOfWork);
            Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServicoFrota = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unitOfWork);

            List<Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta> controlesOsIterna = repControleAlerta.BuscarControlesPorTela(ControleAlertaTela.OrdemServicoInterna);

            if (controlesOsIterna == null || controlesOsIterna.Count == 0)
                return;

            try
            {
                foreach (Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta controle in controlesOsIterna)
                {
                    List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota> ordemServicoFrotasInternas = repOrdemServicoFrota.BuscarPorSituacao(controle.SituacoesOrdemServico?.ToList(), TipoOficina.Interna);
                    foreach (Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordem in ordemServicoFrotasInternas)
                    {
                        if (ordem.DataAlteracao.HasValue && (ordem.DataAlteracao.Value.Date - DateTime.Now.Date).TotalDays >= controle.QuantidadeDiasAlertaOsInterna)
                        {
                            string descricaoAlerta = string.Format(Localization.Resources.Threads.Alerta.OrdemServicoStatus, ordem.Numero, controle.QuantidadeDiasAlertaOsInterna, ordem.Situacao);

                            SalvarGerarEnviarAlerta(controle, descricaoAlerta, ordem.Codigo, ordem.Codigo, "Frota/OrdemServico", "Alerta Ordem de Serviço", ControleAlertaTela.OrdemServicoInterna, unitOfWork, tipoServicoMultisoftware);
                        }

                        ordem.AlertaEnviado = true;
                        repOrdemServicoFrota.Atualizar(ordem);
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void ProcessarAlertaOrdemServicoExterna(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracoes.ControleAlerta repControleAlerta = new Repositorio.Embarcador.Configuracoes.ControleAlerta(unitOfWork);
            Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServicoFrota = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unitOfWork);

            List<Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta> controlesOsExterna = repControleAlerta.BuscarControlesPorTela(ControleAlertaTela.OrdemServicoExterna);

            if (controlesOsExterna == null || controlesOsExterna.Count == 0)
                return;

            try
            {
                foreach (Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta controle in controlesOsExterna)
                {
                    List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota> ordemServicoFrotasExterna = repOrdemServicoFrota.BuscarPorSituacao(controle.SituacoesOrdemServico?.ToList(), TipoOficina.Externa);
                    foreach (Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemEx in ordemServicoFrotasExterna)
                    {
                        if (ordemEx.DataAlteracao.HasValue && (ordemEx.DataAlteracao.Value.Date - DateTime.Now.Date).TotalDays >= controle.QuantidadeDiasAlertaOsInterna)
                        {
                            string descricaoAlerta = string.Format(Localization.Resources.Threads.Alerta.OrdemServicoStatus, ordemEx.Numero, controle.QuantidadeDiasAlertaOsInterna, ordemEx.Situacao);
                            SalvarGerarEnviarAlerta(controle, descricaoAlerta, ordemEx.Codigo, ordemEx.Codigo, "Frota/OrdemServico", "Alerta Ordem de Serviço", ControleAlertaTela.OrdemServicoInterna, unitOfWork, tipoServicoMultisoftware);
                        }

                        ordemEx.AlertaEnviado = true;
                        repOrdemServicoFrota.Atualizar(ordemEx);
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void ProcessarAlertaChecklist(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracoes.ControleAlerta repControleAlerta = new Repositorio.Embarcador.Configuracoes.ControleAlerta(unitOfWork);
            Repositorio.Embarcador.GestaoPatio.GuaritaCheckList repGuaritaChecklist = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckList(unitOfWork);

            List<Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta> controlesChecklists = repControleAlerta.BuscarControlesPorTela(ControleAlertaTela.CheckList);

            if (controlesChecklists == null || controlesChecklists.Count == 0)
                return;

            try
            {
                IList<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.CheckListVistoria> checkListVistoria = repGuaritaChecklist.BuscarCheckListParaAlerta();
                foreach (Dominio.ObjetosDeValor.Embarcador.GestaoPatio.CheckListVistoria vistoria in checkListVistoria)
                {
                    foreach (Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta controle in controlesChecklists)
                    {
                        if (vistoria.DataProgramada.Date.AddDays(-vistoria.DiasVencimento) <= DateTime.Now.Date)
                        {
                            string descricaoAlerta = string.Format(Localization.Resources.Threads.Alerta.VeiculoPlacaVistoriaChecklistVencida, vistoria.Placa, vistoria.DescricaoTipoCheckList);
                            SalvarGerarEnviarAlerta(controle, descricaoAlerta, vistoria.Codigo, 0, "GestaoPatio/GuaritaCheckList", "Alerta de Licença de Checklist", ControleAlertaTela.CheckList, unitOfWork, tipoServicoMultisoftware);
                        }
                    }

                    if (vistoria.DataProgramada.Date.AddDays(-vistoria.DiasVencimento) <= DateTime.Now.Date)
                    {
                        Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                        Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlaca(vistoria.Placa);

                        Servicos.Embarcador.GestaoPatio.GuaritaCheckList.GerarCheckList(null, unitOfWork, vistoria.KmAtual, TipoEntradaSaida.Entrada, null, null, veiculo, "GERADO PELO ALERTA CHECKLIST DEVIDO A PRAZO PERIODICIDADE VENCIMENTO!", vistoria.CodigoTipoChecklist, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void SalvarGerarEnviarAlerta(Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta controle, string descricaoAlerta, int codigoEntidade, int codigoObjetoNotificacao, string URLPagina, string tituloEmail,
            ControleAlertaTela telaAlerta, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string outrosEmails = "")
        {
            Repositorio.Embarcador.Configuracoes.Alerta repAlerta = new Repositorio.Embarcador.Configuracoes.Alerta(unitOfWork);
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Alerta alerta = new Dominio.Entidades.Embarcador.Configuracoes.Alerta();
            alerta.CodigoEntidade = codigoEntidade;
            alerta.Descricao = descricaoAlerta;
            alerta.Empresa = tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? controle.Empresa : null;
            alerta.Funcionario = controle.Funcionario;
            alerta.Ocultar = false;
            alerta.TelaAlerta = telaAlerta;
            alerta.Data = DateTime.Now;
            alerta.FormasAlerta = new List<ControleAlertaForma>();

            if (controle.FormasAlerta.Contains(ControleAlertaForma.Notificacao))
            {
                alerta.FormasAlerta.Add(ControleAlertaForma.Notificacao);
                Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(unitOfWork.StringConexao, null, tipoServicoMultisoftware, string.Empty);
                serNotificacao.GerarNotificacao(controle.Funcionario, codigoObjetoNotificacao, URLPagina, descricaoAlerta, IconesNotificacao.agConfirmacao, TipoNotificacao.todas, tipoServicoMultisoftware, unitOfWork);
            }

            if (controle.FormasAlerta.Contains(ControleAlertaForma.Email))
            {
                alerta.FormasAlerta.Add(ControleAlertaForma.Email);

                List<string> emails = new List<string>();
                if (!string.IsNullOrWhiteSpace(alerta.Funcionario.Email))
                    emails.AddRange(alerta.Funcionario.Email.Split(';').ToList());

                if (!string.IsNullOrWhiteSpace(outrosEmails))
                    emails.AddRange(outrosEmails.Split(';').ToList());

                emails = emails.Distinct().ToList();
                if (emails.Count > 0)
                {
                    Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();
                    if (email != null)
                    {
                        Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, emails.ToArray(), null, tituloEmail, descricaoAlerta, email.Smtp, out string mensagemErro, email.DisplayEmail, null, "",
                            email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unitOfWork, alerta.Empresa?.Codigo ?? 0);
                    }
                }
            }

            if (controle.FormasAlerta.Contains(ControleAlertaForma.PainelAlerta))
                alerta.FormasAlerta.Add(ControleAlertaForma.PainelAlerta);

            repAlerta.Inserir(alerta);
        }

        #endregion
    }
}