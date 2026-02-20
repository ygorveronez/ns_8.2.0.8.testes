using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using Utilidades.Extensions;

namespace SGT.WebAdmin.Controllers.Compras
{
    [CustomAuthorize(new string[] { "BuscarUltimosFornecedores", "EnviarPorEmail" }, "Compras/CotacaoCompra")]
    public class CotacaoCompraController : BaseController
    {
		#region Construtores

		public CotacaoCompraController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Compras.Cotacao repCotacao = new Repositorio.Embarcador.Compras.Cotacao(unitOfWork);

                int numero, codigoProduto = 0;
                int.TryParse(Request.Params("Numero"), out numero);
                int.TryParse(Request.Params("Produto"), out codigoProduto);

                DateTime dataEmissaoDe, dataEmissaoAte;
                DateTime.TryParse(Request.Params("DataEmissaoDe"), out dataEmissaoDe);
                DateTime.TryParse(Request.Params("DataEmissaoAte"), out dataEmissaoAte);

                DateTime dataRetornoDe, dataRetornoAte;
                DateTime.TryParse(Request.Params("DataRetornoDe"), out dataRetornoDe);
                DateTime.TryParse(Request.Params("DataRetornoAte"), out dataRetornoAte);

                double cnpjcpfFornecedor = 0;
                double.TryParse(Request.Params("Fornecedor"), out cnpjcpfFornecedor);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCotacao situacao;
                Enum.TryParse(Request.Params("Situacao"), out situacao);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Situacao", false);
                grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Descrição", "Descricao", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Emissão", "DataEmissao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Prev. Retorno", "DataPrevisao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Valor Total", "ValorTotal", 10, Models.Grid.Align.right, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Entidades.Embarcador.Compras.CotacaoCompra> listaCotacao = repCotacao.Consultar(codigoEmpresa, numero, dataEmissaoDe, dataEmissaoAte, cnpjcpfFornecedor, dataRetornoDe, dataRetornoAte, codigoProduto, situacao, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repCotacao.ContarConsulta(codigoEmpresa, numero, dataEmissaoDe, dataEmissaoAte, cnpjcpfFornecedor, dataRetornoDe, dataRetornoAte, codigoProduto, situacao));
                var lista = (from p in listaCotacao
                             select new
                             {
                                 p.Codigo,
                                 p.Situacao,
                                 p.Numero,
                                 p.Descricao,
                                 DataEmissao = p.DataEmissao.Value.ToString("dd/MM/yyyy"),
                                 DataPrevisao = p.DataPrevisao.Value.ToString("dd/MM/yyyy"),
                                 p.DescricaoSituacao,
                                 ValorTotal = p.ValorTotal.ToString("n2")
                             }).ToList();
                grid.AdicionaRows(lista);
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

