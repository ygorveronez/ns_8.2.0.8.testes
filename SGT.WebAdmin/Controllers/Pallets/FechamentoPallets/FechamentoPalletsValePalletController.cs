using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pallets
{
    [CustomAuthorize(new string[] { "Pesquisa" }, "Pallets/FechamentoPallets")]
    public class FechamentoPalletsValePalletController : BaseController
    {
		#region Construtores

		public FechamentoPalletsValePalletController(Conexao conexao) : base(conexao) { }

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
                Repositorio.Embarcador.Pallets.ValePallet repValePallet = new Repositorio.Embarcador.Pallets.ValePallet(unitOfWork);

                // Parametros
                int codigo = Request.GetIntParam("Codigo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pallets.ValePallet valePallet = repValePallet.BuscarPorCodigo(codigo);

                // Valida
                if (valePallet == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                bool adicionar = Request.GetBoolParam("Adicionar");

                valePallet.AdicionarAoFechamento = adicionar;

                // Persiste dados
                unitOfWork.Start();

                repValePallet.Atualizar(valePallet);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, valePallet, (adicionar ? "Adicionou ao" : "Removeu do") + " fechamento", unitOfWork);

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
            grid.Prop("Numero").Nome("Número").Tamanho(7).Align(Models.Grid.Align.right);
            grid.Prop("NotaFiscal").Nome("NF-e").Tamanho(10).Align(Models.Grid.Align.right);
            grid.Prop("Cliente").Nome("Cliente").Tamanho(20).Align(Models.Grid.Align.right);
            grid.Prop("Filial").Nome("Filial").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("Quantidade").Nome("Qtd.").Tamanho(7).Align(Models.Grid.Align.left);
            grid.Prop("Data").Nome("Data").Tamanho(7).Align(Models.Grid.Align.center);
            grid.Prop("Situacao").Nome("Situação").Tamanho(10).Align(Models.Grid.Align.left);

            return grid;
        }

        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Pallets.ValePallet repValePallet = new Repositorio.Embarcador.Pallets.ValePallet(unitOfWork);

            // Dados do filtro
            int fechamento = Request.GetIntParam("Codigo");

            // Consulta
            List<Dominio.Entidades.Embarcador.Pallets.ValePallet> listaGrid = repValePallet.ConsultaPorFechamento(fechamento, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repValePallet.ContarConsultaPorFechamento(fechamento);

            var lista = from obj in listaGrid
                        select new
                        {

                            obj.Codigo,
                            obj.AdicionarAoFechamento,
                            obj.Numero,
                            Cliente = obj.Devolucao.XMLNotaFiscal?.Destinatario?.Descricao,
                            NotaFiscal = obj.Devolucao.XMLNotaFiscal?.Numero.ToString() ?? String.Empty,
                            Filial = obj.Devolucao.Filial?.Descricao ?? string.Empty,
                            Data = obj.DataLancamento.ToString("dd/MM/yyyy"),
                            Quantidade = obj.Quantidade.ToString(),
                            Situacao = obj.DescricaoSituacao,
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
                propOrdenar = "DataLancamento";
            else if (propOrdenar == "Filial")
                propOrdenar = "Devolucao.Filial.Descricao";
            else if (propOrdenar == "NotaFiscal")
                propOrdenar = "Devolucao.XMLNotaFiscal.Numero";
            else if (propOrdenar == "Cliente")
                propOrdenar = "Devolucao.XMLNotaFiscal.Destinatario.Nome";
        }
        #endregion
    }
}
