using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.GestaoPallet;
using Dominio.ObjetosDeValor.Embarcador.GestaoPallet.AgendamentoColeta;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Models.Grid;
using SGTAdmin.Controllers;
using System.Linq.Dynamic.Core;

namespace SGT.WebAdmin.Controllers.GestaoPallet
{
	[CustomAuthorize("GestaoPallet/AgendamentoColetaPallet")]
	public class AgendamentoColetaPalletController : BaseController
	{

		#region Contrutores

		public AgendamentoColetaPalletController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Globais

		[AllowAuthenticate]
		public async Task<IActionResult> Pesquisa()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

			try
			{
				return new JsonpResult(ObterGridPesquisa(unitOfWork));
			}
			catch (Exception ex)
			{
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao consultar agendamento coleta pallet.");
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
				Grid grid = ObterGridPesquisa(unitOfWork);
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
		public async Task<IActionResult> BuscarPorCodigo()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

			try
			{
				int codigo = Request.GetIntParam("Codigo");

				Repositorio.Embarcador.GestaoPallet.AgendamentoColetaPallet repositorioAgendamentoColetaPallet = new Repositorio.Embarcador.GestaoPallet.AgendamentoColetaPallet(unitOfWork);

				Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoColetaPallet agendamentoColeta = repositorioAgendamentoColetaPallet.BuscarPorCodigo(codigo, false);


				DetalhesAgendamentoColetaPallet detalhesAgendamentoColetaPallet = new DetalhesAgendamentoColetaPallet
				{
					Codigo = agendamentoColeta.Codigo,
					StatusAgendamento = agendamentoColeta.Situacao.ObterDescricao(),
					Status = agendamentoColeta.Situacao,
					QuantidadePallets = agendamentoColeta.QuantidadePallets,
					CodigoCargaEmbarcador = agendamentoColeta.Carga.CodigoCargaEmbarcador,
					Filial = agendamentoColeta.Carga.Filial.Descricao,
					Solicitante = agendamentoColeta.Usuario.Nome,
					Transportador = agendamentoColeta.Transportador.NomeFantasia,
					Cliente = agendamentoColeta.Cliente.NomeFantasia,
					Veiculo = agendamentoColeta.Veiculo.Placa,
					DataOrdem = agendamentoColeta.DataOrdem.ToShortDateString(),
					NumeroOrdem = agendamentoColeta.NumeroOrdem,
					Motorista = agendamentoColeta.Motorista.Nome,
					DataCarregamento = agendamentoColeta.Carga.DataCarregamentoCarga.HasValue ? agendamentoColeta.Carga.DataCarregamentoCarga.Value.ToShortDateString() : string.Empty

				};

				return new JsonpResult(new
				{
					AgendamentoColetaPallet = new
					{
						agendamentoColeta.Codigo,
						agendamentoColeta.QuantidadePallets,
						agendamentoColeta.Situacao,
						Carga = agendamentoColeta.Carga == null ? null : new
						{
							agendamentoColeta.Carga.Codigo,
							agendamentoColeta.Carga.Descricao
						},
						Veiculo = agendamentoColeta.Veiculo == null ? null : new
						{
							agendamentoColeta.Veiculo.Codigo,
							agendamentoColeta.Veiculo.Descricao
						},
						Motorista = agendamentoColeta.Motorista == null ? null : new
						{
							agendamentoColeta.Motorista.Codigo,
							agendamentoColeta.Motorista.Descricao
						},
						Transportador = new
						{
							Codigo = agendamentoColeta.Transportador?.Codigo ?? 0,
							Descricao = agendamentoColeta.Transportador?.Descricao ?? string.Empty
						}
					},
					RetornoAcompanhamentoAgendamentoPallet = detalhesAgendamentoColetaPallet
				});
			}
			catch (Exception ex)
			{
				Servicos.Log.TratarErro(ex);

				return new JsonpResult(false, "Ocorreu uma falha ao obter controle de saldo.");
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

		[AllowAuthenticate]
		public async Task<IActionResult> Imprimir()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

			try
			{
				int codigo = Request.GetIntParam("Codigo");

				Repositorio.Embarcador.GestaoPallet.AgendamentoColetaPallet repositorioAgendamentoColetaPallet = new Repositorio.Embarcador.GestaoPallet.AgendamentoColetaPallet(unitOfWork);

				Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoColetaPallet agendamentoColeta = repositorioAgendamentoColetaPallet.BuscarPorCodigo(codigo, false);

				var arquivo = new Servicos.Embarcador.GestaoPallet.AgendamentoColetaPallet(unitOfWork).ResumoAgendamentoColetaPallet(agendamentoColeta);

				return Arquivo(arquivo, "application/pdf", "Resumo Agendamento - " + agendamentoColeta.Carga.CodigoCargaEmbarcador + ".pdf");
			}
			catch (Exception ex)
			{
				Servicos.Log.TratarErro(ex);

				return new JsonpResult(false, "Ocorreu uma falha ao obter controle de saldo.");
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
				unitOfWork.Start();

				Repositorio.Embarcador.GestaoPallet.AgendamentoColetaPallet repositorioAgendamentoColetaPallet = new Repositorio.Embarcador.GestaoPallet.AgendamentoColetaPallet(unitOfWork);

				Servicos.Embarcador.GestaoPallet.MovimentacaoPallet servicoMovimentacaoPallet = new Servicos.Embarcador.GestaoPallet.MovimentacaoPallet(unitOfWork, Auditado);
				Servicos.Embarcador.GestaoPallet.ControleEstoquePallet servicoControleEstoquePallet = new Servicos.Embarcador.GestaoPallet.ControleEstoquePallet(unitOfWork);

				Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoColetaPallet agendamentoColeta = PreencheEntidade(unitOfWork);

				repositorioAgendamentoColetaPallet.Inserir(agendamentoColeta, Auditado);

				DadosControlePallet dadosControlePallet = new DadosControlePallet(agendamentoColeta.Cliente, agendamentoColeta.Transportador)
				{
					ResponsavelPallet = agendamentoColeta.ResponsavelAgendamentoPallet,
					TipoEstoquePallet = TipoEstoquePallet.Movimentacao
				};

				bool possuiSaldoSuficiente = servicoControleEstoquePallet.PossuiSaldoSuficiente(dadosControlePallet, agendamentoColeta.QuantidadePallets);

				if (!possuiSaldoSuficiente)
					throw new ControllerException("Não foi possivel realizar o agendamento pois o saldo de pallet esta negativo, por gentileza realize o abastecimento do mesmo");

				servicoMovimentacaoPallet.AdicionarMovimentacaoReservaPallet(agendamentoColeta.Carga, agendamentoColeta.QuantidadePallets, agendamentoColeta.ResponsavelAgendamentoPallet, agendamentoColeta.Cliente);

				bool possuiSaldoBaixo = servicoControleEstoquePallet.PossuiSaldoBaixo(dadosControlePallet, 100);

				unitOfWork.CommitChanges();

				if (possuiSaldoBaixo)
					return new JsonpResult(true, true, "Seu saldo de pallet esta baixo, por gentileza realize o abastecimento do mesmo.");

				return new JsonpResult(true);
			}
			catch (BaseException ex)
			{
				unitOfWork.Rollback();
				return new JsonpResult(false, ex.Message);
			}
			catch (Exception ex)
			{
				Servicos.Log.TratarErro(ex);
				unitOfWork.Rollback();
				return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
			}
		}

		[AllowAuthenticate]
		public async Task<IActionResult> Cancelar()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
			try
			{
				unitOfWork.Start();

				int codigo = Request.GetIntParam("Codigo");

				if (codigo == 0)
					throw new ControllerException("Não foi possivel localizar o agendamento");

				Repositorio.Embarcador.GestaoPallet.AgendamentoColetaPallet repositorioAgendamentoColetaPallet = new Repositorio.Embarcador.GestaoPallet.AgendamentoColetaPallet(unitOfWork);

				Servicos.Embarcador.GestaoPallet.MovimentacaoPallet servicoMovimentacaoPallet = new Servicos.Embarcador.GestaoPallet.MovimentacaoPallet(unitOfWork, Auditado);

				Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoColetaPallet agendamentoColeta = repositorioAgendamentoColetaPallet.BuscarPorCodigo(codigo, false);

				agendamentoColeta.Situacao = SituacaoAgendamentoColetaPallet.Cancelado;

				repositorioAgendamentoColetaPallet.Atualizar(agendamentoColeta);

				servicoMovimentacaoPallet.CancelarMovimentacaoReservaPallet(agendamentoColeta.Carga);

				unitOfWork.CommitChanges();

				Servicos.Auditoria.Auditoria.Auditar(Auditado, agendamentoColeta, $"Agendamento Cancelado por {Usuario.Descricao}.", unitOfWork);

				return new JsonpResult(true);
			}
			catch (BaseException ex)
			{
				Servicos.Log.TratarErro(ex);
				unitOfWork.Rollback();
				return new JsonpResult(false, ex.Message);
			}
			catch (Exception ex)
			{
				Servicos.Log.TratarErro(ex);
				unitOfWork.Rollback();
				return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
			}
		}

