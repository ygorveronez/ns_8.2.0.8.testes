using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.ConfiguracaoContabil
{
	[CustomAuthorize("ConfiguracaoContabil/ConfiguracaoContaContabil")]
	public class ConfiguracaoContaContabilController : BaseController
	{
		#region Construtores

		public ConfiguracaoContaContabilController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Globais

		public async Task<IActionResult> Pesquisa()
		{
			try
			{
				return new JsonpResult(ObterGridPesquisa());
			}
			catch (Exception ex)
			{
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
			}
		}

		public async Task<IActionResult> Adicionar()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
			try
			{
				unitOfWork.Start();
				Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil repConfiguracaoContaContabil = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil(unitOfWork);

				Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil configuracaoContaContabil = new Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil();
				string retorno = preecherConfiguracaoContaContabil(ref configuracaoContaContabil, unitOfWork);

				if (string.IsNullOrWhiteSpace(retorno))
				{
					repConfiguracaoContaContabil.Inserir(configuracaoContaContabil, Auditado);
					SalvarConfiguracoesDeContabilizacao(configuracaoContaContabil, unitOfWork);
					SalvarConfiguracoesDeEscrituracao(configuracaoContaContabil, unitOfWork);
					SalvarConfiguracoesDeProvisao(configuracaoContaContabil, unitOfWork);
					unitOfWork.CommitChanges();

					Servicos.Embarcador.ConfiguracaoContabil.RegraConfiguracaoContaContabil regraConfiguracaoContaContabil = Servicos.Embarcador.ConfiguracaoContabil.RegraConfiguracaoContaContabil.GetInstance(unitOfWork);
					regraConfiguracaoContaContabil.AtualizarConfiguracaoContaContabil(unitOfWork);

					Servicos.Embarcador.ICMS.RegrasCalculoImpostos regrasCalculoImpostos = Servicos.Embarcador.ICMS.RegrasCalculoImpostos.GetInstance(unitOfWork);
					return new JsonpResult(true);
				}
				else
				{
					unitOfWork.Rollback();
					return new JsonpResult(false, true, retorno);
				}
			}
			catch (Exception ex)
			{
				unitOfWork.Rollback();
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
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
				unitOfWork.Start();
				Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil repConfiguracaoContaContabil = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil(unitOfWork);
				Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil configuracaoContaContabil = repConfiguracaoContaContabil.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);
				string retorno = preecherConfiguracaoContaContabil(ref configuracaoContaContabil, unitOfWork);

				if (string.IsNullOrWhiteSpace(retorno))
				{
					Dominio.Entidades.Auditoria.HistoricoObjeto historico = repConfiguracaoContaContabil.Atualizar(configuracaoContaContabil, Auditado);
					SalvarConfiguracoesDeContabilizacao(configuracaoContaContabil, unitOfWork, historico);
					SalvarConfiguracoesDeEscrituracao(configuracaoContaContabil, unitOfWork, historico);
					SalvarConfiguracoesDeProvisao(configuracaoContaContabil, unitOfWork, historico);
					unitOfWork.CommitChanges();
					Servicos.Embarcador.ConfiguracaoContabil.RegraConfiguracaoContaContabil regraConfiguracaoContaContabil = Servicos.Embarcador.ConfiguracaoContabil.RegraConfiguracaoContaContabil.GetInstance(unitOfWork);
					regraConfiguracaoContaContabil.AtualizarConfiguracaoContaContabil(unitOfWork);
					return new JsonpResult(true);
				}
				else
				{
					unitOfWork.Rollback();
					return new JsonpResult(false, true, retorno);
				}
			}
			catch (Exception ex)
			{
				unitOfWork.Rollback();
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

		public async Task<IActionResult> BuscarPorCodigo()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
			try
			{
				int codigo = int.Parse(Request.Params("Codigo"));
				Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil repConfiguracaoContaContabil = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil(unitOfWork);
				Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil configuracaoContaContabil = repConfiguracaoContaContabil.BuscarPorCodigo(codigo);

				var entidade = new
				{
					configuracaoContaContabil.Ativo,
					configuracaoContaContabil.Codigo,
					Destinatario = new { Codigo = configuracaoContaContabil.Destinatario != null ? configuracaoContaContabil.Destinatario.CPF_CNPJ : 0, Descricao = configuracaoContaContabil.Destinatario != null ? configuracaoContaContabil.Destinatario.Descricao : "" },
					Remetente = new { Codigo = configuracaoContaContabil.Remetente != null ? configuracaoContaContabil.Remetente.CPF_CNPJ : 0, Descricao = configuracaoContaContabil.Remetente != null ? configuracaoContaContabil.Remetente.Descricao : "" },
					Tomador = new { Codigo = configuracaoContaContabil.Tomador != null ? configuracaoContaContabil.Tomador.CPF_CNPJ : 0, Descricao = configuracaoContaContabil.Tomador != null ? configuracaoContaContabil.Tomador.Descricao : "" },
					CategoriaDestinatario = new { Codigo = configuracaoContaContabil.CategoriaDestinatario?.Codigo ?? 0, Descricao = configuracaoContaContabil.CategoriaDestinatario?.Descricao ?? "" },
					CategoriaRemetente = new { Codigo = configuracaoContaContabil.CategoriaRemetente?.Codigo ?? 0, Descricao = configuracaoContaContabil.CategoriaRemetente?.Descricao ?? "" },
					CategoriaTomador = new { Codigo = configuracaoContaContabil.CategoriaTomador?.Codigo ?? 0, Descricao = configuracaoContaContabil.CategoriaTomador?.Descricao ?? "" },
					GrupoDestinatario = new { Codigo = configuracaoContaContabil.GrupoDestinatario != null ? configuracaoContaContabil.GrupoDestinatario.Codigo : 0, Descricao = configuracaoContaContabil.GrupoDestinatario != null ? configuracaoContaContabil.GrupoDestinatario.Descricao : "" },
					GrupoRemetente = new { Codigo = configuracaoContaContabil.GrupoRemetente != null ? configuracaoContaContabil.GrupoRemetente.Codigo : 0, Descricao = configuracaoContaContabil.GrupoRemetente != null ? configuracaoContaContabil.GrupoRemetente.Descricao : "" },
					GrupoTomador = new { Codigo = configuracaoContaContabil.GrupoTomador != null ? configuracaoContaContabil.GrupoTomador.Codigo : 0, Descricao = configuracaoContaContabil.GrupoTomador != null ? configuracaoContaContabil.GrupoTomador.Descricao : "" },
					TipoOperacao = new { Codigo = configuracaoContaContabil.TipoOperacao != null ? configuracaoContaContabil.TipoOperacao.Codigo : 0, Descricao = configuracaoContaContabil.TipoOperacao != null ? configuracaoContaContabil.TipoOperacao.Descricao : "" },
					Empresa = new { Codigo = configuracaoContaContabil.Empresa != null ? configuracaoContaContabil.Empresa.Codigo : 0, Descricao = configuracaoContaContabil.Empresa != null ? configuracaoContaContabil.Empresa.Descricao : "" },
					GrupoProduto = new { Codigo = configuracaoContaContabil.GrupoProduto != null ? configuracaoContaContabil.GrupoProduto.Codigo : 0, Descricao = configuracaoContaContabil.GrupoProduto != null ? configuracaoContaContabil.GrupoProduto.Descricao : "" },
					RotaFrete = new { Codigo = configuracaoContaContabil.RotaFrete != null ? configuracaoContaContabil.RotaFrete.Codigo : 0, Descricao = configuracaoContaContabil.RotaFrete != null ? configuracaoContaContabil.RotaFrete.Descricao : "" },
					TipoOcorrencia = new { Codigo = configuracaoContaContabil.TipoOcorrencia != null ? configuracaoContaContabil.TipoOcorrencia.Codigo : 0, Descricao = configuracaoContaContabil.TipoOcorrencia != null ? configuracaoContaContabil.TipoOcorrencia.Descricao : "" },
					ModeloDocumentoFiscal = new { Codigo = configuracaoContaContabil.ModeloDocumentoFiscal != null ? configuracaoContaContabil.ModeloDocumentoFiscal.Codigo : 0, Descricao = configuracaoContaContabil.ModeloDocumentoFiscal != null ? configuracaoContaContabil.ModeloDocumentoFiscal.Descricao : "" },
					CanalEntrega = new { Codigo = configuracaoContaContabil.CanalEntrega?.Codigo ?? 0, Descricao = configuracaoContaContabil.CanalEntrega?.Descricao ?? string.Empty },
					CanalVenda = new { Codigo = configuracaoContaContabil.CanalVenda?.Codigo ?? 0, Descricao = configuracaoContaContabil.CanalVenda?.Descricao ?? string.Empty },
					TipoDT = new { Codigo = configuracaoContaContabil.TipoDT?.Codigo ?? 0, Descricao = configuracaoContaContabil.TipoDT?.Descricao ?? string.Empty },
					ConfiguracaoContaContabilContabilizacaos = (from obj in configuracaoContaContabil.ConfiguracaoContaContabilContabilizacoes
																select new
																{
																	Codigo = obj.Codigo,
																	PlanoConta = new { obj.PlanoConta.Codigo, Descricao = obj.PlanoConta.BuscarDescricao },
																	PlanoContaContraPartidaProvisao = new { Codigo = obj.PlanoContaContraPartidaProvisao?.Codigo ?? 0, Descricao = obj.PlanoContaContraPartidaProvisao?.BuscarDescricao ?? "" },
																	obj.TipoContabilizacao,
																	obj.TipoContaContabil,
																	obj.ComponentesDeFreteDoTipoDescontoNaoDevemSomarNaContabilizacao
                                                                }).ToList(),
					ConfiguracaoContaContabilEscrituracaos = (from obj in configuracaoContaContabil.ConfiguracaoContaContabilEscrituracoes
															  select new
															  {
																  Codigo = obj.Codigo,
																  PlanoConta = new { obj.PlanoConta.Codigo, Descricao = obj.PlanoConta.BuscarDescricao },
																  obj.TipoContabilizacao,
																  obj.TipoContaContabil,
																  obj.SempreGerarRegistro
															  }).ToList(),
					ConfiguracaoContaContabilProvisoes = (from obj in configuracaoContaContabil.ConfiguracaoContaContabilProvisoes
														  select new
														  {
															  Codigo = obj.Codigo,
															  PlanoConta = new { obj.PlanoConta.Codigo, Descricao = obj.PlanoConta.BuscarDescricao },
															  obj.TipoContabilizacao,
															  obj.TipoContaContabil,
                                                              ComponentesDeFreteDoTipoDescontoNaoDevemSomarNaProvisao = obj.ComponentesDeFreteDoTipoDescontoNaoDevemSomarNaProvisao ?? false
                                                          }).ToList(),
				};

				return new JsonpResult(entidade);
			}
			catch (Exception ex)
			{
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
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
				int codigo = int.Parse(Request.Params("codigo"));
				Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil repConfiguracaoContaContabil = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil(unitOfWork);
				Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil configuracaoContaContabil = repConfiguracaoContaContabil.BuscarPorCodigo(codigo);
				repConfiguracaoContaContabil.Deletar(configuracaoContaContabil, Auditado);
				Servicos.Embarcador.ConfiguracaoContabil.RegraConfiguracaoContaContabil regraConfiguracaoContaContabil = Servicos.Embarcador.ConfiguracaoContabil.RegraConfiguracaoContaContabil.GetInstance(unitOfWork);
				regraConfiguracaoContaContabil.AtualizarConfiguracaoContaContabil(unitOfWork);
				return new JsonpResult(true);
			}
			catch (Exception ex)
			{
				if (ExcessaoPorPossuirDependeciasNoBanco(ex))
					return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
				else
				{
					Servicos.Log.TratarErro(ex);
					return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
				}
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

		public async Task<IActionResult> ExportarPesquisa()
		{
			try
			{
				var grid = ObterGridPesquisa();

				byte[] arquivoBinario = grid.GerarExcel();

				if (arquivoBinario != null)
					return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

				return new JsonpResult(false, Localization.Resources.Pedidos.TipoOperacao.OcorreuUmaFalhaAoGerarArquivo);
			}
			catch (Exception excecao)
			{
				Servicos.Log.TratarErro(excecao);

				return new JsonpResult(false, Localization.Resources.Pedidos.TipoOperacao.OcorreuUmaFalhaAoExportar);
			}
		}

		public async Task<IActionResult> PossuiIVA()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

			try
			{
				Repositorio.Embarcador.Contabeis.ImpostoValorAgregado repositorioImpostoValorAgregado = new Repositorio.Embarcador.Contabeis.ImpostoValorAgregado(unitOfWork);

				bool possuiIVA = repositorioImpostoValorAgregado.PossuiRegistroIVA();

				return new JsonpResult(possuiIVA);
			}
			catch (Exception ex)
			{
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu ao buscar as configurações de IVA.");
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

		#endregion

		#region MétodosPrivados

		private Models.Grid.Grid ObterGridPesquisa()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
			try
			{
				Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil repConfiguracaoContaContabil = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil(unitOfWork);
				Repositorio.Embarcador.Contabeis.ImpostoValorAgregado repositorioImpostoValorAgregado = new Repositorio.Embarcador.Contabeis.ImpostoValorAgregado(unitOfWork);
				bool possuiIVA = repositorioImpostoValorAgregado.PossuiRegistroIVA();

				Dominio.ObjetosDeValor.Embarcador.Contabil.FiltroPesquisaConfiguracaoContaContabil filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

				Models.Grid.Grid grid = new Models.Grid.Grid(Request)
				{
					header = new List<Models.Grid.Head>()
				};

				grid.AdicionarCabecalho("Codigo", false);
				grid.AdicionarCabecalho("Transportador", "Empresa", 10, Models.Grid.Align.left, true);
				grid.AdicionarCabecalho("Remetente", "Remetente", 10, Models.Grid.Align.left, true);
				grid.AdicionarCabecalho("Destinatário", "Destinatario", 10, Models.Grid.Align.left, true);
				grid.AdicionarCabecalho("Tomador", "Tomador", 10, Models.Grid.Align.left, true);
				grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacao", 10, Models.Grid.Align.left, true);
				grid.AdicionarCabecalho("Modelo de Documento", "ModeloDocumento", 5, Models.Grid.Align.left, false);
				grid.AdicionarCabecalho("Tipo de Ocorrência", "TipoOcorrencia", 10, Models.Grid.Align.left, false);

				if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
					grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 5, Models.Grid.Align.center, false);

				if (possuiIVA)
				{
					grid.AdicionarCabecalho("Canal de Entrega", "CanalEntrega", 10, Models.Grid.Align.left, false);
					grid.AdicionarCabecalho("Canal de Venda", "CanalVenda", 10, Models.Grid.Align.left, false);
					grid.AdicionarCabecalho("Tipo DT", "TipoDT", 10, Models.Grid.Align.left, false);
				}

				Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
				int totalRegistros = repConfiguracaoContaContabil.ContarConsulta(filtrosPesquisa);
				List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil> listaConfiguracaoContaContabil = totalRegistros > 0 ? repConfiguracaoContaContabil.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil>();

				var retorno = (
					from obj in listaConfiguracaoContaContabil
					select new
					{
						obj.Codigo,
						Empresa = obj.Empresa?.Descricao ?? "",
						TipoOperacao = obj.TipoOperacao?.Descricao ?? "",
						ModeloDocumento = obj.ModeloDocumentoFiscal?.Abreviacao ?? "",
						TipoOcorrencia = obj.TipoOcorrencia?.Descricao ?? "",
						Tomador = obj.CategoriaTomador != null ? obj.Tomador.Categoria.Descricao : (obj.GrupoTomador != null ? obj.GrupoTomador.Descricao : (obj.Tomador != null ? obj.Tomador.Descricao : "")),
						Remetente = obj.CategoriaRemetente != null ? obj.CategoriaRemetente.Descricao : (obj.GrupoRemetente != null ? obj.GrupoRemetente.Descricao : (obj.Remetente != null ? obj.Remetente.Descricao : "")),
						Destinatario = obj.CategoriaDestinatario != null ? obj.CategoriaDestinatario.Descricao : (obj.GrupoDestinatario != null ? obj.GrupoDestinatario.Descricao : (obj.Destinatario != null ? obj.Destinatario.Descricao : "")),
						obj.DescricaoAtivo,
						CanalEntrega = obj.CanalEntrega?.Descricao ?? string.Empty,
						CanalVenda = obj.CanalVenda?.Descricao ?? string.Empty,
						TipoDT = obj.TipoDT?.Descricao ?? string.Empty
					}
				).ToList();

				grid.AdicionaRows(retorno);
				grid.setarQuantidadeTotal(totalRegistros);

				return grid;
			}
			catch (Exception)
			{
				throw;
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

		private string preecherConfiguracaoContaContabil(ref Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil configuracaoContaContabil, Repositorio.UnitOfWork unitOfWork)
		{
			Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
			Repositorio.Embarcador.Pessoas.CategoriaPessoa repositorioCategoriaPessoa = new Repositorio.Embarcador.Pessoas.CategoriaPessoa(unitOfWork);
			Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
			Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
			Repositorio.Embarcador.Produtos.GrupoProduto repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProduto(unitOfWork);
			Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
			Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
			Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
			Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);
			Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil repConfiguracaoContaContabil = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil(unitOfWork);
			Repositorio.Embarcador.Pedidos.CanalEntrega repositorioCanalEntrega = new Repositorio.Embarcador.Pedidos.CanalEntrega(unitOfWork);
			Repositorio.Embarcador.Pedidos.CanalVenda repositorioCanalVenda = new Repositorio.Embarcador.Pedidos.CanalVenda(unitOfWork);
			Repositorio.Embarcador.Cargas.TipoDocumentoTransporte.TipoDocumentoTransporte repositorioTipoDocumentoTransporte = new Repositorio.Embarcador.Cargas.TipoDocumentoTransporte.TipoDocumentoTransporte(unitOfWork);
			Repositorio.Embarcador.Contabeis.ImpostoValorAgregado repositorioImpostoValorAgregado = new Repositorio.Embarcador.Contabeis.ImpostoValorAgregado(unitOfWork);

			string retorno = "";

			double remetente, destinatario, tomador;
			double.TryParse(Request.Params("Remetente"), out remetente);
			double.TryParse(Request.Params("Destinatario"), out destinatario);
			double.TryParse(Request.Params("Tomador"), out tomador);

			int grupoRemetente, grupoDestinatario, grupoTomador, rotaFrete, tipoOperacao, grupoProduto, empresa, modeloDocumento, tipoOcorrencia;
			int.TryParse(Request.Params("GrupoRemetente"), out grupoRemetente);
			int.TryParse(Request.Params("GrupoDestinatario"), out grupoDestinatario);
			int.TryParse(Request.Params("GrupoTomador"), out grupoTomador);
			int.TryParse(Request.Params("TipoOperacao"), out tipoOperacao);
			int.TryParse(Request.Params("Empresa"), out empresa);
			int.TryParse(Request.Params("GrupoProduto"), out grupoProduto);
			int.TryParse(Request.Params("RotaFrete"), out rotaFrete);
			int.TryParse(Request.Params("ModeloDocumentoFiscal"), out modeloDocumento);
			int.TryParse(Request.Params("TipoOcorrencia"), out tipoOcorrencia);
			int categoriaDestinatario = Request.GetIntParam("CategoriaDestinatario");
			int categoriaRemetente = Request.GetIntParam("CategoriaRemetente");
			int categoriaTomador = Request.GetIntParam("CategoriaTomador");

			bool ativo;
			bool.TryParse(Request.Params("Ativo"), out ativo);
			int codigoCanalEntrega = Request.GetIntParam("CanalEntrega");
			int codigoCanalVenda = Request.GetIntParam("CanalVenda");
			int codigoTipoDT = Request.GetIntParam("TipoDT");

			configuracaoContaContabil.Ativo = ativo;

			configuracaoContaContabil.Remetente = remetente > 0 ? repCliente.BuscarPorCPFCNPJ(remetente) : null;
			configuracaoContaContabil.Tomador = tomador > 0 ? repCliente.BuscarPorCPFCNPJ(tomador) : null;
			configuracaoContaContabil.TipoOperacao = tipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(tipoOperacao) : null;
			configuracaoContaContabil.Empresa = empresa > 0 ? repEmpresa.BuscarPorCodigo(empresa) : null;
			configuracaoContaContabil.Destinatario = destinatario > 0 ? repCliente.BuscarPorCPFCNPJ(destinatario) : null;
			configuracaoContaContabil.ModeloDocumentoFiscal = modeloDocumento > 0 ? repModeloDocumentoFiscal.BuscarPorId(modeloDocumento) : null;
			configuracaoContaContabil.TipoOcorrencia = tipoOcorrencia > 0 ? repTipoDeOcorrenciaDeCTe.BuscarPorCodigo(tipoOcorrencia) : null;

			configuracaoContaContabil.CategoriaDestinatario = categoriaDestinatario > 0 ? repositorioCategoriaPessoa.BuscarPorCodigo(categoriaDestinatario) : null;
			configuracaoContaContabil.CategoriaRemetente = categoriaRemetente > 0 ? repositorioCategoriaPessoa.BuscarPorCodigo(categoriaRemetente) : null;
			configuracaoContaContabil.CategoriaTomador = categoriaTomador > 0 ? repositorioCategoriaPessoa.BuscarPorCodigo(categoriaTomador) : null;
			configuracaoContaContabil.GrupoDestinatario = repGrupoPessoas.BuscarPorCodigo(grupoDestinatario);
			configuracaoContaContabil.GrupoRemetente = repGrupoPessoas.BuscarPorCodigo(grupoRemetente);
			configuracaoContaContabil.GrupoTomador = repGrupoPessoas.BuscarPorCodigo(grupoTomador);
			configuracaoContaContabil.GrupoProduto = repGrupoProduto.BuscarPorCodigo(grupoProduto);
			configuracaoContaContabil.RotaFrete = repRotaFrete.BuscarPorCodigo(rotaFrete);
			configuracaoContaContabil.CanalEntrega = repositorioCanalEntrega.BuscarPorCodigo(codigoCanalEntrega);
			configuracaoContaContabil.CanalVenda = repositorioCanalVenda.BuscarPorCodigo(codigoCanalVenda);
			configuracaoContaContabil.TipoDT = repositorioTipoDocumentoTransporte.BuscarPorCodigo(codigoTipoDT);

			bool possuiIVA = repositorioImpostoValorAgregado.PossuiRegistroIVA();

			Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil configuracaoContaContabilExistente = repConfiguracaoContaContabil.BuscarPorParametros(remetente, destinatario, tomador, grupoRemetente, grupoDestinatario, grupoTomador, empresa, tipoOperacao, grupoProduto, rotaFrete, categoriaRemetente, categoriaDestinatario, categoriaTomador, modeloDocumento, tipoOcorrencia, possuiIVA, codigoCanalEntrega, codigoCanalVenda, codigoTipoDT);

			if (configuracaoContaContabilExistente != null && configuracaoContaContabilExistente.Codigo != configuracaoContaContabil.Codigo)
				retorno = "Já existe uma regra cadastrada para essa configuração de Centros de Resultado";

			return retorno;
		}

		private void SalvarConfiguracoesDeContabilizacao(Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil configuracaoContaContabil, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Auditoria.HistoricoObjeto historico = null)
		{
			Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilContabilizacao repConfiguracaoContaContabilContabilizacao = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilContabilizacao(unidadeDeTrabalho);
			dynamic dynConfiguracaoContaContabilContabilizacaos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoContaContabilContabilizacaos"));

			List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilContabilizacao> contabilizacoes = repConfiguracaoContaContabilContabilizacao.BuscarPorConfiguracaoContabil(configuracaoContaContabil.Codigo);
			foreach (Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilContabilizacao contabilizacao in contabilizacoes)
				repConfiguracaoContaContabilContabilizacao.Deletar(contabilizacao, historico != null ? Auditado : null, historico);

			if (dynConfiguracaoContaContabilContabilizacaos.Count > 0)
			{
				foreach (var dynConfiguracaoContaContabilContabilizacao in dynConfiguracaoContaContabilContabilizacaos)
				{
					Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilContabilizacao contabilizacao = new Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilContabilizacao();
					contabilizacao.PlanoConta = new Dominio.Entidades.Embarcador.Financeiro.PlanoConta() { Codigo = (int)dynConfiguracaoContaContabilContabilizacao.PlanoConta.Codigo };

					if ((int)dynConfiguracaoContaContabilContabilizacao.PlanoContaContraPartidaProvisao.Codigo > 0)
						contabilizacao.PlanoContaContraPartidaProvisao = new Dominio.Entidades.Embarcador.Financeiro.PlanoConta() { Codigo = (int)dynConfiguracaoContaContabilContabilizacao.PlanoContaContraPartidaProvisao.Codigo };
					else
						contabilizacao.PlanoContaContraPartidaProvisao = null;

					contabilizacao.TipoContabilizacao = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao)dynConfiguracaoContaContabilContabilizacao.TipoContabilizacao;
					contabilizacao.TipoContaContabil = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil)dynConfiguracaoContaContabilContabilizacao.TipoContaContabil;
					contabilizacao.ComponentesDeFreteDoTipoDescontoNaoDevemSomarNaContabilizacao = (bool)dynConfiguracaoContaContabilContabilizacao.ComponentesDeFreteDoTipoDescontoNaoDevemSomarNaContabilizacao;
					contabilizacao.ConfiguracaoContaContabil = configuracaoContaContabil;
					repConfiguracaoContaContabilContabilizacao.Inserir(contabilizacao, historico != null ? Auditado : null, historico);
				}
			}
		}

		private void SalvarConfiguracoesDeEscrituracao(Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil configuracaoContaContabil, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Auditoria.HistoricoObjeto historico = null)
		{
			Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao repConfiguracaoContaContabilEscrituracao = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao(unidadeDeTrabalho);
			dynamic dynConfiguracaoContaContabilEscrituracaos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoContaContabilEscrituracaos"));

			List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao> contabilizacoes = repConfiguracaoContaContabilEscrituracao.BuscarPorConfiguracaoContabil(configuracaoContaContabil.Codigo);
			foreach (Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao Escrituracao in contabilizacoes)
				repConfiguracaoContaContabilEscrituracao.Deletar(Escrituracao, Auditado, historico);

			if (dynConfiguracaoContaContabilEscrituracaos.Count > 0)
			{
				foreach (var dynConfiguracaoContaContabilEscrituracao in dynConfiguracaoContaContabilEscrituracaos)
				{
					Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao Escrituracao = new Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao();
					Escrituracao.PlanoConta = new Dominio.Entidades.Embarcador.Financeiro.PlanoConta() { Codigo = (int)dynConfiguracaoContaContabilEscrituracao.PlanoConta.Codigo };
					Escrituracao.TipoContabilizacao = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao)dynConfiguracaoContaContabilEscrituracao.TipoContabilizacao;
					Escrituracao.TipoContaContabil = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil)dynConfiguracaoContaContabilEscrituracao.TipoContaContabil;
					Escrituracao.SempreGerarRegistro = (bool)dynConfiguracaoContaContabilEscrituracao.SempreGerarRegistro;
					Escrituracao.ConfiguracaoContaContabil = configuracaoContaContabil;
					repConfiguracaoContaContabilEscrituracao.Inserir(Escrituracao, Auditado, historico);
				}
			}
		}

		private void SalvarConfiguracoesDeProvisao(Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil configuracaoContaContabil, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Auditoria.HistoricoObjeto historico = null)
		{
			Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilProvisao repConfiguracaoContaContabilProvisao = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilProvisao(unidadeDeTrabalho);
			dynamic dynConfiguracaoContaContabilProvisaos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoContaContabilProvisoes"));

			List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilProvisao> contabilizacoes = repConfiguracaoContaContabilProvisao.BuscarPorConfiguracaoContabil(configuracaoContaContabil.Codigo);
			foreach (Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilProvisao Provisao in contabilizacoes)
				repConfiguracaoContaContabilProvisao.Deletar(Provisao, historico != null ? Auditado : null, historico);

			if (dynConfiguracaoContaContabilProvisaos.Count > 0)
			{
				foreach (var dynConfiguracaoContaContabilProvisao in dynConfiguracaoContaContabilProvisaos)
				{
					Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilProvisao provisao = new Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilProvisao();
					provisao.PlanoConta = new Dominio.Entidades.Embarcador.Financeiro.PlanoConta() { Codigo = (int)dynConfiguracaoContaContabilProvisao.PlanoConta.Codigo };
					provisao.TipoContabilizacao = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao)dynConfiguracaoContaContabilProvisao.TipoContabilizacao;
					provisao.TipoContaContabil = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil)dynConfiguracaoContaContabilProvisao.TipoContaContabil;
					provisao.ConfiguracaoContaContabil = configuracaoContaContabil;
                    provisao.ComponentesDeFreteDoTipoDescontoNaoDevemSomarNaProvisao = (bool)dynConfiguracaoContaContabilProvisao.ComponentesDeFreteDoTipoDescontoNaoDevemSomarNaProvisao;
                    repConfiguracaoContaContabilProvisao.Inserir(provisao, historico != null ? Auditado : null, historico);
				}
			}
		}

		private Dominio.ObjetosDeValor.Embarcador.Contabil.FiltroPesquisaConfiguracaoContaContabil ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
		{
			return new Dominio.ObjetosDeValor.Embarcador.Contabil.FiltroPesquisaConfiguracaoContaContabil()
			{
				Ativo = Request.GetEnumParam("Ativo", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo),
				CPFCNPJRemetente = Request.GetDoubleParam("Remetente"),
				CPFCNPJDestinatario = Request.GetDoubleParam("Destinatario"),
				CPFCNPJTomador = Request.GetDoubleParam("Tomador"),
				CodigoGrupoRemetente = Request.GetIntParam("GrupoRemetente"),
				CodigoGrupoDestinatario = Request.GetIntParam("GrupoDestinatario"),
				CodigoGrupoTomador = Request.GetIntParam("GrupoTomador"),
				CodigoEmpresa = Request.GetIntParam("Empresa"),
				CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
				CodigoCategoriaDestinatario = Request.GetIntParam("CategoriaDestinatario"),
				CodigoCategoriaRemetente = Request.GetIntParam("CategoriaRemetente"),
				CodigoCategoriaTomador = Request.GetIntParam("CategoriaTomador"),
				CodigoModeloDocumento = Request.GetIntParam("ModeloDocumentoFiscal"),
				CodigoTipoOcorrencia = Request.GetIntParam("TipoOcorrencia"),
				CodigoCanalEntrega = Request.GetIntParam("CanalEntrega"),
				CodigoCanalVenda = Request.GetIntParam("CanalVenda"),
				CodigoTipoDT = Request.GetIntParam("TipoDT"),
			};
		}

		#endregion
	}
}
