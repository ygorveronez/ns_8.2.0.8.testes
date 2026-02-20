using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Servicos.Embarcador.Pedido
{
    public class Produto : ServicoBase
    {
        #region Construtores

        public Produto(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }
        #endregion

        #region Propriedades Privadas

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoTMS;

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoTMS(Repositorio.UnitOfWork unitOfWork)
        {
            if (_configuracaoTMS == null)
                _configuracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoTMS;
        }

        public string BuscarProdutoPredominanteEntreOsPedidos(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);

            string produtoPredominate = "";
            decimal MaiorPeso = 0;

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = repCargaPedidoProduto.BuscarPorCargaPedido(cargaPedido.Codigo);
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto in cargaPedidoProdutos)
                {
                    decimal pesoUnitario = cargaPedidoProduto.PesoUnitario > 0 ? cargaPedidoProduto.PesoUnitario : 1;
                    decimal peso = cargaPedidoProduto.PesoTotalEmbalagem + (pesoUnitario * cargaPedidoProduto.Quantidade);
                    if (peso >= MaiorPeso)
                    {
                        produtoPredominate = cargaPedidoProduto.Produto.Descricao;
                        MaiorPeso = peso;
                    }
                }

            }
            return produtoPredominate;

        }

        public string BuscarProdutoPredominanteEntreOsPedidos(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);

            string produtoPredominate = "";
            decimal MaiorPeso = 0;

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = repCargaPedidoProduto.BuscarPorCargaPedido(cargaPedido.Codigo);
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto in cargaPedidoProdutos)
            {
                decimal pesoUnitario = cargaPedidoProduto.PesoUnitario > 0 ? cargaPedidoProduto.PesoUnitario : cargaPedidoProduto.PesoUnitario;
                decimal peso = cargaPedidoProduto.PesoTotalEmbalagem + (pesoUnitario * cargaPedidoProduto.Quantidade);
                if (peso >= MaiorPeso)
                {
                    produtoPredominate = cargaPedidoProduto.Produto.Descricao;
                }
            }
            return produtoPredominate;

        }

        public void AtualizarProdutosCargaPedidoPorNotaFiscal(List<Dominio.ObjetosDeValor.Embarcador.NFe.Produtos> produtos, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidosProdutosCarga = null, List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> produtosEmbarcadorCarga = null)
        {
            // Problema NaturalOne aonde a NF processada nao possui os Produtos, sendo assim de acordo com as
            // configurações está voltando saldo no pedido para novas roteirizações.
            // Tipo de Operação AtualizarSaldoPedidoProdutosPorXmlNotaFiscal = True
            //Como não tinha produtos.. removia do cargaPedido
            if ((produtos?.Count ?? 0) == 0)
                return;

            Servicos.ProdutoEmbarcador servicoProdutoEmbarcador = new Servicos.ProdutoEmbarcador(unitOfWork);

            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto repCarregamentoPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidosProdutos = cargaPedidosProdutosCarga?.Count() > 0 ? cargaPedidosProdutosCarga.Where(o => o.CargaPedido.Codigo == cargaPedido.Codigo).ToList() : repCargaPedidoProduto.BuscarPorCargaPedido(cargaPedido.Codigo);

            List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto> produtosEmbarcador = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto>();

            cargaPedido.ValorMercadoriaDescontar = 0;
            cargaPedido.PesoMercadoriaDescontar = 0;

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = ObterConfiguracaoTMS(unitOfWork);

            bool atualizarSaldoPedidoProdutosPorXmlNotaFiscal = cargaPedido.Carga.TipoOperacao?.AtualizarSaldoPedidoProdutosPorXmlNotaFiscal ?? false;

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidosDuplicados = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto>();

            foreach (Dominio.ObjetosDeValor.Embarcador.NFe.Produtos produtoNFe in produtos)
            {
                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcadorExiste = null;

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> existentes = (from obj in cargaPedidosProdutos where obj.Produto.CodigoProdutoEmbarcador == produtoNFe.Codigo select obj).ToList();

                Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoprodutoExiste = null;
                if (existentes.Count > 0)
                {
                    cargaPedidoprodutoExiste = existentes.FirstOrDefault();

                    for (int i = 0; i < existentes.Count; i++)
                    {
                        if (i == 0)
                            cargaPedidoprodutoExiste = existentes[i];
                        else
                            cargaPedidosDuplicados.Add(existentes[i]);
                    }
                }

                decimal pesoUnitario = 0;

                if (cargaPedidoprodutoExiste != null)
                    pesoUnitario = cargaPedidoprodutoExiste.PesoUnitario;
                else
                {
                    produtoEmbarcadorExiste = produtosEmbarcadorCarga?.Count() > 0 ? produtosEmbarcadorCarga.Where(o => o.CodigoProdutoEmbarcador == produtoNFe.Codigo).FirstOrDefault() : repProdutoEmbarcador.buscarPorCodigoEmbarcador(produtoNFe.Codigo);

                    if (produtoEmbarcadorExiste != null)
                        pesoUnitario = produtoEmbarcadorExiste.PesoUnitario;
                }

                Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produtoEmbarcador = new Dominio.ObjetosDeValor.Embarcador.Pedido.Produto
                {
                    CodigoProduto = produtoNFe.Codigo,
                    DescricaoProduto = produtoNFe.Descricao,
                    PesoUnitario = pesoUnitario,
                    Quantidade = produtoNFe.QuantidadeComercial,
                    ValorUnitario = produtoNFe.ValorUnitarioComercial
                };

                produtosEmbarcador.Add(produtoEmbarcador);
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto in cargaPedidosDuplicados)
            {
                cargaPedidosProdutos.Remove(cargaPedidoProduto);

                repCargaPedidoProduto.Deletar(cargaPedidoProduto);
            }

            List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> produtosEmbarcadorSalvos = new List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();
            if (!configuracao.SempreInserirNovoProdutoPorIntegracao)
            {
                List<string> codigosProdutosEmbarcador = produtosEmbarcador.Select(obj => obj.CodigoProduto).Distinct().ToList();

                if (produtosEmbarcadorCarga?.Count() > 0)
                {
                        produtosEmbarcadorSalvos = produtosEmbarcadorCarga.Where(o => codigosProdutosEmbarcador.Contains(o.CodigoProdutoEmbarcador)).ToList();
                }
                else
                {
                    if (codigosProdutosEmbarcador.Count < 2000)
                        produtosEmbarcadorSalvos = repProdutoEmbarcador.buscarPorCodigosEmbarcador((from obj in produtosEmbarcador select obj.CodigoProduto).Distinct().ToList());
                    else
                    {
                        decimal decimalBlocos = Math.Ceiling(((decimal)codigosProdutosEmbarcador.Count) / 1000);
                        int blocos = (int)Math.Truncate(decimalBlocos);

                        for (int i = 0; i < blocos; i++)
                        {
                            List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> lstProdutoEmbarcador = repProdutoEmbarcador.buscarPorCodigosEmbarcador((from obj in produtosEmbarcador.Skip(i * 1000).Take(1000) select obj.CodigoProduto).Distinct().ToList());
                            produtosEmbarcadorSalvos.AddRange(lstProdutoEmbarcador);
                        }
                    }
                }
            }

            // Filtrando apenas os produtos que estão na carga e não estão na NF para excluir
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutosExcluir = cargaPedidosProdutos.FindAll(x => !produtosEmbarcador.Exists(p => p.CodigoProduto == x.Produto.CodigoProdutoEmbarcador));
            decimal pesoCargaPedidoProdutosExcluidos = cargaPedidoProdutosExcluir.Sum(x => x.PesoTotal);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto in cargaPedidoProdutosExcluir)
                repCargaPedidoProduto.Deletar(cargaPedidoProduto);

            if (configuracao.AtualizarProdutosCarregamentoPorNota)
            {
                //TODO: PPC - Adicionado log temporário para identificar problema no cargaPedido.Peso.
                Servicos.Log.TratarErro($"Pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} - CargaPedido.Codigo = {cargaPedido.Codigo} - Peso Total De.: {cargaPedido.Peso} - Para.: {cargaPedido.Peso - pesoCargaPedidoProdutosExcluidos}. Produto.AtualizarProdutosCargaPedidoPorNotaFiscal", "PesoCargaPedido");
                cargaPedido.Peso -= pesoCargaPedidoProdutosExcluidos;
            }

            int codigoPedido = (cargaPedido.Pedido?.Codigo ?? 0);
            int codigoCarregamento = (cargaPedido.Carga?.Carregamento?.Codigo ?? 0);

            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido pedidoCarregamento = null;
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> pedidoProdutosCarregamento = null;
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> pedidoProdutosCarregamentoExcluir = null;

            if (codigoCarregamento > 0 && configuracao.AtualizarProdutosCarregamentoPorNota)
            {
                pedidoCarregamento = repCarregamentoPedido.BuscarPorCarregamentoEPedido(codigoPedido, codigoCarregamento);
                // Agora o carregamento pedido produto..
                pedidoProdutosCarregamento = repCarregamentoPedidoProduto.BuscarPorCarregamentoPedidoProdutos(codigoCarregamento, codigoPedido);

                //Alimentando a lista de produtos do carregamento no qual não vieram na NF.. da carga pedido...
                pedidoProdutosCarregamentoExcluir = pedidoProdutosCarregamento.FindAll(x => !produtosEmbarcador.Exists(p => p.CodigoProduto == x.PedidoProduto.Produto.CodigoProdutoEmbarcador));

                foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto carregamentoPedidoProduto in pedidoProdutosCarregamentoExcluir)
                {
                    GerarLog($"1 - Carregamento.: {codigoCarregamento}, Pedido.: {carregamentoPedidoProduto.CarregamentoPedido.Pedido.NumeroPedidoEmbarcador} - Excluiu o produto {carregamentoPedidoProduto.PedidoProduto?.Produto?.CodigoProdutoEmbarcador ?? "S.C.I"} - {carregamentoPedidoProduto.PedidoProduto?.Produto?.Descricao ?? string.Empty} do carregamentoPedidoProduto: Qtde.: {carregamentoPedidoProduto.Quantidade}, Pallet.: {carregamentoPedidoProduto.QuantidadePallet}, M3.: {carregamentoPedidoProduto.MetroCubico}");
                    repCarregamentoPedidoProduto.Deletar(carregamentoPedidoProduto);
                }
            }

            // Problema na NaturalOne, quando faturado apenas 1 item do pedido...
            bool assaiOuPpc = (configuracao.AtualizarProdutosCarregamentoPorNota || configuracao.FatorMetroCubicoProdutoEmbarcadorIntegracao == 1000000);

            if (atualizarSaldoPedidoProdutosPorXmlNotaFiscal && pesoCargaPedidoProdutosExcluidos > 0 && assaiOuPpc)
            {
                GerarLog($"2 - Pedido.: {cargaPedido.Pedido.NumeroPedidoEmbarcador} - Peso excluído.: {pesoCargaPedidoProdutosExcluidos}, carregamentoPedido.: {pedidoCarregamento?.Codigo ?? 0}, pesoCarregamentoPedido.: {pedidoCarregamento?.Peso ?? 0}, Pedido.PesoSaldoRestante.: {cargaPedido.Pedido.PesoSaldoRestante}");
                //Descontando o peso excluido..
                if (pedidoCarregamento != null)
                {
                    pedidoCarregamento.Peso -= pesoCargaPedidoProdutosExcluidos;
                    repCarregamentoPedido.Atualizar(pedidoCarregamento);
                }

                cargaPedido.Pedido.PesoSaldoRestante += pesoCargaPedidoProdutosExcluidos;
                cargaPedido.Pedido.PedidoTotalmenteCarregado = false;
            }

            foreach (Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produtocargaIntegracao in produtosEmbarcador)
            {
                // Se existe o produto na carga...
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto = (from cpp in cargaPedidosProdutos where cpp.Produto.CodigoProdutoEmbarcador == produtocargaIntegracao.CodigoProduto select cpp).FirstOrDefault();
                if (cargaPedidoProduto == null)
                    cargaPedidoProduto = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto();

                cargaPedidoProduto.Initialize();

                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = cargaPedidoProduto.Produto;

                cargaPedidoProduto.CargaPedido = cargaPedido;
                cargaPedidoProduto.Produto = servicoProdutoEmbarcador.IntegrarProduto(produtosEmbarcadorSalvos, configuracao, produtocargaIntegracao.CodigoProduto, produtocargaIntegracao.DescricaoProduto, produtocargaIntegracao.PesoUnitario, null, produtocargaIntegracao.MetroCubito, auditado, produtocargaIntegracao.CodigoDocumentacao, produtocargaIntegracao.Atualizar, produtocargaIntegracao.CodigoNCM, produtocargaIntegracao.QuantidadePorCaixa, produtocargaIntegracao.QuantidadeCaixaPorPallet, produtocargaIntegracao.Altura, produtocargaIntegracao.Largura, produtocargaIntegracao.Comprimento, null, null, null, produtocargaIntegracao.UnidadeMedida, produtocargaIntegracao.Observacao, "", produtocargaIntegracao?.CodigoEAN ?? string.Empty);

                cargaPedidoProduto.ValorUnitarioProduto = produtocargaIntegracao.ValorUnitario;

                // Se o produto já está na carga...
                if (cargaPedidoProduto.Codigo > 0 && configuracao.AtualizarProdutosCarregamentoPorNota)
                {
                    // Diferença = Total Programado - Total Prod Nota.
                    decimal dif = cargaPedidoProduto.Quantidade - produtocargaIntegracao.Quantidade;
                    if (dif != 0)
                    {
                        //Vamos pegar o antigo peso unitário. pois pode vir atualizado e não é atualizado no t_pedido_produto
                        decimal pesoUnitarioProdutoEmbarcador = produtoEmbarcador?.PesoUnitario ?? 0;
                        if (pesoUnitarioProdutoEmbarcador == 0)
                            pesoUnitarioProdutoEmbarcador = produtocargaIntegracao.PesoUnitario;

                        decimal peso = (dif * pesoUnitarioProdutoEmbarcador);

                        if (pedidoCarregamento != null)
                        {
                            if (pedidoProdutosCarregamento != null)
                            {
                                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto produtoPedidoCarregamento = (from ppc in pedidoProdutosCarregamento
                                                                                                                                         where ppc.PedidoProduto.Produto.CodigoProdutoEmbarcador == cargaPedidoProduto.Produto.CodigoProdutoEmbarcador
                                                                                                                                         select ppc).FirstOrDefault();
                                if (produtoPedidoCarregamento != null)
                                {
                                    decimal pesoUnitario = produtoPedidoCarregamento.Peso / (produtoPedidoCarregamento.Quantidade > 0 ? produtoPedidoCarregamento.Quantidade : 1);
                                    decimal metroUnitario = produtoPedidoCarregamento.MetroCubico / (produtoPedidoCarregamento.Quantidade > 0 ? produtoPedidoCarregamento.Quantidade : 1);
                                    decimal palletUnitario = produtoPedidoCarregamento.QuantidadePallet / (produtoPedidoCarregamento.Quantidade > 0 ? produtoPedidoCarregamento.Quantidade : 1);

                                    //Peso unitário programado..
                                    peso = (dif * pesoUnitario);

                                    produtoPedidoCarregamento.MetroCubico -= (dif * metroUnitario);
                                    produtoPedidoCarregamento.Peso -= (dif * pesoUnitario);
                                    produtoPedidoCarregamento.QuantidadePallet -= (dif * palletUnitario);
                                    produtoPedidoCarregamento.Quantidade -= dif;
                                    repCarregamentoPedidoProduto.Atualizar(produtoPedidoCarregamento);
                                }
                            }

                            GerarLog($"3 - Pedido.: {pedidoCarregamento.Pedido.NumeroPedidoEmbarcador} - CarregamentoPedido.Peso.: {pedidoCarregamento.Peso}, Peso descontar.: {peso}");
                            // Descontando ou somando o peso da diferença dos produtos...
                            pedidoCarregamento.Peso -= peso;
                            repCarregamentoPedido.Atualizar(pedidoCarregamento);

                        }

                        if (atualizarSaldoPedidoProdutosPorXmlNotaFiscal && assaiOuPpc)
                        {
                            GerarLog($"4 - Pedido.: {cargaPedido.Pedido.NumeroPedidoEmbarcador} - cargaPedido.Pedido.PesoSaldoRestante.: {cargaPedido.Pedido.PesoSaldoRestante}, Peso somar.: {peso}");
                            cargaPedido.Pedido.PesoSaldoRestante += peso;
                            cargaPedido.Pedido.PedidoTotalmenteCarregado = (cargaPedido.Pedido.PesoSaldoRestante <= 0.5m);
                        }
                        else if (!cargaPedido.Pedido.PedidoTotalmenteCarregado)
                            cargaPedido.Pedido.PedidoTotalmenteCarregado = (cargaPedido.Pedido.PesoSaldoRestante <= 0.5m);

                        GerarLog($"5 - Pedido.: {cargaPedido.Pedido.NumeroPedidoEmbarcador} - cargaPedido.Pedido.PedidoTotalmenteCarregado.: {cargaPedido.Pedido.PedidoTotalmenteCarregado}");

                        //TODO: PPC - Adicionado log temporário para identificar problema no cargaPedido.Peso.
                        Servicos.Log.TratarErro($"Pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} - CargaPedido.Codigo = {cargaPedido.Codigo} - Peso Total De.: {cargaPedido.Peso} - Para.: {cargaPedido.Peso - peso}. Produto.AtualizarProdutosCargaPedidoPorNotaFiscal", "PesoCargaPedido");
                        cargaPedido.Peso -= peso;
                    }
                }

                if (cargaPedido.Carga.TipoOperacao?.ConfiguracaoCarga?.MultiplicarQuantidadeProdutoPorCaixaPelaQuantidadeCaixa ?? false)
                {
                    Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto repositorioXmlNotaFiscalProduto = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto(unitOfWork);
                    Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

                    List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmlsNotaFiscal = repositorioXmlNotaFiscal.BuscarXMLNotaFiscalPorCargaPedido(cargaPedido.Codigo);
                    List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> xmlNotasFiscaisProdutos = repositorioXmlNotaFiscalProduto.BuscarPorNotaFiscais(xmlsNotaFiscal.Select(xnf => xnf.Codigo).ToList());
                    List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> xmlsNotaFiscalProduto = (from o in xmlNotasFiscaisProdutos where o.Produto.Codigo == cargaPedidoProduto.Produto.Codigo select o).ToList();

                    int quantidadePorCaixa = cargaPedidoProduto.Produto.QuantidadeCaixa;
                    if (quantidadePorCaixa == 0)
                        quantidadePorCaixa = 1;

                    GerarLog($"CargaPedidoProduto {cargaPedidoProduto.Codigo}, quantidade anterior {cargaPedidoProduto.Quantidade}, quantidade produto {produtocargaIntegracao.Quantidade}, quantidadePorCaixa {quantidadePorCaixa} - produto {cargaPedidoProduto.Produto.Codigo}");

                    cargaPedidoProduto.QuantidadeOriginal = produtocargaIntegracao.Quantidade;
                    cargaPedidoProduto.Quantidade = produtocargaIntegracao.Quantidade * quantidadePorCaixa;

                    GerarLog($"CargaPedidoProduto {cargaPedidoProduto.Codigo}, calculou como quantidade {cargaPedidoProduto.Quantidade}");

                    foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto xmlNotaFiscalProduto in xmlsNotaFiscalProduto)
                    {
                        GerarLog($"CargaPedidoProduto {cargaPedidoProduto.Codigo}, quantidade anterior {xmlNotaFiscalProduto.Quantidade} do produto {xmlNotaFiscalProduto.Produto.Codigo}");
                        xmlNotaFiscalProduto.QuantidadeOriginal = cargaPedidoProduto.QuantidadeOriginal;
                        xmlNotaFiscalProduto.Quantidade = cargaPedidoProduto.Quantidade;
                        repositorioXmlNotaFiscalProduto.Atualizar(xmlNotaFiscalProduto);
                    }
                }
                else
                    cargaPedidoProduto.Quantidade = produtocargaIntegracao.Quantidade;

                cargaPedidoProduto.PesoUnitario = cargaPedidoProduto.Produto.PesoUnitario;

                if (auditado != null)
                    Auditoria.Auditoria.Auditar(auditado, cargaPedidoProduto, cargaPedidoProduto.GetChanges(), "Salvou o produto", unitOfWork);

                if (cargaPedidoProduto.Codigo == 0)
                    repCargaPedidoProduto.Inserir(cargaPedidoProduto);
                else
                    repCargaPedidoProduto.Atualizar(cargaPedidoProduto);

                if (cargaPedidoProduto.Produto.DescontarPesoProdutoCalculoFrete)
                    cargaPedido.PesoMercadoriaDescontar += cargaPedidoProduto.PesoTotal;

                if (cargaPedidoProduto.Produto.DescontarValorProdutoCalculoFrete)
                    cargaPedido.ValorMercadoriaDescontar += cargaPedidoProduto.ValorUnitarioProduto * cargaPedidoProduto.Quantidade;
            }
        }

        #endregion

        #region Métodos Privados

        private void GerarLog(string log)
        {
            Servicos.Log.TratarErro(log, "AtualizarProdutosCargaPedidoPorNotaFiscal");
        }

        #endregion
    }
}
