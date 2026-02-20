using System;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class AtividadeController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult Consulta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                string descricao = Request.Params["Descricao"];
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);
                var listaAtividades = repAtividade.Consulta(descricao, inicioRegistros, 50);
                int countAtividades = repAtividade.ContarConsulta(descricao);

                var retorno = from obj in listaAtividades select new {obj.Codigo, obj.Descricao};
                
                return Json(retorno, true, null, new string[] { "Código|10", "Descrição|80" }, countAtividades);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as atividades. Tente novamente!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
