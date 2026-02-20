using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Pallets
{
    [CustomAuthorize(new string[] { "Pesquisa" }, "Pallets/FechamentoPallets")]
    public class FechamentoPalletsTransferenciaController : BaseController
    {
		#region Construtores

		public FechamentoPalletsTransferenciaController(Conexao conexao) : base(conexao) { }

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
                Repositorio.Embarcador.Pallets.TransferenciaPallet repTransferenciaPallet = new Repositorio.Embarcador.Pallets.TransferenciaPallet(unitOfWork);

                // Parametros
                int codigo = Request.GetIntParam("Codigo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet transferencia = repTransferenciaPallet.BuscarPorCodigo(codigo);

                // Valida
                if (transferencia == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                bool adicionar = Request.GetBoolParam("Adicionar");

                transferencia.AdicionarAoFechamento = adicionar;

                // Persiste dados
                unitOfWork.Start();

                repTransferenciaPallet.Atualizar(transferencia);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, transferencia, (adicionar ? "Adicionou ao" : "Removeu do") + " fechamento", unitOfWork);

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
            grid.AdicionarCabecalho("Número", "Numero", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data", "Data", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situacao", "Situacao", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Filial", "Filial", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Setor", "Setor", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Turno", "Turno", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Quantidade", "Quantidade", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Solicitante", "Solicitante", 15, Models.Grid.Align.left, true);

            return grid;
        }

        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Pallets.TransferenciaPallet repTransferenciaPallet = new Repositorio.Embarcador.Pallets.TransferenciaPallet(unitOfWork);

            // Dados do filtro
            int fechamento = Request.GetIntParam("Codigo");

            // Consulta
            List<Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet> listaGrid = repTransferenciaPallet.ConsultaPorFechamento(fechamento, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repTransferenciaPallet.ContarConsultaPorFechamento(fechamento);

            var lista = from obj in listaGrid
                        select new
                        {

                            obj.Codigo,
                            obj.AdicionarAoFechamento,
                            obj.Solicitacao.Quantidade,
                            obj.Solicitacao.Solicitante,
                            Data = obj.Solicitacao.Data.ToString("dd/MM/yyyy"),
                            obj.Numero,
                            Filial = obj.Solicitacao.Filial.Descricao,
                            Setor = obj.Solicitacao.Setor.Descricao,
                            Situacao = obj.Situacao.ObterDescricao(),
                            Turno = obj.Solicitacao.Turno.Descricao,
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
            if (propOrdenar == "Data")
                propOrdenar = "Solicitacao.Data";
            else if (propOrdenar == "Filial")
                propOrdenar = "Solicitacao.Filial.Descricao";
            else if (propOrdenar == "Quantidade")
                propOrdenar = "Solicitacao.Quantidade";
            else if (propOrdenar == "Setor")
                propOrdenar = "Solicitacao.Setor.Descricao";
            else if (propOrdenar == "Solicitante")
                propOrdenar = "Solicitacao.Solicitante";
            else if (propOrdenar == "Turno")
                propOrdenar = "Solicitacao.Turno.Descricao";
        }
        #endregion
    }
}
