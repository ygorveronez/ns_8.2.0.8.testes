using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.WebService.Financeiro
{
    public class DocumentoEntrada : ServicoBase
    {        
        public DocumentoEntrada(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #region Métodos Públicos

        public Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoEntrada ConverterObjetoDocumentoEntrada(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unitOfWork);

            Pessoas.Pessoa serPessoa = new Pessoas.Pessoa(unitOfWork);
            Empresa.Empresa serEmpresa = new Empresa.Empresa(unitOfWork);
            Servicos.WebService.Financeiro.TipoMovimento servicoTipoMovimento = new Servicos.WebService.Financeiro.TipoMovimento(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoEntrada documentoEntrada = new Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoEntrada()
            {
                Chave = documento.Chave,
                DataEmissao = documento.DataEmissao,
                DataEntrada = documento.DataEntrada,
                Destinatario = serEmpresa.ConverterObjetoEmpresa(documento.Destinatario),
                Fornecedor = serPessoa.ConverterObjetoPessoa(documento.Fornecedor),
                Numero = documento.Numero,
                OrdemCompra = ConverterObjetoOrdemCompra(documento.OrdemCompra, unitOfWork),
                Produtos = ConverterProdutosDocumentoEntrada(documento.Codigo, unitOfWork),
                Serie = documento.Serie,
                ValorBruto = documento.ValorBruto,
                ValorProdutos = documento.ValorProdutos,
                ValorTotal = documento.ValorTotal,
                ValorTotalDesconto = documento.ValorTotalDesconto,
                Protocolo = documento.Codigo,
                TipoMovimento = servicoTipoMovimento.ConverterObjetoTipoMovimento(documento.TipoMovimento, unitOfWork),
                NaturezaOperacao = this.ConverterObjetoNaturezaDaOperacao(documento.NaturezaOperacao)
            };

            return documentoEntrada;
        }

        public Dominio.Entidades.Embarcador.Compras.OrdemCompra SalvarOrdemCompra(Dominio.ObjetosDeValor.Embarcador.Financeiro.OrdemCompra ordemCompra, Dominio.Entidades.Empresa empresaIntegradora, ref string mensagem, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Servicos.Cliente servicoCliente = new Servicos.Cliente(StringConexao);
            Servicos.WebService.Frota.Veiculo serVeiculo = new Servicos.WebService.Frota.Veiculo(unitOfWork);

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Compras.OrdemCompra repOrdemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(unitOfWork);
            Repositorio.Embarcador.Compras.OrdemCompraMercadoria repOrdemCompraMercadoria = new Repositorio.Embarcador.Compras.OrdemCompraMercadoria(unitOfWork);

            mensagem = "";
            if (string.IsNullOrWhiteSpace(ordemCompra.StrData))
                mensagem += "É obrigatório informar a Data. ";
            else
                ordemCompra.Data = ordemCompra.StrData.ToDateTime();

            if (string.IsNullOrWhiteSpace(ordemCompra.StrDataPrevisaoRetorno))
                mensagem += "É obrigatório informar a Data. ";
            else
                ordemCompra.DataPrevisaoRetorno = ordemCompra.StrDataPrevisaoRetorno.ToDateTime();

            if (ordemCompra.Data == DateTime.MinValue)
                mensagem += "É obrigatório informar a Data. ";
            if (ordemCompra.DataPrevisaoRetorno == DateTime.MinValue)
                mensagem += "É obrigatório informar a Data de Previsão. ";
            if (ordemCompra.MotivoCompra == null)
                mensagem += "É obrigatório informar o Motivo da Compra. ";
            if (ordemCompra.Produtos == null || ordemCompra.Produtos.Count == 0)
                mensagem += "É obrigatório informar ao menos um produto. ";

            if (!string.IsNullOrWhiteSpace(mensagem))
                return null;

            Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoConversaoFornecedor = null;
            Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoConversaoTransportador = null;

            if (ordemCompra.Fornecedor != null)
            {
                retornoConversaoFornecedor = servicoCliente.ConverterObjetoValorPessoa(ordemCompra.Fornecedor, "Fornecedor", unitOfWork, 0, false, false, null, tipoServicoMultisoftware, false, true);
                if (retornoConversaoFornecedor.Status == false)
                {
                    mensagem += retornoConversaoFornecedor.Mensagem;
                    return null;
                }
            }
            if (ordemCompra.Transportador != null)
            {
                retornoConversaoTransportador = servicoCliente.ConverterObjetoValorPessoa(ordemCompra.Transportador, "Transportador", unitOfWork, 0, false, false, null, tipoServicoMultisoftware, false, true);
                if (retornoConversaoTransportador.Status == false)
                {
                    mensagem += retornoConversaoTransportador.Mensagem;
                    return null;
                }
            }

            Dominio.Entidades.Embarcador.Compras.OrdemCompra ordem = new Dominio.Entidades.Embarcador.Compras.OrdemCompra()
            {
                CondicaoPagamento = ordemCompra.CondicaoPagamento,
                Data = ordemCompra.Data,
                DataPrevisaoRetorno = ordemCompra.DataPrevisaoRetorno,
                Fornecedor = retornoConversaoFornecedor?.cliente ?? null,
                Empresa = empresaIntegradora,
                MotivoCompra = ConverterEntidadeMotivoCompra(ordemCompra.MotivoCompra, unitOfWork),
                Numero = repOrdemCompra.BuscarProximoNumero(empresaIntegradora?.Codigo ?? 0),
                Observacao = ordemCompra.Observacao,
                Situacao = SituacaoOrdemCompra.Aberta,
                Transportador = retornoConversaoTransportador?.cliente ?? null,
                Veiculo = serVeiculo.SalvarVeiculo(ordemCompra.Veiculo, empresaIntegradora, false, ref mensagem, unitOfWork, tipoServicoMultisoftware)
            };

            if (!string.IsNullOrWhiteSpace(mensagem))
                return null;

            repOrdemCompra.Inserir(ordem, auditado);

            foreach (var produto in ordemCompra.Produtos)
            {
                Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria merc = new Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria()
                {
                    OrdemCompra = ordem,
                    Produto = ConverterEntidadeProduto(produto, empresaIntegradora?.Codigo ?? 0, unitOfWork, auditado, tipoServicoMultisoftware),
                    Quantidade = produto.Quantidade,
                    QuantidadePendente = produto.Quantidade,
                    ValorUnitario = produto.ValorUnitario
                };

                if (merc.Produto == null)
                {
                    mensagem += "Produto " + produto.DescricaoProduto + " não localizado";
                    return null;
                }

                repOrdemCompraMercadoria.Inserir(merc);
            }

            Dominio.Entidades.Embarcador.Compras.OrdemCompra ordemCompraFinalizar = repOrdemCompra.BuscarPorCodigo(ordem.Codigo);
            Servicos.Embarcador.Compras.OrdemCompra.EtapaAprovacao(ref ordemCompraFinalizar, unitOfWork, tipoServicoMultisoftware, unitOfWork.StringConexao, auditado, empresaIntegradora?.Codigo ?? 0);

            return ordem;
        }


        #endregion

        #region Métodos Privados



        public Dominio.ObjetosDeValor.Embarcador.Financeiro.OrdemCompra ConverterObjetoOrdemCompra(Dominio.Entidades.Embarcador.Compras.OrdemCompra ordemCompra, Repositorio.UnitOfWork unitOfWork)
        {
            if (ordemCompra == null)
                return null;

            Pessoas.Pessoa serPessoa = new Pessoas.Pessoa(unitOfWork);
            Servicos.WebService.Frota.Veiculo serVeiculo = new WebService.Frota.Veiculo(unitOfWork);

            return new Dominio.ObjetosDeValor.Embarcador.Financeiro.OrdemCompra()
            {
                CondicaoPagamento = ordemCompra.CondicaoPagamento,
                Data = ordemCompra.Data,
                DataPrevisaoRetorno = ordemCompra.DataPrevisaoRetorno,
                Fornecedor = serPessoa.ConverterObjetoPessoa(ordemCompra.Fornecedor),
                MotivoCompra = ConverterObjetoMotivoCompra(ordemCompra.MotivoCompra),
                Observacao = ordemCompra.Observacao,
                Produtos = ConverterProdutosOrdemCompra(ordemCompra.Codigo, unitOfWork),
                Transportador = serPessoa.ConverterObjetoPessoa(ordemCompra.Transportador),
                Veiculo = serVeiculo.ConverterObjetoVeiculo(ordemCompra.Veiculo, unitOfWork)
            };
        }

        public Dominio.Entidades.Embarcador.Compras.MotivoCompra ConverterEntidadeMotivoCompra(Dominio.ObjetosDeValor.Embarcador.Financeiro.MotivoCompra motivoCompra, Repositorio.UnitOfWork unitOfWork)
        {
            if (motivoCompra == null)
                return null;

            Repositorio.Embarcador.Compras.MotivoCompra repMotivoCompra = new Repositorio.Embarcador.Compras.MotivoCompra(unitOfWork);
            Dominio.Entidades.Embarcador.Compras.MotivoCompra motivo = repMotivoCompra.BuscarPorCodigoIntegracao(motivoCompra.CodigoIntegracao);

            if (motivo != null)
                return motivo;
            else
            {
                motivo = new Dominio.Entidades.Embarcador.Compras.MotivoCompra()
                {
                    CodigoIntegracao = motivoCompra.CodigoIntegracao,
                    Descricao = motivoCompra.Descricao,
                    Ativo = true,
                    Forma = FormaRequisicaoMercadoria.Compra
                };
                repMotivoCompra.Inserir(motivo);
                return motivo;
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.Financeiro.MotivoCompra ConverterObjetoMotivoCompra(Dominio.Entidades.Embarcador.Compras.MotivoCompra motivoCompra)
        {
            if (motivoCompra == null)
                return null;

            return new Dominio.ObjetosDeValor.Embarcador.Financeiro.MotivoCompra()
            {
                Codigo = motivoCompra.Codigo,
                CodigoIntegracao = motivoCompra.Codigo.ToString("D"),
                Descricao = motivoCompra.Descricao
            };
        }

        public Dominio.Entidades.Produto ConverterEntidadeProduto(Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produto, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoNCMAbastecimento repProdutoNCMAbastecimento = new Repositorio.Embarcador.Produtos.ProdutoNCMAbastecimento(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            if (produto == null)
                return null;

            Dominio.Entidades.Produto prod = repProduto.BuscarPorCodigo(produto.Codigo);
            if (prod == null)
            {
                prod = repProduto.BuscarPorCodigoProduto(prod.CodigoProduto);
                if (prod == null)
                {
                    List<Dominio.Entidades.Embarcador.Produtos.ProdutoNCMAbastecimento> ncmsAbastecimento = repProdutoNCMAbastecimento.BuscarNCMsAtivos();
                    prod = new Dominio.Entidades.Produto()
                    {
                        Descricao = produto.DescricaoProduto,
                        DescricaoNotaFiscal = produto.DescricaoProduto,
                        CodigoBarrasEAN = produto.CodigoEAN,
                        CodigoProduto = produto.CodigoProduto,
                        CodigoNCM = produto.CodigoNCM,
                        UnidadeDeMedida = UnidadeDeMedidaHelper.ObterUnidade(!string.IsNullOrWhiteSpace(produto.UnidadeMedida) ? produto.UnidadeMedida.ToUpper() : "UN"),
                        Status = "A",
                        CategoriaProduto = CategoriaProduto.MercadoriaRevenda,
                        CSTIPIVenda = 0,
                        CSTIPICompra = 0,
                        OrigemMercadoria = OrigemMercadoria.Origem0,
                        GeneroProduto = GeneroProduto.Genero0,
                        IndicadorEscalaRelevante = IndicadorEscalaRelevante.Nenhum,
                        GerarPneuAutomatico = false,
                        ProdutoEPI = false,
                        ProdutoCombustivel = false,
                        ProdutoKIT = false,
                        ProdutoBem = false
                    };

                    if (ncmsAbastecimento != null && ncmsAbastecimento.Count() > 0 && !string.IsNullOrWhiteSpace(produto.CodigoNCM) && !prod.ProdutoCombustivel.Value)
                    {
                        if (ncmsAbastecimento.Where(o => produto.CodigoNCM.Contains(o.NCM)).Count() > 0)
                            prod.ProdutoCombustivel = true;
                        else
                            prod.ProdutoCombustivel = false;
                    }
                    else
                        prod.ProdutoCombustivel = false;

                    prod.CalculoCustoProduto = Servicos.Embarcador.Produto.Custo.ObterFormulaPadrao(unitOfWork);

                    if (produto.Codigo == 0)
                        prod.Empresa = codigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresa) : null;

                    repProduto.Inserir(prod, auditado);

                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                    Servicos.Embarcador.Produto.Estoque servicoEstoque = new Servicos.Embarcador.Produto.Estoque(unitOfWork);
                    servicoEstoque.AdicionarEstoque(prod, null, tipoServicoMultisoftware, configuracaoTMS);
                }
            }

            return prod;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto> ConverterProdutosOrdemCompra(int codigoOrdemCompra, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto> produtos = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto>();

            Repositorio.Embarcador.Compras.OrdemCompraMercadoria repOrdemCompraMercadoria = new Repositorio.Embarcador.Compras.OrdemCompraMercadoria(unitOfWork);
            List<Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria> mercadorias = repOrdemCompraMercadoria.BuscarPorOrdem(codigoOrdemCompra);

            foreach (var mercadoria in mercadorias)
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produto = new Dominio.ObjetosDeValor.Embarcador.Pedido.Produto()
                {
                    Quantidade = mercadoria.Quantidade,
                    ValorUnitario = mercadoria.ValorUnitario,
                    CodigoProduto = mercadoria.Produto.CodigoProduto,
                    DescricaoProduto = mercadoria.Produto.Descricao,
                    Codigo = mercadoria.Produto.Codigo,
                    CodigoNCM = mercadoria.Produto.CodigoNCM,
                    UnidadeMedida = mercadoria.Produto.UnidadeDeMedida.HasValue ? mercadoria.Produto.UnidadeDeMedida.Value.ObterSigla() : "UN",
                    ValorTotal = mercadoria.ValorTotal
                };
                produtos.Add(produto);
            }
            return produtos;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto> ConverterProdutosDocumentoEntrada(int codigoDocumentoEntrada, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto> produtos = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto>();

            Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repDocumentoEntradaItem = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(unitOfWork);
            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> mercadorias = repDocumentoEntradaItem.BuscarPorDocumentoEntrada(codigoDocumentoEntrada);

            foreach (var mercadoria in mercadorias)
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produto = new Dominio.ObjetosDeValor.Embarcador.Pedido.Produto()
                {
                    Quantidade = mercadoria?.Quantidade ?? 0m,
                    ValorUnitario = mercadoria?.ValorUnitario ?? 0m,
                    CodigoProduto = mercadoria.Produto?.CodigoProduto ?? string.Empty,
                    DescricaoProduto = mercadoria.Produto?.Descricao ?? string.Empty,
                    Codigo = mercadoria.Produto?.Codigo ?? 0,
                    CodigoNCM = mercadoria.Produto?.CodigoNCM ?? string.Empty,
                    UnidadeMedida = mercadoria?.UnidadeMedida.ObterSigla() ?? string.Empty,
                    ValorTotal = mercadoria?.ValorTotal ?? 0m,
                    ValorDesconto = mercadoria?.Desconto ?? 0m
                };
                produtos.Add(produto);
            }
            return produtos;
        }

        public Dominio.ObjetosDeValor.Embarcador.Financeiro.NaturezaDaOperacao ConverterObjetoNaturezaDaOperacao(Dominio.Entidades.NaturezaDaOperacao naturezaDaOperacao)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Financeiro.NaturezaDaOperacao()
            {
                Numero = naturezaDaOperacao?.Numero ?? 0,
                Protocolo = naturezaDaOperacao?.Codigo ?? 0,
                Descricao = naturezaDaOperacao?.Descricao ?? string.Empty
            };
        }
        #endregion
    }
}
