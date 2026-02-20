using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Frotas
{
	[CustomAuthorize("Frotas/TabelaPrecoCombustivel")]
	public class TabelaPrecoCombustivelController : BaseController
	{
		#region Construtores 

		public TabelaPrecoCombustivelController(Conexao conexao) : base(conexao) { }

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
				var propOrdenar = grid.header[grid.indiceColunaOrdena].data;

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

		public async Task<IActionResult> Adicionar()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

			try
			{
				Repositorio.Embarcador.Frotas.TabelaPrecoCombustivel repTabelaPrecoCombustivel = new Repositorio.Embarcador.Frotas.TabelaPrecoCombustivel(unitOfWork);

				Dominio.Entidades.Embarcador.Frotas.TabelaPrecoCombustivel tabelaPrecoCombustivel = new Dominio.Entidades.Embarcador.Frotas.TabelaPrecoCombustivel();

				PreencherEntidade(tabelaPrecoCombustivel, unitOfWork);

				unitOfWork.Start();

				repTabelaPrecoCombustivel.Inserir(tabelaPrecoCombustivel, Auditado);

				unitOfWork.CommitChanges();

				return new JsonpResult(true);
			}
			catch (ControllerException ex)
			{
				unitOfWork.Rollback();
				return new JsonpResult(false, true, ex.Message);
			}
			catch (Exception excecao)
			{
				unitOfWork.Rollback();

				Servicos.Log.TratarErro(excecao);

				return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
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
				long codigo = Request.GetLongParam("Codigo");

				Repositorio.Embarcador.Frotas.TabelaPrecoCombustivel repTabelaPrecoCombustivel = new Repositorio.Embarcador.Frotas.TabelaPrecoCombustivel(unitOfWork);

				Dominio.Entidades.Embarcador.Frotas.TabelaPrecoCombustivel tabelaPrecoCombustivel = repTabelaPrecoCombustivel.BuscarPorCodigo(codigo, true);

				if (tabelaPrecoCombustivel == null)
					return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

				PreencherEntidade(tabelaPrecoCombustivel, unitOfWork);

				unitOfWork.Start();

				repTabelaPrecoCombustivel.Atualizar(tabelaPrecoCombustivel, Auditado);

				unitOfWork.CommitChanges();

				return new JsonpResult(true);
			}
			catch (ControllerException ex)
			{
				unitOfWork.Rollback();
				return new JsonpResult(false, true, ex.Message);
			}
			catch (Exception excecao)
			{
				unitOfWork.Rollback();

				Servicos.Log.TratarErro(excecao);

				return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

		public async Task<IActionResult> ExcluirPorCodigo()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

			try
			{
				long codigo = Request.GetLongParam("Codigo");

				Repositorio.Embarcador.Frotas.TabelaPrecoCombustivel repTabelaPrecoCombustivel = new Repositorio.Embarcador.Frotas.TabelaPrecoCombustivel(unitOfWork);

				Dominio.Entidades.Embarcador.Frotas.TabelaPrecoCombustivel tabelaPrecoCombustivel = repTabelaPrecoCombustivel.BuscarPorCodigo(codigo, true);

				if (tabelaPrecoCombustivel == null)
					return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

				unitOfWork.Start();

				repTabelaPrecoCombustivel.Deletar(tabelaPrecoCombustivel, Auditado);

				unitOfWork.CommitChanges();

				return new JsonpResult(true);
			}
			catch (Exception excecao)
			{
				unitOfWork.Rollback();

				Servicos.Log.TratarErro(excecao);

				return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

		public async Task<IActionResult> BuscarPorCodigo()
		{
			var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

			try
			{
				long codigo = Request.GetLongParam("Codigo");

				Repositorio.Embarcador.Frotas.TabelaPrecoCombustivel repTabelaPrecoCombustivel = new Repositorio.Embarcador.Frotas.TabelaPrecoCombustivel(unitOfWork);

				Dominio.Entidades.Embarcador.Frotas.TabelaPrecoCombustivel tabelaPrecoCombustivel = repTabelaPrecoCombustivel.BuscarPorCodigo(codigo, false);

				if (tabelaPrecoCombustivel == null)
					return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

				return new JsonpResult(new
				{
					tabelaPrecoCombustivel.Codigo,
					tabelaPrecoCombustivel.Descricao,
					Empresa = new { Codigo = tabelaPrecoCombustivel.Empresa?.Codigo ?? 0, Descricao = tabelaPrecoCombustivel.Empresa?.Descricao ?? string.Empty },
					TipoOleo = new { Codigo = tabelaPrecoCombustivel.TipoOleo?.Codigo ?? 0, Descricao = tabelaPrecoCombustivel.TipoOleo?.Descricao ?? string.Empty },
					ValorExterno = tabelaPrecoCombustivel.ValorExterno.ToString("n2"),
					ValorInterno = tabelaPrecoCombustivel.ValorInterno.ToString("n2"),
					DataInicioVigencia = tabelaPrecoCombustivel.DataInicioVigencia.ToString("dd/MM/yyyy"),
				});
			}
			catch (Exception excecao)
			{
				Servicos.Log.TratarErro(excecao);

				return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarTabelaPrecoPorVigencia()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("CodigoTipoOleo"), out int codTipoOleo);
                DateTime.TryParse(Request.Params("Data"), out DateTime data);

                Repositorio.Embarcador.Frotas.TabelaPrecoCombustivel repTabelaPrecoCombustivel = new Repositorio.Embarcador.Frotas.TabelaPrecoCombustivel(unitOfWork);
                Dominio.Entidades.Embarcador.Frotas.TabelaPrecoCombustivel tabelaPrecoCombustivel = repTabelaPrecoCombustivel.BuscarPorTipoOleoVigencia(codTipoOleo,data);

                if (tabelaPrecoCombustivel == null)
                    return new JsonpResult(false, false , $"Não foi possível encontrar uma tabela de preço do combustível em vigência na data de {data}.");


                return new JsonpResult(new { ValorInterno = tabelaPrecoCombustivel.ValorInterno.ToString("n2") },true, "Sucesso!");

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        

        #endregion

		#region Métodos Privados

		private void PreencherEntidade(Dominio.Entidades.Embarcador.Frotas.TabelaPrecoCombustivel tabelaPrecoCombustivel, Repositorio.UnitOfWork unitOfWork)
		{
			Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
			Repositorio.Embarcador.Frotas.TipoOleo repTipoOleo = new Repositorio.Embarcador.Frotas.TipoOleo(unitOfWork);

			int codigoEmpresa = Request.GetIntParam("Empresa");
			string descricao = Request.GetStringParam("Descricao");
			int tipoOleo = Request.GetIntParam("TipoOleo");

			decimal valorExterno;
			decimal.TryParse(Request.Params("ValorExterno"), out valorExterno);

			decimal valorInterno;
			decimal.TryParse(Request.Params("ValorInterno"), out valorInterno);

			DateTime dataInicioVigencia;
			DateTime.TryParseExact(Request.Params("DataInicioVigencia"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicioVigencia);

			tabelaPrecoCombustivel.Descricao = !String.IsNullOrEmpty(descricao) ? descricao : throw new ControllerException("A descrição não pode ser nula ou vazia!");
			tabelaPrecoCombustivel.TipoOleo = tipoOleo > 0 ? repTipoOleo.BuscarPorCodigo(tipoOleo) : throw new ControllerException("Tipo de óleo não cadastrado!");
			tabelaPrecoCombustivel.Empresa = codigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresa) : throw new ControllerException("Empresa não cadastrado!");
			tabelaPrecoCombustivel.ValorInterno = valorInterno;
			tabelaPrecoCombustivel.ValorExterno = valorExterno;
			tabelaPrecoCombustivel.DataInicioVigencia = dataInicioVigencia;


		}

		private Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaTabelaPrecoCombustivel ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
		{
			DateTime dataInicioVigencia;
			DateTime.TryParseExact(Request.Params("DataInicioVigencia"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicioVigencia);

			Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaTabelaPrecoCombustivel filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaTabelaPrecoCombustivel()
			{
				Descricao = Request.GetStringParam("Descricao"),
				CodigoTipoOleo = Request.GetIntParam("TipoOleo"),
				CodigoEmpresa = Request.GetIntParam("Empresa"),
				DataInicioVigencia = dataInicioVigencia,
			};

			return filtrosPesquisa;
		}

		private Models.Grid.Grid GridPesquisa()
		{
			Models.Grid.Grid grid = new Models.Grid.Grid(Request)
			{
				header = new List<Models.Grid.Head>()
			};

			grid.AdicionarCabecalho("Codigo", false);
			grid.AdicionarCabecalho("Descrição", "Descricao", 30, Models.Grid.Align.left, true);
			grid.AdicionarCabecalho("Empresa", "Empresa", 30, Models.Grid.Align.center, true);
			grid.AdicionarCabecalho("Tipo de Óleo", "TipoOleo", 30, Models.Grid.Align.center, true);
			grid.AdicionarCabecalho("Data inicio vigência", "DataInicioVigencia", 10, Models.Grid.Align.left, true);
			grid.AdicionarCabecalho("Valor Externo", "ValorExterno", 10, Models.Grid.Align.left, true);
			grid.AdicionarCabecalho("Valor Interno", "ValorInterno", 10, Models.Grid.Align.left, true);

			return grid;
		}

		private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
		{
			Repositorio.Embarcador.Frotas.TabelaPrecoCombustivel repTabelaPrecoCombustivel = new Repositorio.Embarcador.Frotas.TabelaPrecoCombustivel(unitOfWork);

			Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaTabelaPrecoCombustivel filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

			List<Dominio.Entidades.Embarcador.Frotas.TabelaPrecoCombustivel> listaTabelaPrecoCombustivel = repTabelaPrecoCombustivel.Consultar(filtrosPesquisa, propOrdenar, dirOrdena, inicio, limite);
			totalRegistros = repTabelaPrecoCombustivel.ContarConsulta(filtrosPesquisa);

			var dynListaTabelaPrecoCombustivel = from tabelaPrecoCombustivel in listaTabelaPrecoCombustivel
												 select new
												 {
													 tabelaPrecoCombustivel.Codigo,
													 tabelaPrecoCombustivel.Descricao,
													 TipoOleo = tabelaPrecoCombustivel.TipoOleo != null ? tabelaPrecoCombustivel.TipoOleo.Descricao : string.Empty,
													 Empresa = tabelaPrecoCombustivel.Empresa != null ? tabelaPrecoCombustivel.Empresa.Descricao : string.Empty,
													 ValorExterno = tabelaPrecoCombustivel.ValorExterno.ToString("n2"),
													 ValorInterno = tabelaPrecoCombustivel.ValorInterno.ToString("n2"),
													 DataInicioVigencia = tabelaPrecoCombustivel.DataInicioVigencia.ToString("dd/MM/yyyy"),
												 };

			return dynListaTabelaPrecoCombustivel.ToList();
		}

		#endregion
	}
}