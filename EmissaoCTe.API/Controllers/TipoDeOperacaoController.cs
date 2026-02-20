using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class TipoDeOperacaoController : ApiController
    {
        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params["inicioRegistros"], out int inicioRegistros);

                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaTipoOperacao filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaTipoOperacao()
                {
                    Ativo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo,
                    Descricao = Request.Params["Descricao"]
                };

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                {
                    DirecaoOrdenar = "asc",
                    InicioRegistros = inicioRegistros,
                    LimiteRegistros = 50,
                    PropriedadeOrdenar = "Descricao",
                };

                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeDeTrabalho);

                int countTipoOperacao = repositorioTipoOperacao.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> listaTipoOperacao = countTipoOperacao > 0 ? repositorioTipoOperacao.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

                var retorno = from obj in listaTipoOperacao select new { obj.Codigo, obj.Descricao };

                return Json(retorno, true, null, new string[] { "Codigo", "Descrição|80" }, countTipoOperacao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os tipos de cargas.");
            }
        }

        #endregion
    }
}