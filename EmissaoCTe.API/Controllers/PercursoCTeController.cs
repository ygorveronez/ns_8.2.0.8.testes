using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class PercursoCTeController : ApiController
    {

        [AcceptVerbs("POST")]
        public ActionResult BuscarPorCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoCTe = 0;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);

                Repositorio.PercursoCTe repPercurso = new Repositorio.PercursoCTe(unitOfWork);
                List<Dominio.Entidades.PercursoCTe> percursos = repPercurso.BuscarPorCTe(codigoCTe);

                var retorno = from obj in percursos
                              select new Dominio.ObjetosDeValor.PercursoCTe()
                              {
                                  Codigo = obj.Codigo,
                                  Sigla = obj.Estado.Sigla,
                                  Descricao = obj.Estado.Nome,
                                  Excluir = false
                              };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os percursos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

    }
}
