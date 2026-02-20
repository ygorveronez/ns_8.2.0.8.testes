using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class ComponentePrestacaoCTeController : ApiController
    {
        #region Métodos Publicos

        public ActionResult BuscarPorCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoCTe = 0;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);

                if (codigoCTe <= 0)
                    return Json<bool>(false, false, "Parâmetros inválidos.");

                Repositorio.ComponentePrestacaoCTE repositorioComponentePrestacaoCTe = new Repositorio.ComponentePrestacaoCTE(unitOfWork);
                List<Dominio.Entidades.ComponentePrestacaoCTE> componentes = repositorioComponentePrestacaoCTe.BuscarPorCTe(this.EmpresaUsuario.Codigo, codigoCTe);
                var retorno = from obj in componentes
                                select new Dominio.ObjetosDeValor.ComponenteDaPrestacao
                                {
                                    Id = obj.Codigo,
                                    Descricao = obj.Nome,
                                    Valor = obj.Valor,
                                    IncluiBaseCalculoICMS = obj.IncluiNaBaseDeCalculoDoICMS,
                                    IncluiValorAReceber = obj.IncluiNoTotalAReceber,
                                    Excluir = false
                                };
                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os componentes da prestação do CTe.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
