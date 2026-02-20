using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga.ControleEntrega
{
    public class ControleEntregaQualidade : ServicoBase
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaQualidade _repCargaEntregaQualidade;
        private readonly Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega _repositorioCargaEntrega;
        private readonly Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaQualidade _repositorioCargaEntregaQualidade;
        private readonly Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoQualidadeEntrega _configuracaoQualidadeEntrega;
        #endregion

        #region Construtores
        public ControleEntregaQualidade(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoQualidadeEntrega? configuracaoQualidadeEntrega) : base(unitOfWork)
        {
            _configuracaoQualidadeEntrega = configuracaoQualidadeEntrega ?? new Repositorio.Embarcador.TorreControle.ConfiguracaoQualidadeEntrega(_unitOfWork).BuscarPrimeiroRegistro();
            _repositorioCargaEntrega = new(_unitOfWork, _cancellationToken);
            _repositorioCargaEntregaQualidade = new(_unitOfWork, _cancellationToken);
        }
        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Processa todas as regras e executa posterior validação por regra.
        /// </summary>
        public void ProcessarRegrasDeQualidadeDeEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega)
        {
            Servicos.Log.GravarInfo($"Início ProcessarRegrasDeQualidadeDeEntrega {cargaEntrega.Codigo}", "ControleEntregaQualidade");
            if (_configuracaoQualidadeEntrega == null || cargaEntrega == null) return;

            try
            {
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaQualidade cargaEntregaQualidade = _repositorioCargaEntregaQualidade.BuscarPorCodigoCargaEntrega(cargaEntrega.Codigo);
                cargaEntregaQualidade ??= new();

                ProcessarRegra_DataConfirmacaoEntrega(cargaEntregaQualidade, cargaEntrega);

                _repositorioCargaEntregaQualidade.Inserir(cargaEntregaQualidade);
                cargaEntrega.CargaEntregaQualidade = cargaEntregaQualidade;

                _repositorioCargaEntrega.Atualizar(cargaEntrega);

                _unitOfWork.Flush();

                ExecutarDepoisDeProcessarAsRegras(cargaEntregaQualidade, cargaEntrega);
                Servicos.Log.GravarInfo($"Fim ProcessarRegrasDeQualidadeDeEntrega {cargaEntrega.Codigo}", "ControleEntregaQualidade");
            }
            catch (Exception ex)
            {
                /* Enterra para seguir o processo */
                Servicos.Log.TratarErro(ex, "ControleEntregaQualidade");
            }
        }

        public void AjustarItensNotasFiscaisControleEntrega(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(_unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNota = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(_unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto repositorioCargaEntregaProduto = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto(_unitOfWork);
                Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto repXMLNotaFiscalProduto = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto(_unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos = repCargaEntregaPedido.BuscarPorCarga(codigoCarga);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXmlNotaFiscalCarga = repPedidoXMLNotaFiscal.BuscarPorCarga(codigoCarga);

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregasSemProduto = repCargaEntrega.BuscarEntregasSemProdutoPorCarga(codigoCarga);

                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> xmlNotaFiscalProdutos = repXMLNotaFiscalProduto.BuscarPorNotaFiscais(codigoCarga);

                if (xmlNotaFiscalProdutos.Count() <= 0)
                    return;

                bool multiplicar = carga.TipoOperacao?.ConfiguracaoCarga?.MultiplicarQuantidadeProdutoPorCaixaPelaQuantidadeCaixa ?? false;

                foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega in entregasSemProduto)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidoSemProduto = cargaEntregaPedidos.Where(x => x.CargaEntrega.Codigo == entrega.Codigo).ToList();
                    List<int> pedidos = cargaEntregaPedidoSemProduto.Select(x => x.CargaPedido.Pedido.Codigo).ToList();

                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXmlNotaFiscalAdd = pedidosXmlNotaFiscalCarga.Where(x => pedidos.Contains(x.CargaPedido.Pedido.Codigo)).ToList();

                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidosXmlNotaFiscalAdd)
                    {

                        List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> produtosEmbarcador = (from obj in xmlNotaFiscalProdutos where obj.XMLNotaFiscal.Codigo == pedidoXMLNotaFiscal.XMLNotaFiscal.Codigo select obj.Produto).Distinct().ToList();

                        if (produtosEmbarcador != null && produtosEmbarcador.Count > 0)
                        {
                            foreach (var produtoEmbarcador in produtosEmbarcador)
                            {

                                int quantidadeCaixa = produtoEmbarcador.QuantidadeCaixa;
                                if (quantidadeCaixa == 0 || !multiplicar)
                                    quantidadeCaixa = 1;

                                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto cargaEntregaProduto = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto
                                {
                                    CargaEntrega = entrega,
                                    Produto = produtoEmbarcador,
                                    XMLNotaFiscal = pedidoXMLNotaFiscal.XMLNotaFiscal,
                                    Quantidade = quantidadeCaixa * (from obj in xmlNotaFiscalProdutos where obj.Produto.Codigo == produtoEmbarcador.Codigo && obj.XMLNotaFiscal.Codigo == pedidoXMLNotaFiscal.XMLNotaFiscal.Codigo select obj.QuantidadeUtilizar).Sum(),
                                    PesoUnitario = produtoEmbarcador.PesoUnitario

                                };

                                repositorioCargaEntregaProduto.Inserir(cargaEntregaProduto);

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "ControleQualidadeTorre");
            }
        }

        public void AjustarNotasFiscaisControleEntrega(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNota = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto repositorioCargaEntregaProduto = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto(_unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto repXMLNotaFiscalProduto = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(_unitOfWork).BuscarPrimeiroRegistro();

            try
            {

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos = repCargaEntregaPedido.BuscarPorCarga(codigoCarga);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXmlNotaFiscalCarga = repPedidoXMLNotaFiscal.BuscarPorCarga(codigoCarga);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregasSemNota = repCargaEntrega.BuscarEntregasSemNotaPorCarga(codigoCarga);
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> xmlNotaFiscalProdutos = repXMLNotaFiscalProduto.BuscarPorNotaFiscais(codigoCarga);

                bool multiplicar = carga.TipoOperacao?.ConfiguracaoCarga?.MultiplicarQuantidadeProdutoPorCaixaPelaQuantidadeCaixa ?? false;

                foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega in entregasSemNota)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidoSemNota = cargaEntregaPedidos.Where(x => x.CargaEntrega.Codigo == entrega.Codigo).ToList();
                    List<int> pedidos = cargaEntregaPedidoSemNota.Select(x => x.CargaPedido.Pedido.Codigo).ToList();

                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXmlNotaFiscalAdd = pedidosXmlNotaFiscalCarga.Where(x => pedidos.Contains(x.CargaPedido.Pedido.Codigo)).ToList();

                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidosXmlNotaFiscalAdd)
                    {
                        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal cargaEntregaNotaFiscal = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal
                        {
                            CargaEntrega = entrega,
                            PedidoXMLNotaFiscal = pedidoXMLNotaFiscal
                        };

                        repCargaEntregaNota.Inserir(cargaEntregaNotaFiscal);

                        List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> produtosEmbarcador = (from obj in xmlNotaFiscalProdutos where obj.XMLNotaFiscal.Codigo == pedidoXMLNotaFiscal.XMLNotaFiscal.Codigo select obj.Produto).Distinct().ToList();

                        if (produtosEmbarcador != null && produtosEmbarcador.Count > 0)
                        {
                            foreach (var produtoEmbarcador in produtosEmbarcador)
                            {

                                int quantidadeCaixa = produtoEmbarcador.QuantidadeCaixa;
                                if (quantidadeCaixa == 0 || !multiplicar)
                                    quantidadeCaixa = 1;

                                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto cargaEntregaProduto = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto
                                {
                                    CargaEntrega = entrega,
                                    Produto = produtoEmbarcador,
                                    XMLNotaFiscal = pedidoXMLNotaFiscal.XMLNotaFiscal,
                                    Quantidade = quantidadeCaixa * (from obj in xmlNotaFiscalProdutos where obj.Produto.Codigo == produtoEmbarcador.Codigo && obj.XMLNotaFiscal.Codigo == pedidoXMLNotaFiscal.XMLNotaFiscal.Codigo select obj.QuantidadeUtilizar).Sum(),
                                    PesoUnitario = produtoEmbarcador.PesoUnitario

                                };

                                repositorioCargaEntregaProduto.Inserir(cargaEntregaProduto);
                            }
                        }
                    }

                    entrega.EntregaComNotasFiscaisReprocessada = true;
                    repCargaEntrega.Atualizar(entrega);

                    Servicos.Auditoria.Auditoria.Auditar(auditado, entrega, null, "Entrega com notas reprocessadas (Auto correção)", _unitOfWork);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "ControleQualidadeTorre");
            }

        }

        #endregion

        #region Processamento Pós Regras
        private void ExecutarDepoisDeProcessarAsRegras(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaQualidade cargaEntregaQualidade, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega)
        {
            string regra = string.Empty;
            Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega servicoControleEntrega = new(_unitOfWork);
            Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new(_unitOfWork);

            //Valida se alguma regra está habilitada para bloquear disponibilidade do canhoto na consulta.
            if (_configuracaoQualidadeEntrega.VerificarDataConfirmacaoIntervaloRaio && !cargaEntrega.Coleta)
            {
                //Adicionar demais flags para disponibilizar para consulta.
                bool disponivelParaConsulta = cargaEntregaQualidade.RegraDataConfirmacaoIntervaloRaio;

                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotosEntrega = servicoControleEntrega.ObterCanhotosDaCargaEntrega(cargaEntrega);
                canhotosEntrega.ForEach(canhoto => canhoto.DisponivelParaConsulta = disponivelParaConsulta);
                repositorioCanhoto.Atualizar(canhotosEntrega, new() { "DisponivelParaConsulta" }, "T_CANHOTO_NOTA_FISCAL");

                string situacao = disponivelParaConsulta ? "liberado" : "bloqueado";
                regra = "Data de Confirmação Intervalo Raio";
                Servicos.Embarcador.Canhotos.Canhoto.GerarHistoricoCanhotos(canhotosEntrega, null, $"Canhoto {situacao} para consulta via regra de qualidade \"{regra}\".", _unitOfWork);
            }
        }
        #endregion

        #region Processamento de Regras
        /// <summary>
        /// Data de Confirmação da Entrega
        /// Regra: Data da Entrega deve estar entre a Data de Entrada no Alvo e Data de Saída do Alvo.
        /// </summary>
        /// <param name="cargaEntregaQualidade">Entidade do Resultado das Regras de Qualidade</param>
        /// <param name="cargaEntrega">Entrega que será avaliada pela regra</param>
        private void ProcessarRegra_DataConfirmacaoEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaQualidade cargaEntregaQualidade, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega)
        {
            cargaEntregaQualidade.RegraDataConfirmacaoIntervaloRaio = false;

            if (cargaEntrega.DataEntradaRaio.HasValue && cargaEntrega.DataSaidaRaio.HasValue)
            {
                bool considerarHoraMinuto = _configuracaoQualidadeEntrega.ConsiderarDataHoraConfirmacaoIntervaloRaio;


                if (considerarHoraMinuto)
                {
                    cargaEntregaQualidade.RegraDataConfirmacaoIntervaloRaio =
                        cargaEntrega.DataConfirmacao >= cargaEntrega.DataEntradaRaio &&
                        cargaEntrega.DataConfirmacao <= cargaEntrega.DataSaidaRaio;
                }
                else
                {
                    cargaEntregaQualidade.RegraDataConfirmacaoIntervaloRaio =
                        cargaEntrega.DataConfirmacao?.Date >= cargaEntrega.DataEntradaRaio.Value.Date &&
                        cargaEntrega.DataConfirmacao?.Date <= cargaEntrega.DataSaidaRaio.Value.Date;
                }

            }

            cargaEntregaQualidade.DataHoraRegraDataConfirmacaoIntervaloRaio = DateTime.Now;
        }
        #endregion

        #region Métodos Privados
        #endregion
    }
}