        public async Task<IActionResult> Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Embarcador.Compras.CotacaoCompra serCotacaoCompra = new Servicos.Embarcador.Compras.CotacaoCompra(unitOfWork);
                Repositorio.Embarcador.Compras.Cotacao repCotacao = new Repositorio.Embarcador.Compras.Cotacao(unitOfWork);
                Repositorio.Embarcador.Compras.RequisicaoMercadoria repRequisicaoMercadoria = new Repositorio.Embarcador.Compras.RequisicaoMercadoria(unitOfWork);
                Repositorio.Embarcador.Compras.CotacaoCompraRequisicaoMercadoria repCotacaoReq = new Repositorio.Embarcador.Compras.CotacaoCompraRequisicaoMercadoria(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Compras.CotacaoCompra cotacao;
                if (codigo == 0)
                {
                    cotacao = new Dominio.Entidades.Embarcador.Compras.CotacaoCompra();

                    int codigoEmpresa = 0;
                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                        codigoEmpresa = this.Usuario.Empresa.Codigo;

                    cotacao.Numero = repCotacao.ProximoNumero(codigoEmpresa);
                    cotacao.Empresa = this.Usuario.Empresa;
                }
                else
                    cotacao = repCotacao.BuscarPorCodigo(codigo, true);

                DateTime dataEmissao, dataPrevisao;
                DateTime.TryParse(Request.Params("DataEmissao"), out dataEmissao);
                DateTime.TryParse(Request.Params("DataPrevisao"), out dataPrevisao);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCotacao situacao;
                Enum.TryParse(Request.Params("Situacao"), out situacao);

                cotacao.Descricao = Request.Params("Descricao");
                cotacao.DataEmissao = dataEmissao;
                cotacao.DataPrevisao = dataPrevisao;
                cotacao.Situacao = situacao;
                cotacao.Usuario = this.Usuario;

                if (!IsPermitirFinalizarCotacaoCompra(cotacao))
                    return new JsonpResult(false, true, "Para finalizar a cotação de compra é necessário informar um ou mais fornecedores.");

                unitOfWork.Start();

                if (codigo == 0)
                    repCotacao.Inserir(cotacao);
                else
                    repCotacao.Atualizar(cotacao, Auditado);

                SalvarListaMercadoria(cotacao, unitOfWork);
                SalvarListaFornecedor(cotacao, unitOfWork);
                SalvarListaRetornoProdutoFornecedor(cotacao, unitOfWork);

                List<int> codigoRequisicoes = new List<int>();
                if (!string.IsNullOrWhiteSpace(Request.Params("CodigoRequisicaoCompra")))
                    codigoRequisicoes = RetornaCodigoRequisicaoCompra(unitOfWork, Request.Params("CodigoRequisicaoCompra"));
                for (int i = 0; i < codigoRequisicoes.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Compras.CotacaoCompraRequisicaoMercadoria cotacaoReq = repCotacaoReq.BuscarPorCotacao(cotacao.Codigo, codigoRequisicoes[i]);
                    if (cotacaoReq == null)
                    {
                        cotacaoReq = new Dominio.Entidades.Embarcador.Compras.CotacaoCompraRequisicaoMercadoria();
                        cotacaoReq.Cotacao = cotacao;
                        cotacaoReq.RequisicaoMercadoria = repRequisicaoMercadoria.BuscarPorCodigo(codigoRequisicoes[i]);

                        repCotacaoReq.Inserir(cotacaoReq);
                    }
                }

                if (cotacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCotacao.Finalizado)
                {
                    unitOfWork.CommitChanges();

                    unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
                    unitOfWork.Start();
                    repCotacao = new Repositorio.Embarcador.Compras.Cotacao(unitOfWork);
                    cotacao = repCotacao.BuscarPorCodigo(cotacao.Codigo);

                    if (!serCotacaoCompra.GerarOrdemCompra(cotacao, unitOfWork, TipoServicoMultisoftware, _conexao.StringConexao, Auditado, true))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, "Ocorreu uma falha ao gerar ordem de compra.");
                    }
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar.");
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
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Compras.Cotacao repCotacao = new Repositorio.Embarcador.Compras.Cotacao(unitOfWork);
                Repositorio.Embarcador.Compras.CotacaoRetornoFornecedorProduto repRetorno = new Repositorio.Embarcador.Compras.CotacaoRetornoFornecedorProduto(unitOfWork);

                Dominio.Entidades.Embarcador.Compras.CotacaoCompra cotacao = repCotacao.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto> retornos = repRetorno.BuscarPorCotacao(codigo);
                object retorno = null;

