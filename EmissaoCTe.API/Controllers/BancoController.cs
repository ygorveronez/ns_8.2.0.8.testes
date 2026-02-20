using System;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class BancoController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int numero, inicioRegistros;
                int.TryParse(Request.Params["Numero"], out numero);
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                string descricao = Request.Params["Descricao"];

                Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);

                var bancos = repBanco.Consultar(numero, descricao, inicioRegistros, 50);
                int countBancos = repBanco.ContarConsulta(numero, descricao);

                return Json(bancos, true, null, new string[] { "Codigo", "Número|20", "Descrição|60" }, countBancos);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os bancos. Tente novamente!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