		#endregion Métodos Globais

		#region Métodos Privados

		private Grid ObterGridPesquisa(Repositorio.UnitOfWork unidadeTrabalho)
		{
			Repositorio.Embarcador.GestaoPallet.AgendamentoColetaPallet repositorioAgendamentoColetaPallet = new Repositorio.Embarcador.GestaoPallet.AgendamentoColetaPallet(unidadeTrabalho);

			Grid grid = ObterCabecalhosGridPesquisa();
			Dominio.ObjetosDeValor.Embarcador.GestaoPallet.AgendamentoColeta.FiltroPesquisaAgendamentoColetaPallet filtrosPesquisa = ObterFiltrosPesquisa();
			int totalLinhas = repositorioAgendamentoColetaPallet.ContarConsultaAgendamentoColetaPallet(filtrosPesquisa);

			GridPreferencias gridPreferencias = new GridPreferencias(unidadeTrabalho, "AgendamentoColetaPallet/Pesquisa", "grid-pesquisa-agendamento-coleta");
			Dominio.Entidades.Embarcador.Preferencias.PreferenciaGrid preferenciasGrid = gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo);
			grid.AplicarPreferenciasGrid(preferenciasGrid);

			if (totalLinhas == 0)
			{
				grid.AdicionaRows(new List<dynamic>() { });
				return grid;
			}