                if (cotacao != null)
                {
                    retorno = new
                    {
                        cotacao.Codigo,
                        cotacao.Numero,
                        cotacao.Descricao,
                        DataEmissao = cotacao.DataEmissao.Value.ToString("dd/MM/yyyy"),
                        DataPrevisao = cotacao.DataPrevisao.Value.ToString("dd/MM/yyyy"),
                        cotacao.Situacao,
                        ValorTotal = cotacao.ValorTotal.ToString("n2"),
                        ListaMercadoria = (from obj in cotacao.Produtos
                                           select new
                                           {
                                               Codigo = obj.Codigo,
                                               CodigoCotacao = obj.Cotacao.Codigo,
                                               CodigoProduto = obj.Produto.Codigo,
                                               Descricao = obj.Produto.Descricao,
                                               Quantidade = obj.Quantidade.ToString("n2"),
                                               ValorUnitario = obj.ValorUnitario.ToString("n4"),
                                               ValorTotal = obj.ValorTotal.ToString("n2")
                                           }).ToList(),
                        ListaFornecedor = (from obj in cotacao.Fornecedores
                                           select new
                                           {
                                               Codigo = obj.Codigo,
                                               CodigoCotacao = obj.Cotacao.Codigo,
                                               CodigoFornecedor = obj.Fornecedor.Codigo,
                                               Fornecedor = obj.Fornecedor.Nome
                                           }).ToList(),
                        ListaRetornoProdutoFornecedor = (from obj in retornos
                                                         select new
                                                         {
                                                             Codigo = obj.Codigo,
                                                             CodigoCotacao = codigo,
                                                             CodigoMercadoria = obj.CotacaoProduto.Codigo,
                                                             CodigoProduto = obj.CotacaoProduto.Produto.Codigo,
                                                             CodigoCondicaoPagamento = obj.CondicaoPagamento != null ? obj.CondicaoPagamento.Codigo : 0,
                                                             CodigoFornecedor = obj.CotacaoFornecedor.Fornecedor.CPF_CNPJ,
                                                             obj.Observacao,
                                                             CondicaoPagamento = obj.CondicaoPagamento != null ? obj.CondicaoPagamento.Descricao : string.Empty,
                                                             Fornecedor = obj.CotacaoFornecedor.Fornecedor.Nome,
                                                             Produto = obj.CotacaoProduto.Produto.Descricao,
                                                             Quantidade = obj.CotacaoProduto.Quantidade.ToString("n2"),
                                                             QuantidadeOriginal = obj.CotacaoProduto.Quantidade.ToString("n2"),
                                                             QuantidadeRetorno = obj.QuantidadeRetorno.ToString("n2"),
                                                             ValorUnitario = obj.CotacaoProduto.ValorUnitario.ToString("n4"),
                                                             ValorUnitarioOriginal = obj.CotacaoProduto.ValorUnitario.ToString("n4"),
                                                             ValorUnitarioRetorno = obj.ValorUnitarioRetorno.ToString("n4"),
                                                             ValorTotal = obj.CotacaoProduto.ValorTotal.ToString("n2"),
                                                             ValorTotalItemOriginal = obj.CotacaoProduto.ValorTotal.ToString("n2"),
                                                             ValorTotalRetorno = obj.ValorTotalRetorno.ToString("n2"),
                                                             GerarOrdemCompra = obj.GerarOrdemCompra == true ? "Sim" : "Não",
                                                             BollGerarOrdemCompra = obj.GerarOrdemCompra,
                                                             Marca = !string.IsNullOrWhiteSpace(obj.Marca) ? obj.Marca : string.Empty
                                                         }).ToList()
                    };
                }
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar cotação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DuplicarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Compras.Cotacao repCotacao = new Repositorio.Embarcador.Compras.Cotacao(unitOfWork);
                Repositorio.Embarcador.Compras.CotacaoRetornoFornecedorProduto repRetorno = new Repositorio.Embarcador.Compras.CotacaoRetornoFornecedorProduto(unitOfWork);

                Dominio.Entidades.Embarcador.Compras.CotacaoCompra cotacao = repCotacao.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto> retornos = repRetorno.BuscarPorCotacao(codigo);
                object retorno = null;
                Guid g = Guid.NewGuid();
                if (cotacao != null)
                {
                    retorno = new
                    {
                        Codigo = g.ToString(),
                        Numero = "",
                        cotacao.Descricao,
                        DataEmissao = DateTime.Now.Date.ToString("dd/MM/yyyy"),
                        DataPrevisao = "",
                        Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCotacao.Aberto,
                        ValorTotal = cotacao.ValorTotal.ToString("n2"),
                        ListaMercadoria = (from obj in cotacao.Produtos
                                           select new
                                           {
                                               Codigo = g.ToString(),
                                               CodigoCotacao = g.ToString(),
                                               CodigoProduto = obj.Produto.Codigo,
                                               Descricao = obj.Produto.Descricao,
                                               Quantidade = obj.Quantidade.ToString("n2"),
                                               ValorUnitario = obj.ValorUnitario.ToString("n4"),
                                               ValorTotal = obj.ValorTotal.ToString("n2")
                                           }).ToList(),
                        ListaFornecedor = (from obj in cotacao.Fornecedores
                                           select new
                                           {
                                               Codigo = g.ToString(),
                                               CodigoCotacao = g.ToString(),
                                               CodigoFornecedor = obj.Fornecedor.CPF_CNPJ,
                                               Fornecedor = obj.Fornecedor.Nome
                                           }).ToList(),
                        ListaRetornoProdutoFornecedor = (from obj in retornos
                                                         select new
                                                         {
                                                             Codigo = g.ToString(),
                                                             CodigoCotacao = g.ToString(),
                                                             CodigoMercadoria = g.ToString(),
                                                             CodigoProduto = obj.CotacaoProduto.Produto.Codigo,
                                                             CodigoCondicaoPagamento = obj.CondicaoPagamento != null ? obj.CondicaoPagamento.Codigo : 0,
                                                             CodigoFornecedor = obj.CotacaoFornecedor.Fornecedor.CPF_CNPJ,
                                                             obj.Observacao,
                                                             CondicaoPagamento = obj.CondicaoPagamento != null ? obj.CondicaoPagamento.Descricao : string.Empty,
                                                             Fornecedor = obj.CotacaoFornecedor.Fornecedor.Nome,
                                                             Produto = obj.CotacaoProduto.Produto.Descricao,
                                                             Quantidade = obj.CotacaoProduto.Quantidade.ToString("n2"),
                                                             QuantidadeOriginal = obj.CotacaoProduto.Quantidade.ToString("n2"),
                                                             QuantidadeRetorno = obj.QuantidadeRetorno.ToString("n2"),
                                                             ValorUnitario = obj.CotacaoProduto.ValorUnitario.ToString("n4"),
                                                             ValorUnitarioOriginal = obj.CotacaoProduto.ValorUnitario.ToString("n4"),
                                                             ValorUnitarioRetorno = obj.ValorUnitarioRetorno.ToString("n4"),
                                                             ValorTotal = obj.CotacaoProduto.ValorTotal.ToString("n2"),
                                                             ValorTotalItemOriginal = obj.CotacaoProduto.ValorTotal.ToString("n2"),
                                                             ValorTotalRetorno = obj.ValorTotalRetorno.ToString("n2"),
                                                             GerarOrdemCompra = obj.GerarOrdemCompra == true ? "Sim" : "Não",
                                                             BollGerarOrdemCompra = obj.GerarOrdemCompra,
                                                             Marca = !string.IsNullOrWhiteSpace(obj.Marca) ? obj.Marca : string.Empty
                                                         }).ToList()
                    };
                }
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao duplicar cotação.");
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
                List<int> codigoRequisicoes = new List<int>();
                if (!string.IsNullOrWhiteSpace(Request.Params("Codigo")))
                    codigoRequisicoes = RetornaCodigoRequisicaoCompra(unitOfWork, Request.Params("Codigo"));

