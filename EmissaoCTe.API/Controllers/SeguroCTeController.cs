using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class SeguroCTeController : ApiController
    {
        #region Métodos Publicos

        public ActionResult BuscarPorCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoCTe = 0;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);
                if (codigoCTe > 0)
                {
                    Repositorio.SeguroCTE repositorioSeguroCTe = new Repositorio.SeguroCTE(unitOfWork);
                    List<Dominio.Entidades.SeguroCTE> seguros = repositorioSeguroCTe.BuscarPorCTe(this.EmpresaUsuario.Codigo, codigoCTe);
                    var retorno = from obj in seguros
                                  select new
                                  {
                                      Id = obj.Codigo,
                                      Seguradora = obj.NomeSeguradora,
                                      obj.NumeroApolice,
                                      obj.CNPJSeguradora,
                                      NumeroAverberacao = obj.NumeroAverbacao,
                                      Responsavel = obj.Tipo,
                                      DescricaoResponsavel = obj.DescricaoTipo,
                                      ValorMercadoria = obj.Valor,
                                      Excluir = false
                                  };
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
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as informações de seguro do CTe.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
