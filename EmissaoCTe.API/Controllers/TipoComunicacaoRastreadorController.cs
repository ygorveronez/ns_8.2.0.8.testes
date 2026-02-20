using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class TipoComunicacaoRastreadorController : ApiController
    {

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                string descricao = Request.Params["Descricao"];

                Repositorio.Embarcador.Veiculos.TipoComunicacaoRastreador repTipoComunicacaoRastreador = new Repositorio.Embarcador.Veiculos.TipoComunicacaoRastreador(unitOfWork);
                List<Dominio.Entidades.Embarcador.Veiculos.TipoComunicacaoRastreador> tipos = repTipoComunicacaoRastreador.ConsultarAtivos(descricao);

                var retorno = from obj in tipos select new {
                    obj.Codigo,
                    obj.Descricao
                };

                return Json(retorno, true, null, new string[] { "Código", "Descrição|80"}, tipos.Count);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os tipos de comunicação do rastreador.");
            }
        }

        #endregion

    }
}