                if (codigoRequisicoes == null || codigoRequisicoes.Count <= 0)
                    return new JsonpResult(false, "Nenhuma requisição de compra selecionada.");

                Repositorio.Embarcador.Compras.Mercadoria repMercadoria = new Repositorio.Embarcador.Compras.Mercadoria(unitOfWork);
                Repositorio.Embarcador.Compras.CotacaoFornecedor repCotacaoFornecedor = new Repositorio.Embarcador.Compras.CotacaoFornecedor(unitOfWork);

                List<Dominio.Entidades.Embarcador.Compras.Mercadoria> mercadorias = repMercadoria.BuscarPorRequisicaoCompra(codigoRequisicoes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModoRequisicaoMercadoria.Compra);

                object retorno = null;
                int codigo = 1;
                int codigoMercadorias = 1;
                int codigoFornecedores = 1;
                if (mercadorias != null)
                {
                    List<Dominio.Entidades.Cliente> fornecedores = repCotacaoFornecedor.BuscarUltimosFornecedores(mercadorias.Select(o => o.ProdutoEstoque.Produto.Codigo).Distinct().ToList());
                    retorno = new
                    {
                        Codigo = codigo++,
                        Numero = "",
                        Descricao = "GERADO A PARTIR DE REQUISIÇÃO DE COMPRA",
                        DataEmissao = DateTime.Now.Date.ToString("dd/MM/yyyy"),
                        DataPrevisao = "",
                        Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCotacao.Aberto,
                        ValorTotal = (from obj in mercadorias select (obj.Saldo * obj.ProdutoEstoque.Produto.UltimoCusto)).Sum().ToString("n2"),
                        ListaMercadoria = (from obj in mercadorias
                                           select new
                                           {
                                               Codigo = codigoMercadorias++,
                                               CodigoCotacao = codigo,
                                               CodigoProduto = obj.ProdutoEstoque.Produto.Codigo,
                                               Descricao = obj.ProdutoEstoque.Produto.Descricao,
                                               Quantidade = obj.Saldo.ToString("n2"),
                                               ValorUnitario = obj.ProdutoEstoque.Produto.UltimoCusto.ToString("n4"),
                                               ValorTotal = (obj.Saldo * obj.ProdutoEstoque.Produto.UltimoCusto).ToString("n2")
                                           }).ToList(),
                        ListaFornecedor = (from obj in fornecedores
                                           select new
                                           {
                                               Codigo = codigoFornecedores++,
                                               CodigoCotacao = codigo,
                                               CodigoFornecedor = obj.CPF_CNPJ,
                                               Fornecedor = obj.Nome
                                           }).ToList(),
                        ListaRetornoProdutoFornecedor = new List<dynamic>()
                    };
                }
                return new JsonpResult(retorno);
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

