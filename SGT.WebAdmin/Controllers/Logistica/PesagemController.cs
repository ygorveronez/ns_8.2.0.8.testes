using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/Pesagem", "Logistica/Guarita", "GestaoPatio/FluxoPatio")]
    public class PesagemController : BaseController
    {
		#region Construtores

		public PesagemController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaPesagem filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Código Pesagem", "CodigoPesagem", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Transportador", "Transportador", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Motorista", "Usuario", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Carga", "Carga", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data Pesagem", "DataPesagem", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Peso Inicial", "PesoInicial", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Peso Final", "PesoFinal", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Diferença Peso", "DiferencaPeso", 10, Models.Grid.Align.right, false);

                if (filtrosPesquisa.Status == StatusBalanca.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoStatus", 10, Models.Grid.Align.center, false);

                grid.AdicionarCabecalho("CodigoGuarita", false);

                Repositorio.Embarcador.Logistica.Pesagem repPesagem = new Repositorio.Embarcador.Logistica.Pesagem(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.Pesagem> dados = repPesagem.Consultar(filtrosPesquisa, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repPesagem.ContarConsulta(filtrosPesquisa));

                var lista = (from p in dados
                             select new
                             {
                                 p.Codigo,
                                 p.CodigoPesagem,
                                 Veiculo = p.Guarita.Carga.Veiculo?.Placa,
                                 Transportador = p.Guarita.Carga.Empresa?.RazaoSocial,
                                 Usuario = p.Guarita.Carga.NomeMotoristas,
                                 DataPesagem = p.DataPesagem.ToString("dd/MM/yyyy"),
                                 PesoInicial = p.PesoInicial.ToString("n3"),
                                 PesoFinal = p.PesoFinal.ToString("n3"),
                                 Carga = p.Guarita.Carga.CodigoCargaEmbarcador,
                                 DiferencaPeso = p.PesoFinal > 0 ? (p.PesoFinal - p.PesoInicial).ToString("n3") : 0.ToString("n3"),
                                 DescricaoStatus = p.StatusBalanca.ObterDescricao(),
                                 CodigoGuarita = p.Guarita.Codigo
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> IniciarPesagemBalanca()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoGuarita = Request.GetIntParam("CodigoGuarita");
                decimal pesagemInicial = Request.GetDecimalParam("PesagemInicial");

                Repositorio.Embarcador.Logistica.Pesagem repPesagem = new Repositorio.Embarcador.Logistica.Pesagem(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = repositorioCargaGuarita.BuscarPorCodigo(codigoGuarita);
                Dominio.Entidades.Embarcador.Logistica.Pesagem pesagemExite = repPesagem.BuscarPorGuarita(codigoGuarita);

                if (guarita == null)
                    return new JsonpResult(false, true, "Guarita não encontrada.");

                if (pesagemExite != null)
                    return new JsonpResult(false, true, "Pesagem da guarita já foi iniciada.");

                unitOfWork.Start();

                if (pesagemInicial > 0)
                {
                    guarita.Initialize();
                    guarita.PesagemInicial = pesagemInicial;
                    repositorioCargaGuarita.Atualizar(guarita, Auditado);
                }

                Dominio.Entidades.Embarcador.Logistica.Pesagem pesagem = new Dominio.Entidades.Embarcador.Logistica.Pesagem();
                pesagem.DataPesagem = DateTime.Now;
                pesagem.PesoInicial = pesagemInicial;
                pesagem.StatusBalanca = StatusBalanca.AgIntegracao;
                pesagem.Guarita = guarita;

                repPesagem.Inserir(pesagem, Auditado);

                Servicos.Embarcador.Logistica.Pesagem svcPesagem = new Servicos.Embarcador.Logistica.Pesagem(unitOfWork);
                svcPesagem.GerarIntegracoes(pesagem, TipoIntegracaoBalanca.CadastroVeiculo, SituacaoIntegracao.AgIntegracao);

                unitOfWork.CommitChanges();
                return new JsonpResult(pesagem.Codigo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao criar a pesagem.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> GerarAtualizarPesagemBalanca()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoGuarita = Request.GetIntParam("CodigoGuarita");
                int codigoBalancaInicial = Request.GetIntParam("CodigoBalancaInicial");
                int codigoBalancaFinal = Request.GetIntParam("CodigoBalancaFinal");

                if (codigoBalancaInicial == 0 && codigoBalancaFinal == 0)
                    return new JsonpResult(false, true, "Balança não foi selecionada!");

                Repositorio.Embarcador.Logistica.Pesagem repPesagem = new Repositorio.Embarcador.Logistica.Pesagem(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = repositorioCargaGuarita.BuscarPorCodigo(codigoGuarita);
                Dominio.Entidades.Embarcador.Logistica.Pesagem pesagem = repPesagem.BuscarPorGuarita(codigoGuarita);

                if (guarita == null)
                    return new JsonpResult(false, true, "Guarita não encontrada.");

                if (pesagem?.StatusBalanca == StatusBalanca.AgIntegracao)
                    return new JsonpResult(false, true, "Aguarde a integração processar para gerar/atualizar o registro.");

                unitOfWork.Start();

                if (pesagem == null)
                {
                    pesagem = new Dominio.Entidades.Embarcador.Logistica.Pesagem();
                    pesagem.DataPesagem = DateTime.Now;
                    pesagem.Guarita = guarita;
                }

                pesagem.StatusBalanca = StatusBalanca.AgIntegracao;

                if (pesagem.Codigo > 0)
                    repPesagem.Atualizar(pesagem, Auditado, null, "Atualizou a integração da balança");
                else
                    repPesagem.Inserir(pesagem, Auditado);

                Servicos.Embarcador.Logistica.Pesagem servicoPesagem = new Servicos.Embarcador.Logistica.Pesagem(unitOfWork);
                servicoPesagem.GerarAtualizarIntegracoes(pesagem, codigoBalancaFinal > 0 ? TipoIntegracaoBalanca.PesagemFinal : TipoIntegracaoBalanca.PesagemInicial, SituacaoIntegracao.AgIntegracao, codigoBalancaFinal > 0 ? codigoBalancaFinal : codigoBalancaInicial, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(pesagem.Codigo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao criar a pesagem.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DesbloquearPesagemBalanca()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoGuarita = Request.GetIntParam("CodigoGuarita");
                decimal pesagemFinal = Request.GetDecimalParam("PesagemFinal");

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unitOfWork);
                Repositorio.Embarcador.Logistica.Pesagem repPesagem = new Repositorio.Embarcador.Logistica.Pesagem(unitOfWork);
                Servicos.Embarcador.Integracao.Toledo.IntegracaoToledo svcToledo = new Servicos.Embarcador.Integracao.Toledo.IntegracaoToledo(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = repositorioCargaGuarita.BuscarPorCodigo(codigoGuarita);
                Dominio.Entidades.Embarcador.Logistica.Pesagem pesagem = repPesagem.BuscarPorGuarita(codigoGuarita);

                if (guarita == null)
                    return new JsonpResult(false, true, "Guarita não encontrada.");

                if (pesagem == null)
                    return new JsonpResult(false, true, "Pesagem da guarita não encontrada.");

                unitOfWork.Start();

                if (pesagemFinal > 0)
                {
                    guarita.Initialize();
                    guarita.PesagemFinal = pesagemFinal;
                    repositorioCargaGuarita.Atualizar(guarita, Auditado);

                    pesagem.Initialize();
                    pesagem.PesoFinal = pesagemFinal;
                    repPesagem.Atualizar(pesagem, Auditado);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, pesagem, null, "Solicitou desbloqueio da pesagem.", unitOfWork);

                bool retorno = svcToledo.AplicarManutencaoTicket(2, pesagem);

                unitOfWork.CommitChanges();

                if (!retorno)
                    return new JsonpResult(false, true, "Não foi possível desbloquear o ticket! Tente novamente mais tarde ou verifique no histórico da integração o retorno.");

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao desbloquear a pesagem.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RefazerPesagemBalanca()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoGuarita = Request.GetIntParam("CodigoGuarita");

                Repositorio.Embarcador.Logistica.Pesagem repPesagem = new Repositorio.Embarcador.Logistica.Pesagem(unitOfWork);
                Servicos.Embarcador.Integracao.Toledo.IntegracaoToledo svcToledo = new Servicos.Embarcador.Integracao.Toledo.IntegracaoToledo(unitOfWork);
                //svcToledo.ConsultarTicketPeriodo("20/01/2021".ToDateTime(), "27/01/2021".ToDateTime());

                Dominio.Entidades.Embarcador.Logistica.Pesagem pesagem = repPesagem.BuscarPorGuarita(codigoGuarita);

                if (pesagem == null)
                    return new JsonpResult(false, true, "Pesagem da guarita não encontrada.");

                unitOfWork.Start();

                Servicos.Auditoria.Auditoria.Auditar(Auditado, pesagem, null, "Solicitou operação de refazer a pesagem.", unitOfWork);

                bool retorno = svcToledo.RefazUltimaOperacaoAtiva(pesagem);

                unitOfWork.CommitChanges();

                if (!retorno)
                    return new JsonpResult(false, true, "Não foi possível refazer a pesagem! Tente novamente mais tarde ou verifique no histórico da integração o retorno.");

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao refazer a pesagem.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Integrações

        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Logistica.Pesagem repPesagem = new Repositorio.Embarcador.Logistica.Pesagem(unitOfWork);
                int.TryParse(Request.Params("Codigo"), out int codigo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> integracoesArquivos = repPesagem.BuscarArquivosPorIntegracao(codigo, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repPesagem.ContarBuscarArquivosPorIntegracao(codigo));

                var retorno = (from obj in integracoesArquivos
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                   obj.DescricaoTipo,
                                   obj.Mensagem
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Logistica.Pesagem repPesagem = new Repositorio.Embarcador.Logistica.Pesagem(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = repPesagem.BuscarIntegracaoPorCodigo(codigo);

                if (arquivoIntegracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                if (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null)
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Integração.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download dos xmls de integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Integrar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Logistica.PesagemIntegracao repPesagemIntegracao = new Repositorio.Embarcador.Logistica.PesagemIntegracao(unitOfWork);
                Repositorio.Embarcador.Logistica.Pesagem repPesagem = new Repositorio.Embarcador.Logistica.Pesagem(unitOfWork);

                Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao integracao = repPesagemIntegracao.BuscarPorCodigo(codigo, false);

                if (integracao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                List<TipoIntegracao> tiposIntegracaoPesagem = new List<TipoIntegracao> { TipoIntegracao.Qbit, TipoIntegracao.Deca };

                if (integracao.SituacaoIntegracao != SituacaoIntegracao.ProblemaIntegracao)
                    if (!tiposIntegracaoPesagem.Contains(integracao.TipoIntegracao.Tipo) || integracao.SituacaoIntegracao != SituacaoIntegracao.Integrado)
                        return new JsonpResult(false, true, "Não é possível integrar nessa situação!");

                unitOfWork.Start();

                integracao.DataIntegracao = DateTime.Now;
                integracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                integracao.Pesagem.StatusBalanca = StatusBalanca.AgIntegracao;

                repPesagemIntegracao.Atualizar(integracao);
                repPesagem.Atualizar(integracao.Pesagem);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao.Pesagem, "Solicitou o reenvio da integração", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao integrar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaPesagemIntegracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Situacao", false);
                grid.AdicionarCabecalho("Integração", "TipoIntegracao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo", "TipoIntegracaoBalanca", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tentativas", "NumeroTentativas", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data do Envio", "DataIntegracao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "SituacaoIntegracao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Mensagem", "Retorno", 30, Models.Grid.Align.left, false);

                int codigo = Request.GetIntParam("Codigo");
                SituacaoIntegracao? situacao = Request.GetNullableEnumParam<SituacaoIntegracao>("Situacao");

                Repositorio.Embarcador.Logistica.PesagemIntegracao repPesagemIntegracao = new Repositorio.Embarcador.Logistica.PesagemIntegracao(unitOfWork);
                string propriedadeOrdenar = ObterPropriedadeOrdenarPesquisaSeparacaoPedidoIntegracoes(grid.header[grid.indiceColunaOrdena].data);
                List<Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao> listaIntegracoes = repPesagemIntegracao.Consultar(codigo, situacao, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalIntegracoes = repPesagemIntegracao.ContarConsulta(codigo, situacao);

                var listaIntegracoesRetornar = (
                    from integracao in listaIntegracoes
                    select new
                    {
                        integracao.Codigo,
                        Situacao = integracao.SituacaoIntegracao,
                        SituacaoIntegracao = integracao.DescricaoSituacaoIntegracao,
                        TipoIntegracao = integracao.TipoIntegracao.DescricaoTipo,
                        TipoIntegracaoBalanca = integracao.TipoIntegracaoBalanca.ObterDescricao(),
                        Retorno = integracao.ProblemaIntegracao,
                        integracao.NumeroTentativas,
                        DataIntegracao = integracao.DataIntegracao.ToString("dd/MM/yyyy HH:mm:ss"),
                        DT_RowColor = integracao.SituacaoIntegracao.ObterCorLinha(),
                        DT_FontColor = integracao.SituacaoIntegracao.ObterCorFonte(),
                    }
                ).ToList();

                grid.AdicionaRows(listaIntegracoesRetornar);
                grid.setarQuantidadeTotal(totalIntegracoes);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ProblemaIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                string motivo = Request.GetStringParam("Motivo");

                if (string.IsNullOrWhiteSpace(motivo))
                    return new JsonpResult(false, "O motivo deve ser informado.");

                Repositorio.Embarcador.Logistica.PesagemIntegracao repPesagemIntegracao = new Repositorio.Embarcador.Logistica.PesagemIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao integracao = repPesagemIntegracao.BuscarPorCodigo(codigo, false);

                if (integracao == null)
                    return new JsonpResult(false, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                integracao.DataIntegracao = DateTime.Now;
                integracao.NumeroTentativas += 1;
                integracao.ProblemaIntegracao = motivo.Trim();
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                repPesagemIntegracao.Atualizar(integracao);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterTotaisIntegracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Logistica.PesagemIntegracao repPesagemIntegracao = new Repositorio.Embarcador.Logistica.PesagemIntegracao(unitOfWork);

                int totalAguardandoIntegracao = repPesagemIntegracao.ContarConsulta(codigo, SituacaoIntegracao.AgIntegracao);
                int totalIntegrado = repPesagemIntegracao.ContarConsulta(codigo, SituacaoIntegracao.Integrado);
                int totalProblemaIntegracao = repPesagemIntegracao.ContarConsulta(codigo, SituacaoIntegracao.ProblemaIntegracao);
                int totalAguardandoRetorno = repPesagemIntegracao.ContarConsulta(codigo, SituacaoIntegracao.AgRetorno);

                var retorno = new
                {
                    TotalAguardandoIntegracao = totalAguardandoIntegracao,
                    TotalAguardandoRetorno = totalAguardandoRetorno,
                    TotalIntegrado = totalIntegrado,
                    TotalProblemaIntegracao = totalProblemaIntegracao,
                    TotalGeral = totalAguardandoIntegracao + totalIntegrado + totalProblemaIntegracao + totalAguardandoRetorno
                };

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao obter os totais das integrações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaPesagem ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaPesagem()
            {
                Status = Request.GetEnumParam<StatusBalanca>("StatusBalanca"),
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                CodigoPesagem = Request.GetStringParam("CodigoPesagem"),
                DataPesagemInicial = Request.GetDateTimeParam("DataPesagemInicial"),
                DataPesagemFinal = Request.GetDateTimeParam("DataPesagemFinal"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                CodigoMotorista = Request.GetIntParam("Motorista")
            };
        }

        private string ObterPropriedadeOrdenarPesquisaSeparacaoPedidoIntegracoes(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "TipoIntegracao")
                return "TipoIntegracao.Tipo";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
