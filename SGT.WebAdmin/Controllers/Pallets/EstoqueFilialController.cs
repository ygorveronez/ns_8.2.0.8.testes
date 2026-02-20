using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pallets
{
    [CustomAuthorize(new string[] { "PesquisaExtrato"}, "Pallets/EstoqueFilial")]
    public class EstoqueFilialController : BaseController
    {
		#region Construtores

		public EstoqueFilialController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> PesquisaExtrato()
        {
            var unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigoFilial = Request.GetIntParam("Filial");
                var codigosFilial = codigoFilial > 0 ? new List<int>() { codigoFilial } : new List<int>();
                var dataInicio = Request.GetNullableDateTimeParam("DataInicio");
                var dataFim = Request.GetNullableDateTimeParam("DataFim");
                var grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Filial", "Filial", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("CNPJ Filial", "FilialCnpj", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo de Lançamento", "TipoLancamento", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Observação", "Observacao", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Entrada", "Entrada", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Saída", "Saida", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Saldo Total", "SaldoTotal", 10, Models.Grid.Align.left, false);

                var propriedadeAgrupar = "";
                var propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);
                var repositorioEstoque = new Repositorio.Embarcador.Pallets.EstoquePallet(unidadeTrabalho);
                var listaMovimentacaoEstoquePalletsFilial = repositorioEstoque.ConsultarFilial(codigosFilial, dataInicio, dataFim, propriedadeAgrupar, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repositorioEstoque.ContarConsultaFilial(codigosFilial, dataInicio, dataFim));

                grid.AdicionaRows((
                    from movimentacao in listaMovimentacaoEstoquePalletsFilial
                    select new
                    {
                        movimentacao.Codigo,
                        Data = movimentacao.Data.ToString("dd/MM/yyyy HH:mm"),
                        Filial = movimentacao.Filial?.Descricao,
                        FilialCnpj = movimentacao.Filial?.CNPJ_Formatado,
                        movimentacao.Observacao,
                        TipoLancamento = movimentacao.ObterTipoLancamento(),
                        Entrada = movimentacao.ObterQuantidadeEntrada(),
                        Saida = movimentacao.ObterQuantidadeSaida(),
                        movimentacao.SaldoTotal
                    }
                ).ToList());

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private string ObterPropriedadeOrdenar(string nomePropriedadeOrdenar)
        {
            if (nomePropriedadeOrdenar == "Filial")
                return "Filial.Descricao";

            return nomePropriedadeOrdenar;
        }

        #endregion
    }
}
