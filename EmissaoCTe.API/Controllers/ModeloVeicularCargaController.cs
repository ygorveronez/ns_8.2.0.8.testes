using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class ModeloVeicularCargaController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                string descricao = Request.Params["Descricao"];

                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModelo = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

                var listaModelo = repModelo.Consultar(null, descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo, null, null, new List<int>(), "Descricao", "asc", inicioRegistros, 50);
                int countModelo = repModelo.ContarConsulta(null, descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo, null, null, new List<int>());

                var retorno = from obj in listaModelo select new { obj.Codigo, obj.Descricao };

                return Json(retorno, true, null, new string[] { "Código", "Descrição|80" }, countModelo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os modelos veiculares de carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}