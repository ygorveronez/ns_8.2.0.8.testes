
using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Produtos;
using Dominio.ObjetosDeValor.WebService;
using Newtonsoft.Json;
using Servicos.Embarcador.Integracao;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.WebService.Produtos
{
    public class Produto : ServicoWebServiceBase
    {
        #region Variaveis Privadas

        readonly Repositorio.UnitOfWork _unitOfWork;
        readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        readonly TipoServicoMultisoftware _tipoServicoMultisoftware;
        readonly AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _clienteMultisoftware;
        readonly AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso _clienteAcesso;
        readonly protected string _adminStringConexao;

        #endregion

        #region Constructores
        public Produto(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso, string adminStringConexao) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _clienteMultisoftware = clienteMultisoftware;
            _auditado = auditado;
            _clienteAcesso = clienteAcesso;
            _adminStringConexao = adminStringConexao;
        }
        #endregion

        #region Metodos Publicos
        public Retorno<bool> IntegrarPOData(Dominio.ObjetosDeValor.Embarcador.Produtos.OrdemDeCompra ordemDeCompraObjeto)
        {
            Repositorio.Embarcador.Produtos.OrdemCompraPrincipal repositorioOrdemCompra = new Repositorio.Embarcador.Produtos.OrdemCompraPrincipal(_unitOfWork);
            Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraPrincipal ordeCompraPrincipal = repositorioOrdemCompra.BuscarPorCodigoIntegracao(ordemDeCompraObjeto.iDoc.ToString());
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            if (ordeCompraPrincipal == null)
                ordeCompraPrincipal = new Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraPrincipal();

            ordeCompraPrincipal.ControleIntegracaoEmbarcador = ordemDeCompraObjeto.iDoc.ToString();
            ordeCompraPrincipal.DataIntegracao = DateTime.Now;

            if (ordeCompraPrincipal.Codigo > 0)
                repositorioOrdemCompra.Atualizar(ordeCompraPrincipal);
            else
                repositorioOrdemCompra.Inserir(ordeCompraPrincipal);

            ProcessarDocumentosOrdemCompra(ordeCompraPrincipal, ordemDeCompraObjeto.productOrders);
            ordeCompraPrincipal.ProblemaIntegracao = "Integrado com sucesso";

            string request = JsonConvert.SerializeObject(ordemDeCompraObjeto);
            string response = JsonConvert.SerializeObject(Retorno<bool>.CriarRetornoSucesso(true, " Dados recebidos com sucesso"));

            servicoArquivoTransacao.Adicionar(ordeCompraPrincipal, request, response, "json");
            repositorioOrdemCompra.Atualizar(ordeCompraPrincipal);

            return Retorno<bool>.CriarRetornoSucesso(true, "Dados recebidos com sucesso");
        }
        public Retorno<bool> IntegrarHistoryData(Dominio.ObjetosDeValor.Embarcador.Produtos.HistorioalOrdemPrincipal ordemDeCompraObjeto)
        {
            Repositorio.Embarcador.Produtos.OrdemDeCompraHistorial repositorioOdermDeCompraHistorial = new Repositorio.Embarcador.Produtos.OrdemDeCompraHistorial(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repositorioFilia = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repositorioProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Produtos.OrdemCompraHistorial> existeRegistroHistorioal = repositorioOdermDeCompraHistorial.BuscarPorCodigoIntegracaoEmbarcador(ordemDeCompraObjeto.IDoc);

            foreach (ProductOrderHistory ordemProdutoHistori in ordemDeCompraObjeto.ProductOrderHistories)
            {
                foreach (HistoryItem item in ordemProdutoHistori.HistoryItems)
                {
                    foreach (Material material in item.Materials)
                    {
                        foreach (MaterialDocumentYear materialDocumento in material.MaterialDocumentYears)
                        {
                            foreach (MaterialItem materialItem in materialDocumento.MaterialItems)
                            {
                                Dominio.Entidades.Embarcador.Produtos.OrdemCompraHistorial novoItemHistorial = new Dominio.Entidades.Embarcador.Produtos.OrdemCompraHistorial();
                                novoItemHistorial.ControleIntegracaoEmbarcador = ordemDeCompraObjeto.IDoc;
                                novoItemHistorial.NumeroDocumento = ordemProdutoHistori.Number;
                                novoItemHistorial.NumeroItemDocumento = material.Number;
                                novoItemHistorial.ItemNoDocumento = material.Item;
                                novoItemHistorial.AnoDocumento = materialDocumento.Year.ToInt();
                                novoItemHistorial.SequencialClassificacaoContabil = materialItem.AccountAssignmentSequentialNumber;
                                novoItemHistorial.TipoTransacaoHistoricoOrdem = materialItem.TransactionEventType;
                                novoItemHistorial.CategoriaHistorico = materialItem.Category;
                                novoItemHistorial.TipoMovimento = materialItem.MovementType;
                                novoItemHistorial.DataLancamentoDocumento = materialItem.PostingDate.ToNullableDateTime();
                                novoItemHistorial.Quantidade = materialItem.QuantityMENGE;
                                novoItemHistorial.QuantidadeUnidPrecoOrdem = materialItem.QuantityBAMNG;
                                novoItemHistorial.Valor = materialItem.LocalCurrencyAmount;
                                novoItemHistorial.ValorMoedaDocumento = materialItem.DocumentCurrencyAmount;
                                novoItemHistorial.Moeda = materialItem.CurrencyKey;
                                novoItemHistorial.Operacao = materialItem?.DebitCreditIndicator.ToUpper() ?? "";
                                novoItemHistorial.EntregaConcluida = materialItem.DeliveryCompletedIndicator == "x";
                                novoItemHistorial.DocumentoReferencia = materialItem.ReferenceDocumentNumber;
                                novoItemHistorial.AnoDocumento = materialItem.ReferenceDocumentFiscalYear;
                                novoItemHistorial.NumeroDocumentoDocref = materialItem.ReferenceDocumentNo;
                                novoItemHistorial.DataEntradaDocContabil = materialItem.AccountingDocumentEntryDate.ToNullableDateTime();
                                novoItemHistorial.HoraEntradaDoccontabil = materialItem.EntryTime.ToNullableTime();
                                novoItemHistorial.CodigoImposto = materialItem.SalesPurchasesCodeTax;
                                novoItemHistorial.QtdeUnidadeNota = materialItem.DeliveryNoteMeasureUnitQuantity;
                                novoItemHistorial.UnidadeNota = materialItem.DeliveryNoteMeasureUnit;
                                novoItemHistorial.CodigoProdutoSecundario = materialItem.MaterialNumberEMATN;
                                novoItemHistorial.MoedaSecundaria = materialItem.LocalCurrencykey;
                                novoItemHistorial.DataDocumento = materialItem.DocumentDate.ToNullableDateTime();
                                novoItemHistorial.Usuario = materialItem.CreatorPersonName;
                                novoItemHistorial.Produto = repositorioProdutoEmbarcador.buscarPorCodigoEmbarcador(materialItem.MaterialNumberMatnr);
                                novoItemHistorial.Filial = repositorioFilia.BuscarPorCodigoIntegracao(materialItem.Plant);
                                repositorioOdermDeCompraHistorial.Inserir(novoItemHistorial);
                            }
                        }
                    }
                }
            }

            return Retorno<bool>.CriarRetornoSucesso(true, "Dados recebidos com sucesso");
        }
        #endregion

        #region Metodos Privados
        private void ProcessarDocumentosOrdemCompra(Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraPrincipal ordeCompraPrincipal, List<Dominio.ObjetosDeValor.Embarcador.Produtos.OrdemProduto> listaDocumentos)
        {
            Repositorio.Embarcador.Produtos.OrdemCompraDocumento repositorioOrdemCompraDocumento = new Repositorio.Embarcador.Produtos.OrdemCompraDocumento(_unitOfWork);
            Repositorio.Embarcador.Produtos.UnidadeNegocio repositorioUnidadeNegocio = new Repositorio.Embarcador.Produtos.UnidadeNegocio(_unitOfWork);
            Repositorio.Empresa reposiorioEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Cliente reposiorioCliente = new Repositorio.Cliente(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraDocumento> ordeCompraDocumentos = repositorioOrdemCompraDocumento.BuscarPorCodigoOrdemPrincipal(ordeCompraPrincipal.Codigo);

            if (ordeCompraDocumentos == null || ordeCompraDocumentos.Count == 0)
                ordeCompraDocumentos = new List<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraDocumento>();

            foreach (Dominio.ObjetosDeValor.Embarcador.Produtos.OrdemProduto documento in listaDocumentos)
            {
                Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraDocumento ordemCompraDocumento = ordeCompraDocumentos.Where(d => d.NumeroDocumento == documento.number).FirstOrDefault();

                if (ordemCompraDocumento == null)
                    ordemCompraDocumento = new Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraDocumento();

                ordemCompraDocumento.OrdemDeCompraPrincipal = ordeCompraPrincipal;
                ordemCompraDocumento.NumeroDocumento = documento.number;
                ordemCompraDocumento.Empresa = reposiorioEmpresa.BuscarPorCodigoIntegracao(documento.companyCode);
                ordemCompraDocumento.TipoDocumento = documento.documentType;
                ordemCompraDocumento.CategoriaDocumento = documento.documentCategory;
                ordemCompraDocumento.DocumentoCancelado = string.IsNullOrEmpty(documento.deletionIndicator);
                ordemCompraDocumento.StatusDocumento = documento.status;
                ordemCompraDocumento.DataCriacao = documento.creationDate.ToNullableDateTime("yyyyMMdd");
                ordemCompraDocumento.Fornecedor = reposiorioCliente.BuscarPorCodigoIntegracao(documento.accountNumber);
                ordemCompraDocumento.CondicaoPagamento = documento.paymentKeyTerms;
                ordemCompraDocumento.ValorDesconto = documento.cashDiscountDays;
                ordemCompraDocumento.PercentualDesconto = documento.cashDiscountPercentage;
                ordemCompraDocumento.UnidadeNegocio = repositorioUnidadeNegocio.BuscarPorCodigoIntegracao(documento.purchasingOrganization);
                ordemCompraDocumento.GrupoCompra = documento.purchasingGroup;
                ordemCompraDocumento.Moeda = documento.currencyKey;
                ordemCompraDocumento.TaxaCambio = documento.exchangeRate;
                ordemCompraDocumento.DataDocumento = documento.purchaseDate.ToNullableDateTime("yyyyMMdd");
                ordemCompraDocumento.InicioValidade = documento.startValidityPeriod.ToNullableDateTime("yyyyMMdd");
                ordemCompraDocumento.FinValidade = documento.endValidityPeriod.ToNullableDateTime("yyyyMMdd");
                ordemCompraDocumento.CondicaoDocumento = documento.condition;

                if (ordemCompraDocumento.Codigo > 0)
                    repositorioOrdemCompraDocumento.Atualizar(ordemCompraDocumento);
                else
                    repositorioOrdemCompraDocumento.Inserir(ordemCompraDocumento);

                ProcessarDocumentosProdutos(ordemCompraDocumento, documento.productOrderItems);
                ProcessarDocumentosParceiros(ordemCompraDocumento, documento.partners);
               
            }
        }

        private void ProcessarDocumentosProdutos(Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraDocumento ordemCompraDocumento, List<Dominio.ObjetosDeValor.Embarcador.Produtos.ItemProdutoPedido> produtos)
        {
            if (produtos == null || produtos.Count == 0)
                return;

            Repositorio.Embarcador.Produtos.OrdemCompraDocumento repositorioOrdemCompraDocumento = new Repositorio.Embarcador.Produtos.OrdemCompraDocumento(_unitOfWork);
            Repositorio.Embarcador.Produtos.OrdemCompraItem repositorioOrdemCompraItem = new Repositorio.Embarcador.Produtos.OrdemCompraItem(_unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repositorioProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraItem> ordemCompraProdutosItens = repositorioOrdemCompraItem.BuscarPorCodigoOrdemDocumento(ordemCompraDocumento.Codigo);
            ordemCompraDocumento.Filial = produtos != null && produtos.Count > 0 && produtos.Any(x => !string.IsNullOrEmpty(x.plant)) ? repositorioFilial.BuscarPorCodigoIntegracao(produtos?.Where(x => !string.IsNullOrEmpty(x.plant)).FirstOrDefault()?.plant) : null;
            repositorioOrdemCompraDocumento.Atualizar(ordemCompraDocumento);

            foreach (var produto in produtos)
            {
                Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraItem existeProdutoItem = ordemCompraProdutosItens.Where(p => p.NumeroItemDocumento == produto.number.ToString()).FirstOrDefault();
                if (existeProdutoItem == null)
                    existeProdutoItem = new Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraItem();

                existeProdutoItem.NumeroItemDocumento = produto.number.ToString();
                existeProdutoItem.DocumentoCancelado = string.IsNullOrEmpty(produto.deletionIndicator);
                existeProdutoItem.Produto = repositorioProdutoEmbarcador.buscarPorCodigoEmbarcador(produto.mateialNumberMatnr);
                existeProdutoItem.EntregaConcluida = string.IsNullOrEmpty(produto.deliveryCompleted);
                existeProdutoItem.ProdutoProduzidoInternamente = string.IsNullOrEmpty(produto.producedInHouse);
                existeProdutoItem.DataAlteracao = produto.changeDate.ToNullableDateTime();
                existeProdutoItem.OrdemDeCompraDocumento = ordemCompraDocumento;
                existeProdutoItem.LimiteTolerancia = produto.overdeliveryTolerance;
                existeProdutoItem.QuantidadeOrdemCompra = produto.purchaseOrderQuantity;

                if (existeProdutoItem.Codigo > 0)
                    repositorioOrdemCompraItem.Atualizar(existeProdutoItem);
                else
                    repositorioOrdemCompraItem.Inserir(existeProdutoItem);

                ProcessarDocumentosItemCondicoes(existeProdutoItem, produto.conditions);
            }
        }

        private void ProcessarDocumentosParceiros(Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraDocumento ordemCompraDocumento, List<OrdemProdutoParceiro> parceriros)
        {
            if (parceriros == null || parceriros.Count == 0)
                return;

            Repositorio.Embarcador.Produtos.OrdemDeCompraParceiro repositorioOrdemCompraParceiro = new Repositorio.Embarcador.Produtos.OrdemDeCompraParceiro(_unitOfWork);
            Repositorio.Cliente reposiorioCliente = new Repositorio.Cliente(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraParceiro> ordemDeCompraParceiros = repositorioOrdemCompraParceiro.BuscarPorCodigoOrdemDocumento(ordemCompraDocumento.Codigo);

            if (ordemDeCompraParceiros != null && ordemDeCompraParceiros.Count == 0)
                ordemDeCompraParceiros = new List<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraParceiro>();

            foreach (var parceiro in parceriros)
            {
                Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraParceiro existeParceiro = ordemDeCompraParceiros.Where(p => p.FuncaoParceiro == parceiro.partnerFunction
                                                                                                                    && parceiro.OutroFornecedor.CodigoIntegracao == p.Fornecedor.CodigoIntegracao)
                                                                                                                    .FirstOrDefault();
                if (existeParceiro == null)
                    existeParceiro = new Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraParceiro();

                existeParceiro.OrdemDeCompraDocumento = ordemCompraDocumento;
                existeParceiro.FuncaoParceiro = parceiro.partnerFunction;
                existeParceiro.Fornecedor = reposiorioCliente.BuscarPorCodigoIntegracao(parceiro.OutroFornecedor.CodigoIntegracao);

                if (existeParceiro.Codigo > 0)
                    repositorioOrdemCompraParceiro.Atualizar(existeParceiro);
                else
                    repositorioOrdemCompraParceiro.Inserir(existeParceiro);
            }
        }

        private void ProcessarDocumentosItemCondicoes(Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraItem produtoItem, List<Dominio.ObjetosDeValor.Embarcador.Produtos.Condicao> condicoes)
        {
            if (condicoes == null || condicoes.Count == 0)
                return;

            Repositorio.Embarcador.Produtos.OrdemDeCompraCondicao repositorioOrdemCompraCondicao = new Repositorio.Embarcador.Produtos.OrdemDeCompraCondicao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraCondicao> ordemCompraCondicoes = repositorioOrdemCompraCondicao.BuscarPorCodigoOrdemItem(produtoItem.Codigo);

            foreach (var condicao in condicoes)
            {
                Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraCondicao existeCondicao = ordemCompraCondicoes.Where(d => d.NumeroCondicao == condicao.number).FirstOrDefault();

                if (existeCondicao == null)
                    existeCondicao = new Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraCondicao();

                existeCondicao.NumeroCondicao = condicao.number;
                existeCondicao.ItemCondicao = condicao.itemNumber;
                existeCondicao.TipoCondicao = condicao.type;
                existeCondicao.NomeTipoCondicao = condicao.typeName;
                existeCondicao.ValorBase = condicao.baseValue;
                existeCondicao.Taxa = condicao.rate;
                existeCondicao.ValorCondicao = condicao.value;
                existeCondicao.AlteradoManualmente = string.IsNullOrEmpty(condicao.changedManually);

                if (existeCondicao.Codigo > 0)
                    repositorioOrdemCompraCondicao.Atualizar(existeCondicao);
                else
                    repositorioOrdemCompraCondicao.Inserir(existeCondicao);

            }

        }
        #endregion
    }
}
