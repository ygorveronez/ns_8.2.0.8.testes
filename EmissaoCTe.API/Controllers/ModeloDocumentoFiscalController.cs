using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class ModeloDocumentoFiscalController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult BuscarTodos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(unitOfWork);

                List<Dominio.Entidades.ModeloDocumentoFiscal> modelos = repModelo.BuscarTodos();

                var result = from obj in modelos select new { obj.Codigo, obj.Numero, obj.Descricao };

                return Json(result, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as Modelos de Documentos Fiscais. Tente novamente!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
