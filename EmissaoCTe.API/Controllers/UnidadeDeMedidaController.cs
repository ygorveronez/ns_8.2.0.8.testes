using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class UnidadeDeMedidaController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                string descricao = Request.Params["Descricao"];
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                Repositorio.UnidadeDeMedida repUnidadeDeMedida = new Repositorio.UnidadeDeMedida(unitOfWork);

                List<Dominio.Entidades.UnidadeDeMedida> listaUnidadeMedida = repUnidadeDeMedida.Consultar(descricao, inicioRegistros, 50);
                int countUnidadeMedida = repUnidadeDeMedida.ContarConsulta(descricao);

                var retorno = from obj in listaUnidadeMedida select new { obj.Codigo, obj.CodigoDaUnidade, obj.Sigla, obj.Descricao };

                return Json(retorno, true, null, new string[] { "Codigo", "Código|15", "Sigla|15", "Descrição|60" }, countUnidadeMedida);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar as unidades de medida.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarTodos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.UnidadeDeMedida repUnidadeMedida = new Repositorio.UnidadeDeMedida(unitOfWork);

                List<Dominio.Entidades.UnidadeDeMedida> unidades = repUnidadeMedida.BuscarTodos("A");

                var retorno = from obj in unidades select new { obj.Codigo, obj.Sigla, obj.Descricao };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter as unidades de medidas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
