using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class MDFeIntegracaoRetornoController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult BuscarPorMDFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoMDFe;
                int.TryParse(Request.Params["CodigoMDFe"], out codigoMDFe);

                Repositorio.MDFeIntegracaoRetorno repMDFeIntegracaoRetorno = new Repositorio.MDFeIntegracaoRetorno(unitOfWork);

                List<Dominio.Entidades.MDFeIntegracaoRetorno> listaMDFeIntegracaoRetorno = repMDFeIntegracaoRetorno.BuscarPorMDFe(codigoMDFe);

                var retorno = (from obj in listaMDFeIntegracaoRetorno
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

                Repositorio.MDFeIntegracaoRetornoLog repMDFeIntegracaoRetornoLog = new Repositorio.MDFeIntegracaoRetornoLog(unitOfWork);

                List<Dominio.Entidades.MDFeIntegracaoRetornoLog> listaMDFeIntegracaoRetornoLog = repMDFeIntegracaoRetornoLog.BuscarPorRetornoIntegracao(codigo);

                var retorno = (from obj in listaMDFeIntegracaoRetornoLog
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

                Repositorio.MDFeIntegracaoRetorno repMDFeIntegracaoRetorno = new Repositorio.MDFeIntegracaoRetorno(unidadeDeTrabalho);

                Dominio.Entidades.MDFeIntegracaoRetorno MDFeIntegracaoRetorno = repMDFeIntegracaoRetorno.BuscarPorCodigo(codigo);
                if (MDFeIntegracaoRetorno == null)
                    return Json<bool>(false, false, "Integração não encontrada.");

                MDFeIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Aguardando;
                repMDFeIntegracaoRetorno.Atualizar(MDFeIntegracaoRetorno);

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
                    Repositorio.MDFeIntegracaoRetornoLog repMDFeIntegracaoRetornoLog = new Repositorio.MDFeIntegracaoRetornoLog(unidadeDeTrabalho);

                    Dominio.Entidades.MDFeIntegracaoRetornoLog xml = repMDFeIntegracaoRetornoLog.BuscarPorCodigo(codigo);

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