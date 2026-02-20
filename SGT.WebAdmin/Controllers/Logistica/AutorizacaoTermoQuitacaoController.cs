using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize(new string[] { "RegrasAprovacao" }, "Logistica/AutorizacaoTermoQuitacao")]
    public class AutorizacaoTermoQuitacaoController : RegraAutorizacao.AutorizacaoController<
        Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.AprovacaoAlcadaTermoQuitacao,
        Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.RegraAutorizacaoTermoQuitacao,
        Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao
    >
    {
		#region Construtores

		public AutorizacaoTermoQuitacaoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Globais Sobrescritos

		public override IActionResult BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.TermoQuitacao repositorio = new Repositorio.Embarcador.Logistica.TermoQuitacao(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao termoQuitacao = repositorio.BuscarPorCodigo(codigo);

                if (termoQuitacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    termoQuitacao.Codigo,
                    DataBase = termoQuitacao.DataBase.ToString("dd/MM/yyyy"),
                    termoQuitacao.Numero,
                    Transportador = termoQuitacao.Transportador?.Descricao,
                    termoQuitacao.Situacao,
                    SituacaoDescricao = termoQuitacao.Situacao.ObterDescricao()
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaTermoQuitacaoAprovacao ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaTermoQuitacaoAprovacao filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaTermoQuitacaoAprovacao()
            {
                CodigoTransportador = Request.GetIntParam("Transportador"),
                CodigoUsuario = Request.GetIntParam("Usuario"),
                DataBaseInicial = Request.GetNullableDateTimeParam("DataBaseInicial"),
                DataBaseLimite = Request.GetNullableDateTimeParam("DataBaseLimite"),
                Numero = Request.GetIntParam("Numero"),
                Situacao = Request.GetNullableEnumParam<SituacaoTermoQuitacao>("Situacao")
            };

            return filtrosPesquisa;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Transportador")
                return "Transportador.RazaoSocial";

            return propriedadeOrdenar;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override bool IsPermitirDelegar(Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao origem)
        {
            return origem.Situacao == SituacaoTermoQuitacao.AguardandoAprovacao;
        }

        protected override List<int> ObterCodigosOrigensSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao> listaTermoQuitacao;
            var selecionarTodos = Request.GetBoolParam("SelecionarTodos");

            if (selecionarTodos)
            {
                var listaItensNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensNaoSelecionados"));
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaTermoQuitacaoAprovacao filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Logistica.AlcadasTermoQuitacao.AprovacaoAlcadaTermoQuitacao repositorioAprovacaoTermoQuitacao = new Repositorio.Embarcador.Logistica.AlcadasTermoQuitacao.AprovacaoAlcadaTermoQuitacao(unitOfWork);

                listaTermoQuitacao = repositorioAprovacaoTermoQuitacao.Consultar(filtrosPesquisa, new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta() { PropriedadeOrdenar = "Codigo" });

                foreach (var itemNaoSelecionado in listaItensNaoSelecionados)
                    listaTermoQuitacao.Remove(new Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao() { Codigo = (int)itemNaoSelecionado.Codigo });
            }
            else
            {
                Repositorio.Embarcador.Logistica.TermoQuitacao repositorioTermoQuitacao = new Repositorio.Embarcador.Logistica.TermoQuitacao(unitOfWork);
                var listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));

                listaTermoQuitacao = new List<Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao>();

                foreach (var itemSelecionado in listaItensSelecionados)
                    listaTermoQuitacao.Add(repositorioTermoQuitacao.BuscarPorCodigo((int)itemSelecionado.Codigo, auditavel: false));
            }

            return (from termoQuitacao in listaTermoQuitacao select termoQuitacao.Codigo).ToList();
        }

        protected override Models.Grid.Grid ObterGridPesquisa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Base", "DataBase", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Transportador", "Transportador", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 20, Models.Grid.Align.center, true);

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaTermoQuitacaoAprovacao filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Logistica.AlcadasTermoQuitacao.AprovacaoAlcadaTermoQuitacao repositorio = new Repositorio.Embarcador.Logistica.AlcadasTermoQuitacao.AprovacaoAlcadaTermoQuitacao(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao> listaTermoQuitacao = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao>();

                var listaTermoQuitacaoRetornar = (
                    from termoQuitacao in listaTermoQuitacao
                    select new
                    {
                        termoQuitacao.Codigo,
                        termoQuitacao.Numero,
                        DataBase = termoQuitacao.DataBase.ToString("dd/MM/yyyy"),
                        Situacao = termoQuitacao.Situacao.ObterDescricao(),
                        Transportador = termoQuitacao.Transportador.Descricao
                    }
                ).ToList();

                grid.AdicionaRows(listaTermoQuitacaoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        protected override void VerificarSituacaoOrigem(Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao origem, Repositorio.UnitOfWork unitOfWork)
        {
            if (origem.Situacao != SituacaoTermoQuitacao.AguardandoAprovacao)
                return;

            var situacaoRegrasAutorizacao = ObterSituacaoRegrasAutorizacao(origem.Codigo, unitOfWork);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aguardando)
                return;

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aprovadas)
            {
                Servicos.Embarcador.Logistica.TermoQuitacao servicoTermoQuitacao = new Servicos.Embarcador.Logistica.TermoQuitacao(unitOfWork);

                if (servicoTermoQuitacao.LiberarProximaPrioridadeAprovacao(origem, TipoServicoMultisoftware))
                    origem.Situacao = SituacaoTermoQuitacao.Finalizado;
            }
            else
                origem.Situacao = SituacaoTermoQuitacao.AprovacaoRejeitada;

            Repositorio.Embarcador.Logistica.TermoQuitacao repositorioTermoQuitacao = new Repositorio.Embarcador.Logistica.TermoQuitacao(unitOfWork);

            repositorioTermoQuitacao.Atualizar(origem);

            if (origem.Situacao == SituacaoTermoQuitacao.Finalizado)
                Servicos.Auditoria.Auditoria.Auditar(Auditado, origem, $"Termo de quitação finalizado", unitOfWork);
        }

        #endregion
    }
}