			string ordenacao = ObterPropriedadeOrdenarOuAgrupar(grid.header[grid.indiceColunaOrdena].data);
			IList<Dominio.ObjetosDeValor.Embarcador.GestaoPallet.AgendamentoColeta.AgendamentoColetaPalletPesquisa> dados = repositorioAgendamentoColetaPallet.ConsultarAgendamentoColetaPallet(filtrosPesquisa, ordenacao, grid.dirOrdena, grid.inicio, grid.limite);

			grid.AdicionaRows(dados);
			grid.setarQuantidadeTotal(totalLinhas);

			return grid;
		}

		private Dominio.ObjetosDeValor.Embarcador.GestaoPallet.AgendamentoColeta.FiltroPesquisaAgendamentoColetaPallet ObterFiltrosPesquisa()
		{
			Dominio.ObjetosDeValor.Embarcador.GestaoPallet.AgendamentoColeta.FiltroPesquisaAgendamentoColetaPallet filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.GestaoPallet.AgendamentoColeta.FiltroPesquisaAgendamentoColetaPallet()
			{
				CodigosFilial = Request.GetListParam<int>("Filial"),
				CodigoCarga = Request.GetListParam<int>("Carga"),
				NumeroOrdem = Request.GetIntParam("NumeroOrdem"),
				DataOrdem = Request.GetDateTimeParam("DataOrdem"),
				StatusAgendamento = Request.GetNullableEnumParam<SituacaoAgendamentoColetaPallet>("StatusAgendamento")
			};

			if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
			{
				filtrosPesquisa.ResponsavelAgendamento = ResponsavelPallet.Transportador;
				filtrosPesquisa.CodigoTransportador = Usuario.Empresa.Codigo;
			}
			else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
			{
				filtrosPesquisa.ResponsavelAgendamento = ResponsavelPallet.Cliente;
				filtrosPesquisa.CodigoCliente = Usuario.Cliente.Codigo;
			}

			return filtrosPesquisa;
		}

