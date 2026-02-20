using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Pallets
{
    [CustomAuthorize(new string[] { "Pesquisa" }, "Pallets/FechamentoPallets")]
    public class FechamentoPalletsReformaController : BaseController
    {
		#region Construtores

		public FechamentoPalletsReformaController(Conexao conexao) : base(conexao) { }

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
                Repositorio.Embarcador.Pallets.Reforma.ReformaPallet repReformaPallet = new Repositorio.Embarcador.Pallets.Reforma.ReformaPallet(unitOfWork);

                // Parametros
                int codigo = Request.GetIntParam("Codigo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPallet reforma = repReformaPallet.BuscarPorCodigo(codigo);

                // Valida
                if (reforma == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                bool adicionar = Request.GetBoolParam("Adicionar");

                reforma.AdicionarAoFechamento = adicionar;

                // Persiste dados
                unitOfWork.Start();

                repReformaPallet.Atualizar(reforma);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, reforma, (adicionar ? "Adicionou ao" : "Removeu do") + " fechamento", unitOfWork);

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
            grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data", "Data", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situacao", "Situacao", 14, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Filial", "Filial", 22, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("CNPJ Filial", "FilialCnpj", 11, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Fornecedor", "Fornecedor", 22, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("CPF/CNPJ Fornecedor", "FornecedorCpfCnpj", 11, Models.Grid.Align.left, false);

            return grid;
        }

        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Pallets.Reforma.ReformaPallet repReformaPallet = new Repositorio.Embarcador.Pallets.Reforma.ReformaPallet(unitOfWork);

            // Dados do filtro
            int fechamento = Request.GetIntParam("Codigo");

            // Consulta
            List<Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPallet> listaGrid = repReformaPallet.ConsultaPorFechamento(fechamento, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repReformaPallet.ContarConsultaPorFechamento(fechamento);

            var lista = from obj in listaGrid
                        select new
                        {

                            obj.Codigo,
                            obj.AdicionarAoFechamento,
                            Data = obj.Envio.Data.ToString("dd/MM/yyyy"),
                            obj.Numero,
                            Filial = obj.Envio.Filial?.Descricao,
                            FilialCnpj = obj.Envio.Filial?.CNPJ_Formatado,
                            Fornecedor = obj.Envio.Fornecedor.Nome,
                            FornecedorCpfCnpj = obj.Envio.Fornecedor.CPF_CNPJ_Formatado,
                            Situacao = obj.Situacao.ObterDescricao(),
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
                propOrdenar = "Envio.Data";
            else if (propOrdenar == "Filial")
                propOrdenar = "Envio.Filial.Descricao";
            else if (propOrdenar == "Fornecedor")
                propOrdenar = "Envio.Fornecedor.Nome";
        }
        #endregion
    }
}
