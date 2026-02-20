using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class PercursoMDFeController : ApiController
    {

        [AcceptVerbs("POST")]
        public ActionResult BuscarPorMDFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoMDFe = 0;
                int.TryParse(Request.Params["CodigoMDFe"], out codigoMDFe);

                Repositorio.PercursoMDFe repPercurso = new Repositorio.PercursoMDFe(unitOfWork);
                List<Dominio.Entidades.PercursoMDFe> percursos = repPercurso.BuscarPorMDFe(codigoMDFe);

                var retorno = from obj in percursos
                              select new Dominio.ObjetosDeValor.PercursoMDFe()
                              {
                                  Codigo = obj.Codigo,
                                  Data = obj.DataInicioViagem.HasValue ? obj.DataInicioViagem.Value.ToString("dd/MM/yyyy") : string.Empty,
                                  Descricao = obj.Estado.Nome,
                                  Excluir = false,
                                  Hora = obj.DataInicioViagem.HasValue ? obj.DataInicioViagem.Value.ToString("HH:mm") : string.Empty,
                                  Sigla = obj.Estado.Sigla
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
