using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class CTeIntegracaoRetornoController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult BuscarPorCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoCTe;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);

                Repositorio.CTeIntegracaoRetorno repCTeIntegracaoRetorno = new Repositorio.CTeIntegracaoRetorno(unitOfWork);

                List<Dominio.Entidades.CTeIntegracaoRetorno> listaCTeIntegracaoRetorno = repCTeIntegracaoRetorno.BuscarPorCTe(codigoCTe);

                var retorno = (from obj in listaCTeIntegracaoRetorno
                               orderby obj.Codigo descending
                               select new
                               {
                                   Codigo = obj.Codigo,
                                   Integradora = obj.TipoIntegracao.Descricao,
                                   Status = obj.DescricaoStatus,
                                   Mensagem = !string.IsNullOrWhiteSpace(obj.ProblemaIntegracao) ? obj.ProblemaIntegracao : string.Empty
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Integradora|30", "Status|15", "Mensagem|45" }, retorno.Count);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao obter os dados de integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarLogIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.CTeIntegracaoRetornoLog repCTeIntegracaoRetornoLog = new Repositorio.CTeIntegracaoRetornoLog(unitOfWork);

                List<Dominio.Entidades.CTeIntegracaoRetornoLog> listaCTeIntegracaoRetornoLog = repCTeIntegracaoRetornoLog.BuscarPorRetornoIntegracao(codigo);

                var retorno = (from obj in listaCTeIntegracaoRetornoLog
                               orderby obj.Codigo descending
                               select new
                               {
                                   Codigo = obj.Codigo,
                                   Data = obj.Data.ToString("dd/MM/yyyy HH:mm:ss")
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Data Hora|70" }, retorno.Count);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao obter os dados de log de integrações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ReenviarIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.CTeIntegracaoRetorno repCTeIntegracaoRetorno = new Repositorio.CTeIntegracaoRetorno(unidadeDeTrabalho);

                Dominio.Entidades.CTeIntegracaoRetorno cteIntegracaoRetorno = repCTeIntegracaoRetorno.BuscarPorCodigo(codigo);
                if (cteIntegracaoRetorno == null)
                    return Json<bool>(false, false, "Integração não encontrada.");

                cteIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Aguardando;
                repCTeIntegracaoRetorno.Atualizar(cteIntegracaoRetorno);

                //if (cteIntegracaoRetorno.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MagalogEscrituracao)
                //    Servicos.Embarcador.Integracao.Magalog.IntegracaoMagalog.IntegrarCTeParaEscrituracao(ref cteIntegracaoRetorno, unidadeDeTrabalho);
                //else if (cteIntegracaoRetorno.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Magalog)
                //    Servicos.Embarcador.Integracao.Magalog.IntegracaoMagalog.IntegrarRetornoCTe(ref cteIntegracaoRetorno, unidadeDeTrabalho);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao solicitar a integração.");
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadArquivoLog()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params["Codigo"], out int codigo);
                int.TryParse(Request.Params["Tipo"], out int tipo);

                if (codigo > 0)
                {
                    Repositorio.CTeIntegracaoRetornoLog repCTeIntegracaoRetornoLog = new Repositorio.CTeIntegracaoRetornoLog(unidadeDeTrabalho);

                    Dominio.Entidades.CTeIntegracaoRetornoLog xml = repCTeIntegracaoRetornoLog.BuscarPorCodigo(codigo);

                    if (xml != null)
                    {
                        byte[] data = null;
                        if (tipo == 0)
                            data = System.Text.Encoding.UTF8.GetBytes(xml.Request);
                        else
                            data = System.Text.Encoding.UTF8.GetBytes(xml.Response);

                        if (data != null)
                        {
                            if (tipo == 0)
                                return Arquivo(data, "text/json", string.Concat("Requisicao" + xml.Codigo, ".json"));
                            else
                                return Arquivo(data, "text/json", string.Concat("Resposta" + xml.Codigo, ".json"));
                        }
                        else
                            return Json<bool>(false, false, "Ocorreu uma falha ao carregar json, atualize a página e tente novamente.");
                    }
                    else
                        return Json<bool>(false, false, "Nenhum json salvo.");
                }
                else
                    return Json<bool>(false, false, "Integração não encontrada, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao realizar o download.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

    }
}