		private Grid ObterCabecalhosGridPesquisa()
		{
			Grid grid = new Grid(Request)
			{
				header = new List<Head>()
			};

			grid.AdicionarCabecalho("Codigo", false);
			grid.AdicionarCabecalho("Numero da Ordem", "NumeroOrdem", 10, Align.center, true);
			grid.AdicionarCabecalho("Número Carga", "NumeroCarga", 10, Align.center, true);
			grid.AdicionarCabecalho("Quantidade de Pallet", "QuantidadePallets", 10, Align.center, false);
			grid.AdicionarCabecalho("Data", "DataOrdem", 10, Align.center, false);
			grid.AdicionarCabecalho("Transportador", "Transportador", 10, Align.left, false);
			grid.AdicionarCabecalho("Situação", "SituacaoFormatada", 10, Align.center, false);
			grid.AdicionarCabecalho("Cliente", "Cliente", 10, Align.left, false);
			grid.AdicionarCabecalho("Motorista", "Motorista", 10, Align.left, false);
			grid.AdicionarCabecalho("Placa", "Veiculo", 10, Align.left, false);
			grid.AdicionarCabecalho("Solicitante", "Solicitante", 10, Align.left, false);

			return grid;
		}

		private string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
		{
			if (propriedadeOrdenarOuAgrupar.EndsWith("Formatada"))
				return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

			return propriedadeOrdenarOuAgrupar;
		}

		private Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoColetaPallet PreencheEntidade(Repositorio.UnitOfWork unitOfWork)
		{
			Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
			Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
			Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(unitOfWork);
			Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);

			Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoColetaPallet agendamento = new Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoColetaPallet()
			{
				Usuario = Usuario,
				DataOrdem = DateTime.Now,
				Situacao = SituacaoAgendamentoColetaPallet.EmAndamento,
			};

			int codigoCarga = Request.GetIntParam("Carga");
			int quantidadePallets = Request.GetIntParam("QuantidadePallets");
			int codigoVeiculo = Request.GetIntParam("Veiculo");
			int codigoMotorista = Request.GetIntParam("Motorista");
			long codigoCliente = Request.GetLongParam("Cliente");
			Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelPallet responsavelPallet = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelPallet>("ResponsavelPallet");

			Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

			if (carga == null)
				throw new ControllerException("Carga não Localizada");

			List<SituacaoCarga> listaSituacaoCargaEmAberto = SituacaoCargaHelper.ObterSituacoesCargaPermiteAtualizar();
			if (!listaSituacaoCargaEmAberto.Contains(carga.SituacaoCarga))
			{
				throw new ControllerException("Atual situação da carga não pode receber agendamento");
			}

			agendamento.Carga = carga;
			agendamento.Cliente = carga.Pedidos.Select(x => x.Pedido.Destinatario).FirstOrDefault();

			if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
			{
				agendamento.ResponsavelAgendamentoPallet = ResponsavelPallet.Transportador;
				agendamento.Transportador = Usuario.Empresa;

			}
			else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
			{
				agendamento.ResponsavelAgendamentoPallet = ResponsavelPallet.Cliente;
				agendamento.Cliente = Usuario.Cliente;
			}
			else
			{

				if (responsavelPallet == ResponsavelPallet.Cliente)
				{
					agendamento.Cliente = repositorioCliente.BuscarPorCPFCNPJ(codigoCliente);

					if (agendamento.Cliente == null)
						throw new ServicoException("Cliente não encontrado!");
				}

				agendamento.ResponsavelAgendamentoPallet = responsavelPallet;
			}

			if (agendamento.Transportador == null)
				agendamento.Transportador = carga.Empresa;


			agendamento.Veiculo = repositorioVeiculo.BuscarPorCodigo(codigoVeiculo);
			agendamento.Motorista = repositorioMotorista.BuscarPorCodigo(codigoMotorista);
			agendamento.QuantidadePallets = quantidadePallets;

			agendamento.NumeroOrdem = Servicos.Embarcador.GestaoPallet.AgendamentoColetaPalletSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork);

			return agendamento;
		}

		#endregion Métodos Privados
	}
}