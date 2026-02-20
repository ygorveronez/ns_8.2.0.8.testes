using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class MotoristaMDFeController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult BuscarPorMDFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoMDFe = 0;
                int.TryParse(Request.Params["CodigoMDFe"], out codigoMDFe);

                Repositorio.MotoristaMDFe repMotorista = new Repositorio.MotoristaMDFe(unitOfWork);
                List<Dominio.Entidades.MotoristaMDFe> motoristas = repMotorista.BuscarPorMDFe(codigoMDFe);

                var retorno = from obj in motoristas
                              select new Dominio.ObjetosDeValor.MotoristaMDFe()
                              {
                                  Codigo = obj.Codigo,
                                  CPF = obj.CPF,
                                  Excluir = false,
                                  Nome = obj.Nome
                              };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os motoristas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

    }
}