        public async Task<IActionResult> BuscarUltimosFornecedores()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoProduto;
                int.TryParse(Request.Params("CodigoProduto"), out codigoProduto);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Repositorio.Embarcador.Compras.Cotacao repCotacao = new Repositorio.Embarcador.Compras.Cotacao(unitOfWork);
                Dominio.Entidades.Embarcador.Compras.CotacaoCompra ultimaCotacao = repCotacao.BuscarUltimaCotacao(codigoEmpresa, codigoProduto);

                if (ultimaCotacao != null)
                {
                    Guid g = Guid.NewGuid();
                    dynamic dynRetorno = new
                    {
                        ListaFornecedor = (from obj in ultimaCotacao.Fornecedores
                                           select new
                                           {
                                               Codigo = g.ToString(),
                                               CodigoCotacao = g.ToString(),
                                               CodigoFornecedor = obj.Fornecedor.Codigo,
                                               Fornecedor = obj.Fornecedor.Nome
                                           }).ToList()
                    };
                    return new JsonpResult(dynRetorno);
                }
                else
                    return new JsonpResult(true, "Não possui cotação anterior");

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os fornecedores anteriores");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> EnviarPorEmail()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork);
                Repositorio.Embarcador.Compras.CotacaoFornecedor repCotacaoFornecedor = new Repositorio.Embarcador.Compras.CotacaoFornecedor(unitOfWork);
                Repositorio.Embarcador.Compras.Cotacao repCotacao = new Repositorio.Embarcador.Compras.Cotacao(unitOfWork);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                int codigo = int.Parse(Request.Params("Codigo"));
                string nomeEmpresa = Cliente.NomeFantasia;
                string stringConexao = _conexao.StringConexao;

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = repRelatorio.BuscarPadraoPorCodigoControleRelatorio(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R116_CotaoCompraFornecedor, TipoServicoMultisoftware);
                if (relatorio == null)
                    relatorio = serRelatorio.BuscarConfiguracaoPadrao(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R116_CotaoCompraFornecedor, TipoServicoMultisoftware, "Relatorio de Cotação de Compra", "CotacaoCompra", "CotacaoCompraFornecedor.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Numero", "desc", "", "", 0, unitOfWork, false, false);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = serRelatorio.AdicionarRelatorioParaGeracao(relatorio, this.Usuario, Dominio.Enumeradores.TipoArquivoRelatorio.PDF, unitOfWork);
                List<Dominio.Entidades.Embarcador.Compras.CotacaoFornecedor> fornecedores = repCotacaoFornecedor.BuscarPorCotacao(codigo);

