using System;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class NaturezaCargaANTTController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult BuscarTodos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                bool buscarSemOpcaoZero = false;
                bool.TryParse(Request.Params["buscarSemOpcaoZero"], out buscarSemOpcaoZero);

                Repositorio.NaturezaCargaANTT repNaturezaCargaANTT = new Repositorio.NaturezaCargaANTT(unitOfWork);

                var listaNaturezas = repNaturezaCargaANTT.BuscarTodos(buscarSemOpcaoZero);

                var result = from obj in listaNaturezas select new { obj.Codigo, Numero = obj.CodigoNatureza, Descricao = string.IsNullOrWhiteSpace(obj.Descricao) ? string.Empty : obj.Descricao };

                return Json(result, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as naturezas das cargas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

    }
}
