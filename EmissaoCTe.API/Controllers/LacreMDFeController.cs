using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class LacreMDFeController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult BuscarPorMDFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoMDFe = 0;
                int.TryParse(Request.Params["CodigoMDFe"], out codigoMDFe);

                Repositorio.LacreMDFe repLacre = new Repositorio.LacreMDFe(unitOfWork);
                List<Dominio.Entidades.LacreMDFe> lacres = repLacre.BuscarPorMDFe(codigoMDFe);

                var retorno = from obj in lacres
                              select new Dominio.ObjetosDeValor.LacreMDFe()
                              {
                                  Codigo = obj.Codigo,
                                  Excluir = false,
                                  Numero = obj.Numero
                              };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os lacres.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

    }
}
