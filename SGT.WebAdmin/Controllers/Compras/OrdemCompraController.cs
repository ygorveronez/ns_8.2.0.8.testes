using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers.Transportadores;

namespace SGT.WebAdmin.Controllers.Compras
{
	[CustomAuthorize(new string[] { "PesquisaAutorizacoes", "DetalhesAutorizacao" }, "Compras/FluxoCompra", "Compras/OrdemCompra")]
	public class OrdemCompraController : BaseController
	{
		#region Construtores

		public OrdemCompraController(Conexao conexao) : base(conexao) { }

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

		public async Task<IActionResult> PesquisaAutorizacoes()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

			try
			{
				// Respositorios
				Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra repAprovacaoAlcadaOrdemCompra = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra(unitOfWork);

				int.TryParse(Request.Params("Codigo"), out int codigo);

				Models.Grid.Grid grid = new Models.Grid.Grid(Request)
				{
					header = new List<Models.Grid.Head>()
				};
				grid.AdicionarCabecalho("Codigo", false);
				grid.AdicionarCabecalho("Usuário", "Usuario", 10, Models.Grid.Align.left, false);
				grid.AdicionarCabecalho("Situação", "Situacao", 5, Models.Grid.Align.center, false);
				grid.AdicionarCabecalho("Regra", false);
				grid.AdicionarCabecalho("Data", false);
				grid.AdicionarCabecalho("Motivo", false);

				// Ordenacao
				string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

				// Busca
				List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra> listaAutorizacao = repAprovacaoAlcadaOrdemCompra.ConsultarAutorizacoesPorOrdem(codigo, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
				int totalRegistros = repAprovacaoAlcadaOrdemCompra.ContarConsultaAutorizacoesPorOrdem(codigo);

				var lista = (from obj in listaAutorizacao
							 select new
							 {
								 obj.Codigo,
								 Situacao = obj.DescricaoSituacao,
								 Usuario = obj.Usuario?.Nome,
								 Regra = obj.RegraOrdemCompra.Descricao,
								 Data = obj.Data != null ? obj.Data.ToString() : string.Empty,
								 Motivo = !string.IsNullOrWhiteSpace(obj.Motivo) ? obj.Motivo : string.Empty,
								 DT_RowColor = CorAprovacao(obj.Situacao)
							 }).ToList();

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
		public async Task<IActionResult> PesquisaMercadorias()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
			try
			{
				Repositorio.Embarcador.Compras.OrdemCompraMercadoria repMercadoria = new Repositorio.Embarcador.Compras.OrdemCompraMercadoria(unitOfWork);

				Models.Grid.Grid grid = GridPesquisaMercadoria();

				string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
				if (propOrdenar == "Numero")
					propOrdenar = "OrdemCompra.Numero";

				int ordem = Request.GetIntParam("Codigo");
				int produto = Request.GetIntParam("Produto");

				Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemCompra? situacao = null;
				if (Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemCompra situacaoAux))
					situacao = situacaoAux;

				int totalRegistros = repMercadoria.ContarConsultaPorOrdem(situacao, produto, ordem);

				List<Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria> listaGrid = repMercadoria.ContarConsultaPorOrdem(situacao, produto, ordem, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

				var lista = from obj in listaGrid
							select new
							{
								obj.Codigo,
								obj.OrdemCompra.Numero,
								Produto = obj.Produto.Descricao,
								Quantidade = obj.Quantidade.ToString("n2"),
								ValorUnitario = obj.ValorUnitario.ToString("n4"),
								ValorTotal = obj.ValorTotal.ToString("n2"),
							};

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

		public async Task<IActionResult> Imprimir()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
			try
			{
				int codigo = Request.GetIntParam("Codigo");

				Repositorio.Embarcador.Compras.OrdemCompra repOrdemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(unitOfWork);
				Dominio.Entidades.Embarcador.Compras.OrdemCompra ordemCompra = repOrdemCompra.BuscarPorCodigo(codigo);

				if (ordemCompra == null)
					return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

				Servicos.Embarcador.Compras.OrdemCompra servicoOrdemCompra = new Servicos.Embarcador.Compras.OrdemCompra(unitOfWork);

				return Arquivo(servicoOrdemCompra.GerarImpressaoOrdemCompra(ordemCompra), "application/pdf", "Ordem de Compra - " + ordemCompra.Numero + ".pdf");
			}
			catch (Exception ex)
			{
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao gerar o arquivo.");
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

		public async Task<IActionResult> ReprocessarRegras()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
			try
			{
				// Inicia transacao
				unitOfWork.Start();

				// Instancia repositorios
				Repositorio.Embarcador.Compras.OrdemCompra repOrdemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(unitOfWork);

				// Parametros
				int.TryParse(Request.Params("Codigo"), out int codigo);

				// Busca informacoes
				Dominio.Entidades.Embarcador.Compras.OrdemCompra ordemCompra = repOrdemCompra.BuscarPorCodigo(codigo);

				// Valida
				if (ordemCompra == null)
					return new JsonpResult(false, true, "Não foi possível encontrar o registro.");
				if (ordemCompra.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemCompra.SemRegra)
					return new JsonpResult(false, true, "A situação não permite essa operação.");

				int codigoEmpresa = 0;
				if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
					codigoEmpresa = this.Usuario.Empresa.Codigo;

				// Busca as regras
				Servicos.Embarcador.Compras.OrdemCompra.EtapaAprovacao(ref ordemCompra, unitOfWork, TipoServicoMultisoftware, _conexao.StringConexao, Auditado, codigoEmpresa);
				repOrdemCompra.Atualizar(ordemCompra);
				unitOfWork.CommitChanges();

				bool possuiRegra = ordemCompra.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemCompra.SemRegra;

				// Retorna sucesso
				return new JsonpResult(possuiRegra);
			}
			catch (Exception ex)
			{
				unitOfWork.Rollback();
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao buscar regras.");
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

		public async Task<IActionResult> ImportarDeRequisicoes()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

			try
			{
				Repositorio.Embarcador.Compras.Mercadoria repMercadoria = new Repositorio.Embarcador.Compras.Mercadoria(unitOfWork);
				Repositorio.Embarcador.Compras.CotacaoFornecedor repCotacaoFornecedor = new Repositorio.Embarcador.Compras.CotacaoFornecedor(unitOfWork);

				List<int> codigoRequisicoes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(Request.Params("Codigos"));

				if (codigoRequisicoes == null || codigoRequisicoes.Count <= 0)
					return new JsonpResult(false, "Nenhuma requisição de compra selecionada.");

				List<Dominio.Entidades.Embarcador.Compras.Mercadoria> mercadorias = repMercadoria.BuscarPorRequisicaoCompra(codigoRequisicoes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModoRequisicaoMercadoria.Compra);
				int codigo = 1;
				return new JsonpResult(new
				{
					Data = DateTime.Now.Date.ToString("dd/MM/yyyy"),
					ValorTotal = (from obj in mercadorias select (obj.Saldo * obj.ProdutoEstoque.Produto.UltimoCusto)).Sum().ToString("n2"),
					Produtos = (from obj in mercadorias
								select new
								{
									Codigo = codigo++,
									Quantidade = obj.Saldo.ToString("n2"),
									ValorUnitario = obj.ProdutoEstoque.Produto.UltimoCusto.ToString("n4"),
									ValorTotal = (obj.Saldo * obj.ProdutoEstoque.Produto.UltimoCusto).ToString("n2"),
									Produto = new { obj.ProdutoEstoque.Produto.Codigo, obj.ProdutoEstoque.Produto.Descricao },
									VeiculoMercadoria = new { Codigo = 0, Descricao = "" },
								}).ToList(),
				});
			}
			catch (Exception ex)
			{
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao buscar mercadorias da requisição.");
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

        public async Task<IActionResult> ImportarDeLicencas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
			string numeros = ""; 

            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Compras.OrdemCompra repOrdemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(unitOfWork);
                Repositorio.Embarcador.Transportadores.MotoristaLicenca repMotoristaLicenca = new Repositorio.Embarcador.Transportadores.MotoristaLicenca(unitOfWork); 
				Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
				Repositorio.Embarcador.Compras.MotivoCompra repMotivoCompra = new Repositorio.Embarcador.Compras.MotivoCompra(unitOfWork);

               // Busca todas as ocorrencias selecionadas
               MotoristaController motoristaController = new MotoristaController(_conexao);
				List<Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca> licencasSelecionadas = motoristaController.ObterLicencasSelecionadas(unitOfWork, Request);

                foreach (var licenca in licencasSelecionadas)
                {
                    Dominio.Entidades.Embarcador.Compras.OrdemCompra ordemCompra = new Dominio.Entidades.Embarcador.Compras.OrdemCompra();

                    // Preenche entidade com dados
                    int codigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa.Codigo : 0;
                    ordemCompra.Numero = repOrdemCompra.BuscarProximoNumero(codigoEmpresa);
                    ordemCompra.Usuario = this.Usuario;
                    ordemCompra.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemCompra.Aberta;
                    ordemCompra.Empresa = this.Usuario.Empresa;
					ordemCompra.MotivoCompra = repMotivoCompra.BuscaPossivelMotivoCompraLicenca(codigoEmpresa, licenca.Descricao ?? licenca.Licenca?.Descricao);
                    // ordemCompra.Fornecedor = ;
                    ordemCompra.Data = licenca.DataVencimento.HasValue && licenca.DataVencimento < DateTime.Today ? DateTime.Today : licenca.DataVencimento ?? DateTime.Today;
                    ordemCompra.DataPrevisaoRetorno = licenca.DataVencimento.HasValue && licenca.DataVencimento < DateTime.Today ? DateTime.Today : licenca.DataVencimento ?? DateTime.Today;
                    ordemCompra.Observacao = $"Requisição de Exames gerada automaticamente a partir da licença {licenca.Descricao} - Vencimento {licenca.DataVencimento?.ToString("dd/MM/yyyy") ?? "data desconhecida"}";
                    ordemCompra.Motorista = licenca.Motorista;

                    // Persiste dados
                    unitOfWork.Start();
                    repOrdemCompra.Inserir(ordemCompra, Auditado);

                    unitOfWork.CommitChanges();
					numeros += ordemCompra.Numero + " ";
                }
                
                // Retorna sucesso
                return new JsonpResult(new
                {
                    numeros
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DetalhesAutorizacao()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

			try
			{
				// Instancia
				Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra repAprovacaoAlcadaOrdemCompra = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra(unitOfWork);

				// Converte dados
				int codigoAutorizacao = int.Parse(Request.Params("Codigo"));

				// Busca a autorizacao
				Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra autorizacao = repAprovacaoAlcadaOrdemCompra.BuscarPorCodigo(codigoAutorizacao);

				var retorno = new
				{
					autorizacao.Codigo,
					Regra = autorizacao.Delegada ? "(Delegada)" : autorizacao.RegraOrdemCompra.Descricao,
					Situacao = autorizacao.DescricaoSituacao,
					Usuario = autorizacao.Usuario?.Nome ?? string.Empty,

					PodeAprovar = autorizacao.Usuario != null && autorizacao.Usuario.Codigo == this.Usuario.Codigo && autorizacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente,

					Data = autorizacao.Data.HasValue ? autorizacao.Data.Value.ToString("dd/MM/yyyy") : string.Empty,
					Motivo = !string.IsNullOrWhiteSpace(autorizacao.Motivo) ? autorizacao.Motivo : string.Empty,
				};

				return new JsonpResult(retorno);
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

		public async Task<IActionResult> BuscarPorCodigo()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
			try
			{
				Repositorio.Embarcador.Compras.OrdemCompra repOrdemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(unitOfWork);

				int codigo = Request.GetIntParam("Codigo");

				Dominio.Entidades.Embarcador.Compras.OrdemCompra ordemCompra = repOrdemCompra.BuscarPorCodigo(codigo);

				if (ordemCompra == null)
					return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

				var retorno = new
				{
					ordemCompra.Codigo,
					ordemCompra.Numero,
					Fornecedor = new { ordemCompra.Fornecedor?.Codigo, Descricao = ordemCompra.Fornecedor?.Nome },
					Motivo = ordemCompra.MotivoCompra != null ? new { ordemCompra.MotivoCompra.Codigo, ordemCompra.MotivoCompra.Descricao } : null,
					ordemCompra.Observacao,
					ordemCompra.CondicaoPagamento,
					Data = ordemCompra.Data.ToString("dd/MM/yyyy"),
					DataPrevistaRetorno = ordemCompra.DataPrevisaoRetorno.ToString("dd/MM/yyyy"),
					Operador = ordemCompra.Usuario?.Nome ?? string.Empty,
					Veiculo = ordemCompra.Veiculo != null ? new { ordemCompra.Veiculo.Codigo, ordemCompra.Veiculo.Descricao } : null,
					Motorista = ordemCompra.Motorista != null ? new { ordemCompra.Motorista.Codigo, Descricao = ordemCompra.Motorista.Nome } : null,
					Transportador = ordemCompra.Transportador != null ? new { ordemCompra.Transportador.Codigo, Descricao = ordemCompra.Transportador.Nome } : null,
					ordemCompra.Situacao,
					ValorTotal = ordemCompra.ValorTotal.ToString("n2"),
					ExigeInformarVeiculoObrigatoriamente = ordemCompra.MotivoCompra?.ExigeInformarVeiculoObrigatoriamente ?? false,
					BloquearEdicaoOrdemCompraPorAbastecimento = ordemCompra.BloquearEdicaoOrdemCompraPorAbastecimento,

					Resumo = ResumoAutorizacao(ordemCompra, unitOfWork),

					Produtos = (from m in ordemCompra.Mercadorias
								select new
								{
									m.Codigo,
									m.Produto?.CodigoProduto,
									Quantidade = m.Quantidade.ToString("n2"),
									ValorUnitario = m.ValorUnitario.ToString("n4"),
									ValorTotal = m.ValorTotal.ToString("n2"),
									VeiculoMercadoria = new { Codigo = m.VeiculoMercadoria?.Codigo ?? 0, Descricao = m.VeiculoMercadoria?.Placa ?? "" },
									Produto = new { m.Produto.Codigo, m.Produto.Descricao }
								}).ToList()
				};

				return new JsonpResult(retorno);
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

		public async Task<IActionResult> Salvar()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
			try
			{
				// Instancia repositorios
				Repositorio.Embarcador.Compras.OrdemCompra repOrdemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(unitOfWork);

				// Parametros
				int.TryParse(Request.Params("Codigo"), out int codigo);

				// Busca informacoes
				Dominio.Entidades.Embarcador.Compras.OrdemCompra ordemCompra = repOrdemCompra.BuscarPorCodigo(codigo);

				if (ordemCompra == null) ordemCompra = new Dominio.Entidades.Embarcador.Compras.OrdemCompra();
				else ordemCompra.Initialize();

				if (ordemCompra.Codigo > 0 && ordemCompra.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemCompra.Aberta)
					return new JsonpResult(false, true, "Não é possível alterar a ordem nessa situação.");

				// Preenche entidade com dados
				PreencheEntidade(ref ordemCompra, unitOfWork);

				// Valida entidade
				if (!ValidaEntidade(ordemCompra, out string erro))
					return new JsonpResult(false, true, erro);

				// Persiste dados
				unitOfWork.Start();
				if (ordemCompra.Codigo > 0)
				{
					repOrdemCompra.Atualizar(ordemCompra, Auditado);
				}
				else
				{
					repOrdemCompra.Inserir(ordemCompra, Auditado);
					RequisicoesVinculadas(ordemCompra, unitOfWork);
				}

				SalvarMercadorias(ordemCompra, unitOfWork, Auditado, null);
				unitOfWork.CommitChanges();

				// Retorna sucesso
				return new JsonpResult(new
				{
					ordemCompra.Codigo
				});
			}
			catch (ControllerException ex)
			{
				unitOfWork.Rollback();
				return new JsonpResult(false, true, ex.Message);
			}
			catch (Exception ex)
			{
				unitOfWork.Rollback();
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

		public async Task<IActionResult> Finalizar()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
			try
			{
				// Instancia repositorios
				Repositorio.Embarcador.Compras.OrdemCompra repOrdemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(unitOfWork);

				// Parametros
				int.TryParse(Request.Params("Codigo"), out int codigo);

				// Busca informacoes
				Dominio.Entidades.Embarcador.Compras.OrdemCompra ordemCompra = repOrdemCompra.BuscarPorCodigo(codigo);

                if (ordemCompra == null) ordemCompra = new Dominio.Entidades.Embarcador.Compras.OrdemCompra();
				else ordemCompra.Initialize();

				if (ordemCompra.Codigo > 0 && ordemCompra.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemCompra.Aberta)
					return new JsonpResult(false, true, "Não é possível alterar a ordem nessa situação.");

				// Preenche entidade com dados
				PreencheEntidade(ref ordemCompra, unitOfWork);

				// Valida entidade
				if (!ValidaEntidade(ordemCompra, out string erro))
					return new JsonpResult(false, true, erro);

				// Persiste dados
				unitOfWork.Start();
				if (ordemCompra.Codigo > 0)
					repOrdemCompra.Atualizar(ordemCompra, Auditado);
				else
				{
					repOrdemCompra.Inserir(ordemCompra, Auditado);
					RequisicoesVinculadas(ordemCompra, unitOfWork);
				}
				SalvarMercadorias(ordemCompra, unitOfWork, Auditado, null);

				int codigoEmpresa = 0;
				if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
					codigoEmpresa = this.Usuario.Empresa.Codigo;

				Servicos.Embarcador.Compras.OrdemCompra.EtapaAprovacao(ref ordemCompra, unitOfWork, TipoServicoMultisoftware, _conexao.StringConexao, Auditado, codigoEmpresa);
				unitOfWork.CommitChanges();

				// Retorna sucesso
				return new JsonpResult(new
				{
					ordemCompra.Codigo
				});
			}
			catch (ControllerException ex)
			{
				unitOfWork.Rollback();
				return new JsonpResult(false, true, ex.Message);
			}
			catch (Exception ex)
			{
				unitOfWork.Rollback();
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao finalizar.");
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
				// Instancia repositorios
				Repositorio.Embarcador.Compras.OrdemCompra repOrdemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(unitOfWork);

				// Parametros
				int.TryParse(Request.Params("Codigo"), out int codigo);

				// Busca informacoes
				Dominio.Entidades.Embarcador.Compras.OrdemCompra ordemCompra = repOrdemCompra.BuscarPorCodigo(codigo);

				// Valida
				if (ordemCompra == null)
					return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

				// Persiste dados
				unitOfWork.Start();
				repOrdemCompra.Deletar(ordemCompra, Auditado);
				unitOfWork.CommitChanges();

				// Retorna informacoes
				return new JsonpResult(true);
			}
			catch (Exception ex)
			{
				unitOfWork.Rollback();
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

		public async Task<IActionResult> Cancelar()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
			try
			{
				int codigo = Request.GetIntParam("Codigo");

				Repositorio.Embarcador.Compras.OrdemCompra repOrdemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(unitOfWork);
				Dominio.Entidades.Embarcador.Compras.OrdemCompra ordemCompra = repOrdemCompra.BuscarPorCodigo(codigo, true);

				if (ordemCompra == null)
					return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

				if (ordemCompra.Situacao != SituacaoOrdemCompra.Aprovada && ordemCompra.Situacao != SituacaoOrdemCompra.Aberta)
					return new JsonpResult(false, true, "Não é possível cancelar na situação atual.");

				unitOfWork.Start();

				ordemCompra.Situacao = SituacaoOrdemCompra.Cancelada;
				repOrdemCompra.Atualizar(ordemCompra, Auditado);

				unitOfWork.CommitChanges();

				return new JsonpResult(true);
			}
			catch (Exception ex)
			{
				unitOfWork.Rollback();
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao cancelar.");
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

		public async Task<IActionResult> Reabrir()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
			try
			{
				int codigo = Request.GetIntParam("Codigo");

				Repositorio.Embarcador.Compras.OrdemCompra repOrdemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(unitOfWork);
				Dominio.Entidades.Embarcador.Compras.OrdemCompra ordemCompra = repOrdemCompra.BuscarPorCodigo(codigo, true);

				if (ordemCompra == null)
					return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

				if (ordemCompra.Situacao != SituacaoOrdemCompra.AgAprovacao)
					return new JsonpResult(false, true, "Não é possível reabrir na situação atual.");

				unitOfWork.Start();

				ordemCompra.Situacao = SituacaoOrdemCompra.Aberta;
				repOrdemCompra.Atualizar(ordemCompra, Auditado);

				unitOfWork.CommitChanges();

				return new JsonpResult(true);
			}
			catch (Exception ex)
			{
				unitOfWork.Rollback();
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao reabrir.");
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

		public async Task<IActionResult> ReenviarEmail()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
			try
			{
				int codigo = Request.GetIntParam("Codigo");

				Repositorio.Embarcador.Compras.OrdemCompra repOrdemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(unitOfWork);
				Dominio.Entidades.Embarcador.Compras.OrdemCompra ordemCompra = repOrdemCompra.BuscarPorCodigo(codigo);

				if (ordemCompra == null)
					return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

				if (ordemCompra.Situacao != SituacaoOrdemCompra.Aprovada)
					return new JsonpResult(false, true, "Só é possível reenviar e-mail com a Situação Aprovada.");

				Servicos.Embarcador.Compras.OrdemCompra servicoOrdemCompra = new Servicos.Embarcador.Compras.OrdemCompra(unitOfWork);
				servicoOrdemCompra.EnviarEmailOrdemCompra(ordemCompra, TipoServicoMultisoftware);

				return new JsonpResult(true);
			}
			catch (ServicoException ex)
			{
				return new JsonpResult(false, true, ex.Message);
			}
			catch (Exception ex)
			{
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao reenviar o email.");
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

		public async Task<IActionResult> LoteOrdemCompra()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

			try
			{
				Repositorio.Embarcador.Compras.OrdemCompra repositorioOrdemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(unitOfWork);

				Dominio.Entidades.Embarcador.Compras.OrdemCompra ordemCompra = new Dominio.Entidades.Embarcador.Compras.OrdemCompra();

				PreencheEntidade(ref ordemCompra, unitOfWork);

				int repeticao = Request.GetIntParam("Repeticao");
				int numeroOrdens = Request.GetIntParam("NumeroOrdens");
				int diaOrdem = Request.GetIntParam("DiaOrdem");

				if (!ValidaEntidade(ordemCompra, out string erro))
					return new JsonpResult(false, true, erro);

				unitOfWork.Start();

				repositorioOrdemCompra.Inserir(ordemCompra, Auditado);

				List<Dominio.Entidades.Embarcador.Compras.OrdemCompra> ordensCompra = new List<Dominio.Entidades.Embarcador.Compras.OrdemCompra>()
				{
					ordemCompra
				};

				Dominio.Entidades.Embarcador.Compras.OrdemCompra novaOrdemCompra = new Dominio.Entidades.Embarcador.Compras.OrdemCompra();
				novaOrdemCompra = ordemCompra;

				int diferencaDataPrevisaoRetorno = (ordemCompra.DataPrevisaoRetorno.Date - ordemCompra.Data.Date).Days;
				for (int i = 0; i < numeroOrdens - 1; i++)
				{
					novaOrdemCompra = novaOrdemCompra.Clonar<Dominio.Entidades.Embarcador.Compras.OrdemCompra>();
					novaOrdemCompra.Codigo = 0;
					novaOrdemCompra.Numero = repositorioOrdemCompra.BuscarProximoNumero(novaOrdemCompra.Empresa?.Codigo ?? 0);

                    novaOrdemCompra.Data = DefinirProximaDataOrdemCompra(novaOrdemCompra.Data, diaOrdem, repeticao);
					novaOrdemCompra.DataPrevisaoRetorno = novaOrdemCompra.Data.AddDays(diferencaDataPrevisaoRetorno);

					repositorioOrdemCompra.Inserir(novaOrdemCompra, Auditado);
					ordensCompra.Add(novaOrdemCompra);
				}

				foreach(var ordem in ordensCompra)
				{
					SalvarMercadorias(ordem, unitOfWork, Auditado, null);
				}

				unitOfWork.CommitChanges();

				return new JsonpResult(true);
			}
			catch (Exception ex)
			{
				unitOfWork.Rollback();
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao gerar lote de ordem de compra.");
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

        [AllowAuthenticate]
        public async Task<IActionResult> FinalizarPorRequisicao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Compras.OrdemCompra repOrdemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(unitOfWork);
                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
                

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Compras.OrdemCompra ordemCompra = repOrdemCompra.BuscarPorCodigo(codigo);

                if (ordemCompra.Codigo > 0 && ordemCompra.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemCompra.Aberta)
                    return new JsonpResult(false, true, "Não é possível alterar a ordem nessa situação.");

                unitOfWork.Start();

                repOrdemCompra.Atualizar(ordemCompra, Auditado);

                ordemCompra.Situacao = SituacaoOrdemCompra.Finalizada;
                repOrdemCompra.Atualizar(ordemCompra);

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    ordemCompra.Codigo
                });
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao finalizar.");
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
			Models.Grid.Grid grid = new Models.Grid.Grid(Request)
			{
				header = new List<Models.Grid.Head>()
			};

			grid.Prop("Codigo");
			grid.Prop("Numero").Nome("Número").Tamanho(7).Align(Models.Grid.Align.right);
			grid.Prop("Fornecedor").Nome("Fornecedor").Tamanho(20).Align(Models.Grid.Align.left);
			grid.Prop("Data").Nome("Data").Tamanho(10).Align(Models.Grid.Align.center);
			grid.Prop("DataPrevisaoRetorno").Nome("Data Prev").Tamanho(10).Align(Models.Grid.Align.center);
			grid.Prop("Situacao").Nome("Situação").Tamanho(10).Align(Models.Grid.Align.center).Ord(false);
			grid.Prop("ValorTotal").Nome("Valor Total").Tamanho(10).Align(Models.Grid.Align.right).Ord(false);
			grid.Prop("NumeroCotacao").Nome("Nº Cotação").Tamanho(10).Align(Models.Grid.Align.right).Ord(false);
			grid.Prop("NumeroRequisicao").Nome("Nº Requisição").Tamanho(10).Align(Models.Grid.Align.right).Ord(false);
			grid.Prop("CodigoVeiculo");
			grid.Prop("Veiculo");
			grid.Prop("CodigoSituacao");
			grid.Prop("Descricao");

			return grid;
		}

		private Models.Grid.Grid GridPesquisaMercadoria()
		{
			Models.Grid.Grid grid = new Models.Grid.Grid(Request)
			{
				header = new List<Models.Grid.Head>()
			};

			grid.Prop("Codigo");
			grid.Prop("Numero");
			grid.Prop("Produto").Nome("Produto").Tamanho(35).Align(Models.Grid.Align.left);
			grid.Prop("Quantidade").Nome("Quantidade").Tamanho(10).Align(Models.Grid.Align.right);
			grid.Prop("ValorUnitario").Nome("Valor Unitário").Tamanho(12).Align(Models.Grid.Align.right);
			grid.Prop("ValorTotal").Nome("Valor Total").Tamanho(17).Align(Models.Grid.Align.right).Ord(false);

			return grid;
		}

		private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
		{
			Repositorio.Embarcador.Compras.OrdemCompra repOrdemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(unitOfWork);

			Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaOrdemCompra filtrosPesquisa = ObterFiltroPesquisa(unitOfWork);

			List<Dominio.Entidades.Embarcador.Compras.OrdemCompra> listaGrid = repOrdemCompra.Consultar(filtrosPesquisa, propOrdenar, dirOrdena, inicio, limite);
			totalRegistros = repOrdemCompra.ContarConsulta(filtrosPesquisa);

			var lista = from obj in listaGrid
						select new
						{
							obj.Codigo,
							obj.Numero,
							Fornecedor = obj.Fornecedor?.Nome,
							Data = obj.Data.ToString("dd/MM/yyyy"),
							DataPrevisaoRetorno = obj.DataPrevisaoRetorno.ToString("dd/MM/yyyy"),
							Situacao = obj.DescricaoSituacao,
							ValorTotal = obj.ValorTotal.ToString("n2"),
							CodigoVeiculo = obj.Veiculo?.Codigo ?? 0,
							Veiculo = obj.Veiculo?.Placa ?? string.Empty,
							CodigoSituacao = obj.Situacao,
							obj.Descricao,
							NumeroCotacao = obj.CotacaoCompra?.Numero ?? 0,
							NumeroRequisicao = obj.Requisicoes.Count() > 0 ? obj.Requisicoes?.FirstOrDefault().Requisicao?.Numero ?? 0 : 0
						};

			return lista.ToList();
		}

		private void PropOrdena(ref string propOrdenar)
		{
			if (propOrdenar == "Fornecedor") propOrdenar = "Fornecedor.Nome";
		}

		private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Compras.OrdemCompra ordemCompra, Repositorio.UnitOfWork unitOfWork)
		{
			// Instancia Repositorios
			Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
			Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
			Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);

			Repositorio.Embarcador.Compras.OrdemCompra repOrdemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(unitOfWork);
			Repositorio.Embarcador.Compras.MotivoCompra repMotivoCompra = new Repositorio.Embarcador.Compras.MotivoCompra(unitOfWork);

			// Converte valores
			double.TryParse(Request.Params("Fornecedor"), out double fornecedor);
			double.TryParse(Request.Params("Transportador"), out double transportador);

			DateTime.TryParse(Request.Params("Data"), out DateTime data);
			DateTime.TryParse(Request.Params("DataPrevistaRetorno"), out DateTime dataPrevisaoRetorno);

			int.TryParse(Request.Params("Motivo"), out int motivo);
			int.TryParse(Request.Params("Veiculo"), out int veiculo);
			int.TryParse(Request.Params("Motorista"), out int motorista);

			// Vincula dados
			ordemCompra.Fornecedor = fornecedor > 0 ? repCliente.BuscarPorCPFCNPJ(fornecedor) : null;
			ordemCompra.Data = data;
			ordemCompra.DataPrevisaoRetorno = dataPrevisaoRetorno;
			ordemCompra.MotivoCompra = motivo > 0 ? repMotivoCompra.BuscarPorCodigo(motivo) : null;
			ordemCompra.Transportador = transportador > 0 ? repCliente.BuscarPorCPFCNPJ(transportador) : null;
			ordemCompra.Observacao = Request.Params("Observacao");
			ordemCompra.CondicaoPagamento = Request.Params("CondicaoPagamento");

			if (ordemCompra.MotivoCompra.ExigeInformarVeiculoObrigatoriamente && veiculo <= 0)
				throw new ControllerException("É necessário informar um veículo.");

			ordemCompra.Veiculo = veiculo > 0 ? repVeiculo.BuscarPorCodigo(veiculo) : null;
			ordemCompra.Motorista = motorista > 0 ? repMotorista.BuscarMotoristaPorCodigo(motorista) : null;

			if (ordemCompra.Codigo == 0)
			{
				int codigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa.Codigo : 0;
				ordemCompra.Numero = repOrdemCompra.BuscarProximoNumero(codigoEmpresa);
				ordemCompra.Usuario = this.Usuario;
				ordemCompra.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemCompra.Aberta;
				ordemCompra.Empresa = this.Usuario.Empresa;
			}
		}

		private void RequisicoesVinculadas(Dominio.Entidades.Embarcador.Compras.OrdemCompra ordemCompra, Repositorio.UnitOfWork unitOfWork)
		{
			Repositorio.Embarcador.Compras.RequisicaoMercadoria repRequisicaoMercadoria = new Repositorio.Embarcador.Compras.RequisicaoMercadoria(unitOfWork);
			Repositorio.Embarcador.Compras.OrdemCompraRequisicao repOrdemCompraRequisicao = new Repositorio.Embarcador.Compras.OrdemCompraRequisicao(unitOfWork);

			List<int> codigoRequisicoes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(Request.Params("Requisicoes"));
			if (codigoRequisicoes != null && codigoRequisicoes.Count > 0)
			{
				List<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria> requisicoes = repRequisicaoMercadoria.BuscarPorCodigos(codigoRequisicoes);
				foreach (var requisicao in requisicoes)
				{
					Dominio.Entidades.Embarcador.Compras.OrdemCompraRequisicao vinculo = new Dominio.Entidades.Embarcador.Compras.OrdemCompraRequisicao()
					{
						OrdemCompra = ordemCompra,
						Requisicao = requisicao,
					};
					repOrdemCompraRequisicao.Inserir(vinculo);
				}
			}
		}

		private bool ValidaEntidade(Dominio.Entidades.Embarcador.Compras.OrdemCompra ordemCompra, out string msgErro)
		{
			msgErro = "";

			if (ordemCompra.Fornecedor == null)
			{
				msgErro = "Fornecedor é obrigatório.";
				return false;
			}

			if (ordemCompra.Data == DateTime.MinValue)
			{
				msgErro = "Data é obrigatório.";
				return false;
			}

			if (ordemCompra.DataPrevisaoRetorno == DateTime.MinValue)
			{
				msgErro = "Data Previsão é obrigatório.";
				return false;
			}

			return true;
		}

		private void SalvarMercadorias(Dominio.Entidades.Embarcador.Compras.OrdemCompra ordemCompra, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null, Dominio.Entidades.Auditoria.HistoricoObjeto historicoPai = null)
		{
			Repositorio.Embarcador.Compras.OrdemCompraMercadoria repOrdemCompraMercadoria = new Repositorio.Embarcador.Compras.OrdemCompraMercadoria(unitOfWork);
			Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
			Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

			List<dynamic> mercadoriasOrdem = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.Params("Produtos"));
			if (mercadoriasOrdem == null) return;

			List<int> codigosMercadorias = new List<int>();
			foreach (dynamic codigo in mercadoriasOrdem)
			{
				int.TryParse((string)codigo.Codigo, out int intcodigo);
				codigosMercadorias.Add(intcodigo);
			}
			codigosMercadorias = codigosMercadorias.Where(o => o > 0).Distinct().ToList();

			List<int> codigosExcluir = repOrdemCompraMercadoria.BuscarNaoPesentesNaLista(ordemCompra.Codigo, codigosMercadorias);

			foreach (dynamic dynMercadoria in mercadoriasOrdem)
			{
				int.TryParse((string)dynMercadoria.Codigo, out int codigo);
				Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria mercadoria = repOrdemCompraMercadoria.BuscarPorOrdemEMercadoria(ordemCompra.Codigo, codigo);

				if (mercadoria == null) mercadoria = new Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria();
				else mercadoria.Initialize();

				decimal quantidade = Utilidades.Decimal.Converter((string)dynMercadoria.Quantidade);
				decimal valorUnitario = Utilidades.Decimal.Converter((string)dynMercadoria.ValorUnitario);

				int codigoVeiculo = ((string)dynMercadoria.VeiculoMercadoria.Codigo).ToInt();

				mercadoria.OrdemCompra = ordemCompra;
				mercadoria.Produto = repProduto.BuscarPorCodigo((int)dynMercadoria.Produto.Codigo);
				mercadoria.VeiculoMercadoria = codigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(codigoVeiculo) : null;
				mercadoria.Quantidade = quantidade;
				mercadoria.ValorUnitario = valorUnitario;

				if (mercadoria.Produto != null)
				{
					if (mercadoria.Codigo == 0)
						repOrdemCompraMercadoria.Inserir(mercadoria, auditado, historicoPai);
					else
						repOrdemCompraMercadoria.Atualizar(mercadoria, auditado, historicoPai);
				}
			}

			foreach (int excluir in codigosExcluir)
			{
				Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria objParExcluir = repOrdemCompraMercadoria.BuscarPorOrdemEMercadoria(ordemCompra.Codigo, excluir);
				if (objParExcluir != null) repOrdemCompraMercadoria.Deletar(objParExcluir, auditado, historicoPai);
			}
		}

		private string CorAprovacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra situacao)
		{
			if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada)
				return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Success;

			if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada)
				return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Danger;

			if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente)
				return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Warning;

			return "";
		}

		private dynamic ResumoAutorizacao(Dominio.Entidades.Embarcador.Compras.OrdemCompra ordem, Repositorio.UnitOfWork unitOfWork)
		{
			Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra repAprovacaoAlcadaOrdemCompra = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra(unitOfWork);

			int aprovacoesNecessarias = repAprovacaoAlcadaOrdemCompra.ContarAprovacoesNecessarias(ordem.Codigo);
			int aprovacoes = repAprovacaoAlcadaOrdemCompra.ContarAprovacoes(ordem.Codigo);
			int reprovacoes = repAprovacaoAlcadaOrdemCompra.ContarReprovacoes(ordem.Codigo);

			return new
			{
				Solicitante = ordem.Usuario?.Nome ?? string.Empty,
				DataSolicitacao = ordem.Data.ToString("dd/MM/yyyy") ?? string.Empty,
				AprovacoesNecessarias = aprovacoesNecessarias,
				Aprovacoes = aprovacoes,
				Reprovacoes = reprovacoes,
				Situacao = ordem.DescricaoSituacao,
			};
		}

		private Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaOrdemCompra ObterFiltroPesquisa(Repositorio.UnitOfWork unitOfWork)
		{

			Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaOrdemCompra filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaOrdemCompra
			{
				Numero = Request.GetIntParam("Numero"),
				Operador = Request.GetIntParam("Operador"),
				Produto = Request.GetIntParam("Produto"),
				Fornecedor = Request.GetDoubleParam("Fornecedor"),
				Transportador = Request.GetDoubleParam("Transportador"),
				DataGeracaoInicio = Request.GetDateTimeParam("DataGeracaoInicio"),
				DataGeracaoFim = Request.GetDateTimeParam("DataGeracaoFim"),
				DataRetornoInicio = Request.GetDateTimeParam("DataRetornoInicio"),
				DataRetornoFim = Request.GetDateTimeParam("DataRetornoFim"),
				Situacao = Request.GetNullableEnumParam<SituacaoOrdemCompra>("Situacao"),
				Veiculo = Request.GetIntParam("Veiculo"),
				NumeroCotacao = Request.GetIntParam("NumeroCotacao"),
				NumeroRequisicao = Request.GetIntParam("NumeroRequisicao"),
			};

			if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
				filtrosPesquisa.CodigoEmpresa = this.Usuario.Empresa.Codigo;

			return filtrosPesquisa;
		}

		private DateTime DefinirProximaDataOrdemCompra(DateTime dataAnterior, int diaDoMes, int tipoRepeticao)
		{
			if (tipoRepeticao == 1)
				return dataAnterior.AddDays(7);

			var data = tipoRepeticao == 2 ? dataAnterior.AddMonths(1) : dataAnterior.AddYears(1);
			int lastDayOfMonth = data.LastDayOfMonth().Day;

			// Se o dia do mês escolhido para a ordem de compra for menor que o último dia do mês, então o define como o dia da data, caso contrário é definido o último dia do mês
			if (diaDoMes < lastDayOfMonth)
				data = new DateTime(data.Year, data.Month, diaDoMes);
			else
				data = new DateTime(data.Year, data.Month, lastDayOfMonth);

			return data;
		}

        #endregion
    }
}
