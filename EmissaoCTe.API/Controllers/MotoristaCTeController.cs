using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class MotoristaCTeController : ApiController 
    {
        public ActionResult BuscarPorCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoCTe = 0;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);
                if (codigoCTe > 0)
                {
                    Repositorio.MotoristaCTE repMotoristaCTe = new Repositorio.MotoristaCTE(unitOfWork);
                    List<Dominio.Entidades.MotoristaCTE> motoristas = repMotoristaCTe.BuscarPorCTe(this.EmpresaUsuario.Codigo, codigoCTe);
                    var retorno = from obj in motoristas
                                  select new Dominio.ObjetosDeValor.Motorista()
                                  {
                                      Codigo = obj.Codigo,
                                      CPF = obj.CPFMotorista,
                                      Nome = obj.NomeMotorista,
                                      Excluir = false
                                  };
                    return Json(retorno, true);
                }
                else
                {
                    return Json<bool>(true, false, "Parâmetros inválidos.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as informações de motoristas do CTe.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
