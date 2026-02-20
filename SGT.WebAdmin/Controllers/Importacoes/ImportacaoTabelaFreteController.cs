using Dominio.Entidades.Embarcador.Frete;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using OfficeOpenXml;
using SGTAdmin.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Importacoes
{
	[CustomAuthorize("Importacoes/ImportacaoTabelaFrete")]
	public class ImportacaoTabelaFreteController : BaseController
	{
		#region Construtores

		public ImportacaoTabelaFreteController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Globais

		[AllowAuthenticate]
		public async Task<IActionResult> ConsultarLayoutImportacao()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

			try
			{
				int codigoTabelaFrete = Request.GetIntParam("CodigoTabelaFrete");

				var grid = new Models.Grid.Grid(Request)
				{
					header = new List<Models.Grid.Head>()
				};

				grid.AdicionarCabecalho("Codigo", false);
				grid.AdicionarCabecalho("Descricao", "Descricao", 35, Models.Grid.Align.left, true);

				Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLayout repositorioImportacaoLayout = new Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLayout(unitOfWork);
				List<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLayout> listaLayouts = repositorioImportacaoLayout.BuscarLayoutsPorTabelaFrete(codigoTabelaFrete);

				grid.setarQuantidadeTotal(listaLayouts.Count);

				grid.AdicionaRows((
					from o in listaLayouts
					select new
					{
						o.Codigo,
						o.Descricao,
					}).ToList()
				);

				return new JsonpResult(grid);
			}
			catch (Exception ex)
			{
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

		[AllowAuthenticate]
		public async Task<IActionResult> SalvarLayoutAtual()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

			try
			{
				int codigoTabelaFrete = Request.GetIntParam("CodigoTabelaFrete");
				string descricao = Request.GetStringParam("Descricao");
				string jsonColunas = Request.GetStringParam("JSONColunas");

				Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLayout repositorioImportacaoLayout = new Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLayout(unitOfWork);
				Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork);

				Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = repTabelaFrete.BuscarPorCodigo(codigoTabelaFrete);

				if (tabelaFrete == null)
					return new JsonpResult(false, "Falha ao buscar Tabela de Frete");

				Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLayout importacaoTabelaFreteLayout = new Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLayout
				{
					Descricao = descricao,
					JSONLayout = jsonColunas
				};

				repositorioImportacaoLayout.Inserir(importacaoTabelaFreteLayout);

				tabelaFrete.Layouts.Add(importacaoTabelaFreteLayout);
				repTabelaFrete.Atualizar(tabelaFrete);

				return new JsonpResult(true);
			}
			catch (Exception ex)
			{
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao salvar o Layout de Importação");
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

		[AllowAuthenticate]
		public async Task<IActionResult> BuscarLayoutPorCodigo()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

			try
			{
				int codigoTabelaFrete = Request.GetIntParam("CodigoTabelaFrete");
				int codigoLayout = Request.GetIntParam("Codigo");

				Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLayout repositorioImportacaoLayout = new Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLayout(unitOfWork);
				Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork);

				Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = repTabelaFrete.BuscarPorCodigo(codigoTabelaFrete);

				if (tabelaFrete == null)
					return new JsonpResult(false, "Falha ao buscar Tabela de Frete");

				Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLayout layoutImportacao = tabelaFrete.Layouts.Where(o => o.Codigo == codigoLayout).FirstOrDefault();

				if (layoutImportacao == null)
					return new JsonpResult(false, "Layout não encontrado");

				return new JsonpResult(layoutImportacao.JSONLayout);
			}
			catch (Exception ex)
			{
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao salvar o Layout de Importação");
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

		[AllowAuthenticate]
		public async Task<IActionResult> ExcluirLayoutPorCodigo()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

			try
			{
				int codigoTabelaFrete = Request.GetIntParam("CodigoTabelaFrete");
				int codigoLayout = Request.GetIntParam("Codigo");

				Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLayout repositorioImportacaoLayout = new Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLayout(unitOfWork);
				Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork);

				Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = repTabelaFrete.BuscarPorCodigo(codigoTabelaFrete);

				if (tabelaFrete == null)
					return new JsonpResult(false, "Falha ao buscar Tabela de Frete");

				Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLayout layoutImportacao = tabelaFrete.Layouts.Where(o => o.Codigo == codigoLayout).FirstOrDefault();

				if (layoutImportacao == null)
					return new JsonpResult(false, "Layout não encontrado");

				tabelaFrete.Layouts.Remove(layoutImportacao);
				repositorioImportacaoLayout.Deletar(layoutImportacao);

				return new JsonpResult(true);
			}
			catch (Exception ex)
			{
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao salvar o Layout de Importação");
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

		[Obsolete("Este método será substituído pelo método ImportarPorServico")]
		public async Task<IActionResult> Importar()
		{
			Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

			try
			{
				List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

				if (files.Count <= 0)
					return new JsonpResult(false, "Selecione um arquivo para envio.");

				Servicos.DTO.CustomFile file = files[0];

				string extensao = Path.GetExtension(file.FileName).ToLowerInvariant();

				if (extensao != ".xlsx")
					return new JsonpResult(false, "Extensão do arquivo inválida. Selecione um arquivo com a extensão .xlsx.");

				dynamic parametros = JsonConvert.DeserializeObject<object>(GetParametroImportacao("Parametros"));

				if (parametros == null || parametros.Count <= 0)
					return new JsonpResult(false, "Adicione os parâmetros para importação da tabela de frete.");

				bool existeParametroEntregaExcedente = false;
				bool existeParametroPorEntrega = false;

				foreach (dynamic parametro in parametros)
				{
					string item = (string)parametro.ItemParametroBase;

					if (!existeParametroPorEntrega)
						existeParametroPorEntrega = item.StartsWith("5_");

					if (!existeParametroEntregaExcedente)
						existeParametroEntregaExcedente = item.Contains("EntregaExcedente_");
				}

				if (existeParametroEntregaExcedente && !existeParametroPorEntrega)
					return new JsonpResult(false, "Obrigatório informar coluna para valor por entrega quando tiver coluna para entrega excedente.");

				int codigoDestino = GetParametroImportacao("Destino").ToInt();
				int codigoEmpresa = GetParametroImportacao("Empresa").ToInt();
				int codigoOrigem = GetParametroImportacao("Origem").ToInt();
				int codigoTabelaFrete = GetParametroImportacao("TabelaFrete").ToInt();
				int codigoTipoOperacao = GetParametroImportacao("TipoOperacao").ToInt();
				int codigoVigencia = GetParametroImportacao("Vigencia").ToInt();

				Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? moeda = GetParametroImportacao("Moeda").ToNullableEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral>();
				Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoEmissao? tipoPagamento = GetParametroImportacao("TipoPagamento").ToNullableEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoEmissao>();

				int colunaCEPDestino = GetParametroImportacao("ColunaCEPDestino").ToInt();
				int colunaCEPOrigem = GetParametroImportacao("ColunaCEPOrigem").ToInt();
				int colunaCEPDestinoDiasUteis = GetParametroImportacao("ColunaCEPDestinoDiasUteis").ToInt();
				int colunaColunaLeadTime = GetParametroImportacao("ColunaLeadTime").ToInt();
				int colunaTomadorDestino = GetParametroImportacao("ColunaTomadorDestino").ToInt();
				int colunaClienteDestino = GetParametroImportacao("ColunaClienteDestino").ToInt();
				int colunaClienteOrigem = GetParametroImportacao("ColunaClienteOrigem").ToInt();
				int colunaCodigoIntegracao = GetParametroImportacao("ColunaCodigoIntegracao").ToInt();
				int colunaDestino = GetParametroImportacao("ColunaDestino").ToInt();
				int colunaEstadoDestino = GetParametroImportacao("ColunaEstadoDestino").ToInt();
				int colunaEstadoOrigem = GetParametroImportacao("ColunaEstadoOrigem").ToInt();
				int colunaOrigem = GetParametroImportacao("ColunaOrigem").ToInt();
				int colunaRotaDestino = GetParametroImportacao("ColunaRotaDestino").ToInt();
				int colunaRegiaoOrigem = GetParametroImportacao("ColunaRegiaoOrigem").ToInt();
				int colunaRegiaoDestino = GetParametroImportacao("ColunaRegiaoDestino").ToInt();
				int colunaRotaOrigem = GetParametroImportacao("ColunaRotaOrigem").ToInt();
				int colunaTransportador = GetParametroImportacao("ColunaTransportador").ToInt();
				int colunaPrioridadeUso = GetParametroImportacao("ColunaPrioridadeUso").ToInt();
				int colunaFronteira = GetParametroImportacao("ColunaFronteira").ToInt();
				int colunaKMSistema = GetParametroImportacao("ColunaKMSistema").ToInt();
				int colunaParametroBase = GetParametroImportacao("ColunaParametrosBase").ToInt();
				int linhaInicioDados = GetParametroImportacao("LinhaInicioDados").ToInt();
				int colunaCanalEntrega = GetParametroImportacao("ColunaCanalEntrega").ToInt();
				int colunaCanalVenda = GetParametroImportacao("ColunaCanalVenda").ToInt();
				int colunaTipoOperacao = GetParametroImportacao("ColunaTipoOperacao").ToInt();
				int colunaTipoDeCarga = GetParametroImportacao("ColunaTipoCarga").ToInt();

				bool freteValidoParaQualquerDestino = GetParametroImportacao("FreteValidoParaQualquerDestino").ToBool();
				bool freteValidoParaQualquerOrigem = GetParametroImportacao("FreteValidoParaQualquerOrigem").ToBool();
				bool naoAtualizarValoresZerados = GetParametroImportacao("NaoAtualizarValoresZerados").ToBool();

				int colunaSeg = GetParametroImportacao("ColunaSeg").ToInt();
				int colunaTer = GetParametroImportacao("ColunaTer").ToInt();
				int colunaQua = GetParametroImportacao("ColunaQua").ToInt();
				int colunaQui = GetParametroImportacao("ColunaQui").ToInt();
				int colunaSex = GetParametroImportacao("ColunaSex").ToInt();
				int colunaSab = GetParametroImportacao("ColunaSab").ToInt();
				int colunaDom = GetParametroImportacao("ColunaDom").ToInt();

				int colunaPercentualRota = GetParametroImportacao("ColunaPercentualRota").ToInt();
				int colunaQuantidadeEntregas = GetParametroImportacao("ColunaQuantidadeEntregas").ToInt();
				int colunaCapacidadeOTM = GetParametroImportacao("ColunaCapacidadeOTM").ToInt();
				int colunaDominioOTM = GetParametroImportacao("ColunaDominioOTM").ToInt();
				int colunaTipoIntegracao = GetParametroImportacao("ColunaTipoIntegracao").ToInt();
				int colunaPontoPlanejamentoTransporte = GetParametroImportacao("ColunaPontoPlanejamentoTransporte").ToInt();
				int colunaContratoTransportador = GetParametroImportacao("ColunaContratoTransportador").ToInt();
				int colunaGrupoCarga = GetParametroImportacao("ColunaGrupoCarga").ToInt();
				int colunaGerenciarCapacidade = GetParametroImportacao("ColunaGerenciarCapacidade").ToInt();
				int colunaEstruturaTabela = GetParametroImportacao("ColunaEstruturaTabela").ToInt();
				int colunaLeadTimeDias = GetParametroImportacao("ColunaLeadTimeDias").ToInt();
				int colunaObservacaoInterna = GetParametroImportacao("ColunaObservacaoInterna").ToInt();

				Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
				Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeDeTrabalho);
				Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unidadeDeTrabalho);
				Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
				Repositorio.Embarcador.Frete.VigenciaTabelaFrete repVigencia = new Repositorio.Embarcador.Frete.VigenciaTabelaFrete(unidadeDeTrabalho);
				Repositorio.Embarcador.Frete.TabelaFreteCliente repTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unidadeDeTrabalho);
				Repositorio.Embarcador.Frete.ParametroBaseCalculoTabelaFrete repParametro = new Repositorio.Embarcador.Frete.ParametroBaseCalculoTabelaFrete(unidadeDeTrabalho);
				Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete repItemParametroBase = new Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete(unidadeDeTrabalho);
				Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
				Repositorio.Embarcador.Frete.TabelaFreteClienteCEPOrigem repTabelaFreteClienteCEPOrigem = new Repositorio.Embarcador.Frete.TabelaFreteClienteCEPOrigem(unidadeDeTrabalho);
				Repositorio.Embarcador.Frete.TabelaFreteClienteCEPDestino repTabelaFreteClienteCEPDestino = new Repositorio.Embarcador.Frete.TabelaFreteClienteCEPDestino(unidadeDeTrabalho);
				Repositorio.Embarcador.Frete.TabelaFreteClienteFrequenciaEntrega repTabelaFreteClienteFrequenciaEntrega = new Repositorio.Embarcador.Frete.TabelaFreteClienteFrequenciaEntrega(unidadeDeTrabalho);
				Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete repConfiguracaoTabelaFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete(unidadeDeTrabalho);
				Repositorio.Embarcador.Pedidos.CanalEntrega repositorioCanalEntrega = new Repositorio.Embarcador.Pedidos.CanalEntrega(unidadeDeTrabalho);
				Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeDeTrabalho);

				Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete = repConfiguracaoTabelaFrete.BuscarPrimeiroRegistro();

				Servicos.Embarcador.Frete.TabelaFreteAprovacaoAlcada servicoAprovacaoTabelaFrete = new Servicos.Embarcador.Frete.TabelaFreteAprovacaoAlcada(unidadeDeTrabalho, notificarUsuario: false);
				Servicos.Embarcador.Frete.TabelaFreteIntegracao servicoTabelaFreteIntegracao = new Servicos.Embarcador.Frete.TabelaFreteIntegracao(unidadeDeTrabalho);
				Servicos.Embarcador.Frete.TabelaFreteCliente servicoTabelaFreteCliente = new Servicos.Embarcador.Frete.TabelaFreteCliente(unidadeDeTrabalho);
				Servicos.Embarcador.Frete.TabelaFreteClienteIntegracao servicoTabelaFreteClienteIntegracao = new Servicos.Embarcador.Frete.TabelaFreteClienteIntegracao(unidadeDeTrabalho);
				Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = repTabelaFrete.BuscarPorCodigo(codigoTabelaFrete);

				if (tabelaFrete == null)
					return new JsonpResult(false, "Tabela de frete não encontrada.");

				Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete vigencia = repVigencia.BuscarPorCodigo(codigoVigencia);

				if (vigencia == null)
					return new JsonpResult(false, "Vigência não encontrada.");

				Dominio.Entidades.Empresa empresa = codigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresa) : null;
				Dominio.Entidades.Localidade origem = codigoOrigem > 0 ? repLocalidade.BuscarPorCodigoFetch(codigoOrigem) : null;
				Dominio.Entidades.Localidade destino = codigoDestino > 0 ? repLocalidade.BuscarPorCodigoFetch(codigoDestino) : null;
				Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = codigoTipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao) : null;
				bool vincularFiliaisClientesRelacionadas = repCliente.ExisteClienteComFiliaisClientesRelacionadas();
				bool validarPermissaoSolicitarAprovacao = repositorioTipoIntegracao.ExistePorTipo(TipoIntegracao.LBC);

				if ((origem == null) && (colunaOrigem <= 0) && (colunaClienteOrigem <= 0) && (colunaEstadoOrigem <= 0) && (colunaCEPOrigem <= 0) && (colunaRotaOrigem <= 0) && (colunaRegiaoOrigem <= 0))
					return new JsonpResult(false, "É necessário selecionar a origem ou informar uma das colunas de origem (origem, estado, remetente, CEP, rota ou região).");

				if ((destino == null) && (colunaDestino <= 0) && (colunaClienteDestino <= 0) && (colunaEstadoDestino <= 0) && (colunaCEPDestino <= 0) && (colunaRotaDestino <= 0) && (colunaRegiaoDestino <= 0))
					return new JsonpResult(false, "É necessário selecionar o destino ou informar uma das colunas de destino (destino, estado, destinatário, CEP, rota ou região).");

				if (linhaInicioDados <= 0)
					return new JsonpResult(false, "Linha de início dos dados inválida.");

				List<Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosBaseTabelaFrete> listaParametrosBaseTabelaFrete = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosBaseTabelaFrete>();
				if (colunaParametroBase > 0)
				{
					if (tabelaFrete.ParametroBase != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ModeloReboque)
						return new JsonpResult(false, "Informação da Coluna para parametro base disponível apenas para o Modelos de Reboque.");

					if (tabelaFrete.ModelosReboque == null || tabelaFrete.ModelosReboque.Count == 0)
						return new JsonpResult(false, "Tabela de frete sem modelos de reboques configurado.");

					listaParametrosBaseTabelaFrete = ModelosReboqueParaParametrosBase(tabelaFrete.ModelosReboque.ToList());

					if (listaParametrosBaseTabelaFrete == null || listaParametrosBaseTabelaFrete.Count == 0)
						return new JsonpResult(false, "Não foi possível carregar lista de parametros base Modelo Reboque.");
				}

				ExcelPackage package = new ExcelPackage(file.InputStream);

                if (package == null || package.Workbook.Worksheets.Count == 0)
					return new JsonpResult(false, "Não foi possível carregar a planilha.");

				ExcelWorksheet worksheet = package.Workbook.Worksheets.First();
				List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.RetornoImportacaoTabelaFrete> erros = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.RetornoImportacaoTabelaFrete>();
				System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("pt-BR");

				// Itera sobre as linhas da tabela
				for (var i = linhaInicioDados; i <= worksheet.Dimension.End.Row; i++)
				{
					unidadeDeTrabalho.FlushAndClear();

					int? prioridadeUso = null;
					int? quantidadadeEntregas = null;
					decimal? percentualRota = null;

					bool? capacidadeOtm = null;
					bool gerenciarCapacidade = false;

					Dominio.ObjetosDeValor.Embarcador.Enumeradores.DominioOTM? dominioOTM = null;
					Dominio.ObjetosDeValor.Embarcador.Enumeradores.PontoPlanejamentoTransporte? pontoPlanejamentoTransporte = null;
					Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoUnilever? tipoIntegracao = null;
					Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGrupoCarga tipoGrupoCarga = TipoGrupoCarga.Nenhum;
					Dominio.ObjetosDeValor.Embarcador.Enumeradores.EstruturaTabela? estruturaTabela = EstruturaTabela.CustoFixo;

					List<Dominio.Entidades.Cliente> fronteiras = null;
					List<Dominio.Entidades.Localidade> localidadesOrigem = new List<Dominio.Entidades.Localidade>();
					List<Dominio.Entidades.Localidade> localidadesDestino = new List<Dominio.Entidades.Localidade>();
					List<Dominio.Entidades.Cliente> clientesDestino = null;
					List<Dominio.Entidades.Cliente> clientesOrigem = null;
					List<Dominio.Entidades.RotaFrete> rotasOrigem = null;
					List<Dominio.Entidades.RotaFrete> rotasDestino = null;
					List<Dominio.Entidades.Embarcador.Localidades.Regiao> regioesOrigem = null;
					List<Dominio.Entidades.Embarcador.Localidades.Regiao> regioesDestino = null;
					List<Dominio.ObjetosDeValor.Embarcador.Frete.FaixaCEP> cepsOrigem = new List<Dominio.ObjetosDeValor.Embarcador.Frete.FaixaCEP>();
					List<Dominio.ObjetosDeValor.Embarcador.Frete.FaixaCEP> cepsDestino = new List<Dominio.ObjetosDeValor.Embarcador.Frete.FaixaCEP>();
					Dominio.Entidades.Empresa empresaPorTabelaFreteCliente = null;
					Dominio.Entidades.Embarcador.Pedidos.CanalEntrega canalEntrega = null;
					Dominio.Entidades.Embarcador.Pedidos.CanalVenda canalVenda = null;
					Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete contratoTransportador = null;

					List<Dominio.Entidades.Estado> estadosDestino = null;
					List<Dominio.Entidades.Estado> estadosOrigem = null;
					List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacao = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();
					List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tiposDeCarga = new List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();
					Dominio.Entidades.Cliente tomador = null;

					Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.RetornoImportacaoTabelaFrete linhaRetorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.RetornoImportacaoTabelaFrete();

					if (configuracaoTabelaFrete.ImportarTabelaFreteClienteInformandoOrigensDestinosEmDiferentesColunasNoMesmoArquivo)
					{
						List<IList> origens = new List<IList>();
						List<IList> destinos = new List<IList>();

						if (origem != null)
						{
							localidadesOrigem.Add(origem);
						}
						if (colunaOrigem > 0)
						{
							string descricaoOrigem = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaOrigem].Text);
							string descricaoEstadoOrigem = colunaEstadoOrigem > 0 ? worksheet.Cells[i, colunaEstadoOrigem].Text : string.Empty;

							ObterLocalidades(out string erro, out localidadesOrigem, descricaoOrigem, descricaoEstadoOrigem, unidadeDeTrabalho);

							origens.Add(localidadesOrigem);
						}
						if (colunaClienteOrigem > 0)
						{
							ObterClientes(out string erro, out clientesOrigem, worksheet.Cells[i, colunaClienteOrigem].Text, unidadeDeTrabalho, vincularFiliaisClientesRelacionadas, fronteira: false);

							origens.Add(clientesOrigem);
						}
						if (colunaCEPOrigem > 0)
						{
							ObterCEPs(out string erro, out cepsOrigem, worksheet.Cells[i, colunaCEPOrigem].Text);

							origens.Add(cepsOrigem);
						}
						if (colunaRotaOrigem > 0)
						{
							ObterRotas(out string erro, out rotasOrigem, worksheet.Cells[i, colunaRotaOrigem].Text, unidadeDeTrabalho);

							origens.Add(rotasOrigem);
						}
						if (colunaRegiaoOrigem > 0)
						{
							ObterRegioes(out string erro, out regioesOrigem, worksheet.Cells[i, colunaRegiaoOrigem].Text, unidadeDeTrabalho);

							origens.Add(regioesOrigem);
						}
						if (localidadesOrigem.Count <= 0 && colunaEstadoOrigem > 0)
						{
							ObterEstados(out string erro, out estadosOrigem, worksheet.Cells[i, colunaEstadoOrigem].Text, unidadeDeTrabalho);

							origens.Add(estadosOrigem);
						}

						int quantidadeColunasPreenchidas = 0;
						foreach (IList list in origens)
						{
							if (list.Count > 0)
								quantidadeColunasPreenchidas++;
						}

						if (quantidadeColunasPreenchidas == 0)
						{
							linhaRetorno.CodigoErro = i;
							linhaRetorno.LinhaErro = $"Linha {i}";
							linhaRetorno.DescricaoErro = "Nenhum tipo de Origem preenchido";
							erros.Add(linhaRetorno);

							continue;
						}
						else if (quantidadeColunasPreenchidas > 1)
						{
							linhaRetorno.CodigoErro = i;
							linhaRetorno.LinhaErro = $"Linha {i}";
							linhaRetorno.DescricaoErro = "Não é permitido informar mais de um tipo de Origem";
							erros.Add(linhaRetorno);

							continue;
						}

						if (destino != null)
						{
							localidadesDestino.Add(destino);
						}
						if (colunaDestino > 0)
						{
							string descricaoDestino = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaDestino].Text);
							string descricaoEstadoDestino = colunaEstadoDestino > 0 ? worksheet.Cells[i, colunaEstadoDestino].Text : string.Empty;

							ObterLocalidades(out string erro, out localidadesDestino, descricaoDestino, descricaoEstadoDestino, unidadeDeTrabalho);

							destinos.Add(localidadesDestino);
						}
						if (localidadesDestino.Count <= 0 && colunaEstadoDestino > 0)
						{
							string descricaoEstadoDestino = colunaEstadoDestino > 0 ? worksheet.Cells[i, colunaEstadoDestino].Text : string.Empty;

							ObterEstados(out string erro, out estadosDestino, descricaoEstadoDestino, unidadeDeTrabalho);

							destinos.Add(estadosDestino);
						}
						if (colunaClienteDestino > 0)
						{
							ObterClientes(out string erro, out clientesDestino, worksheet.Cells[i, colunaClienteDestino].Text, unidadeDeTrabalho, vincularFiliaisClientesRelacionadas, fronteira: false);

							destinos.Add(clientesDestino);
						}
						if (colunaCEPDestino > 0)
						{
							string diasUteis = colunaCEPDestinoDiasUteis > 0 ? worksheet.Cells[i, colunaCEPDestinoDiasUteis].Text : string.Empty;

							ObterCEPs(out string erro, out cepsDestino, worksheet.Cells[i, colunaCEPDestino].Text, diasUteis);

							destinos.Add(cepsDestino);
						}
						if (colunaRotaDestino > 0)
						{
							ObterRotas(out string erro, out rotasDestino, worksheet.Cells[i, colunaRotaDestino].Text, unidadeDeTrabalho);

							destinos.Add(rotasDestino);
						}
						if (colunaRegiaoDestino > 0)
						{
							ObterRegioes(out string erro, out regioesDestino, worksheet.Cells[i, colunaRegiaoDestino].Text, unidadeDeTrabalho);

							destinos.Add(regioesDestino);
						}

						quantidadeColunasPreenchidas = 0;
						foreach (IList list in destinos)
						{
							if (list.Count > 0)
								quantidadeColunasPreenchidas++;
						}

						if (quantidadeColunasPreenchidas == 0)
						{
							linhaRetorno.CodigoErro = i;
							linhaRetorno.LinhaErro = $"Linha {i}";
							linhaRetorno.DescricaoErro = "Nenhum tipo de Destino preenchido";
							erros.Add(linhaRetorno);

							continue;
						}
						else if (quantidadeColunasPreenchidas > 1)
						{
							linhaRetorno.CodigoErro = i;
							linhaRetorno.LinhaErro = $"Linha {i}";
							linhaRetorno.DescricaoErro = "Não é permitido informar mais de um tipo de Destino";
							erros.Add(linhaRetorno);

							continue;
						}
					}
					else
					{
						if (origem != null)
						{
							localidadesOrigem.Add(origem);
						}
						else if (colunaOrigem > 0)
						{
							string descricaoOrigem = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaOrigem].Text);
							string descricaoEstadoOrigem = colunaEstadoOrigem > 0 ? worksheet.Cells[i, colunaEstadoOrigem].Text : string.Empty;

							if (!ObterLocalidades(out string erro, out localidadesOrigem, descricaoOrigem, descricaoEstadoOrigem, unidadeDeTrabalho))
							{
								linhaRetorno.CodigoErro = i;
								linhaRetorno.LinhaErro = $"Linha {i}";
								linhaRetorno.DescricaoErro = erro;
								erros.Add(linhaRetorno);

								continue;
							}
						}
						else if (colunaClienteOrigem > 0)
						{
							if (!ObterClientes(out string erro, out clientesOrigem, worksheet.Cells[i, colunaClienteOrigem].Text, unidadeDeTrabalho, vincularFiliaisClientesRelacionadas, fronteira: false))
							{
								linhaRetorno.CodigoErro = i;
								linhaRetorno.LinhaErro = $"Linha {i}";
								linhaRetorno.DescricaoErro = erro;
								erros.Add(linhaRetorno);

								continue;
							}
						}
						else if (colunaCEPOrigem > 0)
						{
							if (!ObterCEPs(out string erro, out cepsOrigem, worksheet.Cells[i, colunaCEPOrigem].Text))
							{
								linhaRetorno.CodigoErro = i;
								linhaRetorno.LinhaErro = $"Linha {i}";
								linhaRetorno.DescricaoErro = erro;
								erros.Add(linhaRetorno);

								continue;
							}
						}
						else if (colunaRotaOrigem > 0)
						{
							if (!ObterRotas(out string erro, out rotasOrigem, worksheet.Cells[i, colunaRotaOrigem].Text, unidadeDeTrabalho))
							{
								linhaRetorno.CodigoErro = i;
								linhaRetorno.LinhaErro = $"Linha {i}";
								linhaRetorno.DescricaoErro = erro;
								erros.Add(linhaRetorno);

								continue;
							}
						}
						else if (colunaRegiaoOrigem > 0)
						{
							if (!ObterRegioes(out string erro, out regioesOrigem, worksheet.Cells[i, colunaRegiaoOrigem].Text, unidadeDeTrabalho))
							{
								linhaRetorno.CodigoErro = i;
								linhaRetorno.LinhaErro = $"Linha {i}";
								linhaRetorno.DescricaoErro = erro;
								erros.Add(linhaRetorno);

								continue;
							}
						}
						else if (colunaOrigem <= 0 && colunaEstadoOrigem > 0)
						{
							if (!ObterEstados(out string erro, out estadosOrigem, worksheet.Cells[i, colunaEstadoOrigem].Text, unidadeDeTrabalho))
							{
								linhaRetorno.CodigoErro = i;
								linhaRetorno.LinhaErro = $"Linha {i}";
								linhaRetorno.DescricaoErro = erro;
								erros.Add(linhaRetorno);

								continue;
							}
						}

						if (destino != null)
						{
							localidadesDestino.Add(destino);
						}
						else if (colunaDestino > 0)
						{
							string descricaoDestino = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaDestino].Text);
							string descricaoEstadoDestino = colunaEstadoDestino > 0 ? worksheet.Cells[i, colunaEstadoDestino].Text : string.Empty;

							if (!ObterLocalidades(out string erro, out localidadesDestino, descricaoDestino, descricaoEstadoDestino, unidadeDeTrabalho))
							{
								linhaRetorno.CodigoErro = i;
								linhaRetorno.LinhaErro = $"Linha {i}";
								linhaRetorno.DescricaoErro = erro;
								erros.Add(linhaRetorno);

								continue;
							}
						}
						else if (colunaDestino <= 0 && colunaEstadoDestino > 0)
						{
							string descricaoEstadoDestino = colunaEstadoDestino > 0 ? worksheet.Cells[i, colunaEstadoDestino].Text : string.Empty;

							if (!ObterEstados(out string erro, out estadosDestino, descricaoEstadoDestino, unidadeDeTrabalho))
							{
								linhaRetorno.CodigoErro = i;
								linhaRetorno.LinhaErro = $"Linha {i}";
								linhaRetorno.DescricaoErro = erro;
								erros.Add(linhaRetorno);

								continue;
							}
						}
						else if (colunaClienteDestino > 0)
						{
							if (!ObterClientes(out string erro, out clientesDestino, worksheet.Cells[i, colunaClienteDestino].Text, unidadeDeTrabalho, vincularFiliaisClientesRelacionadas, fronteira: false))
							{
								linhaRetorno.CodigoErro = i;
								linhaRetorno.LinhaErro = $"Linha {i}";
								linhaRetorno.DescricaoErro = erro;
								erros.Add(linhaRetorno);

								continue;
							}
						}
						else if (colunaCEPDestino > 0)
						{
							string diasUteis = colunaCEPDestinoDiasUteis > 0 ? worksheet.Cells[i, colunaCEPDestinoDiasUteis].Text : string.Empty;

							if (!ObterCEPs(out string erro, out cepsDestino, worksheet.Cells[i, colunaCEPDestino].Text, diasUteis))
							{
								linhaRetorno.CodigoErro = i;
								linhaRetorno.LinhaErro = $"Linha {i}";
								linhaRetorno.DescricaoErro = erro;
								erros.Add(linhaRetorno);

								continue;
							}
						}
						else if (colunaRotaDestino > 0)
						{
							if (!ObterRotas(out string erro, out rotasDestino, worksheet.Cells[i, colunaRotaDestino].Text, unidadeDeTrabalho))
							{
								linhaRetorno.CodigoErro = i;
								linhaRetorno.LinhaErro = $"Linha {i}";
								linhaRetorno.DescricaoErro = erro;
								erros.Add(linhaRetorno);

								continue;
							}
						}
						else if (colunaRegiaoDestino > 0)
						{
							if (!ObterRegioes(out string erro, out regioesDestino, worksheet.Cells[i, colunaRegiaoDestino].Text, unidadeDeTrabalho))
							{
								linhaRetorno.CodigoErro = i;
								linhaRetorno.LinhaErro = $"Linha {i}";
								linhaRetorno.DescricaoErro = erro;
								erros.Add(linhaRetorno);

								continue;
							}
						}
					}

					int leadtime = 0;
					if (colunaColunaLeadTime > 0)
						int.TryParse(Utilidades.String.OnlyNumbers(worksheet.Cells[i, colunaColunaLeadTime].Text), out leadtime);

					int leadTimeDias = 0;
					if (colunaLeadTimeDias > 0)
						int.TryParse(Utilidades.String.OnlyNumbers(worksheet.Cells[i, colunaLeadTimeDias].Text), out leadTimeDias);

					int quilometragem = 0;
                    if (colunaKMSistema > 0)
                        int.TryParse(Utilidades.String.OnlyNumbers(worksheet.Cells[i, colunaKMSistema].Text), out quilometragem);

                    string codigoIntegracao = "";
					if (colunaCodigoIntegracao > 0)
						codigoIntegracao = worksheet.Cells[i, colunaCodigoIntegracao].Text;

					if (colunaTransportador > 0)
					{
						string cnpjEmpresa = worksheet.Cells[i, colunaTransportador].Text;
						if (!ObterEmpresa(out string erro, out empresaPorTabelaFreteCliente, cnpjEmpresa, unidadeDeTrabalho))
						{
							linhaRetorno.CodigoErro = i;
							linhaRetorno.LinhaErro = $"Linha {i}";
							linhaRetorno.DescricaoErro = erro;
							erros.Add(linhaRetorno);

							continue;
						}
					}

					if (colunaFronteira > 0)
					{
						if (!ObterClientes(out string erro, out fronteiras, worksheet.Cells[i, colunaFronteira].Text, unidadeDeTrabalho, vincularFiliaisClientesRelacionadas, fronteira: true))
						{
							linhaRetorno.CodigoErro = i;
							linhaRetorno.LinhaErro = $"Linha {i}";
							linhaRetorno.DescricaoErro = erro;
							erros.Add(linhaRetorno);

							continue;
						}
					}

					if (colunaCanalEntrega > 0)
					{
						string codigoIntegracaoCanalEntrega = worksheet.Cells[i, colunaCanalEntrega].Text;
						if (!ObterCanalEntrega(out string erro, out canalEntrega, codigoIntegracaoCanalEntrega, unidadeDeTrabalho))
						{
							linhaRetorno.CodigoErro = i;
							linhaRetorno.LinhaErro = $"Linha {i}";
							linhaRetorno.DescricaoErro = erro;
							erros.Add(linhaRetorno);

							continue;
						}
					}

					if (colunaCanalVenda > 0)
					{
						string codigoIntegracaoCanalVenda = worksheet.Cells[i, colunaCanalVenda].Text;
						if (!ObterCanalVenda(out string erro, out canalVenda, codigoIntegracaoCanalVenda, unidadeDeTrabalho))
						{
							linhaRetorno.CodigoErro = i;
							linhaRetorno.LinhaErro = $"Linha {i}";
							linhaRetorno.DescricaoErro = erro;
							erros.Add(linhaRetorno);

							continue;
						}
					}

					if (colunaTipoOperacao > 0)
					{
						string codigoIntegracaoTipoOperacao = worksheet.Cells[i, colunaTipoOperacao].Text;
						if (!ObterTipoOperacao(out string erro, out tiposOperacao, codigoIntegracaoTipoOperacao, unidadeDeTrabalho))
						{
							linhaRetorno.CodigoErro = i;
							linhaRetorno.LinhaErro = $"Linha {i}";
							linhaRetorno.DescricaoErro = erro;
							erros.Add(linhaRetorno);

							continue;
						}
					}

					if (colunaTipoDeCarga > 0)
					{
						string codigoIntegracaoTipoDeCarga = worksheet.Cells[i, colunaTipoDeCarga].Text;
						if (!ObterTipoDeCarga(out string erro, out tiposDeCarga, codigoIntegracaoTipoDeCarga, unidadeDeTrabalho))
						{
							linhaRetorno.CodigoErro = i;
							linhaRetorno.LinhaErro = $"Linha {i}";
							linhaRetorno.DescricaoErro = erro;
							erros.Add(linhaRetorno);

							continue;
						}
					}

					if (colunaPercentualRota > 0)
						percentualRota = (worksheet.Cells[i, colunaPercentualRota].Text).ToString().Replace(",", ".").ToNullableDecimal();

					if (colunaQuantidadeEntregas > 0)
						quantidadadeEntregas = worksheet.Cells[i, colunaQuantidadeEntregas].Text?.ToNullableInt();

					if (colunaCapacidadeOTM > 0)
						capacidadeOtm = worksheet.Cells[i, colunaCapacidadeOTM].Text?.ToNullableBool();

					if (colunaDominioOTM > 0)
					{
						Enum.TryParse(worksheet.Cells[i, colunaDominioOTM].Text, out DominioOTM dominioEnum);

						dominioOTM = dominioEnum;
					}

					if (colunaPontoPlanejamentoTransporte > 0)
					{
						Enum.TryParse(worksheet.Cells[i, colunaPontoPlanejamentoTransporte].Text, out PontoPlanejamentoTransporte planejamentoEnum);

						pontoPlanejamentoTransporte = planejamentoEnum;
					}

					if (colunaTipoIntegracao > 0)
					{
						Enum.TryParse(worksheet.Cells[i, colunaTipoIntegracao].Text, out TipoIntegracaoUnilever integracaoEnum);

						tipoIntegracao = integracaoEnum;
					}

					if (colunaContratoTransportador > 0)
					{
						string codigoContratoTransportador = worksheet.Cells[i, colunaContratoTransportador].Text;
						if (!ObterContratoTranspotadorFrete(out string erro, out contratoTransportador, codigoContratoTransportador, unidadeDeTrabalho))
						{
							linhaRetorno.CodigoErro = i;
							linhaRetorno.LinhaErro = $"Linha {i}";
							linhaRetorno.DescricaoErro = erro;
							erros.Add(linhaRetorno);

							continue;
						}
						else if (!vigencia.DataFinal.HasValue || contratoTransportador.DataInicio > vigencia.DataInicial || contratoTransportador.DataInicio > contratoTransportador.DataFim || contratoTransportador.DataFim < vigencia.DataInicial || contratoTransportador.DataFim < vigencia.DataFinal)
						{
							linhaRetorno.CodigoErro = i;
							linhaRetorno.LinhaErro = $"Linha {i}";
							linhaRetorno.DescricaoErro = $"A vigência informada não pode estar fora do período do contrato do transportador (De {contratoTransportador.DataInicio.ToString("dd/MM/yyyy")} até {contratoTransportador.DataFim.ToString("dd/MM/yyyy")}).";
							erros.Add(linhaRetorno);

							continue;
						}
					}

					if (colunaGrupoCarga > 0)
						tipoGrupoCarga = worksheet.Cells[i, colunaGrupoCarga].Text.ToEnum<TipoGrupoCarga>();

					if (colunaGerenciarCapacidade > 0)
						gerenciarCapacidade = (worksheet.Cells[i, colunaGerenciarCapacidade].Text?.ToString() == "Sim" ? true : false);

					if (colunaEstruturaTabela > 0)
						estruturaTabela = worksheet.Cells[i, colunaEstruturaTabela].Text.ToEnum<EstruturaTabela>();

					if (colunaPrioridadeUso > 0)
						prioridadeUso = worksheet.Cells[i, colunaPrioridadeUso].Text?.ToNullableInt();

					string observacaoInterna = "";
					if (colunaObservacaoInterna > 0)
						observacaoInterna = worksheet.Cells[i, colunaObservacaoInterna].Text;

					List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana> diasEntrega = new List<DiaSemana>();
					if (colunaSeg > 0 && !string.IsNullOrWhiteSpace(worksheet.Cells[i, colunaSeg].Text))
						diasEntrega.Add(DiaSemana.Segunda);
					if (colunaTer > 0 && !string.IsNullOrWhiteSpace(worksheet.Cells[i, colunaTer].Text))
						diasEntrega.Add(DiaSemana.Terca);
					if (colunaQua > 0 && !string.IsNullOrWhiteSpace(worksheet.Cells[i, colunaQua].Text))
						diasEntrega.Add(DiaSemana.Quarta);
					if (colunaQui > 0 && !string.IsNullOrWhiteSpace(worksheet.Cells[i, colunaQui].Text))
						diasEntrega.Add(DiaSemana.Quinta);
					if (colunaSex > 0 && !string.IsNullOrWhiteSpace(worksheet.Cells[i, colunaSex].Text))
						diasEntrega.Add(DiaSemana.Sexta);
					if (colunaSab > 0 && !string.IsNullOrWhiteSpace(worksheet.Cells[i, colunaSab].Text))
						diasEntrega.Add(DiaSemana.Sabado);
					if (colunaDom > 0 && !string.IsNullOrWhiteSpace(worksheet.Cells[i, colunaDom].Text))
						diasEntrega.Add(DiaSemana.Domingo);

					string descricaoParametro = "";
					if (colunaParametroBase > 0)
						descricaoParametro = worksheet.Cells[i, colunaParametroBase].Text;

					int codigoEmpresaAtual = codigoEmpresa > 0 ? codigoEmpresa : (empresaPorTabelaFreteCliente?.Codigo ?? 0);
					int codigoCanalEntrega = canalEntrega?.Codigo ?? 0;
					int codigoCanalVenda = canalVenda?.Codigo ?? 0;

					if (tipoOperacao != null)
						tiposOperacao.Add(tipoOperacao);

					if (colunaTomadorDestino > 0)
					{
						string CpfCnpjTomador = worksheet.Cells[i, colunaTomadorDestino].Text;

						if (!ObterTomador(out string erro, out tomador, CpfCnpjTomador, repCliente))
						{
							linhaRetorno.CodigoErro = i;
							linhaRetorno.LinhaErro = $"Linha {i}";
							linhaRetorno.DescricaoErro = erro;
							erros.Add(linhaRetorno);

							continue;
						}
					}

					Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = repTabelaFreteCliente.BuscarTabelaIgual(tabelaFrete.Codigo, localidadesOrigem, localidadesDestino, estadosOrigem, estadosDestino, regioesOrigem, null, clientesOrigem, clientesDestino, cepsOrigem, cepsDestino, rotasOrigem, rotasDestino, fronteiras, vigencia.Codigo, tiposOperacao, tiposDeCarga, codigoEmpresaAtual, codigoCanalEntrega, codigoCanalVenda);

					try
					{
						unidadeDeTrabalho.Start();

						bool adicionarTabelaFreteCliente = (tabelaFreteCliente == null);

						if (adicionarTabelaFreteCliente)
						{
							tabelaFreteCliente = new Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente
							{
								TabelaFrete = tabelaFrete,
								Vigencia = vigencia,
								Destinos = localidadesDestino,
								Empresa = empresaPorTabelaFreteCliente ?? empresa,
								ClientesDestino = clientesDestino,
								ClientesOrigem = clientesOrigem,
								RotasOrigem = rotasOrigem,
								RotasDestino = rotasDestino,
								RegioesOrigem = regioesOrigem,
								EstadosOrigem = estadosOrigem,
								EstadosDestino = estadosDestino,
								RegioesDestino = regioesDestino,
								Origens = localidadesOrigem,
								TipoPagamento = tipoPagamento.HasValue ? tipoPagamento.Value : tomador != null ? TipoPagamentoEmissao.Outros : TipoPagamentoEmissao.UsarDaNotaFiscal,
								HerdarInclusaoICMSTabelaFrete = true,
								IncluirICMSValorFrete = tabelaFrete.IncluirICMSValorFrete,
								PercentualICMSIncluir = tabelaFrete.PercentualICMSIncluir,
								FreteValidoParaQualquerOrigem = freteValidoParaQualquerOrigem,
								FreteValidoParaQualquerDestino = freteValidoParaQualquerDestino,
								CodigoIntegracao = codigoIntegracao,
								Ativo = true,
								Moeda = moeda ?? MoedaCotacaoBancoCentral.Real,
								PrioridadeUso = prioridadeUso,
								CanalEntrega = canalEntrega,
								PercentualRota = percentualRota,
								PontoPlanejamentoTransporte = pontoPlanejamentoTransporte,
								TipoIntegracao = tipoIntegracao,
								CanalVenda = canalVenda,
								TipoGrupoCarga = tipoGrupoCarga,
								GerenciarCapacidade = gerenciarCapacidade,
								EstruturaTabela = estruturaTabela,
								ContratoTransporteFrete = contratoTransportador,
								DominioOTM = dominioOTM,
								ObservacaoInterna = observacaoInterna,
								Tomador = tomador,
								Quilometragem = quilometragem
							};

							if (tiposOperacao.Count > 0)
								tabelaFreteCliente.TiposOperacao = tiposOperacao;

							if (tiposDeCarga.Count > 0)
								tabelaFreteCliente.TiposCarga = tiposDeCarga;

							repTabelaFreteCliente.Inserir(tabelaFreteCliente);

							if (configuracaoTabelaFrete.GravarAuditoriaImportarTabelaFrete)
								Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFreteCliente, "Adicionado via importação de planilha", unidadeDeTrabalho);

							List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPOrigem> cepsOrigemCadastrados = new List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPOrigem>();

							if (cepsOrigem.Count > 0)
							{
								foreach (Dominio.ObjetosDeValor.Embarcador.Frete.FaixaCEP faixaCEP in cepsOrigem)
								{
									Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPOrigem cepOrigem = new Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPOrigem()
									{
										CEPFinal = faixaCEP.CEPFinal,
										CEPInicial = faixaCEP.CEPInicial,
										TabelaFreteCliente = tabelaFreteCliente
									};

									repTabelaFreteClienteCEPOrigem.Inserir(cepOrigem);

									cepsOrigemCadastrados.Add(cepOrigem);
								}
							}

							List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPDestino> cepsDestinoCadastrados = new List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPDestino>();

							if (cepsDestino.Count > 0)
							{
								foreach (Dominio.ObjetosDeValor.Embarcador.Frete.FaixaCEP faixaCEP in cepsDestino)
								{
									Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPDestino cepDestino = new Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPDestino()
									{
										CEPFinal = faixaCEP.CEPFinal,
										CEPInicial = faixaCEP.CEPInicial,
										DiasUteis = faixaCEP.DiasUteis,
										TabelaFreteCliente = tabelaFreteCliente
									};

									repTabelaFreteClienteCEPDestino.Inserir(cepDestino);

									cepsDestinoCadastrados.Add(cepDestino);
								}
							}

							tabelaFreteCliente.DescricaoOrigem = ConcatenarOrigens(tabelaFreteCliente, cepsOrigemCadastrados);
							tabelaFreteCliente.DescricaoDestino = ConcatenarDestinos(tabelaFreteCliente, cepsDestinoCadastrados);

							repTabelaFreteCliente.Atualizar(tabelaFreteCliente);

							Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteClienteExiste = repTabelaFreteCliente.BuscarTabelaComMesmaIncidencia(tabelaFreteCliente, cepsOrigemCadastrados, cepsDestinoCadastrados);

							if (tabelaFreteClienteExiste != null)
								throw new ControllerException($"Já existe uma tabela com a mesma incidência (diferente desta tabela) cadastrada: {tabelaFreteClienteExiste.DescricaoOrigem} até {tabelaFreteClienteExiste.DescricaoDestino}");

							servicoTabelaFreteIntegracao.AdicionarAlteracao(tabelaFreteCliente);
							servicoTabelaFreteClienteIntegracao.AdicionarIntegracoes(tabelaFreteCliente);
						}
						else
						{
							if (new Servicos.Embarcador.Frete.MensagemAlertaTabelaFreteCliente(unidadeDeTrabalho).IsMensagemSemConfirmacao(tabelaFreteCliente, TipoMensagemAlerta.AjusteTabelaFreteCliente))
								throw new ControllerException("Não é possível alterar valores da tabela de frete com ajuste aguardando retorno");

                            if (configuracaoTabelaFrete?.PermitirInformarLeadTimeTabelaFreteCliente ?? false)
                                repTabelaFreteClienteFrequenciaEntrega.DeletarPorTabelaFreteCliente(tabelaFreteCliente.Codigo);
                            
                            servicoTabelaFreteCliente.DuplicarParaHistoricoAlteracao(tabelaFreteCliente, this.Usuario);

                            tabelaFreteCliente.Initialize();
                        }

						if (tabelaFreteCliente.ContratoTransporteFrete != null && tabelaFreteCliente.Empresa != null && tabelaFreteCliente.ContratoTransporteFrete.Transportador?.Codigo != tabelaFreteCliente.Empresa?.Codigo)
							throw new ControllerException(Localization.Resources.Fretes.TabelaFreteCliente.NaoEPossivelCadastrarUmaTabelaParaUmClienteComUmTransportadorDiferenteDoContrato);

						if (fronteiras?.Count > 0)
							tabelaFreteCliente.Fronteiras = fronteiras;

						if (diasEntrega.Count > 0 && diasEntrega.Count < 7)
						{
							foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana diaSemana in diasEntrega)
							{
								Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteFrequenciaEntrega tabelaFreteClienteFrequenciaEntrega = new Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteFrequenciaEntrega();
								tabelaFreteClienteFrequenciaEntrega.TabelaFreteCliente = tabelaFreteCliente;
								tabelaFreteClienteFrequenciaEntrega.DiaSemana = diaSemana;
								repTabelaFreteClienteFrequenciaEntrega.Inserir(tabelaFreteClienteFrequenciaEntrega);
							}
						}

						if (colunaColunaLeadTime > 0)
							tabelaFreteCliente.LeadTime = leadtime;

						if (colunaLeadTimeDias > 0)
							tabelaFreteCliente.LeadTime = leadTimeDias;

						if (colunaKMSistema > 0)
							tabelaFreteCliente.Quilometragem = quilometragem;

						SalvarDadosModeloVeicular(descricaoParametro, percentualRota, capacidadeOtm, quantidadadeEntregas, tabelaFreteCliente, tabelaFrete, unidadeDeTrabalho);

						foreach (dynamic parametro in parametros)
						{
							string item = (string)parametro.ItemParametroBase;
							int codigoItem = codigoItem = int.Parse(item.Split('_')[1]);
							int codigoParametroBase = 0;

							if (!string.IsNullOrWhiteSpace(descricaoParametro))
							{
								Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosBaseTabelaFrete parametrosBaseTabelaFrete = listaParametrosBaseTabelaFrete.Where(o => o.Descricao == descricaoParametro || o.CodigoIntegracao == descricaoParametro).FirstOrDefault();

								if (parametrosBaseTabelaFrete == null)
									throw new ControllerException($"Não foi possível encontrar o parâmetro base na tabela de frete ({descricaoParametro})");

								codigoParametroBase = parametrosBaseTabelaFrete.Codigo;
							}
							else
								codigoParametroBase = tabelaFrete.ParametroBase.HasValue ? (int)parametro.ParametroBase : 0;

							string itemTipoObjeto = item.Split('_')[0];
							TipoParametroBaseTabelaFrete? tipoObjeto = itemTipoObjeto.ToNullableEnum<TipoParametroBaseTabelaFrete>();
							TipoCampoValorTabelaFrete tipoValor = (TipoCampoValorTabelaFrete)parametro.TipoValor;
							string valor = RemoveExtraText(worksheet.Cells[i, (int)parametro.Coluna].Text);

							if (string.IsNullOrWhiteSpace(valor))
								valor = "0,00";

							decimal valorConvertido = Utilidades.Decimal.Converter(valor);

							if (valorConvertido > 0 || !naoAtualizarValoresZerados)
							{
								if (codigoParametroBase > 0)
								{
									Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametroBase = this.ObterParametroBase(tabelaFreteCliente, codigoParametroBase, unidadeDeTrabalho);

									if (tipoObjeto.HasValue)
									{
										Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemParametroBase = this.ObterItemParametroBase(tabelaFreteCliente, parametroBase.Codigo, codigoItem, tipoObjeto.Value, unidadeDeTrabalho);

										if (itemParametroBase == null)
											itemParametroBase = new Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete()
											{
												CodigoObjeto = codigoItem,
												ParametroBaseCalculo = parametroBase,
												TipoObjeto = tipoObjeto.Value
											};
										else
											itemParametroBase.Initialize();

										itemParametroBase.TipoValor = tipoValor;

										if (configuracaoTabelaFrete.GravarAuditoriaImportarTabelaFrete)
											servicoTabelaFreteCliente.DefinirValorItem(itemParametroBase, valorConvertido, Auditado, configuracaoTabelaFrete.GravarAuditoriaImportarTabelaFrete);
										else
											servicoTabelaFreteCliente.DefinirValorItem(itemParametroBase, valorConvertido);

										if (itemParametroBase.Codigo > 0)
											repItemParametroBase.Atualizar(itemParametroBase);
										else
											repItemParametroBase.Inserir(itemParametroBase);
									}
									else if (itemTipoObjeto == "ValorBase")
									{
										parametroBase.ValorBaseOriginal = (parametroBase.ValorBase != valorConvertido) ? parametroBase.ValorBase : parametroBase.ValorBaseOriginal;
										parametroBase.ValorBase = valorConvertido;
										repParametro.Atualizar(parametroBase);
										if (configuracaoTabelaFrete.GravarAuditoriaImportarTabelaFrete)
											Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFreteCliente, $"Alterou o valor do Valor Base para {parametroBase.ValorBase} via Importação.", unidadeDeTrabalho);
									}
									else if (itemTipoObjeto == "ValorMinimoGarantido")
									{
										parametroBase.ValorMinimoGarantidoOriginal = (parametroBase.ValorMinimoGarantido != valorConvertido) ? parametroBase.ValorMinimoGarantido : parametroBase.ValorMinimoGarantidoOriginal;
										parametroBase.ValorMinimoGarantido = valorConvertido;
										repParametro.Atualizar(parametroBase);
										if (configuracaoTabelaFrete.GravarAuditoriaImportarTabelaFrete)
											Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFreteCliente, $"Alterou o valor do Valor Mínimo Garantido para {parametroBase.ValorMinimoGarantido} via Importação.", unidadeDeTrabalho);
									}
									else if (itemTipoObjeto == "EntregaExcedente")
									{
										parametroBase.ValorEntregaExcedenteOriginal = (parametroBase.ValorEntregaExcedente != valorConvertido) ? parametroBase.ValorEntregaExcedente : parametroBase.ValorEntregaExcedenteOriginal;
										parametroBase.ValorEntregaExcedente = valorConvertido;
										repParametro.Atualizar(parametroBase);
										if (configuracaoTabelaFrete.GravarAuditoriaImportarTabelaFrete)
											Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFreteCliente, $"Alterou o valor do Entrega Excedente para {parametroBase.ValorEntregaExcedente} via Importação.", unidadeDeTrabalho);
									}
									else if (itemTipoObjeto == "PesoExcedente")
									{
										parametroBase.ValorPesoExcedenteOriginal = (parametroBase.ValorPesoExcedente != valorConvertido) ? parametroBase.ValorPesoExcedente : parametroBase.ValorPesoExcedenteOriginal;
										parametroBase.ValorPesoExcedente = valorConvertido;
										repParametro.Atualizar(parametroBase);
										if (configuracaoTabelaFrete.GravarAuditoriaImportarTabelaFrete)
											Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFreteCliente, $"Alterou o valor do Peso Excedente para {parametroBase.ValorPesoExcedente} via Importação.", unidadeDeTrabalho);
									}
									else if (itemTipoObjeto == "PacoteExcedente")
									{
										parametroBase.ValorPacoteExcedenteOriginal = (parametroBase.ValorPacoteExcedente != valorConvertido) ? parametroBase.ValorPacoteExcedente : parametroBase.ValorPacoteExcedenteOriginal;
										parametroBase.ValorPacoteExcedente = valorConvertido;
										repParametro.Atualizar(parametroBase);
										if (configuracaoTabelaFrete.GravarAuditoriaImportarTabelaFrete)
											Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFreteCliente, $"Alterou o valor do Pacote Excedente para {parametroBase.ValorPacoteExcedente} via Importação.", unidadeDeTrabalho);
									}
								}
								else
								{
									if (tipoObjeto.HasValue)
									{
										Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemParametroBase = this.ObterItemParametroBase(tabelaFreteCliente, 0, codigoItem, tipoObjeto.Value, unidadeDeTrabalho);

										if (itemParametroBase == null)
											itemParametroBase = new Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete()
											{
												CodigoObjeto = codigoItem,
												TabelaFrete = tabelaFreteCliente,
												TipoObjeto = tipoObjeto.Value
											};
										else
											itemParametroBase.Initialize();

										itemParametroBase.TipoValor = tipoValor;

										servicoTabelaFreteCliente.DefinirValorItem(itemParametroBase, valorConvertido, Auditado, configuracaoTabelaFrete.GravarAuditoriaImportarTabelaFrete);

										if (itemParametroBase.Codigo > 0)
											repItemParametroBase.Atualizar(itemParametroBase);
										else
											repItemParametroBase.Inserir(itemParametroBase);
									}
									else if (itemTipoObjeto == "ValorBase")
									{
										tabelaFreteCliente.ValorBaseOriginal = (tabelaFreteCliente.ValorBase != valorConvertido) ? tabelaFreteCliente.ValorBase : tabelaFreteCliente.ValorBaseOriginal;
										tabelaFreteCliente.ValorBase = valorConvertido;
										repTabelaFreteCliente.Atualizar(tabelaFreteCliente);
										if (configuracaoTabelaFrete.GravarAuditoriaImportarTabelaFrete)
											Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFreteCliente, $"Alterou o valor do Valor Base para {tabelaFreteCliente.ValorBase} via Importação.", unidadeDeTrabalho);
									}
									else if (itemTipoObjeto == "ValorMinimoGarantido")
									{
										tabelaFreteCliente.ValorMinimoGarantidoOriginal = (tabelaFreteCliente.ValorMinimoGarantido != valorConvertido) ? tabelaFreteCliente.ValorMinimoGarantido : tabelaFreteCliente.ValorMinimoGarantidoOriginal;
										tabelaFreteCliente.ValorMinimoGarantido = valorConvertido;
										repTabelaFreteCliente.Atualizar(tabelaFreteCliente);
										if (configuracaoTabelaFrete.GravarAuditoriaImportarTabelaFrete)
											Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFreteCliente, $"Alterou o valor do Valor Mínimo Garantido Base para {tabelaFreteCliente.ValorMinimoGarantido} via Importação.", unidadeDeTrabalho);
									}
									else if (itemTipoObjeto == "EntregaExcedente")
									{
										tabelaFreteCliente.ValorEntregaExcedenteOriginal = (tabelaFreteCliente.ValorEntregaExcedente != valorConvertido) ? tabelaFreteCliente.ValorEntregaExcedente : tabelaFreteCliente.ValorEntregaExcedenteOriginal;
										tabelaFreteCliente.ValorEntregaExcedente = valorConvertido;
										repTabelaFreteCliente.Atualizar(tabelaFreteCliente);
										if (configuracaoTabelaFrete.GravarAuditoriaImportarTabelaFrete)
											Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFreteCliente, $"Alterou o valor do Entrega Excedente Base para {tabelaFreteCliente.ValorEntregaExcedente} via Importação.", unidadeDeTrabalho);
									}
									else if (itemTipoObjeto == "PesoExcedente")
									{
										tabelaFreteCliente.ValorPesoExcedenteOriginal = (tabelaFreteCliente.ValorPesoExcedente != valorConvertido) ? tabelaFreteCliente.ValorPesoExcedente : tabelaFreteCliente.ValorPesoExcedenteOriginal;
										tabelaFreteCliente.ValorPesoExcedente = valorConvertido;
										repTabelaFreteCliente.Atualizar(tabelaFreteCliente);
										if (configuracaoTabelaFrete.GravarAuditoriaImportarTabelaFrete)
											Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFreteCliente, $"Alterou o valor do Peso Excedente Base para {tabelaFreteCliente.ValorPesoExcedente} via Importação.", unidadeDeTrabalho);
									}
									else if (itemTipoObjeto == "PacoteExcedente")
									{
										tabelaFreteCliente.ValorPacoteExcedenteOriginal = (tabelaFreteCliente.ValorPacoteExcedente != valorConvertido) ? tabelaFreteCliente.ValorPacoteExcedente : tabelaFreteCliente.ValorPacoteExcedenteOriginal;
										tabelaFreteCliente.ValorPacoteExcedente = valorConvertido;
										repTabelaFreteCliente.Atualizar(tabelaFreteCliente);
										if (configuracaoTabelaFrete.GravarAuditoriaImportarTabelaFrete)
											Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFreteCliente, $"Alterou o valor do Pacote Excedente Base para {tabelaFreteCliente.ValorPacoteExcedente} via Importação.", unidadeDeTrabalho);
									}
								}
							}
						}

						if (
							adicionarTabelaFreteCliente ||
							!validarPermissaoSolicitarAprovacao ||
							(tabelaFreteCliente.TabelaFrete.ParametroBase.HasValue && repItemParametroBase.ExistePendenteAprovacaoPorParametrosTabelaFrete(tabelaFreteCliente.Codigo)) ||
							(!tabelaFreteCliente.TabelaFrete.ParametroBase.HasValue && repItemParametroBase.ExistePendenteAprovacaoPorTabelaFrete(tabelaFreteCliente.Codigo))
						)
						{
							if (tabelaFreteCliente.Tipo != TipoTabelaFreteCliente.Alteracao)
								tabelaFreteCliente.PermitirCalcularFreteEmAlteracao = (!adicionarTabelaFreteCliente && validarPermissaoSolicitarAprovacao);

							servicoAprovacaoTabelaFrete.AtualizarAprovacao(tabelaFreteCliente, Usuario, TipoServicoMultisoftware);
						}

						if (!adicionarTabelaFreteCliente && configuracaoTabelaFrete.GravarAuditoriaImportarTabelaFrete)
							Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFreteCliente, tabelaFreteCliente.GetChanges(), "Atualizado via importação de planilha", unidadeDeTrabalho);

						unidadeDeTrabalho.CommitChanges();
					}
					catch (BaseException excecao)
					{
						unidadeDeTrabalho.Rollback();
						linhaRetorno.CodigoErro = i;
						linhaRetorno.LinhaErro = $"Linha {i}";
						linhaRetorno.DescricaoErro = excecao.Message;
						erros.Add(linhaRetorno);
					}
				}

				package.Dispose();

				return new JsonpResult(erros);
			}
			catch (Exception excecao)
			{
				unidadeDeTrabalho.Rollback();
				Servicos.Log.TratarErro(excecao);
				return new JsonpResult(false, "Ocorreu uma falha ao importar o arquivo.");
			}
			finally
			{
				unidadeDeTrabalho.Dispose();
			}
		}

		public async Task<IActionResult> ImportarPorServico()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

			try
			{
				List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

				if (files.Count <= 0)
					return new JsonpResult(false, "Selecione um arquivo para envio.");

				Servicos.DTO.CustomFile arquivo = files[0];
				string extensaoArquivo = Path.GetExtension(arquivo.FileName).ToLowerInvariant();

				if ((extensaoArquivo != ".xls") && (extensaoArquivo != ".xlsx"))
					return new JsonpResult(false, "Extensão do arquivo inválida. Selecione um arquivo com a extensão .xls ou .xlsx.");

				ExcelPackage arquivoExcel = new ExcelPackage(arquivo.InputStream);

				if ((arquivoExcel == null) || (arquivoExcel.Workbook.Worksheets.Count == 0))
					return new JsonpResult(false, "Não foi possível carregar a planilha.");

				ExcelWorksheet planilha = arquivoExcel.Workbook.Worksheets.Where(x => x.Hidden == eWorkSheetHidden.Visible).First();

				Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.ImportacaoTabelaFreteParametro parametrosImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.ImportacaoTabelaFreteParametro()
				{
					CodigoDestino = GetParametroImportacao("Destino").ToInt(),
					CodigoEmpresa = GetParametroImportacao("Empresa").ToInt(),
					CodigoOrigem = GetParametroImportacao("Origem").ToInt(),
					CodigoTabelaFrete = GetParametroImportacao("TabelaFrete").ToInt(),
					CodigoTipoOperacao = GetParametroImportacao("TipoOperacao").ToInt(),
					CodigoVigencia = GetParametroImportacao("Vigencia").ToInt(),
					Moeda = GetParametroImportacao("Moeda").ToNullableEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral>(),
					TipoPagamento = GetParametroImportacao("TipoPagamento").ToNullableEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoEmissao>(),
					FreteValidoParaQualquerDestino = GetParametroImportacao("FreteValidoParaQualquerDestino").ToBool(),
					FreteValidoParaQualquerOrigem = GetParametroImportacao("FreteValidoParaQualquerOrigem").ToBool(),
					IndiceColunaCepDestino = GetParametroImportacao("ColunaCEPDestino").ToInt(),
					IndiceColunaCepDestinoDiasUteis = GetParametroImportacao("ColunaCEPDestinoDiasUteis").ToInt(),
					IndiceColunaTomadorDestino = GetParametroImportacao("ColunaTomadorDestino").ToInt(),
					IndiceColunaLeadTime = GetParametroImportacao("ColunaLeadTime").ToInt(),
					IndiceColunaCepOrigem = GetParametroImportacao("ColunaCEPOrigem").ToInt(),
					IndiceColunaClienteDestino = GetParametroImportacao("ColunaClienteDestino").ToInt(),
					IndiceColunaClienteOrigem = GetParametroImportacao("ColunaClienteOrigem").ToInt(),
					IndiceColunaCodigoIntegracao = GetParametroImportacao("ColunaCodigoIntegracao").ToInt(),
					IndiceColunaDestino = GetParametroImportacao("ColunaDestino").ToInt(),
					IndiceColunaEstadoDestino = GetParametroImportacao("ColunaEstadoDestino").ToInt(),
					IndiceColunaEstadoOrigem = GetParametroImportacao("ColunaEstadoOrigem").ToInt(),
					IndiceColunaOrigem = GetParametroImportacao("ColunaOrigem").ToInt(),
					IndiceColunaParametroBase = GetParametroImportacao("ColunaParametrosBase").ToInt(),
					IndiceColunaRegiaoDestino = GetParametroImportacao("ColunaRegiaoDestino").ToInt(),
					IndiceColunaRegiaoOrigem = GetParametroImportacao("ColunaRegiaoOrigem").ToInt(),
					IndiceColunaRotaDestino = GetParametroImportacao("ColunaRotaDestino").ToInt(),
					IndiceColunaRotaOrigem = GetParametroImportacao("ColunaRotaOrigem").ToInt(),
					IndiceColunaTransportador = GetParametroImportacao("ColunaTransportador").ToInt(),
					IndiceColunaFronteira = GetParametroImportacao("ColunaFronteira").ToInt(),
					IndiceColunaKMSistema = GetParametroImportacao("ColunaKMSistema").ToInt(),
					IndiceColunaPrioridadeUso = GetParametroImportacao("ColunaPrioridadeUso").ToInt(),
					IndiceLinhaIniciarImportacao = GetParametroImportacao("LinhaInicioDados").ToInt(),
					NaoAtualizarValoresZerados = GetParametroImportacao("NaoAtualizarValoresZerados").ToBool(),
					Parametros = JsonConvert.DeserializeObject<object>(GetParametroImportacao("Parametros")),
					IndiceColunaSeg = GetParametroImportacao("ColunaSeg").ToInt(),
					IndiceColunaTer = GetParametroImportacao("ColunaTer").ToInt(),
					IndiceColunaQua = GetParametroImportacao("ColunaQua").ToInt(),
					IndiceColunaQui = GetParametroImportacao("ColunaQui").ToInt(),
					IndiceColunaSex = GetParametroImportacao("ColunaSex").ToInt(),
					IndiceColunaSab = GetParametroImportacao("ColunaSab").ToInt(),
					IndiceColunaDom = GetParametroImportacao("ColunaDom").ToInt(),
					IndiceColunaTipoOperacao = GetParametroImportacao("ColunaTipoOperacao").ToInt(),
					IndiceColunaTipoDeCarga = GetParametroImportacao("ColunaTipoCarga").ToInt(),
					IndiceColunaLeadTimeDias = GetParametroImportacao("ColunaLeadTimeDias").ToInt(),
					IndiceColunaContratoTransportador = GetParametroImportacao("ColunaContratoTransportador").ToInt(),
					IndiceColunaGrupoCarga = GetParametroImportacao("ColunaGrupoCarga").ToInt(),
					IndiceColunaGerenciarCapacidade = GetParametroImportacao("ColunaGerenciarCapacidade").ToInt(),
					IndiceColunaEstruturaTabela = GetParametroImportacao("ColunaEstruturaTabela").ToInt(),
					IndiceColunaPercentualRota = GetParametroImportacao("ColunaPercentualRota").ToInt(),
					IndiceColunaQuantidadeEntregas = GetParametroImportacao("ColunaQuantidadeEntregas").ToInt(),
					IndiceColunaCapacidadeOTM = GetParametroImportacao("ColunaCapacidadeOTM").ToInt()
				};

				Servicos.Embarcador.Importacao.ImportacaoTabelaFrete servicoImportacaoTabelaFrete = new Servicos.Embarcador.Importacao.ImportacaoTabelaFrete(unitOfWork);
				List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.RetornoImportacaoTabelaFrete> listaErros = servicoImportacaoTabelaFrete.Importar(parametrosImportacao, planilha, arquivo.FileName, Usuario, TipoServicoMultisoftware);

				arquivoExcel.Dispose();

				return new JsonpResult(listaErros);
			}
			catch (Exception excecao)
			{
				unitOfWork.Rollback();
				Servicos.Log.TratarErro(excecao);
				return new JsonpResult(false, "Ocorreu uma falha ao importar o arquivo.");
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

		/// <summary>
		/// Método utilizado para importar a tabela do GPA Ocorrências
		/// </summary>
		public async Task<IActionResult> ImportarTabelaCliente()
		{
			Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

			try
			{
				List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

				if (files.Count <= 0)
					return new JsonpResult(false, "Selecione um arquivo para envio.");

				Servicos.DTO.CustomFile file = files[0];

				string extensao = Path.GetExtension(file.FileName).ToLowerInvariant();

				if (extensao != ".xls" && extensao != ".xlsx")
					return new JsonpResult(false, "Extensão do arquivo inválida. Selecione um arquivo com a extensão .xls ou .xlsx.");

				int codigoTabelaFrete;
				int.TryParse(Request.Params("TabelaFrete"), out codigoTabelaFrete);

				Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
				Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unidadeDeTrabalho);
				Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unidadeDeTrabalho);
				Repositorio.Embarcador.Frete.TabelaFreteCliente repTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unidadeDeTrabalho);
				Repositorio.Embarcador.Frete.ParametroBaseCalculoTabelaFrete repParametroBaseCalculoTabelaFrete = new Repositorio.Embarcador.Frete.ParametroBaseCalculoTabelaFrete(unidadeDeTrabalho);
				Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete repItemParametroBase = new Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete(unidadeDeTrabalho);
				Repositorio.Embarcador.Ocorrencias.ParametroOcorrencia repParametroOcorrencia = new Repositorio.Embarcador.Ocorrencias.ParametroOcorrencia(unidadeDeTrabalho);
				Repositorio.Embarcador.Frete.VigenciaTabelaFrete repVigencia = new Repositorio.Embarcador.Frete.VigenciaTabelaFrete(unidadeDeTrabalho);
				Servicos.Embarcador.Frete.TabelaFreteAprovacaoAlcada servicoAprovacaoTabelaFrete = new Servicos.Embarcador.Frete.TabelaFreteAprovacaoAlcada(unidadeDeTrabalho, notificarUsuario: false);
				Servicos.Embarcador.Frete.TabelaFreteIntegracao servicoTabelaFreteIntegracao = new Servicos.Embarcador.Frete.TabelaFreteIntegracao(unidadeDeTrabalho);
				Servicos.Embarcador.Frete.TabelaFreteClienteIntegracao servicoTabelaFreteClienteIntegracao = new Servicos.Embarcador.Frete.TabelaFreteClienteIntegracao(unidadeDeTrabalho);
				Servicos.Embarcador.Frete.TabelaFreteCliente servicoTabelaFreteCliente = new Servicos.Embarcador.Frete.TabelaFreteCliente(unidadeDeTrabalho);
				Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = repTabelaFrete.BuscarPorCodigo(codigoTabelaFrete);

				if (tabelaFrete == null)
					return new JsonpResult(false, "Tabela de frete não encontrada.");

				var linhaInicioDados = 2;
				var colunaRemetente = 1;
				var colunaDestinatario = 2;
				var colunaModeloVeicular = 3;
				var colunaValorReentrega = 4;
				var colunaValorPeriodo = 5;
				var colunaValorReboque = 6;
				var colunaDataVigencia = 7;

				ExcelPackage package = new ExcelPackage(file.InputStream);

				ExcelWorksheet worksheet = package.Workbook.Worksheets.First();

				StringBuilder erros = new StringBuilder();

				System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("pt-BR");

				//unidadeDeTrabalho.Start();

				Servicos.Log.TratarErro("Planilha: " + worksheet.Dimension.End.Row.ToString() + " registros.", "ImportacaoPlanilhaFrete");

				int contador = 0;

				for (var i = linhaInicioDados; i <= worksheet.Dimension.End.Row; i++)
				{
					contador = contador + 1;
					//Servicos.Log.TratarErro("Linha: " + i.ToString() + " de " + worksheet.Dimension.End.Row.ToString());
					Servicos.Log.TratarErro("Linha: " + i.ToString() + " de " + worksheet.Dimension.End.Row.ToString() + ".", "ImportacaoPlanilhaFrete");

					if (i >= linhaInicioDados)
					{
						string cnpjRemetente = Utilidades.String.OnlyNumbers(worksheet.Cells[i, colunaRemetente].Text);
						string cnpjDestinatario = Utilidades.String.OnlyNumbers(worksheet.Cells[i, colunaDestinatario].Text);
						string codigoModeloVeicular = worksheet.Cells[i, colunaModeloVeicular].Text;
						Servicos.Log.TratarErro("Remetente: " + cnpjRemetente + ".", "ImportacaoPlanilhaFrete");
						Servicos.Log.TratarErro("Destinatário: " + cnpjDestinatario + ".", "ImportacaoPlanilhaFrete");
						Servicos.Log.TratarErro("ModeloVeicular: " + codigoModeloVeicular + ".", "ImportacaoPlanilhaFrete");

						decimal valorReentrega, valorPeriodo, valorReboque = 0;

						string valor = RemoveExtraText(worksheet.Cells[i, colunaValorReentrega].Text);
						if (string.IsNullOrWhiteSpace(valor))
							valor = "0,00";
						valorReentrega = decimal.Parse(valor, cultura);
						Servicos.Log.TratarErro("Valor Reentrega: " + valor + ".", "ImportacaoPlanilhaFrete");

						valor = RemoveExtraText(worksheet.Cells[i, colunaValorPeriodo].Text);
						if (string.IsNullOrWhiteSpace(valor))
							valor = "0,00";
						valorPeriodo = decimal.Parse(valor, cultura);
						Servicos.Log.TratarErro("Valor Periodo: " + valor + ".", "ImportacaoPlanilhaFrete");

						valor = RemoveExtraText(worksheet.Cells[i, colunaValorReboque].Text);
						if (string.IsNullOrWhiteSpace(valor))
							valor = "4,98";
						valorReboque = decimal.Parse(valor, cultura);
						Servicos.Log.TratarErro("Valor Reboque: " + valor + ".", "ImportacaoPlanilhaFrete");

						DateTime dataVigencia = DateTime.Today;
						valor = RemoveExtraText(worksheet.Cells[i, colunaDataVigencia].Text);
						DateTime.TryParseExact(valor, "ddMMyyyy", null, System.Globalization.DateTimeStyles.None, out dataVigencia);
						if (dataVigencia <= DateTime.MinValue)
							dataVigencia = DateTime.Today;

						Servicos.Log.TratarErro("Vigência: " + valor + ".", "ImportacaoPlanilhaFrete");
						Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete vigencia = null;
						List<Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete> vigencias = repVigencia.Buscar(dataVigencia, tabelaFrete.Codigo, 0);
						if (vigencias.Count == 1)
							vigencia = vigencias.FirstOrDefault();

						if (vigencia == null)
						{
							vigencia = new Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete();
							vigencia.DataInicial = dataVigencia;
							vigencia.DataFinal = dataVigencia;
							vigencia.TabelaFrete = tabelaFrete;
							repVigencia.Inserir(vigencia);
						}

						if (!string.IsNullOrWhiteSpace(cnpjRemetente) && !string.IsNullOrWhiteSpace(cnpjDestinatario) && !string.IsNullOrWhiteSpace(codigoModeloVeicular))
						{
							Dominio.Entidades.Cliente remetente = repCliente.BuscarPorCPFCNPJ(double.Parse(cnpjRemetente));
							Dominio.Entidades.Cliente destinatario = repCliente.BuscarPorCPFCNPJ(double.Parse(cnpjDestinatario));

							if (remetente != null && destinatario != null)
							{
								Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicular = repModeloVeicularCarga.buscarPorCodigoIntegracao(codigoModeloVeicular);
								if (modeloVeicular == null && !string.IsNullOrWhiteSpace(codigoModeloVeicular))
								{
									modeloVeicular = new Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga();

									//var descricaoModeloVeicular = "Importado da planilha de frete";

									modeloVeicular.CodigoIntegracao = codigoModeloVeicular;
									modeloVeicular.Descricao = codigoModeloVeicular;
									modeloVeicular.Ativo = true;
									modeloVeicular.CapacidadePesoTransporte = 1;
									modeloVeicular.ToleranciaPesoExtra = 1;
									modeloVeicular.VeiculoPaletizado = true;
									modeloVeicular.NumeroEixos = 0;
									modeloVeicular.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga.Geral;
									modeloVeicular.Cubagem = 1;
									repModeloVeicularCarga.Inserir(modeloVeicular);
								}

								Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = repTabelaFreteCliente.BuscarPorTabelaFreteCliente(tabelaFrete.Codigo, null, null, remetente, destinatario, 0, string.Empty, vigencia.Codigo, 0, 0, 0, true);

								if (tabelaFreteCliente == null)
								{
									tabelaFreteCliente = new Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente();

									tabelaFreteCliente.TabelaFrete = tabelaFrete;
									tabelaFreteCliente.ClienteOrigem = remetente;
									tabelaFreteCliente.ClienteDestino = destinatario;
									tabelaFreteCliente.TipoPagamento = TipoPagamentoEmissao.UsarDaNotaFiscal;
									tabelaFreteCliente.HerdarInclusaoICMSTabelaFrete = true;
									tabelaFreteCliente.IncluirICMSValorFrete = tabelaFrete.IncluirICMSValorFrete;
									tabelaFreteCliente.PercentualICMSIncluir = tabelaFrete.PercentualICMSIncluir;
									tabelaFreteCliente.Vigencia = vigencia;

									repTabelaFreteCliente.Inserir(tabelaFreteCliente);
								}
								else
									servicoTabelaFreteCliente.DuplicarParaHistoricoAlteracao(tabelaFreteCliente, this.Usuario);

								servicoAprovacaoTabelaFrete.AtualizarAprovacao(tabelaFreteCliente, Usuario, TipoServicoMultisoftware);
								servicoTabelaFreteIntegracao.AdicionarAlteracao(tabelaFreteCliente);
								servicoTabelaFreteClienteIntegracao.AdicionarIntegracoes(tabelaFreteCliente);

								Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametro = repParametroBaseCalculoTabelaFrete.Buscar(tabelaFreteCliente.Codigo, modeloVeicular.Codigo);
								if (parametro == null)
								{
									parametro = new Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete();
									parametro.TabelaFrete = tabelaFreteCliente;
									parametro.CodigoObjeto = modeloVeicular.Codigo;
									parametro.ImprimirObservacaoCTe = false;
									repParametroBaseCalculoTabelaFrete.Inserir(parametro);
								}

								Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia parametroReentrega = repParametroOcorrencia.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroOcorrencia.Inteiro);
								if (parametroReentrega != null)
								{
									Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemParametro = repItemParametroBase.BuscarPorCodigoObjetoETipoItem(tabelaFreteCliente.Codigo, parametro.Codigo, parametroReentrega.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ParametrosOcorrencia);

									if (itemParametro == null)
										itemParametro = new Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete()
										{
											CodigoObjeto = parametroReentrega.Codigo,
											ParametroBaseCalculo = parametro,
											TabelaFrete = tabelaFreteCliente,
											TipoObjeto = TipoParametroBaseTabelaFrete.ParametrosOcorrencia
										};

									itemParametro.TipoValor = TipoCampoValorTabelaFrete.ValorFixo;
									itemParametro.ValorOriginal = (itemParametro.Valor != valorReentrega) ? itemParametro.Valor : itemParametro.ValorOriginal;
									itemParametro.Valor = valorReentrega;

									if (itemParametro.Codigo > 0)
										repItemParametroBase.Atualizar(itemParametro);
									else
										repItemParametroBase.Inserir(itemParametro);
								}

								Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia parametroPeriodo = repParametroOcorrencia.BuscarPorTipo(TipoParametroOcorrencia.Periodo);
								if (parametroPeriodo != null)
								{
									Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemParametro = repItemParametroBase.BuscarPorCodigoObjetoETipoItem(tabelaFreteCliente.Codigo, parametro.Codigo, parametroPeriodo.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ParametrosOcorrencia);

									if (itemParametro == null)
										itemParametro = new Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete()
										{
											CodigoObjeto = parametroPeriodo.Codigo,
											TabelaFrete = tabelaFreteCliente,
											ParametroBaseCalculo = parametro,
											TipoObjeto = TipoParametroBaseTabelaFrete.ParametrosOcorrencia
										};

									itemParametro.TipoValor = TipoCampoValorTabelaFrete.ValorFixo;
									itemParametro.ValorOriginal = (itemParametro.Valor != valorPeriodo) ? itemParametro.Valor : itemParametro.ValorOriginal;
									itemParametro.Valor = valorPeriodo;

									if (itemParametro.Codigo > 0)
										repItemParametroBase.Atualizar(itemParametro);
									else
										repItemParametroBase.Inserir(itemParametro);
								}

								Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia parametroReboque = repParametroOcorrencia.BuscarPorTipo(TipoParametroOcorrencia.Booleano);
								if (parametroReboque != null)
								{
									Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemParametro = repItemParametroBase.BuscarPorCodigoObjetoETipoItem(tabelaFreteCliente.Codigo, parametro.Codigo, parametroReboque.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ParametrosOcorrencia);

									if (itemParametro == null)
										itemParametro = new Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete()
										{
											CodigoObjeto = parametroReboque.Codigo,
											TabelaFrete = tabelaFreteCliente,
											ParametroBaseCalculo = parametro,
											TipoObjeto = TipoParametroBaseTabelaFrete.ParametrosOcorrencia
										};

									itemParametro.TipoValor = TipoCampoValorTabelaFrete.ValorFixo;
									itemParametro.ValorOriginal = (itemParametro.Valor != valorReboque) ? itemParametro.Valor : itemParametro.ValorOriginal;
									itemParametro.Valor = valorReboque;

									if (itemParametro.Codigo > 0)
										repItemParametroBase.Atualizar(itemParametro);
									else
										repItemParametroBase.Inserir(itemParametro);
								}
							}
							else
							{
								if (remetente == null)
									Servicos.Log.TratarErro("CNPJ Origem não cadastrado: " + cnpjRemetente, "ImportacaoPlanilhaFrete");
								if (destinatario == null)
									Servicos.Log.TratarErro("CNPJ Destino não cadastrado: " + cnpjDestinatario, "ImportacaoPlanilhaFrete");
							}
						}
						else
						{
							if (string.IsNullOrWhiteSpace(cnpjRemetente))
								Servicos.Log.TratarErro("CNPJ Origem não informado: " + worksheet.Cells[i, colunaRemetente].Text, "ImportacaoPlanilhaFrete");
							if (string.IsNullOrWhiteSpace(cnpjDestinatario))
								Servicos.Log.TratarErro("CNPJ Destino não informado: " + worksheet.Cells[i, colunaDestinatario].Text, "ImportacaoPlanilhaFrete");
							if (string.IsNullOrWhiteSpace(codigoModeloVeicular))
								Servicos.Log.TratarErro("Tipo veículo não informado: " + worksheet.Cells[i, colunaModeloVeicular].Text, "ImportacaoPlanilhaFrete");
						}
					}

					//if (contador == 100)
					//{
					//    contador = 0;

					//    repCliente = null;
					//    repModeloVeicularCarga = null;
					//    repTabelaFrete = null;
					//    repTabelaFreteCliente = null;
					//    repParametroBaseCalculoTabelaFrete = null;
					//    repItemParametroBase = null;
					//    repParametroOcorrencia = null;
					//    unidadeDeTrabalho.Dispose();
					//    unidadeDeTrabalho = null;

					//    unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

					//    repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
					//    repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unidadeDeTrabalho);
					//    repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unidadeDeTrabalho);
					//    repTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unidadeDeTrabalho);
					//    repParametroBaseCalculoTabelaFrete = new Repositorio.Embarcador.Frete.ParametroBaseCalculoTabelaFrete(unidadeDeTrabalho);
					//    repItemParametroBase = new Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete(unidadeDeTrabalho);
					//    repParametroOcorrencia = new Repositorio.Embarcador.Ocorrencias.ParametroOcorrencia(unidadeDeTrabalho);
					//}
					unidadeDeTrabalho.FlushAndClear();

				}

				//unidadeDeTrabalho.CommitChanges();

				package.Dispose();

				if (erros.Length > 0)
					return new JsonpResult(false, true, erros.ToString());

				return new JsonpResult(true);
			}
			catch (Exception ex)
			{
				//unidadeDeTrabalho.Rollback();

				Servicos.Log.TratarErro(ex);

				return new JsonpResult(false, "Ocorreu uma falha ao importar o arquivo.");
			}
			finally
			{
				unidadeDeTrabalho.Dispose();
			}
		}

		#endregion

		#region Métodos Privados

		private Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete ObterParametroBase(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFrete, int codigoParametro, Repositorio.UnitOfWork unidadeDeTrabalho)
		{
			Repositorio.Embarcador.Frete.ParametroBaseCalculoTabelaFrete repParametro = new Repositorio.Embarcador.Frete.ParametroBaseCalculoTabelaFrete(unidadeDeTrabalho);

			Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametro = repParametro.Buscar(tabelaFrete.Codigo, codigoParametro);

			if (parametro != null)
				return parametro;

			parametro = new Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete
			{
				CodigoObjeto = codigoParametro,
				TabelaFrete = tabelaFrete,
			};

			repParametro.Inserir(parametro);

			return parametro;
		}

		private Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete ObterItemParametroBase(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFrete, int codigoParametro, int codigoItem, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete tipoItem, Repositorio.UnitOfWork unidadeDeTrabalho)
		{
			Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete repItemParametro = new Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete(unidadeDeTrabalho);

			return repItemParametro.BuscarPorCodigoObjetoETipoItem(tabelaFrete.Codigo, codigoParametro, codigoItem, tipoItem);
		}

		private string RemoveExtraText(string value)
		{
			var allowedChars = "01234567890.,";
			return new string(value.Where(c => allowedChars.Contains(c)).ToArray());
		}

		private bool ObterLocalidades(out string erro, out List<Dominio.Entidades.Localidade> localidades, string localidade, string estado, Repositorio.UnitOfWork unitOfWork)
		{
			Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

			localidades = new List<Dominio.Entidades.Localidade>();

			string[] descricaoLocalidades = localidade.Trim().Split('/');

			for (var i = 0; i < descricaoLocalidades.Length; i++)
			{
				string descLocalidade = descricaoLocalidades[i].Trim();
				string descEstado = estado.Trim();

				if (string.IsNullOrWhiteSpace(descLocalidade))
					continue;

				if (string.IsNullOrWhiteSpace(estado))
				{
					string[] localidadeSplitted = descLocalidade.Split('-');

					if (localidadeSplitted.Length > 1)
						descEstado = localidadeSplitted[1].Trim();
					else
					{
						erro = "Localidade inválida: " + descLocalidade + ".";
						return false;
					}

					descLocalidade = Utilidades.String.RemoveDiacritics(localidadeSplitted[0]).Trim();
				}

				Dominio.Entidades.Localidade eLocalidade = repLocalidade.BuscarPorDescricaoEUF(descLocalidade, descEstado);

				if (eLocalidade == null)
				{
					erro = "Localidade não encontrada: " + descLocalidade + "-" + descEstado + ".";
					return false;
				}

				localidades.Add(eLocalidade);
			}

			if (localidades.Count <= 0)
			{
				erro = "Nenhuma localidade encontrada com a descrição fornecida: " + localidade + ".";
				return false;
			}

			erro = string.Empty;
			return true;
		}

		private bool ObterEstados(out string erro, out List<Dominio.Entidades.Estado> estados, string estado, Repositorio.UnitOfWork unitOfWork)
		{
			Repositorio.Estado repositorioestado = new Repositorio.Estado(unitOfWork);
			estados = new List<Dominio.Entidades.Estado>();

			string siglaEstado = estado.Trim();

			if (!string.IsNullOrWhiteSpace(siglaEstado))
			{
				Dominio.Entidades.Estado EstadoImportar = repositorioestado.BuscarPorSigla(siglaEstado);
				if (EstadoImportar != null)
					estados.Add(EstadoImportar);
				else
				{
					erro = "Nenhum Estado encontrado com a descrição fornecida: " + estado + ".";
					return false;
				}

			}
			erro = string.Empty;
			return true;
		}

		private bool ObterClientes(out string erro, out List<Dominio.Entidades.Cliente> clientes, string cpfCnpjs, Repositorio.UnitOfWork unitOfWork, bool vincularFiliaisClientesRelacionadas, bool fronteira)
		{
			Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

			clientes = new List<Dominio.Entidades.Cliente>();

			string[] cpfCnpjClientes = cpfCnpjs.Split('/');

			for (var i = 0; i < cpfCnpjClientes.Length; i++)
			{
				Dominio.Entidades.Cliente cliente = null;

				double.TryParse(Utilidades.String.OnlyNumbers(cpfCnpjClientes[i]), out double cpfCnpjCliente);

				if (cpfCnpjCliente <= 0d)
				{
					if (cpfCnpjClientes[i] != "")
						cliente = repCliente.BuscarPorCodigoIntegracao(cpfCnpjClientes[i]);

					if (cliente == null)
					{
						erro = "Cliente não encontrado: " + cpfCnpjClientes[i];
						return false;
					}
				}
				else
					cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpjCliente);

				if (cliente == null)
				{
					if (cpfCnpjClientes[i] != "")
						cliente = repCliente.BuscarPorCodigoIntegracao(cpfCnpjClientes[i]);

					if (cliente == null)
					{
						erro = "Pessoa não encontrada: " + cpfCnpjClientes[i] + ".";
						return false;
					}
				}

				if (fronteira && !cliente.FronteiraAlfandega)
				{
					erro = "Pessoa não é uma fronteira: " + cpfCnpjClientes[i] + ".";
					return false;
				}

				clientes.Add(cliente);
			}

			if (clientes.Count <= 0)
			{
				erro = "Nenhuma pessoa encontrada com o CPF/CNPJ fornecido: " + cpfCnpjs + ".";
				return false;
			}

			if (!fronteira && vincularFiliaisClientesRelacionadas)
				clientes = new Servicos.Cliente().ObterFiliaisClientesRelacionadas(clientes, unitOfWork);

			erro = string.Empty;
			return true;
		}

		private bool ObterCEPs(out string erro, out List<Dominio.ObjetosDeValor.Embarcador.Frete.FaixaCEP> ceps, string faixaCEPs, string diasUteis = null)
		{
			ceps = new List<Dominio.ObjetosDeValor.Embarcador.Frete.FaixaCEP>();

			try
			{
				string[] faixasCEPs = faixaCEPs.Split('/');
				diasUteis = Utilidades.String.OnlyNumbers(diasUteis);

				for (var i = 0; i < faixasCEPs.Length; i++)
				{
					string[] faixaCEP = faixasCEPs[i].Split('-');

					int.TryParse(Utilidades.String.OnlyNumbers(faixaCEP[0]), out int cepInicial);
					int.TryParse(Utilidades.String.OnlyNumbers(faixaCEP[1]), out int cepFinal);

					if (cepInicial > cepFinal)
					{
						erro = "O CEP inicial é maior que o CEP final (" + faixasCEPs[i] + "). Formato de faixa de CEP deve ser 34800000-34800974/34740000-34740970.";
						return false;
					}

					for (var j = 0; j < ceps.Count; j++)
					{
						Dominio.ObjetosDeValor.Embarcador.Frete.FaixaCEP cepExistente = ceps[j];

						if ((cepInicial >= cepExistente.CEPInicial && cepInicial <= cepExistente.CEPFinal) ||
							(cepFinal >= cepExistente.CEPInicial && cepFinal <= cepExistente.CEPFinal) ||
							(cepExistente.CEPInicial >= cepInicial && cepExistente.CEPInicial <= cepFinal) ||
							(cepExistente.CEPFinal >= cepInicial && cepExistente.CEPFinal <= cepFinal))
						{
							erro = "Existem faixas de CEP conflitantes (" + string.Format("{0:00000-000}", cepInicial) + " à " + string.Format("{0:00000-000}", cepFinal) + " e " + string.Format("{0:00000-000}", cepExistente.CEPInicial) + " à " + string.Format("{0:00000-000}", cepExistente.CEPFinal) + ").";
							return false;
						}
					}

					ceps.Add(new Dominio.ObjetosDeValor.Embarcador.Frete.FaixaCEP()
					{
						CEPInicial = cepInicial,
						CEPFinal = cepFinal,
						DiasUteis = diasUteis.ToInt()
					});
				}

				if (ceps.Count <= 0)
				{
					erro = "Nenhuma faixa de CEP informada: " + faixaCEPs + ".";
					return false;
				}

				erro = string.Empty;
				return true;
			}
			catch (Exception ex)
			{
				Servicos.Log.TratarErro(ex);
				erro = "Faixa de CEP inválida (" + faixaCEPs + "), formato deve ser 34800000-34800974/34740000-34740970";
				return false;
			}
		}

		private bool ObterEmpresa(out string erro, out Dominio.Entidades.Empresa empresa, string cnpjEmpresa, Repositorio.UnitOfWork unitOfWork)
		{
			Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
			empresa = null;
			erro = string.Empty;

			if (string.IsNullOrWhiteSpace(cnpjEmpresa))
				return true;

			string cnpjEmpresaFormatado = cnpjEmpresa.Trim().ObterSomenteNumeros().ToLong().ToString("d14");
			empresa = repositorioEmpresa.BuscarPorCNPJ(cnpjEmpresaFormatado);

			if (empresa == null)
			{
				erro = $"Transportador não encontrado: {cnpjEmpresa.Trim()}.";
				return false;
			}

			return true;
		}

		private bool ObterRotas(out string erro, out List<Dominio.Entidades.RotaFrete> rotas, string codigosIntegracao, Repositorio.UnitOfWork unitOfWork)
		{
			Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(unitOfWork);
			rotas = new List<Dominio.Entidades.RotaFrete>();
			string[] listaCodigoIntegracaoRotaFrete = codigosIntegracao.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

			if (listaCodigoIntegracaoRotaFrete.Length == 0)
			{
				erro = "Nenhum código de integração de rota fornecido";
				return false;
			}

			for (var i = 0; i < listaCodigoIntegracaoRotaFrete.Length; i++)
			{
				Dominio.Entidades.RotaFrete rotaFrete = repositorioRotaFrete.BuscarPorCodigoIntegracao(listaCodigoIntegracaoRotaFrete[i].Trim());

				if (rotaFrete == null)
				{
					erro = $"Rota não encontrada: {listaCodigoIntegracaoRotaFrete[i].Trim()}.";
					return false;
				}

				rotas.Add(rotaFrete);
			}

			if (rotas.Count <= 0)
			{
				if (listaCodigoIntegracaoRotaFrete.Length == 1)
					erro = $"Nenhuma rota encontrada com o código de integração fornecido: {codigosIntegracao}";
				else
					erro = $"Nenhuma rota encontrada com os códigos de integração fornecidos: {codigosIntegracao}";

				return false;
			}

			erro = string.Empty;

			return true;
		}

		private bool ObterRegioes(out string erro, out List<Dominio.Entidades.Embarcador.Localidades.Regiao> regioes, string codigosIntegracao, Repositorio.UnitOfWork unitOfWork)
		{
			Repositorio.Embarcador.Localidades.Regiao repositorioRegiao = new Repositorio.Embarcador.Localidades.Regiao(unitOfWork);
			regioes = new List<Dominio.Entidades.Embarcador.Localidades.Regiao>();
			string[] listaCodigoIntegracaoRegiao = codigosIntegracao.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

			if (listaCodigoIntegracaoRegiao.Length == 0)
			{
				erro = "Nenhum código de integração de região fornecido";
				return false;
			}

			for (var i = 0; i < listaCodigoIntegracaoRegiao.Length; i++)
			{
				Dominio.Entidades.Embarcador.Localidades.Regiao regiao = repositorioRegiao.BuscarPorCodigoIntegracao(listaCodigoIntegracaoRegiao[i].Trim());

				if (regiao == null)
				{
					erro = $"Região não encontrada: {listaCodigoIntegracaoRegiao[i].Trim()}.";
					return false;
				}

				regioes.Add(regiao);
			}

			if (regioes.Count <= 0)
			{
				if (listaCodigoIntegracaoRegiao.Length == 1)
					erro = $"Nenhuma região encontrada com o código de integração fornecido: {codigosIntegracao}";
				else
					erro = $"Nenhuma região encontrada com os códigos de integração fornecidos: {codigosIntegracao}";

				return false;
			}

			erro = string.Empty;

			return true;
		}

		private bool ObterCanalEntrega(out string erro, out Dominio.Entidades.Embarcador.Pedidos.CanalEntrega canalEntrega, string codigoIntegracaoCanalEntrega, Repositorio.UnitOfWork unitOfWork)
		{
			Repositorio.Embarcador.Pedidos.CanalEntrega repositorioCanalEntrega = new Repositorio.Embarcador.Pedidos.CanalEntrega(unitOfWork);
			canalEntrega = null;
			erro = string.Empty;

			if (string.IsNullOrWhiteSpace(codigoIntegracaoCanalEntrega))
				return true;

			canalEntrega = repositorioCanalEntrega.BuscarPorCodigoIntegracao(codigoIntegracaoCanalEntrega);

			if (canalEntrega == null)
			{
				erro = $"Canal de Entrega não encontrado: {codigoIntegracaoCanalEntrega.Trim()}.";
				return false;
			}

			return true;
		}

		private bool ObterCanalVenda(out string erro, out Dominio.Entidades.Embarcador.Pedidos.CanalVenda canalVenda, string codigoIntegracaoCanalVenda, Repositorio.UnitOfWork unitOfWork)
		{
			Repositorio.Embarcador.Pedidos.CanalVenda repositorioCanalVenda = new Repositorio.Embarcador.Pedidos.CanalVenda(unitOfWork);
			canalVenda = null;
			erro = string.Empty;

			if (string.IsNullOrWhiteSpace(codigoIntegracaoCanalVenda))
				return true;

			canalVenda = repositorioCanalVenda.BuscarPorCodigoIntegracao(codigoIntegracaoCanalVenda);

			if (canalVenda == null)
			{
				erro = $"Canal de Venda não encontrado: {codigoIntegracaoCanalVenda.Trim()}.";
				return false;
			}

			return true;
		}

		private bool ObterTipoOperacao(out string erro, out List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacoes, string codigosIntegracaoTipoOperacao, Repositorio.UnitOfWork unitOfWork)
		{

			Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
			tiposOperacoes = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();
			erro = string.Empty;

			if (string.IsNullOrWhiteSpace(codigosIntegracaoTipoOperacao))
				return true;

			string[] listaCodigoIntegracaoTipoOperacao = codigosIntegracaoTipoOperacao.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

			for (var i = 0; i < listaCodigoIntegracaoTipoOperacao.Length; i++)
			{
				Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarPorCodigoIntegracao(listaCodigoIntegracaoTipoOperacao[i].Trim());

				if (tipoOperacao == null)
				{
					erro = $"Tipo de Operação não encontrado: {listaCodigoIntegracaoTipoOperacao[i].Trim()}.";
					return false;
				}

				tiposOperacoes.Add(tipoOperacao);
			}

			if (tiposOperacoes.Count <= 0)
			{
				if (listaCodigoIntegracaoTipoOperacao.Length == 1)
					erro = $"Nenhum tipo de operação encontrado com o código de integração fornecido: {codigosIntegracaoTipoOperacao}";
				else
					erro = $"Nenhum tipo de operação encontrado com os códigos de integração fornecidos: {codigosIntegracaoTipoOperacao}";

				return false;
			}

			return true;
		}

		private bool ObterTipoDeCarga(out string erro, out List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tiposDeCarga, string codigosIntegracaoTipoDeCarga, Repositorio.UnitOfWork unitOfWork)
		{

			Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoOperacao = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
			tiposDeCarga = new List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();
			erro = string.Empty;

			if (string.IsNullOrWhiteSpace(codigosIntegracaoTipoDeCarga))
				return true;

			string[] listaCodigoIntegracaoTipoDeCarga = codigosIntegracaoTipoDeCarga.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

			for (var i = 0; i < listaCodigoIntegracaoTipoDeCarga.Length; i++)
			{
				Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCarga = repositorioTipoOperacao.BuscarPorCodigoEmbarcador(listaCodigoIntegracaoTipoDeCarga[i].Trim());

				if (tipoDeCarga == null)
				{
					erro = $"Tipo de carga não encontrado: {listaCodigoIntegracaoTipoDeCarga[i].Trim()}.";
					return false;
				}

				tiposDeCarga.Add(tipoDeCarga);
			}

			if (tiposDeCarga.Count <= 0)
			{
				if (listaCodigoIntegracaoTipoDeCarga.Length == 1)
					erro = $"Nenhum tipo de carga encontrado com o código de integração fornecido: {codigosIntegracaoTipoDeCarga}";
				else
					erro = $"Nenhum tipo de carga encontrado com os códigos de integração fornecidos: {codigosIntegracaoTipoDeCarga}";

				return false;
			}

			return true;
		}

		private bool ObterContratoTranspotadorFrete(out string erro, out Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete contratoTransportador, string strCodigoContratoTransportador, Repositorio.UnitOfWork unitOfWork)
		{
			Repositorio.Embarcador.Frete.ContratoTransporteFrete repContratoTransporteFrete = new Repositorio.Embarcador.Frete.ContratoTransporteFrete(unitOfWork);

			contratoTransportador = null;
			erro = string.Empty;

			if (string.IsNullOrWhiteSpace(strCodigoContratoTransportador.ObterSomenteNumeros()))
				return true;

			int codigoContratoTransportador = strCodigoContratoTransportador.ObterSomenteNumeros().ToInt();
			contratoTransportador = repContratoTransporteFrete.BuscarPorNumeroContrato(codigoContratoTransportador);

			if (contratoTransportador == null)
			{
				erro = $"Contrato Transportador não encontrado: {strCodigoContratoTransportador.Trim()}.";
				return false;
			}

			return true;
		}

		private string GetParametroImportacao(string parametro)
		{
			return Request.Params(parametro);
		}

		private List<Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosBaseTabelaFrete> ModelosReboqueParaParametrosBase(List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosReboqueTabela)
		{
			return modelosReboqueTabela.Select(o => new Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosBaseTabelaFrete()
			{
				Codigo = o.Codigo,
				Descricao = o.Descricao,
				CodigoIntegracao = o.CodigoIntegracao
			}).ToList();
		}

		private void SalvarDadosModeloVeicular(string descricaoParametro, decimal? percentualRota, bool? capacidadeOTM, int? quantidadadeEntregas, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Repositorio.UnitOfWork unitOfWork)
		{
			if (string.IsNullOrWhiteSpace(descricaoParametro))
				return;

			if ((percentualRota == null) && (quantidadadeEntregas == null) && (capacidadeOTM == null))
				return;

			if (tabelaFrete.ParametroBase != TipoParametroBaseTabelaFrete.ModeloReboque)
				throw new ControllerException($"Parâmetro base da tabela deve ser modelos de reboque para importar os parâmetros de saída.");

			Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
			Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repositorioModeloVeicularCarga.buscarPorDescricao(descricaoParametro);

			if (modeloVeicularCarga == null)
				throw new ControllerException($"Modelo Veicular (Parâmetro base) não encontrado: {descricaoParametro.Trim()}.");

			if (!tabelaFrete.ModelosReboque.Any(modelo => modelo.Codigo == modeloVeicularCarga.Codigo))
				throw new ControllerException($"O modelo {modeloVeicularCarga.Descricao} não está cadastrado na tabela.");

			Repositorio.Embarcador.Frete.TabelaFreteClienteModeloVeicularCarga repositorioTabelaFreteClienteModeloVeicularCarga = new Repositorio.Embarcador.Frete.TabelaFreteClienteModeloVeicularCarga(unitOfWork);
			Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteModeloVeicularCarga modeloVeicularCargaSalvar = repositorioTabelaFreteClienteModeloVeicularCarga.BuscarPorTabelaFreteClienteEModeloVeicularCarga(tabelaFreteCliente.Codigo, modeloVeicularCarga.Codigo);
			List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> valoresAlterados = new List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade>();

			if (modeloVeicularCargaSalvar == null)
				modeloVeicularCargaSalvar = new Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteModeloVeicularCarga()
				{
					TabelaFreteCliente = tabelaFreteCliente,
					ModeloVeicularCarga = modeloVeicularCarga,
					PendenteIntegracao = true
				};
			else
				modeloVeicularCargaSalvar.Initialize();

			modeloVeicularCargaSalvar.PercentualRota = percentualRota ?? 0;
			modeloVeicularCargaSalvar.QuantidadeEntregas = quantidadadeEntregas ?? 0;
			modeloVeicularCargaSalvar.CapacidadeOTM = capacidadeOTM ?? false;

			if (modeloVeicularCargaSalvar.Codigo > 0)
			{
				List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> alteracoes = modeloVeicularCargaSalvar.GetCurrentChanges();

				foreach (Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade alteracao in alteracoes)
					valoresAlterados.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
					{
						De = alteracao.De,
						Para = alteracao.Para,
						Propriedade = $"ModelosVeicularesCarga.{alteracao.Propriedade} - {modeloVeicularCargaSalvar.ModeloVeicularCarga.Descricao}".Left(200)
					});

				modeloVeicularCargaSalvar.PendenteIntegracao = modeloVeicularCargaSalvar.IsChanged();

				repositorioTabelaFreteClienteModeloVeicularCarga.Atualizar(modeloVeicularCargaSalvar);
			}
			else
			{
				valoresAlterados.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
				{
					De = "",
					Para = modeloVeicularCargaSalvar.ModeloVeicularCarga.Descricao,
					Propriedade = "ModelosVeicularesCarga"
				});

				repositorioTabelaFreteClienteModeloVeicularCarga.Inserir(modeloVeicularCargaSalvar);
			}

			tabelaFreteCliente.SetExternalChanges(valoresAlterados);
		}

		private bool ObterTomador(out string erro, out Dominio.Entidades.Cliente tomador, string cpfCnpj, Repositorio.Cliente repCliente)
		{
			double CpfCnpjTomadorConvertido = Utilidades.String.OnlyNumbers(cpfCnpj).ToDouble();

			if (CpfCnpjTomadorConvertido > 0d)
				tomador = repCliente.BuscarPorCPFCNPJ(CpfCnpjTomadorConvertido);
			else
				tomador = repCliente.BuscarPorCodigoIntegracao(cpfCnpj);

			erro = "O Cliente Tomador não está cadastrado no Sistema";
			return tomador != null ? true : false;
		}

		private string ConcatenarOrigens(TabelaFreteCliente tabelaFreteCliente, List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPOrigem> ceps)
		{
			if (tabelaFreteCliente.Origens != null && tabelaFreteCliente.Origens.Count > 0)
				return string.Join(" / ", tabelaFreteCliente.Origens.Select(o => o.DescricaoCidadeEstado));
			else if (tabelaFreteCliente.ClientesOrigem != null && tabelaFreteCliente.ClientesOrigem.Count > 0)
			{
				List<Dominio.Entidades.Cliente> clientesPai = tabelaFreteCliente.ClientesOrigem.Where(cliente => cliente.PossuiFilialCliente).ToList();
				return (clientesPai.Count > 0) ? string.Join(" / ", clientesPai.Select(cliente => cliente.Descricao)) : string.Join(" / ", tabelaFreteCliente.ClientesOrigem.Select(cliente => cliente.Descricao));
			}
			else if (tabelaFreteCliente.RotasOrigem?.Count > 0)
				return string.Join(" / ", tabelaFreteCliente.RotasOrigem.Select(o => o.Descricao));
			else if (tabelaFreteCliente.EstadosOrigem?.Count > 0)
				return string.Join(" / ", tabelaFreteCliente.EstadosOrigem.Select(o => o.Nome));

			if (ceps.Count > 0)
				return string.Join(" / ", ceps.Select(o => o.Descricao));

			return string.Empty;
		}

		private string ConcatenarDestinos(TabelaFreteCliente tabelaFreteCliente, List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPDestino> ceps)
		{
			if (tabelaFreteCliente.Destinos != null && tabelaFreteCliente.Destinos.Count > 0)
				return string.Join(" / ", tabelaFreteCliente.Destinos.Select(o => o.DescricaoCidadeEstado));
			else if (tabelaFreteCliente.ClientesDestino != null && tabelaFreteCliente.ClientesDestino.Count > 0)
			{
				List<Dominio.Entidades.Cliente> clientesPai = tabelaFreteCliente.ClientesDestino.Where(cliente => cliente.PossuiFilialCliente).ToList();
				return (clientesPai.Count > 0) ? string.Join(" / ", clientesPai.Select(cliente => cliente.Descricao)) : string.Join(" / ", tabelaFreteCliente.ClientesDestino.Select(cliente => cliente.Descricao));
			}
			else if (tabelaFreteCliente.RotasDestino?.Count > 0)
				return string.Join(" / ", tabelaFreteCliente.RotasDestino.Select(o => o.Descricao));
			else if (tabelaFreteCliente.EstadosDestino?.Count > 0)
				return string.Join(" / ", tabelaFreteCliente.EstadosDestino.Select(o => o.Nome));
			else if (tabelaFreteCliente.RegioesDestino?.Count > 0)
				return string.Join(" / ", tabelaFreteCliente.RegioesDestino.Select(o => o.Descricao));

			if (ceps.Count > 0)
				return string.Join(" / ", ceps.Select(o => o.Descricao));

			return string.Empty;
		}

		#endregion
	}
}
