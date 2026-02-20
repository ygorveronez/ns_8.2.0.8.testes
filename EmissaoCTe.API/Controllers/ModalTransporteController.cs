using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class ModalTransporteController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult BuscarTodas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.ModalTransporte repModalTransporte = new Repositorio.ModalTransporte(unitOfWork);
                List<Dominio.Entidades.ModalTransporte> modalidades = repModalTransporte.BuscarTodos();

                var retorno = from obj in modalidades select new { obj.Codigo, obj.Numero, obj.Descricao };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter as modalidades de transporte.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
