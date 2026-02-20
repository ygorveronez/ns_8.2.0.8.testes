using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Frota
{
    [CustomAuthorize("Frota/MovimentacaoPneu")]
    public class MovimentacaoPneuController : BaseController
    {
		#region Construtores

		public MovimentacaoPneuController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarEstoque()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaPneu filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaPneu()
                {
                    CodigoAlmoxarifado = Request.GetIntParam("Codigo"),
                    Situacao = SituacaoPneu.Disponivel,
                    NumeroFogo = Request.GetStringParam("NumeroFogo")
                };
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                {
                    DirecaoOrdenar = "asc",
                    InicioRegistros = Request.GetIntParam("Inicio"),
                    LimiteRegistros = Request.GetIntParam("Limite"),
                    PropriedadeOrdenar = "NumeroFogo"
                };
                Repositorio.Embarcador.Frota.Pneu repositorio = new Repositorio.Embarcador.Frota.Pneu(unitOfWork);
                int quantidadeRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Frota.Pneu> listaPneu = (quantidadeRegistros > 0) ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Frota.Pneu>();

                dynamic listaRetornar = (
                    from pneu in listaPneu
                    select new
                    {
                        CodigoPneu = pneu.Codigo,
                        Almoxarifado = new { pneu.Almoxarifado.Codigo, pneu.Almoxarifado.Descricao },
                        BandaRodagem = new { pneu.BandaRodagem.Codigo, pneu.BandaRodagem.Descricao },
                        pneu.NumeroFogo,
                        Vida = pneu.VidaAtual.ObterDescricao(),
                        pneu.KmAtualRodado,
                        Marca = pneu.Modelo.Marca.Descricao,
                        pneu.Sulco,
                        Modelo = new { Codigo = pneu.Modelo?.Codigo ?? 0, Descricao = pneu.Modelo?.Descricao ?? "" },
                        DataMovimentacao = pneu.DataMovimentacaoEstoque.HasValue && pneu.DataMovimentacaoEstoque.Value > DateTime.MinValue ? pneu.DataMovimentacaoEstoque.Value.ToString("dd/MM/yyyy HH:mm") : pneu.DataEntrada > DateTime.MinValue ? pneu.DataEntrada.ToString("dd/MM/yyyy HH:mm") : ""
                    }
                ).ToList();

                return new JsonpResult(new
                {
                    Pneus = listaRetornar,
                    QuantidadeRegistros = quantidadeRegistros
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar o estoque.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarReformaPorCodigo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaPneu filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaPneu()
                {
                    Codigo = Request.GetIntParam("Codigo"),
                    Situacao = SituacaoPneu.Reforma,
                    NumeroFogo = Request.GetStringParam("NumeroFogo")
                };
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                {
                    DirecaoOrdenar = "asc",
                    InicioRegistros = Request.GetIntParam("Inicio"),
                    LimiteRegistros = Request.GetIntParam("Limite"),
                    PropriedadeOrdenar = "NumeroFogo"
                };
                Repositorio.Embarcador.Frota.Pneu repositorio = new Repositorio.Embarcador.Frota.Pneu(unitOfWork);
                int quantidadeRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Frota.Pneu> listaPneu = (quantidadeRegistros > 0) ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Frota.Pneu>();

                dynamic listaRetornar = (
                    from pneu in listaPneu
                    select new
                    {
                        CodigoPneu = pneu.Codigo,
                        Almoxarifado = new { pneu.Almoxarifado.Codigo, pneu.Almoxarifado.Descricao },
                        BandaRodagem = new { pneu.BandaRodagem.Codigo, pneu.BandaRodagem.Descricao },
                        ValorMaoObra = pneu?.OrdemServicoFrota?.Orcamento?.ValorTotalMaoObra ?? 0,
                        ValorProdutos = pneu?.OrdemServicoFrota?.Orcamento?.ValorTotalProdutos ?? 0,
                        NumeroOS = pneu?.OrdemServicoFrota?.Numero,
                        LocalManutencao = new { Codigo = pneu?.OrdemServicoFrota?.LocalManutencao?.Codigo ?? 0, Descricao = pneu?.OrdemServicoFrota?.LocalManutencao?.Descricao ?? ""},
                        pneu.NumeroFogo,
                        Vida = pneu.VidaAtual.ObterDescricao(),
                        pneu.KmAtualRodado,
                        Marca = pneu.Modelo.Marca.Descricao ?? "",
                        pneu.Sulco,  
                        OrdemServico = pneu.OrdensServico,
                        Modelo = new { Codigo = pneu.Modelo?.Codigo ?? 0, Descricao = pneu.Modelo?.Descricao ?? "" },
                        DataMovimentacao = pneu.DataMovimentacaoReforma.HasValue && pneu.DataMovimentacaoReforma.Value > DateTime.MinValue ? pneu.DataMovimentacaoReforma.Value.ToString("dd/MM/yyyy HH:mm") : ""
                    }
                ).ToList();

                return new JsonpResult(new
                {
                    Pneus = listaRetornar,
                    QuantidadeRegistros = quantidadeRegistros
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar os pneus em reforma.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarVeiculoPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                string numeroFogo = Request.GetStringParam("NumeroFogo");
                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                Dominio.Entidades.Veiculo veiculo = null;

                if (codigo > 0)
                    veiculo = repositorioVeiculo.BuscarPorCodigo(codigo);
                else if (!string.IsNullOrWhiteSpace(numeroFogo))
                    veiculo = repositorioVeiculo.BuscarPorNumeroFogo(numeroFogo);

                if (veiculo == null)
                    return new JsonpResult(false, true, "Não foi localizado o veículo com esses filtros");

                return new JsonpResult(new
                {
                    veiculo.Codigo,
                    DadosVeiculo = ObterDadosVeiculo(veiculo),
                    Eixos = ObterEixos(veiculo),
                    Estepes = ObterEstepes(veiculo)
                });
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar o veículo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarParaEstoque()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Frota.Pneu pneu = ObterPneu(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.Almoxarifado almoxarifado = ObterAlmoxarifado(unitOfWork, Request.GetIntParam("Almoxarifado"));
                Dominio.Entidades.Embarcador.Frota.Almoxarifado almoxarifadoOrigem = ObterAlmoxarifado(unitOfWork, Request.GetIntParam("CodigoAlmoxarifadoOrigem"));
                bool atualizarDadosSulcos = Request.GetBoolParam("UtilizarDadosAdicionais");

                AdicionarMovimentacaoParaVeiculoSaida(unitOfWork, pneu);
                AdicionarHistoricoTrocaAlmoxarifado(unitOfWork, pneu, almoxarifado, almoxarifadoOrigem);

                Repositorio.Embarcador.Frota.Pneu repositorioPneu = new Repositorio.Embarcador.Frota.Pneu(unitOfWork);

                pneu.Initialize();
                pneu.Situacao = SituacaoPneu.Disponivel;
                pneu.Almoxarifado = almoxarifado;
                pneu.DataMovimentacaoEstoque = Request.GetDateTimeParam("DataHora");

                if (atualizarDadosSulcos)
                {
                    pneu.SulcoAnterior = pneu.Sulco;
                    pneu.Sulco = Request.GetDecimalParam("SulcoAtual");
                }

                AtualizarKMPneuVeiculo(unitOfWork, pneu, atualizarKmRodadoEntreSulcos: atualizarDadosSulcos, true);

                repositorioPneu.Atualizar(pneu, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar para o estoque.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarParaReforma()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Frota.Pneu pneu = ObterPneu(unitOfWork);

                AdicionarMovimentacaoParaVeiculoSaida(unitOfWork, pneu);

                Dominio.Entidades.Embarcador.Frota.PneuEnvioReforma envioReforma = AdicionarEnvioReforma(unitOfWork, pneu);
                Repositorio.Embarcador.Frota.Pneu repositorioPneu = new Repositorio.Embarcador.Frota.Pneu(unitOfWork);
                Repositorio.Embarcador.Frota.OrdemServicoFrotaTipo repositorioTipoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaTipo(unitOfWork);

                long codigoTipoOrdemServico = Request.GetLongParam("TipoOrdemServico");
                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaTipo tipoOrdemServico = codigoTipoOrdemServico > 0L ? repositorioTipoOrdemServico.BuscarPorCodigo(codigoTipoOrdemServico, false) : null;

                pneu.Initialize();
                pneu.Situacao = SituacaoPneu.Reforma;
                pneu.SulcoAnterior = pneu.Sulco;
                pneu.Sulco = envioReforma.SulcoAtual;
                pneu.DataMovimentacaoReforma = Request.GetDateTimeParam("DataHora");
                envioReforma.TipoOrdemServico = tipoOrdemServico;

                AtualizarKMPneuVeiculo(unitOfWork, pneu, true, true);
                InserirOrdemServico(unitOfWork, pneu, envioReforma);

                repositorioPneu.Atualizar(pneu, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar para a reforma.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarParaSucata()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Frota/MovimentacaoPneu");

                if (!Usuario.UsuarioAdministrador && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.MovimentacaoPneu_PermitirSucatearPneu))
                    return new JsonpResult(false, true, "Usuário sem permissão para enviar Pneu para Sucata");

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Frota.Pneu pneu = ObterPneu(unitOfWork);
                pneu.Initialize();

                AdicionarMovimentacaoParaVeiculoSaida(unitOfWork, pneu);
                AdicionarEnvioSucata(unitOfWork, pneu);
                BaixarEstoqueProduto(unitOfWork, pneu, TipoServicoMultisoftware);

                Repositorio.Embarcador.Frota.Pneu repositorioPneu = new Repositorio.Embarcador.Frota.Pneu(unitOfWork);

                pneu.Situacao = SituacaoPneu.Sucata;

                AtualizarKMPneuVeiculo(unitOfWork, pneu, atualizarKmRodadoEntreSulcos: false, true);

                repositorioPneu.Atualizar(pneu, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();

                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao enviar para sucata.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarReformaParaEstoque()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Frota.Pneu pneu = ObterPneu(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.PneuRetornoReforma retornoReforma = AdicionarRetornoReforma(unitOfWork, pneu);

                List<Dominio.Entidades.Embarcador.Frota.PneuRetornoReformaProduto> listaProdutos = AdicionarRetornoReformaProdutos(unitOfWork, retornoReforma);
                AdicionarHistoricoTrocaAlmoxarifado(unitOfWork, pneu, retornoReforma.Almoxarifado, pneu.Almoxarifado);

                string retorno = FinalizarOrdemServico(unitOfWork, pneu, listaProdutos);
                if (!string.IsNullOrWhiteSpace(retorno))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, retorno);
                }

                Repositorio.Embarcador.Frota.Pneu repositorioPneu = new Repositorio.Embarcador.Frota.Pneu(unitOfWork);

                pneu.Initialize();
                pneu.Almoxarifado = retornoReforma.Almoxarifado;
                pneu.BandaRodagem = retornoReforma.BandaRodagem;
                pneu.KmRodadoEntreSulcos = 0;
                pneu.Situacao = SituacaoPneu.Disponivel;
                pneu.SulcoAnterior = pneu.Sulco;
                pneu.Sulco = retornoReforma.SulcoAtual;
                pneu.VidaAtual = retornoReforma.Vida;
                pneu.DataMovimentacaoEstoque = Request.GetDateTimeParam("DataHora");

                repositorioPneu.Atualizar(pneu, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar para o estoque.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> MovimentarPneuParaVeiculo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Frota.Pneu pneu = ObterPneu(unitOfWork);

                TratarRegrasConfigGeralVeiculo(unitOfWork);

                AdicionarMovimentacaoParaVeiculoSaida(unitOfWork, pneu);
                AdicionarMovimentacaoParaVeiculoEntrada(unitOfWork, pneu);
                AdicionarMovimentacaoViradaPneuVeiculo(unitOfWork, pneu);

                Repositorio.Embarcador.Frota.Pneu repositorioPneu = new Repositorio.Embarcador.Frota.Pneu(unitOfWork);

                pneu.Initialize();
                pneu.Situacao = SituacaoPneu.EmUso;
                pneu.SulcoAnterior = pneu.Sulco;
                pneu.Sulco = Request.GetDecimalParam("SulcoAtual");
                pneu.Calibragem = Request.GetIntParam("Calibragem");
                pneu.Milimitragem1 = Request.GetDecimalParam("Milimitragem1");
                pneu.Milimitragem2 = Request.GetDecimalParam("Milimitragem2");
                pneu.Milimitragem3 = Request.GetDecimalParam("Milimitragem3");
                pneu.Milimitragem4 = Request.GetDecimalParam("Milimitragem4");
                pneu.NumeroFogo = Request.GetStringParam("NumeroFogo");

                AtualizarKMPneuVeiculo(unitOfWork, pneu, false, false);

                repositorioPneu.Atualizar(pneu, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar a movimentação de pneu do veículo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void TratarRegrasConfigGeralVeiculo(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo repConfigVeiculo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo(unitOfWork);
            var configVeiculo = repConfigVeiculo.BuscarConfiguracaoPadrao();

            if (configVeiculo.NaoPermitirVincularPneuVeiculoAbastecimentoAberto)
            {
                VerificarAbastecimentoEmAberto(unitOfWork);
            }
        }

        private bool VerificarAbastecimentoEmAberto(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Veiculo veiculo = ObterVeiculo(unitOfWork, Request.GetIntParam("CodigoVeiculo"));
            Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);

            return repAbastecimento.VeiculoPossuiAbastecimentoEmAberto(veiculo.Codigo) ?
                throw new ControllerException("Não é possível adicionar um Pneu novo neste Veículo, pois o mesmo possui abastecimento(s) em Aberto")
                : false;
        }

        public async Task<IActionResult> LiberarVeiculoManutencao()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Veiculos.SituacaoVeiculo repSituacaoVeiculo = new Repositorio.Embarcador.Veiculos.SituacaoVeiculo(unitOfWork);
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);

                Dominio.Entidades.Veiculo veiculo = null;

                if (codigo > 0)
                    veiculo = repositorioVeiculo.BuscarPorCodigo(codigo);

                if (veiculo == null)
                    return new JsonpResult(false, "Veículo não localizado");

                veiculo.Initialize();
                veiculo.SituacaoVeiculo = SituacaoVeiculo.Disponivel;
                veiculo.DataHoraPrevisaoDisponivel = null;
                veiculo.VeiculoVazio = true;
                veiculo.LocalidadeAtual = null;

                repositorioVeiculo.Atualizar(veiculo, Auditado);

                Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo situacao = new Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo();
                Dominio.Entidades.Usuario veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);

                situacao.DataHoraEmissao = DateTime.Now;
                situacao.DataHoraSituacao = DateTime.Now;
                situacao.Usuario = this.Usuario;
                situacao.Localidade = null;
                situacao.Veiculo = veiculo;
                situacao.DataHoraSaidaManutencao = DateTime.Now;
                situacao.Situacao = SituacaoVeiculo.EmManutencao;
                situacao.Motorista = veiculoMotorista;

                repSituacaoVeiculo.Inserir(situacao);

                Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServicoFrota = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota> ordemServicos = repOrdemServicoFrota.BuscarPorVeiculo(veiculo.Codigo, SituacaoOrdemServicoFrota.EmManutencao);
                if (ordemServicos != null && ordemServicos.Count > 0)
                {
                    foreach (var ordemServico in ordemServicos)
                    {
                        ordemServico.Situacao = SituacaoOrdemServicoFrota.AgNotaFiscal;
                        repOrdemServicoFrota.Atualizar(ordemServico);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, ordemServico, null, "Ordem de Serviço liberada pela movimentação de Pneu", unitOfWork);
                    }
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, situacao, null, "Veículo liberado pela movimentação de pneu", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, null, "Veículo liberado pela movimentação de pneu", unitOfWork);

                return new JsonpResult(true, true, "Sucesso");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar o veículo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ObterConfiguracaoImportacao();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Repositorio.Embarcador.Frota.Pneu repositorioPneu = new Repositorio.Embarcador.Frota.Pneu(unitOfWork);

            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ObterConfiguracaoImportacao();
                string dados = Request.Params("Dados");
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
                retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();
                int contador = 0;

                for (int i = 0; i < linhas.Count; i++)
                {
                    try
                    {
                        unitOfWork.FlushAndClear();
                        unitOfWork.Start();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];
                        string retorno = "";

                        Dominio.Entidades.Embarcador.Frota.Pneu existePneu = null;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaNumeroFogo = (from obj in linha.Colunas where obj.NomeCampo == "NumeroFogo" select obj).FirstOrDefault();
                        if (colunaNumeroFogo == null)
                            throw new ControllerException("É obrigatório informar número de fogo.");

                        var numeroFogoSemEspacos = colunaNumeroFogo.Valor.Trim();
                        existePneu = repositorioPneu.BuscarPorNumeroFogo(numeroFogoSemEspacos);

                        if (existePneu == null)
                            throw new ControllerException("Não foi localizado pneu pelo Número de Fogo.");

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaSulcoAtual = (from obj in linha.Colunas where obj.NomeCampo == "SulcoAtual" select obj).FirstOrDefault();
                        decimal sulcoAtual = 0;
                        if (!string.IsNullOrWhiteSpace((string)colunaSulcoAtual?.Valor) && colunaSulcoAtual?.Valor != null)
                        {
                            string strValor = (string)colunaSulcoAtual.Valor;
                            decimal.TryParse(strValor, out sulcoAtual);
                        }

                        existePneu.Sulco = sulcoAtual;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaCalibragem = (from obj in linha.Colunas where obj.NomeCampo == "Calibragem" select obj).FirstOrDefault();
                        int calibragem = 0;
                        if (!string.IsNullOrWhiteSpace((string)colunaCalibragem?.Valor) && colunaCalibragem?.Valor != null )
                        {
                            string strValor = (string)colunaCalibragem.Valor;
                            int.TryParse(strValor, out calibragem);
                        }

                        existePneu.Calibragem = calibragem;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaMilimitragem1 = (from obj in linha.Colunas where obj.NomeCampo == "Milimitragem1" select obj).FirstOrDefault();
                        decimal milimigratem1 = 0;
                        if (!string.IsNullOrWhiteSpace((string)colunaMilimitragem1?.Valor) && colunaMilimitragem1?.Valor != null)
                        {
                            string strValor = (string)colunaMilimitragem1.Valor;
                            decimal.TryParse(strValor, out milimigratem1);
                        }

                        existePneu.Milimitragem1 = milimigratem1;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaMilimitragem2 = (from obj in linha.Colunas where obj.NomeCampo == "Milimitragem2" select obj).FirstOrDefault();
                        decimal milimigratem2 = 0;
                        if (!string.IsNullOrWhiteSpace((string)colunaMilimitragem2?.Valor) && colunaMilimitragem2?.Valor != null)
                        {
                            string strValor = (string)colunaMilimitragem2.Valor;
                            decimal.TryParse(strValor, out milimigratem2);
                        }

                        existePneu.Milimitragem2 = milimigratem2;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaMilimitragem3 = (from obj in linha.Colunas where obj.NomeCampo == "Milimitragem3" select obj).FirstOrDefault();
                        decimal milimigratem3 = 0;
                        if (!string.IsNullOrWhiteSpace((string)colunaMilimitragem3?.Valor) && colunaMilimitragem3?.Valor != null)
                        {
                            string strValor = (string)colunaMilimitragem3.Valor;
                            decimal.TryParse(strValor, out milimigratem3);
                        }

                        existePneu.Milimitragem3 = milimigratem3;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaMilimitragem4 = (from obj in linha.Colunas where obj.NomeCampo == "Milimitragem4" select obj).FirstOrDefault();
                        decimal milimigratem4 = 0;
                        if (!string.IsNullOrWhiteSpace((string)colunaMilimitragem4?.Valor) && colunaMilimitragem4?.Valor != null)
                        {
                            string strValor = (string)colunaMilimitragem4.Valor;
                            decimal.TryParse(strValor, out milimigratem4);
                        }

                        existePneu.Milimitragem4 = milimigratem4;

                        if (!string.IsNullOrWhiteSpace(retorno))
                        {
                            unitOfWork.Rollback();
                            retornoImportacao.Retornolinhas.Add(Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha.CriarRetornoFalha(retorno, i));
                        }
                        else
                        {
                            contador++;
                            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha.CriarRetornoSucesso(i, contar: false);
                            retornoImportacao.Retornolinhas.Add(retornoLinha);

                            repositorioPneu.Atualizar(existePneu);

                            AdicionarMovimentacaoPneuVeiculoDadosAdicionais(unitOfWork);

                            unitOfWork.CommitChanges();
                        }
                    }

                    catch (ControllerException ex2)
                    {
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro(ex2);
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(ex2.Message, i));
                    }
                }

                retornoImportacao.MensagemAviso = "";
                retornoImportacao.Total = linhas.Count();
                retornoImportacao.Importados = contador;

                return new JsonpResult(retornoImportacao);
            }
          
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao importar planilha.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> MovimentarPneusRodizio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoPneu_1 = Request.GetIntParam("CodigoPneu_1");
                int codigoPneu_2 = Request.GetIntParam("CodigoPneu_2");
                Repositorio.Embarcador.Frota.Pneu repositorioPneu = new Repositorio.Embarcador.Frota.Pneu(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.Pneu pneu1 = repositorioPneu.BuscarPorCodigo(codigoPneu_1);
                Dominio.Entidades.Embarcador.Frota.Pneu pneu2 = repositorioPneu.BuscarPorCodigo(codigoPneu_2);

                if (pneu1 == null || pneu2 == null)
                    throw new ControllerException("Um ou ambos os pneus não foram encontrados.");

                RealizarRodizioPneus(unitOfWork);

                pneu1.Initialize();
                pneu1.Situacao = SituacaoPneu.EmUso;
                pneu1.SulcoAnterior = pneu1.Sulco;
                pneu1.Sulco = Request.GetNullableDecimalParam("SulcoAtual_1") ?? 0;
                pneu1.Calibragem = Request.GetNullableIntParam("Calibragem_1") ?? 0;
                pneu1.Milimitragem1 = Request.GetNullableDecimalParam("MilimetragemUm_1") ?? 0;
                pneu1.Milimitragem2 = Request.GetNullableDecimalParam("MilimetragemDois_1") ?? 0;
                pneu1.Milimitragem3 = Request.GetNullableDecimalParam("MilimetragemTres_1") ?? 0;
                pneu1.Milimitragem4 = Request.GetNullableDecimalParam("MilimetragemQuatro_1") ?? 0;
                pneu1.NumeroFogo = Request.GetStringParam("NumeroFogo_1");

                pneu2.Initialize();
                pneu2.Situacao = SituacaoPneu.EmUso;
                pneu2.SulcoAnterior = pneu2.Sulco;
                pneu2.Sulco = Request.GetNullableDecimalParam("SulcoAtual_2") ?? 0;
                pneu2.Calibragem = Request.GetNullableIntParam("Calibragem_2") ?? 0;
                pneu2.Milimitragem1 = Request.GetNullableDecimalParam("MilimetragemUm_2") ?? 0;
                pneu2.Milimitragem2 = Request.GetNullableDecimalParam("MilimetragemDois_2") ?? 0;
                pneu2.Milimitragem3 = Request.GetNullableDecimalParam("MilimetragemTres_2") ?? 0;
                pneu2.Milimitragem4 = Request.GetNullableDecimalParam("MilimetragemQuatro_2") ?? 0;
                pneu2.NumeroFogo = Request.GetStringParam("NumeroFogo_2");

                AtualizarKMPneusRodizio(unitOfWork, pneu1, pneu2);

                repositorioPneu.Atualizar(pneu1, Auditado);
                repositorioPneu.Atualizar(pneu2, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true,"Rodízio realizado com sucesso!");
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o rodízio de pneus.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        //private void AtualizarKMPneuVeiculo(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Frota.Pneu pneu)
        //{
        //    AtualizarKMPneuVeiculo(unitOfWork, pneu, atualizarKmRodadoEntreSulcos: true);
        //}

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, mensagemFalha = mensagem, processou = false };
            return retorno;
        }

        private void AtualizarKMPneusRodizio(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Frota.Pneu pneu1, Dominio.Entidades.Embarcador.Frota.Pneu pneu2)
        {
            int codigoEixoPneuOrigem_1 = Request.GetIntParam("CodigoEixoPneuOrigem_1");
            int codigoEstepeOrigem_1 = Request.GetIntParam("CodigoEstepeOrigem_1");
            int codigoEixoPneuOrigem_2 = Request.GetIntParam("CodigoEixoPneuOrigem_2");
            int codigoEstepeOrigem_2 = Request.GetIntParam("CodigoEstepeOrigem_2");

            bool pneu1EstavaEmEixo = codigoEixoPneuOrigem_1 > 0;
            bool pneu2EstavaEmEixo = codigoEixoPneuOrigem_2 > 0;

            if (!pneu1EstavaEmEixo && !pneu2EstavaEmEixo)
                return;

            int codigoVeiculo = Request.GetIntParam("CodigoVeiculo");
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
            Dominio.Entidades.Veiculo veiculo = ObterVeiculo(unitOfWork, codigoVeiculo);

            if (veiculo == null)
                throw new ControllerException("Veículo não encontrado para atualizar o KM dos pneus.");

            int hodometro = Request.GetNullableIntParam("Hodometro") ?? 0;

            if (hodometro > veiculo.KilometragemAtual)
            {
                int kmRodado = hodometro - veiculo.KilometragemAtual;

                if (pneu1EstavaEmEixo)
                {
                    pneu1.KmAnteriorRodado = pneu1.KmAtualRodado;
                    pneu1.KmAtualRodado += kmRodado;

                    if (pneu1.ValorCustoAtualizado > 0 && pneu1.KmAtualRodado > 0)
                        pneu1.ValorCustoKmAtualizado = pneu1.ValorCustoAtualizado / pneu1.KmAtualRodado;
                }

                if (pneu2EstavaEmEixo)
                {
                    pneu2.KmAnteriorRodado = pneu2.KmAtualRodado;
                    pneu2.KmAtualRodado += kmRodado;

                    if (pneu2.ValorCustoAtualizado > 0 && pneu2.KmAtualRodado > 0)
                        pneu2.ValorCustoKmAtualizado = pneu2.ValorCustoAtualizado / pneu2.KmAtualRodado;
                }

                if (!ConfiguracaoEmbarcador.MovimentarKMApenasPelaGuarita)
                    veiculo.KilometragemAtual = hodometro;

                repositorioVeiculo.Atualizar(veiculo, Auditado, null, "Movimentou KM por rodízio através da movimentação de pneus");
            }
        }

        private void AtualizarKMPneuVeiculo(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Frota.Pneu pneu, bool atualizarKmRodadoEntreSulcos, bool atualizarKmPneu)
        {
            int hodometro = Request.GetIntParam("Hodometro");
            int codigoVeiculo = Request.GetIntParam("CodigoVeiculo");
            int codigoEixoPneu = Request.GetIntParam("CodigoEixoPneu");
            int codigoEixoPneuDestino = Request.GetIntParam("CodigoEixoPneuDestino");
            int codigoEixoPneuOrigem = Request.GetIntParam("CodigoEixoPneuOrigem");
            int codigoEstepeDestino = Request.GetIntParam("CodigoEstepeDestino");
            int codigoEstepeOrigem = Request.GetIntParam("CodigoEstepeOrigem");

            if (hodometro > 0 && codigoVeiculo > 0 && (codigoEixoPneu > 0 || codigoEixoPneuOrigem > 0 || codigoEstepeOrigem > 0 || codigoEixoPneuDestino > 0 || codigoEstepeDestino > 0))
            {
                if (codigoEstepeOrigem > 0 || codigoEstepeDestino > 0)
                    return;

                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                Dominio.Entidades.Veiculo veiculo = ObterVeiculo(unitOfWork, codigoVeiculo);

                if (veiculo != null)
                    veiculo.Initialize();

                if ((veiculo.KilometragemAnterior == 0) || (veiculo.KilometragemAtual != hodometro))
                    veiculo.KilometragemAnterior = veiculo.KilometragemAtual;

                bool atualizarKMPneuIgual = veiculo.KilometragemAtual != hodometro;

                if (veiculo.KilometragemAnterior < hodometro)
                {
                    int kmRodado = (hodometro - veiculo.KilometragemAnterior);

                    if (pneu.KmAnteriorRodado == (pneu.KmAtualRodado - kmRodado) || ((pneu.KmAtualRodado - kmRodado) < 0))
                    {
                        if (!atualizarKMPneuIgual)
                            return;
                    }

                    if (atualizarKmPneu)
                    {
                        pneu.KmAnteriorRodado = pneu.KmAtualRodado;
                        pneu.KmAtualRodado = pneu.KmAtualRodado + kmRodado;

                        if (atualizarKmRodadoEntreSulcos)
                            pneu.KmRodadoEntreSulcos = kmRodado;

                        if ((pneu.ValorCustoAtualizado > 0) && (pneu.KmAtualRodado > 0))
                            pneu.ValorCustoKmAtualizado = pneu.ValorCustoAtualizado / pneu.KmAtualRodado;
                    }

                    if (!ConfiguracaoEmbarcador.MovimentarKMApenasPelaGuarita)
                        veiculo.KilometragemAtual = hodometro;

                    repositorioVeiculo.Atualizar(veiculo, Auditado, null, "Movimentou KM através da movimentação de pneus");
                }
            }
        }

        private Dominio.Entidades.Embarcador.Frota.Almoxarifado ObterAlmoxarifado(Repositorio.UnitOfWork unitOfWork, int codigoAlmoxarifado)
        {
            Repositorio.Embarcador.Frota.Almoxarifado repositorioAlmoxarifado = new Repositorio.Embarcador.Frota.Almoxarifado(unitOfWork);

            return repositorioAlmoxarifado.BuscarPorCodigo(codigoAlmoxarifado) ?? throw new ControllerException("Almoxarifado não encontrado.");
        }

        private dynamic ObterDadosVeiculo(Dominio.Entidades.Veiculo veiculo)
        {
            return new
            {
                Hodometro = veiculo.KilometragemAtual,
                PlacaVeiculo = veiculo.Placa,
                FrotaVeiculo = veiculo.NumeroFrota,
                veiculo.SituacaoVeiculo
            };
        }

        private List<dynamic> ObterEixos(Dominio.Entidades.Veiculo veiculo)
        {
            if (veiculo.ModeloVeicularCarga == null)
                throw new ControllerException("O veículo não possui modelo veicular de carga");

            if ((veiculo.ModeloVeicularCarga.Eixos == null) || (veiculo.ModeloVeicularCarga.Eixos.Count == 0))
                throw new ControllerException("O modelo veicular de carga do veículo não possui eixos cadastrados");

            List<dynamic> eixos = new List<dynamic>();

            foreach (Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEixo eixo in veiculo.ModeloVeicularCarga.Eixos.OrderBy(o => o.Numero))
            {
                List<dynamic> eixosPneu = new List<dynamic>();

                foreach (Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEixoPneu eixoPneu in eixo.Pneus.OrderBy(o => o.ObterPosicaoPneu()))
                {
                    var pneuVeiculo = (from pneu in veiculo.Pneus where pneu.EixoPneu.Codigo == eixoPneu.Codigo select pneu.Pneu).FirstOrDefault();
                    var veiculoPneu = (from pneu in veiculo.Pneus where pneu.EixoPneu.Codigo == eixoPneu.Codigo select pneu).FirstOrDefault();

                    if (pneuVeiculo != null)
                    {
                        eixosPneu.Add(new
                        {
                            eixoPneu.Codigo,
                            PosicaoLadoDireito = eixoPneu.Posicao.IsLadoDireito(),
                            PosicaoPneu = eixoPneu.ObterPosicaoPneu(),
                            CodigoPneu = pneuVeiculo.Codigo,
                            Almoxarifado = new { pneuVeiculo.Almoxarifado.Codigo, pneuVeiculo.Almoxarifado.Descricao },
                            BandaRodagem = new { pneuVeiculo.BandaRodagem.Codigo, pneuVeiculo.BandaRodagem.Descricao },
                            pneuVeiculo.NumeroFogo,
                            Vida = pneuVeiculo.VidaAtual.ObterDescricao(),
                            pneuVeiculo.KmAtualRodado,
                            Marca = pneuVeiculo.Modelo.Marca.Descricao,
                            pneuVeiculo.Sulco,
                            Modelo = new { Codigo = pneuVeiculo.Modelo?.Codigo ?? 0, Descricao = pneuVeiculo.Modelo?.Descricao ?? "" },
                            DataMovimentacao = veiculoPneu.DataMovimentacaoPneu.HasValue && veiculoPneu.DataMovimentacaoPneu.Value > DateTime.MinValue ? veiculoPneu.DataMovimentacaoPneu.Value.ToString("dd/MM/yyyy HH:mm") : veiculoPneu.DataMovimentacaoPneu.HasValue && veiculoPneu.DataMovimentacao.Value > DateTime.MinValue ? veiculoPneu.DataMovimentacao.Value.ToString("dd/MM/yyyy") : ""
                        });
                    }
                    else
                    {
                        eixosPneu.Add(new
                        {
                            eixoPneu.Codigo,
                            PosicaoLadoDireito = eixoPneu.Posicao.IsLadoDireito(),
                            PosicaoPneu = eixoPneu.ObterPosicaoPneu(),
                            CodigoPneu = 0,
                            Almoxarifado = new { Codigo = 0, Descricao = "" },
                            BandaRodagem = new { Codigo = 0, Descricao = "" },
                            NumeroFogo = "",
                            Vida = "",
                            KmAtualRodado = "",
                            Marca = "",
                            Sulco = ""
                        });
                    }
                }

                eixos.Add(new
                {
                    eixo.Codigo,
                    NomeImagem = eixo.QuantidadePneu == QuantidadePneuEixo.Duplo ? "img/Eixos/EixoDuploSemPneu.png" : "img/Eixos/EixoSimplesSemPneu.png",
                    eixo.Numero,
                    Pneus = eixosPneu
                });
            }

            return eixos;
        }

        private List<dynamic> ObterEstepes(Dominio.Entidades.Veiculo veiculo)
        {
            List<dynamic> estepes = new List<dynamic>();

            if ((veiculo.ModeloVeicularCarga?.Estepes == null) || (veiculo.ModeloVeicularCarga.Estepes.Count == 0))
                return estepes;

            foreach (Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEstepe estepe in veiculo.ModeloVeicularCarga.Estepes.OrderBy(o => o.Numero))
            {
                var pneuEstepeVeiculo = (from estepeVeiculo in veiculo.Estepes where estepeVeiculo.Estepe.Codigo == estepe.Codigo select estepeVeiculo.Pneu).FirstOrDefault();
                var veiculoStepe = (from estepeVeiculo in veiculo.Estepes where estepeVeiculo.Estepe.Codigo == estepe.Codigo select estepeVeiculo).FirstOrDefault();

                if (pneuEstepeVeiculo != null)
                {
                    estepes.Add(new
                    {
                        estepe.Codigo,
                        CodigoPneu = pneuEstepeVeiculo.Codigo,
                        Almoxarifado = new { pneuEstepeVeiculo.Almoxarifado.Codigo, pneuEstepeVeiculo.Almoxarifado.Descricao },
                        BandaRodagem = new { pneuEstepeVeiculo.BandaRodagem.Codigo, pneuEstepeVeiculo.BandaRodagem.Descricao },
                        estepe.Numero,
                        pneuEstepeVeiculo.NumeroFogo,
                        Vida = pneuEstepeVeiculo.VidaAtual.ObterDescricao(),
                        pneuEstepeVeiculo.KmAtualRodado,
                        Marca = pneuEstepeVeiculo.Modelo.Marca.Descricao,
                        pneuEstepeVeiculo.Sulco,
                        Modelo = new { Codigo = pneuEstepeVeiculo.Modelo?.Codigo ?? 0, Descricao = pneuEstepeVeiculo.Modelo?.Descricao ?? "" },
                        DataMovimentacao = veiculoStepe.DataMovimentacaoPneu.HasValue && veiculoStepe.DataMovimentacaoPneu.Value > DateTime.MinValue ? veiculoStepe.DataMovimentacaoPneu.Value.ToString("dd/MM/yyyy HH:mm") : veiculoStepe.DataMovimentacao.HasValue && veiculoStepe.DataMovimentacao.Value > DateTime.MinValue ? veiculoStepe.DataMovimentacao.Value.ToString("dd/MM/yyyy") : ""
                    });
                }
                else
                {
                    estepes.Add(new
                    {
                        estepe.Codigo,
                        CodigoPneu = 0,
                        Almoxarifado = new { Codigo = 0, Descricao = "" },
                        BandaRodagem = new { Codigo = 0, Descricao = "" },
                        estepe.Numero,
                        NumeroFogo = "",
                        Vida = "",
                        KmAtualRodado = "",
                        Marca = "",
                        Sulco = ""
                    });
                }
            }

            return estepes;
        }

        private Dominio.Entidades.Embarcador.Frota.Pneu ObterPneu(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoPneu = Request.GetIntParam("CodigoPneu");
            Repositorio.Embarcador.Frota.Pneu repositorioPneu = new Repositorio.Embarcador.Frota.Pneu(unitOfWork);

            return repositorioPneu.BuscarPorCodigo(codigoPneu) ?? throw new ControllerException("Pneu não encontrado.");
        }

        private Dominio.Entidades.Veiculo ObterVeiculo(Repositorio.UnitOfWork unitOfWork, int codigoVeiculo)
        {
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);

            return repositorioVeiculo.BuscarPorCodigo(codigoVeiculo) ?? throw new ControllerException("Veículo não encontrado.");
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ObterConfiguracaoImportacao()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Número de Fogo", Propriedade = "NumeroFogo", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Sulco Atual", Propriedade = "SulcoAtual", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "Calibragem", Propriedade = "Calibragem", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "Milimitragem 1", Propriedade = "Milimitragem1", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "Milimitragem 2", Propriedade = "Milimitragem2", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = "Milimitragem 3", Propriedade = "Milimitragem3", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = "Milimitragem 4", Propriedade = "Milimitragem4", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });

            return configuracoes;
        }

        #endregion

        #region Métodos Privados de Envio para Reforma

        private Dominio.Entidades.Embarcador.Frota.PneuEnvioReforma AdicionarEnvioReforma(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Frota.Pneu pneu)
        {
            Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota servico = ObterServicoVeiculoFrota(unitOfWork);
            Repositorio.Embarcador.Frota.PneuHistorico repositorioPneuHistorico = new Repositorio.Embarcador.Frota.PneuHistorico(unitOfWork);
            Repositorio.Embarcador.Frota.OrdemServicoFrotaTipo repositorioTipoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaTipo(unitOfWork);

            Dominio.Entidades.Embarcador.Frota.PneuHistorico pneuHistorico = new Dominio.Entidades.Embarcador.Frota.PneuHistorico()
            {
                Data = DateTime.Now,
                Descricao = $"Enviado para reforma",
                Pneu = pneu,
                Tipo = TipoPneuHistorico.EnvioReforma,
                Servicos = servico.Descricao,
                BandaRodagem = pneu.BandaRodagem,
                KmAtualRodado = pneu.KmAtualRodado,
                CustoEstimado = Request.GetDecimalParam("CustoEstimado"),
                DataHoraMovimentacao = Request.GetDateTimeParam("DataHora"),
                Usuario = this.Usuario,
            };

            repositorioPneuHistorico.Inserir(pneuHistorico);

            long codigoTipoOrdemServico = Request.GetLongParam("TipoOrdemServico");
            Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaTipo tipoOrdemServico = codigoTipoOrdemServico > 0L ? repositorioTipoOrdemServico.BuscarPorCodigo(codigoTipoOrdemServico, false) : null;

            Repositorio.Embarcador.Frota.PneuEnvioReforma repositorioPneuEnvioReforma = new Repositorio.Embarcador.Frota.PneuEnvioReforma(unitOfWork);
            Dominio.Entidades.Embarcador.Frota.PneuEnvioReforma pneuEnvioReforma = new Dominio.Entidades.Embarcador.Frota.PneuEnvioReforma()
            {
                CustoEstimado = Request.GetDecimalParam("CustoEstimado"),
                Data = Request.GetDateTimeParam("DataHora"),
                DataCadastro = DateTime.Now,
                Fornecedor = ObterFornecedor(unitOfWork),
                Hodometro = Request.GetIntParam("Hodometro"),
                Observacao = Request.GetNullableStringParam("Observacao"),
                Pneu = pneu,
                PneuHistorico = pneuHistorico,
                ServicoVeiculo = servico,
                SulcoAnterior = Request.GetDecimalParam("SulcoAnterior"),
                SulcoAtual = Request.GetDecimalParam("SulcoAtual"),
                TipoOrdemServico = tipoOrdemServico
            };

            repositorioPneuEnvioReforma.Inserir(pneuEnvioReforma);

            return pneuEnvioReforma;
        }

        private void InserirOrdemServico(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Frota.Pneu pneu, Dominio.Entidades.Embarcador.Frota.PneuEnvioReforma envioReforma)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
            Repositorio.Embarcador.Frota.OrdemServicoFrotaTipo repositorioTipoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaTipo(unitOfWork);

            Dominio.Entidades.Veiculo veiculo = null;
            Dominio.Entidades.Cliente fornecedor = null;

            int codigoVeiculo = Request.GetIntParam("CodigoVeiculo");
            double cnpjFornecedor = Request.GetDoubleParam("Fornecedor");
            int kmMovimentacao = Request.GetIntParam("Hodometro");
            string observacao = Request.GetStringParam("Observacao");
            decimal custoEstimado = Request.GetDecimalParam("CustoEstimado");
            bool origemPneuDoEstoque = Request.GetBoolParam("OrigemPneuDoEstoque");
            long codigoTipoOrdemServico = Request.GetLongParam("TipoOrdemServico");

            Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaTipo tipoOrdemServico = codigoTipoOrdemServico > 0L ? repositorioTipoOrdemServico.BuscarPorCodigo(codigoTipoOrdemServico, false) : null;

            if (codigoVeiculo > 0 && !origemPneuDoEstoque)
                veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);
            if (cnpjFornecedor > 0)
                fornecedor = repCliente.BuscarPorCPFCNPJ(cnpjFornecedor);

            Dominio.ObjetosDeValor.Embarcador.Frota.ObjetoOrdemServico objetoOrdemServico = new Dominio.ObjetosDeValor.Embarcador.Frota.ObjetoOrdemServico()
            {
                DataProgramada = DateTime.Now.Date,
                LocalManutencao = fornecedor,
                Motorista = veiculo != null ? repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo) : null,
                Observacao = "GERADO A PARTIR DO CONSERTO DO PNEU " + pneu.NumeroFogo + (!string.IsNullOrWhiteSpace(observacao) ? " " + observacao : ""),
                Operador = Usuario,
                Veiculo = veiculo,
                QuilometragemVeiculo = kmMovimentacao > 0 ? kmMovimentacao : veiculo?.KilometragemAtual ?? 0,
                Pneu = pneu,
                Empresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa : null,
                PneuEnvioReforma = envioReforma,
                Custo = custoEstimado,
                ServicoVeiculo = ObterServicoVeiculoFrota(unitOfWork),
                TipoOrdemServico = tipoOrdemServico,
            };

            Servicos.Embarcador.Frota.OrdemServico.GerarOrdemServicoAteEtapaDeFechamento(objetoOrdemServico, Auditado, unitOfWork);
        }

        private Dominio.Entidades.Cliente ObterFornecedor(Repositorio.UnitOfWork unitOfWork)
        {
            double cpfCnpjFornecedor = Request.GetDoubleParam("Fornecedor");

            if (cpfCnpjFornecedor == 0d)
                return null;

            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);

            return repositorioCliente.BuscarPorCPFCNPJ(cpfCnpjFornecedor) ?? throw new ControllerException("Fornecedor não encontrado.");
        }

        private Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota ObterServicoVeiculoFrota(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoServicoVeiculo = Request.GetIntParam("ServicoVeiculo");
            Repositorio.Embarcador.Frota.ServicoVeiculoFrota repositorioServicoVeiculoFrota = new Repositorio.Embarcador.Frota.ServicoVeiculoFrota(unitOfWork);

            return repositorioServicoVeiculoFrota.BuscarPorCodigo(codigoServicoVeiculo) ?? throw new ControllerException("Serviço do veículo não encontrado.");
        }

        #endregion

        #region Métodos Privados de Envio para Sucata

        private void AdicionarEnvioSucata(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Frota.Pneu pneu)
        {
            Dominio.Entidades.Embarcador.Frota.MotivoSucateamentoPneu motivo = ObterMotivoSucateamentoPneu(unitOfWork);
            Repositorio.Embarcador.Frota.PneuHistorico repositorioPneuHistorico = new Repositorio.Embarcador.Frota.PneuHistorico(unitOfWork);
            Dominio.Entidades.Embarcador.Frota.PneuHistorico pneuHistorico = new Dominio.Entidades.Embarcador.Frota.PneuHistorico()
            {
                Data = DateTime.Now,
                Descricao = $"Enviado para sucata. Motivo: {motivo.Descricao}",
                Pneu = pneu,
                Tipo = TipoPneuHistorico.EnvioSucata,
                BandaRodagem = pneu.BandaRodagem,
                KmAtualRodado = pneu.KmAtualRodado,
                DataHoraMovimentacao = Request.GetDateTimeParam("DataHora"),
                Usuario = this.Usuario,
            };

            repositorioPneuHistorico.Inserir(pneuHistorico);

            Repositorio.Embarcador.Frota.PneuSucata repositorioPneuSucata = new Repositorio.Embarcador.Frota.PneuSucata(unitOfWork);
            Dominio.Entidades.Embarcador.Frota.PneuSucata pneuSucata = new Dominio.Entidades.Embarcador.Frota.PneuSucata()
            {
                Data = Request.GetDateTimeParam("DataHora"),
                DataCadastro = DateTime.Now,
                Motivo = motivo,
                Observacao = Request.GetNullableStringParam("Observacao"),
                Pneu = pneu,
                PneuHistorico = pneuHistorico,
                ValorCarcaca = Request.GetDecimalParam("ValorCarcaca")
            };

            repositorioPneuSucata.Inserir(pneuSucata);

            if (motivo != null && motivo.TipoMovimento != null)
            {
                Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_conexao.StringConexao);
                servProcessoMovimento.GerarMovimentacao(out string erro, motivo.TipoMovimento, DateTime.Now.Date, pneuSucata.ValorCarcaca, pneu.NumeroFogo, "ENVIO PARA SUCATA DO PNEU " + pneu.NumeroFogo + " " + pneuSucata.Observacao, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Manual, TipoServicoMultisoftware);
            }
        }

        private void BaixarEstoqueProduto(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Frota.Pneu pneu, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (pneu.Produto != null && pneu.DocumentoEntradaItem != null && pneu.DocumentoEntradaItem.NaturezaOperacao != null && pneu.DocumentoEntradaItem.NaturezaOperacao.ControlaEstoque)
            {
                Servicos.Embarcador.Produto.Estoque servicoEstoque = new Servicos.Embarcador.Produto.Estoque(unitOfWork);

                servicoEstoque.MovimentarEstoque(out string erro, pneu.Produto, 1, Dominio.Enumeradores.TipoMovimento.Saida, "PNEU", pneu.NumeroFogo, pneu.ValorCustoAtualizado, pneu.Empresa, DateTime.Now, TipoServicoMultisoftware);
            }
        }

        private Dominio.Entidades.Embarcador.Frota.MotivoSucateamentoPneu ObterMotivoSucateamentoPneu(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoMotivo = Request.GetIntParam("Motivo");
            Repositorio.Embarcador.Frota.MotivoSucateamentoPneu repositorioMotivoSucateamentoPneu = new Repositorio.Embarcador.Frota.MotivoSucateamentoPneu(unitOfWork);

            return repositorioMotivoSucateamentoPneu.BuscarPorCodigo(codigoMotivo) ?? throw new ControllerException("Motivo de sucateamento do pneu não encontrado.");
        }

        #endregion

        #region Métodos Privados de Envio Reforma para Estoque

        private Dominio.Entidades.Embarcador.Frota.PneuRetornoReforma AdicionarRetornoReforma(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Frota.Pneu pneu)
        {
            Repositorio.Embarcador.Frota.PneuHistorico repositorioPneuHistorico = new Repositorio.Embarcador.Frota.PneuHistorico(unitOfWork);
            Dominio.Entidades.Embarcador.Frota.PneuHistorico pneuHistorico = new Dominio.Entidades.Embarcador.Frota.PneuHistorico()
            {
                Data = DateTime.Now,
                Descricao = $"Retornado da reforma",
                Pneu = pneu,
                Tipo = TipoPneuHistorico.RetornoReforma,
                BandaRodagem = pneu.BandaRodagem,
                KmAtualRodado = pneu.KmAtualRodado,
                DataHoraMovimentacao = Request.GetDateTimeParam("DataHora"),
                Usuario = this.Usuario,
            };

            repositorioPneuHistorico.Inserir(pneuHistorico);

            Repositorio.Embarcador.Frota.PneuRetornoReforma repositorioPneuRetornoReforma = new Repositorio.Embarcador.Frota.PneuRetornoReforma(unitOfWork);
            Dominio.Entidades.Embarcador.Frota.PneuRetornoReforma pneuRetornoReforma = new Dominio.Entidades.Embarcador.Frota.PneuRetornoReforma()
            {
                Almoxarifado = ObterAlmoxarifado(unitOfWork, Request.GetIntParam("Almoxarifado")),
                BandaRodagem = ObterBandaRodagemPneu(unitOfWork),
                Data = Request.GetDateTimeParam("DataHora"),
                DataCadastro = DateTime.Now,
                Observacao = Request.GetNullableStringParam("Observacao"),
                Pneu = pneu,
                PneuHistorico = pneuHistorico,
                ResponsavelOrcamento = Request.GetNullableStringParam("ResponsavelOrcamento"),
                ServicoRealizado = Request.GetEnumParam<ServicoRealizadoPneu>("ServicoRealizado"),
                SulcoAnterior = Request.GetDecimalParam("SulcoAnterior"),
                SulcoAtual = Request.GetDecimalParam("SulcoAtual"),
                ValorMaoObra = Request.GetDecimalParam("ValorMaoObra"),
                ValorProdutos = Request.GetDecimalParam("ValorProdutos"),
                ValorResidualAtualPneu = Request.GetDecimalParam("ValorResidualAtualPneu"),
                Vida = Request.GetEnumParam<VidaPneu>("Vida")
            };

            if (pneuRetornoReforma.BandaRodagem == null)
                pneuRetornoReforma.BandaRodagem = pneu.BandaRodagem;

            repositorioPneuRetornoReforma.Inserir(pneuRetornoReforma);

            return pneuRetornoReforma;
        }

        private List<Dominio.Entidades.Embarcador.Frota.PneuRetornoReformaProduto> AdicionarRetornoReformaProdutos(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Frota.PneuRetornoReforma retornoReforma)
        {
            List<Dominio.Entidades.Embarcador.Frota.PneuRetornoReformaProduto> retorno = new List<Dominio.Entidades.Embarcador.Frota.PneuRetornoReformaProduto>();
            dynamic produtos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Produtos"));
            Repositorio.Embarcador.Frota.PneuRetornoReformaProduto repositorioPneuRetornoReformaProduto = new Repositorio.Embarcador.Frota.PneuRetornoReformaProduto(unitOfWork);


            foreach (dynamic produto in produtos)
            {
                Dominio.Entidades.Embarcador.Frota.PneuRetornoReformaProduto pneuRetornoReformaProduto = new Dominio.Entidades.Embarcador.Frota.PneuRetornoReformaProduto()
                {
                    PneuRetornoReforma = retornoReforma,
                    Produto = ObterProduto(unitOfWork, ((string)produto.CodigoProduto).ToInt()),
                    FinalidadeProduto = ObterFinalidadeProdutoOrdemServico(unitOfWork, ((string)produto.CodigoFinalidadeProduto).ToInt()),
                    Quantidade = ((string)produto.Quantidade).ToDecimal(),
                    Valor = ((string)produto.Valor).ToDecimal()
                };

                repositorioPneuRetornoReformaProduto.Inserir(pneuRetornoReformaProduto);
                retorno.Add(pneuRetornoReformaProduto);
            }
            return retorno;
        }

        private string FinalizarOrdemServico(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Frota.Pneu pneu, List<Dominio.Entidades.Embarcador.Frota.PneuRetornoReformaProduto> produtos)
        {
            string erro = string.Empty;
            Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unitOfWork);
            Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo repServicoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo(unitOfWork);
            Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamento repOrdemServicoFrotaOrcamento = new Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamento(unitOfWork);
            Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico repOrdemServicoFrotaOrcamentoServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico(unitOfWork);
            Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto repOrdemServicoFrotaFechamentoProduto = new Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto(unitOfWork);

            Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico = repOrdemServico.BuscarOSAbertaPorPneu(pneu.Codigo);
            if (ordemServico != null)
            {
                string responsavelOrcamento = Request.GetStringParam("ResponsavelOrcamento");
                decimal valorMaoObra = 0, valorProdutos = 0;
                valorMaoObra = Request.GetDecimalParam("ValorMaoObra");
                valorProdutos = Request.GetDecimalParam("ValorProdutos");

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo ultimoRealizado = null;
                if (ordemServico.PneuEnvioReforma != null && ordemServico.PneuEnvioReforma.ServicoVeiculo != null && ordemServico.Veiculo != null)
                    ultimoRealizado = repServicoOrdemServico.BuscarUltimoRealizado(ordemServico.PneuEnvioReforma.ServicoVeiculo.Codigo, ordemServico.Veiculo?.Codigo ?? 0, 0);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo ordemServicoFrotaServicoVeiculo = repServicoOrdemServico.BuscarPorOrdemServicoEServico(ordemServico.Codigo, ordemServico.PneuEnvioReforma.ServicoVeiculo.Codigo);
                if (ordemServicoFrotaServicoVeiculo == null)
                {
                    ordemServicoFrotaServicoVeiculo = new Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo()
                    {
                        CustoEstimado = ordemServico.PneuEnvioReforma.CustoEstimado,
                        CustoMedio = ordemServico.PneuEnvioReforma.CustoEstimado,
                        Observacao = "SERVIÇO VINCULADO COM PNEU " + pneu.NumeroFogo,
                        OrdemServico = ordemServico,
                        Servico = ordemServico.PneuEnvioReforma.ServicoVeiculo,
                        TipoManutencao = ordemServico.PneuEnvioReforma.ServicoVeiculo.Motivo == MotivoServicoVeiculo.Reforma || ordemServico.PneuEnvioReforma.ServicoVeiculo.Motivo == MotivoServicoVeiculo.ConsertoEReforma ? TipoManutencaoServicoVeiculoOrdemServicoFrota.Preventiva : TipoManutencaoServicoVeiculoOrdemServicoFrota.Corretiva,
                        UltimaManutencao = ultimoRealizado
                    };
                    repServicoOrdemServico.Inserir(ordemServicoFrotaServicoVeiculo);
                }
                else
                {
                    ordemServicoFrotaServicoVeiculo.CustoEstimado = ordemServico.PneuEnvioReforma.CustoEstimado;
                    ordemServicoFrotaServicoVeiculo.CustoMedio = ordemServico.PneuEnvioReforma.CustoEstimado;
                    ordemServicoFrotaServicoVeiculo.Observacao = "SERVIÇO VINCULADO COM PNEU " + pneu.NumeroFogo;
                    ordemServicoFrotaServicoVeiculo.OrdemServico = ordemServico;
                    ordemServicoFrotaServicoVeiculo.Servico = ordemServico.PneuEnvioReforma.ServicoVeiculo;
                    ordemServicoFrotaServicoVeiculo.TipoManutencao = ordemServico.PneuEnvioReforma.ServicoVeiculo.Motivo == MotivoServicoVeiculo.Reforma || ordemServico.PneuEnvioReforma.ServicoVeiculo.Motivo == MotivoServicoVeiculo.ConsertoEReforma ? TipoManutencaoServicoVeiculoOrdemServicoFrota.Preventiva : TipoManutencaoServicoVeiculoOrdemServicoFrota.Corretiva;
                    ordemServicoFrotaServicoVeiculo.UltimaManutencao = ultimoRealizado;

                    repServicoOrdemServico.Atualizar(ordemServicoFrotaServicoVeiculo);
                }

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamento ordemServicoFrotaOrcamento = repOrdemServicoFrotaOrcamento.BuscarPorOrdemServico(ordemServico.Codigo);
                if (ordemServicoFrotaOrcamento == null)
                {
                    ordemServicoFrotaOrcamento = new Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamento()
                    {
                        Observacao = "SERVIÇO VINCULADO COM PNEU " + pneu.NumeroFogo,
                        OrdemServico = ordemServico,
                        Parcelas = 0,
                        ValorTotalMaoObra = valorMaoObra,
                        ValorTotalOrcado = valorMaoObra + valorProdutos,
                        ValorTotalPreAprovado = valorMaoObra + valorProdutos,
                        ValorTotalProdutos = valorProdutos
                    };
                    repOrdemServicoFrotaOrcamento.Inserir(ordemServicoFrotaOrcamento);
                }
                else
                {
                    ordemServicoFrotaOrcamento.Observacao = "SERVIÇO VINCULADO COM PNEU " + pneu.NumeroFogo;
                    ordemServicoFrotaOrcamento.Parcelas = 0;
                    ordemServicoFrotaOrcamento.ValorTotalMaoObra = valorMaoObra;
                    ordemServicoFrotaOrcamento.ValorTotalOrcado = valorMaoObra + valorProdutos;
                    ordemServicoFrotaOrcamento.ValorTotalPreAprovado = valorMaoObra + valorProdutos;
                    ordemServicoFrotaOrcamento.ValorTotalProdutos = valorProdutos;
                    repOrdemServicoFrotaOrcamento.Atualizar(ordemServicoFrotaOrcamento);
                }

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico ordemServicoFrotaOrcamentoServico = repOrdemServicoFrotaOrcamentoServico.BuscarPorOrdemServicoEManutencao(ordemServico.Codigo, ordemServicoFrotaServicoVeiculo.Codigo);
                if (ordemServicoFrotaOrcamentoServico == null)
                {
                    ordemServicoFrotaOrcamentoServico = new Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico()
                    {
                        Manutencao = ordemServicoFrotaServicoVeiculo,
                        Observacao = "SERVIÇO VINCULADO COM PNEU " + pneu.NumeroFogo,
                        OrcadoPor = responsavelOrcamento,
                        Orcamento = ordemServicoFrotaOrcamento,
                        ValorMaoObra = valorMaoObra,
                        ValorProdutos = valorProdutos
                    };
                    repOrdemServicoFrotaOrcamentoServico.Inserir(ordemServicoFrotaOrcamentoServico);
                }
                else
                {
                    ordemServicoFrotaOrcamentoServico.Manutencao = ordemServicoFrotaServicoVeiculo;
                    ordemServicoFrotaOrcamentoServico.Observacao = "SERVIÇO VINCULADO COM PNEU " + pneu.NumeroFogo;
                    ordemServicoFrotaOrcamentoServico.OrcadoPor = responsavelOrcamento;
                    ordemServicoFrotaOrcamentoServico.Orcamento = ordemServicoFrotaOrcamento;
                    ordemServicoFrotaOrcamentoServico.ValorMaoObra = valorMaoObra;
                    ordemServicoFrotaOrcamentoServico.ValorProdutos = valorProdutos;

                    repOrdemServicoFrotaOrcamentoServico.Atualizar(ordemServicoFrotaOrcamentoServico);
                }

                foreach (var prod in produtos)
                {
                    Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto ordemServicoFrotaFechamentoProduto = new Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto()
                    {
                        Autorizado = true,
                        FinalidadeProduto = prod.FinalidadeProduto,
                        Garantia = false,
                        OrdemServico = ordemServico,
                        Origem = TipoLancamento.Automatico,
                        Produto = prod.Produto,
                        QuantidadeDocumento = prod.Quantidade,
                        QuantidadeOrcada = prod.Quantidade,
                        Situacao = SituacaoProdutoFechamentoOrdemServicoFrota.ConformeOrcado,
                        ValorDocumento = prod.Valor * prod.Quantidade,
                        ValorOrcado = prod.Valor * prod.Quantidade,
                        ValorUnitario = prod.Valor
                    };
                    repOrdemServicoFrotaFechamentoProduto.Inserir(ordemServicoFrotaFechamentoProduto);
                }

                if (!Servicos.Embarcador.Frota.OrdemServico.FinalizarOrdemServico(out erro, ref ordemServico, Usuario, unitOfWork, TipoServicoMultisoftware, Auditado))
                    return erro;
            }
            return erro;

        }

        private Dominio.Entidades.Embarcador.Frota.BandaRodagemPneu ObterBandaRodagemPneu(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoBandaRodagem = Request.GetIntParam("BandaRodagem");
            Repositorio.Embarcador.Frota.BandaRodagemPneu RepositorioBandaRodagemPneu = new Repositorio.Embarcador.Frota.BandaRodagemPneu(unitOfWork);

            return RepositorioBandaRodagemPneu.BuscarPorCodigo(codigoBandaRodagem) ?? null;
        }

        private Dominio.Entidades.Produto ObterProduto(Repositorio.UnitOfWork unitOfWork, int codigoProduto)
        {
            Repositorio.Produto RepositorioProduto = new Repositorio.Produto(unitOfWork);

            return RepositorioProduto.BuscarPorCodigo(codigoProduto) ?? throw new ControllerException("Produto não encontrado.");
        }

        private Dominio.Entidades.Embarcador.Frota.FinalidadeProdutoOrdemServico ObterFinalidadeProdutoOrdemServico(Repositorio.UnitOfWork unitOfWork, int codigoFinalidadeProdutoOrdemServico)
        {
            Repositorio.Embarcador.Frota.FinalidadeProdutoOrdemServico repFinalidadeProdutoOrdemServico = new Repositorio.Embarcador.Frota.FinalidadeProdutoOrdemServico(unitOfWork);

            return repFinalidadeProdutoOrdemServico.BuscarPorCodigo(codigoFinalidadeProdutoOrdemServico) ?? null;
        }

        #endregion

        #region Métodos Privados de Movimentação para Veículo

        private void RealizarRodizioPneus(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frota.PneuHistorico repositorioPneuHistorico = new Repositorio.Embarcador.Frota.PneuHistorico(unitOfWork);

            int codigoVeiculo = Request.GetIntParam("CodigoVeiculo");
            DateTime dataHora = Request.GetDateTimeParam("DataHora");
            string motivoRodizio = Request.GetStringParam("MotivoRodizio");

            int codigoPneu_1 = Request.GetIntParam("CodigoPneu_1");
            int codigoEixoPneuOrigem_1 = Request.GetIntParam("CodigoEixoPneuOrigem_1");
            int codigoEstepeOrigem_1 = Request.GetIntParam("CodigoEstepeOrigem_1");

            int codigoPneu_2 = Request.GetIntParam("CodigoPneu_2");
            int codigoEixoPneuOrigem_2 = Request.GetIntParam("CodigoEixoPneuOrigem_2");
            int codigoEstepeOrigem_2 = Request.GetIntParam("CodigoEstepeOrigem_2");

            Repositorio.Embarcador.Frota.Pneu repositorioPneu = new Repositorio.Embarcador.Frota.Pneu(unitOfWork);
            Dominio.Entidades.Embarcador.Frota.Pneu pneu1 = repositorioPneu.BuscarPorCodigo(codigoPneu_1);
            Dominio.Entidades.Embarcador.Frota.Pneu pneu2 = repositorioPneu.BuscarPorCodigo(codigoPneu_2);

            if (pneu1 == null || pneu2 == null)
                throw new ControllerException("Um ou ambos os pneus não foram encontrados.");

            Dominio.Entidades.Veiculo veiculo = ObterVeiculo(unitOfWork, codigoVeiculo);
            if (veiculo == null)
                throw new ControllerException("Veículo não encontrado.");

            string posicaoOrigemPneu1 = codigoEixoPneuOrigem_1 > 0
                ? $"eixo {ObterModeloVeicularCargaEixoPneu(unitOfWork, codigoEixoPneuOrigem_1).Eixo.Numero} posição {ObterModeloVeicularCargaEixoPneu(unitOfWork, codigoEixoPneuOrigem_1).Posicao.ObterDescricao().ToLower()}"
                : $"estepe número {ObterModeloVeicularCargaEstepe(unitOfWork, codigoEstepeOrigem_1).Numero}";
            string posicaoDestinoPneu1 = codigoEixoPneuOrigem_2 > 0
                ? $"eixo {ObterModeloVeicularCargaEixoPneu(unitOfWork, codigoEixoPneuOrigem_2).Eixo.Numero} posição {ObterModeloVeicularCargaEixoPneu(unitOfWork, codigoEixoPneuOrigem_2).Posicao.ObterDescricao().ToLower()}"
                : $"estepe número {ObterModeloVeicularCargaEstepe(unitOfWork, codigoEstepeOrigem_2).Numero}";

            string posicaoOrigemPneu2 = codigoEixoPneuOrigem_2 > 0
                ? $"eixo {ObterModeloVeicularCargaEixoPneu(unitOfWork, codigoEixoPneuOrigem_2).Eixo.Numero} posição {ObterModeloVeicularCargaEixoPneu(unitOfWork, codigoEixoPneuOrigem_2).Posicao.ObterDescricao().ToLower()}"
                : $"estepe número {ObterModeloVeicularCargaEstepe(unitOfWork, codigoEstepeOrigem_2).Numero}";
            string posicaoDestinoPneu2 = codigoEixoPneuOrigem_1 > 0
                ? $"eixo {ObterModeloVeicularCargaEixoPneu(unitOfWork, codigoEixoPneuOrigem_1).Eixo.Numero} posição {ObterModeloVeicularCargaEixoPneu(unitOfWork, codigoEixoPneuOrigem_1).Posicao.ObterDescricao().ToLower()}"
                : $"estepe número {ObterModeloVeicularCargaEstepe(unitOfWork, codigoEstepeOrigem_1).Numero}";


            Dominio.Entidades.Embarcador.Frota.PneuHistorico historicoPneu1 = new Dominio.Entidades.Embarcador.Frota.PneuHistorico()
            {
                Data = DateTime.Now,
                Descricao = $"Rodízio realizado: Pneu {pneu1.NumeroFogo} movido de {posicaoOrigemPneu1} para {posicaoDestinoPneu1} do veículo {veiculo.Placa_Formatada}. Motivo: {motivoRodizio}",
                Pneu = pneu1,
                Tipo = TipoPneuHistorico.Rodizio, 
                BandaRodagem = pneu1.BandaRodagem,
                KmAtualRodado = pneu1.KmAtualRodado,
                DataHoraMovimentacao = dataHora,
                Usuario = this.Usuario,
            };
            repositorioPneuHistorico.Inserir(historicoPneu1);

            Dominio.Entidades.Embarcador.Frota.PneuHistorico historicoPneu2 = new Dominio.Entidades.Embarcador.Frota.PneuHistorico()
            {
                Data = DateTime.Now,
                Descricao = $"Rodízio realizado: Pneu {pneu2.NumeroFogo} movido de {posicaoOrigemPneu2} para {posicaoDestinoPneu2} do veículo {veiculo.Placa_Formatada}. Motivo: {motivoRodizio}",
                Pneu = pneu2,
                Tipo = TipoPneuHistorico.Rodizio, 
                BandaRodagem = pneu2.BandaRodagem,
                KmAtualRodado = pneu2.KmAtualRodado,
                DataHoraMovimentacao = dataHora,
                Usuario = this.Usuario,
            };
            repositorioPneuHistorico.Inserir(historicoPneu2);

            Dominio.Entidades.Embarcador.Frota.MovimentacaoPneuVeiculoDadosAdicionais dadosAdicionaisPneu1 = CriarDadosAdicionaisRodizio(unitOfWork, "1");
            Dominio.Entidades.Embarcador.Frota.MovimentacaoPneuVeiculoDadosAdicionais dadosAdicionaisPneu2 = CriarDadosAdicionaisRodizio(unitOfWork, "2");

            if (codigoEixoPneuOrigem_1 > 0)
                AdicionarMovimentacaoEixoPneuSaida(unitOfWork, pneu1, codigoVeiculo, codigoEixoPneuOrigem_1, historicoPneu1, dadosAdicionaisPneu1);
            else if (codigoEstepeOrigem_1 > 0)
                AdicionarMovimentacaoEstepeSaida(unitOfWork, pneu1, codigoVeiculo, codigoEstepeOrigem_1, historicoPneu1, dadosAdicionaisPneu1);

            if (codigoEixoPneuOrigem_2 > 0)
                AdicionarMovimentacaoEixoPneuSaida(unitOfWork, pneu2, codigoVeiculo, codigoEixoPneuOrigem_2, historicoPneu2, dadosAdicionaisPneu2);
            else if (codigoEstepeOrigem_2 > 0)
                AdicionarMovimentacaoEstepeSaida(unitOfWork, pneu2, codigoVeiculo, codigoEstepeOrigem_2, historicoPneu2, dadosAdicionaisPneu2);

            if (codigoEixoPneuOrigem_2 > 0)
                AdicionarMovimentacaoEixoPneuEntrada(unitOfWork, pneu1, codigoVeiculo, codigoEixoPneuOrigem_2, historicoPneu1, dadosAdicionaisPneu1);
            else if (codigoEstepeOrigem_2 > 0)
                AdicionarMovimentacaoEstepeEntrada(unitOfWork, pneu1, codigoVeiculo, codigoEstepeOrigem_2, historicoPneu1, dadosAdicionaisPneu1);

            if (codigoEixoPneuOrigem_1 > 0)
                AdicionarMovimentacaoEixoPneuEntrada(unitOfWork, pneu2, codigoVeiculo, codigoEixoPneuOrigem_1, historicoPneu2, dadosAdicionaisPneu2);
            else if (codigoEstepeOrigem_1 > 0)
                AdicionarMovimentacaoEstepeEntrada(unitOfWork, pneu2, codigoVeiculo, codigoEstepeOrigem_1, historicoPneu2, dadosAdicionaisPneu2);

        }

        private void AdicionarMovimentacaoEixoPneuEntrada(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Frota.Pneu pneu, int codigoVeiculo, int codigoEixoPneuDestino, Dominio.Entidades.Embarcador.Frota.PneuHistorico? pneuHistorico = null, Dominio.Entidades.Embarcador.Frota.MovimentacaoPneuVeiculoDadosAdicionais? dadosAdicionais = null)
        {
            Repositorio.VeiculoPneu repositorioVeiculoPneu = new Repositorio.VeiculoPneu(unitOfWork);
            Dominio.Entidades.VeiculoPneu veiculoPneu = repositorioVeiculoPneu.BuscarPorEixoPneu(codigoEixoPneuDestino, codigoVeiculo);

            if (veiculoPneu != null)
                throw new ControllerException($"Existe um pneu no eixo {veiculoPneu.EixoPneu.Eixo.Numero} posição {veiculoPneu.EixoPneu.Posicao.ObterDescricao().ToLower()} do veículo {veiculoPneu.Veiculo.Placa_Formatada}.");

            Dominio.Entidades.Veiculo veiculo = ObterVeiculo(unitOfWork, codigoVeiculo);
            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEixoPneu eixoPneu = ObterModeloVeicularCargaEixoPneu(unitOfWork, codigoEixoPneuDestino);
            veiculoPneu = new Dominio.Entidades.VeiculoPneu()
            {
                EixoPneu = eixoPneu,
                Pneu = pneu,
                Veiculo = veiculo,
                DataMovimentacao = DateTime.Now,
                DataMovimentacaoPneu = Request.GetDateTimeParam("DataHora")
            };

            if (veiculo != null && veiculo.ModeloVeicularCarga != null && eixoPneu != null && eixoPneu.Eixo != null && eixoPneu.Eixo.ModeloVeicularCarga != null && veiculo.ModeloVeicularCarga.Codigo != eixoPneu.Eixo.ModeloVeicularCarga.Codigo)
                throw new ControllerException($"O modelo veícular do veículo ({veiculoPneu.Veiculo.ModeloVeicularCarga.Descricao}) está diferente do modelo do eixo ({eixoPneu.Eixo.ModeloVeicularCarga.Descricao}). Favor atualize a página e verifique os cadastros.");

            repositorioVeiculoPneu.Inserir(veiculoPneu);

            if (pneuHistorico == null)
            {
                Repositorio.Embarcador.Frota.PneuHistorico repositorioPneuHistorico = new Repositorio.Embarcador.Frota.PneuHistorico(unitOfWork);
                pneuHistorico = new Dominio.Entidades.Embarcador.Frota.PneuHistorico()
                {
                    Data = DateTime.Now,
                    Descricao = $"Adicionado o pneu no eixo {veiculoPneu.EixoPneu.Eixo.Numero} posição {eixoPneu.Posicao.ObterDescricao().ToLower()} do veículo {veiculo.Placa_Formatada}",
                    Pneu = pneu,
                    Tipo = TipoPneuHistorico.MovimentacaoVeiculoEntrada,
                    BandaRodagem = pneu.BandaRodagem,
                    KmAtualRodado = pneu.KmAtualRodado,
                    DataHoraMovimentacao = Request.GetDateTimeParam("DataHora"),
                    Usuario = this.Usuario,
                };
                repositorioPneuHistorico.Inserir(pneuHistorico);
            }

            if (dadosAdicionais == null)
                dadosAdicionais = AdicionarMovimentacaoPneuVeiculoDadosAdicionais(unitOfWork);

            Repositorio.Embarcador.Frota.MovimentacaoPneuVeiculo repositorioMovimentacaoPneuVeiculo = new Repositorio.Embarcador.Frota.MovimentacaoPneuVeiculo(unitOfWork);
            Dominio.Entidades.Embarcador.Frota.MovimentacaoPneuVeiculo movimentacao = new Dominio.Entidades.Embarcador.Frota.MovimentacaoPneuVeiculo()
            {
                DadosAdicionais = dadosAdicionais,
                Data = Request.GetDateTimeParam("DataHora"),
                DataCadastro = DateTime.Now,
                EixoPneu = eixoPneu,
                Pneu = pneu,
                PneuHistorico = pneuHistorico,
                Tipo = TipoMovimentacaoPneuVeiculo.Entrada,
                Veiculo = veiculo
            };

            repositorioMovimentacaoPneuVeiculo.Inserir(movimentacao);
        }

        private void AdicionarMovimentacaoEixoPneuSaida(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Frota.Pneu pneu, int codigoVeiculo, int codigoEixoPneuOrigem, Dominio.Entidades.Embarcador.Frota.PneuHistorico? pneuHistorico = null, Dominio.Entidades.Embarcador.Frota.MovimentacaoPneuVeiculoDadosAdicionais? dadosAdicionais = null)
        {
            Repositorio.VeiculoPneu repositorioVeiculoPneu = new Repositorio.VeiculoPneu(unitOfWork);
            Dominio.Entidades.VeiculoPneu veiculoPneu = repositorioVeiculoPneu.BuscarPorEixoPneu(codigoEixoPneuOrigem, codigoVeiculo);

            if (veiculoPneu == null)
                throw new ControllerException("Pneu não encontrado no veículo para realizar a movimentação de saída.");

            if (pneuHistorico == null)
            {
                Repositorio.Embarcador.Frota.PneuHistorico repositorioPneuHistorico = new Repositorio.Embarcador.Frota.PneuHistorico(unitOfWork);
                pneuHistorico = new Dominio.Entidades.Embarcador.Frota.PneuHistorico()
                {
                    Data = DateTime.Now,
                    Descricao = $"Removido o pneu do eixo {veiculoPneu.EixoPneu.Eixo.Numero} posição {veiculoPneu.EixoPneu.Posicao.ObterDescricao().ToLower()} do veículo {veiculoPneu.Veiculo.Placa_Formatada}",
                    Pneu = pneu,
                    Tipo = TipoPneuHistorico.MovimentacaoVeiculoSaida,
                    BandaRodagem = pneu.BandaRodagem,
                    KmAtualRodado = pneu.KmAtualRodado,
                    DataHoraMovimentacao = Request.GetDateTimeParam("DataHora"),
                    Usuario = this.Usuario,
                };
                repositorioPneuHistorico.Inserir(pneuHistorico);
            }

            if (Request.GetBoolParam("UtilizarDadosAdicionais") && dadosAdicionais == null)
                dadosAdicionais = AdicionarMovimentacaoPneuVeiculoDadosAdicionais(unitOfWork);

            Repositorio.Embarcador.Frota.MovimentacaoPneuVeiculo repositorioMovimentacaoPneuVeiculo = new Repositorio.Embarcador.Frota.MovimentacaoPneuVeiculo(unitOfWork);
            Dominio.Entidades.Embarcador.Frota.MovimentacaoPneuVeiculo movimentacao = new Dominio.Entidades.Embarcador.Frota.MovimentacaoPneuVeiculo()
            {
                DadosAdicionais = dadosAdicionais,
                Data = Request.GetDateTimeParam("DataHora"),
                DataCadastro = DateTime.Now,
                EixoPneu = veiculoPneu.EixoPneu,
                Pneu = pneu,
                PneuHistorico = pneuHistorico,
                Tipo = TipoMovimentacaoPneuVeiculo.Saida,
                Veiculo = veiculoPneu.Veiculo
            };

            repositorioMovimentacaoPneuVeiculo.Inserir(movimentacao);
            repositorioVeiculoPneu.Deletar(veiculoPneu);
        }

        private void AdicionarMovimentacaoEixoPneuVirada(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Frota.Pneu pneu, int codigoVeiculo, int codigoEixoPneu)
        {
            Dominio.Entidades.Veiculo veiculo = ObterVeiculo(unitOfWork, codigoVeiculo);
            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEixoPneu eixoPneu = ObterModeloVeicularCargaEixoPneu(unitOfWork, codigoEixoPneu);

            Repositorio.Embarcador.Frota.PneuHistorico repositorioPneuHistorico = new Repositorio.Embarcador.Frota.PneuHistorico(unitOfWork);
            Dominio.Entidades.Embarcador.Frota.PneuHistorico pneuHistorico = new Dominio.Entidades.Embarcador.Frota.PneuHistorico()
            {
                Data = DateTime.Now,
                Descricao = $"Realizada a virada do pneu no eixo {eixoPneu.Eixo.Numero} posição {eixoPneu.Posicao.ObterDescricao().ToLower()} do veículo {veiculo.Placa_Formatada}",
                Pneu = pneu,
                Tipo = TipoPneuHistorico.ViradaPneu,
                BandaRodagem = pneu.BandaRodagem,
                KmAtualRodado = pneu.KmAtualRodado,
                DataHoraMovimentacao = Request.GetDateTimeParam("DataHora"),
                Usuario = this.Usuario,
            };

            repositorioPneuHistorico.Inserir(pneuHistorico);

            Repositorio.Embarcador.Frota.MovimentacaoPneuVeiculo repositorioMovimentacaoPneuVeiculo = new Repositorio.Embarcador.Frota.MovimentacaoPneuVeiculo(unitOfWork);
            Dominio.Entidades.Embarcador.Frota.MovimentacaoPneuVeiculo movimentacao = new Dominio.Entidades.Embarcador.Frota.MovimentacaoPneuVeiculo()
            {
                DadosAdicionais = AdicionarMovimentacaoPneuVeiculoDadosAdicionais(unitOfWork),
                Data = Request.GetDateTimeParam("DataHora"),
                DataCadastro = DateTime.Now,
                EixoPneu = eixoPneu,
                Pneu = pneu,
                PneuHistorico = pneuHistorico,
                Tipo = TipoMovimentacaoPneuVeiculo.Virada,
                Veiculo = veiculo
            };

            repositorioMovimentacaoPneuVeiculo.Inserir(movimentacao);
        }

        private void AdicionarMovimentacaoEstepeEntrada(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Frota.Pneu pneu, int codigoVeiculo, int codigoEstepeDestino, Dominio.Entidades.Embarcador.Frota.PneuHistorico? pneuHistorico = null, Dominio.Entidades.Embarcador.Frota.MovimentacaoPneuVeiculoDadosAdicionais? dadosAdicionais = null)
        {
            Repositorio.VeiculoEstepe repositorioVeiculoEstepe = new Repositorio.VeiculoEstepe(unitOfWork);
            Dominio.Entidades.VeiculoEstepe veiculoEstepe = repositorioVeiculoEstepe.BuscarPorEstepe(codigoEstepeDestino, codigoVeiculo);

            if (veiculoEstepe != null)
                throw new ControllerException($"Existe um pneu no estepe número {veiculoEstepe.Estepe.Numero} do veículo {veiculoEstepe.Veiculo.Placa_Formatada}.");

            Dominio.Entidades.Veiculo veiculo = ObterVeiculo(unitOfWork, codigoVeiculo);
            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEstepe estepe = ObterModeloVeicularCargaEstepe(unitOfWork, codigoEstepeDestino);
            veiculoEstepe = new Dominio.Entidades.VeiculoEstepe()
            {
                Estepe = estepe,
                Pneu = pneu,
                Veiculo = veiculo,
                DataMovimentacao = DateTime.Now,
                DataMovimentacaoPneu = Request.GetDateTimeParam("DataHora")
            };

            if (veiculo != null && veiculo.ModeloVeicularCarga != null && estepe != null && estepe.ModeloVeicularCarga != null && veiculo.ModeloVeicularCarga.Codigo != estepe.ModeloVeicularCarga.Codigo)
                throw new ControllerException($"O modelo veícular do veículo ({veiculo.ModeloVeicularCarga.Descricao}) está diferente do modelo do estepe ({estepe.ModeloVeicularCarga.Descricao}). Favor atualize a página e verifique os cadastros.");

            repositorioVeiculoEstepe.Inserir(veiculoEstepe);

            if (pneuHistorico == null)
            {
                Repositorio.Embarcador.Frota.PneuHistorico repositorioPneuHistorico = new Repositorio.Embarcador.Frota.PneuHistorico(unitOfWork);
                pneuHistorico = new Dominio.Entidades.Embarcador.Frota.PneuHistorico()
                {
                    Data = DateTime.Now,
                    Descricao = $"Adicionado o pneu no estepe número {veiculoEstepe.Estepe.Numero} do veículo {veiculoEstepe.Veiculo.Placa_Formatada}",
                    Pneu = pneu,
                    Tipo = TipoPneuHistorico.MovimentacaoVeiculoEntrada,
                    BandaRodagem = pneu.BandaRodagem,
                    KmAtualRodado = pneu.KmAtualRodado,
                    DataHoraMovimentacao = Request.GetDateTimeParam("DataHora"),
                    Usuario = this.Usuario,
                };
                repositorioPneuHistorico.Inserir(pneuHistorico);
            }

            if (dadosAdicionais == null)
                dadosAdicionais = AdicionarMovimentacaoPneuVeiculoDadosAdicionais(unitOfWork);

            Repositorio.Embarcador.Frota.MovimentacaoPneuVeiculo repositorioMovimentacaoPneuVeiculo = new Repositorio.Embarcador.Frota.MovimentacaoPneuVeiculo(unitOfWork);
            Dominio.Entidades.Embarcador.Frota.MovimentacaoPneuVeiculo movimentacao = new Dominio.Entidades.Embarcador.Frota.MovimentacaoPneuVeiculo()
            {
                DadosAdicionais = dadosAdicionais,
                Data = Request.GetDateTimeParam("DataHora"),
                DataCadastro = DateTime.Now,
                Estepe = estepe,
                Pneu = pneu,
                PneuHistorico = pneuHistorico,
                Tipo = TipoMovimentacaoPneuVeiculo.Entrada,
                Veiculo = veiculo
            };

            repositorioMovimentacaoPneuVeiculo.Inserir(movimentacao);
        }

        private void AdicionarMovimentacaoEstepeSaida(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Frota.Pneu pneu, int codigoVeiculo, int codigoEstepeOrigem, Dominio.Entidades.Embarcador.Frota.PneuHistorico? pneuHistorico = null, Dominio.Entidades.Embarcador.Frota.MovimentacaoPneuVeiculoDadosAdicionais? dadosAdicionais = null)
        {
            Repositorio.VeiculoEstepe repositorioVeiculoEstepe = new Repositorio.VeiculoEstepe(unitOfWork);
            Dominio.Entidades.VeiculoEstepe veiculoEstepe = repositorioVeiculoEstepe.BuscarPorEstepe(codigoEstepeOrigem, codigoVeiculo);

            if (veiculoEstepe == null)
                throw new ControllerException("Pneu não encontrado no estepe veículo para realizar a movimentação de saída.");

            if (pneuHistorico == null)
            {
                Repositorio.Embarcador.Frota.PneuHistorico repositorioPneuHistorico = new Repositorio.Embarcador.Frota.PneuHistorico(unitOfWork);
                pneuHistorico = new Dominio.Entidades.Embarcador.Frota.PneuHistorico()
                {
                    Data = DateTime.Now,
                    Descricao = $"Removido o pneu do estepe número {veiculoEstepe.Estepe.Numero} do veículo {veiculoEstepe.Veiculo.Placa_Formatada}",
                    Pneu = pneu,
                    Tipo = TipoPneuHistorico.MovimentacaoVeiculoSaida,
                    BandaRodagem = pneu.BandaRodagem,
                    KmAtualRodado = pneu.KmAtualRodado,
                    DataHoraMovimentacao = Request.GetDateTimeParam("DataHora"),
                    Usuario = this.Usuario,
                };
                repositorioPneuHistorico.Inserir(pneuHistorico);
            }

            if (Request.GetBoolParam("UtilizarDadosAdicionais") && dadosAdicionais == null)
                dadosAdicionais = AdicionarMovimentacaoPneuVeiculoDadosAdicionais(unitOfWork); 

            Repositorio.Embarcador.Frota.MovimentacaoPneuVeiculo repositorioMovimentacaoPneuVeiculo = new Repositorio.Embarcador.Frota.MovimentacaoPneuVeiculo(unitOfWork);
            Dominio.Entidades.Embarcador.Frota.MovimentacaoPneuVeiculo movimentacao = new Dominio.Entidades.Embarcador.Frota.MovimentacaoPneuVeiculo()
            {
                DadosAdicionais = dadosAdicionais,
                Data = Request.GetDateTimeParam("DataHora"),
                DataCadastro = DateTime.Now,
                Estepe = veiculoEstepe.Estepe,
                Pneu = pneu,
                PneuHistorico = pneuHistorico,
                Tipo = TipoMovimentacaoPneuVeiculo.Saida,
                Veiculo = veiculoEstepe.Veiculo
            };

            repositorioMovimentacaoPneuVeiculo.Inserir(movimentacao);
            repositorioVeiculoEstepe.Deletar(veiculoEstepe);
        }

        private void AdicionarMovimentacaoParaVeiculoEntrada(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Frota.Pneu pneu)
        {
            int codigoEixoPneuDestino = Request.GetIntParam("CodigoEixoPneuDestino");
            int codigoEstepeDestino = Request.GetIntParam("CodigoEstepeDestino");
            int codigoVeiculo = Request.GetIntParam("CodigoVeiculo");

            if (codigoEixoPneuDestino > 0)
                AdicionarMovimentacaoEixoPneuEntrada(unitOfWork, pneu, codigoVeiculo, codigoEixoPneuDestino);
            else if (codigoEstepeDestino > 0)
                AdicionarMovimentacaoEstepeEntrada(unitOfWork, pneu, codigoVeiculo, codigoEstepeDestino);
        }

        private void AdicionarMovimentacaoParaVeiculoSaida(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Frota.Pneu pneu)
        {
            int codigoEixoPneuOrigem = Request.GetIntParam("CodigoEixoPneuOrigem");
            int codigoEstepeOrigem = Request.GetIntParam("CodigoEstepeOrigem");
            int codigoVeiculo = Request.GetIntParam("CodigoVeiculo");

            if (codigoEixoPneuOrigem > 0)
                AdicionarMovimentacaoEixoPneuSaida(unitOfWork, pneu, codigoVeiculo, codigoEixoPneuOrigem);
            else if (codigoEstepeOrigem > 0)
                AdicionarMovimentacaoEstepeSaida(unitOfWork, pneu, codigoVeiculo, codigoEstepeOrigem);
        }

        private void AdicionarMovimentacaoViradaPneuVeiculo(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Frota.Pneu pneu)
        {
            int codigoEixoPneu = Request.GetIntParam("CodigoEixoPneu");
            int codigoVeiculo = Request.GetIntParam("CodigoVeiculo");

            if (codigoEixoPneu > 0)
                AdicionarMovimentacaoEixoPneuVirada(unitOfWork, pneu, codigoVeiculo, codigoEixoPneu);
        }

        private Dominio.Entidades.Embarcador.Frota.MovimentacaoPneuVeiculoDadosAdicionais AdicionarMovimentacaoPneuVeiculoDadosAdicionais(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frota.MovimentacaoPneuVeiculoDadosAdicionais repositorioMovimentacaoPneuVeiculoDadosAdicionais = new Repositorio.Embarcador.Frota.MovimentacaoPneuVeiculoDadosAdicionais(unitOfWork);
            Dominio.Entidades.Embarcador.Frota.MovimentacaoPneuVeiculoDadosAdicionais dadosAdicionais = new Dominio.Entidades.Embarcador.Frota.MovimentacaoPneuVeiculoDadosAdicionais()
            {
                Calibragem = Request.GetIntParam("Calibragem"),
                MilimetragemDois = Request.GetDecimalParam("MilimetragemDois"),
                MilimetragemMedia = Request.GetDecimalParam("MilimetragemMedia"),
                MilimetragemQuatro = Request.GetDecimalParam("MilimetragemQuatro"),
                MilimetragemTres = Request.GetDecimalParam("MilimetragemTres"),
                MilimetragemUm = Request.GetDecimalParam("MilimetragemUm")
            };

            repositorioMovimentacaoPneuVeiculoDadosAdicionais.Inserir(dadosAdicionais);

            return dadosAdicionais;
        }

        private Dominio.Entidades.Embarcador.Frota.MovimentacaoPneuVeiculoDadosAdicionais CriarDadosAdicionaisRodizio(Repositorio.UnitOfWork unitOfWork, string sufixo)
        {
            Repositorio.Embarcador.Frota.MovimentacaoPneuVeiculoDadosAdicionais repositorioMovimentacaoPneuVeiculoDadosAdicionais = new Repositorio.Embarcador.Frota.MovimentacaoPneuVeiculoDadosAdicionais(unitOfWork);
            decimal milimetragemUm = Request.GetNullableDecimalParam($"MilimetragemUm_{sufixo}") ?? 0;
            decimal milimetragemDois = Request.GetNullableDecimalParam($"MilimetragemDois_{sufixo}") ?? 0;
            decimal milimetragemTres = Request.GetNullableDecimalParam($"MilimetragemTres_{sufixo}") ?? 0;
            decimal milimetragemQuatro = Request.GetNullableDecimalParam($"MilimetragemQuatro_{sufixo}") ?? 0;
            decimal milimetragemMedia = (milimetragemUm + milimetragemDois + milimetragemTres + milimetragemQuatro) / 4;

            Dominio.Entidades.Embarcador.Frota.MovimentacaoPneuVeiculoDadosAdicionais dadosAdicionais = new Dominio.Entidades.Embarcador.Frota.MovimentacaoPneuVeiculoDadosAdicionais()
            {
                Calibragem = Request.GetNullableIntParam($"Calibragem_{sufixo}") ?? 0 ,
                MilimetragemDois = milimetragemDois,
                MilimetragemMedia = milimetragemMedia,
                MilimetragemQuatro = milimetragemQuatro,
                MilimetragemTres = milimetragemTres,
                MilimetragemUm = milimetragemUm
            };

            repositorioMovimentacaoPneuVeiculoDadosAdicionais.Inserir(dadosAdicionais);
            return dadosAdicionais;
        }

        private Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEixoPneu ObterModeloVeicularCargaEixoPneu(Repositorio.UnitOfWork unitOfWork, int codigoEixoPneu)
        {
            Repositorio.Embarcador.Cargas.ModeloVeicularCargaEixoPneu RepositorioEixoPneu = new Repositorio.Embarcador.Cargas.ModeloVeicularCargaEixoPneu(unitOfWork);

            return RepositorioEixoPneu.BuscarPorCodigo(codigoEixoPneu, auditavel: false) ?? throw new ControllerException("Posição do pneu no eixo não encontrada no modelo veícular de carga.");
        }

        private Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEstepe ObterModeloVeicularCargaEstepe(Repositorio.UnitOfWork unitOfWork, int codigoEstepe)
        {
            Repositorio.Embarcador.Cargas.ModeloVeicularCargaEstepe RepositorioEstepe = new Repositorio.Embarcador.Cargas.ModeloVeicularCargaEstepe(unitOfWork);

            return RepositorioEstepe.BuscarPorCodigo(codigoEstepe, auditavel: false) ?? throw new ControllerException("Posição do estepe não encontrada no modelo veícular de carga.");
        }

        #endregion

        #region Métodos Privados de Troca de Almoxarifado

        private void AdicionarHistoricoTrocaAlmoxarifado(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Frota.Pneu pneu, Dominio.Entidades.Embarcador.Frota.Almoxarifado almoxarifado, Dominio.Entidades.Embarcador.Frota.Almoxarifado almoxarifadoOrigem)
        {
            if (almoxarifado.Codigo != almoxarifadoOrigem.Codigo)
            {
                Repositorio.Embarcador.Frota.PneuHistorico repositorioPneuHistorico = new Repositorio.Embarcador.Frota.PneuHistorico(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.PneuHistorico pneuHistorico = new Dominio.Entidades.Embarcador.Frota.PneuHistorico()
                {
                    Data = DateTime.Now,
                    Descricao = $"Alterado o almoxarifado de {almoxarifadoOrigem.Descricao} para {almoxarifado.Descricao}",
                    Pneu = pneu,
                    Tipo = TipoPneuHistorico.TrocaAlmoxarifado,
                    BandaRodagem = pneu.BandaRodagem,
                    KmAtualRodado = pneu.KmAtualRodado,
                    DataHoraMovimentacao = Request.GetDateTimeParam("DataHora"),
                    Usuario = this.Usuario,
                };

                repositorioPneuHistorico.Inserir(pneuHistorico);
            }
        }

        #endregion
    }
}