                IList<Dominio.Relatorios.Embarcador.DataSource.Compras.CotacaoCompraFornecedor> dadosCotacaoCompra = repCotacao.RelatorioCotacaoCompraFornecedor(codigo);
                if (dadosCotacaoCompra.Count > 0)
                {
                    //Task.Factory.StartNew(() => GerarRelatorioCotacaoCompraFornecedor(ref unitOfWork, codigo, nomeEmpresa, stringConexao, relatorioControleGeracao, dadosCotacaoCompra, fornecedores, this.Empresa, true));
                    GerarRelatorioCotacaoCompraFornecedor(ref unitOfWork, codigo, nomeEmpresa, stringConexao, relatorioControleGeracao, dadosCotacaoCompra, fornecedores, this.Empresa, true);
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, false, "Nenhum registro de pedido de cotação para regar o relatório.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o relatório. " + ex.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Privados

        private void GerarRelatorioCotacaoCompraFornecedor(ref Repositorio.UnitOfWork unitOfWork, int codigo, string nomeEmpresa, string stringConexao, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, IList<Dominio.Relatorios.Embarcador.DataSource.Compras.CotacaoCompraFornecedor> dadosCotacaoCompra, List<Dominio.Entidades.Embarcador.Compras.CotacaoFornecedor> fornecedores, Dominio.Entidades.Empresa empresa, bool enviarEmail = false)
        {
            var result = ReportRequest.WithType(ReportType.CotacaoCompra)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("Codigo", codigo)
                .AddExtraData("NomeEmpresa", nomeEmpresa)
                .AddExtraData("RelatorioControleGeracao", relatorioControleGeracao.Codigo)
                .AddExtraData("DadosCotacaoCompraDs", dadosCotacaoCompra.ToJson())
                .CallReport();
            
            if (enviarEmail)
            {
                string caminhoArquivo = result.FullPath.Replace("-", "");
                foreach (var fornecedor in fornecedores)
                {
                    EnviarEmailCotacaoFornecedor(ref unitOfWork, dadosCotacaoCompra[0].Numero, codigo, caminhoArquivo, fornecedor.Fornecedor, empresa);
                }
            }
        }

        private void EnviarEmailCotacaoFornecedor(ref Repositorio.UnitOfWork unitOfWork, int numero, int codigo, string caminhoRelatorio, Dominio.Entidades.Cliente fornecedor, Dominio.Entidades.Empresa empresa)
        {

            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(empresa.Codigo);

            if (email == null)
                throw new Exception("Não há um e-mail configurado para realizar o envio.");

            if (fornecedor != null)
            {
                string assunto = "";
                string mensagemEmail = "";

                assunto = "Cotação de Compra nº " + numero.ToString("n0") + " de " + empresa.RazaoSocial;
                mensagemEmail = "Olá,<br/><br/>Segue em anexo a Cotação de Compra da Empresa: " + empresa.RazaoSocial + ".<br/>";
                mensagemEmail += "Acesso o portal com o seu usuário e senha para responder esta cotação.<br/><br/><br/>";
                mensagemEmail += "E-mail enviado automaticamente. Por favor, não responda.";
                if (!string.IsNullOrWhiteSpace(email.MensagemRodape))
                    mensagemEmail += "<br/>" + "<br/>" + "<br/>" + email.MensagemRodape.Replace("#qLinha#", "<br/>");
                string mensagemErro = "Erro ao enviar e-mail";

                if (!string.IsNullOrWhiteSpace(caminhoRelatorio) && Utilidades.IO.FileStorageService.Storage.Exists(caminhoRelatorio))
                {
                    List<string> emails = new List<string>();
                    if (!string.IsNullOrWhiteSpace(fornecedor.Email))
                        emails.Add(fornecedor.Email);

                    for (int a = 0; a < fornecedor.Emails.Count; a++)
                    {
                        if (!string.IsNullOrWhiteSpace(fornecedor.Emails[a].Email) && fornecedor.Emails[a].EmailStatus == "A")
                            emails.Add(fornecedor.Emails[a].Email);
                    }

                    if (emails.Count > 0)
                    {
                        byte[] pdf = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoRelatorio);
                        bool sucesso = Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, emails.ToArray(), null, assunto, mensagemEmail, email.Smtp, out mensagemErro, email.DisplayEmail, new List<System.Net.Mail.Attachment>() { new System.Net.Mail.Attachment(new System.IO.MemoryStream(pdf), System.IO.Path.GetFileName(caminhoRelatorio), "application/pdf") }, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unitOfWork, empresa.Codigo);
                        if (!sucesso)
                            throw new Exception("Problemas ao enviar a cotação por e-mail: " + mensagemErro);
                    }
                }
                else
                    throw new Exception("Não foi possível localizar o arquivo PDF da cotação.");
            }
            else
                throw new Exception("Cotação não localizado para enviar e-mail.");
        }

        private bool IsPermitirFinalizarCotacaoCompra(Dominio.Entidades.Embarcador.Compras.CotacaoCompra cotacao)
        {
            if (cotacao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCotacao.Finalizado)
                return true;

            List<JsonFornecedor> listaFornecedores = JsonConvert.DeserializeObject<List<JsonFornecedor>>(Request.Params("ListaFornecedor"));

            return (listaFornecedores?.Count ?? 0) > 0;
        }

        private class JsonMercadoria
        {
            public string Codigo { get; set; }
            public string CodigoCotacao { get; set; }
            public int CodigoProduto { get; set; }
            public string Descricao { get; set; }
            public string Quantidade { get; set; }
            public string ValorUnitario { get; set; }
            public string ValorTotal { get; set; }
        }

        private class JsonFornecedor
        {
            public string Codigo { get; set; }
            public string CodigoCotacao { get; set; }
            public double CodigoFornecedor { get; set; }
            public string Fornecedor { get; set; }
        }

        private class JsonRetornoProdutoFornecedor
        {
            public string Codigo { get; set; }
            public string CodigoCotacao { get; set; }
            public string CodigoMercadoria { get; set; }
            public int CodigoProduto { get; set; }
            public int CodigoCondicaoPagamento { get; set; }
            public double CodigoFornecedor { get; set; }
            public string Observacao { get; set; }
            public string CondicaoPagamento { get; set; }
            public string Fornecedor { get; set; }
            public string Produto { get; set; }
            public string Quantidade { get; set; }
            public string QuantidadeRetorno { get; set; }
            public string ValorUnitario { get; set; }
            public string ValorUnitarioRetorno { get; set; }
            public string ValorTotal { get; set; }
            public string ValorTotalRetorno { get; set; }
            public string GerarOrdemCompra { get; set; }
            public bool BollGerarOrdemCompra { get; set; }
            public string Marca { get; set; }
        }

