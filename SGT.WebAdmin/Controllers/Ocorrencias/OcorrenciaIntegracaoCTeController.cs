using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Ocorrencias
{
    [CustomAuthorize(new string[] { "ConsultarHistoricoIntegracao", "DownloadArquivosHistoricoIntegracao", "ObterTotais" }, "Ocorrencias/Ocorrencia")]
    public class OcorrenciaIntegracaoCTeController : BaseController
    {
		#region Construtores

		public OcorrenciaIntegracaoCTeController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
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

                int codigoOcorrencia;
                int.TryParse(Request.Params("Ocorrencia"), out codigoOcorrencia);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Tipo", false);
                grid.AdicionarCabecalho("CT-e", "CTe", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.Emissor, "Emissor", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.Integracao, "TipoIntegracao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.Tentativas, "Tentativas", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.DataDoEnvio, "DataEnvio", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.Situacao, "Situacao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.Retorno, "Retorno", 20, Models.Grid.Align.left, false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unidadeDeTrabalho);

                int countCTes = repIntegracao.ContarConsulta(codigoOcorrencia, situacao, tipo);

                List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao> listaIntegracao = new List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao>();

                if (countCTes > 0)
                {
                    if (propOrdena == "CTe")
                        propOrdena = "CargaCTe.CTe.Numero";
                    else if (propOrdena == "Emissor")
                        propOrdena = "CargaCTe.CTe.Empresa.CNPJ";
                    else if (propOrdena == "TipoIntegracao")
                        propOrdena = "TipoIntegracao.Tipo";
                    else if (propOrdena == "Tentativas")
                        propOrdena = "NumeroTentativas";
                    else if (propOrdena == "DataEnvio")
                        propOrdena = "DataIntegracao";
                    else if (propOrdena == "Situacao")
                        propOrdena = "SituacaoIntegracao";

                    listaIntegracao = repIntegracao.Consultar(codigoOcorrencia, situacao, tipo, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                }


                grid.setarQuantidadeTotal(countCTes);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> cargaPedidosXMLsNotaFiscalCTe = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();
                if (listaIntegracao.Any(obj => obj.CargaCTe.CTe == null))
                    cargaPedidosXMLsNotaFiscalCTe = repCargaPedidoXMLNotaFiscalCTe.BuscarPorCargaPedidoXMLNotaFiscalCTePorCargaCTe((from obj in listaIntegracao where obj.CargaCTe.CTe == null select obj.CargaCTe.Codigo).ToList());

                grid.AdicionaRows((from obj in listaIntegracao
                                   select new
                                   {
                                       obj.Codigo,
                                       obj.TipoIntegracao.Tipo,
                                       CTe = obj.CargaCTe.CTe?.Numero.ToString() ?? (string.Join(", ", (from nf in cargaPedidosXMLsNotaFiscalCTe where nf.CargaCTe.Codigo == obj.CargaCTe.Codigo select nf.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero).Distinct().ToList()) + " (Pré CT-e)"),
                                       Emissor = obj.CargaCTe.CTe?.Empresa.CNPJ_Formatado ?? obj.CargaCTe.PreCTe.Empresa.CNPJ_Formatado,
                                       Situacao = obj.DescricaoSituacaoIntegracao,
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
                int codigoOcorrencia;
                int.TryParse(Request.Params("Ocorrencia"), out codigoOcorrencia);

                Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repCargaEDIIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(unidadeDeTrabalho);

                int totalAguardandoIntegracao = repCargaEDIIntegracao.ContarPorOcorrencia(codigoOcorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao);
                int totalIntegrado = repCargaEDIIntegracao.ContarPorOcorrencia(codigoOcorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado);
                int totalProblemaIntegracao = repCargaEDIIntegracao.ContarPorOcorrencia(codigoOcorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao);
                int totalAguardandoRetorno = repCargaEDIIntegracao.ContarPorOcorrencia(codigoOcorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno);

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

                return new JsonpResult(false, Localization.Resources.Ocorrencias.Ocorrencia.OcorreuUmaFalhaAoObterOsTotaisDasIntegracoesDeCtE);
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

                Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao OcorrenciaCTeIntegracao = repOcorrenciaCTeIntegracao.BuscarPorCodigo(codigo);

                if (OcorrenciaCTeIntegracao == null)
                    return new JsonpResult(false, Localization.Resources.Ocorrencias.Ocorrencia.IntegracaoNaoEncontrada);

                if (OcorrenciaCTeIntegracao.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura)
                    return new JsonpResult(false, Localization.Resources.Ocorrencias.Ocorrencia.NaoEPossivelEnviarAsIntegracoesDaNaturaIndividualmenteUtilizeAOpcaoReenviarTodos);

                OcorrenciaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                OcorrenciaCTeIntegracao.Lote = null;

                repOcorrenciaCTeIntegracao.Atualizar(OcorrenciaCTeIntegracao);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, OcorrenciaCTeIntegracao, null, Localization.Resources.Ocorrencias.Ocorrencia.ReenviouAIntegracao, unidadeDeTrabalho);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Ocorrencias.Ocorrencia.OcorreuUmaFalhaAoEnviarAIntegracao);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> AnteciparEnvio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                int numeroTentativas = 2;
                double minutosACadaTentativa = 5;
                int numeroRegistrosPorVez = 15;

                Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(unitOfWork);

                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao OcorrenciaCTeIntegracao = repOcorrenciaCTeIntegracao.BuscarPorCodigo(codigo);

                if (OcorrenciaCTeIntegracao == null)
                    return new JsonpResult(false, Localization.Resources.Ocorrencias.Ocorrencia.IntegracaoNaoEncontrada);

                if (OcorrenciaCTeIntegracao.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura)
                    return new JsonpResult(false, Localization.Resources.Ocorrencias.Ocorrencia.NaoEPossivelEnviarAsIntegracoesDaNaturaIndividualmenteUtilizeAOpcaoReenviarTodos);

                OcorrenciaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                OcorrenciaCTeIntegracao.Lote = null;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, OcorrenciaCTeIntegracao, null, Localization.Resources.Ocorrencias.Ocorrencia.ReenviouAIntegracao, unitOfWork);


                List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao> ocorrenciaCTeIntegracoesPendente = repOcorrenciaCTeIntegracao.BuscarCTeIntegracaoPendente(numeroTentativas, minutosACadaTentativa, "Codigo", "asc", numeroRegistrosPorVez, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioIntegracao.Individual);

                foreach (Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracaoPendente in ocorrenciaCTeIntegracoesPendente)
                {
                    switch (ocorrenciaCTeIntegracaoPendente.TipoIntegracao.Tipo)
                    {
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Frimesa:
                            new Servicos.Embarcador.Integracao.Frimesa.IntegracaoFrimesa(unitOfWork).IntegrarOcorrencia(ocorrenciaCTeIntegracaoPendente);
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Camil:
                            new Servicos.Embarcador.Integracao.Camil.IntegracaoCamil(unitOfWork).IntegrarCargaOcorrencia(ocorrenciaCTeIntegracaoPendente);
                            break;

                        default:
                            break;
                    }

                    repOcorrenciaCTeIntegracao.Atualizar(OcorrenciaCTeIntegracao);
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Ocorrencias.Ocorrencia.OcorreuUmaFalhaAoEnviarAIntegracao);
            }
            finally
            {
                unitOfWork.Dispose();
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

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoAux;
                if (Enum.TryParse(Request.Params("Tipo"), out tipoAux))
                    tipo = tipoAux;

                int codigoOcorrencia = 0;
                int.TryParse(Request.Params("Ocorrencia"), out codigoOcorrencia);

                Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(unidadeDeTrabalho);

                List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao> integracoes = repOcorrenciaCTeIntegracao.BuscarPorOcorrencia(codigoOcorrencia, situacao, tipo);

                bool utilizarTransacao = false;
                if (integracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura))
                    utilizarTransacao = true;

                if (utilizarTransacao)
                    unidadeDeTrabalho.Start();

                foreach (Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao integracao in integracoes)
                {
                    if (!utilizarTransacao)
                    {
                        unidadeDeTrabalho.FlushAndClear();
                    }

                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                    integracao.Lote = null;

                    repOcorrenciaCTeIntegracao.Atualizar(integracao);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao, null, Localization.Resources.Ocorrencias.Ocorrencia.ReenviouAIntegracao, unidadeDeTrabalho);
                }

                if (utilizarTransacao)
                    unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Ocorrencias.Ocorrencia.OcorreuUmaFalhaAoEnviarAIntegracao);
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

                Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(unidadeDeTrabalho);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.Data, "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.Tipo, "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.Retorno, "Mensagem", 40, Models.Grid.Align.left, false);

                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao integracao = repIntegracao.BuscarPorCodigo(codigo);
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

                Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao integracao = repIntegracao.BuscarPorCodigoArquivo(codigo);

                if (integracao == null)
                    return new JsonpResult(true, false, Localization.Resources.Ocorrencias.Ocorrencia.HistoricoNaoEncontrado);

                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo arquivoIntegracao = integracao.ArquivosTransacao.Where(o => o.Codigo == codigo).FirstOrDefault();

                if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, Localization.Resources.Ocorrencias.Ocorrencia.NaoHaRegistrosDeArquivosSalvosParaEsteHistoricoDeConsulta);

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Consulta CT-e " + (integracao.CargaCTe.CTe?.Numero ?? integracao.CargaCTe.PreCTe?.Codigo) + ".zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, Localization.Resources.Ocorrencias.Ocorrencia.OcorreuUmaFalhaAoRealizarODownloadDoXmlDeIntegracao);
            }
        }
    }
}
