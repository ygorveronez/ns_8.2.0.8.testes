using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    [CustomAuthorize(new string[] { "ExportarPesquisa" }, "GestaoPatio/DisponibilidadeVeiculo")]
    public class DisponibilidadeVeiculoController : BaseController
    {
		#region Construtores

		public DisponibilidadeVeiculoController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais
        [AllowAuthenticate]
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

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
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

                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
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
        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Placa", "Placa", 10, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Modelo Veicular", "ModeloVeicular", 15, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Transportador", "Transportador", 35, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Disponível", "Disponivel", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Situação", "Situacao", 15, Models.Grid.Align.center, true);

            return grid;
        }

        
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.GestaoPatio.DisponibilidadeVeiculo repDisponibilidadeVeiculo = new Repositorio.Embarcador.GestaoPatio.DisponibilidadeVeiculo(unitOfWork);

            // Dados do filtro
            int.TryParse(Request.Params("Transportador"), out int transportador);
            int.TryParse(Request.Params("ContratoFrete"), out int contratoFrete);

            Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.PesquisaDisponibilidadeVeiculo pesquisaDisponibilidadeVeiculo);

            DateTime dataBase = DateTime.Today;

            // Consulta 
            List<Dominio.Entidades.Embarcador.GestaoPatio.DisponibilidadeVeiculo> listaGrid = repDisponibilidadeVeiculo.ConsultarDisponibilidadeVeiculo(transportador, contratoFrete, dataBase, pesquisaDisponibilidadeVeiculo, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repDisponibilidadeVeiculo.ContarConsultaDisponibilidadeVeiculo(transportador, contratoFrete, dataBase, pesquisaDisponibilidadeVeiculo);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            Disponivel = obj.Disponivel?.ToString("dd/MM/yyyy HH:mm"),
                            obj.Veiculo.Placa,
                            Transportador = obj.Veiculo?.Empresa?.Descricao ?? string.Empty,
                            ModeloVeicular = obj.Veiculo?.ModeloVeicularCarga?.Descricao ?? string.Empty,
                            Situacao = obj.Disponivel.HasValue ? "Disponível" : "Em Viagem",
                        };

            return lista.ToList();
        }

        private void PropOrdena(ref string propOrdenar)
        {
            //if (propOrdenar == "Disponivel")
            //    propOrdenar = "Disponivel ";
        }
        #endregion
    }
}
