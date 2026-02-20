using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class IntegracaoMDFeController : Controller
    {
        [AcceptVerbs("POST")]
        public ActionResult AdicionarNaFilaDeConsulta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoMDFe = 0;
                int.TryParse(Request.Params["CodigoMDFe"], out codigoMDFe);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);

                if (mdfe != null)
                {
                    if (mdfe.SistemaEmissor == TipoEmissorDocumento.Integrador)
                        FilaConsultaCTe.GetInstance().QueueItem(2, mdfe.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.MDFe, Conexao.StringConexao);
                    return Json(new { Sucesso = true });
                }
                else
                {
                    return Json(new { Sucesso = false, Erro = "MDF-e não encontrado." });
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json(new { Sucesso = false, Erro = "Ocorreu uma falha ao adicionar o MDF-e na fila de consulta." });
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
