using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class PaisController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult BuscarTodos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.Pais repPais = new Repositorio.Pais(unitOfWork);

                List<Dominio.Entidades.Pais> listaPaises = repPais.BuscarTodos();

                var retorno = from obj in listaPaises select new { obj.Sigla, obj.Nome };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os dados dos países!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
