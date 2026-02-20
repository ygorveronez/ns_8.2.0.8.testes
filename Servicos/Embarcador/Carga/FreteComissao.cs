using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga
{
    public class FreteComissao : ServicoBase
    {
        public FreteComissao(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete CalcularFretePorComissao(ref Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Repositorio.UnitOfWork unitOfWork, bool apenasVerificar, bool calculoFreteFilialEmissora, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Frete.TabelaFreteTipoOperacao repTabelaFreteTipoOperacao = new Repositorio.Embarcador.Frete.TabelaFreteTipoOperacao(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete();
            Dominio.ObjetosDeValor.Embarcador.Frete.FreteComissao freteComissao = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteComissao();

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);


            if (carga.Empresa == null)
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                carga.Empresa = repEmpresa.BuscarPorCNPJ(long.Parse(cargaPedidos.FirstOrDefault().Recebedor.CPF_CNPJ.ToString()).ToString("d14"));
            }

            if (carga.Empresa != null)
            {
                freteComissao.Empresa = new
                {
                    Codigo = carga.Empresa.Codigo,
                    Descricao = carga.Empresa.RazaoSocial
                };

                Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);
                Repositorio.Embarcador.Frete.TabelaFreteComissaoProduto repTabelaFreteComissaoProduto = new Repositorio.Embarcador.Frete.TabelaFreteComissaoProduto(unitOfWork);
                Repositorio.Embarcador.Frete.TabelaFreteComissaoGrupoProduto repTabelaFreteComissaoGrupoProduto = new Repositorio.Embarcador.Frete.TabelaFreteComissaoGrupoProduto(unitOfWork);
                freteComissao.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoFreteComissao.FreteValido;
                retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido;

                List<Dominio.ObjetosDeValor.Embarcador.Frete.ProdutoComissao> produtos = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ProdutoComissao>();
                List<Dominio.ObjetosDeValor.Embarcador.Frete.ProdutoComissao> produtosSemPercentual = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ProdutoComissao>();

                List<Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete> composicaoFretes = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete>();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                {
                    cargaPedido.ValorFrete = 0;
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = repCargaPedidoProduto.BuscarPorCargaPedido(cargaPedido.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto in cargaPedidoProdutos)
                    {
                        Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoProduto freteComissaoProduto = null;
                        Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoGrupoProduto freteComissaoGrupoProduto = null;

                        freteComissaoProduto = repTabelaFreteComissaoProduto.BuscarPorProdutoEPessoa(tabelaFrete.Codigo, cargaPedidoProduto.Produto.Codigo, cargaPedido.Pedido.Destinatario.CPF_CNPJ, carga.Empresa.Codigo, DateTime.Now);
                        if (freteComissaoProduto == null && cargaPedido.Pedido.Destinatario.GrupoPessoas != null)
                            freteComissaoProduto = repTabelaFreteComissaoProduto.BuscarPorProdutoEGrupoPessoas(tabelaFrete.Codigo, cargaPedidoProduto.Produto.Codigo, cargaPedido.Pedido.Destinatario.GrupoPessoas.Codigo, carga.Empresa.Codigo, DateTime.Now);

                        if (freteComissaoProduto == null && cargaPedidoProduto.Produto.GrupoProduto != null)
                            freteComissaoGrupoProduto = repTabelaFreteComissaoGrupoProduto.BuscarPorGrupoProdutoEPessoa(tabelaFrete.Codigo, cargaPedidoProduto.Produto.GrupoProduto.Codigo, cargaPedido.Pedido.Destinatario.CPF_CNPJ, carga.Empresa.Codigo, DateTime.Now);

                        if (freteComissaoProduto == null && freteComissaoGrupoProduto == null && cargaPedido.Pedido.Destinatario.GrupoPessoas != null && cargaPedidoProduto.Produto.GrupoProduto != null)
                            freteComissaoGrupoProduto = repTabelaFreteComissaoGrupoProduto.BuscarPorGrupoProdutoEGrupoPessoas(tabelaFrete.Codigo, cargaPedidoProduto.Produto.GrupoProduto.Codigo, cargaPedido.Pedido.Destinatario.GrupoPessoas.Codigo, carga.Empresa.Codigo, DateTime.Now);

                        if (freteComissaoProduto == null && freteComissaoGrupoProduto == null)
                            freteComissaoProduto = repTabelaFreteComissaoProduto.BuscarPorProduto(tabelaFrete.Codigo, cargaPedidoProduto.Produto.Codigo, carga.Empresa.Codigo, DateTime.Now);

                        if (freteComissaoProduto == null && freteComissaoGrupoProduto == null && cargaPedidoProduto.Produto.GrupoProduto != null)
                            freteComissaoGrupoProduto = repTabelaFreteComissaoGrupoProduto.BuscarPorGrupoProduto(tabelaFrete.Codigo, cargaPedidoProduto.Produto.GrupoProduto.Codigo, carga.Empresa.Codigo, DateTime.Now);

                        Dominio.ObjetosDeValor.Embarcador.Frete.ProdutoComissao produtoComissao = new Dominio.ObjetosDeValor.Embarcador.Frete.ProdutoComissao();
                        produtoComissao.Codigo = cargaPedidoProduto.Produto.CodigoProdutoEmbarcador;
                        produtoComissao.Descricao = cargaPedidoProduto.Produto.Descricao;

                        if (freteComissaoProduto != null || freteComissaoGrupoProduto != null)
                        {
                            decimal valorTotalProduto = (cargaPedidoProduto.Quantidade * cargaPedidoProduto.ValorUnitarioProduto);
                            decimal valorPercentualProduto = 0m;

                            if (freteComissaoProduto != null)
                            {
                                valorPercentualProduto = (freteComissaoProduto.PercentualValorProduto / 100) * valorTotalProduto;
                                composicaoFretes.Add(Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Percentual sobre Produto * Valor do Produto ", freteComissaoProduto.PercentualValorProduto.ToString("n2") + "% * " + valorTotalProduto.ToString("n2"), freteComissaoProduto.PercentualValorProduto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ValorFreteLiquido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor, "Percentual sobre o produto " + freteComissaoProduto.ProdutoEmbarcador.CodigoProdutoEmbarcador + " - " + freteComissaoProduto.ProdutoEmbarcador.Descricao, 0, valorPercentualProduto));
                            }
                            else
                            {
                                valorPercentualProduto = (freteComissaoGrupoProduto.PercentualValorProduto / 100) * valorTotalProduto;
                                composicaoFretes.Add(Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Percentual sobre Grupo de Produto * Valor do Produto ", freteComissaoGrupoProduto.PercentualValorProduto.ToString("n2") + "% * " + valorTotalProduto.ToString("n2"), freteComissaoGrupoProduto.PercentualValorProduto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ValorFreteLiquido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor, "Percentual sobre o Grupo de Produto " + freteComissaoGrupoProduto.GrupoProduto.CodigoGrupoProdutoEmbarcador + " - " + freteComissaoGrupoProduto.GrupoProduto.Descricao, 0, valorPercentualProduto));
                            }

                            valorPercentualProduto = Math.Round(valorPercentualProduto, 2, MidpointRounding.AwayFromZero);

                            if (!calculoFreteFilialEmissora)
                                cargaPedido.ValorFrete += valorPercentualProduto;
                            else
                                cargaPedido.ValorFreteFilialEmissora += valorPercentualProduto;

                            produtoComissao.Valor = valorPercentualProduto;
                            produtoComissao.PagoPorTonelada = false;
                            produtoComissao.Quantidade = cargaPedidoProduto.Quantidade;
                            if (!produtos.Contains(produtoComissao))
                            {
                                produtos.Add(produtoComissao);
                            }
                            else
                            {
                                Dominio.ObjetosDeValor.Embarcador.Frete.ProdutoComissao temp = produtos.Find(obj => obj.Codigo == produtoComissao.Codigo);
                                temp.Quantidade += produtoComissao.Quantidade;
                                temp.Valor += produtoComissao.Valor;
                            }
                        }
                        else
                        {
                            if (!produtosSemPercentual.Contains(produtoComissao))
                                produtosSemPercentual.Add(produtoComissao);

                            freteComissao.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoFreteComissao.ProdutoSemComissao;
                            cargaPedido.ValorFrete = 0;
                        }
                    }
                }
                if (freteComissao.situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoFreteComissao.FreteValido)
                {
                    Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicaoFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete();

                    decimal adicionarCarroceria = Frete.CalcularValorFreteAdicionalPorModeloCarroceriaVeiculo(carga.Veiculo, carga.VeiculosVinculados, ref composicaoFrete, null, carga.ValorFrete);
                    if (adicionarCarroceria > 0)
                    {
                        composicaoFrete.TipoParametro = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ValorFreteLiquido;
                        composicaoFretes.Add(composicaoFrete);
                        carga.ValorFrete += adicionarCarroceria;
                    }

                    if (!apenasVerificar)
                    {
                        if (!calculoFreteFilialEmissora)
                            carga.TabelaFrete = tabelaFrete;
                        else
                            carga.TabelaFreteFilialEmissora = tabelaFrete;

                        if (tabelaFrete != null && (tabelaFrete.UtilizaModeloVeicularVeiculo || tabelaFrete.UtilizarModeloVeicularDaCargaParaCalculo))
                        {
                            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCalculo = null;
                            if (carga.VeiculosVinculados != null && carga.VeiculosVinculados.Count > 0 && carga.VeiculosVinculados.FirstOrDefault().ModeloVeicularCarga != null)
                                modeloVeicularCalculo = carga.VeiculosVinculados.FirstOrDefault().ModeloVeicularCarga;
                            else if (carga.Veiculo != null && carga.Veiculo.ModeloVeicularCarga != null)
                                modeloVeicularCalculo = carga.Veiculo.ModeloVeicularCarga;
                            if (modeloVeicularCalculo != null)
                                carga.ModeloVeicularCarga = modeloVeicularCalculo;
                        }

                        Servicos.Embarcador.Carga.RateioFrete serRateioFrete = new RateioFrete(unitOfWork);
                        serRateioFrete.RatearValorDoFrenteEntrePedidos(carga, cargaPedidos, configuracao, calculoFreteFilialEmissora, unitOfWork, tipoServicoMultisoftware);
                        Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.SetarComposicaoFrete(carga, null, null, null, calculoFreteFilialEmissora, composicaoFretes, unitOfWork, null);
                    }

                    freteComissao.produtos = produtos;
                    freteComissao.Tabela = carga.TabelaFrete.Descricao;

                    if (!calculoFreteFilialEmissora)
                    {
                        bool destacarComponenteTabelaFrete = Servicos.Embarcador.Carga.Frete.DestacarComponenteTabelaFrete(carga.TabelaFrete, carga.ContratoFreteTransportador?.ComponenteFreteValorContrato);
                        bool descontarComponenteFreteLiquido = (destacarComponenteTabelaFrete ? carga.TabelaFrete?.DescontarComponenteFreteLiquido : carga.ContratoFreteTransportador?.ComponenteFreteValorContrato?.DescontarComponenteFreteLiquido) ?? false;

                        retorno.valorFrete = carga.ValorFrete;
                        retorno.ValorFreteLiquido = carga.ValorFreteLiquido + (((carga.ContratoFreteTransportador?.ComponenteFreteValorContrato?.SomarComponenteFreteLiquido ?? false) || descontarComponenteFreteLiquido) ? 0m : carga.ValorFreteContratoFreteTotal);
                        retorno.valorFreteAPagar = carga.ValorFreteAPagar;
                        retorno.ValorFreteNegociado = carga.ValorFreteNegociado;
                        retorno.valorFreteTabelaFrete = carga.ValorFreteAPagar;
                        retorno.valorICMS = carga.ValorICMS;
                        retorno.valorISS = carga.ValorISS;
                        retorno.ValorRetencaoISS = carga.ValorRetencaoISS;
                        retorno.valorFreteAPagarComICMSeISS = carga.ValorTotalAReceberComICMSeISS;
                        retorno.aliquotaICMS = repCargaPedido.BuscarMediaAliquotaICMSdaCarga(carga.Codigo);
                        retorno.csts = repCargaPedido.BuscarCSTICMSdaCarga(carga.Codigo);
                        retorno.taxaDocumentacao = Servicos.Embarcador.Carga.Frete.RetornarTaxaDocumental(carga);
                        retorno.aliquotaISS = repCargaPedido.BuscarMediaAliquotaISSdaCarga(carga.Codigo);
                        retorno.valorMercadoria = repPedidoXMLNotaFiscal.ObterValorTotalPorCarga(carga.Codigo);
                        retorno.peso = repPedidoXMLNotaFiscal.ObterPesoTotalPorCarga(carga.Codigo);
                        retorno.Moeda = carga.Moeda ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real;
                        retorno.ValorCotacaoMoeda = carga.ValorCotacaoMoeda ?? 0m;
                        retorno.ValorTotalMoeda = carga.ValorTotalMoeda ?? 0m;
                        retorno.ValorTotalMoedaPagar = carga.ValorTotalMoedaPagar ?? 0m;
                        retorno.CustoFrete = carga.DadosSumarizados?.CustoFrete ?? string.Empty;
                        retorno.PercentualBonificacaoTransportador = carga.PercentualBonificacaoTransportador;
                        retorno.DescricaoBonificacaoTransportador = (carga.PercentualBonificacaoTransportador != 0m) ? carga.BonificacaoTransportador?.ComponenteFrete?.Descricao ?? string.Empty : string.Empty;
                    }
                    else
                    {
                        bool destacarComponenteTabelaFrete = Servicos.Embarcador.Carga.Frete.DestacarComponenteTabelaFrete(carga.TabelaFrete, carga.ContratoFreteTransportador?.ComponenteFreteValorContrato);
                        bool descontarComponenteFreteLiquido = (destacarComponenteTabelaFrete ? carga.TabelaFrete?.DescontarComponenteFreteLiquido : carga.ContratoFreteTransportador?.ComponenteFreteValorContrato?.DescontarComponenteFreteLiquido) ?? false;

                        retorno.valorFrete = carga.ValorFreteFilialEmissora;
                        retorno.ValorFreteLiquido = carga.ValorFreteLiquido + (((carga.ContratoFreteTransportador?.ComponenteFreteValorContrato?.SomarComponenteFreteLiquido ?? false) || descontarComponenteFreteLiquido) ? 0m : carga.ValorFreteContratoFreteTotal);
                        retorno.valorFreteAPagar = carga.ValorFreteAPagarFilialEmissora;
                        retorno.valorFreteTabelaFrete = carga.ValorFreteAPagarFilialEmissora;
                        retorno.valorICMS = carga.ValorICMSFilialEmissora;
                        retorno.valorISS = carga.ValorISS;
                        retorno.ValorRetencaoISS = carga.ValorRetencaoISS;
                        retorno.valorFreteAPagarComICMSeISS = carga.ValorTotalAReceberComICMSeISSFilialEmissora;
                    }

                    ComponetesFrete serComponentesFrete = new ComponetesFrete(unitOfWork);
                    serComponentesFrete.BuscarComponentesDeFreteDaCarga(ref retorno, carga, calculoFreteFilialEmissora, unitOfWork, tipoServicoMultisoftware);

                }
                else
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                        cargaPedido.ValorFrete = 0;

                    retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete;
                    freteComissao.produtos = produtosSemPercentual;
                }
            }
            else
            {
                freteComissao.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoFreteComissao.TransportadorNaoCadastrado;
                retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete;
            }

            retorno.dadosRetornoTipoFrete = freteComissao;
            return retorno;


        }

    }
}