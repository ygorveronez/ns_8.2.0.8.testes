using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using SGTAdmin.Controllers;
using Newtonsoft.Json;

namespace SGT.WebAdmin.Controllers.Acertos
{
	[CustomAuthorize(new string[] { "DadosAbastecimentoVeiculo", "ContemAbastecimentoPendenteAutorizacao" }, "Acertos/AcertoViagem")]
	public class AcertoAbastecimentoController : BaseController
	{
		#region Construtores

		public AcertoAbastecimentoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Globais

		public async Task<IActionResult> AtualizarAbastecimento()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
			Servicos.Log.TratarErro(" Inicio Controller AtualizarAbastecimento " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
			try
			{
				Repositorio.Embarcador.Acerto.AcertoAbastecimento repAcertoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoAbastecimento(unitOfWork);
				Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
				Repositorio.Embarcador.Acerto.AcertoResumoAbastecimento repAcertoResumoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoResumoAbastecimento(unitOfWork);
				Servicos.Embarcador.Acerto.AcertoViagem servAcertoViagem = new Servicos.Embarcador.Acerto.AcertoViagem(unitOfWork);

				int.TryParse(Request.Params("Codigo"), out int codigo);

				Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem = repAcertoViagem.BuscarPorCodigo(codigo, true);
				if (acertoViagem == null)
					return new JsonpResult(false, "Por favor inicie o acerto de viagem antes.");

				unitOfWork.Start();

				Enum.TryParse(Request.Params("Etapa"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem etapa);
				Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem situacao);

				acertoViagem.Etapa = etapa;
				acertoViagem.Situacao = situacao;
				acertoViagem.DataAlteracao = DateTime.Now;
				acertoViagem.AbastecimentoSalvo = true;

				repAcertoResumoAbastecimento.DeletarResumoSemVeiculo(codigo);

				var contemResumoPendenteArla = servAcertoViagem.ContemAbastecimentoPendenteAutorizacao(codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Arla, unitOfWork);
				var contemResumoPendenteDiesel = servAcertoViagem.ContemAbastecimentoPendenteAutorizacao(codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel, unitOfWork);

				if (!contemResumoPendenteArla && !contemResumoPendenteDiesel)
					acertoViagem.AprovacaoAbastecimento = true;
				else
					acertoViagem.AprovacaoAbastecimento = false;

				if (ConfiguracaoEmbarcador.NaoSolicitarAtuorizacaoAbastecimento)
					acertoViagem.AprovacaoAbastecimento = true;

				repAcertoViagem.Atualizar(acertoViagem, Auditado);

				servAcertoViagem.InserirLogAcerto(acertoViagem, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoLogAcertoViagem.Abastecimentos, this.Usuario);

				unitOfWork.CommitChanges();

				var dynRetorno = new { Codigo = acertoViagem.Codigo }; //servAcertoViagem.RetornaObjetoCompletoAcertoViagem(acertoViagem.Codigo, unitOfWork);

				if (acertoViagem.AprovacaoAbastecimento)
					return new JsonpResult(dynRetorno, true, "Sucesso");
				else
				{
					List<string> placasPendenteArla = servAcertoViagem.PlacasAbastecimentoPendenteAutorizacao(codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Arla, unitOfWork);
					List<string> placasPendenteDiesel = servAcertoViagem.PlacasAbastecimentoPendenteAutorizacao(codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel, unitOfWork);
					string placasRetorno = "";
					if (placasPendenteArla != null && placasPendenteArla.Count > 0)
						placasPendenteDiesel.AddRange(placasPendenteArla);
					if (placasPendenteDiesel != null && placasPendenteDiesel.Count > 0)
						placasPendenteDiesel = placasPendenteDiesel.Distinct().ToList();
					if (placasPendenteDiesel != null && placasPendenteDiesel.Count > 0)
					{
						placasRetorno = string.Join(" ,", placasPendenteDiesel);
						return new JsonpResult(dynRetorno, true, "Placas com pendências: " + placasRetorno);
					}
					else
						return new JsonpResult(dynRetorno, true, "Sucesso");
				}
			}
			catch (Exception ex)
			{
				unitOfWork.Rollback();
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao atualiar os abastecimentos.");
			}
			finally
			{
				Servicos.Log.TratarErro(" Fim Controller AtualizarAbastecimento " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
				unitOfWork.Dispose();
			}
		}

		[AllowAuthenticate]
		public async Task<IActionResult> PesquisarAbastecimento()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
			Servicos.Log.TratarErro(" Inicio Controller PesquisarAbastecimento " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
			try
			{
				int codigoAcerto, codigoVeiculo = 0;
				int.TryParse(Request.Params("CodigoAcerto"), out codigoAcerto);
				int.TryParse(Request.Params("CodigoVeiculo"), out codigoVeiculo);

				Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento;
				Enum.TryParse(Request.Params("TipoAbastecimento"), out tipoAbastecimento);

				Models.Grid.Grid grid = new Models.Grid.Grid(Request);
				grid.header = new List<Models.Grid.Head>();
				grid.AdicionarCabecalho("Codigo", false);
				grid.AdicionarCabecalho("CodigoVeiculo", false);
				grid.AdicionarCabecalho("Data", "DataHora", 15, Models.Grid.Align.center, true);
				grid.AdicionarCabecalho("Fornecedor", "Fornecedor", 30, Models.Grid.Align.left, false);
				grid.AdicionarCabecalho("CodigoFornecedor", false);
				grid.AdicionarCabecalho("Nº Cupom", "NumeroDocumento", 15, Models.Grid.Align.center, false);
				grid.AdicionarCabecalho("KM", "Kilometragem", 15, Models.Grid.Align.right, true);
				grid.AdicionarCabecalho("Hr", "Horimetro", 10, Models.Grid.Align.right, true);
				grid.AdicionarCabecalho("Litros", "Litros", 15, Models.Grid.Align.right, false);
				grid.AdicionarCabecalho("Valor Bomba", "ValorUnitario", 13, Models.Grid.Align.right, false);
				grid.AdicionarCabecalho("Valor Tabela", "ValorTabela", 13, Models.Grid.Align.right, false);
				grid.AdicionarCabecalho("LancadoManualmente", false);
				grid.AdicionarCabecalho("CodigoEquipamento", false);
				grid.AdicionarCabecalho("MoedaCotacaoBancoCentral", false);
				grid.AdicionarCabecalho("DataBaseCRT", false);
				grid.AdicionarCabecalho("ValorMoedaCotacao", false);
				grid.AdicionarCabecalho("ValorOriginalMoedaEstrangeira", false);
				grid.AdicionarCabecalho("Cód. Fechamento", "CodigoFechamentoAbastecimento", 8, Models.Grid.Align.right, false);

				string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
				if (propOrdenar == "Kilometragem" || propOrdenar == "Codigo")
					propOrdenar = "Abastecimento.Kilometragem";
				if (propOrdenar == "DataHora")
					propOrdenar = "Abastecimento.Data";

				Repositorio.Embarcador.Acerto.AcertoAbastecimento repAcertoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoAbastecimento(unitOfWork);
				Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
				Repositorio.Embarcador.Pessoas.PostoCombustivelTabelaValores repTabelaValor = new Repositorio.Embarcador.Pessoas.PostoCombustivelTabelaValores(unitOfWork);
				Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repModalidadeFornecedor = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unitOfWork);

				List<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento> listaAcertoAbastecimento = repAcertoAbastecimento.BuscarPorVeiculoAcerto(codigoAcerto, codigoVeiculo, tipoAbastecimento, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
				grid.setarQuantidadeTotal(repAcertoAbastecimento.ContarBuscarPorVeiculoAcerto(codigoAcerto, codigoVeiculo, tipoAbastecimento));
				var dynXmlAbastecimento = (from obj in listaAcertoAbastecimento
										   select new
										   {
											   obj.Codigo,
											   CodigoVeiculo = obj.Abastecimento.Codigo,
											   DataHora = obj.Abastecimento.Data.Value.ToString("dd/MM/yyyy HH:mm"),
											   Fornecedor = obj.Abastecimento.Posto != null ? obj.Abastecimento.Posto.Nome + " (" + obj.Abastecimento.Posto.CPF_CNPJ_Formatado + ")" : string.Empty,
											   CodigoFornecedor = obj.Abastecimento.Posto != null ? obj.Abastecimento.Posto.CPF_CNPJ : 0,
											   NumeroDocumento = obj.Abastecimento.Documento,
											   Kilometragem = obj.Abastecimento.Kilometragem.ToString("n0"),
											   Horimetro = obj.Abastecimento.Horimetro.ToString("n0"),
											   Litros = obj.Abastecimento.Litros.ToString("n2"),
											   ValorUnitario = obj.Abastecimento.ValorUnitario.ToString("n4"),
											   ValorTabela = obj.Abastecimento.Produto != null && obj.Abastecimento.Posto != null ? repTabelaValor.BuscarValorCombustivel(obj.Abastecimento.Produto.Codigo, obj.Abastecimento.Posto.CPF_CNPJ).ToString("n4") : 0.ToString("n4"),
											   LancadoManualmente = obj.LancadoManualmente,
											   DT_RowColor = obj.Abastecimento.Posto == null || obj.Abastecimento.Posto.Modalidades == null || repModalidadeFornecedor.BuscarPorCliente(obj.Abastecimento.Posto.CPF_CNPJ) == null || !repModalidadeFornecedor.BuscarPorCliente(obj.Abastecimento.Posto.CPF_CNPJ).PagoPorFatura ? "#FAFAD2" : obj.Abastecimento.Situacao != "F" ? "#DCDCDC" : repAbastecimento.AbastecimentoDuplicado(obj.Abastecimento) ? "#FF8C69" : obj.LancadoManualmente ? "#ADD8E6" : "#FFFFFF",
											   CodigoEquipamento = obj.Abastecimento.Equipamento?.Codigo ?? 0,
											   obj.Abastecimento.MoedaCotacaoBancoCentral,
											   DataBaseCRT = obj.Abastecimento.DataBaseCRT.HasValue ? obj.Abastecimento.DataBaseCRT.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
											   ValorMoedaCotacao = obj.Abastecimento.ValorMoedaCotacao.ToString("n10"),
											   ValorOriginalMoedaEstrangeira = obj.Abastecimento.ValorOriginalMoedaEstrangeira.ToString("n2"),
											   CodigoFechamentoAbastecimento = obj.Abastecimento.FechamentoAbastecimento?.Codigo ?? 0
										   }).ToList();

				grid.AdicionaRows(dynXmlAbastecimento);
				return new JsonpResult(grid);
			}
			catch (Exception ex)
			{
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
			}
			finally
			{
				Servicos.Log.TratarErro(" Fim Controller PesquisarAbastecimento " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
				unitOfWork.Dispose();
			}
		}

		public async Task<IActionResult> InserirAbastecimento()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
			Servicos.Log.TratarErro(" Inicio Controller InserirAbastecimento " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
			try
			{
				Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
				Repositorio.Embarcador.Acerto.AcertoAbastecimento repAcertoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoAbastecimento(unitOfWork);
				Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
				Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
				Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
				Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
				Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(unitOfWork);
				Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repModalidadeFornecedor = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unitOfWork);
				Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoAcertoViagem repConfiguracaoFinanceiraContratoAcertoViagem = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoAcertoViagem(unitOfWork);
				Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoAcertoViagem configuracaoAcertoViagem = repConfiguracaoFinanceiraContratoAcertoViagem.BuscarPrimeiroRegistro();

				unitOfWork.Start();

				dynamic abastecimentos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Abastecimentos"));
				foreach (var abast in abastecimentos)
				{
					int codigoAcerto = (int)abast.CodigoAcertoViagem;
					int codigoVeiculo = (int)abast.CodigoVeiculo;
					int codigoCombustivel = (int)abast.Combustivel;
					int codigoAcertoAbastecimento = (int)abast.CodigoAcertoAbastecimento;
					int codigoAbastecimento = (int)abast.Codigo;
					int codigoEquipamento = (int)abast.Equipamento;

					Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento;
					Enum.TryParse((string)abast.TipoAbastecimento, out tipoAbastecimento);

					Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);
					Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem = repAcertoViagem.BuscarPorCodigo(codigoAcerto);
					Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento = repEquipamento.BuscarPorCodigo(codigoEquipamento);

					if (veiculo == null)
						return new JsonpResult(false, "Para adicionar os abastecimentos é necessário informar as cargas que o motorista fez.");

					if (acertoViagem == null)
						return new JsonpResult(false, "Acerto de viagem não localizado.");

					double cnpjFornecedor = (double)abast.Fornecedor;

					DateTime data = ((string)abast.DataAbastecimento).ToDateTime();
					//DateTime.TryParse((string)abast.DataAbastecimento, out data);                    

					decimal litros, valorUnitario, valorTotal = 0;
					decimal.TryParse((string)abast.Litros, out litros);
					decimal.TryParse((string)abast.ValorUnitario, out valorUnitario);
					decimal.TryParse((string)abast.ValorTotal, out valorTotal);

					string numeroDocumento = (string)abast.NumeroDocumento;

					int kilometragem = 0;
					int.TryParse((string)abast.Kilometragem, out kilometragem);

					int horimetro = 0;
					int.TryParse((string)abast.Horimetro, out horimetro);

					horimetro = ((string)abast.Horimetro).ToInt(0);
					kilometragem = ((string)abast.Kilometragem).ToInt(0);

					Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento acertoAbastecimento;
					if (codigoAcertoAbastecimento == 0)
						acertoAbastecimento = new Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento();
					else
						acertoAbastecimento = repAcertoAbastecimento.BuscarPorCodigo(codigoAcertoAbastecimento, true);

					if (kilometragem <= 0 && horimetro <= 0)
						return new JsonpResult(false, "É necessário infomar o KM ou o Horímetro do abastecimento.");

					Dominio.Entidades.Abastecimento abastecimento;
					if (codigoAbastecimento > 0)
						abastecimento = repAbastecimento.BuscarPorCodigo(codigoAbastecimento, true);
					else if (codigoAcertoAbastecimento == 0)
						abastecimento = new Dominio.Entidades.Abastecimento();
					else
					{
						abastecimento = repAbastecimento.BuscarPorCodigoAcerto(codigoAcertoAbastecimento);
						if (abastecimento == null)
							return new JsonpResult(false, "Abastecimento não localizado.");

						bool PagoPorMotorista = false;
						Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas modalidade = null;
						if (abastecimento.Posto.Modalidades != null && abastecimento.Posto.Modalidades.Count > 0)
						{
							modalidade = repModalidadeFornecedor.BuscarPorCliente(abastecimento.Posto.CPF_CNPJ);
							if (modalidade != null)
								PagoPorMotorista = !modalidade.PagoPorFatura;
						}
						if (!PagoPorMotorista && abastecimento.Situacao == "F" && acertoAbastecimento != null && acertoAbastecimento.Codigo > 0)
							return new JsonpResult(false, "Não é possível editar um abastecimento já fechado. Favor remova do Acerto e reabra pela tela de Abastecimento.");

						abastecimento.Initialize();
					}

					if (abastecimento.FechamentoAbastecimento != null && acertoAbastecimento != null && acertoAbastecimento.Codigo > 0)
						return new JsonpResult(false, "Não é possível editar um abastecimento que está em um Fechamento.");

					abastecimento.Data = data;
					abastecimento.Documento = numeroDocumento;
					abastecimento.Kilometragem = kilometragem;
					abastecimento.Horimetro = horimetro;
					abastecimento.Produto = repProduto.BuscarPorCodigo(0, codigoCombustivel);
					if (acertoViagem.Motorista != null)
						abastecimento.Motorista = acertoViagem.Motorista;
					abastecimento.TipoAbastecimento = tipoAbastecimento;
					abastecimento.Litros = litros;
					abastecimento.Posto = repCliente.BuscarPorCPFCNPJ(cnpjFornecedor);
					abastecimento.NomePosto = abastecimento.Posto.Nome;
					if (codigoAbastecimento == 0 && abastecimento.Situacao != "F")
					{
						abastecimento.FechamentoAbastecimento = null;
						abastecimento.Situacao = "A";
						abastecimento.Status = "A";
					}
					abastecimento.DataAlteracao = DateTime.Now;
					abastecimento.ValorUnitario = valorUnitario;
					abastecimento.Veiculo = veiculo;
					abastecimento.Equipamento = equipamento;

					Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moedaCotacaoBancoCentral;
					Enum.TryParse((string)abast.MoedaCotacaoBancoCentral, out moedaCotacaoBancoCentral);

					DateTime? dataBaseCRT = ((string)abast.DataBaseCRT).ToNullableDateTime();

					decimal valorOriginalMoedaEstrangeira, valorMoedaCotacao;
					decimal.TryParse((string)abast.ValorOriginalMoedaEstrangeira, out valorOriginalMoedaEstrangeira);
					decimal.TryParse((string)abast.ValorMoedaCotacao, out valorMoedaCotacao);

					abastecimento.MoedaCotacaoBancoCentral = moedaCotacaoBancoCentral;
					abastecimento.DataBaseCRT = dataBaseCRT > DateTime.MinValue ? dataBaseCRT : null;
					abastecimento.ValorMoedaCotacao = valorMoedaCotacao;
					abastecimento.ValorOriginalMoedaEstrangeira = valorOriginalMoedaEstrangeira;

					Servicos.Embarcador.Abastecimento.Abastecimento.ProcessarViradaKMHorimetro(abastecimento, abastecimento.Veiculo, abastecimento.Equipamento);

					if (!ValidaEntidade(abastecimento, out string erro))
						return new JsonpResult(false, erro);

					if (codigoAcertoAbastecimento == 0 && codigoAbastecimento == 0)
					{
						bool PagoPorMotorista = true;
						Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas modalidade = null;
						if (abastecimento.Posto.Modalidades != null && abastecimento.Posto.Modalidades.Count > 0)
						{
							modalidade = repModalidadeFornecedor.BuscarPorCliente(abastecimento.Posto.CPF_CNPJ);
							if (modalidade != null)
								PagoPorMotorista = !modalidade.PagoPorFatura;
						}
						if (PagoPorMotorista)
							abastecimento.Situacao = "F";
						if (PagoPorMotorista && configuracaoAcertoViagem != null && configuracaoAcertoViagem.GerarMovimentoAutomaticoNaGeracaoAcertoViagem && configuracaoAcertoViagem.TipoMovimentoAbastecimentoPagoPeloMotorista != null)
							abastecimento.TipoMovimento = configuracaoAcertoViagem.TipoMovimentoAbastecimentoPagoPeloMotorista;
						else if (!PagoPorMotorista && configuracaoAcertoViagem != null && configuracaoAcertoViagem.GerarMovimentoAutomaticoNaGeracaoAcertoViagem && configuracaoAcertoViagem.TipoMovimentoAbastecimentoPagoPelaEmpresa != null)
							abastecimento.TipoMovimento = configuracaoAcertoViagem.TipoMovimentoAbastecimentoPagoPelaEmpresa;

						repAbastecimento.Inserir(abastecimento, Auditado);

						if (ConfiguracaoEmbarcador.DesabilitarSaldoViagemAcerto && abastecimento.Situacao == "A")
							Servicos.Embarcador.Abastecimento.Abastecimento.ValidarAbastecimentoInconsistente(ref abastecimento, unitOfWork, abastecimento.Veiculo, null, ConfiguracaoEmbarcador);

						abastecimento.Integrado = false;
						repAbastecimento.Atualizar(abastecimento);
					}
					else
					{
						if (ConfiguracaoEmbarcador.DesabilitarSaldoViagemAcerto && abastecimento.Situacao == "A")
							Servicos.Embarcador.Abastecimento.Abastecimento.ValidarAbastecimentoInconsistente(ref abastecimento, unitOfWork, abastecimento.Veiculo, null, ConfiguracaoEmbarcador);
						repAbastecimento.Atualizar(abastecimento, Auditado);
						Servicos.Auditoria.Auditoria.Auditar(Auditado, abastecimento, null, "Atualizou o abastecimento pela tela de acerto de viagem.", unitOfWork);
					}

					acertoAbastecimento.Abastecimento = abastecimento;
					acertoAbastecimento.AcertoViagem = acertoViagem;
					if (codigoAbastecimento > 0)
						acertoAbastecimento.LancadoManualmente = false;
					else if (codigoAcertoAbastecimento == 0)
						acertoAbastecimento.LancadoManualmente = true;

					if (codigoAcertoAbastecimento == 0)
						repAcertoAbastecimento.Inserir(acertoAbastecimento);
					else
						repAcertoAbastecimento.Atualizar(acertoAbastecimento);

					Servicos.Auditoria.Auditoria.Auditar(Auditado, acertoAbastecimento.AcertoViagem, null, "Abastecimento " + acertoAbastecimento.Descricao + " adicionado ao acerto", unitOfWork);
				}

				unitOfWork.CommitChanges();
				return new JsonpResult(true, "Sucesso");
			}
			catch (Exception ex)
			{
				unitOfWork.Rollback();
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao inserir novo abastecimento.");
			}
			finally
			{
				Servicos.Log.TratarErro(" Fim Controller InserirAbastecimento " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
				unitOfWork.Dispose();
			}
		}

		public async Task<IActionResult> RemoverArlaSelecionados()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
			Servicos.Log.TratarErro(" Inicio Controller RemoverArlaSelecionados " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
			try
			{
				List<int> codigosAbastecimentos = JsonConvert.DeserializeObject<List<int>>(Request.Params("Codigos"));

				foreach (var codigo in codigosAbastecimentos)
				{
					Repositorio.Embarcador.Acerto.AcertoAbastecimento repAcertoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoAbastecimento(unitOfWork);

					Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento acertoAbastecimento = repAcertoAbastecimento.BuscarPorCodigo(codigo);

					repAcertoAbastecimento.Deletar(acertoAbastecimento/*, Auditado*/);
				}

				return new JsonpResult(true, "Sucesso");
			}
			catch (Exception ex)
			{
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao deletar as arlas.");
			}
			finally
			{
				Servicos.Log.TratarErro(" Fim Controller RemoverArlaSelecionados " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
				unitOfWork.Dispose();
			}
		}


		public async Task<IActionResult> RemoverAbastecimentoSelecionados()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
			Servicos.Log.TratarErro(" Inicio Controller RemoverAbastecimentoSelecionados " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
			try
			{
				List<int> codigosAbastecimentos = JsonConvert.DeserializeObject<List<int>>(Request.Params("Codigos"));

				foreach (var codigo in codigosAbastecimentos)
				{
					Repositorio.Embarcador.Acerto.AcertoAbastecimento repAcertoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoAbastecimento(unitOfWork);

					Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento acertoAbastecimento = repAcertoAbastecimento.BuscarPorCodigo(codigo);

					repAcertoAbastecimento.Deletar(acertoAbastecimento/*, Auditado*/);
				}

				return new JsonpResult(true, "Sucesso");
			}
			catch (Exception ex)
			{
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao deletar os abastecimentos.");
			}
			finally
			{
				Servicos.Log.TratarErro(" Fim Controller RemoverAbastecimentoSelecionados " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
				unitOfWork.Dispose();
			}
		}

		public async Task<IActionResult> RemoverAbastecimento()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
			Servicos.Log.TratarErro(" Inicio Controller RemoverAbastecimento " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
			try
			{
				int codigo = 0;
				int.TryParse(Request.Params("Codigo"), out codigo);

				Repositorio.Embarcador.Acerto.AcertoAbastecimento repAcertoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoAbastecimento(unitOfWork);

				Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento acertoAbastecimento = repAcertoAbastecimento.BuscarPorCodigo(codigo);

				repAcertoAbastecimento.Deletar(acertoAbastecimento/*, Auditado*/);

				return new JsonpResult(true, "Sucesso");
			}
			catch (Exception ex)
			{
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao deletar o abastecimento.");
			}
			finally
			{
				Servicos.Log.TratarErro(" Fim Controller RemoverAbastecimento " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
				unitOfWork.Dispose();
			}
		}

		public async Task<IActionResult> DadosAbastecimentoVeiculo()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
			Servicos.Log.TratarErro(" Inicio Controller DadosAbastecimentoVeiculo " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
			try
			{
				Repositorio.Embarcador.Acerto.AcertoAbastecimento repAcertoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoAbastecimento(unitOfWork);
				Repositorio.Embarcador.Acerto.AcertoResumoAbastecimento repAcertoResumoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoResumoAbastecimento(unitOfWork);
				Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
				Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
				Servicos.Embarcador.Acerto.AcertoViagem servAcertoViagem = new Servicos.Embarcador.Acerto.AcertoViagem(unitOfWork);

				int codigoVeiculo, codigo = 0;
				int.TryParse(Request.Params("CodigoVeiculo"), out codigoVeiculo);
				int.TryParse(Request.Params("Codigo"), out codigo);

				decimal mediaIdeal = 0;
				decimal.TryParse(Request.Params("MediaIdeal"), out mediaIdeal);
				if (mediaIdeal <= 0)
					mediaIdeal = 0;

				Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento;
				Enum.TryParse(Request.Params("TipoAbastecimento"), out tipoAbastecimento);

				unitOfWork.Start();

				Dominio.Entidades.Embarcador.Acerto.AcertoResumoAbastecimento resumoAbastecimento = repAcertoResumoAbastecimento.BuscarPorCodigoAcertoVeiculoTipo(codigo, codigoVeiculo, tipoAbastecimento);

				bool inserir = resumoAbastecimento == null;

				if (inserir)
					resumoAbastecimento = new Dominio.Entidades.Embarcador.Acerto.AcertoResumoAbastecimento();
				else
					resumoAbastecimento.Initialize();

				resumoAbastecimento.AcertoViagem = repAcertoViagem.BuscarPorCodigo(codigo);
				resumoAbastecimento.Veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);

				if (resumoAbastecimento.AcertoViagem.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.EmAntamento)
				{
					resumoAbastecimento.KMInicial = (int)repAcertoAbastecimento.KMInicialAbastecimentos(codigo, resumoAbastecimento.Veiculo, tipoAbastecimento);
					resumoAbastecimento.HorimetroInicial = (int)repAcertoAbastecimento.HorimetroInicialAbastecimentos(codigo, resumoAbastecimento.Veiculo, tipoAbastecimento);
				}

				resumoAbastecimento.KMFinal = (int)repAcertoAbastecimento.KMFinalAbastecimentos(codigo, codigoVeiculo, tipoAbastecimento);
				resumoAbastecimento.HorimetroFinal = (int)repAcertoAbastecimento.HorimetroFinalAbastecimentos(codigo, codigoVeiculo, tipoAbastecimento);
				resumoAbastecimento.Litros = repAcertoAbastecimento.QuantidadeLitrosAbastecimentos(codigo, codigoVeiculo, tipoAbastecimento);
				resumoAbastecimento.ValorTotal = repAcertoAbastecimento.ValorTotalAbastecimentos(codigo, codigoVeiculo, tipoAbastecimento);
				resumoAbastecimento.LitrosEquipamento = repAcertoAbastecimento.QuantidadeLitrosAbastecimentosEquipamento(codigo, codigoVeiculo, tipoAbastecimento);
				resumoAbastecimento.ValorTotalEquipamento = repAcertoAbastecimento.ValorTotalAbastecimentosEquipamento(codigo, codigoVeiculo, tipoAbastecimento);
				resumoAbastecimento.TipoAbastecimento = tipoAbastecimento;

				resumoAbastecimento.HorimetroTotal = resumoAbastecimento.HorimetroFinal - resumoAbastecimento.HorimetroInicial;
				if (resumoAbastecimento.HorimetroTotalAjustado <= 0)
					resumoAbastecimento.HorimetroTotalAjustado = resumoAbastecimento.HorimetroTotal;

				if (resumoAbastecimento.HorimetroTotalAjustado < 0)
				{
					resumoAbastecimento.HorimetroTotalAjustado = 0;
					resumoAbastecimento.PercentualAjusteHorimetro = 0;
				}
				if (resumoAbastecimento.HorimetroTotal < 0)
				{
					resumoAbastecimento.HorimetroTotal = 0;
					resumoAbastecimento.HorimetroTotalAjustado = 0;
					resumoAbastecimento.PercentualAjusteHorimetro = 0;
				}

				int horimetroRodado = resumoAbastecimento.HorimetroTotalAjustado;
				if (horimetroRodado > 0 && resumoAbastecimento.LitrosEquipamento > 0)
					resumoAbastecimento.MediaHorimetro = resumoAbastecimento.LitrosEquipamento / horimetroRodado;
				else
					resumoAbastecimento.MediaHorimetro = 0;

				resumoAbastecimento.KMTotal = resumoAbastecimento.KMFinal - resumoAbastecimento.KMInicial;

				if (resumoAbastecimento.KMTotalAjustado <= 0)
					resumoAbastecimento.KMTotalAjustado = resumoAbastecimento.KMTotal;

				if (resumoAbastecimento.KMTotalAjustado < 0)
				{
					resumoAbastecimento.KMTotalAjustado = 0;
					resumoAbastecimento.PercentualAjuste = 0;
				}
				if (resumoAbastecimento.KMTotal < 0)
				{
					resumoAbastecimento.KMTotal = 0;
					resumoAbastecimento.KMTotalAjustado = 0;
					resumoAbastecimento.PercentualAjuste = 0;
				}

				int kmRodado = resumoAbastecimento.KMTotalAjustado;
				if (kmRodado > 0 && resumoAbastecimento.Litros > 0)
					resumoAbastecimento.Media = kmRodado / resumoAbastecimento.Litros;
				else
					resumoAbastecimento.Media = 0;

				if (ConfiguracaoEmbarcador.NaoSolicitarAtuorizacaoAbastecimento)
				{
					resumoAbastecimento.ResumoAprovado = true;
					resumoAbastecimento.ResumoAprovado = true;
				}
				else if (resumoAbastecimento.Media > 0 && resumoAbastecimento.MediaIdeal > 0 && !resumoAbastecimento.ResumoAprovado)
				{
					var contemResumoPendenteDiesel = servAcertoViagem.ContemAbastecimentoVeiculoPendenteAutorizacao(codigo, codigoVeiculo, tipoAbastecimento, unitOfWork);
					if (!contemResumoPendenteDiesel)
						resumoAbastecimento.ResumoAprovado = true;
					else
						resumoAbastecimento.ResumoAprovado = false;
				}
				else if (resumoAbastecimento.MediaHorimetro > 0 && resumoAbastecimento.MediaIdealHorimetro > 0 && !resumoAbastecimento.ResumoAprovado)
				{
					var contemResumoPendenteDiesel = servAcertoViagem.ContemAbastecimentoVeiculoPendenteAutorizacao(codigo, codigoVeiculo, tipoAbastecimento, unitOfWork);
					if (!contemResumoPendenteDiesel)
						resumoAbastecimento.ResumoAprovado = true;
					else
						resumoAbastecimento.ResumoAprovado = false;
				}

				if (inserir)
					repAcertoResumoAbastecimento.Inserir(resumoAbastecimento/*, Auditado*/);
				else
					repAcertoResumoAbastecimento.Atualizar(resumoAbastecimento/*, Auditado*/);

				unitOfWork.CommitChanges();

				var dynRetorno = new
				{
					resumoAbastecimento.KMInicial,
					resumoAbastecimento.KMFinal,
					Litros = resumoAbastecimento.Litros.ToString("n3"),
					ValorTotal = resumoAbastecimento.ValorTotal.ToString("n2"),
					MediaFinal = resumoAbastecimento.Media.ToString("n2"),
					MediaIdeal = resumoAbastecimento.MediaIdeal.ToString("n2"),
					resumoAbastecimento.KMTotal,
					resumoAbastecimento.KMTotalAjustado,
					PercentualAjuste = resumoAbastecimento.PercentualAjuste.ToString("n2"),

					resumoAbastecimento.HorimetroInicial,
					resumoAbastecimento.HorimetroFinal,
					LitrosEquipamento = resumoAbastecimento.LitrosEquipamento.ToString("n3"),
					ValorTotalEquipamento = resumoAbastecimento.ValorTotalEquipamento.ToString("n2"),
					MediaFinalHorimetro = resumoAbastecimento.MediaHorimetro.ToString("n2"),
					MediaIdealHorimetro = resumoAbastecimento.MediaIdealHorimetro.ToString("n2"),
					resumoAbastecimento.HorimetroTotal,
					resumoAbastecimento.HorimetroTotalAjustado,
					PercentalAjusteHorimetro = resumoAbastecimento.PercentualAjusteHorimetro.ToString("n2")
				};

				return new JsonpResult(dynRetorno, true, "Sucesso");
			}
			catch (Exception ex)
			{
				unitOfWork.Rollback();
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao consultar o resumo do veículo.");
			}
			finally
			{
				Servicos.Log.TratarErro(" Fim Controller DadosAbastecimentoVeiculo " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
				unitOfWork.Dispose();
			}
		}

		public async Task<IActionResult> SalvarMediaIdealAbastecimento()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
			Servicos.Log.TratarErro(" Inicio Controller SalvarMediaIdealAbastecimento " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
			try
			{
				Repositorio.Embarcador.Acerto.AcertoAbastecimento repAcertoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoAbastecimento(unitOfWork);
				Repositorio.Embarcador.Acerto.AcertoResumoAbastecimento repAcertoResumoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoResumoAbastecimento(unitOfWork);
				Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
				Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

				int codigoVeiculo, codigo, kmTotalAjustado = 0;
				int.TryParse(Request.Params("CodigoVeiculo"), out codigoVeiculo);
				int.TryParse(Request.Params("Codigo"), out codigo);
				int.TryParse(Utilidades.String.OnlyNumbers(Request.Params("KMTotalAjustado")), out kmTotalAjustado);

				decimal mediaIdeal, percentalAjusteKM = 0;
				decimal.TryParse(Request.Params("MediaIdeal"), out mediaIdeal);
				decimal.TryParse(Request.Params("PercentalAjusteKM"), out percentalAjusteKM);
				if (mediaIdeal <= 0)
					mediaIdeal = 0;

				Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento;
				Enum.TryParse(Request.Params("TipoAbastecimento"), out tipoAbastecimento);

				unitOfWork.Start();

				Dominio.Entidades.Embarcador.Acerto.AcertoResumoAbastecimento resumoAbastecimento;

				resumoAbastecimento = repAcertoResumoAbastecimento.BuscarPorCodigoAcertoVeiculoTipo(codigo, codigoVeiculo, tipoAbastecimento);
				bool inserir = resumoAbastecimento == null;
				if (inserir)
					return new JsonpResult(null, true, "Sucesso");

				resumoAbastecimento.Initialize();

				int kmRodado = kmTotalAjustado;
				if (kmRodado <= 0)
				{
					kmRodado = resumoAbastecimento.KMFinal - resumoAbastecimento.KMInicial;
				}
				if (kmRodado > 0 && resumoAbastecimento.Litros > 0)
					resumoAbastecimento.Media = kmRodado / resumoAbastecimento.Litros;
				else
					resumoAbastecimento.Media = 0;
				if (kmRodado < 0)
					kmRodado = 0;

				resumoAbastecimento.KMTotalAjustado = kmRodado;
				resumoAbastecimento.PercentualAjuste = percentalAjusteKM;
				resumoAbastecimento.MediaIdeal = mediaIdeal;

				repAcertoResumoAbastecimento.Atualizar(resumoAbastecimento/*, Auditado*/);

				unitOfWork.CommitChanges();

				var dynRetorno = new
				{
					resumoAbastecimento.KMInicial,
					resumoAbastecimento.KMFinal,
					Litros = resumoAbastecimento.Litros.ToString("n3"),
					ValorTotal = resumoAbastecimento.ValorTotal.ToString("n2"),
					MediaFinal = resumoAbastecimento.Media.ToString("n2"),
					MediaIdeal = resumoAbastecimento.MediaIdeal.ToString("n2"),
					resumoAbastecimento.KMTotal,
					resumoAbastecimento.KMTotalAjustado,
					PercentualAjuste = resumoAbastecimento.PercentualAjuste.ToString("n2")
				};

				return new JsonpResult(dynRetorno, true, "Sucesso");
			}
			catch (Exception ex)
			{
				unitOfWork.Rollback();
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao consultar o resumo do veículo.");
			}
			finally
			{
				Servicos.Log.TratarErro(" Fim Controller SalvarMediaIdealAbastecimento " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
				unitOfWork.Dispose();
			}
		}

		public async Task<IActionResult> SalvarMediaIdealHorimetroAbastecimento()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
			Servicos.Log.TratarErro(" Inicio Controller SalvarMediaIdealHorimetroAbastecimento " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
			try
			{
				Repositorio.Embarcador.Acerto.AcertoAbastecimento repAcertoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoAbastecimento(unitOfWork);
				Repositorio.Embarcador.Acerto.AcertoResumoAbastecimento repAcertoResumoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoResumoAbastecimento(unitOfWork);
				Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
				Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

				int codigoVeiculo, codigo, horimetroTotalAjustado = 0;
				int.TryParse(Request.Params("CodigoVeiculo"), out codigoVeiculo);
				int.TryParse(Request.Params("Codigo"), out codigo);
				int.TryParse(Utilidades.String.OnlyNumbers(Request.Params("HorimetroTotalAjustado")), out horimetroTotalAjustado);

				decimal mediaIdealHorimetro, percentalAjusteHorimetro = 0;
				decimal.TryParse(Request.Params("MediaIdealHorimetro"), out mediaIdealHorimetro);
				decimal.TryParse(Request.Params("PercentalAjusteHorimetro"), out percentalAjusteHorimetro);
				if (mediaIdealHorimetro <= 0)
					mediaIdealHorimetro = 0;

				Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento;
				Enum.TryParse(Request.Params("TipoAbastecimento"), out tipoAbastecimento);

				unitOfWork.Start();

				Dominio.Entidades.Embarcador.Acerto.AcertoResumoAbastecimento resumoAbastecimento;

				resumoAbastecimento = repAcertoResumoAbastecimento.BuscarPorCodigoAcertoVeiculoTipo(codigo, codigoVeiculo, tipoAbastecimento);
				bool inserir = resumoAbastecimento == null;
				if (inserir)
					return new JsonpResult(null, true, "Sucesso");

				resumoAbastecimento.Initialize();

				int horimetroRodado = horimetroTotalAjustado;
				if (horimetroRodado <= 0)
				{
					horimetroRodado = resumoAbastecimento.HorimetroFinal - resumoAbastecimento.HorimetroInicial;
				}
				if (horimetroRodado > 0 && resumoAbastecimento.Litros > 0)
					resumoAbastecimento.MediaHorimetro = resumoAbastecimento.Litros / horimetroRodado;
				else
					resumoAbastecimento.MediaHorimetro = 0;
				if (horimetroRodado < 0)
					horimetroRodado = 0;

				resumoAbastecimento.HorimetroTotalAjustado = horimetroRodado;
				resumoAbastecimento.PercentualAjusteHorimetro = percentalAjusteHorimetro;
				resumoAbastecimento.MediaIdealHorimetro = mediaIdealHorimetro;

				repAcertoResumoAbastecimento.Atualizar(resumoAbastecimento/*, Auditado*/);

				unitOfWork.CommitChanges();

				var dynRetorno = new
				{
					resumoAbastecimento.HorimetroInicial,
					resumoAbastecimento.HorimetroFinal,
					Litros = resumoAbastecimento.Litros.ToString("n3"),
					ValorTotal = resumoAbastecimento.ValorTotal.ToString("n2"),
					MediaFinalHorimetro = resumoAbastecimento.MediaHorimetro.ToString("n2"),
					MediaIdealHorimetro = resumoAbastecimento.MediaIdealHorimetro.ToString("n2"),
					resumoAbastecimento.HorimetroTotal,
					resumoAbastecimento.HorimetroTotalAjustado,
					PercentualAjusteHorimetro = resumoAbastecimento.PercentualAjusteHorimetro.ToString("n2")
				};

				return new JsonpResult(dynRetorno, true, "Sucesso");
			}
			catch (Exception ex)
			{
				unitOfWork.Rollback();
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao consultar o resumo do veículo.");
			}
			finally
			{
				Servicos.Log.TratarErro(" Fim Controller SalvarMediaIdealHorimetroAbastecimento " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
				unitOfWork.Dispose();
			}
		}

		public async Task<IActionResult> AutorizarAbastecimento()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
			Servicos.Log.TratarErro(" Inicio Controller AutorizarAbastecimento " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
			try
			{
				List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Acertos/AcertoViagem");

				Repositorio.Embarcador.Acerto.AcertoResumoAbastecimento repAcertoResumoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoResumoAbastecimento(unitOfWork);
				Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
				Servicos.Embarcador.Acerto.AcertoViagem servAcertoViagem = new Servicos.Embarcador.Acerto.AcertoViagem(unitOfWork);

				int codigoVeiculo, codigo = 0;
				int.TryParse(Request.Params("CodigoVeiculo"), out codigoVeiculo);
				int.TryParse(Request.Params("Codigo"), out codigo);

				Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento;
				Enum.TryParse(Request.Params("TipoAbastecimento"), out tipoAbastecimento);

				if (tipoAbastecimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel)
				{
					if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Acerto_PermiteLiberarAutorizarAbastecimento)))
						return new JsonpResult(false, "Seu usuário não possui permissão para autorizar o abastecimento.");
				}
				else
				{
					if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Acerto_PermiteLiberarAutorizarArla)))
						return new JsonpResult(false, "Seu usuário não possui permissão para autorizar a arla.");
				}


				Dominio.Entidades.Embarcador.Acerto.AcertoResumoAbastecimento resumoAbastecimento;
				resumoAbastecimento = repAcertoResumoAbastecimento.BuscarPorCodigoAcertoVeiculoTipo(codigo, codigoVeiculo, tipoAbastecimento);
				bool inserir = resumoAbastecimento == null;
				if (inserir)
					return new JsonpResult(false, "Não foi possível encontrar o resumo do abastecimento.");

				unitOfWork.Start();

				resumoAbastecimento.ResumoAprovado = true;

				unitOfWork.CommitChanges();

				return new JsonpResult(true, "Abastecimento aprovado com sucesso.");
			}
			catch (Exception ex)
			{
				unitOfWork.Rollback();
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao consultar o resumo do veículo.");
			}
			finally
			{
				Servicos.Log.TratarErro(" Fim Controller AutorizarAbastecimento " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
				unitOfWork.Dispose();
			}
		}

		[AllowAuthenticate]
		public async Task<IActionResult> PesquisarCombustivel()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
			Servicos.Log.TratarErro(" Inicio Controller PesquisarCombustivel " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
			try
			{
				int codigo = 0;
				int.TryParse(Request.Params("Codigo"), out codigo);

				Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento;
				Enum.TryParse(Request.Params("TipoAbastecimento"), out tipoAbastecimento);

				Repositorio.Embarcador.Acerto.AcertoAbastecimento repAcertoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoAbastecimento(unitOfWork);
				Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento abastecimento = repAcertoAbastecimento.BuscarPorCodigo(codigo);

				var dynRetorno = new
				{
					Codigo = abastecimento.Codigo,
					VeiculoAbastecimento = abastecimento.Abastecimento.Veiculo != null ? new { Codigo = abastecimento.Abastecimento.Veiculo.Codigo, Descricao = abastecimento.Abastecimento.Veiculo.Placa } : null,
					Fornecedor = abastecimento.Abastecimento.Posto != null ? new { Codigo = abastecimento.Abastecimento.Posto.CPF_CNPJ, Descricao = abastecimento.Abastecimento.Posto.Nome } : null,
					Equipamento = abastecimento.Abastecimento.Equipamento != null ? new { Codigo = abastecimento.Abastecimento.Equipamento.Codigo, Descricao = abastecimento.Abastecimento.Equipamento.Descricao } : null,
					DataAbastecimento = abastecimento.Abastecimento.Data.Value.ToString("dd/MM/yyyy HH:mm"),
					Combustivel = abastecimento.Abastecimento.Produto != null ? new { Codigo = abastecimento.Abastecimento.Produto.Codigo, Descricao = abastecimento.Abastecimento.Produto.Descricao } : null,
					NumeroDocumento = abastecimento.Abastecimento.Documento,
					Horimetro = abastecimento.Abastecimento.Horimetro,
					Kilometragem = abastecimento.Abastecimento.Kilometragem,
					Litros = abastecimento.Abastecimento.Litros.ToString("n2"),
					ValorUnitario = abastecimento.Abastecimento.ValorUnitario.ToString("n4"),
					ValorTotal = abastecimento.Abastecimento.ValorTotal.ToString("n2"),
					TipoVeiculo = abastecimento.Abastecimento.Veiculo?.DescricaoTipoVeiculo ?? "",
					abastecimento.Abastecimento.MoedaCotacaoBancoCentral,
					DataBaseCRT = abastecimento.Abastecimento.DataBaseCRT.HasValue ? abastecimento.Abastecimento.DataBaseCRT.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
					ValorMoedaCotacao = abastecimento.Abastecimento.ValorMoedaCotacao.ToString("n10"),
					ValorOriginalMoedaEstrangeira = abastecimento.Abastecimento.ValorOriginalMoedaEstrangeira.ToString("n2"),
					CodigoFechamentoAbastecimento = abastecimento.Abastecimento.FechamentoAbastecimento?.Codigo ?? 0
				};

				return new JsonpResult(dynRetorno, true, "Sucesso");
			}
			catch (Exception ex)
			{
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
			}
			finally
			{
				Servicos.Log.TratarErro(" Fim Controller PesquisarCombustivel " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
				unitOfWork.Dispose();
			}
		}

		public async Task<IActionResult> ContemAbastecimentoPendenteAutorizacao()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
			Servicos.Log.TratarErro(" Inicio Controller ContemAbastecimentoPendenteAutorizacao " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
			try
			{
				Repositorio.Embarcador.Acerto.AcertoAbastecimento repAcertoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoAbastecimento(unitOfWork);
				Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
				Servicos.Embarcador.Acerto.AcertoViagem servAcertoViagem = new Servicos.Embarcador.Acerto.AcertoViagem(unitOfWork);

				int codigo, codigoVeiculo;
				int.TryParse(Request.Params("Codigo"), out codigo);
				int.TryParse(Request.Params("CodigoVeiculo"), out codigoVeiculo);

				Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento;
				Enum.TryParse(Request.Params("TipoAbastecimento"), out tipoAbastecimento);

				Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem;
				if (codigo > 0)
					acertoViagem = repAcertoViagem.BuscarPorCodigo(codigo);
				else
					return new JsonpResult(false, "Por favor inicie o acerto de viagem antes.");

				var contemResumoPendente = servAcertoViagem.ContemAbastecimentoVeiculoPendenteAutorizacao(codigo, codigoVeiculo, tipoAbastecimento, unitOfWork);
				if (ConfiguracaoEmbarcador.NaoSolicitarAtuorizacaoAbastecimento)
					contemResumoPendente = false;

				var dynRetorno = new
				{
					ContemResumoPendente = contemResumoPendente
				};

				return new JsonpResult(dynRetorno, true, "Sucesso");
			}
			catch (Exception ex)
			{
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao atualiar os abastecimentos.");
			}
			finally
			{
				Servicos.Log.TratarErro(" Fim Controller ContemAbastecimentoPendenteAutorizacao " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
				unitOfWork.Dispose();
			}
		}

		#endregion

		private bool ValidaEntidade(Dominio.Entidades.Abastecimento abastecimento, out string erro)
		{
			erro = string.Empty;

			Dominio.Entidades.Veiculo veiculo = abastecimento.Veiculo;
			if (veiculo != null)
			{
				if (veiculo.Modelo != null && (veiculo.Modelo.PossuiArla32 == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Nao && abastecimento.Produto.CodigoNCM.StartsWith("310210")))
				{
					erro = "O modelo do veículo selecionado não permite o lançamento de ARLA 32.";
					return false;
				}

				if (veiculo.Tipo == "T" && abastecimento.TipoMovimento == null)
				{
					erro = "Movimento Financeiro é obrigatório quando Veículo é de Terceiro.";
					return false;
				}
			}

			if (abastecimento.Data.HasValue && abastecimento.Data.Value > DateTime.Now)
			{
				erro = "Não é possível lançar um abastecimento com data futura.";
				return false;
			}

			if (veiculo == null && abastecimento.Equipamento == null)
			{
				erro = "Favor informe um veículo ou um equipamento para o lançamento do abastecimento.";
				return false;
			}

			return string.IsNullOrEmpty(erro);
		}
	}
}


