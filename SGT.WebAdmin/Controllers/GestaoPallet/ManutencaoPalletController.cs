using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.GestaoPallet;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.GestaoPallet
{
	[CustomAuthorize("GestaoPallet/ManutencaoPallet")]
	public class ManutencaoPalletController : BaseController
	{
		#region Construtores

		public ManutencaoPalletController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Globais

		[AllowAuthenticate]
		public async Task<IActionResult> Pesquisa()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
			try
			{
				return new JsonpResult(await ObterGridPesquisa(unitOfWork));
			}
			catch (Exception excecao)
			{
				Servicos.Log.TratarErro(excecao);
				return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

		[AllowAuthenticate]
		public async Task<IActionResult> ObterTotais()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
			try
			{
				Repositorio.Embarcador.GestaoPallet.ManutencaoPallet repositorioManutencaoPallet = new Repositorio.Embarcador.GestaoPallet.ManutencaoPallet(unitOfWork);

				Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ManutencaoPallet.FiltroPesquisaManutencaoPallet filtroPesquisa = ObterFiltrosPesquisaManutencaoPallet();
				Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ManutencaoPallet.TotalizadorManutencaoPallet totalizadores = await repositorioManutencaoPallet.ObterTotalizadorManutencaoPalletAsync(filtroPesquisa);

				return new JsonpResult(totalizadores);
			}
			catch (Exception excecao)
			{
				Servicos.Log.TratarErro(excecao);
				return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
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
				Models.Grid.Grid grid = await ObterGridPesquisa(unitOfWork);
				byte[] arquivoBinario = grid.GerarExcel();

				if (arquivoBinario == null)
					return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);

				return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");
			}
			catch (Exception ex)
			{
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

		[AllowAuthenticate]
		public async Task<IActionResult> Adicionar()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
			try
			{
				Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

				Servicos.Embarcador.GestaoPallet.ManutencaoPallet servicoManutencaoPallet = new Servicos.Embarcador.GestaoPallet.ManutencaoPallet(unitOfWork, Auditado);

				int quantidadePallets = Request.GetIntParam("QuantidadePallets");
				string observacao = Request.GetStringParam("Observacao");
				int codigoFilial = Request.GetIntParam("Filial");
				Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManutencaoPallet tipoManutencaoPallet = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManutencaoPallet>("TipoManutencaoPallet");
				Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida tipoEntradaSaida = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida>("TipoEntradaSaida");

				if (quantidadePallets <= 0)
					throw new ControllerException("Quantidade de pallets não pode estar zerada!");

				Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo(codigoFilial);

				if (filial == null)
					throw new ControllerException("Filial não encontrada!");

				unitOfWork.Start();

				AdicionarManutencaoPallet adicionarManutencaoPallet = new AdicionarManutencaoPallet()
				{
					QuantidadePallet = quantidadePallets,
					Filial = filial,
					TipoManutencaoPallet = tipoManutencaoPallet,
					TipoMovimentacao = tipoEntradaSaida,
					Observacao = observacao
				};

				servicoManutencaoPallet.AdicionarManutencaoPallet(adicionarManutencaoPallet);

				unitOfWork.CommitChanges();

				return await Task.FromResult(new JsonpResult(true));
			}
			catch (BaseException excecao)
			{
				unitOfWork.Rollback();
				return new JsonpResult(false, true, excecao.Message);
			}
			catch (Exception excecao)
			{
				Servicos.Log.TratarErro(excecao);
				unitOfWork.Rollback();
				return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

		#endregion

		#region Métodos Privados

		private async Task<Models.Grid.Grid> ObterGridPesquisa(Repositorio.UnitOfWork unitOfWork)
		{
			Repositorio.Embarcador.GestaoPallet.ManutencaoPallet repositorioManutencaoPallet = new Repositorio.Embarcador.GestaoPallet.ManutencaoPallet(unitOfWork);

			Models.Grid.Grid grid = new Models.Grid.Grid(Request)
			{
				header = new List<Models.Grid.Head>()
			};

			grid.AdicionarCabecalho("Codigo", false);
			grid.AdicionarCabecalho("Número Nota Fiscal", "NumeroNotaFiscal", 4, Models.Grid.Align.left, true);
			grid.AdicionarCabecalho("Carga", "Carga", 4, Models.Grid.Align.left, true);
			grid.AdicionarCabecalho("Quantidade Pallets", "QuantidadePallet", 4, Models.Grid.Align.left, true);
			grid.AdicionarCabecalho("Data Recebimento", "DataCriacao", 4, Models.Grid.Align.left, true);
			grid.AdicionarCabecalho("Filial", "FilialDescricao", 7, Models.Grid.Align.left, true);
			grid.AdicionarCabecalho("TipoManutencaoPallet", false);
			grid.AdicionarCabecalho("Tipo Manutenção Pallet", "TipoManutencaoPalletDescricao", 4, Models.Grid.Align.left, false);
			grid.AdicionarCabecalho("TipoMovimentacao", false);
			grid.AdicionarCabecalho("Tipo Movimentação", "TipoMovimentacaoDescricao", 4, Models.Grid.Align.left, false);
			grid.AdicionarCabecalho("Observação", "Observacao", 9, Models.Grid.Align.left, false);

			Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "ManutencaoPallet/Pesquisa", "grid-gestao-pallet-manutencao-pallet");
			grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

			Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ManutencaoPallet.FiltroPesquisaManutencaoPallet filtroPesquisa = ObterFiltrosPesquisaManutencaoPallet();

			Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

			int totalRegistros = await repositorioManutencaoPallet.ContarManutencaoPalletAsync(filtroPesquisa, parametrosConsulta);
			List<Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ManutencaoPallet.ConsultaManutencaoPallet> listaConsultaControlePallet = (totalRegistros > 0) ? await repositorioManutencaoPallet.ObterManutencaoPalletAsync(filtroPesquisa, parametrosConsulta) : new List<Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ManutencaoPallet.ConsultaManutencaoPallet>();

			grid.setarQuantidadeTotal(totalRegistros);
			grid.AdicionaRows(listaConsultaControlePallet);

			return grid;
		}

		private Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ManutencaoPallet.FiltroPesquisaManutencaoPallet ObterFiltrosPesquisaManutencaoPallet()
		{
			Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ManutencaoPallet.FiltroPesquisaManutencaoPallet filtroPesquisa = new Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ManutencaoPallet.FiltroPesquisaManutencaoPallet
			{
				Carga = Request.GetStringParam("Carga"),
				Filial = Request.GetIntParam("Filial"),
				Cliente = Request.GetLongParam("Cliente"),
				Transportador = Request.GetIntParam("Transportador"),
				TipoMovimentacao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida>("TipoMovimentacao"),
				TipoManutencaoPallet = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManutencaoPallet>("TipoManutencaoPallet"),
				DataInicialMovimentacao = Request.GetDateTimeParam("DataInicialMovimentacao"),
				DataFinalMovimentacao = Request.GetDateTimeParam("DataFinalMovimentacao"),
			};

			return filtroPesquisa;
		}

		private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
		{
			if (propriedadeOrdenar == "Carga")
				return "Carga.CAR_CODIGO_CARGA_EMBARCADOR";

			if (propriedadeOrdenar == "FilialDescricao")
				return "Filial.FIL_DESCRICAO";

			if (propriedadeOrdenar == "QuantidadePallets")
				return "ManutencaoPallet.MNP_QUANTIDADE_PALLETS";

			if (propriedadeOrdenar == "DataRecebimentoNotaFiscal")
				return "ManutencaoPallet.MNP_DATA_CRIACAO";

			return propriedadeOrdenar;
		}

		#endregion
	}
}