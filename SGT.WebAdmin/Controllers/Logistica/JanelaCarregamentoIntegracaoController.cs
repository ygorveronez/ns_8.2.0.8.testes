using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGT.WebAdmin.Models.Grid;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/JanelaCarregamentoIntegracao")]
	public class JanelaCarregamentoIntegracaoController : BaseController
	{
		#region Construtores

		public JanelaCarregamentoIntegracaoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Globais

		[AllowAuthenticate]
		public async Task<IActionResult> Pesquisa()
		{
			try
			{
				return new JsonpResult(ObterGridPesquisa());
			}
			catch (Exception ex)
			{
				Servicos.Log.TratarErro(ex);

				return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
			}
		}


		public async Task<IActionResult> Reenviar()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

			try
			{
				Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao repositorioJanelaCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao(unitOfWork);
				Servicos.Embarcador.Logistica.CargaJanelaCarregamentoIntegracao servicoJanelaCarregamentoIntegracao = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoIntegracao(unitOfWork, TipoServicoMultisoftware);
				int codigo = Request.GetIntParam("Codigo");

				Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao janelaCarregamentoIntegracao = repositorioJanelaCarregamentoIntegracao.BuscarPorCodigo(codigo, false);

				if (janelaCarregamentoIntegracao == null)
					return new JsonpResult(false, Localization.Resources.Gerais.Geral.RegistroNaoEncontrado);

				if (!servicoJanelaCarregamentoIntegracao.ReenviarIntegracao(janelaCarregamentoIntegracao, out string mensagemErro))
					return new JsonpResult(false, true, mensagemErro);

				Servicos.Auditoria.Auditoria.Auditar(Auditado, janelaCarregamentoIntegracao, Localization.Resources.Logistica.JanelaCarregamentoIntegracao.SolicitadoReenvioDaIntegracao, unitOfWork);

				return new JsonpResult(true);
			}
			catch (Exception ex)
			{
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoIntegracao.OcorreuFalhaAoReenviarIntegracao);
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

		public async Task<IActionResult> ConsultarHistoricoIntegracao()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

			try
			{
				Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao repositorioJanelaCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao(unitOfWork);
				
				int codigo = Request.GetIntParam("Codigo");

				Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao janelaCarregamentoIntegracao = repositorioJanelaCarregamentoIntegracao.BuscarPorCodigo(codigo, false);

				if (janelaCarregamentoIntegracao == null)
					return new JsonpResult(false, Localization.Resources.Gerais.Geral.RegistroNaoEncontrado);

				Models.Grid.Grid grid = new Models.Grid.Grid(Request)
				{
					header = new List<Head>()
				};

				grid.AdicionarCabecalho("Codigo", false);
				grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.Data, "Data", 20, Models.Grid.Align.left, false);
				grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.Tipo, "DescricaoTipo", 20, Models.Grid.Align.left, false);
				grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.Retorno, "Mensagem", 40, Models.Grid.Align.left, false);

				int quantidadeRegistros = janelaCarregamentoIntegracao.ArquivosTransacao.Count();
				var registros = (from obj in janelaCarregamentoIntegracao.ArquivosTransacao.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
					select new
					{
						obj.Codigo,
						Data = obj.Data.ToString("dd/MM/yyyy HH:mm:ss"),
						obj.DescricaoTipo,
						obj.Mensagem
					}).ToList();

				grid.setarQuantidadeTotal(quantidadeRegistros);
				grid.AdicionaRows(registros);

				return new JsonpResult(grid);
			}
			catch (Exception ex)
			{
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoIntegracao.OcorreuFalhaAoConsultarHistoricoIntegracao);
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

		[AcceptVerbs("POST", "GET")]
		public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

			try
			{
				int codigo = Request.GetIntParam("Codigo");

				Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoIntegracaoArquivo repositorioJanelaCarregamentoIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoIntegracaoArquivo(unitOfWork);

				Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracaoArquivo arquivoIntegracao = repositorioJanelaCarregamentoIntegracaoArquivo.BuscarPorCodigo(codigo, false);

				if (arquivoIntegracao == null)
					return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoIntegracao.HistoricoNaoEncontrado);

				byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

				return Arquivo(arquivo, "application/zip", $"Arquivo integração janela de carregamento.zip");
			}
			catch (Exception ex)
			{
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoRealizarDownload);
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

		[AllowAuthenticate]
		public async Task<IActionResult> ExportarPesquisa()
		{
			try
			{
				var grid = ObterGridPesquisa();

				byte[] arquivoBinario = grid.GerarExcel();

				if (arquivoBinario != null)
					return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

				return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
			}
			catch (Exception excecao)
			{
				Servicos.Log.TratarErro(excecao);

				return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExportar);
			}
		}

		public async Task<IActionResult> EnviarDocumentosReprovados()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

			try
			{
				Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao repositorioJanelaCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao(unitOfWork);
				Servicos.Embarcador.Integracao.Klios.IntegracaoKlios servicoIntegracaoKlios = new Servicos.Embarcador.Integracao.Klios.IntegracaoKlios(unitOfWork, TipoServicoMultisoftware);
				int codigo = Request.GetIntParam("Codigo");

				Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao janelaCarregamentoIntegracao = repositorioJanelaCarregamentoIntegracao.BuscarPorCodigo(codigo, false);

				if (janelaCarregamentoIntegracao == null)
					return new JsonpResult(false, Localization.Resources.Gerais.Geral.RegistroNaoEncontrado);

				servicoIntegracaoKlios.EnviarDocumentosReprovadosIntegracao(janelaCarregamentoIntegracao);			

				Servicos.Auditoria.Auditoria.Auditar(Auditado, janelaCarregamentoIntegracao, Localization.Resources.Logistica.JanelaCarregamentoIntegracao.SolicitadoReenvioDaIntegracao, unitOfWork);

				return new JsonpResult(true);
			}
			catch (Exception ex)
			{
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoIntegracao.OcorreuFalhaAoReenviarIntegracao);
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

		#endregion

		#region Métodos Privados

		private Models.Grid.Grid ObterGridPesquisa()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

			try
			{
				Models.Grid.Grid grid = new Models.Grid.Grid(Request)
				{
					header = new List<Models.Grid.Head>()
				};

				grid.AdicionarCabecalho("Codigo", false);
				grid.AdicionarCabecalho("TipoRetornoRecebimento", false);
				grid.AdicionarCabecalho("TipoEvento", false);
				grid.AdicionarCabecalho("SituacaoIntegracao", false);
				grid.AdicionarCabecalho("Nº da Carga", "NumeroCargaEmbarcador", 10, Models.Grid.Align.left, false);
				grid.AdicionarCabecalho("Data da Carga", "DataCarga", 10, Models.Grid.Align.center, false);
				grid.AdicionarCabecalho("Tipo da Carga", "TipoCarga", 10, Models.Grid.Align.left, false);
				grid.AdicionarCabecalho("Filial", "Filial", 10, Models.Grid.Align.left, false);
				grid.AdicionarCabecalho("Centro de Carregamento", "CentroCarregamento", 10, Models.Grid.Align.left, false);
				grid.AdicionarCabecalho("Tipo", "TipoRetornoRecebimentoDescricao", 10, Models.Grid.Align.center, false);
				grid.AdicionarCabecalho("Tipo de Evento", "TipoEventoDescricao", 10, Models.Grid.Align.center, false);
				grid.AdicionarCabecalho("Situação", "SituacaoIntegracaoDescricao", 10, Models.Grid.Align.left, false);
				grid.AdicionarCabecalho("Retorno", "Mensagem", 10, Models.Grid.Align.left, false);

				Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao repsitorioJanelaCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao(unitOfWork);
				Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJanelaCarregamentoIntegracao filtrosPesquisa = ObterFiltrosPesquisa();

				Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

				int totalRegistros = repsitorioJanelaCarregamentoIntegracao.ContarConsulta(filtrosPesquisa);

				List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao> listaJanelaCarregamentoIntegracao = totalRegistros > 0 ? repsitorioJanelaCarregamentoIntegracao.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao>();

				var lista = (from obj in listaJanelaCarregamentoIntegracao
							 select new
							 {
								 obj.Codigo,
								 obj.TipoRetornoRecebimento,
								 obj.TipoEvento,
								 obj.SituacaoIntegracao,
								 NumeroCargaEmbarcador = ObterNumeroCargaEmbarcador(obj),
								 DataCarga = ObterDataCarga(obj),
								 TipoCarga = ObterTipoCarga(obj),
								 Filial = ObterFilial(obj),
								 CentroCarregamento = ObterCentroCarregamento(obj),
								 obj.TipoRetornoRecebimentoDescricao,
								 obj.TipoEventoDescricao,
								 SituacaoIntegracaoDescricao = obj.SituacaoIntegracao.ObterDescricao(),
								 obj.Mensagem
							 }).ToList();

				if (lista.Any(o => o.TipoEvento == 0))
					grid.OcultarCabecalhos(new string[]{"TipoEventoDescricao", "TipoRetornoRecebimentoDescricao"});

				grid.AdicionaRows(lista);
				grid.setarQuantidadeTotal(totalRegistros);

				return grid;
			}
			catch (Exception ex)
			{
				throw;
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

		private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJanelaCarregamentoIntegracao ObterFiltrosPesquisa()
		{
			return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJanelaCarregamentoIntegracao()
			{
				NumeroCargaEmbarcador = Request.GetStringParam("NumeroCargaEmbarcador"),
				DataCargaInicial = Request.GetNullableDateTimeParam("DataInicio"),
				DataCargaFinal = Request.GetNullableDateTimeParam("DataFim"),
				SituacaoIntegracao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao>("SituacaoIntegracao"),
				CodigoFilial = Request.GetIntParam("Filial"),
				CodigoCentroCarregamento = Request.GetIntParam("CentroCarregamento"),
				NumeroViagem = Request.GetStringParam("NumeroShipment")
			};
		}

		private string ObterNumeroCargaEmbarcador(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao janelaCarregamentoIntegracao)
		{
			if (janelaCarregamentoIntegracao.CargaJanelaCarregamento != null)
				return janelaCarregamentoIntegracao.CargaJanelaCarregamento.Carga.CodigoCargaEmbarcador ?? string.Empty;

			if (janelaCarregamentoIntegracao.CargaJanelaCarregamentoViagem != null)
				return string.Join(", ", (from o in janelaCarregamentoIntegracao.CargaJanelaCarregamentoViagem.CargasJanelaCarregamento select o.Carga.CodigoCargaEmbarcador ?? string.Empty).Distinct());

			return string.Empty;
		}

		private string ObterTipoCarga(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao janelaCarregamentoIntegracao)
		{
			if (janelaCarregamentoIntegracao.CargaJanelaCarregamento != null)
				return janelaCarregamentoIntegracao.CargaJanelaCarregamento.Carga?.TipoDeCarga?.Descricao ?? string.Empty;

			if (janelaCarregamentoIntegracao.CargaJanelaCarregamentoViagem != null)
				return string.Join(", ", (from o in janelaCarregamentoIntegracao.CargaJanelaCarregamentoViagem.CargasJanelaCarregamento select o.Carga?.TipoDeCarga?.Descricao ?? string.Empty).Distinct());

			return string.Empty;
		}

		private string ObterDataCarga(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao janelaCarregamentoIntegracao)
		{
			if (janelaCarregamentoIntegracao.CargaJanelaCarregamento != null)
				return janelaCarregamentoIntegracao.CargaJanelaCarregamento.Carga?.DataCriacaoCarga.ToString("dd/MM/yyyy");

			if (janelaCarregamentoIntegracao.CargaJanelaCarregamentoViagem != null)
				return string.Join(", ", (from o in janelaCarregamentoIntegracao.CargaJanelaCarregamentoViagem.CargasJanelaCarregamento select o.Carga?.DataCriacaoCarga.ToString("dd/MM/yyyy")).Distinct());

			return string.Empty;
		}

		private string ObterFilial(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao janelaCarregamentoIntegracao)
		{
			if (janelaCarregamentoIntegracao.CargaJanelaCarregamento != null)
				return janelaCarregamentoIntegracao.CargaJanelaCarregamento.Carga?.Filial?.Descricao ?? string.Empty;

			if (janelaCarregamentoIntegracao.CargaJanelaCarregamentoViagem != null)
				return string.Join(", ", (from o in janelaCarregamentoIntegracao.CargaJanelaCarregamentoViagem.CargasJanelaCarregamento select o.Carga?.Filial?.Descricao ?? string.Empty).Distinct());

			return string.Empty;
		}

		private string ObterCentroCarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao janelaCarregamentoIntegracao)
		{
			if (janelaCarregamentoIntegracao.CargaJanelaCarregamento != null)
				return janelaCarregamentoIntegracao.CargaJanelaCarregamento.CentroCarregamento?.Descricao ?? string.Empty;

			if (janelaCarregamentoIntegracao.CargaJanelaCarregamentoViagem != null)
				return string.Join(", ", (from o in janelaCarregamentoIntegracao.CargaJanelaCarregamentoViagem.CargasJanelaCarregamento select o.CentroCarregamento?.Descricao ?? string.Empty).Distinct());

			return string.Empty;
		}

		#endregion

	}
}
