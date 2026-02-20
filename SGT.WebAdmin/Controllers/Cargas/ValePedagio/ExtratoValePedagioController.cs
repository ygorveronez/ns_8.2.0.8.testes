using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.ValePedagio
{
    [CustomAuthorize("Cargas/ExtratoValePedagio")]
	public class ExtratoValePedagioController : BaseController
	{
		#region Construtores

		public ExtratoValePedagioController(Conexao conexao) : base(conexao) { }

		#endregion


		#region Métodos Globais
		
		public async Task<IActionResult> Pesquisa()
		{
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
			{
				
				Models.Grid.Grid grid = ObterGridPreenchida(unitOfWork);

				return new JsonpResult(grid);
			}
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar vales pedágio.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> PesquisaExtratos()
		{
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
			{
                Repositorio.Embarcador.Cargas.ExtratoValePedagio.ExtratosValePedagio repositorioExtratosValePedagio = new Repositorio.Embarcador.Cargas.ExtratoValePedagio.ExtratosValePedagio(unitOfWork);

                int codigoExtrato = Request.GetIntParam("CodigoExtrato");

                Dominio.Entidades.Embarcador.Cargas.ExtratoValePedagio.ExtratosValePedagio extratoValePedagio = repositorioExtratosValePedagio.BuscarPorCodigo(codigoExtrato, false);

                if (extratoValePedagio == null)
                    return new JsonpResult(false, false, "Registro não econtrato.");

                Models.Grid.Grid grid = ObterGridExtrato();

                var extratos = 
                    (from extrato in extratoValePedagio.Extratos
                    select new
                    {
                        extrato.Codigo,
                        PracaPedagio = extrato.NomePraca,
                        extrato.Acao,
                        Valor = (extrato.ValorOperacao ?? 0) < 0 ? (extrato.ValorOperacao.Value * -1).ToString("n2") : extrato.ValorOperacao?.ToString("n2") ?? string.Empty,
                        DataPassagem = extrato.DataPassagem?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    
                    }).ToList();

                grid.AdicionaRows(extratos);
                grid.setarQuantidadeTotal(extratos.Count);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
			{
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu um erro ao consultar os extratos do vale pedágio.");
			}
            finally
			{
                unitOfWork.Dispose();
			}
		}

        #endregion

        #region Métodos privados

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("SituacaoValePedagio", false);
            grid.AdicionarCabecalho("SituacaoExtrato", false);
            grid.AdicionarCabecalho("Carga", "CodigoCargaEmbarcador", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data da carga", "DataCriacaoCarga", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Veículo", "Placa", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Número VP", "NumeroViagem", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Situação do extrato", "DescricaoSituacaoExtrato", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Fatura", "Fatura", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Data da fatura", "DataFatura", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Situação do VP", "DescricaoSituacaoValePedagio", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Valor VP", "ValorValePedagio", 10, Models.Grid.Align.right, true);
            return grid;
        }

        private Models.Grid.Grid ObterGridExtrato()
		{
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Praça de Pedágio", "PracaPedagio", 40, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Ação", "Acao", 20, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Valor", "Valor", 20, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Data de Passagem", "DataPassagem", 20, Models.Grid.Align.center, false);
            return grid;
        }

        private Models.Grid.Grid ObterGridPreenchida(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ExtratoValePedagio.ExtratosValePedagio repositorioExtratosValePedagio = new Repositorio.Embarcador.Cargas.ExtratoValePedagio.ExtratosValePedagio(unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaIntegracaoValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Carga.ExtratoValePedagio.FiltroPesquisaExtratosValePedagio filtroPesquisa = ObterFiltrosPesquisa(unitOfWork);

            int total = repositorioExtratosValePedagio.ContarConsulta(filtroPesquisa);

            List<Dominio.Entidades.Embarcador.Cargas.ExtratoValePedagio.ExtratosValePedagio> extratosValePedagio = new List<Dominio.Entidades.Embarcador.Cargas.ExtratoValePedagio.ExtratosValePedagio>();

            Models.Grid.Grid grid = ObterGridPesquisa();

            if (total > 0)
            {
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                extratosValePedagio = repositorioExtratosValePedagio.Consultar(filtroPesquisa, parametrosConsulta);
            }

            PreencherGrid(grid, total, extratosValePedagio);
            return grid;
        }

        private void PreencherGrid(Models.Grid.Grid grid, int total, List<Dominio.Entidades.Embarcador.Cargas.ExtratoValePedagio.ExtratosValePedagio> extratosValePedagio)
        {
            var rows = (
                from extrato in extratosValePedagio
                select new
                {
                    extrato.Codigo,
                    extrato.SituacaoExtrato,
                    extrato.DescricaoSituacaoExtrato,
                    CodigoCargaEmbarcador = extrato.ValePedagio?.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                    DataCriacaoCarga = extrato.ValePedagio?.Carga?.DataCriacaoCarga.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    Placa = extrato.ValePedagio?.Carga?.Veiculo?.Placa ?? string.Empty,
                    extrato.NumeroViagem,
                    Fatura = !string.IsNullOrWhiteSpace(extrato.Fatura) ? extrato.Fatura : string.Empty,
                    DataFatura = extrato.DataFatura?.ToString("dd/MM/yyyy") ?? string.Empty,
                    SituacaoValePedagio = extrato.ValePedagio?.SituacaoValePedagio ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Pendete,
                    extrato.DescricaoSituacaoValePedagio,
                    ValorValePedagio = extrato.ValePedagio?.ValorValePedagio.ToString("n2") ?? string.Empty,
                }).ToList();
            grid.AdicionaRows(rows);
            grid.setarQuantidadeTotal(total);
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.ExtratoValePedagio.FiltroPesquisaExtratosValePedagio ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.ExtratoValePedagio.FiltroPesquisaExtratosValePedagio filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.ExtratoValePedagio.FiltroPesquisaExtratosValePedagio()
            {
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                DataCargaInicial = Request.GetNullableDateTimeParam("DataCargaInicial"),
                DataCargaFinal = Request.GetNullableDateTimeParam("DataCargaFinal"),
                NumeroValePedagio = Request.GetLongParam("NumeroValePedagio"),
                SituacaoExtrato = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoExtratoValePedagio>("SituacaoExtrato")
            };
            return filtrosPesquisa;
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                Repositorio.Embarcador.Cargas.ExtratoValePedagio.ExtratosValePedagio repositorioExtratosValePedagio = new Repositorio.Embarcador.Cargas.ExtratoValePedagio.ExtratosValePedagio(unitOfWork);
                Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaIntegracaoValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Carga.ExtratoValePedagio.FiltroPesquisaExtratosValePedagio filtroPesquisa = ObterFiltrosPesquisa(unitOfWork);
                
                var grid = ObterGridPreenchida(unitOfWork);

                int total = repositorioExtratosValePedagio.ContarConsulta(filtroPesquisa);

                List<Dominio.Entidades.Embarcador.Cargas.ExtratoValePedagio.ExtratosValePedagio> extratosValePedagio = new List<Dominio.Entidades.Embarcador.Cargas.ExtratoValePedagio.ExtratosValePedagio>();

                if (total > 0)
                {
                    Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                    extratosValePedagio = repositorioExtratosValePedagio.Consultar(filtroPesquisa, parametrosConsulta);
                }

                PreencherGrid(grid, total, extratosValePedagio);

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
