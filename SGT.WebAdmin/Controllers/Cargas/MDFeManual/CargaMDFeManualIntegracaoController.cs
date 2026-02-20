using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Cargas.Cancelamento
{
    [CustomAuthorize(new string[] { "ObterDadosIntegracoes" }, "Cargas/CargaMDFeManual")]
    public class CargaMDFeManualIntegracaoController : BaseController
    {
        #region Construtores

        public CargaMDFeManualIntegracaoController(Conexao conexao) : base(conexao) { }

        #endregion

        public async Task<IActionResult> ObterDadosIntegracoes()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCargaMDFeManual;
                int.TryParse(Request.Params("CargaMDFeManual"), out codigoCargaMDFeManual);

                Repositorio.Embarcador.Cargas.CargaMDFeManualIntegracao repCargaMDFeManualIntegracao = new Repositorio.Embarcador.Cargas.CargaMDFeManualIntegracao(unidadeDeTrabalho);

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracoesCTe = repCargaMDFeManualIntegracao.BuscarTipoIntegracaoPorCargaMDFeManual(codigoCargaMDFeManual);

                return new JsonpResult(new
                {
                    TiposIntegracoesCTe = tiposIntegracoesCTe,
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Cargas.CancelamentoCarga.OcorreuUmaFalhaObterDadosIntegracoes);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Finalizar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCargaMDFeManual;
                int.TryParse(Request.Params("CargaMDFeManual"), out codigoCargaMDFeManual);

                Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual = repCargaMDFeManual.BuscarPorCodigo(codigoCargaMDFeManual, true);

                if (cargaMDFeManual == null)
                    return new JsonpResult(true, false, "CT-e agrupado não encontrado");

                if (cargaMDFeManual.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.AgIntegracao && cargaMDFeManual.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.FalhaIntegracao)
                    return new JsonpResult(true, false, "Situacão do Cancelamento não Permite Finalização");

                unidadeDeTrabalho.Start();

                cargaMDFeManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.Finalizado;

                repCargaMDFeManual.Atualizar(cargaMDFeManual);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaMDFeManual, Localization.Resources.Cargas.CancelamentoCarga.LiberouEtapaIntegracao, unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Cargas.CancelamentoCarga.OcorreuUmaFalhaFinalizarEtapa);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoAux;
                if (Enum.TryParse(Request.Params("Situacao"), out situacaoAux))
                    situacao = situacaoAux;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoAux;
                if (Enum.TryParse(Request.Params("Tipo"), out tipoAux))
                    tipo = tipoAux;

                int codigoCargaMDFeManual;
                int.TryParse(Request.Params("CargaMDFeManual"), out codigoCargaMDFeManual);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Tipo", false);
                grid.AdicionarCabecalho("SituacaoIntegracao", false);
                grid.AdicionarCabecalho("MDF-e", "MDFe", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Integracao, "TipoIntegracao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCarga.Tentativas, "Tentativas", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCarga.DataEnvio, "DataEnvio", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "Situacao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCarga.Retorno, "Retorno", 20, Models.Grid.Align.left, false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Cargas.CargaMDFeManualIntegracao repCargaMDFeManualIntegracao = new Repositorio.Embarcador.Cargas.CargaMDFeManualIntegracao(unidadeDeTrabalho);

                int countCTes = repCargaMDFeManualIntegracao.ContarConsulta(codigoCargaMDFeManual, situacao, tipo);

                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao> listaIntegracao = new List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao>();

                if (countCTes > 0)
                {
                    if (propOrdena == "TipoIntegracao")
                        propOrdena = "TipoIntegracao.Tipo";
                    else if (propOrdena == "Tentativas")
                        propOrdena = "NumeroTentativas";
                    else if (propOrdena == "DataEnvio")
                        propOrdena = "DataIntegracao";
                    else if (propOrdena == "Situacao")
                        propOrdena = "SituacaoIntegracao";

                    listaIntegracao = repCargaMDFeManualIntegracao.Consultar(codigoCargaMDFeManual, situacao, tipo, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                }

                grid.setarQuantidadeTotal(countCTes);

                grid.AdicionaRows((from obj in listaIntegracao
                                   select new
                                   {
                                       obj.Codigo,
                                       obj.TipoIntegracao.Tipo,
                                       MDFe = obj.CargaMDFeManual.MDFeManualMDFes?.FirstOrDefault()?.MDFe.Numero.ToString(),
                                       Situacao = obj.DescricaoSituacaoIntegracao,
                                       obj.SituacaoIntegracao,
                                       TipoIntegracao = obj.TipoIntegracao.DescricaoTipo,
                                       Retorno = obj.ProblemaIntegracao,
                                       Tentativas = obj.NumeroTentativas,
                                       DataEnvio = obj.DataIntegracao.ToString("dd/MM/yyyy HH:mm:ss"),
                                       DT_RowColor = obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Verde :
                                                     obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Vermelho :
                                                     obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Amarelo :
                                                     Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Azul,
                                       DT_FontColor = obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Branco : "",
                                   }).ToList());

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ObterTotais()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCargaMDFeManual;
                int.TryParse(Request.Params("CargaMDFeManual"), out codigoCargaMDFeManual);

                Repositorio.Embarcador.Cargas.CargaMDFeManualIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.CargaMDFeManualIntegracao(unidadeDeTrabalho);

                int totalAguardandoIntegracao = repIntegracao.ContarPorCargaMDFeManual(codigoCargaMDFeManual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao);
                int totalIntegrado = repIntegracao.ContarPorCargaMDFeManual(codigoCargaMDFeManual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado);
                int totalProblemaIntegracao = repIntegracao.ContarPorCargaMDFeManual(codigoCargaMDFeManual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao);
                int totalAguardandoRetorno = repIntegracao.ContarPorCargaMDFeManual(codigoCargaMDFeManual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno);

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
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoObterOsTotaisDasIntegracoes);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Reenviar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.CargaMDFeManualIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.CargaMDFeManualIntegracao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao cargaMDFeManualIntegracao = repIntegracao.BuscarPorCodigo(codigo);

                if (cargaMDFeManualIntegracao == null)
                    return new JsonpResult(false, "Integração não encontrada");

                unidadeDeTrabalho.Start();

                cargaMDFeManualIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaMDFeManualIntegracao, null, "Reenviou Integracao", unidadeDeTrabalho);

                repIntegracao.Atualizar(cargaMDFeManualIntegracao);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu Uma Falha ao Enviar a Integração");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ReenviarTodos()
        {

            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoAux;
                if (Enum.TryParse(Request.Params("Situacao"), out situacaoAux))
                    situacao = situacaoAux;

                int codigoCargaMDFeManual;
                int.TryParse(Request.Params("CargaMDFeManual"), out codigoCargaMDFeManual);

                Repositorio.Embarcador.Cargas.CargaMDFeManualIntegracao repCargaMDfeManualIntegracao = new Repositorio.Embarcador.Cargas.CargaMDFeManualIntegracao(unidadeDeTrabalho);

                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao> integracoes = repCargaMDfeManualIntegracao.BuscarPorCargaMDFeManual(codigoCargaMDFeManual, situacao);

                unidadeDeTrabalho.Start();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao integracao in integracoes)
                {
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                    repCargaMDfeManualIntegracao.Atualizar(integracao);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao, null, "Reenviou Integracao", unidadeDeTrabalho);
                }
                /*
                if (integracoes.Count > 0)
                {
                    if (cargaCancelamento.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento)
                    {
                        cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.EmCancelamento;

                        repCargaCancelamento.Atualizar(cargaCancelamento);
                    }
                }
                */
                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Cargas.CancelamentoCarga.OcorreuUmaFalhaEnviarIntegração);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.CargaMDFeManualIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.CargaMDFeManualIntegracao(unidadeDeTrabalho);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao integracao = repIntegracao.BuscarPorCodigo(codigo, false);

                grid.setarQuantidadeTotal(integracao.ArquivosTransacao.Count());

                var retorno = (from obj in integracao.ArquivosTransacao.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
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
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.CargaMDFeManualIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.CargaMDFeManualIntegracao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao integracao = repIntegracao.BuscarPorCodigoArquivo(codigo);

                if (integracao == null)
                    return new JsonpResult(true, false, "Historico Não Encontrado");

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = integracao.ArquivosTransacao.Where(o => o.Codigo == codigo).FirstOrDefault();

                if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, Localization.Resources.Cargas.CancelamentoCarga.NaoHaRegistrosParaEsseHistorico);

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", Localization.Resources.Cargas.CancelamentoCarga.ArquivosIntegracaoCancelamentoCTe + integracao.CargaMDFeManual.MDFeManualMDFes?.FirstOrDefault()?.MDFe.Numero.ToString() + ".zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoRealizarDownload);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }
    }
}
