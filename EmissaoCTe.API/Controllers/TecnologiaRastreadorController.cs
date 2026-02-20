using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class TecnologiaRastreadorController : ApiController
    {

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                string descricao = Request.Params["Descricao"];

                Repositorio.Embarcador.Veiculos.TecnologiaRastreador repTecnologiaRastreador = new Repositorio.Embarcador.Veiculos.TecnologiaRastreador(unitOfWork);
                List<Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreador> tecnologias = repTecnologiaRastreador.ConsultarAtivos(descricao);

                var retorno = from obj in tecnologias select new {
                    obj.Codigo,
                    obj.Descricao
                };

                return Json(retorno, true, null, new string[] { "Código", "Descrição|80"}, tecnologias.Count);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as tecnologias do rastreador.");
            }
        }

        #endregion

    }
}
