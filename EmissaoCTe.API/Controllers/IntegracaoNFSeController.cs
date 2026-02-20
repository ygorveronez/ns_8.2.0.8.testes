using System;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class IntegracaoNFSeController : Controller
    {
        [AcceptVerbs("POST")]
        public ActionResult AdicionarNaFilaDeConsulta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoNFSe = 0;
                int.TryParse(Request.Params["CodigoNFSe"], out codigoNFSe);

                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unitOfWork);
                Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorCodigo(codigoNFSe);

                if (nfse != null)
                {
                    FilaConsultaCTe.GetInstance().QueueItem(3, nfse.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.NFSe, Conexao.StringConexao);
                    return Json(new { Sucesso = true });
                }
                else
                {
                    return Json(new { Sucesso = false, Erro = "NFS-e não encontrada." });
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json(new { Sucesso = false, Erro = "Ocorreu uma falha ao adicionar a NFS-e na fila de consulta." });
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
