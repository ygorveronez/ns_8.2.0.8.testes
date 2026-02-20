using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class EspecieDocumentoFiscalController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult BuscarTodos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.EspecieDocumentoFiscal repEspecie = new Repositorio.EspecieDocumentoFiscal(unitOfWork);

                List<Dominio.Entidades.EspecieDocumentoFiscal> especies = repEspecie.BuscarTodos();

                var result = from obj in especies select new { obj.Codigo, obj.Sigla, obj.Descricao };

                return Json(result, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as Espécies de Documentos Fiscais. Tente novamente!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
