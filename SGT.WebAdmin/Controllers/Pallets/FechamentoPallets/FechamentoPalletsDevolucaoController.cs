using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pallets
{
    [CustomAuthorize(new string[] { "Pesquisa" }, "Pallets/FechamentoPallets")]
    public class FechamentoPalletsDevolucaoController : BaseController
    {
		#region Construtores

		public FechamentoPalletsDevolucaoController(Conexao conexao) : base(conexao) { }

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
                Repositorio.Embarcador.Pallets.DevolucaoPallet repDevolucaoPallet = new Repositorio.Embarcador.Pallets.DevolucaoPallet(unitOfWork);

                // Parametros
                int codigo = Request.GetIntParam("Codigo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet devolucao = repDevolucaoPallet.BuscarPorCodigo(codigo);

                // Valida
                if (devolucao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                bool adicionar = Request.GetBoolParam("Adicionar");

                devolucao.AdicionarAoFechamento = adicionar;

                // Persiste dados
                unitOfWork.Start();

                repDevolucaoPallet.Atualizar(devolucao);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, devolucao, (adicionar ? "Adicionou ao" : "Removeu do") + " fechamento", unitOfWork);

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
            grid.AdicionarCabecalho("Nº Devolução", "NumeroDevolucao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Carga", "Carga", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Nota Fiscal", "NotaFiscal", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Nº Pallets", "Pallets", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Entregues", "PalletsEntregues", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Data de Transporte", "DataTransporte", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Veículo", "Veiculo", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 12, Models.Grid.Align.left, true);

            return grid;
        }

        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Pallets.DevolucaoPallet repDevolucaoPallet = new Repositorio.Embarcador.Pallets.DevolucaoPallet(unitOfWork);

            // Dados do filtro
            int fechamento = Request.GetIntParam("Codigo");

            // Consulta
            List<Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet> listaGrid = repDevolucaoPallet.ConsultaPorFechamento(fechamento, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repDevolucaoPallet.ContarConsultaPorFechamento(fechamento);

            var lista = from obj in listaGrid
                        select new
                        {

                            obj.Codigo,
                            obj.AdicionarAoFechamento,
                            NumeroDevolucao = obj.NumeroDevolucao > 0 ? obj.NumeroDevolucao.ToString() : "",
                            Carga = obj.CargaPedido?.Carga.CodigoCargaEmbarcador ?? "Sem carga",
                            NotaFiscal = obj.XMLNotaFiscal?.Numero,
                            Pallets = obj.QuantidadePallets,
                            PalletsEntregues = obj.Situacoes?.Where(o => o.AcresceSaldo).Sum(o => o.Quantidade) ?? 0,
                            DataTransporte = obj.CargaPedido?.Carga.DataCarregamentoCarga?.ToString("dd/MM/yyyy") ?? obj.XMLNotaFiscal?.DataEmissao.ToString("dd/MM/yyyy"),
                            Transportador = obj.Transportador.RazaoSocial,
                            Motorista = obj.CargaPedido?.Carga.RetornarMotoristas ?? string.Empty,
                            Veiculo = obj.CargaPedido?.Carga.RetornarPlacas ?? string.Empty,
                            obj.DescricaoSituacao,
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
            if (propOrdenar == "Carga")
                propOrdenar = "CargaPedido.Carga.CodigoCargaEmbarcador";
            else if (propOrdenar == "Veiculo")
                propOrdenar = "CargaPedido.Carga.Veiculo.Placa";
            else if (propOrdenar == "NotaFiscal")
                propOrdenar = "XMLNotaFiscal.Numero";
            else if (propOrdenar == "DataTransporte")
                propOrdenar = "CargaPedido.Carga.DataCarregamentoCarga";
            else if (propOrdenar == "Transportador")
                propOrdenar = "CargaPedido.Carga.Empresa.RazaoSocial";
            else if (propOrdenar == "DescricaoSituacao")
                propOrdenar = "Situacao";
        }
        #endregion
    }
}
