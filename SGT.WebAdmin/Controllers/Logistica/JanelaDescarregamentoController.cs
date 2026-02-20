using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/JanelaDescarregamento")]
    public class JanelaDescarregamentoController : BaseController
    {
		#region Construtores

		public JanelaDescarregamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        #endregion Métodos Globais

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJanelaDescarregamento ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJanelaDescarregamento()
            {
                CodigoCentroDescarregamento = Request.GetIntParam("CentroDescarregamento"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataFim"),
                Situacao = Request.GetNullableEnumParam<SituacaoJanelaDescarregamento>("Situacao")
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Escala", "Escala", 23, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Produto", "Produto", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Quantidade", "Quantidade", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Motorista", "Motorista", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Transportador", "Transportador", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 13, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Previsao Chegada", "PrevisaoChegada", 15, Models.Grid.Align.center, true);

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJanelaDescarregamento filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Logistica.JanelaDescarregamento repositorioJanelaDescarregamento = new Repositorio.Embarcador.Logistica.JanelaDescarregamento(unitOfWork);
                int totalRegistros = repositorioJanelaDescarregamento.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Logistica.JanelaDescarregamento> listaJanelaDescarregamento = (totalRegistros > 0) ? repositorioJanelaDescarregamento.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Logistica.JanelaDescarregamento>();

                var listaJanelaDescarregamentoRetornar = (
                    from janelaDescarregamento in listaJanelaDescarregamento
                    select new
                    {
                        Codigo = janelaDescarregamento.Codigo,
                        Escala = janelaDescarregamento.Escala.Descricao,
                        Produto = janelaDescarregamento.Produto.Descricao,
                        Quantidade = janelaDescarregamento.Quantidade.ToString("n3"),
                        Veiculo = janelaDescarregamento.Veiculo.Placa,
                        Motorista = janelaDescarregamento.Motorista.Nome,
                        Transportador = janelaDescarregamento.Transportador.RazaoSocial,
                        Situacao = janelaDescarregamento.Situacao.ObterDescricao(),
                        PrevisaoChegada = janelaDescarregamento.PrevisaoChegada.ToString("dd/MM/yyyy hh:mm"),
                        DT_RowColor = janelaDescarregamento.Situacao.ObterCor()
                    }
                ).ToList();

                grid.AdicionaRows(listaJanelaDescarregamentoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Escala")
                return "Escala.Descricao";

            if (propriedadeOrdenar == "Produto")
                return "Produto.Descricao";

            if (propriedadeOrdenar == "Veiculo")
                return "Veiculo.Placa";

            if (propriedadeOrdenar == "Motorista")
                return "Motorista.Nome";

            if (propriedadeOrdenar == "Transportador")
                return "Transportador.RazaoSocial";

            return propriedadeOrdenar;
        }

        #endregion Métodos Privados
    }
}
