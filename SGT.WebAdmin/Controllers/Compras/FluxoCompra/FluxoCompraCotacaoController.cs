using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Compras.FluxoCompra
{
    [CustomAuthorize("Compras/FluxoCompra")]
    public class FluxoCompraCotacaoController : BaseController
    {
		#region Construtores

		public FluxoCompraCotacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> GerarCotacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Compras.FluxoCompra repFluxoCompra = new Repositorio.Embarcador.Compras.FluxoCompra(unitOfWork);

                Dominio.Entidades.Embarcador.Compras.FluxoCompra fluxoCompra = repFluxoCompra.BuscarPorCodigo(codigo);

                if (fluxoCompra == null)
                    return new JsonpResult(false, true, "Fluxo de compra não encontrado.");

                if (fluxoCompra.EtapaAtual != EtapaFluxoCompra.AprovacaoRequisicao)
                    return new JsonpResult(false, true, "Cotação já foi gerada para esse fluxo de compra.");

                unitOfWork.Start();

                fluxoCompra.EtapaAtual = EtapaFluxoCompra.Cotacao;

                fluxoCompra.Cotacao = GerarCotacao(fluxoCompra, unitOfWork);

                repFluxoCompra.Atualizar(fluxoCompra);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, fluxoCompra, null, "Gerou a cotação", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(fluxoCompra.Cotacao.Codigo);
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
                return new JsonpResult(false, "Ocorreu uma falha ao gerar a cotação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarCotacaoPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Compras.Cotacao repCotacao = new Repositorio.Embarcador.Compras.Cotacao(unitOfWork);

                Dominio.Entidades.Embarcador.Compras.CotacaoCompra cotacao = repCotacao.BuscarPorCodigo(codigo);

                if (cotacao == null)
                    return new JsonpResult(false, true, "Cotação não encontrada.");

                dynamic retorno = new
                {
                    cotacao.Codigo,
                    cotacao.Numero,
                    cotacao.Descricao,
                    DataEmissao = cotacao.DataEmissao.Value.ToString("dd/MM/yyyy"),
                    DataPrevisao = cotacao.DataPrevisao?.ToString("dd/MM/yyyy") ?? string.Empty,
                    Mercadorias = (from obj in cotacao.Produtos
                                   select new
                                   {
                                       Codigo = obj.Codigo,
                                       Quantidade = obj.Quantidade.ToString("n2"),
                                       ValorUnitario = obj.ValorUnitario.ToString("n4"),
                                       ValorTotal = obj.ValorTotal.ToString("n2"),
                                       Produto = new { Codigo = obj.Produto?.Codigo ?? 0, Descricao = obj.Produto?.Descricao ?? string.Empty }
                                   }).ToList(),
                    Fornecedores = (from obj in cotacao.Fornecedores
                                    select new
                                    {
                                        Codigo = obj.Codigo,
                                        Fornecedor = new { Codigo = obj.Fornecedor?.CPF_CNPJ ?? 0, Descricao = obj.Fornecedor?.Descricao ?? string.Empty }
                                    }).ToList()
                };

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

        public async Task<IActionResult> AtualizarCotacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                int codigoFluxoCompra = Request.GetIntParam("CodigoFluxoCompra");

                Repositorio.Embarcador.Compras.Cotacao repCotacao = new Repositorio.Embarcador.Compras.Cotacao(unitOfWork);
                Repositorio.Embarcador.Compras.FluxoCompra repFluxoCompra = new Repositorio.Embarcador.Compras.FluxoCompra(unitOfWork);

                Dominio.Entidades.Embarcador.Compras.CotacaoCompra cotacao = repCotacao.BuscarPorCodigo(codigo, true);
                Dominio.Entidades.Embarcador.Compras.FluxoCompra fluxoCompra = repFluxoCompra.BuscarPorCodigo(codigoFluxoCompra);

                if (cotacao == null)
                    return new JsonpResult(false, true, "Cotação não encontrada.");

                if (fluxoCompra == null)
                    return new JsonpResult(false, true, "Fluxo de compra não encontrado.");

                if (cotacao.Situacao != SituacaoCotacao.Aberto)
                    return new JsonpResult(false, true, "Cotação não está mais em aberta.");

                if (fluxoCompra.EtapaAtual != EtapaFluxoCompra.Cotacao)
                    return new JsonpResult(false, true, "O fluxo de compra não está mais na etapa de cotação.");

                if (fluxoCompra.VoltouParaEtapaAtual)
                    return new JsonpResult(false, true, "Não é permitido usar essa funcionalidade quando voltou etapa.");

                unitOfWork.Start();

                cotacao.DataEmissao = Request.GetDateTimeParam("DataEmissao");
                cotacao.DataPrevisao = Request.GetDateTimeParam("DataPrevisao");
                cotacao.Descricao = Request.GetStringParam("Descricao");

                repCotacao.Atualizar(cotacao, Auditado);

                SalvarMercadorias(cotacao, unitOfWork);
                SalvarFornecedores(cotacao, unitOfWork, false);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar a cotação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DisponibilizarCotacaoParaFornecedor()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                int codigoFluxoCompra = Request.GetIntParam("CodigoFluxoCompra");

                Repositorio.Embarcador.Compras.FluxoCompra repFluxoCompra = new Repositorio.Embarcador.Compras.FluxoCompra(unitOfWork);
                Repositorio.Embarcador.Compras.Cotacao repCotacao = new Repositorio.Embarcador.Compras.Cotacao(unitOfWork);

                Dominio.Entidades.Embarcador.Compras.FluxoCompra fluxoCompra = repFluxoCompra.BuscarPorCodigo(codigoFluxoCompra);
                Dominio.Entidades.Embarcador.Compras.CotacaoCompra cotacao = repCotacao.BuscarPorCodigo(codigo, true);

                if (fluxoCompra == null)
                    return new JsonpResult(false, true, "Fluxo de compra não encontrado.");

                if (cotacao == null)
                    return new JsonpResult(false, true, "Cotação não encontrada.");

                if (fluxoCompra.EtapaAtual != EtapaFluxoCompra.Cotacao)
                    return new JsonpResult(false, true, "O fluxo de compra não está mais na etapa de cotação.");

                if (cotacao.Situacao != SituacaoCotacao.Aberto)
                    return new JsonpResult(false, true, "Cotação não está mais em aberta.");

                if (cotacao.Fornecedores.Count == 0)
                    return new JsonpResult(false, true, "É necessário informar um ou mais fornecedores.");

                if (fluxoCompra.VoltouParaEtapaAtual)
                    return new JsonpResult(false, true, "Não é permitido usar essa funcionalidade quando voltou etapa.");

                unitOfWork.Start();

                fluxoCompra.EtapaAtual = EtapaFluxoCompra.RetornoCotacao;
                repFluxoCompra.Atualizar(fluxoCompra);

                cotacao.Situacao = SituacaoCotacao.AguardandoRetorno;
                repCotacao.Atualizar(cotacao, Auditado);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, fluxoCompra, null, "Enviou a cotação para o fornecedor", unitOfWork);

                unitOfWork.CommitChanges();

                Servicos.Embarcador.Compras.CotacaoCompra serCotacaoCompra = new Servicos.Embarcador.Compras.CotacaoCompra(unitOfWork);
                serCotacaoCompra.EnviarEmailNovaCotacaoParaFornecedores(fluxoCompra.Cotacao, unitOfWork, TipoServicoMultisoftware, null);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao disponibilizar a cotação ao fornecedor.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarRetornoCotacaoPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Compras.Cotacao repCotacao = new Repositorio.Embarcador.Compras.Cotacao(unitOfWork);
                Repositorio.Embarcador.Compras.CotacaoRetornoFornecedorProduto repRetorno = new Repositorio.Embarcador.Compras.CotacaoRetornoFornecedorProduto(unitOfWork);

                Dominio.Entidades.Embarcador.Compras.CotacaoCompra cotacao = repCotacao.BuscarPorCodigo(codigo);

                if (cotacao == null)
                    return new JsonpResult(false, true, "Cotação não encontrada.");

                List<Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto> retornos = repRetorno.BuscarPorCotacao(codigo);

                dynamic retorno = new
                {
                    cotacao.Codigo,
                    Mercadorias = (from obj in cotacao.Produtos
                                   select new
                                   {
                                       Codigo = obj.Codigo,
                                       Quantidade = obj.Quantidade.ToString("n2"),
                                       ValorUnitario = obj.ValorUnitario.ToString("n4"),
                                       ValorTotal = obj.ValorTotal.ToString("n2"),
                                       Produto = new { Codigo = obj.Produto?.Codigo ?? 0, Descricao = obj.Produto?.Descricao ?? string.Empty }
                                   }).ToList(),
                    Fornecedores = (from obj in cotacao.Fornecedores
                                    select new
                                    {
                                        CodigoFornecedor = obj.Fornecedor.CPF_CNPJ
                                    }).ToList(),
                    Retornos = (from obj in retornos
                                select new
                                {
                                    Codigo = obj.Codigo,

                                    Produto = new { Codigo = obj.CotacaoProduto.Produto.Codigo, Descricao = obj.CotacaoProduto.Produto.Descricao },
                                    Fornecedor = new { Codigo = obj.CotacaoFornecedor.Fornecedor.CPF_CNPJ, Descricao = obj.CotacaoFornecedor.Fornecedor.Nome },
                                    CondicaoPagamento = new { Codigo = obj.CondicaoPagamento?.Codigo ?? 0, Descricao = obj.CondicaoPagamento?.Descricao ?? string.Empty },

                                    Quantidade = obj.CotacaoProduto.Quantidade.ToString("n2"),
                                    QuantidadeOriginal = obj.CotacaoProduto.Quantidade.ToString("n2"),
                                    QuantidadeRetorno = obj.QuantidadeRetorno.ToString("n2"),
                                    ValorUnitario = obj.CotacaoProduto.ValorUnitario.ToString("n4"),
                                    ValorUnitarioOriginal = obj.CotacaoProduto.ValorUnitario.ToString("n4"),
                                    ValorUnitarioRetorno = obj.ValorUnitarioRetorno.ToString("n4"),
                                    ValorTotal = obj.CotacaoProduto.ValorTotal.ToString("n2"),
                                    ValorTotalItemOriginal = obj.CotacaoProduto.ValorTotal.ToString("n2"),
                                    ValorTotalRetorno = obj.ValorTotalRetorno.ToString("n2"),
                                    obj.GerarOrdemCompra,
                                    obj.Observacao,
                                    Marca = obj.Marca ?? string.Empty
                                }).ToList()
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar a cotação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarRetornoCotacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Compras.Cotacao repCotacao = new Repositorio.Embarcador.Compras.Cotacao(unitOfWork);

                Dominio.Entidades.Embarcador.Compras.CotacaoCompra cotacao = repCotacao.BuscarPorCodigo(codigo);

                if (cotacao == null)
                    return new JsonpResult(false, true, "Cotação não encontrada.");

                if (cotacao.Situacao != SituacaoCotacao.AguardandoRetorno)
                    return new JsonpResult(false, true, "Cotação não está mais aguardando retorno.");

                unitOfWork.Start();

                SalvarRetornoProdutoFornecedor(cotacao, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar o retorno da cotação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> VoltarParaCotacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Compras.FluxoCompra repFluxoCompra = new Repositorio.Embarcador.Compras.FluxoCompra(unitOfWork);

                Dominio.Entidades.Embarcador.Compras.FluxoCompra fluxoCompra = repFluxoCompra.BuscarPorCodigo(codigo);

                if (fluxoCompra == null)
                    return new JsonpResult(false, true, "Fluxo de compra não encontrado.");

                if (fluxoCompra.EtapaAtual != EtapaFluxoCompra.RetornoCotacao)
                    return new JsonpResult(false, true, "A etapa atual do fluxo de compra não é a de Retorno da Cotação.");

                if (fluxoCompra.Cotacao.Situacao != SituacaoCotacao.AguardandoRetorno)
                    return new JsonpResult(false, true, "Cotação não está mais aguardando retorno.");

                if (fluxoCompra.VoltouParaEtapaAtual)
                    return new JsonpResult(false, true, "A etapa atual do fluxo de compra já foi retornada.");

                unitOfWork.Start();

                fluxoCompra.EtapaAtual = EtapaFluxoCompra.Cotacao;
                fluxoCompra.VoltouParaEtapaAtual = true;

                repFluxoCompra.Atualizar(fluxoCompra);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, fluxoCompra, null, "Voltou para a etapa de cotação", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao retornar para a cotação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AvancarCotacaoRetornada()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("CodigoFluxoCompra");

                Repositorio.Embarcador.Compras.FluxoCompra repFluxoCompra = new Repositorio.Embarcador.Compras.FluxoCompra(unitOfWork);

                Dominio.Entidades.Embarcador.Compras.FluxoCompra fluxoCompra = repFluxoCompra.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Compras.CotacaoCompra cotacao = fluxoCompra.Cotacao;

                if (fluxoCompra == null)
                    return new JsonpResult(false, true, "Fluxo de compra não encontrado.");

                if (fluxoCompra.EtapaAtual != EtapaFluxoCompra.Cotacao)
                    return new JsonpResult(false, true, "A etapa atual do fluxo de compra não é a de Cotação.");

                if (!fluxoCompra.VoltouParaEtapaAtual)
                    return new JsonpResult(false, true, "A etapa atual do fluxo de compra não foi retornada.");

                unitOfWork.Start();

                fluxoCompra.EtapaAtual = EtapaFluxoCompra.RetornoCotacao;
                fluxoCompra.VoltouParaEtapaAtual = false;

                repFluxoCompra.Atualizar(fluxoCompra);

                if (cotacao.Situacao == SituacaoCotacao.AguardandoRetorno)
                    SalvarFornecedores(cotacao, unitOfWork, true);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, fluxoCompra, null, "Avançou para a etapa de retorno da cotação", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao avançar a cotação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Compras.CotacaoCompra GerarCotacao(Dominio.Entidades.Embarcador.Compras.FluxoCompra fluxoCompra, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria> requisicoes = fluxoCompra.RequisicoesMercadoria.Where(o => o.Situacao != SituacaoRequisicaoMercadoria.Rejeitada && o.Situacao != SituacaoRequisicaoMercadoria.Cancelado).ToList();

            if (requisicoes.Count == 0)
                throw new ControllerException("Nenhuma requisição está aprovada.");

            if (requisicoes.Any(o => o.Situacao != SituacaoRequisicaoMercadoria.Aprovada))
                throw new ControllerException("Há requisições que não estão aprovadas, não sendo possível gerar a cotação.");

            Repositorio.Embarcador.Compras.Cotacao repCotacao = new Repositorio.Embarcador.Compras.Cotacao(unitOfWork);
            Repositorio.Embarcador.Compras.Mercadoria repMercadoria = new Repositorio.Embarcador.Compras.Mercadoria(unitOfWork);
            Repositorio.Embarcador.Compras.CotacaoProduto repCotacaoProduto = new Repositorio.Embarcador.Compras.CotacaoProduto(unitOfWork);
            Repositorio.Embarcador.Compras.CotacaoFornecedor repCotacaoFornecedor = new Repositorio.Embarcador.Compras.CotacaoFornecedor(unitOfWork);
            Repositorio.Embarcador.Compras.CotacaoCompraRequisicaoMercadoria repCotacaoRequisicao = new Repositorio.Embarcador.Compras.CotacaoCompraRequisicaoMercadoria(unitOfWork);

            List<Dominio.Entidades.Embarcador.Compras.Mercadoria> mercadorias = repMercadoria.BuscarPorRequisicaoCompra(requisicoes.Select(o => o.Codigo).ToList(), ModoRequisicaoMercadoria.Compra);
            List<Dominio.Entidades.Cliente> fornecedores = repCotacaoFornecedor.BuscarUltimosFornecedores(mercadorias.Select(o => o.ProdutoEstoque.Produto.Codigo).Distinct().ToList());

            int codigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? Empresa.Codigo : 0;

            Dominio.Entidades.Embarcador.Compras.CotacaoCompra cotacao = new Dominio.Entidades.Embarcador.Compras.CotacaoCompra();

            cotacao.DataEmissao = DateTime.Now.Date;
            cotacao.DataPrevisao = DateTime.Now.Date;
            cotacao.Situacao = SituacaoCotacao.Aberto;
            cotacao.Numero = repCotacao.ProximoNumero(codigoEmpresa);
            cotacao.Usuario = Usuario;
            cotacao.Empresa = Empresa;
            cotacao.Descricao = $"GERADO A PARTIR DO FLUXO DE COMPRA Nº { fluxoCompra.Numero } ";

            repCotacao.Inserir(cotacao, Auditado);

            foreach (Dominio.Entidades.Embarcador.Compras.Mercadoria mercadoria in mercadorias)
            {
                Dominio.Entidades.Embarcador.Compras.CotacaoProduto cotacaoProduto = new Dominio.Entidades.Embarcador.Compras.CotacaoProduto();

                cotacaoProduto.Cotacao = cotacao;
                cotacaoProduto.Produto = mercadoria.ProdutoEstoque.Produto;
                cotacaoProduto.Quantidade = mercadoria.Saldo;
                cotacaoProduto.ValorUnitario = mercadoria.ProdutoEstoque.Produto.UltimoCusto;
                cotacaoProduto.ValorTotal = mercadoria.Saldo * mercadoria.ProdutoEstoque.Produto.UltimoCusto;

                repCotacaoProduto.Inserir(cotacaoProduto);
            }

            foreach (Dominio.Entidades.Cliente fornecedor in fornecedores)
            {
                Dominio.Entidades.Embarcador.Compras.CotacaoFornecedor cotacaoFornecedor = new Dominio.Entidades.Embarcador.Compras.CotacaoFornecedor();
                cotacaoFornecedor.Cotacao = cotacao;
                cotacaoFornecedor.Fornecedor = fornecedor;
                repCotacaoFornecedor.Inserir(cotacaoFornecedor);
            }

            foreach (Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria requisicao in requisicoes)
            {
                Dominio.Entidades.Embarcador.Compras.CotacaoCompraRequisicaoMercadoria cotacaoRequisicao = new Dominio.Entidades.Embarcador.Compras.CotacaoCompraRequisicaoMercadoria();
                cotacaoRequisicao.Cotacao = cotacao;
                cotacaoRequisicao.RequisicaoMercadoria = requisicao;
                repCotacaoRequisicao.Inserir(cotacaoRequisicao);
            }

            return cotacao;
        }

        private void SalvarMercadorias(Dominio.Entidades.Embarcador.Compras.CotacaoCompra cotacao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Compras.CotacaoProduto repCotacaoProduto = new Repositorio.Embarcador.Compras.CotacaoProduto(unidadeDeTrabalho);
            Repositorio.Produto repProduto = new Repositorio.Produto(unidadeDeTrabalho);

            dynamic mercadorias = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Mercadorias"));

            if (cotacao.Produtos != null && cotacao.Produtos.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic mercadoria in mercadorias)
                    if (mercadoria.Codigo != null)
                        codigos.Add((int)mercadoria.Codigo);

                List<Dominio.Entidades.Embarcador.Compras.CotacaoProduto> produtoDeletar = (from obj in cotacao.Produtos where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < produtoDeletar.Count; i++)
                    repCotacaoProduto.Deletar(produtoDeletar[i], Auditado);
            }
            else
                cotacao.Produtos = new List<Dominio.Entidades.Embarcador.Compras.CotacaoProduto>();

            foreach (dynamic mercadoria in mercadorias)
            {
                int.TryParse((string)mercadoria.Codigo, out int codigoCotacaoProduto);
                Dominio.Entidades.Embarcador.Compras.CotacaoProduto cotacaoProduto = codigoCotacaoProduto > 0 ? repCotacaoProduto.BuscarPorCodigo(codigoCotacaoProduto, true) : null;

                if (cotacaoProduto == null)
                {
                    cotacaoProduto = new Dominio.Entidades.Embarcador.Compras.CotacaoProduto();
                    cotacaoProduto.Cotacao = cotacao;
                }

                cotacaoProduto.Quantidade = Utilidades.Decimal.Converter((string)mercadoria.Quantidade);
                cotacaoProduto.ValorUnitario = Utilidades.Decimal.Converter((string)mercadoria.ValorUnitario);
                cotacaoProduto.ValorTotal = Utilidades.Decimal.Converter((string)mercadoria.ValorTotal);

                int codigoProduto = ((string)mercadoria.Produto.Codigo).ToInt();
                cotacaoProduto.Produto = repProduto.BuscarPorCodigo(codigoProduto);

                if (cotacaoProduto.Codigo > 0)
                    repCotacaoProduto.Atualizar(cotacaoProduto, Auditado);
                else
                    repCotacaoProduto.Inserir(cotacaoProduto, Auditado);
            }
        }

        private void SalvarFornecedores(Dominio.Entidades.Embarcador.Compras.CotacaoCompra cotacao, Repositorio.UnitOfWork unidadeDeTrabalho, bool voltouParaEtapaAtual)
        {
            Repositorio.Embarcador.Compras.CotacaoFornecedor repCotacaoFornecedor = new Repositorio.Embarcador.Compras.CotacaoFornecedor(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

            dynamic fornecedores = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Fornecedores"));

            if (!voltouParaEtapaAtual)
            {
                if (cotacao.Fornecedores?.Count > 0)
                {
                    List<int> codigos = new List<int>();

                    foreach (dynamic fornecedor in fornecedores)
                        if (fornecedor.Codigo != null)
                            codigos.Add((int)fornecedor.Codigo);

                    List<Dominio.Entidades.Embarcador.Compras.CotacaoFornecedor> produtoDeletar = (from obj in cotacao.Fornecedores where !codigos.Contains(obj.Codigo) select obj).ToList();

                    for (var i = 0; i < produtoDeletar.Count; i++)
                        repCotacaoFornecedor.Deletar(produtoDeletar[i], Auditado);
                }
                else
                    cotacao.Fornecedores = new List<Dominio.Entidades.Embarcador.Compras.CotacaoFornecedor>();
            }

            List<Dominio.Entidades.Cliente> novosFornecedores = new List<Dominio.Entidades.Cliente>();
            foreach (dynamic fornecedor in fornecedores)
            {
                int.TryParse((string)fornecedor.Codigo, out int codigoCotacaoFornecedor);
                Dominio.Entidades.Embarcador.Compras.CotacaoFornecedor cotacaoFornecedor = codigoCotacaoFornecedor > 0 ? repCotacaoFornecedor.BuscarPorCodigo(codigoCotacaoFornecedor) : null;

                if (cotacaoFornecedor == null)
                {
                    cotacaoFornecedor = new Dominio.Entidades.Embarcador.Compras.CotacaoFornecedor();
                    cotacaoFornecedor.Cotacao = cotacao;

                    double codigoFornecedor = ((string)fornecedor.Fornecedor.Codigo).ToDouble();
                    cotacaoFornecedor.Fornecedor = repCliente.BuscarPorCPFCNPJ(codigoFornecedor);

                    repCotacaoFornecedor.Inserir(cotacaoFornecedor, Auditado);

                    novosFornecedores.Add(cotacaoFornecedor.Fornecedor);
                }
            }

            if (voltouParaEtapaAtual && novosFornecedores.Count > 0)
            {
                Servicos.Embarcador.Compras.CotacaoCompra serCotacaoCompra = new Servicos.Embarcador.Compras.CotacaoCompra(unidadeDeTrabalho);
                serCotacaoCompra.EnviarEmailNovaCotacaoParaFornecedores(cotacao, unidadeDeTrabalho, TipoServicoMultisoftware, novosFornecedores);
            }
        }

        private void SalvarRetornoProdutoFornecedor(Dominio.Entidades.Embarcador.Compras.CotacaoCompra cotacao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Compras.CotacaoRetornoFornecedorProduto repRetorno = new Repositorio.Embarcador.Compras.CotacaoRetornoFornecedorProduto(unidadeDeTrabalho);
            Repositorio.Embarcador.Compras.CondicaoPagamento repCondicaoPagamento = new Repositorio.Embarcador.Compras.CondicaoPagamento(unidadeDeTrabalho);
            Repositorio.Embarcador.Compras.CotacaoFornecedor repCotacaoFornecedor = new Repositorio.Embarcador.Compras.CotacaoFornecedor(unidadeDeTrabalho);
            Repositorio.Embarcador.Compras.CotacaoProduto repCotacaoProduto = new Repositorio.Embarcador.Compras.CotacaoProduto(unidadeDeTrabalho);

            dynamic dynRetornos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Retornos"));

            List<Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto> retornos = repRetorno.BuscarPorCotacao(cotacao.Codigo);

            if (retornos.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic retorno in dynRetornos)
                    if (retorno.Codigo != null)
                        codigos.Add((int)retorno.Codigo);

                List<Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto> listaDeletar = (from obj in retornos where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < listaDeletar.Count; i++)
                    repRetorno.Deletar(listaDeletar[i], Auditado);
            }

            foreach (dynamic retorno in dynRetornos)
            {
                int.TryParse((string)retorno.Codigo, out int codigoCotacaoRetorno);
                Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto cotacaoRetorno = codigoCotacaoRetorno > 0 ? repRetorno.BuscarPorCodigo(codigoCotacaoRetorno, true) : null;

                if (cotacaoRetorno == null)
                {
                    cotacaoRetorno = new Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto();
                    cotacaoRetorno.CotacaoProduto = repCotacaoProduto.BuscarPorCodigoProduto(cotacao.Codigo, (int)retorno.Produto.Codigo);
                }

                cotacaoRetorno.CondicaoPagamento = (int)retorno.CondicaoPagamento.Codigo > 0 ? repCondicaoPagamento.BuscarPorCodigo((int)retorno.CondicaoPagamento.Codigo) : null;
                cotacaoRetorno.GerarOrdemCompra = retorno.GerarOrdemCompra;
                cotacaoRetorno.Observacao = retorno.Observacao;
                cotacaoRetorno.Marca = retorno.Marca;
                cotacaoRetorno.QuantidadeRetorno = Utilidades.Decimal.Converter((string)retorno.QuantidadeRetorno);
                cotacaoRetorno.ValorTotalRetorno = Utilidades.Decimal.Converter((string)retorno.ValorTotalRetorno);
                cotacaoRetorno.ValorUnitarioRetorno = Utilidades.Decimal.Converter((string)retorno.ValorUnitarioRetorno);
                cotacaoRetorno.CotacaoFornecedor = repCotacaoFornecedor.BuscarPorCodigoFornecedor(cotacao.Codigo, (double)retorno.Fornecedor.Codigo);

                if (cotacaoRetorno.Codigo > 0)
                    repRetorno.Atualizar(cotacaoRetorno, Auditado);
                else
                    repRetorno.Inserir(cotacaoRetorno, Auditado);
            }
        }

        #endregion
    }
}
