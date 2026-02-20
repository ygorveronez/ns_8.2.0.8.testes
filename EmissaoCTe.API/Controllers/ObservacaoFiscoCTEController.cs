using System;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class ObservacaoFiscoCTEController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult BuscarPorCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoCTe;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);
                if (codigoCTe > 0)
                {
                    Repositorio.ObservacaoFiscoCTE repObservacaoFiscoCTE = new Repositorio.ObservacaoFiscoCTE(unitOfWork);
                    var listaObservacoes = repObservacaoFiscoCTE.BuscarPorCTe(this.EmpresaUsuario.Codigo, codigoCTe);
                    var retorno = from obj in listaObservacoes select new { obj.Codigo, obj.Descricao, obj.Identificador, Excluir = false };
                    return Json(retorno, true);
                }
                else
                {
                    return Json<bool>(false, false, "Parâmetros inválidos!");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as observações do fisco!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

    }
}
