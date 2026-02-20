using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Produtos
{
    [CustomAuthorize("Produtos/EnderecoProduto")]
    public class EnderecoProdutoController : BaseController
    {
		#region Construtores

		public EnderecoProdutoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa(unitOfWork);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Produtos.EnderecoProduto repositorioEnderecoProduto = new Repositorio.Embarcador.Produtos.EnderecoProduto(unitOfWork);

            // Dados do filtro
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo;
            if (!string.IsNullOrWhiteSpace(Request.Params("Ativo")))
                Enum.TryParse(Request.Params("Ativo"), out ativo);
            else
                ativo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

            string descricao = Request.Params("Descricao");

            string codigoIntegracao = Request.Params("CodigoIntegracao");

            int codigoFilial = Request.GetIntParam("Filial");

            // Consulta
            List<Dominio.Entidades.Embarcador.Produtos.EnderecoProduto> listaGrid = repositorioEnderecoProduto.Consultar(descricao, codigoIntegracao, codigoFilial, ativo, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repositorioEnderecoProduto.ContarConsulta(descricao, codigoIntegracao, codigoFilial, ativo);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            CodigoFilial = obj.Filial?.Codigo ?? 0,
                            Filial = obj.Filial?.Descricao ?? "",
                            obj.Descricao,
                            obj.CodigoIntegracao,
                            obj.DescricaoAtivo,
                            obj.NivelPrioridade
                        };

            return lista.ToList();
        }

        private void PropOrdena(ref string propOrdenar)
        {
            /* PropOrdena
             * Recebe o campo ordenado na grid
             * Retorna o elemento especifico da entidade para ordenacao
             */

            if (propOrdenar == "DescricaoAtivo") propOrdenar = "Ativo";
        }

        private Models.Grid.Grid GridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("CodigoFilial", false);
            grid.AdicionarCabecalho("Filial", "Filial", 35, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Prioridade", "NivelPrioridade", 10, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Código Integração", "CodigoIntegracao", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Ativo", "DescricaoAtivo", 10, Models.Grid.Align.left, true);

            return grid;
        }

        #endregion
    }
}
