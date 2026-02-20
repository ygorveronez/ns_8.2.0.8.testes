using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pallets
{
    [CustomAuthorize(new string[] { "Pesquisa" } , "Pallets/FechamentoPallets")]
    public class FechamentoPalletsEstoquePalletsController : BaseController
    {
		#region Construtores

		public FechamentoPalletsEstoquePalletsController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

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

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            { 
                // Instancia repositorios
                Repositorio.Embarcador.Pallets.EstoquePallet repEstoquePallet = new Repositorio.Embarcador.Pallets.EstoquePallet(unitOfWork);

                // Parametros
                int codigo = Request.GetIntParam("Codigo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pallets.EstoquePallet estoqueFechamento = repEstoquePallet.BuscarPorCodigo(codigo);

                // Valida
                if (estoqueFechamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                bool adicionar = Request.GetBoolParam("Adicionar");

                estoqueFechamento.AdicionarAoFechamento = adicionar;

                // Persiste dados
                unitOfWork.Start();

                repEstoquePallet.Atualizar(estoqueFechamento);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, estoqueFechamento, (adicionar ? "Adicionou ao" : "Removeu do") + " fechamento" , unitOfWork);

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #region Métodos Privados


        /* GridPesquisa
         * Retorna o model de Grid para a o módulo
         */
        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("AdicionarAoFechamento", false);
            grid.AdicionarCabecalho("Data", "Data", 16, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Tipo de Lançamento", "TipoLancamento", 16, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Observação", "Observacao", 32, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Entrada", "Entrada", 12, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Saída", "Saida", 12, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Saldo Total", "SaldoTotal", 12, Models.Grid.Align.left, false);

            return grid;
        }

        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Pallets.EstoquePallet repEstoquePallet = new Repositorio.Embarcador.Pallets.EstoquePallet(unitOfWork);

            // Dados do filtro
            int fechamento = Request.GetIntParam("Codigo");

            // Consulta
            List<Dominio.Entidades.Embarcador.Pallets.EstoquePallet> listaGrid = repEstoquePallet.ConsultaPorFechamento(fechamento, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repEstoquePallet.ContarConsultaPorFechamento(fechamento);

            var lista = from obj in listaGrid
                        select new
                        {

                            obj.Codigo,
                            obj.AdicionarAoFechamento,
                            Data = obj.Data.ToString("dd/MM/yyyy HH:mm"),
                            obj.Observacao,
                            TipoLancamento = obj.ObterTipoLancamento(),
                            Entrada = obj.ObterQuantidadeEntrada(),
                            Saida = obj.ObterQuantidadeSaida(),
                            obj.SaldoTotal,
                            DT_RowColor = obj.AdicionarAoFechamento ? "" : "#949494",
                            DT_FontColor = obj.AdicionarAoFechamento ? "" : "#fff",
                        };

            return lista.ToList();
        }

        /* PropOrdena
         * Recebe o campo ordenado na grid
         * Retorna o elemento especifico da entidade para ordenacao
         */
        private void PropOrdena(ref string propOrdenar)
        {
            //if (propOrdenar == "Relacional") propOrdenar = "Relacional.Codigo";
        }
        #endregion
    }
}
