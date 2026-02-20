using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 10000)]

    public class Fatura : LongRunningProcessBase<Fatura>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            VerificarCargaFatura(_stringConexao, _stringConexaoAdmin, _clienteMultisoftware.Codigo, _tipoServicoMultisoftware, unitOfWork);
            await IntegrarFaturasPendentesAsync(unitOfWork, cancellationToken);
            ConsultarRetornoFaturasPendentes(unitOfWork, _tipoServicoMultisoftware);

            VerificarFaturaLoteConfirmarDocumento(_stringConexao, _stringConexaoAdmin, _clienteMultisoftware.Codigo, _tipoServicoMultisoftware, unitOfWork);
            VerificarFaturaLoteConfirmarFechamento(_stringConexao, _stringConexaoAdmin, _clienteMultisoftware.Codigo, _tipoServicoMultisoftware, unitOfWork);
        }

        private void VerificarFaturaLoteConfirmarFechamento(string stringConexao, string adminStringConexao, int clienteCodigo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(adminStringConexao);

            Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);
            Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
            Repositorio.Embarcador.Fatura.FaturaCarga repFaturaCarga = new Repositorio.Embarcador.Fatura.FaturaCarga(unitOfWork);
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            AdminMultisoftware.Repositorio.Pessoas.Cliente repCliente = new AdminMultisoftware.Repositorio.Pessoas.Cliente(adminUnitOfWork);
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente = repCliente.BuscarPorCodigo(clienteCodigo);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            List<int> listaFaturas = repFatura.BuscarCodigosPorEtapaSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.EmAntamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura.Fechamento, true, true);

            try
            {
                for (int i = 0; i < listaFaturas.Count; i++)
                {
                    unitOfWork.FlushAndClear();

                    Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(listaFaturas[i]);

                    unitOfWork.Start();

                    fatura.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.EmFechamento;
                    fatura.DataFechamento = DateTime.Now;
                    fatura.Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura.Fechamento;

                    if (fatura.Numero == 0)
                    {
                        if (configuracaoTMS.GerarNumeracaoFaturaAnual)
                        {
                            int anoAtual = DateTime.Now.Year;
                            fatura.Numero = repFatura.UltimoNumeracao(anoAtual) + 1;
                            anoAtual = (anoAtual % 100);
                            if (fatura.Numero == 0 || (fatura.Numero < ((anoAtual * 1000000) + 1)))
                                fatura.Numero = (anoAtual * 1000000) + 1;
                        }
                        else
                            fatura.Numero = repFatura.UltimoNumeracao() + 1;
                        fatura.ControleNumeracao = null;
                    }

                    if (fatura.Carga == null && configuracaoTMS.UtilizaEmissaoMultimodal)
                    {
                        Dominio.Entidades.Embarcador.Cargas.Carga primeiraCarga = repFaturaDocumento.BuscarPrimeiraCarga(fatura.Codigo);
                        if (primeiraCarga != null && primeiraCarga.CargaTakeOrPay)
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiraPorCarga(primeiraCarga.Codigo);
                            fatura.Carga = primeiraCarga;
                            fatura.ImprimeObservacaoFatura = true;

                            if (cargaPedido?.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.DemurrageCabotagem || cargaPedido?.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.DetentionCabotagem)
                            {
                                fatura.ObservacaoFatura += "Navio/Viagem/Direção: " + (fatura.Carga.PedidoViagemNavio?.Descricao ?? "") + " \n";
                                fatura.ObservacaoFatura += "Porto de Origem: " + (fatura.Carga.PortoOrigem?.Descricao ?? "") + "\n";
                                fatura.ObservacaoFatura += "Porto de Destino: " + (fatura.Carga.PortoDestino?.Descricao ?? "") + "\n";
                                fatura.ObservacaoFatura += "Tipo Proposta: " + (cargaPedido?.TipoPropostaMultimodal.ObterDescricao()) + "\n" + "\n";
                            }
                            else if (cargaPedido?.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.FaturamentoContabilidade)
                            {
                                int qtdDisponibilizadas = repCargaPedido.BuscarQuantidadeDisponibilizadas(fatura.Carga.Codigo);
                                int qtdNaoEmbarcadas = repCargaPedido.BuscarQuantidadeNaoEmbarcadas(fatura.Carga.Codigo);

                                fatura.ObservacaoFatura += "Fatura de Penalidade Contratual \n\n";
                                fatura.ObservacaoFatura += "Navio/Viagem/Direção: " + (fatura.Carga.PedidoViagemNavio?.Descricao ?? "") + " \n";
                                fatura.ObservacaoFatura += "Porto de Origem: " + (fatura.Carga.PortoOrigem?.Descricao ?? "") + "\n";
                                fatura.ObservacaoFatura += "Porto de Destino: " + (fatura.Carga.PortoDestino?.Descricao ?? "") + "\n";
                                fatura.ObservacaoFatura += "Quantidade de unidades disponibilizadas: " + (Utilidades.String.OnlyNumbers(qtdDisponibilizadas.ToString("n0"))) + " \n";
                                fatura.ObservacaoFatura += "Quantidade de unidades não embarcadas: " + (Utilidades.String.OnlyNumbers(qtdNaoEmbarcadas.ToString("n0"))) + "\n";
                            }
                            else
                                fatura.FaturaPropostaFaturamento = true;

                            if (!string.IsNullOrWhiteSpace(primeiraCarga.ObservacaoParaFaturamento))
                            {
                                if (!string.IsNullOrWhiteSpace(fatura.ObservacaoFatura))
                                    fatura.ObservacaoFatura += "\n\n";
                                fatura.ObservacaoFatura += primeiraCarga.ObservacaoParaFaturamento;
                            }
                        }
                    }

                    repFatura.Atualizar(fatura);

                    servFatura.InserirLog(fatura, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura.FechouFatura, fatura.Usuario);
                    servFatura.GerarIntegracoesFatura(fatura, unitOfWork, tipoServicoMultisoftware, null, configuracaoTMS);
                    servFatura.SalvarTituloVencimentoDocumentoFaturamento(fatura, unitOfWork);

                    unitOfWork.CommitChanges();
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                throw;
            }
            finally
            {
                adminUnitOfWork.Dispose();
            }
        }

        private void VerificarFaturaLoteConfirmarDocumento(string stringConexao, string adminStringConexao, int clienteCodigo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            //Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(adminStringConexao);

            Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);
            Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
            Repositorio.Embarcador.Fatura.FaturaCarga repFaturaCarga = new Repositorio.Embarcador.Fatura.FaturaCarga(unitOfWork);
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            AdminMultisoftware.Repositorio.Pessoas.Cliente repCliente = new AdminMultisoftware.Repositorio.Pessoas.Cliente(adminUnitOfWork);
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente = repCliente.BuscarPorCodigo(clienteCodigo);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            List<int> listaFaturas = repFatura.BuscarCodigosPorEtapaSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.EmAntamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura.Documentos, true, false);

            try
            {
                for (int i = 0; i < listaFaturas.Count; i++)
                {

                    unitOfWork.FlushAndClear();

                    Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(listaFaturas[i]);

                    unitOfWork.Start();

                    fatura.Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura.Fechamento;
                    fatura.ImprimeObservacaoFatura = false;

                    if (fatura.Numero == 0)
                    {
                        if (configuracaoTMS.GerarNumeracaoFaturaAnual)
                        {
                            int anoAtual = DateTime.Now.Year;
                            fatura.Numero = repFatura.UltimoNumeracao(anoAtual) + 1;
                            anoAtual = (anoAtual % 100);
                            if (fatura.Numero == 0 || (fatura.Numero < ((anoAtual * 1000000) + 1)))
                                fatura.Numero = (anoAtual * 1000000) + 1;
                        }
                        else
                            fatura.Numero = repFatura.UltimoNumeracao() + 1;
                        fatura.ControleNumeracao = null;
                    }

                    if (fatura.Empresa == null)
                        fatura.Empresa = repFaturaDocumento.ObterPrimeiraEmpresaEmissora(fatura.Codigo);

                    if (configuracaoTMS.UtilizarDadosBancariosDaEmpresa && fatura.Empresa != null && fatura.Empresa.Banco != null)
                    {
                        fatura.Banco = fatura.Empresa.Banco;
                        fatura.Agencia = fatura.Empresa.Agencia;
                        fatura.DigitoAgencia = fatura.Empresa.DigitoAgencia;
                        fatura.NumeroConta = fatura.Empresa.NumeroConta;
                        fatura.TipoContaBanco = fatura.Empresa.TipoContaBanco;
                    }

                    if (fatura.Carga?.TipoOperacao?.UsarConfiguracaoFaturaPorTipoOperacao ?? false)
                    {
                        if (fatura.Banco == null)
                        {
                            fatura.Banco = fatura.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.Banco;
                            fatura.Agencia = fatura.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.Agencia;
                            fatura.DigitoAgencia = fatura.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.DigitoAgencia;
                            fatura.NumeroConta = fatura.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.NumeroConta;
                            fatura.TipoContaBanco = fatura.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.TipoContaBanco;
                        }
                        fatura.ClienteTomadorFatura = fatura.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.ClienteTomadorFatura;
                        fatura.ObservacaoFatura += fatura.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.ObservacaoFatura ?? "";
                    }
                    else if (fatura.Cliente != null && !fatura.Cliente.NaoUsarConfiguracaoFaturaGrupo)
                    {
                        if (fatura.Cliente.GrupoPessoas != null)
                        {
                            if (fatura.Banco == null)
                            {
                                fatura.Banco = fatura.Cliente.GrupoPessoas.Banco;
                                fatura.Agencia = fatura.Cliente.GrupoPessoas.Agencia;
                                fatura.DigitoAgencia = fatura.Cliente.GrupoPessoas.DigitoAgencia;
                                fatura.NumeroConta = fatura.Cliente.GrupoPessoas.NumeroConta;
                                fatura.TipoContaBanco = fatura.Cliente.GrupoPessoas.TipoContaBanco;
                            }
                            fatura.ClienteTomadorFatura = fatura.Cliente.GrupoPessoas.ClienteTomadorFatura;
                            fatura.ObservacaoFatura += fatura.Cliente.GrupoPessoas.ObservacaoFatura;
                        }
                        else
                        {
                            if (fatura.Banco == null)
                            {
                                fatura.Banco = fatura.Cliente.Banco;
                                fatura.Agencia = fatura.Cliente.Agencia;
                                fatura.DigitoAgencia = fatura.Cliente.DigitoAgencia;
                                fatura.NumeroConta = fatura.Cliente.NumeroConta;
                                fatura.TipoContaBanco = fatura.Cliente.TipoContaBanco;
                            }
                            fatura.ClienteTomadorFatura = fatura.Cliente.ClienteTomadorFatura;
                            fatura.ObservacaoFatura += fatura.Cliente.ObservacaoFatura;
                        }
                    }
                    else if (fatura.Cliente != null)
                    {
                        if (fatura.Banco == null)
                        {
                            fatura.Banco = fatura.Cliente.Banco;
                            fatura.Agencia = fatura.Cliente.Agencia;
                            fatura.DigitoAgencia = fatura.Cliente.DigitoAgencia;
                            fatura.NumeroConta = fatura.Cliente.NumeroConta;
                            fatura.TipoContaBanco = fatura.Cliente.TipoContaBanco;
                        }
                        fatura.ClienteTomadorFatura = fatura.Cliente.ClienteTomadorFatura;
                        fatura.ObservacaoFatura += fatura.Cliente.ObservacaoFatura;
                    }
                    else if (fatura.GrupoPessoas != null)
                    {
                        if (fatura.Banco == null)
                        {
                            fatura.Banco = fatura.GrupoPessoas.Banco;
                            fatura.Agencia = fatura.GrupoPessoas.Agencia;
                            fatura.DigitoAgencia = fatura.GrupoPessoas.DigitoAgencia;
                            fatura.NumeroConta = fatura.GrupoPessoas.NumeroConta;
                            fatura.TipoContaBanco = fatura.GrupoPessoas.TipoContaBanco;
                        }
                        fatura.ClienteTomadorFatura = fatura.GrupoPessoas.ClienteTomadorFatura;
                        fatura.ObservacaoFatura += fatura.GrupoPessoas.ObservacaoFatura;
                    }
                    if (!string.IsNullOrWhiteSpace(fatura.ObservacaoFatura))
                        fatura.ImprimeObservacaoFatura = true;

                    if (fatura.Carga == null && configuracaoTMS.UtilizaEmissaoMultimodal)
                    {
                        Dominio.Entidades.Embarcador.Cargas.Carga primeiraCarga = repFaturaDocumento.BuscarPrimeiraCarga(fatura.Codigo);
                        if (primeiraCarga != null && primeiraCarga.CargaTakeOrPay)
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiraPorCarga(primeiraCarga.Codigo);
                            fatura.Carga = primeiraCarga;
                            fatura.ImprimeObservacaoFatura = true;

                            if (cargaPedido?.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.DemurrageCabotagem || cargaPedido?.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.DetentionCabotagem)
                            {
                                fatura.ObservacaoFatura += "Navio/Viagem/Direção: " + (fatura.Carga.PedidoViagemNavio?.Descricao ?? "") + " \n";
                                fatura.ObservacaoFatura += "Porto de Origem: " + (fatura.Carga.PortoOrigem?.Descricao ?? "") + "\n";
                                fatura.ObservacaoFatura += "Porto de Destino: " + (fatura.Carga.PortoDestino?.Descricao ?? "") + "\n";
                                fatura.ObservacaoFatura += "Tipo Proposta: " + (cargaPedido?.TipoPropostaMultimodal.ObterDescricao()) + "\n" + "\n";
                            }
                            else if (cargaPedido?.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.FaturamentoContabilidade)
                            {
                                int qtdDisponibilizadas = repCargaPedido.BuscarQuantidadeDisponibilizadas(fatura.Carga.Codigo);
                                int qtdNaoEmbarcadas = repCargaPedido.BuscarQuantidadeNaoEmbarcadas(fatura.Carga.Codigo);

                                fatura.ObservacaoFatura += "Fatura de Penalidade Contratual \n\n";
                                fatura.ObservacaoFatura += "Navio/Viagem/Direção: " + (fatura.Carga.PedidoViagemNavio?.Descricao ?? "") + " \n";
                                fatura.ObservacaoFatura += "Porto de Origem: " + (fatura.Carga.PortoOrigem?.Descricao ?? "") + "\n";
                                fatura.ObservacaoFatura += "Porto de Destino: " + (fatura.Carga.PortoDestino?.Descricao ?? "") + "\n";
                                fatura.ObservacaoFatura += "Quantidade de unidades disponibilizadas: " + (Utilidades.String.OnlyNumbers(qtdDisponibilizadas.ToString("n0"))) + " \n";
                                fatura.ObservacaoFatura += "Quantidade de unidades não embarcadas: " + (Utilidades.String.OnlyNumbers(qtdNaoEmbarcadas.ToString("n0"))) + "\n";
                            }
                            else
                                fatura.FaturaPropostaFaturamento = true;

                            if (!string.IsNullOrWhiteSpace(primeiraCarga.ObservacaoParaFaturamento))
                            {
                                if (!string.IsNullOrWhiteSpace(fatura.ObservacaoFatura))
                                    fatura.ObservacaoFatura += "\n\n";
                                fatura.ObservacaoFatura += primeiraCarga.ObservacaoParaFaturamento;
                            }
                        }
                    }

                    if (fatura.ClienteTomadorFatura == null)
                        fatura.ClienteTomadorFatura = repFaturaDocumento.ObterPrimeiroTomador(fatura.Codigo);

                    repFatura.Atualizar(fatura);

                    servFatura.InserirLog(fatura, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura.SalvouCargas, fatura.Usuario);
                    //if (configuracaoTMS.UtilizaMoedaEstrangeira)
                    //    servFatura.LancarMoedaEstrangeira(fatura, unitOfWork);
                    servFatura.LancarParcelaFatura(fatura, unitOfWork, configuracaoTMS.UtilizaEmissaoMultimodal);

                    unitOfWork.CommitChanges();
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                throw;
            }
            finally
            {
                adminUnitOfWork.Dispose();
            }
        }

        public static void VerificarCargaFatura(string stringConexao, string adminStringConexao, int clienteCodigo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(adminStringConexao);

            Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);
            Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
            Repositorio.Embarcador.Fatura.FaturaCarga repFaturaCarga = new Repositorio.Embarcador.Fatura.FaturaCarga(unitOfWork);
            AdminMultisoftware.Repositorio.Pessoas.Cliente repCliente = new AdminMultisoftware.Repositorio.Pessoas.Cliente(adminUnitOfWork);
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente = repCliente.BuscarPorCodigo(clienteCodigo);

            List<int> listaFaturas = repFatura.BuscarCodigosPorEtapa(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura.LancandoCargas);

            try
            {
                for (int i = 0; i < listaFaturas.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(listaFaturas[i]);

                    if (fatura.NovoModelo)
                    {
                        servFatura.InserirDocumentosFatura(fatura, cliente, tipoServicoMultisoftware, adminStringConexao, fatura.Usuario, unitOfWork);
                    }
                    else
                    {
                        if (servFatura.InserirCargaFatura(fatura, stringConexao, cliente, tipoServicoMultisoftware, adminStringConexao, fatura.Usuario, unitOfWork))
                        {
                            unitOfWork.FlushAndClear();

                            fatura = repFatura.BuscarPorCodigo(listaFaturas[i]);

                            unitOfWork.Start();

                            fatura.Total = repFaturaCarga.ValorConhecimentos(fatura.Codigo, 0);
                            fatura.Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura.Fatura;

                            repFatura.Atualizar(fatura);

                            unitOfWork.CommitChanges();
                        }
                    }
                }
            }
            catch
            {
                unitOfWork.Rollback();
                throw;
            }
            finally
            {
                adminUnitOfWork.Dispose();
            }
        }

        private void AjustarRenvio(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao integracaoPendente)
        {
            if (integracaoPendente.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado && integracaoPendente.ReenviarAutomaticamenteOutraVezAposMinutos > 0)
            {
                integracaoPendente.DataEnvio = integracaoPendente.DataEnvio.Value.AddMinutes(integracaoPendente.ReenviarAutomaticamenteOutraVezAposMinutos);
                integracaoPendente.ReenviarAutomaticamenteOutraVezAposMinutos = 0;
                integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
            }
        }

        private async Task IntegrarFaturasPendentesAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            double minutosACadaTentativa = 10;
            int numeroRegistrosPorVez = 5;

            try
            {
                Servicos.Embarcador.Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork, cancellationToken);

                Repositorio.Embarcador.Fatura.FaturaIntegracao repositorioFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(unitOfWork, cancellationToken);

                List<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao> integracoesPendentes = await repositorioFaturaIntegracao.BuscarIntegracoesPendentesEnvioAsync(minutosACadaTentativa, "Codigo", "asc", numeroRegistrosPorVez, _tipoServicoMultisoftware, cancellationToken);

                foreach (Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao integracaoPendente in integracoesPendentes)
                {
                    switch (integracaoPendente.TipoIntegracao.Tipo)
                    {
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avon:
                            Servicos.Embarcador.Integracao.Avon.IntegracaoFaturaAvon.EnviarFatura(integracaoPendente, unitOfWork);
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura:
                            Servicos.Embarcador.Integracao.Natura.IntegracaoFaturaNatura.EnviarFatura(integracaoPendente, unitOfWork);
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP:
                            await new Servicos.Embarcador.Integracao.FTP.IntegracaoFTP(unitOfWork, _tipoServicoMultisoftware, cancellationToken).EnviarEDIAsync(integracaoPendente);
                            AjustarRenvio(integracaoPendente);
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.PortalCabotagem:
                            new Servicos.Embarcador.Integracao.PortalCabotagem.IntegracaoPortalCabotagem(unitOfWork).Integrar(integracaoPendente.CodigosCTes.FirstOrDefault(), false);
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email:
                            Servicos.Embarcador.Integracao.Email.IntegracaoEmail.EnviarEDI(integracaoPendente, unitOfWork);
                            AjustarRenvio(integracaoPendente);
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Protheus:
                            new Servicos.Embarcador.Integracao.Protheus.IntegracaoProtheus(unitOfWork).IntegrarFatura(integracaoPendente);
                            AjustarRenvio(integracaoPendente);
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Intercab:
                            new Servicos.Embarcador.Integracao.Intercab.IntegracaoIntercab(unitOfWork).IntegrarFatura(integracaoPendente);
                            AjustarRenvio(integracaoPendente);
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Marilan:
                            new Servicos.Embarcador.Integracao.Marilan.IntegracaoMarilan(unitOfWork).IntegrarFatura(integracaoPendente);
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao:
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada:
                            integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                            servicoCargaDadosSumarizados.AtualizarDadosCTesFaturadosIntegrados(integracaoPendente.Fatura.Codigo, unitOfWork);
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.EMP:
                            new Servicos.Embarcador.Integracao.EMP.IntegracaoEMP(unitOfWork).IntegrarFatura(integracaoPendente, _clienteUrlAcesso?.URLAcesso ?? "");
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NFTP:
                            new Servicos.Embarcador.Integracao.NFTP.IntegracaoNFTP(unitOfWork).IntegrarFatura(integracaoPendente, _clienteUrlAcesso?.URLAcesso ?? "");
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP:
                            new Servicos.Embarcador.Integracao.SAP.IntegracaoSAP(unitOfWork).IntegrarFatura(integracaoPendente);
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_ESTORNO_FATURA:
                            new Servicos.Embarcador.Integracao.SAP.IntegracaoSAP(unitOfWork).IntegrarEstornoFatura(integracaoPendente);
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.LoggiFaturas:
                            new Servicos.Embarcador.Integracao.LoggiFaturas.IntegracaoLoggiFaturas(unitOfWork).IntegrarFatura(integracaoPendente);
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Calisto:
                            new Servicos.Embarcador.Integracao.Calisto.IntegracaoCalisto(unitOfWork).IntegrarFatura(integracaoPendente);
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Globus:
                            new Servicos.Embarcador.Integracao.Globus.IntegracaoGlobus(unitOfWork).IntegrarFatura(integracaoPendente);
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Efesus:
                            new Servicos.Embarcador.Integracao.Efesus.IntegracaoEfesus(unitOfWork).IntegrarFatura(integracaoPendente);
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Olfar:
                            new Servicos.Embarcador.Integracao.Olfar.IntegracaoOlfar(unitOfWork).IntegrarFatura(integracaoPendente);
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ItalacFaturas:
                            new Servicos.Embarcador.Integracao.Italac.IntegracaoItalac(unitOfWork).IntegrarFatura(integracaoPendente);
                            break;
                        default:
                            integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                            integracaoPendente.MensagemRetorno = "Integração não implementada";
                            break;
                    }

                    await repositorioFaturaIntegracao.AtualizarAsync(integracaoPendente);
                }


                List<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao> integracoesPendentesEmailFatura = await repositorioFaturaIntegracao.BuscarIntegracoesPendentesEnvioEmailAsync(minutosACadaTentativa, "Codigo", "asc", numeroRegistrosPorVez, cancellationToken);
                foreach (Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao integracaoPendenteEmail in integracoesPendentesEmailFatura)
                {
                    Servicos.Embarcador.Integracao.Email.IntegracaoEmail.EnviarFatura(integracaoPendenteEmail, unitOfWork, unitOfWork.StringConexao, _tipoServicoMultisoftware);
                    await repositorioFaturaIntegracao.AtualizarAsync(integracaoPendenteEmail);
                }

            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
            }
        }

        private void ConsultarRetornoFaturasPendentes(Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            int numeroRegistrosPorVez = 3;

            try
            {
                Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(unidadeTrabalho);

                List<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao> integracoesPendentes = repFaturaIntegracao.BuscarIntegracoesPendentesRetorno("Codigo", "asc", numeroRegistrosPorVez, tipoServicoMultisoftware);

                foreach (Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao integracaoPendente in integracoesPendentes)
                {
                    switch (integracaoPendente.TipoIntegracao.Tipo)
                    {
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avon:
                            Servicos.Embarcador.Integracao.Avon.IntegracaoFaturaAvon.ConsultarRetornoFatura(integracaoPendente, unidadeTrabalho);
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP:
                            new Servicos.Embarcador.Integracao.SAP.IntegracaoSAP(unidadeTrabalho).IntegrarFatura(integracaoPendente);
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_ESTORNO_FATURA:
                            new Servicos.Embarcador.Integracao.SAP.IntegracaoSAP(unidadeTrabalho).IntegrarEstornoFatura(integracaoPendente);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
            }
        }
    }
}