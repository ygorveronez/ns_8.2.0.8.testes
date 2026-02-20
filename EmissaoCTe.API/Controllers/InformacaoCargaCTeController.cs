using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class InformacaoCargaCTeController : ApiController
    {
        #region Métodos Publicos

        public InformacaoCargaCTeController()
        {
        }

        public ActionResult BuscarPorCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoCTe = 0;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);
                if (codigoCTe > 0)
                {
                    Repositorio.InformacaoCargaCTE repositorioInformacaoCargaCTe = new Repositorio.InformacaoCargaCTE(unitOfWork);
                    List<Dominio.Entidades.InformacaoCargaCTE> informacoes = repositorioInformacaoCargaCTe.BuscarPorCTe(this.EmpresaUsuario.Codigo, codigoCTe);
                    var retorno = (from obj in informacoes
                                  select new
                                  {
                                      Id = obj.Codigo,
                                      obj.Quantidade,
                                      TipoUnidade = obj.Tipo,
                                      UnidadeMedida = int.Parse(obj.UnidadeMedida),
                                      obj.DescricaoUnidadeMedida,
                                      Excluir = false
                                  }).ToList();
                    return Json(retorno, true);
                }
                else
                {
                    return Json<bool>(false, false, "Parâmetros inválidos.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as informações da carga do CTe.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