        private void SalvarListaMercadoria(Dominio.Entidades.Embarcador.Compras.CotacaoCompra cotacao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Compras.CotacaoProduto repCotacaoProduto = new Repositorio.Embarcador.Compras.CotacaoProduto(unidadeDeTrabalho);
            Repositorio.Produto repProduto = new Repositorio.Produto(unidadeDeTrabalho);

            List<int> produtosMantidos = new List<int>();

            List<JsonMercadoria> listaProdutos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<JsonMercadoria>>(Request.Params("ListaMercadoria"));
            if (listaProdutos != null)
            {
                foreach (JsonMercadoria produto in listaProdutos)
                {
                    int codigo = 0;
                    int.TryParse(produto.Codigo, out codigo);
                    Dominio.Entidades.Embarcador.Compras.CotacaoProduto prod = repCotacaoProduto.BuscarPorCodigo(codigo);
                    if (prod == null) prod = new Dominio.Entidades.Embarcador.Compras.CotacaoProduto();
                    else prod.Initialize();

                    prod.Cotacao = cotacao;
                    prod.Produto = repProduto.BuscarPorCodigo(produto.CodigoProduto);
                    prod.Quantidade = Utilidades.Decimal.Converter(produto.Quantidade);
                    prod.ValorTotal = Utilidades.Decimal.Converter(produto.ValorTotal);
                    prod.ValorUnitario = Utilidades.Decimal.Converter(produto.ValorUnitario);

                    if (prod.Codigo > 0)
                        repCotacaoProduto.Atualizar(prod, Auditado);
                    else
                        repCotacaoProduto.Inserir(prod, Auditado);

                    produtosMantidos.Add(prod.Codigo);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cotacao, null, (prod.Codigo > 0 ? "Atualizou" : "Adicionou") + " o produto " + prod.Produto.Descricao + " a cotação.", unidadeDeTrabalho);
                }


                List<Dominio.Entidades.Embarcador.Compras.CotacaoProduto> produtos = repCotacaoProduto.BuscarPorProdutosMantidos(produtosMantidos, cotacao.Codigo);
                for (int i = 0; i < produtos.Count(); i++)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cotacao, null, "Removeu o produto " + produtos[i].Produto.Descricao + " da cotação.", unidadeDeTrabalho);
                    repCotacaoProduto.Deletar(produtos[i]);
                }
            }
        }

        private void SalvarListaFornecedor(Dominio.Entidades.Embarcador.Compras.CotacaoCompra cotacao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Compras.CotacaoFornecedor repCotacaoFornecedor = new Repositorio.Embarcador.Compras.CotacaoFornecedor(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

            List<int> codigosMantidos = new List<int>();

            List<JsonFornecedor> listaFornecedores = Newtonsoft.Json.JsonConvert.DeserializeObject<List<JsonFornecedor>>(Request.Params("ListaFornecedor"));
            if (listaFornecedores != null)
            {
                foreach (JsonFornecedor fornecedor in listaFornecedores)
                {
                    int codigo = 0;
                    int.TryParse(fornecedor.Codigo, out codigo);
                    Dominio.Entidades.Embarcador.Compras.CotacaoFornecedor forn = repCotacaoFornecedor.BuscarPorCodigo(codigo);
                    if (forn == null) forn = new Dominio.Entidades.Embarcador.Compras.CotacaoFornecedor();
                    else forn.Initialize();

                    forn.Cotacao = cotacao;
                    forn.Fornecedor = repCliente.BuscarPorCPFCNPJ(fornecedor.CodigoFornecedor);

                    if (forn.Codigo > 0)
                        repCotacaoFornecedor.Atualizar(forn, Auditado);
                    else
                        repCotacaoFornecedor.Inserir(forn, Auditado);

                    codigosMantidos.Add(forn.Codigo);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cotacao, null, (forn.Codigo > 0 ? "Atualizou" : "Adicionou") + " o fornecedor " + forn.Fornecedor.Nome + " a cotação.", unidadeDeTrabalho);
                }

                List<Dominio.Entidades.Embarcador.Compras.CotacaoFornecedor> fornecedores = repCotacaoFornecedor.BuscarPorCodigosMantidos(codigosMantidos, cotacao.Codigo);
                for (int i = 0; i < fornecedores.Count(); i++)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cotacao, null, "Removeu o fornecedor " + fornecedores[i].Fornecedor.Nome + " da cotação.", unidadeDeTrabalho);
                    repCotacaoFornecedor.Deletar(fornecedores[i]);
                }
            }
        }

        private void SalvarListaRetornoProdutoFornecedor(Dominio.Entidades.Embarcador.Compras.CotacaoCompra cotacao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Compras.CotacaoRetornoFornecedorProduto repRetorno = new Repositorio.Embarcador.Compras.CotacaoRetornoFornecedorProduto(unidadeDeTrabalho);
            Repositorio.Embarcador.Compras.CondicaoPagamento repCondicaoPagamento = new Repositorio.Embarcador.Compras.CondicaoPagamento(unidadeDeTrabalho);
            Repositorio.Embarcador.Compras.CotacaoFornecedor repCotacaoFornecedor = new Repositorio.Embarcador.Compras.CotacaoFornecedor(unidadeDeTrabalho);
            Repositorio.Embarcador.Compras.CotacaoProduto repCotacaoProduto = new Repositorio.Embarcador.Compras.CotacaoProduto(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
            Repositorio.Produto repProduto = new Repositorio.Produto(unidadeDeTrabalho);

            List<int> codigosMantidos = new List<int>();

            List<JsonRetornoProdutoFornecedor> listaRetornos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<JsonRetornoProdutoFornecedor>>(Request.Params("ListaRetornoProdutoFornecedor"));
            if (listaRetornos != null)
            {
                foreach (JsonRetornoProdutoFornecedor retorno in listaRetornos)
                {
                    int codigo = 0;
                    int.TryParse(retorno.Codigo, out codigo);
                    Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto ret = repRetorno.BuscarPorCodigo(codigo);
                    if (ret == null) ret = new Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto();
                    else ret.Initialize();

                    ret.CondicaoPagamento = retorno.CodigoCondicaoPagamento > 0 ? repCondicaoPagamento.BuscarPorCodigo(retorno.CodigoCondicaoPagamento) : null;
                    ret.GerarOrdemCompra = retorno.BollGerarOrdemCompra;
                    ret.Observacao = retorno.Observacao;
                    ret.Marca = retorno.Marca;
                    ret.QuantidadeRetorno = Utilidades.Decimal.Converter(retorno.QuantidadeRetorno);
                    ret.ValorTotalRetorno = Utilidades.Decimal.Converter(retorno.ValorTotalRetorno);
                    ret.ValorUnitarioRetorno = Utilidades.Decimal.Converter(retorno.ValorUnitarioRetorno);
                    ret.CotacaoFornecedor = repCotacaoFornecedor.BuscarPorCodigoFornecedor(cotacao.Codigo, retorno.CodigoFornecedor);
                    ret.CotacaoProduto = repCotacaoProduto.BuscarPorCodigoProduto(cotacao.Codigo, retorno.CodigoProduto);

                    if (ret.Codigo > 0)
                        repRetorno.Atualizar(ret, Auditado);
                    else
                        repRetorno.Inserir(ret, Auditado);

                    codigosMantidos.Add(ret.Codigo);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cotacao, null, (ret.Codigo > 0 ? "Atualizou" : "Adicionou") + " o fornecedor " + ret.CotacaoFornecedor.Fornecedor.Nome + " do produto " + ret.CotacaoProduto.Produto.Descricao + " a cotação.", unidadeDeTrabalho);
                }

                List<Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto> retornos = repRetorno.BuscarPorCodigosMantidos(codigosMantidos, cotacao.Codigo);
                for (int i = 0; i < retornos.Count(); i++)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cotacao, null, "Removeu o fornecedor " + retornos[i].CotacaoFornecedor.Fornecedor.Nome + " do produto " + retornos[i].CotacaoProduto.Produto.Descricao + " da cotação.", unidadeDeTrabalho);
                    repRetorno.Deletar(retornos[i]);
                }
            }
        }

        private List<int> RetornaCodigoRequisicaoCompra(Repositorio.UnitOfWork unidadeDeTrabalho, string codigoRequisicaoCompra)
        {
            List<int> listaCodigos = new List<int>();
            if (!string.IsNullOrWhiteSpace(codigoRequisicaoCompra))
            {
                dynamic listaRequisicoes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(codigoRequisicaoCompra);
                if (listaRequisicoes != null)
                {
                    foreach (var req in listaRequisicoes)
                    {
                        listaCodigos.Add(int.Parse(Utilidades.String.OnlyNumbers((string)req.Codigo)));
                    }
                }
            }
            return listaCodigos;
        }

        #endregion
    }
}
