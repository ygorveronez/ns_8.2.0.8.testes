using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Servicos.Embarcador.Carga
{
    public class RateioFrete : ServicoBase
    {
        public RateioFrete() : base() { }

        public RateioFrete(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        #region Propriedades Privadas

        private static Dominio.Entidades.ModeloDocumentoFiscal ModeloDocumentoFiscalCTePadrao;

        #endregion

        #region Métodos Públicos

        public void RatearValorFreteCargaEmitidaParcialmente(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if ((carga.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.PreCheckIn) || (carga.TipoOperacao?.ConfiguracaoCalculoFrete?.RatearValorFreteEntrePedidosAposReceberDocumentos ?? false))
                RatearValorDoFrenteEntrePedidos(carga, listaCargaPedidos, configuracaoEmbarcador, false, unitOfWork, tipoServicoMultisoftware);

            if (!carga.ExigeNotaFiscalParaCalcularFrete)
            {
                Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repositorioCargaPedidoComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
                Servicos.Embarcador.Carga.RateioNotaFiscal servicoRateioNotaFiscal = new Servicos.Embarcador.Carga.RateioNotaFiscal(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidosComponentesFreteCarga = repositorioCargaPedidoComponenteFrete.BuscarPorCarga(carga.Codigo, false);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosNaoEmitidos;

                if (carga.EmpresaFilialEmissora != null && carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                    cargaPedidosNaoEmitidos = listaCargaPedidos.Where(o => !o.CTesFilialEmissoraEmitidos && o.SituacaoEmissao == SituacaoNF.NFEnviada).ToList();
                else
                    cargaPedidosNaoEmitidos = listaCargaPedidos.Where(o => !o.CTesEmitidos && o.SituacaoEmissao == SituacaoNF.NFEnviada).ToList();

                servicoRateioNotaFiscal.RatearFreteCargaPedidoEntreNotas(carga, cargaPedidosNaoEmitidos, cargaPedidosComponentesFreteCarga, false, tipoServicoMultisoftware, unitOfWork, configuracaoEmbarcador);

                if (carga.EmpresaFilialEmissora != null)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidosComponentesFreteCargaFilialEmissora = repositorioCargaPedidoComponenteFrete.BuscarPorCarga(carga.Codigo, true);
                    servicoRateioNotaFiscal.RatearFreteCargaPedidoEntreNotas(carga, cargaPedidosNaoEmitidos, cargaPedidosComponentesFreteCargaFilialEmissora, true, tipoServicoMultisoftware, unitOfWork, configuracaoEmbarcador);
                }
            }
        }

        public void RatearValorDoFrenteEntrePedidos(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configEmbarcador, bool rateioFreteFilialEmissora, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            RatearValorDoFrenteEntrePedidos(carga, cargaPedidos, configEmbarcador, rateioFreteFilialEmissora, unitOfWork, tipoServicoMultisoftware, new Dominio.ObjetosDeValor.Embarcador.Carga.ConfiguracaoRateioValorFreteEntrePedidos());
        }

        public void RatearValorDoFrenteEntrePedidos(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configEmbarcador, bool rateioFreteFilialEmissora, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Carga.ConfiguracaoRateioValorFreteEntrePedidos configuracaoRateio)
        {
            //todo: criar regra para diferentes formas de rateio
            //Servicos.Log.TratarErro("RatearValorDoFrenteEntrePedidos Carga " + carga.CodigoCargaEmbarcador);

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao repCargaPedidoContaContabilContabilizacao = new Repositorio.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete repCargaComposicaoFrete = new Repositorio.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaFechamento repCargaFechamento = new Repositorio.Embarcador.Cargas.CargaFechamento(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia repositorioConfiguracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork);

            Servicos.Embarcador.Carga.ComponetesFrete serComponetesFrete = new ComponetesFrete(unitOfWork);
            Servicos.Embarcador.Carga.ICMS serCargaICMS = new Servicos.Embarcador.Carga.ICMS(unitOfWork);

            cargaPedidos = (from obj in cargaPedidos where obj.Pedido.TipoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedido.Entrega select obj).ToList();
            cargaPedidos = (from obj in cargaPedidos where obj.Pedido.CanalEntrega == null || !obj.Pedido.CanalEntrega.LiberarPedidoSemNFeAutomaticamente select obj).ToList();

            Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFreteICMS = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS);
            Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFretePisCONFIS = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork).BuscarPrimeiroRegistro();

            if (!rateioFreteFilialEmissora)
                carga.ValorFrete = Math.Round(carga.ValorFrete, 2, MidpointRounding.AwayFromZero);
            else
                carga.ValorFreteFilialEmissora = Math.Round(carga.ValorFreteFilialEmissora, 2, MidpointRounding.AwayFromZero);

            decimal valorFrete = !rateioFreteFilialEmissora ? carga.ValorFrete : carga.ValorFreteFilialEmissora;
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosDesconsideradosNoRateio = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            if ((carga.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.PreCheckIn) || (carga.TipoOperacao?.ConfiguracaoCalculoFrete?.RatearValorFreteEntrePedidosAposReceberDocumentos ?? false))
            {
                // Recalcular valor do componente AD Valorem do carga pedido e da carga
                decimal valorComponenteAdValorem = 0;
                decimal valorTotalDeTodasNotasFiscais = 0;

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponenteFrete = repCargaPedidoComponenteFrete.BuscarPorCompomente(cargaPedido.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ADVALOREM, null, false);

                    if (cargaPedidoComponenteFrete == null)
                        continue;

                    if ((carga.TipoContratacaoCarga == TipoContratacaoCarga.Normal) || (carga.TipoContratacaoCarga == TipoContratacaoCarga.NormalESubContratada) || (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador))
                    {
                        decimal valorTotalNotasFiscais = repPedidoXMLNotaFiscal.BuscarTotalPorCargaPedido(cargaPedido.Codigo);

                        cargaPedidoComponenteFrete.ValorComponente = ((cargaPedidoComponenteFrete.Percentual / 100) * valorTotalNotasFiscais);
                        valorComponenteAdValorem += cargaPedidoComponenteFrete.ValorComponente;
                        valorTotalDeTodasNotasFiscais += valorTotalNotasFiscais;

                        repCargaPedidoComponenteFrete.Atualizar(cargaPedidoComponenteFrete);
                    }
                }

                Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponenteFreteAdVALOREM = repCargaComponentesFrete.BuscarPorCargaETipo(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ADVALOREM, null, false);

                if (cargaComponenteFreteAdVALOREM != null)
                {
                    cargaComponenteFreteAdVALOREM.ValorComponente = valorComponenteAdValorem;

                    Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete cargaComposicao = repCargaComposicaoFrete.BuscarPorCodigoComponente(cargaComponenteFreteAdVALOREM.ComponenteFrete.Codigo, carga.Codigo);

                    if (cargaComposicao != null)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicaoFrete = Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("(Valor Total Mercadoria * Percentual)", " (" + valorTotalDeTodasNotasFiscais.ToString("n5") + " * " + cargaComponenteFreteAdVALOREM.Percentual.ToString("n5") + "%)", cargaComponenteFreteAdVALOREM.Percentual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ComponenteFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, "AD VALOREM", cargaComponenteFreteAdVALOREM.ComponenteFrete.Codigo, valorComponenteAdValorem);
                        Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.AtualizarComposicaoFrete(carga, cargaComposicao, null, null, null, false, composicaoFrete, unitOfWork);
                    }
                }
            }

            if (carga.TipoOperacao?.ConfiguracaoCalculoFrete?.RatearValorFreteEntrePedidosAposReceberDocumentos ?? false)
            {
                cargaPedidosDesconsideradosNoRateio = cargaPedidos.Where(o => o.CTesEmitidos).ToList();
                valorFrete -= cargaPedidosDesconsideradosNoRateio.Sum(o => o.ValorFrete);
                cargaPedidos = cargaPedidos.Where(o => !o.CTesEmitidos).ToList();
            }

            else if (repCargaFechamento.CargaEstaEmFechamentoAgRateio(carga.Codigo))//se esta em fechamento (ULTRAGAS 36506) 
            {
                valorFrete -= cargaPedidos.Where(o => (o.Pedido.NumeroControle == "" || o.Pedido.NumeroControle == null)).Sum(o => o.ValorFrete); // FAZ O RATEIO COM O VALOR TOTAL - VALOR DOS PEDIDOS SEM NUMERO CONTROLE
                cargaPedidos = cargaPedidos.Where(o => o.Pedido.NumeroControle != null).ToList(); // FAZ O RATEIO NOS PEDIDOS QUE POSSUEM NUMERO CONTROLE
            }

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosLiberados = (from obj in cargaPedidos where !obj.PedidoSemNFe select obj).ToList(); //Servicos.Embarcador.Carga.CargaPedido.ObterCargaPedidosParaRateio(carga, unitOfWork);
            Dominio.Entidades.Cliente tomador = cargaPedidos?.FirstOrDefault()?.ObterTomador();

            if (!rateioFreteFilialEmissora)
            {
                bool utilizaContaRazao = carga.TipoOperacao?.TipoOperacaoUtilizaContaRazao ?? false;
                if (!utilizaContaRazao)
                {
                    repCargaPedidoContaContabilContabilizacao.DeletarPorCarga(carga.Codigo);
                    carga.PossuiPendenciaConfiguracaoContabil = false;
                }
            }

            if (configEmbarcador.ComponenteFreteDescontoSeguro != null)
            {
                repCargaComponentesFrete.DeletarPorCarga(carga.Codigo, rateioFreteFilialEmissora, configEmbarcador.ComponenteFreteDescontoSeguro.Codigo);
                if (carga.CargaAgrupada)
                    repCargaComponentesFrete.DeletarPorCargaAgrupamento(carga.Codigo, rateioFreteFilialEmissora, configEmbarcador.ComponenteFreteDescontoSeguro.Codigo);

                if (carga.DescontoSeguro > 0 || carga.PercentualDescontoSeguro > 0)
                {
                    decimal desconto = carga.DescontoSeguro;
                    if (carga.PercentualDescontoSeguro > 0)
                        desconto = Math.Round((carga.ValorFrete * carga.PercentualDescontoSeguro) / 100, 2, MidpointRounding.ToEven);

                    serComponetesFrete.AdicionarComponenteFreteCarga(carga, configEmbarcador.ComponenteFreteDescontoSeguro, -desconto, 0, rateioFreteFilialEmissora, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, configEmbarcador.ComponenteFreteDescontoSeguro.TipoComponenteFrete, null, configEmbarcador.IncluirBCCompontentesDesconto, false, null, tipoServicoMultisoftware, null, unitOfWork, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete.TabelaFrete, true, false, tomador);
                }
            }

            if (carga.Filial?.Descontos?.Count > 0)
            {
                List<int> codigosPedidos = cargaPedidos.Select(o => o.Pedido.Codigo).ToList();
                List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> listaProdutos = new List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();

                if (carga.Carregamento != null)
                {
                    Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto repCarregamentoPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> cpp = repCarregamentoPedidoProduto.BuscarPorPedidos(carga.Carregamento.Codigo, codigosPedidos);
                    listaProdutos = (from pe in cpp select pe.PedidoProduto.Produto).ToList();
                }

                if (listaProdutos.Count == 0)
                {
                    Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
                    listaProdutos = repPedidoProduto.BuscarProdutosPorPedidos(codigosPedidos);
                }

                new Servicos.Embarcador.Filiais.Filial(unitOfWork).InformarValorDescontoFilialCarga(carga.Carregamento?.ModeloVeicularCarga ?? carga.ModeloVeicularCarga, carga, carga.Filial, listaProdutos, carga.Empresa, carga.DataCarregamentoCarga);
            }

            if (carga.DescontoFilial > 0)
            {
                if (configEmbarcador.ComponenteFreteDescontoFilial != null)
                {
                    repCargaComponentesFrete.DeletarPorCarga(carga.Codigo, rateioFreteFilialEmissora, configEmbarcador.ComponenteFreteDescontoFilial.Codigo);
                    if (carga.CargaAgrupada)
                        repCargaComponentesFrete.DeletarPorCargaAgrupamento(carga.Codigo, rateioFreteFilialEmissora, configEmbarcador.ComponenteFreteDescontoFilial.Codigo);

                    serComponetesFrete.AdicionarComponenteFreteCarga(carga, configEmbarcador.ComponenteFreteDescontoFilial, -carga.DescontoFilial, 0, rateioFreteFilialEmissora, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, configEmbarcador.ComponenteFreteDescontoFilial.TipoComponenteFrete, null, configEmbarcador.IncluirBCCompontentesDesconto, false, null, tipoServicoMultisoftware, null, unitOfWork, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete.TabelaFrete, true, false, tomador);
                }
                else
                    Servicos.Log.TratarErro("Falta configurar o componente padrão de desconto da filial na t_configuracao_embarcador");
            }

            carga.BonificacaoTransportador = null;
            carga.PercentualBonificacaoTransportador = 0m;

            if (carga.TabelaFrete != null && carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
            {
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = repositorioConfiguracaoOcorrencia.BuscarConfiguracaoPadrao();

                if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && !configuracaoOcorrencia.UtilizarBonificacaoParaTransportadoresViaOcorrencia)
                {
                    Servicos.Embarcador.Frete.BonificacaoTransportador servicoBonificacaoTransportador = new Embarcador.Frete.BonificacaoTransportador(unitOfWork);
                    Dominio.Entidades.Embarcador.Frete.BonificacaoTransportador bonificacao = servicoBonificacaoTransportador.ObterBonificacao(carga);

                    if (bonificacao != null)
                    {
                        repCargaComponentesFrete.DeletarPorCarga(carga.Codigo, rateioFreteFilialEmissora, bonificacao.ComponenteFrete.Codigo);

                        if (carga.CargaAgrupada)
                            repCargaComponentesFrete.DeletarPorCargaAgrupamento(carga.Codigo, rateioFreteFilialEmissora, bonificacao.ComponenteFrete.Codigo);


                        decimal valorComponente = 0m;

                        if (bonificacao.NaoIncluirComponentesFreteCalculoBonificacao)
                        {
                            valorComponente = Math.Round(carga.ValorFrete * (bonificacao.PercentualAplicar / 100), 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            decimal valorComponentesFreteSemImpostos = repCargaComponentesFrete.BuscarValorTotalPorCargaSemImpostos(carga.Codigo);
                            valorComponente = Math.Round((carga.ValorFrete + valorComponentesFreteSemImpostos) * (bonificacao.PercentualAplicar / 100), 2, MidpointRounding.AwayFromZero);
                        }

                        if (valorComponente != 0m)
                        {
                            carga.BonificacaoTransportador = bonificacao;
                            carga.PercentualBonificacaoTransportador = bonificacao.PercentualAplicar;
                            serComponetesFrete.AdicionarComponenteFreteCarga(carga, bonificacao.ComponenteFrete, valorComponente, bonificacao.Percentual, rateioFreteFilialEmissora, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, bonificacao.ComponenteFrete.TipoComponenteFrete, null, bonificacao.IncluirBaseCalculoICMS, false, null, tipoServicoMultisoftware, null, unitOfWork, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete.TabelaFrete, true, false, tomador);
                        }
                    }
                }

                if (carga.TabelaFrete.TipoTabelaFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaRota || carga.TabelaFrete.TipoTabelaFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaCliente)
                {
                    RatearFreteEntrePedidos(carga, cargaPedidos, configEmbarcador, valorFrete, rateioFreteFilialEmissora, tipoServicoMultisoftware, unitOfWork);
                }
                else if (carga.TabelaFrete.TipoTabelaFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaComissaoProduto)
                {
                    RatearFreteEntrePedidosTabelaComissao(carga, cargaPedidos, rateioFreteFilialEmissora, tipoServicoMultisoftware, unitOfWork, configEmbarcador, configuracaoGeralCarga);
                }
            }
            else
            {
                //if (carga.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada)
                //{
                //Servicos.Log.TratarErro("RatearValorDoFrenteEntrePedidos 2 Carga " + carga.CodigoCargaEmbarcador);

                Repositorio.Embarcador.Cargas.CargaTabelaFreteSubContratacao repCargaTabelaFreteSubContratacao = new Repositorio.Embarcador.Cargas.CargaTabelaFreteSubContratacao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteSubContratacao cargaTabelaFreteSubContratacao = repCargaTabelaFreteSubContratacao.BuscarPorCarga(carga.Codigo);

                if (cargaTabelaFreteSubContratacao != null)
                {
                    RatearFreteEntrePedidos(carga, cargaPedidos, configEmbarcador, valorFrete, rateioFreteFilialEmissora, tipoServicoMultisoftware, unitOfWork);
                }
                else if (carga.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.AutorizacaoEmissao)
                {
                    RatearFreteEntrePedidos(carga, cargaPedidos, configEmbarcador, valorFrete, rateioFreteFilialEmissora, tipoServicoMultisoftware, unitOfWork);
                }
                else if (carga.ContratoFreteTransportador != null)
                {
                    RatearFreteEntrePedidos(carga, cargaPedidos, configEmbarcador, valorFrete, rateioFreteFilialEmissora, tipoServicoMultisoftware, unitOfWork);
                }
                else if (
                    (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && (carga.TipoFreteEscolhido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Operador || carga.TipoFreteEscolhido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)) ||
                    (configEmbarcador.PermitirOperadorInformarValorFreteMaiorQueTabela && carga.TipoFreteEscolhido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Operador)
                )
                {
                    //Servicos.Log.TratarErro("RatearValorDoFrenteEntrePedidos 3 Carga " + carga.CodigoCargaEmbarcador);

                    RatearFreteEntrePedidos(carga, cargaPedidos, configEmbarcador, valorFrete, rateioFreteFilialEmissora, tipoServicoMultisoftware, unitOfWork);

                    if (!rateioFreteFilialEmissora && (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || !carga.ApoliceSeguroInformadaManualmente))
                        Servicos.Embarcador.Seguro.Seguro.SetarDadosSeguroCarga(carga, cargaPedidos, configEmbarcador, tipoServicoMultisoftware, unitOfWork);
                }
                else if (configuracaoRateio.ValorFreteInformadoPeloTransportador)
                    RatearFreteEntrePedidos(carga, cargaPedidos, configEmbarcador, valorFrete, rateioFreteFilialEmissora, tipoServicoMultisoftware, unitOfWork);
                else if (carga.TipoFreteEscolhido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador && (carga.TipoOperacao?.ConfiguracaoCalculoFrete?.RatearValorFreteInformadoEmbarcador ?? false))
                    RatearFreteEntrePedidos(carga, cargaPedidos, configEmbarcador, valorFrete, rateioFreteFilialEmissora, tipoServicoMultisoftware, unitOfWork);

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && carga.TipoFreteEscolhido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                {
                    Servicos.Embarcador.Carga.ComponetesFrete serCargaComponetesFrete = new Servicos.Embarcador.Carga.ComponetesFrete(unitOfWork);
                    Repositorio.Embarcador.ICMS.PedagioEstadoBaseCalculo repPedagioEstadoBaseCalculo = new Repositorio.Embarcador.ICMS.PedagioEstadoBaseCalculo(unitOfWork);
                    Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete repositorioConfiguracaoTabelaFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete(unitOfWork);

                    if (cargaPedidosLiberados.Count > 0)
                        ZerarValoresDaCarga(carga, rateioFreteFilialEmissora, unitOfWork);

                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidosComponentesFreteCarga = repCargaPedidoComponenteFrete.BuscarPorCarga(carga.Codigo, rateioFreteFilialEmissora);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFretesDiretos = new List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();
                    List<Dominio.Entidades.Embarcador.ICMS.PedagioEstadoBaseCalculo> pedagioEstadosBaseCalculo = repPedagioEstadoBaseCalculo.BuscarPorEstados((from obj in cargaPedidosLiberados select obj.Origem.Estado.Sigla).Distinct().ToList());
                    List<Dominio.ObjetosDeValor.Embarcador.ICMS.TabelaAliquota> tabelaAliquotas = new List<Dominio.ObjetosDeValor.Embarcador.ICMS.TabelaAliquota>();
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = serCargaICMS.ObterProdutosCargaContidosEmRegras(carga, unitOfWork);
                    List<Dominio.Entidades.Cliente> tomadoresFilialEmissora = Servicos.Embarcador.Carga.FilialEmissora.ObterTomadoresFilialEmissora((from obj in cargaPedidos where obj.CargaPedidoFilialEmissora select obj.CargaOrigem.EmpresaFilialEmissora.CNPJ_SemFormato).Distinct().ToList(), unitOfWork);
                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigem = Servicos.Embarcador.Carga.Carga.ObterCargasOrigem(carga, unitOfWork);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidosFilialEmissoraComponentesFreteCargaImpostos = repCargaPedidoComponenteFrete.BuscarPorCargaComponentesImpostos(carga.Codigo, true);
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete = repositorioConfiguracaoTabelaFrete.BuscarPrimeiroRegistro();

                    bool possuiFilialEmissoraTrechoAnterior = false;

                    if (carga.EmpresaFilialEmissora != null && cargaPedidosLiberados.Count > 0)
                        ZerarValoresDaCarga(carga, true, unitOfWork);

                    bool abriuTransacao = false;
                    if (!unitOfWork.IsActiveTransaction())
                    {
                        unitOfWork.Start();
                        abriuTransacao = true;
                    }

                    if (!Servicos.Embarcador.Carga.CTeSimplificado.ValidarCTeSimplificado(cargaPedidos, unitOfWork, tipoServicoMultisoftware, configEmbarcador))
                        Servicos.Embarcador.Carga.CTeGlobalizado.ValidarCTeGlobalizadoPorDestinatario(cargaPedidos, unitOfWork, tipoServicoMultisoftware, configEmbarcador);

                    Servicos.Embarcador.Carga.RateioFrete serRateioFrete = new Embarcador.Carga.RateioFrete(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> pedidosAgrupados = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
                    int i = 0;
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidosLiberados)
                    {
                        Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem = (from obj in cargasOrigem where obj.Codigo == cargaPedido.CargaOrigem.Codigo select obj).FirstOrDefault();

                        i++;
                        if (!cargaPedido.ImpostoInformadoPeloEmbarcador)
                        {
                            decimal valorFretePedido = cargaPedido.ValorFrete;
                            if (rateioFreteFilialEmissora)
                                valorFretePedido = cargaPedido.ValorFreteFilialEmissora;

                            if (cargaPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoAgrupado
                                || cargaPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupada
                                || cargaPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupadaEntrePedidos
                                || cargaPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.NaoInformado || cargaPedido.IndicadorCTeGlobalizadoDestinatario)
                            {
                                if (!rateioFreteFilialEmissora)
                                    cargaPedido.ValorFrete = Math.Round(valorFretePedido, 2, MidpointRounding.AwayFromZero);
                                else
                                    cargaPedido.ValorFreteFilialEmissora = Math.Round(valorFretePedido, 2, MidpointRounding.AwayFromZero);

                                pedidosAgrupados.Add(cargaPedido);
                            }
                            else
                            {
                                serRateioFrete.CalcularImpostos(ref carga, cargaOrigem, cargaPedido, valorFretePedido, rateioFreteFilialEmissora, tipoServicoMultisoftware, unitOfWork, configEmbarcador, cargaPedidosComponentesFreteCarga, pedagioEstadosBaseCalculo, cargaPedidoProdutos, tabelaAliquotas, tomadoresFilialEmissora, configuracaoGeralCarga);
                            }
                        }
                        else
                        {
                            if (!rateioFreteFilialEmissora)
                            {
                                Dominio.ObjetosDeValor.Embarcador.Carga.CargaValoresAcrescentar cargaValoresAcrescentar = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaValoresAcrescentar()
                                {
                                    ValorFrete = cargaPedido.ValorFrete,
                                    ValorFreteLiquido = 0m,
                                    ValorFreteAPagar = cargaPedido.ValorFreteAPagar,
                                    ValorICMS = cargaPedido.ValorICMS,
                                    ValorPis = cargaPedido.ValorPis,
                                    ValorCofins = cargaPedido.ValorCofins,
                                    ValorISS = cargaPedido.ValorISS,
                                    ValorRetencaoISS = cargaPedido.ValorRetencaoISS,
                                    ValorIBSEstadual = cargaPedido.ValorIBSEstadual,
                                    ValorIBSMunicipal = cargaPedido.ValorIBSMunicipal,
                                    ValorCBS = cargaPedido.ValorCBS,
                                };

                                AcrescentarValoresDaCarga(carga, cargaValoresAcrescentar);
                            }
                            else
                            {
                                Dominio.ObjetosDeValor.Embarcador.Carga.CargaValoresAcrescentar cargaValoresAcrescentar = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaValoresAcrescentar()
                                {
                                    ValorFrete = cargaPedido.ValorFreteFilialEmissora,
                                    ValorFreteAPagar = cargaPedido.ValorFreteAPagarFilialEmissora,
                                    ValorICMS = cargaPedido.ValorICMSFilialEmissora,
                                    ValorIBSEstadual = cargaPedido.ValorIBSEstadualFilialEmissora,
                                    ValorIBSMunicipal = cargaPedido.ValorIBSMunicipalFilialEmissora,
                                    ValorCBS = cargaPedido.ValorCBSFilialEmissora,
                                };

                                AcrescentarValoresFilialEmissoraDaCarga(carga, cargaValoresAcrescentar);
                            }
                        }

                        if (!rateioFreteFilialEmissora)
                            InformarDadosContabeisCargaPedido(cargaPedido, cargaOrigem, configEmbarcador, tipoServicoMultisoftware, unitOfWork);

                        //if (cargaPedido.ValorFreteAPagar > 0)
                        //    Servicos.Embarcador.Carga.FreteFilialEmissora.SetarFreteEmbarcadorFilialEmissora(ref carga, cargaPedido, tipoServicoMultisoftware, true, unitOfWork, configEmbarcador, cargaPedidosComponentesFreteCarga, pedagioEstadosBaseCalculo, cargaPedidoProdutos, tabelaAliquotas, tomadoresFilialEmissora, cargaPedidosFilialEmissoraComponentesFreteCargaImpostos, componenteFreteICMS, out possuiFilialEmissoraTrechoAnterior);
                    }


                    if (abriuTransacao)
                        unitOfWork.CommitChanges();

                    if (possuiFilialEmissoraTrechoAnterior)
                        Servicos.Embarcador.Carga.FreteFilialEmissora.SetarValorFreteFilialTrechoAnterior(ref carga, false, tipoServicoMultisoftware, unitOfWork, configEmbarcador);

                    serCargaComponetesFrete.AdicionarComponentesCargaAgrupada(carga, rateioFreteFilialEmissora, cargaPedidosComponentesFreteCarga, unitOfWork);
                    AcrescentarValoresDaCargaAgrupada(carga, rateioFreteFilialEmissora, cargaPedidos, unitOfWork);

                    if (pedidosAgrupados.Count > 0)
                    {
                        List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidosValoresNota = repPedidoXMLNotaFiscal.BuscarTotalSumarizadoPorCarga(carga.Codigo);

                        abriuTransacao = false;
                        if (!unitOfWork.IsActiveTransaction())
                        {
                            unitOfWork.Start();
                            abriuTransacao = true;
                        }
                        CalcularImpostosAgrupados(ref carga, cargasOrigem, pedidosAgrupados, rateioFreteFilialEmissora, tipoServicoMultisoftware, unitOfWork, configEmbarcador, cargaPedidosComponentesFreteCarga, pedagioEstadosBaseCalculo, cargaPedidoProdutos, tabelaAliquotas, tomadoresFilialEmissora, cargaPedidosValoresNota, configuracaoTabelaFrete, configuracaoGeralCarga);

                        if (abriuTransacao)
                            unitOfWork.CommitChanges();
                    }

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                    {
                        cargaPedido.ValorBaseFrete = cargaPedido.ValorFreteAPagar;
                        if (carga.MaiorValorBaseFreteDosPedidos < cargaPedido.ValorBaseFrete)
                            carga.MaiorValorBaseFreteDosPedidos = cargaPedido.ValorBaseFrete;
                    }

                    if (carga.EmpresaFilialEmissora != null)
                    {
                        serCargaComponetesFrete.AdicionarComponentesCargaAgrupada(carga, true, cargaPedidosComponentesFreteCarga, unitOfWork);
                        AcrescentarValoresDaCargaAgrupada(carga, true, cargaPedidos, unitOfWork);
                    }

                    if (carga.ExigeNotaFiscalParaCalcularFrete)
                    {
                        RateioNotaFiscal serRateioNotaFiscal = new RateioNotaFiscal(unitOfWork);
                        serRateioNotaFiscal.RatearFreteCargaPedidoEntreNotas(carga, cargaPedidos, cargaPedidosComponentesFreteCarga, rateioFreteFilialEmissora, tipoServicoMultisoftware, unitOfWork, configEmbarcador);
                    }

                    Servicos.Embarcador.Carga.FreteFilialEmissora.VerificarCargaAguardaValorRedespacho(ref carga, unitOfWork);

                    if (!rateioFreteFilialEmissora)
                        Servicos.Embarcador.Seguro.Seguro.SetarDadosSeguroCarga(carga, cargaPedidos, configEmbarcador, tipoServicoMultisoftware, unitOfWork);
                }
            }

            if (cargaPedidos.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidosDesconsideradosNoRateio)
                {
                    if (!rateioFreteFilialEmissora)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Carga.CargaValoresAcrescentar cargaValoresAcrescentar = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaValoresAcrescentar()
                        {
                            ValorFrete = cargaPedido.ValorFrete,
                            ValorFreteLiquido = cargaPedido.ValorFrete,
                            ValorFreteAPagar = cargaPedido.ValorFreteAPagar,
                            ValorICMS = cargaPedido.ValorICMS,
                            ValorPis = cargaPedido.ValorPis,
                            ValorCofins = cargaPedido.ValorCofins,
                            ValorISS = cargaPedido.ValorISS,
                            ValorRetencaoISS = cargaPedido.ValorRetencaoISS,
                            ValorIBSEstadual = cargaPedido.ValorIBSEstadual,
                            ValorIBSMunicipal = cargaPedido.ValorIBSMunicipal,
                            ValorCBS = cargaPedido.ValorCBS,
                        };

                        AcrescentarValoresDaCarga(carga, cargaValoresAcrescentar);
                    }
                    else
                    {
                        Dominio.ObjetosDeValor.Embarcador.Carga.CargaValoresAcrescentar cargaValoresAcrescentar = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaValoresAcrescentar()
                        {
                            ValorFrete = cargaPedido.ValorFreteFilialEmissora,
                            ValorFreteAPagar = cargaPedido.ValorFreteAPagarFilialEmissora,
                            ValorICMS = cargaPedido.ValorICMSFilialEmissora,
                            ValorIBSEstadual = cargaPedido.ValorIBSEstadualFilialEmissora,
                            ValorIBSMunicipal = cargaPedido.ValorIBSMunicipalFilialEmissora,
                            ValorCBS = cargaPedido.ValorCBSFilialEmissora,
                        };

                        AcrescentarValoresFilialEmissoraDaCarga(carga, cargaValoresAcrescentar);
                    }
                }
            }

            Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new CargaDadosSumarizados(unitOfWork);
            serCargaDadosSumarizados.AtualizarTiposDocumentos(carga, cargaPedidos, unitOfWork);

            if (!cargaPedidos.Any(o => o.CTeEmitidoNoEmbarcador))
            {
                Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFreteISS = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidosComponentesFreteCargaImpostos = repCargaPedidoComponenteFrete.BuscarPorCargaComponentesImpostos(carga.Codigo, rateioFreteFilialEmissora);

                bool abriuTransacaoICMS = false;
                if (!unitOfWork.IsActiveTransaction())
                {
                    unitOfWork.Start();
                    abriuTransacaoICMS = true;
                }

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidosLiberados)
                {
                    GerarComponenteICMS(cargaPedido, rateioFreteFilialEmissora, componenteFreteICMS, cargaPedidosComponentesFreteCargaImpostos, unitOfWork);

                    if (!rateioFreteFilialEmissora)
                        GerarComponenteISS(cargaPedido, componenteFreteISS, cargaPedidosComponentesFreteCargaImpostos, false, unitOfWork);

                    if (!rateioFreteFilialEmissora)
                        GerarComponentePisCofins(cargaPedido, componenteFretePisCONFIS, cargaPedidosComponentesFreteCargaImpostos, unitOfWork);
                }

                if (abriuTransacaoICMS)
                    unitOfWork.CommitChanges();

                GerarComponenteICMS(carga, cargaPedidosLiberados, false, rateioFreteFilialEmissora, unitOfWork);
                if (!rateioFreteFilialEmissora)
                    GerarComponenteISS(carga, cargaPedidosLiberados, unitOfWork);

                if (!rateioFreteFilialEmissora)
                    GerarComponentePisCofins(carga, cargaPedidosLiberados, false, unitOfWork);
            }

            Servicos.Embarcador.Frete.ContratoFreteTransportador.GerarSaldoContratoPorCarga(carga, configEmbarcador, tipoServicoMultisoftware, unitOfWork);
        }

        public void GerarComponentePisCofins(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);

            Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS);
            Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponenteFrete = repCargaPedidoComponenteFrete.BuscarPorCompomente(cargaPedido.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS, null, false);

            decimal ValorPISCONFIS = cargaPedido.ValorPis + cargaPedido.ValorCofins;

            if (ValorPISCONFIS > 0m)
            {
                if (cargaPedidoComponenteFrete == null)
                    cargaPedidoComponenteFrete = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete();

                cargaPedidoComponenteFrete.NaoSomarValorTotalAReceber = false;
                cargaPedidoComponenteFrete.NaoSomarValorTotalPrestacao = false;
                cargaPedidoComponenteFrete.AcrescentaValorTotalAReceber = false;
                cargaPedidoComponenteFrete.CargaPedido = cargaPedido;
                cargaPedidoComponenteFrete.ComponenteFrete = componenteFrete;
                cargaPedidoComponenteFrete.DescontarValorTotalAReceber = false;
                cargaPedidoComponenteFrete.IncluirBaseCalculoICMS = false;
                cargaPedidoComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro = false;
                cargaPedidoComponenteFrete.TipoComponenteFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS;
                cargaPedidoComponenteFrete.ValorComponente = ValorPISCONFIS;

                if (cargaPedidoComponenteFrete.Codigo > 0)
                    repCargaPedidoComponenteFrete.Atualizar(cargaPedidoComponenteFrete);
                else
                    repCargaPedidoComponenteFrete.Inserir(cargaPedidoComponenteFrete);
            }
            else if (cargaPedidoComponenteFrete != null)
            {
                repCargaPedidoComponenteFrete.Deletar(cargaPedidoComponenteFrete);
            }
        }

        public void GerarComponenteICMS(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, bool componenteFilialEmissora, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);

            Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS);
            Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponenteFrete = repCargaPedidoComponenteFrete.BuscarPorCompomente(cargaPedido.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS, null, componenteFilialEmissora);

            string CST = cargaPedido.CST;
            decimal valorICMS = cargaPedido.ValorICMS;
            decimal valorICMSIncluso = cargaPedido.ValorICMSIncluso;
            bool incluirICMSBaseCalculo = cargaPedido.IncluirICMSBaseCalculo;
            if (componenteFilialEmissora)
            {
                CST = cargaPedido.CSTFilialEmissora;
                valorICMS = cargaPedido.ValorICMSFilialEmissora;
                incluirICMSBaseCalculo = cargaPedido.IncluirICMSBaseCalculoFilialEmissora;
            }

            if (incluirICMSBaseCalculo && CST != "60" && (valorICMS > 0m || valorICMSIncluso > 0m))
            {
                if (cargaPedidoComponenteFrete == null)
                    cargaPedidoComponenteFrete = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete();

                cargaPedidoComponenteFrete.NaoSomarValorTotalAReceber = false;
                cargaPedidoComponenteFrete.NaoSomarValorTotalPrestacao = false;
                cargaPedidoComponenteFrete.AcrescentaValorTotalAReceber = false;
                cargaPedidoComponenteFrete.CargaPedido = cargaPedido;
                cargaPedidoComponenteFrete.ComponenteFilialEmissora = componenteFilialEmissora;
                cargaPedidoComponenteFrete.ComponenteFrete = componenteFrete;
                cargaPedidoComponenteFrete.DescontarValorTotalAReceber = false;
                cargaPedidoComponenteFrete.IncluirBaseCalculoICMS = false;
                cargaPedidoComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro = false;
                cargaPedidoComponenteFrete.TipoComponenteFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS;
                cargaPedidoComponenteFrete.ValorComponente = valorICMSIncluso > 0m ? valorICMSIncluso : valorICMS;

                if (cargaPedidoComponenteFrete.Codigo > 0)
                    repCargaPedidoComponenteFrete.Atualizar(cargaPedidoComponenteFrete);
                else
                    repCargaPedidoComponenteFrete.Inserir(cargaPedidoComponenteFrete);
            }
            else if (cargaPedidoComponenteFrete != null)
            {
                repCargaPedidoComponenteFrete.Deletar(cargaPedidoComponenteFrete);
            }

            if (!componenteFilialEmissora)
                GerarComponentePisCofins(cargaPedido, unitOfWork);
        }

        public void GerarComponentePisCofins(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponenteFrete = (from obj in cargaPedidoComponentesCarga where obj.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS && obj.CargaPedido.Codigo == cargaPedido.Codigo && obj.ComponenteFilialEmissora == false select obj).FirstOrDefault();

            decimal valorPISCONFIS = cargaPedido.ValorPis + cargaPedido.ValorCofins;

            if (valorPISCONFIS > 0m)
            {
                if (cargaPedidoComponenteFrete == null)
                    cargaPedidoComponenteFrete = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete();

                cargaPedidoComponenteFrete.NaoSomarValorTotalAReceber = false;
                cargaPedidoComponenteFrete.NaoSomarValorTotalPrestacao = false;
                cargaPedidoComponenteFrete.AcrescentaValorTotalAReceber = false;
                cargaPedidoComponenteFrete.CargaPedido = cargaPedido;
                cargaPedidoComponenteFrete.ComponenteFrete = componenteFrete;
                cargaPedidoComponenteFrete.DescontarValorTotalAReceber = false;
                cargaPedidoComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro = false;
                cargaPedidoComponenteFrete.IncluirBaseCalculoICMS = false;
                cargaPedidoComponenteFrete.TipoComponenteFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS;
                cargaPedidoComponenteFrete.ValorComponente = valorPISCONFIS;

                if (cargaPedidoComponenteFrete.Codigo > 0)
                    repCargaPedidoComponenteFrete.Atualizar(cargaPedidoComponenteFrete);
                else
                    repCargaPedidoComponenteFrete.Inserir(cargaPedidoComponenteFrete);
            }
            else if (cargaPedidoComponenteFrete != null)
            {
                repCargaPedidoComponenteFrete.Deletar(cargaPedidoComponenteFrete);
            }
        }

        public void GerarComponenteICMS(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, bool componenteFilialEmissora, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponenteFrete = (from obj in cargaPedidoComponentesCarga where obj.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS && obj.CargaPedido.Codigo == cargaPedido.Codigo && obj.ComponenteFilialEmissora == componenteFilialEmissora select obj).FirstOrDefault();
            string CST = cargaPedido.CST;
            decimal valorICMS = cargaPedido.ValorICMS;
            decimal valorICMSIncluso = cargaPedido.ValorICMSIncluso;
            bool incluirICMSBaseCalculo = cargaPedido.IncluirICMSBaseCalculo;
            if (componenteFilialEmissora)
            {
                CST = cargaPedido.CSTFilialEmissora;
                valorICMS = cargaPedido.ValorICMSFilialEmissora;
                incluirICMSBaseCalculo = cargaPedido.IncluirICMSBaseCalculoFilialEmissora;
            }

            if (incluirICMSBaseCalculo && CST != "60" && (valorICMS > 0m || valorICMSIncluso > 0m))
            {
                if (cargaPedidoComponenteFrete == null)
                    cargaPedidoComponenteFrete = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete();

                cargaPedidoComponenteFrete.NaoSomarValorTotalAReceber = false;
                cargaPedidoComponenteFrete.NaoSomarValorTotalPrestacao = false;
                cargaPedidoComponenteFrete.AcrescentaValorTotalAReceber = false;
                cargaPedidoComponenteFrete.CargaPedido = cargaPedido;
                cargaPedidoComponenteFrete.ComponenteFilialEmissora = componenteFilialEmissora;
                cargaPedidoComponenteFrete.ComponenteFrete = componenteFrete;
                cargaPedidoComponenteFrete.DescontarValorTotalAReceber = false;
                cargaPedidoComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro = false;
                cargaPedidoComponenteFrete.IncluirBaseCalculoICMS = false;
                cargaPedidoComponenteFrete.TipoComponenteFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS;
                cargaPedidoComponenteFrete.ValorComponente = valorICMSIncluso > 0m ? valorICMSIncluso : valorICMS;

                if (cargaPedidoComponenteFrete.Codigo > 0)
                    repCargaPedidoComponenteFrete.Atualizar(cargaPedidoComponenteFrete);
                else
                    repCargaPedidoComponenteFrete.Inserir(cargaPedidoComponenteFrete);
            }
            else if (cargaPedidoComponenteFrete != null)
            {
                repCargaPedidoComponenteFrete.Deletar(cargaPedidoComponenteFrete);
            }
        }

        public void GerarComponentePisCofins(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, decimal valorPISCONFIS, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);

            Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS);
            Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponenteFrete = repCargaPedidoComponenteFrete.BuscarPorCompomente(cargaPedido.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS, null, false);

            if (valorPISCONFIS > 0m)
            {
                if (cargaPedidoComponenteFrete == null)
                    cargaPedidoComponenteFrete = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete();

                cargaPedidoComponenteFrete.NaoSomarValorTotalAReceber = false;
                cargaPedidoComponenteFrete.NaoSomarValorTotalPrestacao = false;
                cargaPedidoComponenteFrete.AcrescentaValorTotalAReceber = false;
                cargaPedidoComponenteFrete.CargaPedido = cargaPedido;
                cargaPedidoComponenteFrete.ComponenteFrete = componenteFrete;
                cargaPedidoComponenteFrete.DescontarValorTotalAReceber = false;
                cargaPedidoComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro = false;
                cargaPedidoComponenteFrete.IncluirBaseCalculoICMS = false;
                cargaPedidoComponenteFrete.TipoComponenteFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS;
                cargaPedidoComponenteFrete.ValorComponente = valorPISCONFIS;

                if (cargaPedidoComponenteFrete.Codigo > 0)
                    repCargaPedidoComponenteFrete.Atualizar(cargaPedidoComponenteFrete);
                else
                    repCargaPedidoComponenteFrete.Inserir(cargaPedidoComponenteFrete);
            }
            else if (cargaPedidoComponenteFrete != null)
            {
                repCargaPedidoComponenteFrete.Deletar(cargaPedidoComponenteFrete);
            }

            if (cargaPedido.Carga.CargaAgrupada)
                GerarComponentePisCofins(cargaPedido.CargaOrigem, valorPISCONFIS, unitOfWork, true);
        }

        public void GerarComponenteICMS(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, decimal valorICMS, bool componenteFilialEmissora, Repositorio.UnitOfWork unitOfWork, decimal valorICMSIncluso)
        {
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);

            Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS);
            Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponenteFrete = repCargaPedidoComponenteFrete.BuscarPorCompomente(cargaPedido.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS, null, componenteFilialEmissora);

            if (valorICMS > 0m || valorICMSIncluso > 0m)
            {
                if (cargaPedidoComponenteFrete == null)
                    cargaPedidoComponenteFrete = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete();

                cargaPedidoComponenteFrete.NaoSomarValorTotalAReceber = false;
                cargaPedidoComponenteFrete.NaoSomarValorTotalPrestacao = false;
                cargaPedidoComponenteFrete.AcrescentaValorTotalAReceber = false;
                cargaPedidoComponenteFrete.CargaPedido = cargaPedido;
                cargaPedidoComponenteFrete.ComponenteFrete = componenteFrete;
                cargaPedidoComponenteFrete.DescontarValorTotalAReceber = false;
                cargaPedidoComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro = false;
                cargaPedidoComponenteFrete.ComponenteFilialEmissora = componenteFilialEmissora;
                cargaPedidoComponenteFrete.IncluirBaseCalculoICMS = false;
                cargaPedidoComponenteFrete.TipoComponenteFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS;
                cargaPedidoComponenteFrete.ValorComponente = valorICMSIncluso > 0m ? valorICMSIncluso : valorICMS;

                if (cargaPedidoComponenteFrete.Codigo > 0)
                    repCargaPedidoComponenteFrete.Atualizar(cargaPedidoComponenteFrete);
                else
                    repCargaPedidoComponenteFrete.Inserir(cargaPedidoComponenteFrete);
            }
            else if (cargaPedidoComponenteFrete != null)
            {
                repCargaPedidoComponenteFrete.Deletar(cargaPedidoComponenteFrete);
            }

            if (cargaPedido.Carga.CargaAgrupada)
                GerarComponenteICMS(cargaPedido.CargaOrigem, valorICMS, componenteFilialEmissora, unitOfWork, valorICMSIncluso, true);
        }

        public void GerarComponentePisCofins(Dominio.Entidades.Embarcador.Cargas.Carga carga, decimal valorPisCofins, Repositorio.UnitOfWork unitOfWork, bool somarSeExiste = false)
        {
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponenteFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);

            Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS);
            Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponenteFrete = repCargaComponenteFrete.BuscarPorCargaETipo(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS, null, false);

            if (valorPisCofins > 0m)
            {
                if (cargaComponenteFrete == null)
                    cargaComponenteFrete = new Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete();

                cargaComponenteFrete.AcrescentaValorTotalAReceber = false;
                cargaComponenteFrete.Carga = carga;
                cargaComponenteFrete.ComponenteFrete = componenteFrete;
                cargaComponenteFrete.NaoSomarValorTotalAReceber = false;
                cargaComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro = false;
                cargaComponenteFrete.NaoSomarValorTotalPrestacao = false;
                cargaComponenteFrete.DescontarValorTotalAReceber = false;
                cargaComponenteFrete.IncluirBaseCalculoICMS = false;
                cargaComponenteFrete.TipoComponenteFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS;
                if (somarSeExiste)
                    cargaComponenteFrete.ValorComponente += valorPisCofins;
                else
                    cargaComponenteFrete.ValorComponente = valorPisCofins;

                if (cargaComponenteFrete.Codigo > 0)
                    repCargaComponenteFrete.Atualizar(cargaComponenteFrete);
                else
                    repCargaComponenteFrete.Inserir(cargaComponenteFrete);
            }
            else if (cargaComponenteFrete != null)
            {
                repCargaComponenteFrete.Deletar(cargaComponenteFrete);
            }
        }

        public void GerarComponenteICMS(Dominio.Entidades.Embarcador.Cargas.Carga carga, decimal valorICMS, bool componenteFilialEmissora, Repositorio.UnitOfWork unitOfWork, decimal valorICMSIncluso, bool somarSeExiste = false)
        {
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponenteFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);

            Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS);
            Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponenteFrete = repCargaComponenteFrete.BuscarPorCargaETipo(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS, null, componenteFilialEmissora);

            if (valorICMS > 0m || valorICMSIncluso > 0m)
            {
                if (cargaComponenteFrete == null)
                    cargaComponenteFrete = new Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete();

                cargaComponenteFrete.AcrescentaValorTotalAReceber = false;
                cargaComponenteFrete.Carga = carga;
                cargaComponenteFrete.ComponenteFrete = componenteFrete;
                cargaComponenteFrete.NaoSomarValorTotalAReceber = false;
                cargaComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro = false;
                cargaComponenteFrete.NaoSomarValorTotalPrestacao = false;
                cargaComponenteFrete.DescontarValorTotalAReceber = false;
                cargaComponenteFrete.IncluirBaseCalculoICMS = false;
                cargaComponenteFrete.ComponenteFilialEmissora = componenteFilialEmissora;
                cargaComponenteFrete.TipoComponenteFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS;
                if (somarSeExiste)
                    cargaComponenteFrete.ValorComponente += valorICMSIncluso > 0m ? valorICMSIncluso : valorICMS;
                else
                    cargaComponenteFrete.ValorComponente = valorICMSIncluso > 0m ? valorICMSIncluso : valorICMS;

                if (cargaComponenteFrete.Codigo > 0)
                    repCargaComponenteFrete.Atualizar(cargaComponenteFrete);
                else
                    repCargaComponenteFrete.Inserir(cargaComponenteFrete);
            }
            else if (cargaComponenteFrete != null)
            {
                repCargaComponenteFrete.Deletar(cargaComponenteFrete);
            }
        }

        public void GerarComponentePisCofins(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, bool somarSeExiste, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponenteFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);

            Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS);
            Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponenteFrete = repCargaComponenteFrete.BuscarPorCargaETipo(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS, null, false);
            decimal valorPISCONFINS = cargaPedidos.Sum(o => o.ValorPis + o.ValorCofins);

            if (valorPISCONFINS > 0m)
            {
                if (cargaComponenteFrete == null)
                    cargaComponenteFrete = new Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete();

                cargaComponenteFrete.AcrescentaValorTotalAReceber = false;
                cargaComponenteFrete.NaoSomarValorTotalAReceber = false;
                cargaComponenteFrete.NaoSomarValorTotalPrestacao = false;
                cargaComponenteFrete.Carga = carga;
                cargaComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro = false;
                cargaComponenteFrete.ComponenteFrete = componenteFrete;
                cargaComponenteFrete.DescontarValorTotalAReceber = false;
                cargaComponenteFrete.IncluirBaseCalculoICMS = false;
                cargaComponenteFrete.TipoComponenteFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS;
                if (somarSeExiste)
                    cargaComponenteFrete.ValorComponente += valorPISCONFINS;
                else
                    cargaComponenteFrete.ValorComponente = valorPISCONFINS;

                if (cargaComponenteFrete.Codigo > 0)
                    repCargaComponenteFrete.Atualizar(cargaComponenteFrete);
                else
                    repCargaComponenteFrete.Inserir(cargaComponenteFrete);
            }
            else if (cargaComponenteFrete != null)
            {
                repCargaComponenteFrete.Deletar(cargaComponenteFrete);
            }

            if (carga.CargaAgrupada)
            {
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem in carga.CargasAgrupamento)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidoAgrupamento = repCargaPedido.BuscarPorCargaOrigem(cargaOrigem.Codigo);
                    GerarComponentePisCofins(cargaOrigem, cargasPedidoAgrupamento, somarSeExiste, unitOfWork);
                }
            }
        }

        public void GerarComponenteICMS(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, bool somarSeExiste, bool componenteFilialEmissora, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponenteFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComponentesFreteAuxiliar repCargaComponentesFreteAuxiliar = new Repositorio.Embarcador.Cargas.CargaComponentesFreteAuxiliar(unitOfWork);

            Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS);
            Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponenteFrete = repCargaComponenteFrete.BuscarPorCargaETipo(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS, null, componenteFilialEmissora);
            Dominio.Entidades.Embarcador.Cargas.CargaComponentesFreteAuxiliar cargaComponenteFreteAuxiliar = repCargaComponentesFreteAuxiliar.BuscarPrimeiroPorCargaPorCompomente(cargaPedidos.FirstOrDefault()?.Carga.Codigo ?? 0, componenteFrete);

            decimal valorICMS = 0;
            decimal valorICMSIncluso = 0;

            if (!componenteFilialEmissora)
            {
                valorICMS = cargaPedidos.Where(o => o.IncluirICMSBaseCalculo && o.CST != "60").Sum(o => o.ValorICMS);
                valorICMSIncluso = cargaPedidos.Where(o => o.IncluirICMSBaseCalculo && o.CST != "60").Sum(o => o.ValorICMSIncluso);
            }
            else
            {
                valorICMS = cargaPedidos.Where(o => o.IncluirICMSBaseCalculoFilialEmissora && o.CSTFilialEmissora != "60").Sum(o => o.ValorICMSFilialEmissora);
                valorICMSIncluso = cargaPedidos.Where(o => o.IncluirICMSBaseCalculoFilialEmissora && o.CSTFilialEmissora != "60").Sum(o => o.ValorICMSFilialEmissora);
            }

            if (cargaComponenteFreteAuxiliar != null && cargaComponenteFreteAuxiliar.ValorComponente > 0)
                valorICMSIncluso = cargaComponenteFreteAuxiliar.ValorComponente;

            if (valorICMS > 0m || valorICMSIncluso > 0m)
            {
                if (cargaComponenteFrete == null)
                    cargaComponenteFrete = new Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete();

                cargaComponenteFrete.AcrescentaValorTotalAReceber = false;
                cargaComponenteFrete.NaoSomarValorTotalAReceber = false;
                cargaComponenteFrete.NaoSomarValorTotalPrestacao = false;
                cargaComponenteFrete.Carga = carga;
                cargaComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro = false;
                cargaComponenteFrete.ComponenteFrete = componenteFrete;
                cargaComponenteFrete.ComponenteFilialEmissora = componenteFilialEmissora;
                cargaComponenteFrete.DescontarValorTotalAReceber = false;
                cargaComponenteFrete.IncluirBaseCalculoICMS = false;
                cargaComponenteFrete.TipoComponenteFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS;
                if (somarSeExiste)
                    cargaComponenteFrete.ValorComponente += valorICMSIncluso > 0m ? valorICMSIncluso : valorICMS;
                else
                    cargaComponenteFrete.ValorComponente = valorICMSIncluso > 0m ? valorICMSIncluso : valorICMS;

                if (cargaComponenteFrete.Codigo > 0)
                    repCargaComponenteFrete.Atualizar(cargaComponenteFrete);
                else
                    repCargaComponenteFrete.Inserir(cargaComponenteFrete);
            }
            else if (cargaComponenteFrete != null)
            {
                repCargaComponenteFrete.Deletar(cargaComponenteFrete);
            }

            if (carga.CargaAgrupada)
            {
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem in carga.CargasAgrupamento)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidoAgrupamento = repCargaPedido.BuscarPorCargaOrigem(cargaOrigem.Codigo);
                    GerarComponenteICMS(cargaOrigem, cargasPedidoAgrupamento, somarSeExiste, componenteFilialEmissora, unitOfWork);
                }
            }
        }

        public void GerarComponenteISS(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, decimal valorISS, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe repositorioTransportadorConfiguracaoNFSe = new Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe(unitOfWork);

            Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS);
            Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponenteFrete = repCargaPedidoComponenteFrete.BuscarPorCompomente(cargaPedido.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS, null, false);

            if (valorISS > 0m)
            {
                if (cargaPedidoComponenteFrete == null)
                    cargaPedidoComponenteFrete = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete();

                cargaPedidoComponenteFrete.NaoSomarValorTotalAReceber = false;
                cargaPedidoComponenteFrete.NaoSomarValorTotalPrestacao = false;
                cargaPedidoComponenteFrete.AcrescentaValorTotalAReceber = false;
                cargaPedidoComponenteFrete.CargaPedido = cargaPedido;
                cargaPedidoComponenteFrete.ComponenteFrete = componenteFrete;
                cargaPedidoComponenteFrete.DescontarValorTotalAReceber = false;
                cargaPedidoComponenteFrete.IncluirBaseCalculoICMS = false;
                cargaPedidoComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro = false;
                cargaPedidoComponenteFrete.TipoComponenteFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS;
                cargaPedidoComponenteFrete.ValorComponente = valorISS;

                if (cargaPedido.Carga.Empresa != null && repositorioTransportadorConfiguracaoNFSe.ExisteRealizarArredondamentoCalculoIss(cargaPedido.Carga.Empresa.Codigo))
                    cargaPedidoComponenteFrete.ValorComponente = ObterValorArredondadoIncidenciaISS(cargaPedidoComponenteFrete.ValorComponente);

                if (cargaPedidoComponenteFrete.Codigo > 0)
                    repCargaPedidoComponenteFrete.Atualizar(cargaPedidoComponenteFrete);
                else
                    repCargaPedidoComponenteFrete.Inserir(cargaPedidoComponenteFrete);
            }
            else if (cargaPedidoComponenteFrete != null)
            {
                repCargaPedidoComponenteFrete.Deletar(cargaPedidoComponenteFrete);
            }

            if (cargaPedido.Carga.CargaAgrupada)
                GerarComponenteISS(cargaPedido.CargaOrigem, valorISS, unitOfWork, true);
        }

        public void GerarComponenteISS(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesFreteCarga, bool somarSeExiste, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);

            Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe repositorioTransportadorConfiguracaoNFSe = new Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponenteFrete = (from obj in cargaPedidoComponentesFreteCarga where obj.CargaPedido.Codigo == cargaPedido.Codigo && obj.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS && obj.ComponenteFilialEmissora == false select obj).FirstOrDefault();

            if (cargaPedido.IncluirISSBaseCalculo && cargaPedido.ValorISS > 0m)
            {
                if (cargaPedidoComponenteFrete == null)
                    cargaPedidoComponenteFrete = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete();

                cargaPedidoComponenteFrete.NaoSomarValorTotalAReceber = false;
                cargaPedidoComponenteFrete.NaoSomarValorTotalPrestacao = false;
                cargaPedidoComponenteFrete.AcrescentaValorTotalAReceber = false;
                cargaPedidoComponenteFrete.CargaPedido = cargaPedido;
                cargaPedidoComponenteFrete.ComponenteFrete = componenteFrete;
                cargaPedidoComponenteFrete.DescontarValorTotalAReceber = false;
                cargaPedidoComponenteFrete.IncluirBaseCalculoICMS = false;
                cargaPedidoComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro = false;
                cargaPedidoComponenteFrete.TipoComponenteFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS;
                if (somarSeExiste)
                    cargaPedidoComponenteFrete.ValorComponente += cargaPedido.ValorISS;
                else
                    cargaPedidoComponenteFrete.ValorComponente = cargaPedido.ValorISS;

                if (cargaPedido.Carga.Empresa != null && repositorioTransportadorConfiguracaoNFSe.ExisteRealizarArredondamentoCalculoIss(cargaPedido.Carga.Empresa.Codigo))
                    cargaPedidoComponenteFrete.ValorComponente = ObterValorArredondadoIncidenciaISS(cargaPedidoComponenteFrete.ValorComponente);

                if (cargaPedidoComponenteFrete.Codigo > 0)
                    repCargaPedidoComponenteFrete.Atualizar(cargaPedidoComponenteFrete);
                else
                    repCargaPedidoComponenteFrete.Inserir(cargaPedidoComponenteFrete);
            }
            else if (cargaPedidoComponenteFrete != null)
            {
                repCargaPedidoComponenteFrete.Deletar(cargaPedidoComponenteFrete);
            }

            if (cargaPedido.Carga.CargaAgrupada)
                GerarComponenteISS(cargaPedido.CargaOrigem, cargaPedido.ValorISS, unitOfWork, true);
        }

        public void GerarComponenteISS(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, bool somarSeExiste, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe repositorioTransportadorConfiguracaoNFSe = new Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe(unitOfWork);

            Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS);
            Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponenteFrete = repCargaPedidoComponenteFrete.BuscarPorCompomente(cargaPedido.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS, null, false);

            if (cargaPedido.IncluirISSBaseCalculo && cargaPedido.ValorISS > 0m)
            {
                if (cargaPedidoComponenteFrete == null)
                    cargaPedidoComponenteFrete = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete();

                cargaPedidoComponenteFrete.NaoSomarValorTotalAReceber = false;
                cargaPedidoComponenteFrete.NaoSomarValorTotalPrestacao = false;
                cargaPedidoComponenteFrete.AcrescentaValorTotalAReceber = false;
                cargaPedidoComponenteFrete.CargaPedido = cargaPedido;
                cargaPedidoComponenteFrete.ComponenteFrete = componenteFrete;
                cargaPedidoComponenteFrete.DescontarValorTotalAReceber = false;
                cargaPedidoComponenteFrete.IncluirBaseCalculoICMS = false;
                cargaPedidoComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro = false;
                cargaPedidoComponenteFrete.TipoComponenteFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS;
                if (somarSeExiste)
                    cargaPedidoComponenteFrete.ValorComponente += cargaPedido.ValorISS;
                else
                    cargaPedidoComponenteFrete.ValorComponente = cargaPedido.ValorISS;

                if (cargaPedido.Carga.Empresa != null && repositorioTransportadorConfiguracaoNFSe.ExisteRealizarArredondamentoCalculoIss(cargaPedido.Carga.Empresa.Codigo))
                    cargaPedidoComponenteFrete.ValorComponente = ObterValorArredondadoIncidenciaISS(cargaPedidoComponenteFrete.ValorComponente);

                if (cargaPedidoComponenteFrete.Codigo > 0)
                    repCargaPedidoComponenteFrete.Atualizar(cargaPedidoComponenteFrete);
                else
                    repCargaPedidoComponenteFrete.Inserir(cargaPedidoComponenteFrete);

            }
            else if (cargaPedidoComponenteFrete != null)
            {
                repCargaPedidoComponenteFrete.Deletar(cargaPedidoComponenteFrete);
            }

            if (cargaPedido.Carga.CargaAgrupada)
                GerarComponenteISS(cargaPedido.CargaOrigem, cargaPedido.ValorISS, unitOfWork, true);
        }

        public void GerarComponenteISS(Dominio.Entidades.Embarcador.Cargas.Carga carga, decimal valorISS, Repositorio.UnitOfWork unitOfWork, bool somaSeExiste = false)
        {
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponenteFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe repositorioTransportadorConfiguracaoNFSe = new Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe(unitOfWork);

            Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS);
            Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponenteFrete = repCargaComponenteFrete.BuscarPorCargaETipo(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS, null, false);

            if (valorISS > 0m)
            {
                if (cargaComponenteFrete == null)
                    cargaComponenteFrete = new Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete();

                cargaComponenteFrete.AcrescentaValorTotalAReceber = false;
                cargaComponenteFrete.NaoSomarValorTotalAReceber = false;
                cargaComponenteFrete.NaoSomarValorTotalPrestacao = false;
                cargaComponenteFrete.Carga = carga;
                cargaComponenteFrete.ComponenteFrete = componenteFrete;
                cargaComponenteFrete.DescontarValorTotalAReceber = false;
                cargaComponenteFrete.IncluirBaseCalculoICMS = false;
                cargaComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro = false;
                cargaComponenteFrete.TipoComponenteFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS;
                if (somaSeExiste)
                    cargaComponenteFrete.ValorComponente += valorISS;
                else
                    cargaComponenteFrete.ValorComponente = valorISS;

                if (carga.Empresa != null && repositorioTransportadorConfiguracaoNFSe.ExisteRealizarArredondamentoCalculoIss(carga.Empresa.Codigo))
                    cargaComponenteFrete.ValorComponente = ObterValorArredondadoIncidenciaISS(cargaComponenteFrete.ValorComponente);

                if (cargaComponenteFrete.Codigo > 0)
                    repCargaComponenteFrete.Atualizar(cargaComponenteFrete);
                else
                    repCargaComponenteFrete.Inserir(cargaComponenteFrete);
            }
            else if (cargaComponenteFrete != null)
            {
                repCargaComponenteFrete.Deletar(cargaComponenteFrete);
            }
        }

        public void GerarComponenteISS(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork, bool somaSeExiste = false)
        {
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponenteFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe repositorioTransportadorConfiguracaoNFSe = new Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe(unitOfWork);

            Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS);
            Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponenteFrete = repCargaComponenteFrete.BuscarPorCargaETipo(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS, null, false);

            decimal valorISS = cargaPedidos.Where(o => o.IncluirISSBaseCalculo).Sum(o => o.ValorISS);

            if (valorISS > 0m)
            {
                if (cargaComponenteFrete == null)
                    cargaComponenteFrete = new Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete();

                cargaComponenteFrete.AcrescentaValorTotalAReceber = false;
                cargaComponenteFrete.NaoSomarValorTotalAReceber = false;
                cargaComponenteFrete.NaoSomarValorTotalPrestacao = false;
                cargaComponenteFrete.Carga = carga;
                cargaComponenteFrete.ComponenteFrete = componenteFrete;
                cargaComponenteFrete.DescontarValorTotalAReceber = false;
                cargaComponenteFrete.IncluirBaseCalculoICMS = false;
                cargaComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro = false;
                cargaComponenteFrete.TipoComponenteFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS;
                if (somaSeExiste)
                    cargaComponenteFrete.ValorComponente += valorISS;
                else
                    cargaComponenteFrete.ValorComponente = valorISS;

                if (carga.Empresa != null && repositorioTransportadorConfiguracaoNFSe.ExisteRealizarArredondamentoCalculoIss(carga.Empresa.Codigo))
                    cargaComponenteFrete.ValorComponente = ObterValorArredondadoIncidenciaISS(cargaComponenteFrete.ValorComponente);

                if (cargaComponenteFrete.Codigo > 0)
                    repCargaComponenteFrete.Atualizar(cargaComponenteFrete);
                else
                    repCargaComponenteFrete.Inserir(cargaComponenteFrete);
            }
            else if (cargaComponenteFrete != null)
            {
                repCargaComponenteFrete.Deletar(cargaComponenteFrete);
            }

            if (carga.CargaAgrupada)
            {
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem in carga.CargasAgrupamento)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidoAgrupamento = repCargaPedido.BuscarPorCargaOrigem(cargaOrigem.Codigo);
                    GerarComponenteISS(cargaOrigem, cargasPedidoAgrupamento, unitOfWork, somaSeExiste);
                }
            }
        }

        public void CalcularImpostosAgrupados(ref Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigem, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, bool rateioFreteFilialEmissora, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesFretesCarga, List<Dominio.Entidades.Embarcador.ICMS.PedagioEstadoBaseCalculo> pedagioEstadosBaseCalculo, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos, List<Dominio.ObjetosDeValor.Embarcador.ICMS.TabelaAliquota> tabelaAliquotas, List<Dominio.Entidades.Cliente> tomadoresFilialEmissora, List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidosValoresNota, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga)
        {
            List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tiposCargaPedido = new List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();
            if (configuracao.EmitirCTesSeparandoPorTipoCarga)
                tiposCargaPedido = (from obj in cargaPedidos select obj.Pedido.TipoDeCarga).Distinct().ToList();
            else
                tiposCargaPedido.Add(null);

            for (int tc = 0; tc < tiposCargaPedido.Count; tc++)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosTipoCarga = cargaPedidos;

                if (configuracao.EmitirCTesSeparandoPorTipoCarga)
                    cargaPedidosTipoCarga = (from obj in cargaPedidos where obj.Pedido.TipoDeCarga == tiposCargaPedido[tc] select obj).ToList();

                List<bool> indicadoresNFSGlobalizado = (from obj in cargaPedidosTipoCarga select obj.IndicadorNFSGlobalizado).Distinct().ToList();
                for (int inf = 0; inf < indicadoresNFSGlobalizado.Count; inf++)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosIndicadorNFS = (from obj in cargaPedidosTipoCarga where obj.IndicadorNFSGlobalizado == indicadoresNFSGlobalizado[inf] select obj).ToList();

                    List<bool> indicadoresGlobalizadoDestinatario = (from obj in cargaPedidosIndicadorNFS select obj.IndicadorCTeGlobalizadoDestinatario).Distinct().ToList();
                    for (int idg = 0; idg < indicadoresGlobalizadoDestinatario.Count; idg++)
                    {
                        bool indicadorGlobalizadoDestinatario = indicadoresGlobalizadoDestinatario[idg];
                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosIndicadorGlobalizadoDetinatario = (from obj in cargaPedidosIndicadorNFS where obj.IndicadorCTeGlobalizadoDestinatario == indicadorGlobalizadoDestinatario select obj).ToList();

                        List<bool> redespachos = (from obj in cargaPedidosIndicadorGlobalizadoDetinatario select obj.Redespacho).Distinct().ToList();
                        for (int a = 0; a < redespachos.Count; a++)
                        {
                            bool redespacho = redespachos[a];
                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosEmissaoRedespacho = (from obj in cargaPedidosIndicadorGlobalizadoDetinatario where obj.Redespacho == redespacho select obj).ToList();

                            #region Agrupamento Ct-e Simplificado

                            List<bool> indicadoresCteSimplificado = (from obj in cargaPedidosEmissaoRedespacho select obj.IndicadorCTeSimplificado).Distinct().ToList();
                            for (int ics = 0; a < indicadoresCteSimplificado.Count; a++)
                            {
                                bool indicadorCteSimplificado = indicadoresCteSimplificado[ics];
                                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosIndicadorCteSimplificado = (from obj in cargaPedidosEmissaoRedespacho where obj.IndicadorCTeSimplificado == indicadorCteSimplificado select obj).ToList();

                                #region Processar informações de acordo com a origem

                                List<int> codigoCargasOrigem = (from obj in cargaPedidosIndicadorCteSimplificado select obj.CargaOrigem.Codigo).Distinct().ToList();
                                for (int co = 0; co < codigoCargasOrigem.Count; co++)
                                {
                                    int codigoCargaOrigem = codigoCargasOrigem[co];
                                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosCargaOrigem = (from obj in cargaPedidosEmissaoRedespacho where obj.CargaOrigem.Codigo == codigoCargaOrigem select obj).ToList();

                                    List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos> tipoEmissaoDocumentos = (from obj in cargaPedidosCargaOrigem select obj.TipoRateio).Distinct().ToList();
                                    for (int b = 0; b < tipoEmissaoDocumentos.Count; b++)
                                    {
                                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos tipoEmissaoDocumento = tipoEmissaoDocumentos[b];
                                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosEmissaoDocumento = (from obj in cargaPedidosCargaOrigem where obj.TipoRateio == tipoEmissaoDocumento select obj).ToList();

                                        List<bool> tiposCargaFilialEmissora = (from obj in cargaPedidosEmissaoDocumento select obj.CargaPedidoFilialEmissora).Distinct().ToList();
                                        for (int u = 0; u < tiposCargaFilialEmissora.Count; u++)
                                        {
                                            bool emitirEmitirCTeSub = false;
                                            if (carga.EmpresaFilialEmissora != null && carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                                                emitirEmitirCTeSub = true;
                                            bool tipoCargaPedidoFilialEmissora = tiposCargaFilialEmissora[u];
                                            if (rateioFreteFilialEmissora && !tipoCargaPedidoFilialEmissora && !carga.CalcularFreteCliente)
                                                continue;
                                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosTiposCargaFilialEmissora = (from obj in cargaPedidosEmissaoDocumento where obj.CargaPedidoFilialEmissora == tipoCargaPedidoFilialEmissora select obj).ToList();
                                            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes> tipoEmissaoCTeParticipantes = (from obj in cargaPedidosTiposCargaFilialEmissora select obj.TipoEmissaoCTeParticipantes).Distinct().ToList();
                                            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga> tiposContratacoesCarga = null;
                                            if (!emitirEmitirCTeSub || !tipoCargaPedidoFilialEmissora)
                                                tiposContratacoesCarga = (from obj in cargaPedidosTiposCargaFilialEmissora select obj.TipoContratacaoCarga).Distinct().ToList();
                                            else
                                                tiposContratacoesCarga = (from obj in cargaPedidosTiposCargaFilialEmissora select obj.TipoContratacaoCargaSubContratacaoFilialEmissora).Distinct().ToList();

                                            for (int t = 0; t < tiposContratacoesCarga.Count; t++)
                                            {

                                                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga tipoContratacaoCarga = tiposContratacoesCarga[t];
                                                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosTipoContratacaoCarga = null;

                                                if (!emitirEmitirCTeSub || !tipoCargaPedidoFilialEmissora)
                                                    cargaPedidosTipoContratacaoCarga = (from obj in cargaPedidosTiposCargaFilialEmissora where obj.TipoContratacaoCarga == tipoContratacaoCarga select obj).ToList();
                                                else
                                                    cargaPedidosTipoContratacaoCarga = (from obj in cargaPedidosTiposCargaFilialEmissora where obj.TipoContratacaoCargaSubContratacaoFilialEmissora == tipoContratacaoCarga select obj).ToList();

                                                List<Dominio.Entidades.Cliente> recebedores = (from obj in cargaPedidosTipoContratacaoCarga select obj.Recebedor).Distinct().ToList();
                                                List<Dominio.Entidades.Cliente> expedidores = (from obj in cargaPedidosTipoContratacaoCarga select obj.Expedidor).Distinct().ToList();

                                                if (recebedores.Count <= 0)
                                                    recebedores.Add(null);

                                                if (expedidores.Count <= 0)
                                                    expedidores.Add(null);

                                                for (int c = 0; c < expedidores.Count; c++)
                                                {
                                                    Dominio.Entidades.Cliente expedidor = expedidores[c];
                                                    for (int d = 0; d < recebedores.Count; d++)
                                                    {
                                                        Dominio.Entidades.Cliente recebedor = recebedores[d];

                                                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosIntermediadores = (from obj in cargaPedidosTipoContratacaoCarga where obj.Recebedor == recebedor && obj.Expedidor == expedidor select obj).ToList();
                                                        List<Dominio.Entidades.Cliente> remetentes = (from obj in cargaPedidosIntermediadores select obj.Pedido.Remetente).Distinct().ToList();

                                                        for (int e = 0; e < remetentes.Count; e++)
                                                        {
                                                            Dominio.Entidades.Cliente remetente = remetentes[e];
                                                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosRemetentes = (from obj in cargaPedidosIntermediadores where obj.Pedido.Remetente == remetente select obj).ToList();

                                                            List<Dominio.Entidades.Cliente> destinatarios = null;
                                                            if (indicadorGlobalizadoDestinatario || indicadorCteSimplificado)
                                                            {
                                                                Servicos.Cliente serCliente = new Servicos.Cliente(StringConexao);
                                                                Servicos.WebService.Pessoas.Pessoa serPessoa = new Servicos.WebService.Pessoas.Pessoa(unitOfWork);
                                                                Dominio.ObjetosDeValor.RetornoVerificacaoCliente retorno = serCliente.ConverterObjetoValorPessoa(serPessoa.ConverterObjetoEmpresa(carga.Empresa), "Transportador Destinatário Globalizado", unitOfWork, 0, false);
                                                                destinatarios = new List<Dominio.Entidades.Cliente>();

                                                                if (retorno.Status == true)
                                                                    destinatarios.Add(retorno.cliente);
                                                                else
                                                                {
                                                                    carga.PossuiPendencia = true;
                                                                    carga.MotivoPendencia = retorno.Mensagem;
                                                                }
                                                            }
                                                            else
                                                                destinatarios = (from obj in cargaPedidosRemetentes select obj.Pedido.Destinatario).Distinct().ToList();

                                                            for (int f = 0; f < destinatarios.Count; f++)
                                                            {
                                                                Dominio.Entidades.Cliente destinatario = destinatarios[f];

                                                                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosCliente = null;
                                                                if (!indicadorGlobalizadoDestinatario && !indicadorCteSimplificado)
                                                                    cargaPedidosCliente = (from obj in cargaPedidosRemetentes where obj.Pedido.Destinatario.CPF_CNPJ == destinatario.CPF_CNPJ select obj).ToList();
                                                                else
                                                                    cargaPedidosCliente = cargaPedidosRemetentes;

                                                                List<Dominio.Enumeradores.TipoPagamento> tiposPagamento = (from obj in cargaPedidosCliente select obj.Pedido.TipoPagamento).Distinct().ToList();

                                                                for (int g = 0; g < tiposPagamento.Count; g++)
                                                                {
                                                                    Dominio.Enumeradores.TipoPagamento tipoPagamento = tiposPagamento[g];

                                                                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosTipoPagamento = (from obj in cargaPedidosCliente where obj.Pedido.TipoPagamento == tipoPagamento select obj).ToList();

                                                                    List<Dominio.Enumeradores.TipoTomador> tipoTomadores = (from obj in cargaPedidosTipoPagamento select obj.TipoTomador).Distinct().ToList();

                                                                    for (int h = 0; h < tipoTomadores.Count; h++)
                                                                    {
                                                                        Dominio.Enumeradores.TipoTomador tipoTomador = tipoTomadores[h];

                                                                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosRequisitantes = (from obj in cargaPedidosTipoPagamento where obj.TipoTomador == tipoTomador select obj).ToList();

                                                                        List<Dominio.Entidades.Cliente> tomadores = (from obj in cargaPedidosRequisitantes select obj.Tomador).Distinct().ToList();

                                                                        if (tomadores.Count <= 0)
                                                                            tomadores.Add(null);

                                                                        for (int i = 0; i < tomadores.Count; i++)
                                                                        {
                                                                            Dominio.Entidades.Cliente tomador = tomadores[i];
                                                                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosTomadores = (from obj in cargaPedidosRequisitantes where obj.Tomador == tomador select obj).ToList();

                                                                            List<Dominio.Entidades.Localidade> origensDaPrestacao = (from obj in cargaPedidosTomadores select obj.Origem).Distinct().ToList();

                                                                            for (int j = 0; j < origensDaPrestacao.Count; j++)
                                                                            {
                                                                                Dominio.Entidades.Localidade origem = origensDaPrestacao[j];

                                                                                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosOrigem = (from obj in cargaPedidosTomadores where obj.Origem.Codigo == origem.Codigo select obj).ToList();

                                                                                List<Dominio.Entidades.Localidade> destinosDaPrestacao = null;
                                                                                if (!indicadorGlobalizadoDestinatario && !indicadorCteSimplificado)
                                                                                    destinosDaPrestacao = (from obj in cargaPedidosOrigem select obj.Destino).Distinct().ToList();
                                                                                else
                                                                                {
                                                                                    destinosDaPrestacao = new List<Dominio.Entidades.Localidade>();
                                                                                    destinosDaPrestacao.Add(remetente.Localidade);
                                                                                }

                                                                                List<Dominio.Entidades.Embarcador.Pedidos.Stage> stages = new List<Dominio.Entidades.Embarcador.Pedidos.Stage>();
                                                                                if (!indicadorGlobalizadoDestinatario && !indicadorCteSimplificado)
                                                                                    stages = (from obj in cargaPedidosOrigem select obj.StageRelevanteCusto).Distinct().ToList();
                                                                                else
                                                                                    stages.Add(null);

                                                                                for (int st = 0; st < stages.Count; st++)
                                                                                {
                                                                                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosStages = null;
                                                                                    if (!indicadorGlobalizadoDestinatario && !indicadorCteSimplificado)
                                                                                        cargaPedidosStages = (from obj in cargaPedidosOrigem where obj.StageRelevanteCusto == stages[st] select obj).ToList();
                                                                                    else
                                                                                        cargaPedidosStages = cargaPedidosOrigem;

                                                                                    List<int> codigosCargaPedidoSumarizados = new List<int>();

                                                                                    for (int l = 0; l < destinosDaPrestacao.Count; l++)
                                                                                    {
                                                                                        Dominio.Entidades.Localidade destino = destinosDaPrestacao[l];
                                                                                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosDestinos = null;
                                                                                        if (!indicadorGlobalizadoDestinatario && !indicadorCteSimplificado)
                                                                                            cargaPedidosDestinos = (from obj in cargaPedidosStages where obj.Destino.Codigo == destino.Codigo select obj).ToList();
                                                                                        else
                                                                                            cargaPedidosDestinos = cargaPedidosStages;

                                                                                        List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco> clienteOutrosEnderecoOrigem = (from obj in cargaPedidosStages select obj.Pedido.EnderecoOrigem?.ClienteOutroEndereco).Distinct().ToList();

                                                                                        for (int eo = 0; eo < clienteOutrosEnderecoOrigem.Count; eo++)
                                                                                        {
                                                                                            Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco clienteOutroEnderecoOrigem = clienteOutrosEnderecoOrigem[eo];

                                                                                            List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco> clienteOutrosEnderecoDestino = null;
                                                                                            if (!indicadorGlobalizadoDestinatario && !indicadorCteSimplificado)
                                                                                                clienteOutrosEnderecoDestino = (from obj in cargaPedidosStages where obj.Pedido.EnderecoOrigem?.ClienteOutroEndereco == clienteOutroEnderecoOrigem select obj.Pedido.EnderecoDestino?.ClienteOutroEndereco).Distinct().ToList();
                                                                                            else
                                                                                            {
                                                                                                clienteOutrosEnderecoDestino = new List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco>();
                                                                                                clienteOutrosEnderecoDestino.Add(null);
                                                                                            }

                                                                                            for (int ed = 0; ed < clienteOutrosEnderecoDestino.Count; ed++)
                                                                                            {
                                                                                                Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco clienteOutroEnderecoDestino = clienteOutrosEnderecoDestino[ed];

                                                                                                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosEnderecos = null;
                                                                                                if (!indicadorGlobalizadoDestinatario && !indicadorCteSimplificado)
                                                                                                    cargaPedidosEnderecos = (from obj in cargaPedidosStages where obj.Pedido.EnderecoDestino?.ClienteOutroEndereco == clienteOutroEnderecoDestino && obj.Pedido.EnderecoOrigem?.ClienteOutroEndereco == clienteOutroEnderecoOrigem select obj).ToList();
                                                                                                else
                                                                                                    cargaPedidosEnderecos = (from obj in cargaPedidosStages where obj.Pedido.EnderecoOrigem?.ClienteOutroEndereco == clienteOutroEnderecoOrigem select obj).ToList();

                                                                                                cargaPedidosEnderecos = cargaPedidosEnderecos.Where(x => !codigosCargaPedidoSumarizados.Contains(x.Codigo)).ToList();

                                                                                                codigosCargaPedidoSumarizados.AddRange(cargaPedidosEnderecos.Select(x => x.Codigo));

                                                                                                if (cargaPedidosEnderecos.Count > 0)
                                                                                                    CalcularValoresImpostoAgrupado(ref carga, cargasOrigem, cargaPedidosEnderecos, !rateioFreteFilialEmissora ? cargaPedidosEnderecos.Sum(o => o.ValorFrete) : cargaPedidosEnderecos.Sum(o => o.ValorFreteFilialEmissora), rateioFreteFilialEmissora, tipoServicoMultisoftware, unitOfWork, configuracao, cargaPedidoComponentesFretesCarga, pedagioEstadosBaseCalculo, cargaPedidoProdutos, tabelaAliquotas, tomadoresFilialEmissora, cargaPedidosValoresNota, configuracaoTabelaFrete, configuracaoGeralCarga);

                                                                                            }
                                                                                        }
                                                                                    }// limite ultimo for
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                #endregion Processar informações de acordo com a origem
                            }

                            #endregion Agrupamento Ct-e Simplificado
                        }
                    }
                }
            }
        }

        public void RatearFreteEntrePedidos(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, decimal valorTotalFrete, bool rateioFreteFilialEmissora, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (cargaPedidos.Exists(o => o.CTeEmitidoNoEmbarcador && !o.PedidoSemNFe))
                return;

            if (carga?.CargaSVM ?? false)
                return;

            RateioFormula serRateioFormula = new RateioFormula(unitOfWork);
            Servicos.Embarcador.Carga.ComponetesFrete serCargaComponetesFrete = new Servicos.Embarcador.Carga.ComponetesFrete(unitOfWork);
            Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);
            Servicos.Embarcador.Carga.ICMS serCargaICMS = new Servicos.Embarcador.Carga.ICMS(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
            Repositorio.Embarcador.ICMS.PedagioEstadoBaseCalculo repPedagioEstadoBaseCalculo = new Repositorio.Embarcador.ICMS.PedagioEstadoBaseCalculo(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPreviaDocumento repCargaPreviaDocumento = new Repositorio.Embarcador.Cargas.CargaPreviaDocumento(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscalComponente repXMLNotaFiscalComponenteFrete = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalComponente(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete repositorioConfiguracaoTabelaFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repositorioPontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> cargaValePedagios = repCargaValePedagio.BuscarPorCarga(carga.Codigo);

            if (configuracao.UtilizarComponentesCliente)
                serCargaPedido.RemoverComponentesCliente(carga, rateioFreteFilialEmissora, unitOfWork, tipoServicoMultisoftware);

            if (carga.TipoOperacao?.UtilizarValorFreteNotasFiscais ?? false)
            {
                decimal valorTotalFreteNotasFiscais = repPedidoXMLNotaFiscal.BuscarValorTotalFretePorCarga(carga.Codigo);

                if (valorTotalFreteNotasFiscais > 0m)
                    valorTotalFrete = valorTotalFreteNotasFiscais;
            }

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosLiberados = (from obj in cargaPedidos where !obj.PedidoSemNFe select obj).ToList();
            if (cargaPedidosLiberados.Count == 0)
                return;

            repCargaPedido.ZerarValorFreteSemNota(carga.Codigo);

            int qtdPedidosParaRaterio = cargaPedidosLiberados.Count;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moeda = carga.Moeda ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real;
            decimal cotacaoMoeda = carga.ValorCotacaoMoeda ?? 0m;

            List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFretes = repCargaComponentesFrete.BuscarPorCargaFilialEmissora(carga.Codigo, rateioFreteFilialEmissora);

            ZerarValoresDaCarga(carga, rateioFreteFilialEmissora, unitOfWork);

            Servicos.Embarcador.Carga.FreteCliente serFreteCliente = new FreteCliente(unitOfWork);

            decimal valorFixoSubContratacaoParcial = 0m;

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                valorFixoSubContratacaoParcial = serFreteCliente.ObterValorFixoSubContratacaoParcial(carga, cargaPedidosLiberados);

                if (valorFixoSubContratacaoParcial > 0m && cargaPedidosLiberados.All(o => o.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada))
                {
                    valorTotalFrete -= valorFixoSubContratacaoParcial;
                }
            }

            Dominio.Entidades.Embarcador.Cargas.CargaPedido ultimaCargaPedido = cargaPedidosLiberados.Last();
            Dominio.Entidades.Embarcador.Rateio.RateioFormula formulaRateio = cargaPedidosLiberados.First().FormulaRateio;

            decimal pesoLiquidoTotal = 0m, pesoTotal = 0m, valorTotalNF = 0m, metrosCubicosTotais = 0m, valorTotalPedido = 0m, valorTotalMoedaPedido = 0m;
            int totalPedidosNormais = 0, totalPedidosSubcontratacao = 0, volumeTotal = 0;

            decimal pesoTotalParaCalculoFatorCubagem = 0;

            if (formulaRateio?.ParametroRateioFormula == ParametroRateioFormula.PorPesoUtilizandoFatorCubagem)
                pesoTotalParaCalculoFatorCubagem = serRateioFormula.ObterPesoTotalCubadoFatorCubagem(repPedidoXMLNotaFiscal.BuscarPorCarga(carga.Codigo));

            if (valorFixoSubContratacaoParcial > 0m)
            {
                pesoTotal = cargaPedidosLiberados.Where(o => o.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal || o.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada).Sum(obj => obj.Peso);
                pesoLiquidoTotal = cargaPedidosLiberados.Where(o => o.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal || o.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada).Sum(obj => obj.Pedido.QtVolumes);
                valorTotalNF = repPedidoXMLNotaFiscal.BuscarTotalPorCargaPedidosNormais(carga.Codigo);
                metrosCubicosTotais = repPedidoXMLNotaFiscal.BuscarMetrosCubicosPorCargaPedidosNormais(carga.Codigo);

                totalPedidosNormais = cargaPedidosLiberados.Count(o => o.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal || o.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada);
                totalPedidosSubcontratacao = cargaPedidosLiberados.Count(o => o.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal && o.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada);
            }
            else
            {
                if (configuracao.NaoAtualizarPesoPedidoPelaNFe)
                    volumeTotal = cargaPedidos.Sum(obj => obj.Pedido.QtVolumes);
                else
                    volumeTotal = cargaPedidos.Sum(obj => obj.QtVolumes);

                pesoLiquidoTotal = cargaPedidosLiberados.Sum(obj => obj.PesoLiquido);
                pesoTotal = cargaPedidosLiberados.Sum(obj => obj.Peso);
                if (carga.ExigeNotaFiscalParaCalcularFrete)
                    valorTotalNF = repPedidoXMLNotaFiscal.BuscarTotalPorCarga(carga.Codigo);
                else
                    valorTotalNF = repCargaPedido.BuscarValorTotalPedidos(carga.Codigo);

                metrosCubicosTotais = repPedidoXMLNotaFiscal.BuscarMetrosCubicosPorCarga(carga.Codigo);

                totalPedidosNormais = cargaPedidosLiberados.Count;
            }

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && !rateioFreteFilialEmissora)
            {
                new Servicos.Embarcador.Carga.CargaPallets(unitOfWork, configuracao).RatearPaletesModeloVeicularCargaEntreOsPedidos(carga, cargaPedidos);
                Servicos.Embarcador.Carga.Carga.CalcularValoresDescarga(carga, cargaPedidosLiberados, configuracao, rateioFreteFilialEmissora, tipoServicoMultisoftware, unitOfWork);
            }

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> pedidosAgrupados = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            IList<Dominio.ObjetosDeValor.Embarcador.Pedido.XMLNotaFiscalComponente> componentesXMLCarga = repXMLNotaFiscalComponenteFrete.BuscarSumarizadoPorCargaPedido(carga.Codigo);

            if ((carga.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.PreCheckIn) || (carga.TipoOperacao?.ConfiguracaoCalculoFrete?.RatearValorFreteEntrePedidosAposReceberDocumentos ?? false))
                repCargaPedidoComponenteFrete.DeletarPorCargaNaoEmitidos(carga.Codigo, rateioFreteFilialEmissora);
            else
                repCargaPedidoComponenteFrete.DeletarPorCarga(carga.Codigo, rateioFreteFilialEmissora);

            List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidosValoresNota = repPedidoXMLNotaFiscal.BuscarTotalSumarizadoPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidosComponentesFreteCarga = repCargaPedidoComponenteFrete.BuscarPorCarga(carga.Codigo, rateioFreteFilialEmissora);
            Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFreteDescarga = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.DESCARGA);
            Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFretePedagio = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO);
            List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFretesDiretos = new List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();
            List<Dominio.Entidades.Embarcador.Pessoas.ClienteComponente> listaComponentesCliente = Servicos.Embarcador.Carga.ComponetesFrete.ObterComponentesCliente(carga, configuracao, cargaPedidos, unitOfWork);
            List<Dominio.Entidades.Embarcador.ICMS.PedagioEstadoBaseCalculo> pedagioEstadosBaseCalculo = repPedagioEstadoBaseCalculo.BuscarPorEstados((from obj in cargaPedidosLiberados select obj.Origem.Estado.Sigla).Distinct().ToList());
            List<Dominio.ObjetosDeValor.Embarcador.ICMS.TabelaAliquota> tabelaAliquotas = new List<Dominio.ObjetosDeValor.Embarcador.ICMS.TabelaAliquota>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = serCargaICMS.ObterProdutosCargaContidosEmRegras(carga, unitOfWork);
            List<Dominio.Entidades.Cliente> tomadoresFilialEmissora = Servicos.Embarcador.Carga.FilialEmissora.ObterTomadoresFilialEmissora((from obj in cargaPedidos where obj.CargaPedidoFilialEmissora && obj.CargaOrigem?.EmpresaFilialEmissora != null select obj.CargaOrigem.EmpresaFilialEmissora.CNPJ_SemFormato).Distinct().ToList(), unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigem = Servicos.Embarcador.Carga.Carga.ObterCargasOrigem(carga, unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete = repositorioConfiguracaoTabelaFrete.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork).BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = carga.TabelaFrete ?? null;

            bool abriuTransacao = false;
            if (!unitOfWork.IsActiveTransaction())
            {
                unitOfWork.Start();
                abriuTransacao = true;
            }

            bool possuiValorFreteNegociado = cargaPedidos.All(o => o.Pedido.ValorFreteNegociado > 0m) || cargaPedidos.All(o => o.Pedido.ValorFreteToneladaNegociado > 0m);

            if (!Servicos.Embarcador.Carga.CTeSimplificado.ValidarCTeSimplificado(cargaPedidos, unitOfWork, tipoServicoMultisoftware, configuracao))
                Servicos.Embarcador.Carga.CTeGlobalizado.ValidarCTeGlobalizadoPorDestinatario(cargaPedidos, unitOfWork, tipoServicoMultisoftware, configuracao);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutoCarga = repCargaPedidoProduto.BuscarPorCarga(carga.Codigo);

            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosPassagem = repositorioPontosPassagem.BuscarPorCarga(carga.Codigo);
            List<Dominio.ObjetosDeValor.Embarcador.Carga.RateioPonderacaoDistanciaPeso> rateiosPonderacaoPesoDistancia = serRateioFormula.CalcularRateioPonderacaoDistanciaPeso(cargaPedidos, pontosPassagem);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidosLiberados)
            {
                SetarObservacaoPedido(carga, cargaPedido, unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Carga.RateioPonderacaoDistanciaPeso rateioPonderacaoDistanciaPeso = rateiosPonderacaoPesoDistancia.FirstOrDefault(o => o.CargaPedido.Codigo == cargaPedido.Codigo);

                decimal metrosCubicosPedido = (from obj in cargaPedidosValoresNota where obj.Codigo == cargaPedido.Codigo select obj.MetrosCubicos).Sum();
                Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem = (from obj in cargasOrigem where obj.Codigo == cargaPedido.CargaOrigem.Codigo select obj).FirstOrDefault();
                decimal pesoPedido = cargaPedido.Peso;

                if ((cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor ||
                cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor) && cargaPedido.Recebedor != null && cargaPedido.Recebedor.Localidade != null)
                {
                    cargaPedido.Destino = cargaPedido.Recebedor.Localidade;
                    if (cargaPedido.Recebedor != null && cargaPedido.Pedido.EnderecoRecebedor != null && cargaPedido.Pedido.EnderecoRecebedor.Localidade != null)
                        cargaPedido.Destino = cargaPedido.Pedido.EnderecoRecebedor.Localidade;
                }
                else if (cargaPedido.Destino == null && cargaPedido.Pedido.Destino != null)
                {
                    if (cargaPedido.Pedido.UsarOutroEnderecoDestino && (cargaPedido.Pedido.EnderecoDestino?.Localidade != null))
                        cargaPedido.Destino = cargaPedido.Pedido.EnderecoDestino.Localidade;
                    else
                        cargaPedido.Destino = cargaPedido.Pedido.Destino;
                }

                if ((cargaPedido.Carga.TipoOperacao?.ConfiguracaoEmissaoDocumento?.UtilizarOutroEnderecoPedidoMesmoSePossuirRecebedor ?? false) && (cargaPedido.Pedido?.Recebedor?.CPF_CNPJ ?? 0) > 0)
                {
                    Repositorio.Embarcador.Pessoas.ClienteOutroEndereco repClienteOutroEndereco = new Repositorio.Embarcador.Pessoas.ClienteOutroEndereco(unitOfWork);
                    Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco endereco = repClienteOutroEndereco.BuscarPorPessoa(cargaPedido.Pedido.Recebedor.CPF_CNPJ).FirstOrDefault();

                    if (endereco != null)
                        cargaPedido.Destino = endereco.Localidade;
                }

                if (!rateioFreteFilialEmissora || (configuracao.CalcularFreteFilialEmissoraPorTabelaDeFrete || (carga.TipoOperacao?.CalculaFretePorTabelaFreteFilialEmissora ?? false)) || carga.TipoFreteEscolhido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador || carga.TipoFreteEscolhido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Operador)
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Pedido.XMLNotaFiscalComponente> componentesCargaPedido = (from obj in componentesXMLCarga where obj.CodigoCargaPedido == cargaPedido.Codigo select obj).ToList();

                    cargaPedido.PercentualPagamentoAgregado = carga.PercentualPagamentoAgregado;

                    if (valorFixoSubContratacaoParcial > 0m &&
                        cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal &&
                        cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada)
                    {
                        cargaPedido.ValorFrete = valorFixoSubContratacaoParcial / totalPedidosSubcontratacao;

                        CalcularImpostos(ref carga, cargaOrigem, cargaPedido, cargaPedido.ValorFrete, rateioFreteFilialEmissora, tipoServicoMultisoftware, unitOfWork, configuracao, cargaPedidosComponentesFreteCarga, pedagioEstadosBaseCalculo, cargaPedidoProdutos, tabelaAliquotas, tomadoresFilialEmissora, configuracaoGeralCarga);
                    }
                    else
                    {
                        decimal densidadeProdutos = (from obj in cargaPedidoProdutoCarga where obj.CargaPedido.Codigo == cargaPedido.Codigo select obj.Produto.MetroCubito).Sum(); //cargaPedido.Produtos?.Sum(o => o.Produto?.MetroCubito) ?? 0m;
                        decimal valorNFsPedido = 0m;
                        if (carga.ExigeNotaFiscalParaCalcularFrete)
                            valorNFsPedido = (from obj in cargaPedidosValoresNota where obj.Codigo == cargaPedido.Codigo select obj.ValorTotalNotaFiscal).Sum();
                        else
                            valorNFsPedido = cargaPedido.Pedido.ValorTotalNotasFiscais;


                        decimal valorNotasSemPallets = 0m;
                        bool possuiComponenteNaoIncideNotasPallet = cargaComponentesFretes.Where(obj => obj.ComponenteFrete.NaoDeveIncidirSobreNotasFiscaisPateles).Any();
                        if (possuiComponenteNaoIncideNotasPallet)
                            valorNotasSemPallets = cargaPedidosLiberados.SelectMany(obj => obj.NotasFiscais.Select(nf => nf.XMLNotaFiscal).Where(nf => nf.TipoNotaFiscalIntegrada != TipoNotaFiscalIntegrada.RemessaPallet)).Sum(obj => obj.Valor);

                        decimal pesoParaCalculoFatorCubagem = 0;

                        if (formulaRateio?.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.PorPesoUtilizandoFatorCubagem)
                            pesoParaCalculoFatorCubagem = serRateioFormula.ObterPesoCubadoFatorCubagem(cargaPedido.FormulaRateio?.ParametroRateioFormula, cargaPedido.TipoUsoFatorCubagemRateioFormula, cargaPedido.FatorCubagemRateioFormula ?? 0, configuracao.NaoAtualizarPesoPedidoPelaNFe ? cargaPedido.Pedido.QtVolumes : cargaPedido.QtVolumes, pesoPedido, repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo)?.Sum(x => x.XMLNotaFiscal.MetrosCubicos) ?? 0);

                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponentesFrete in cargaComponentesFretes)
                        {
                            //ratear igualmente entre os pedidos e não chamar a formula
                            int quantidadeDocumentosRateio = 0;
                            decimal valorRateado = 0m;
                            decimal valorMoedaRateado = 0m;
                            decimal valorComponente = 0m;
                            decimal valorRateioOriginal2 = 0;
                            bool naoIncidirSobreNotasPallet = cargaComponentesFrete.ComponenteFrete?.NaoDeveIncidirSobreNotasFiscaisPateles ?? false;
                            bool aplicouRateioSemValorPallets = false;

                            Dominio.ObjetosDeValor.Embarcador.Pedido.XMLNotaFiscalComponente componenteXMLNotaFiscal = componentesCargaPedido.Where(o => o.CodigoComponenteFrete == cargaComponentesFrete.ComponenteFrete.Codigo).FirstOrDefault();

                            if (componenteXMLNotaFiscal == null)
                            {
                                if (cargaComponentesFrete.PorQuantidadeDocumentos && cargaComponentesFrete.TipoCalculoQuantidadeDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete.PorDocumentoEmitido)
                                {
                                    quantidadeDocumentosRateio = repCargaPreviaDocumento.ObterQuantidadePorCargaPedidoEModeloDocumento(cargaPedido.Codigo, cargaComponentesFrete.ModeloDocumentoFiscal?.Codigo ?? 0);

                                    if (formulaRateio != null && formulaRateio.RatearPrimeiroIgualmenteEntrePedidos)
                                    {
                                        valorRateado = Math.Floor((cargaComponentesFrete.ValorComponente / qtdPedidosParaRaterio) * 100) / 100;
                                    }
                                    else if (moeda != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real)
                                    {
                                        valorMoedaRateado = Math.Floor((cargaComponentesFrete.ValorTotalMoeda ?? 0m) / cargaComponentesFrete.QuantidadeTotalDocumentos * quantidadeDocumentosRateio * 100) / 100;
                                        valorRateado = valorMoedaRateado * cotacaoMoeda;
                                    }
                                    else
                                        valorRateado = (cargaComponentesFrete.ValorComponente / cargaComponentesFrete.QuantidadeTotalDocumentos) * quantidadeDocumentosRateio;
                                }
                                else
                                {
                                    if (formulaRateio != null && formulaRateio.RatearPrimeiroIgualmenteEntrePedidos)
                                    {
                                        if (naoIncidirSobreNotasPallet)
                                        {
                                            valorRateado = valorNotasSemPallets * (cargaComponentesFrete.Percentual / 100);
                                            aplicouRateioSemValorPallets = true;
                                        }
                                        else
                                            valorRateado = Math.Floor((cargaComponentesFrete.ValorComponente / qtdPedidosParaRaterio) * 100) / 100;
                                    }
                                    else if (moeda != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real)
                                    {
                                        valorMoedaRateado = serRateioFormula.AplicarFormulaRateio(formulaRateio, cargaComponentesFrete.ValorTotalMoeda ?? 0m, totalPedidosNormais, 0, pesoTotal, pesoPedido, valorNFsPedido, valorTotalNF, cargaComponentesFrete.Percentual, cargaComponentesFrete.TipoValor, 0, 0, ref valorRateioOriginal2, 0m, 0m, 0m, metrosCubicosPedido, metrosCubicosTotais, densidadeProdutos, true, cargaPedido.PesoLiquido, pesoLiquidoTotal, configuracao.NaoAtualizarPesoPedidoPelaNFe ? cargaPedido.Pedido.QtVolumes : cargaPedido.QtVolumes, volumeTotal, rateioPonderacaoDistanciaPeso, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                                        valorRateado = valorMoedaRateado * cotacaoMoeda;
                                    }
                                    else
                                        valorRateado = serRateioFormula.AplicarFormulaRateio(formulaRateio, cargaComponentesFrete.ValorComponente, totalPedidosNormais, 0, pesoTotal, pesoPedido, valorNFsPedido, valorTotalNF, cargaComponentesFrete.Percentual, cargaComponentesFrete.TipoValor, 0, 0, ref valorRateioOriginal2, 0m, 0m, 0m, metrosCubicosPedido, metrosCubicosTotais, densidadeProdutos, true, cargaPedido.PesoLiquido, pesoLiquidoTotal, configuracao.NaoAtualizarPesoPedidoPelaNFe ? cargaPedido.Pedido.QtVolumes : cargaPedido.QtVolumes, volumeTotal, rateioPonderacaoDistanciaPeso, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                                }

                                valorComponente = Math.Round(valorRateado, 2, MidpointRounding.AwayFromZero);

                                if (cargaPedido.Equals(ultimaCargaPedido))
                                {
                                    if (moeda != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real)
                                    {
                                        decimal valorTotalMoeda = (from obj in cargaComponentesFretes where obj.TipoComponenteFrete == cargaComponentesFrete.TipoComponenteFrete && (cargaComponentesFrete.ComponenteFrete == null || obj.ComponenteFrete.Equals(cargaComponentesFrete.ComponenteFrete)) select obj.ValorTotalMoeda ?? 0m).Sum();
                                        decimal valorTotalMoedaCargaPedido = repCargaPedidoComponenteFrete.BuscarTotalMoedaCargaPorCompomente(carga.Codigo, cargaComponentesFrete.TipoComponenteFrete, cargaComponentesFrete.ComponenteFrete, rateioFreteFilialEmissora) + valorMoedaRateado;

                                        valorMoedaRateado += valorTotalMoeda - valorTotalMoedaCargaPedido;
                                        valorComponente = valorMoedaRateado * cotacaoMoeda;
                                    }
                                    else
                                    {
                                        decimal valorTotalComponente = (from obj in cargaComponentesFretes where obj.TipoComponenteFrete == cargaComponentesFrete.TipoComponenteFrete && (cargaComponentesFrete.ComponenteFrete == null || obj.ComponenteFrete.Equals(cargaComponentesFrete.ComponenteFrete)) select obj.ValorComponente).Sum();
                                        decimal valorTotalCargaPedidoComponente = repCargaPedidoComponenteFrete.BuscarTotalCargaPorCompomente(carga.Codigo, cargaComponentesFrete.TipoComponenteFrete, cargaComponentesFrete.ComponenteFrete, rateioFreteFilialEmissora) + valorComponente;

                                        if (!aplicouRateioSemValorPallets)
                                            valorComponente += valorTotalComponente - valorTotalCargaPedidoComponente;
                                    }
                                }
                            }
                            else
                            {
                                valorComponente = componenteXMLNotaFiscal.Valor;
                            }

                            serCargaComponetesFrete.AdicionarCargaPedidoComponente(cargaPedido, valorComponente, cargaComponentesFrete.Percentual, cargaComponentesFrete.TipoValor, cargaComponentesFrete.TipoComponenteFrete, cargaComponentesFrete.ComponenteFrete, cargaComponentesFrete.IncluirBaseCalculoICMS, cargaComponentesFrete.IncluirIntegralmenteContratoFreteTerceiro, cargaComponentesFrete.ModeloDocumentoFiscal, cargaComponentesFrete.RateioFormula, cargaComponentesFrete.OutraDescricaoCTe, cargaComponentesFrete.DescontarValorTotalAReceber, cargaComponentesFrete.AcrescentaValorTotalAReceber, rateioFreteFilialEmissora, unitOfWork, cargaComponentesFrete.PorQuantidadeDocumentos, cargaComponentesFrete.TipoCalculoQuantidadeDocumentos, quantidadeDocumentosRateio, cargaComponentesFrete.ModeloDocumentoFiscalRateio, cargaComponentesFrete.NaoSomarValorTotalAReceber, ref cargaPedidosComponentesFreteCarga, cargaComponentesFrete.NaoSomarValorTotalPrestacao, moeda, cotacaoMoeda, valorMoedaRateado, cargaComponentesFrete.UtilizarFormulaRateioCarga ?? false);
                        }

                        bool removeuComponentesAnteriormente = cargaValePedagios.Exists(x => x.ValidaCompraRemoveuComponentes);//caso ja tenha removido componentes de vale pedagio, nao será adicionado novamente.

                        serCargaPedido.ValidarValorDescargaPorCargaPedido(carga, cargaPedido, rateioFreteFilialEmissora, unitOfWork, tipoServicoMultisoftware, componenteFreteDescarga, cargaPedidosComponentesFreteCarga, cargaComponentesFretesDiretos);
                        serCargaPedido.ValidarValorPedagioPorCargaPedido(cargaPedido, rateioFreteFilialEmissora, unitOfWork, tipoServicoMultisoftware, componenteFretePedagio, cargaPedidosComponentesFreteCarga, cargaComponentesFretesDiretos, removeuComponentesAnteriormente, tabelaFrete);
                        if (configuracao.UtilizarComponentesCliente)
                            serCargaPedido.GerarComponentesClientePorCargaPedido(cargaPedido, rateioFreteFilialEmissora, unitOfWork, tipoServicoMultisoftware, listaComponentesCliente, cargaPedidosComponentesFreteCarga, cargaComponentesFretesDiretos);

                        //ratear igualmente entre os pedidos e não chamar a formula
                        decimal valorFretePedidoRateado = 0m;
                        decimal valorMoedaPedidoRateado = 0m;
                        decimal valorRateioOriginal = 0;

                        if (carga.TipoOperacao?.UtilizarValorFreteNotasFiscais ?? false)
                            valorFretePedidoRateado = repPedidoXMLNotaFiscal.BuscarValorTotalFretePorCargaPedido(cargaPedido.Codigo);

                        if (cargaPedido.FormulaRateio?.ExigirConferenciaManual ?? false)
                            valorFretePedidoRateado = repPedidoXMLNotaFiscal.BuscarValorTotalFreteAjusteManualPorCargaPedido(cargaPedido.Codigo);

                        if (valorFretePedidoRateado <= 0m)
                        {
                            if ((carga.TipoOperacao?.FixarValorFreteNegociadoRateioPedidos ?? false) && possuiValorFreteNegociado)
                            {
                                if (configuracao.SolicitarValorFretePorTonelada)
                                {
                                    valorFretePedidoRateado = cargaPedido.Pedido.ValorFreteToneladaNegociado;
                                    if (valorFretePedidoRateado > 0)
                                    {
                                        if (pesoTotal > 0)
                                            valorFretePedidoRateado = (valorFretePedidoRateado * (pesoTotal / 1000));
                                    }
                                }
                                else
                                    valorFretePedidoRateado = cargaPedido.Pedido.ValorFreteNegociado;
                            }
                            else if (formulaRateio != null && formulaRateio.RatearPrimeiroIgualmenteEntrePedidos)
                            {
                                valorFretePedidoRateado = Math.Floor((valorTotalFrete / qtdPedidosParaRaterio) * 100) / 100;
                            }
                            else if (moeda != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real)
                            {
                                valorMoedaPedidoRateado = serRateioFormula.AplicarFormulaRateio(formulaRateio, carga.ValorTotalMoeda ?? 0m, totalPedidosNormais, 0, pesoTotal, pesoPedido, valorNFsPedido, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, metrosCubicosPedido, metrosCubicosTotais, densidadeProdutos, false, cargaPedido.PesoLiquido, pesoLiquidoTotal, configuracao.NaoAtualizarPesoPedidoPelaNFe ? cargaPedido.Pedido.QtVolumes : cargaPedido.QtVolumes, volumeTotal, rateioPonderacaoDistanciaPeso, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);

                                if (cargaPedido.Equals(ultimaCargaPedido))
                                    valorMoedaPedidoRateado += (carga.ValorTotalMoeda ?? 0m) - (valorTotalMoedaPedido + valorMoedaPedidoRateado);

                                valorFretePedidoRateado = valorMoedaPedidoRateado * cotacaoMoeda;
                            }
                            else
                            {
                                valorFretePedidoRateado = serRateioFormula.AplicarFormulaRateio(formulaRateio, valorTotalFrete, totalPedidosNormais, 0, pesoTotal, pesoPedido, valorNFsPedido, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, metrosCubicosPedido, metrosCubicosTotais, densidadeProdutos, false, cargaPedido.PesoLiquido, pesoLiquidoTotal, configuracao.NaoAtualizarPesoPedidoPelaNFe ? cargaPedido.Pedido.QtVolumes : cargaPedido.QtVolumes, volumeTotal, rateioPonderacaoDistanciaPeso, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                            }
                        }

                        decimal valorFretePedido = Math.Round(valorFretePedidoRateado, 2, MidpointRounding.AwayFromZero);

                        valorTotalPedido += valorFretePedido;
                        valorTotalMoedaPedido += valorMoedaPedidoRateado;

                        if (cargaPedido.Equals(ultimaCargaPedido))
                            valorFretePedido += valorTotalFrete - valorTotalPedido;

                        cargaPedido.ValorTotalMoeda = valorMoedaPedidoRateado;
                        cargaPedido.Moeda = moeda;
                        cargaPedido.ValorCotacaoMoeda = cotacaoMoeda;

                        if (!rateioFreteFilialEmissora)
                            InformarDadosContabeisCargaPedido(cargaPedido, cargaOrigem, configuracao, tipoServicoMultisoftware, unitOfWork);

                        if ((carga.TabelaFrete?.UtilizarValorMinimoParaRateio ?? false) && valorFretePedido < carga.TabelaFrete.ValorMinimoParaRateio)
                            valorFretePedido = carga.TabelaFrete.ValorMinimoParaRateio;

                        if (cargaPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoAgrupado
                            || cargaPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupada
                            || cargaPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupadaEntrePedidos
                            || cargaPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.NaoInformado
                            || cargaPedido.IndicadorCTeGlobalizadoDestinatario)
                        {
                            if (!rateioFreteFilialEmissora)
                                cargaPedido.ValorFrete = Math.Round(valorFretePedido, 2, MidpointRounding.AwayFromZero);
                            else
                                cargaPedido.ValorFreteFilialEmissora = Math.Round(valorFretePedido, 2, MidpointRounding.AwayFromZero);

                            pedidosAgrupados.Add(cargaPedido);
                        }
                        else
                        {
                            CalcularImpostos(ref carga, cargaOrigem, cargaPedido, valorFretePedido, rateioFreteFilialEmissora, tipoServicoMultisoftware, unitOfWork, configuracao, cargaPedidosComponentesFreteCarga, pedagioEstadosBaseCalculo, cargaPedidoProdutos, tabelaAliquotas, tomadoresFilialEmissora, configuracaoGeralCarga);
                        }
                    }
                    cargaPedido.ValorBaseFrete = cargaPedido.ValorFreteAPagar;
                    if (carga.MaiorValorBaseFreteDosPedidos < cargaPedido.ValorBaseFrete)
                        carga.MaiorValorBaseFreteDosPedidos = cargaPedido.ValorBaseFrete;
                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Carga.CargaValoresAcrescentar cargaValoresAcrescentar = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaValoresAcrescentar()
                    {
                        ValorFrete = cargaPedido.ValorFreteFilialEmissora,
                        ValorFreteAPagar = cargaPedido.ValorFreteAPagarFilialEmissora,
                        ValorICMS = cargaPedido.ValorICMSFilialEmissora,
                        ValorIBSEstadual = cargaPedido.ValorIBSEstadualFilialEmissora,
                        ValorIBSMunicipal = cargaPedido.ValorIBSMunicipalFilialEmissora,
                        ValorCBS = cargaPedido.ValorCBSFilialEmissora,
                    };

                    AcrescentarValoresFilialEmissoraDaCarga(carga, cargaValoresAcrescentar);
                }


            }

            if (abriuTransacao)
                unitOfWork.CommitChanges();

            serCargaComponetesFrete.AdicionarComponentesCargaAgrupada(carga, rateioFreteFilialEmissora, cargaPedidosComponentesFreteCarga, unitOfWork);

            AcrescentarValoresDaCargaAgrupada(carga, rateioFreteFilialEmissora, cargaPedidos, unitOfWork);

            if (pedidosAgrupados.Count > 0)
            {
                abriuTransacao = false;
                if (!unitOfWork.IsActiveTransaction())
                {
                    unitOfWork.Start();
                    abriuTransacao = true;
                }
                CalcularImpostosAgrupados(ref carga, cargasOrigem, pedidosAgrupados, rateioFreteFilialEmissora, tipoServicoMultisoftware, unitOfWork, configuracao, cargaPedidosComponentesFreteCarga, pedagioEstadosBaseCalculo, cargaPedidoProdutos, tabelaAliquotas, tomadoresFilialEmissora, cargaPedidosValoresNota, configuracaoTabelaFrete, configuracaoGeralCarga);

                if (abriuTransacao)
                    unitOfWork.CommitChanges();
            }

            if (!rateioFreteFilialEmissora)
            {
                carga.ValorPis = Math.Round(carga.ValorPis, 2, MidpointRounding.AwayFromZero);
                carga.ValorCofins = Math.Round(carga.ValorCofins, 2, MidpointRounding.AwayFromZero);
                carga.ValorICMS = Math.Round(carga.ValorICMS, 2, MidpointRounding.AwayFromZero);
                carga.ValorISS = Math.Round(carga.ValorISS, 2, MidpointRounding.AwayFromZero);
                carga.ValorRetencaoISS = Math.Round(carga.ValorRetencaoISS, 2, MidpointRounding.AwayFromZero);
                carga.ValorFreteAPagar = Math.Round(carga.ValorFreteAPagar, 2, MidpointRounding.AwayFromZero);
                carga.ValorFrete = Math.Round(carga.ValorFrete, 2, MidpointRounding.AwayFromZero);
                cargaComponentesFretes = repCargaComponentesFrete.BuscarPorCargaFilialEmissora(carga.Codigo, rateioFreteFilialEmissora);
                decimal valorFreteLiquido = (from obj in cargaComponentesFretes where obj.SomarComponenteFreteLiquido select obj.ValorComponente).Sum();
                carga.ValorFreteLiquido = Math.Round(carga.ValorFrete + valorFreteLiquido, 2, MidpointRounding.AwayFromZero);

                carga.ValorIBSEstadual = Math.Round(carga.ValorIBSEstadual, 3, MidpointRounding.AwayFromZero);
                carga.ValorIBSMunicipal = Math.Round(carga.ValorIBSMunicipal, 3, MidpointRounding.AwayFromZero);
                carga.ValorCBS = Math.Round(carga.ValorCBS, 3, MidpointRounding.AwayFromZero);
            }
            else
            {
                carga.ValorICMSFilialEmissora = Math.Round(carga.ValorICMSFilialEmissora, 2, MidpointRounding.AwayFromZero);
                carga.ValorFreteAPagarFilialEmissora = Math.Round(carga.ValorFreteAPagarFilialEmissora, 2, MidpointRounding.AwayFromZero);
                carga.ValorFreteFilialEmissora = Math.Round(carga.ValorFreteFilialEmissora, 2, MidpointRounding.AwayFromZero);

                carga.ValorIBSEstadualFilialEmissora = Math.Round(carga.ValorIBSEstadualFilialEmissora, 3, MidpointRounding.AwayFromZero);
                carga.ValorIBSMunicipalFilialEmissora = Math.Round(carga.ValorIBSMunicipalFilialEmissora, 3, MidpointRounding.AwayFromZero);
                carga.ValorCBSFilialEmissora = Math.Round(carga.ValorCBSFilialEmissora, 3, MidpointRounding.AwayFromZero);
            }

            if (carga.ExigeNotaFiscalParaCalcularFrete)
            {
                RateioNotaFiscal serRateioNotaFiscal = new RateioNotaFiscal(unitOfWork);
                serRateioNotaFiscal.RatearFreteCargaPedidoEntreNotas(carga, cargaPedidos, cargaPedidosComponentesFreteCarga, rateioFreteFilialEmissora, tipoServicoMultisoftware, unitOfWork, configuracao);
            }
        }

        public void RatearFreteEntrePedidos(Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga, decimal valorTotalFrete, bool rateioFreteFilialEmissora, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            RateioFormula serRateioFormula = new RateioFormula(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoComponenteFrete repPedidoComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoComponenteFrete(unitOfWork);

            Repositorio.Embarcador.PreCargas.PreCargaCompomenteFrete repCargaComponentesFrete = new Repositorio.Embarcador.PreCargas.PreCargaCompomenteFrete(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = preCarga.Pedidos.ToList();
            List<Dominio.Entidades.Embarcador.PreCargas.PreCargaCompomenteFrete> preCargaComponentesFretes = repCargaComponentesFrete.BuscarPorPreCarga(preCarga.Codigo);

            preCarga.ValorFrete = 0;

            Dominio.Entidades.Embarcador.Pedidos.Pedido ultimoPedido = pedidos.Last();
            decimal valorTotalPedido = 0;
            decimal valorRateioOriginal = 0;

            Dominio.Entidades.Embarcador.Rateio.RateioFormula formulaRateio = null;

            int volumeTotal = pedidos.Sum(obj => obj.QtVolumes);
            decimal pesoLiquidoTotal = pedidos.Sum(obj => obj.PesoLiquidoTotal);
            decimal pesoTotal = pedidos.Sum(obj => obj.PesoTotal);
            decimal valorTotalNF = pedidos.Sum(obj => obj.ValorTotalNotasFiscais);

            int totalPedidosNormais = pedidos.Count;

            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
            {
                pedido.IncluirBaseCalculoICMS = preCarga.TabelaFrete?.IncluirICMSValorFrete ?? false;
                pedido.PercentualInclusaoBC = preCarga.TabelaFrete?.PercentualICMSIncluir ?? 0;
                repPedidoComponenteFrete.DeletarPorPedido(pedido.Codigo, rateioFreteFilialEmissora);

                decimal valorNFsPedido = pedido.ValorTotalNotasFiscais;

                foreach (Dominio.Entidades.Embarcador.PreCargas.PreCargaCompomenteFrete preCargaComponentesFrete in preCargaComponentesFretes)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete pedidoComponenteFrete = new Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete();
                    pedidoComponenteFrete.ComponenteFrete = preCargaComponentesFrete.ComponenteFrete;
                    pedidoComponenteFrete.IncluirBaseCalculoICMS = preCargaComponentesFrete.IncluirBaseCalculoICMS;
                    pedidoComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro = false;
                    if (preCargaComponentesFrete.ComponenteFrete.ImprimirOutraDescricaoCTe)
                        pedidoComponenteFrete.OutraDescricaoCTe = preCargaComponentesFrete.ComponenteFrete.DescricaoCTe;
                    pedidoComponenteFrete.Pedido = pedido;
                    pedidoComponenteFrete.TipoComponenteFrete = preCargaComponentesFrete.TipoComponenteFrete;
                    pedidoComponenteFrete.ValorComponente += preCargaComponentesFrete.ValorComponente;
                    repPedidoComponenteFrete.Inserir(pedidoComponenteFrete);
                }

                decimal valorFretePedidoRateado = serRateioFormula.AplicarFormulaRateio(formulaRateio, valorTotalFrete, totalPedidosNormais, 0, pesoTotal, pedido.PesoTotal, valorNFsPedido, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, 0m, 0m, 0m, false, pedido.PesoLiquidoTotal, pesoLiquidoTotal, pedido.QtVolumes, volumeTotal);
                decimal valorFretePedido = Math.Round(valorFretePedidoRateado, 2, MidpointRounding.AwayFromZero);

                valorTotalPedido += valorFretePedido;
                if (pedido.Equals(ultimoPedido))
                    valorFretePedido += valorTotalFrete - valorTotalPedido;

                CalcularImpostos(ref preCarga, pedido, valorFretePedido, rateioFreteFilialEmissora, tipoServicoMultisoftware, unitOfWork, configuracao);

            }

            preCarga.ValorFrete = Math.Round(preCarga.ValorFrete, 2, MidpointRounding.AwayFromZero);
        }

        public void CalcularImpostos(ref Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, decimal valorFretePedido, bool calculoImpostosFilialEmissora, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.Pedidos.PedidoComponenteFrete repPedidoComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            Servicos.Embarcador.Carga.ICMS servicoIcms = new Servicos.Embarcador.Carga.ICMS(unitOfWork);

            decimal valorBaseCalculo = 0;

            pedido.ValorFreteNegociado = Math.Round(valorFretePedido, 2, MidpointRounding.AwayFromZero);
            valorBaseCalculo = pedido.ValorFreteNegociado;

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete> pedidoComponentesFretes = repPedidoComponenteFrete.BuscarPorCargaPedidoSemComponenteFreteValor(pedido.Codigo, calculoImpostosFilialEmissora);
            Dominio.Entidades.Empresa empresa = preCarga.Empresa;

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete cargaPedidoComponenteFrete in pedidoComponentesFretes)
                valorBaseCalculo += servicoIcms.ObterValorIcmsComponenteFrete(cargaPedidoComponenteFrete, empresa, pedido.Origem.Estado.Sigla, unitOfWork, tipoServicoMultisoftware);

            Dominio.Entidades.Cliente tomador = pedido.ObterTomador();

            if (tomador == null)
                throw new ServicoException("Tomador não encontrado");

            pedido.ValorFreteAReceber = Math.Round((pedido.ValorFreteNegociado + pedidoComponentesFretes.Sum(obj => obj.ValorComponente)), 2, MidpointRounding.AwayFromZero);

            bool incluirICMS = pedido.IncluirBaseCalculoICMS;
            decimal percentualIncluir = pedido.PercentualInclusaoBC;

            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = null;

            Dominio.Entidades.Localidade origem = pedido.Origem;
            Dominio.Entidades.Localidade destino = pedido.Destino;

            if (configuracao.UtilizarLocalidadePrestacaoPedido)
            {
                if (pedido.LocalidadeInicioPrestacao != null)
                    origem = pedido.LocalidadeInicioPrestacao;

                if (pedido.LocalidadeTerminoPrestacao != null)
                    destino = pedido.LocalidadeTerminoPrestacao;
            }

            regraICMS = servicoIcms.BuscarRegraICMSPreCarga(preCarga, pedido, empresa, pedido.Remetente, pedido.Destinatario, tomador, origem, destino, ref incluirICMS, ref percentualIncluir, valorBaseCalculo, null, unitOfWork, tipoServicoMultisoftware, configuracao);

            pedido.IncluirBaseCalculoICMS = incluirICMS;
            pedido.PercentualInclusaoBC = percentualIncluir;
            pedido.BaseCalculoICMS = regraICMS.ValorBaseCalculoICMS;
            pedido.PercentualAliquota = regraICMS.Aliquota;
            pedido.AliquotaPis = regraICMS.AliquotaPis;
            pedido.AliquotaCofins = regraICMS.AliquotaCofins;
            pedido.PercentualAliquotaInternaDifal = regraICMS.AliquotaInternaDifal;
            pedido.ValorICMS = Math.Round(regraICMS.ValorICMS, 2, MidpointRounding.AwayFromZero);
            pedido.ValorPis = Math.Round(regraICMS.ValorPis, 2, MidpointRounding.AwayFromZero);
            pedido.ValorCofins = Math.Round(regraICMS.ValorCofins, 2, MidpointRounding.AwayFromZero);
            pedido.ValorICMSIncluso = Math.Round(regraICMS.ValorICMSIncluso, 2, MidpointRounding.AwayFromZero);

            if (pedido.IncluirBaseCalculoICMS && regraICMS.CST != "60")
            {
                if (pedido.ValorICMSIncluso > 0)
                    pedido.ValorFreteAReceber += pedido.ValorICMSIncluso + pedido.ValorPis + pedido.ValorCofins;
                else
                    pedido.ValorFreteAReceber += pedido.ValorICMS + pedido.ValorPis + pedido.ValorCofins;
            }
            repPedido.Atualizar(pedido);

            preCarga.ValorFrete += pedido.ValorFreteCotado;
        }

        public void CalcularValoresImpostoAgrupado(ref Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigem, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, decimal valorTotal, bool calculoImpostosFilialEmissora, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesFretesCarga, List<Dominio.Entidades.Embarcador.ICMS.PedagioEstadoBaseCalculo> pedagioEstadosBaseCalculo, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos, List<Dominio.ObjetosDeValor.Embarcador.ICMS.TabelaAliquota> tabelaAliquotas, List<Dominio.Entidades.Cliente> tomadoresFilialEmissora, List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidosValoresNota, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga)
        {
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            Servicos.Embarcador.Carga.ICMS servicoIcms = new Servicos.Embarcador.Carga.ICMS(unitOfWork);
            Servicos.Embarcador.Carga.ISS serCargaISS = new Servicos.Embarcador.Carga.ISS(unitOfWork);
            Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);
            Servicos.Embarcador.Imposto.ImpostoIBSCBS servicoImpostoIBSCBS = new Servicos.Embarcador.Imposto.ImpostoIBSCBS(unitOfWork);

            Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = carga.TabelaFrete ?? null;

            decimal valorBaseCalculo = valorTotal;
            decimal valorBaseCalculoIBSCBS = valorTotal;

            bool freteFilialEmissoraOperador = carga.TipoFreteEscolhido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Operador && (configuracaoGeralCarga.PermiteInformarFreteOperadorFilialEmissora ?? false);

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoPadrao = cargaPedidos.FirstOrDefault();
            if (cargaPedidoPadrao.IndicadorCTeGlobalizadoDestinatario && cargaPedidoPadrao.Origem?.Codigo == cargaPedidoPadrao.Destino?.Codigo && cargaPedidos.Exists(obj => obj.Origem?.Codigo != obj.Destino?.Codigo))
                cargaPedidoPadrao = cargaPedidos.Find(obj => obj.Origem?.Codigo != obj.Destino?.Codigo);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga tipoContratacao = cargaPedidoPadrao.TipoContratacaoCarga;
            List<int> codigosCargaPedido = (from obj in cargaPedidos select obj.Codigo).ToList();

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesBaseCalculoFretes = (from obj in cargaPedidoComponentesFretesCarga
                                                                                                                             where codigosCargaPedido.Contains(obj.CargaPedido.Codigo)
                                                                                                                             && (obj.ComponenteFrete == null || !obj.ComponenteFrete.ComponentePertenceComposicaoFreteValor)
                                                                                                                             && obj.ComponenteFilialEmissora == calculoImpostosFilialEmissora
                                                                                                                             && !(obj.ComponenteFrete?.ComponenteApenasInformativoDocumentoEmitido ?? false)
                                                                                                                             && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS
                                                                                                                             && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS
                                                                                                                             && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS
                                                                                                                             && ((cargaPedidoPadrao.ModeloDocumentoFiscal == null && (obj.ModeloDocumentoFiscal == null || (obj.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)))
                                                                                                                             || cargaPedidoPadrao.ModeloDocumentoFiscal != null && (obj.ModeloDocumentoFiscal == null || obj.ModeloDocumentoFiscal.Codigo == cargaPedidoPadrao.ModeloDocumentoFiscal.Codigo))
                                                                                                                             select obj).ToList();

            Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem = (from obj in cargasOrigem where obj.Codigo == cargaPedidoPadrao.CargaOrigem.Codigo select obj).FirstOrDefault();

            Dominio.Entidades.Empresa empresa = calculoImpostosFilialEmissora ? cargaOrigem.EmpresaFilialEmissora : cargaOrigem.Empresa;

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponenteFrete in cargaPedidoComponentesBaseCalculoFretes)
            {
                valorBaseCalculo += servicoIcms.ObterValorIcmsComponenteFrete(cargaPedidoComponenteFrete, empresa, cargaPedidoPadrao.Origem.Estado.Sigla, pedagioEstadosBaseCalculo, unitOfWork, tipoServicoMultisoftware);
                valorBaseCalculoIBSCBS += cargaPedidoComponenteFrete.ValorComponente;
            }

            valorBaseCalculo = Math.Round(valorBaseCalculo, 2, MidpointRounding.AwayFromZero);
            valorBaseCalculoIBSCBS = Math.Round(valorBaseCalculoIBSCBS, 2, MidpointRounding.AwayFromZero);

            if (ModeloDocumentoFiscalCTePadrao == null)
                ModeloDocumentoFiscalCTePadrao = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.CTe);

            Dominio.Entidades.Cliente tomador = cargaPedidoPadrao.ObterTomador();

            if (tomador == null)
                throw new ServicoException("Tomador não encontrado");

            bool possuiCTe = false;
            bool possuiNFS = false;
            bool possuiNFSManual = false;
            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoIntramunicipal = null;

            bool modeloProprio = false;
            if (cargaPedidoPadrao.ModeloDocumentoEmpresaPropria)
                modeloProprio = true;

            serCargaPedido.VerificarQuaisDocumentosDeveEmitir(carga, cargaPedidoPadrao, cargaPedidoPadrao.Origem, cargaPedidoPadrao.Destino, tipoServicoMultisoftware, unitOfWork, out possuiCTe, out possuiNFS, out possuiNFSManual, out modeloDocumentoIntramunicipal, configuracao, out bool sempreDisponibilizarDocumentoNFSManual);

            if (modeloProprio || cargaPedidoPadrao.ModeloDocumentoEmpresaPropria)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                {
                    cargaPedido.ModeloDocumentoEmpresaPropria = cargaPedidoPadrao.ModeloDocumentoEmpresaPropria;
                    cargaPedido.ModeloDocumentoFiscal = cargaPedidoPadrao.ModeloDocumentoFiscal;
                    repCargaPedido.Atualizar(cargaPedido);
                }
            }

            int i = 1;
            int contadorISS = 1;
            decimal ValorRateioTotalICMS = 0;
            decimal ValorRateioTotalPis = 0;
            decimal ValorRateioTotalCofins = 0;
            decimal ValorRateioTotalICMSIncluso = 0;
            decimal ValorRateioTotalBaseCalculo = 0;
            decimal ValorRateioTotalISS = 0;
            decimal ValorRateioTotalRetencaoISS = 0;
            decimal ValorRateioTotalBaseCalculoISS = 0;
            decimal ValorRateioTotalIR = 0;
            decimal ValorRateioTotalRetencaoIR = 0;
            decimal ValorRateioTotalBaseCalculoIR = 0;
            decimal valorRateioTotalIBSEstadual = 0;
            decimal valorRateioTotalBaseCalculoIBSCBS = 0;
            decimal valorRateioTotalIBSMunicipal = 0;
            decimal valorBaseCalculoIBSCBSTotal = 0;
            decimal valorRateioTotalCBS = 0;

            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMSTotal = null;
            Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBSTotal = null;

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                cargaPedido.DisponibilizarDocumentoNFSManual = sempreDisponibilizarDocumentoNFSManual;

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesFretes = (from obj in cargaPedidoComponentesFretesCarga
                                                                                                                      where cargaPedido.Codigo == obj.CargaPedido.Codigo
                                                                                                                      && (obj.ComponenteFrete == null || !obj.ComponenteFrete.ComponentePertenceComposicaoFreteValor)
                                                                                                                      && !(obj.ComponenteFrete?.ComponenteApenasInformativoDocumentoEmitido ?? false)
                                                                                                                      && obj.ComponenteFilialEmissora == calculoImpostosFilialEmissora
                                                                                                                      && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS
                                                                                                                      && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS
                                                                                                                      && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS
                                                                                                                      && ((cargaPedidoPadrao.ModeloDocumentoFiscal == null && (obj.ModeloDocumentoFiscal == null || (obj.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)))
                                                                                                                      || cargaPedidoPadrao.ModeloDocumentoFiscal != null && (obj.ModeloDocumentoFiscal == null || obj.ModeloDocumentoFiscal.Codigo == cargaPedidoPadrao.ModeloDocumentoFiscal.Codigo))
                                                                                                                      select obj).ToList();

                int codigoComponenteDestacado = tabelaFrete?.ComponenteFreteDestacar?.Codigo ?? 0;
                decimal valorTotalMoedaComponente = Math.Round((cargaPedidoComponentesFretes.Sum(obj => (obj.ValorTotalMoeda ?? 0m))), 2, MidpointRounding.AwayFromZero);
                decimal valorTotalComponente = Math.Round((cargaPedidoComponentesFretes.Sum(obj => obj.ValorComponente)), 2, MidpointRounding.AwayFromZero);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete componente in cargaPedidoComponentesFretes)
                {

                    if (codigoComponenteDestacado == componente.ComponenteFrete?.Codigo)
                    {
                        if (tabelaFrete?.NaoSomarValorTotalAReceber ?? false)
                            valorTotalComponente -= componente.ValorComponente;

                        if ((tabelaFrete?.DescontarDoValorAReceberValorComponente ?? false) && !(tabelaFrete?.NaoSomarValorTotalPrestacao ?? false))
                            valorTotalComponente -= componente.ValorComponente;

                        continue;
                    }

                    if (componente.NaoSomarValorTotalAReceber && componente.ValorComponente > 0)
                        valorTotalComponente -= componente.ValorComponente;

                    if (componente.DescontarValorTotalAReceber && !componente.NaoSomarValorTotalPrestacao && componente.ValorComponente > 0)
                        valorTotalComponente -= componente.ValorComponente;
                }

                if (!calculoImpostosFilialEmissora)
                {
                    cargaPedido.ValorFreteAPagar = Math.Round((cargaPedido.ValorFrete + valorTotalComponente), 2, MidpointRounding.AwayFromZero);
                    cargaPedido.ValorTotalMoedaPagar = Math.Round(((cargaPedido.ValorTotalMoeda ?? 0m) + valorTotalMoedaComponente), 2, MidpointRounding.AwayFromZero);
                }
                else
                    cargaPedido.ValorFreteAPagarFilialEmissora = Math.Round((cargaPedido.ValorFreteFilialEmissora + valorTotalComponente), 2, MidpointRounding.AwayFromZero);

                decimal valorbaseRateado = Math.Round(cargaPedidoComponentesFretes.Where(obj => obj.IncluirBaseCalculoICMS).Sum(obj => obj.ValorComponente), 2, MidpointRounding.AwayFromZero);
                decimal valorbaseRateadoIBSCBS = Math.Round(cargaPedidoComponentesFretes.Sum(obj => obj.ValorComponente), 2, MidpointRounding.AwayFromZero);

                if (tabelaFrete?.NaoAdicionarOValorDoComponenteABaseDeCalculoDoICMS ?? false)
                {
                    decimal valorTotalComponenteDescontar = cargaPedidoComponentesFretes
                        .Where(x => x.IncluirBaseCalculoICMS && x.ComponenteFrete.Codigo == tabelaFrete.ComponenteFreteDestacar.Codigo)
                        .Sum(obj => obj.ValorComponente);

                    valorbaseRateado -= valorTotalComponenteDescontar;
                    valorBaseCalculo -= valorTotalComponenteDescontar;
                }

                if (!calculoImpostosFilialEmissora)
                {
                    valorbaseRateado += cargaPedido.ValorFrete;
                    valorbaseRateadoIBSCBS += cargaPedido.ValorFrete;
                }
                else
                {
                    valorbaseRateado += cargaPedido.ValorFreteFilialEmissora;
                    valorbaseRateadoIBSCBS += cargaPedido.ValorFreteFilialEmissora;
                }

                if (possuiCTe)//calcula ICMS
                {
                    bool incluirICMS = !calculoImpostosFilialEmissora ? cargaPedidoPadrao.IncluirICMSBaseCalculo : cargaPedidoPadrao.IncluirICMSBaseCalculoFilialEmissora;
                    decimal percentualIncluir = !calculoImpostosFilialEmissora ? cargaPedidoPadrao.PercentualIncluirBaseCalculo : cargaPedidoPadrao.PercentualIncluirBaseCalculoFilialEmissora;

                    if (cargaPedidoPadrao.ModeloDocumentoFiscal == null || cargaPedidoPadrao.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                        cargaPedidoPadrao.ModeloDocumentoFiscal = ModeloDocumentoFiscalCTePadrao;

                    Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = null;
                    Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = null;

                    Dominio.Entidades.Cliente remetente = cargaPedidoPadrao.Pedido.Remetente;
                    Dominio.Entidades.Cliente destinatario = cargaPedidoPadrao.Pedido.Destinatario;

                    Dominio.Entidades.Localidade origem = cargaPedidoPadrao.Origem;
                    Dominio.Entidades.Localidade destino = cargaPedidoPadrao.ObterDestinoParaEmissao();

                    if (calculoImpostosFilialEmissora && cargaPedidoPadrao.CargaPedidoFilialEmissora && (cargaPedido.CargaPedidoProximoTrecho != null || cargaPedido.ProximoTrechoComplementaFilialEmissora))
                        destino = destinatario.Localidade;

                    if (carga.TipoOperacao?.ConfiguracaoEmissaoDocumento?.EmitirDocumentoSempreOrigemDestinoPedido ?? false)
                    {
                        origem = remetente.Localidade;
                        destino = destinatario.Localidade;
                    }

                    if (configuracao.UtilizarLocalidadePrestacaoPedido)
                    {
                        if (cargaPedido.Pedido.LocalidadeInicioPrestacao != null)
                            origem = cargaPedido.Pedido.LocalidadeInicioPrestacao;

                        if (cargaPedido.Pedido.LocalidadeTerminoPrestacao != null)
                            destino = cargaPedido.Pedido.LocalidadeTerminoPrestacao;
                    }

                    if ((!cargaPedidoPadrao.ImpostoInformadoPeloEmbarcador || calculoImpostosFilialEmissora) && !freteFilialEmissoraOperador)
                    {
                        if (!calculoImpostosFilialEmissora || !cargaPedidoPadrao.EmitirComplementarFilialEmissora)
                        {
                            regraICMS = servicoIcms.BuscarRegraICMS(carga, cargaPedidoPadrao, empresa, remetente, destinatario, tomador, origem, destino, ref incluirICMS, ref percentualIncluir, valorbaseRateado, cargaPedidoProdutos, unitOfWork, tipoServicoMultisoftware, configuracao, tabelaAliquotas, tomadoresFilialEmissora, tipoContratacao);

                            if (!incluirICMS)
                                valorbaseRateadoIBSCBS -= Math.Round(regraICMS.ValorICMS, 2, MidpointRounding.AwayFromZero);

                            impostoIBSCBS = servicoImpostoIBSCBS.ObterImpostoIBSCBS(new Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBS { BaseCalculo = valorbaseRateadoIBSCBS, CodigoLocalidade = cargaPedido.Pedido.Destinatario.Localidade.Codigo, SiglaUF = cargaPedido.Pedido.Destinatario.Localidade.Estado.Sigla, CodigoTipoOperacao = carga.TipoOperacao?.Codigo ?? 0, Empresa = empresa });
                        }
                        else
                        {
                            regraICMS = BuscarRegraICMSComplementoFilialEmissora(cargaPedidoPadrao, empresa, remetente, destinatario, tomador, origem, destino, ref incluirICMS, ref percentualIncluir, valorbaseRateado, null, unitOfWork, tipoServicoMultisoftware);

                            if (!incluirICMS)
                                valorbaseRateadoIBSCBS -= Math.Round(regraICMS.ValorICMS, 2, MidpointRounding.AwayFromZero);

                            impostoIBSCBS = servicoImpostoIBSCBS.ObterImpostoIBSCBSComTributacaoDefinidaFilialEmissora(cargaPedido, valorbaseRateadoIBSCBS);
                        }
                    }
                    else
                    {
                        regraICMS = BuscarRegraICMSPeloEmbarcador(cargaPedidoPadrao, empresa, remetente, destinatario, tomador, origem, destino, ref incluirICMS, ref percentualIncluir, valorbaseRateado, null, unitOfWork, tipoServicoMultisoftware);

                        if (!incluirICMS)
                            valorbaseRateadoIBSCBS -= Math.Round(regraICMS.ValorICMS, 2, MidpointRounding.AwayFromZero);

                        impostoIBSCBS = servicoImpostoIBSCBS.ObterImpostoIBSCBSComTributacaoDefinida(cargaPedido, valorbaseRateadoIBSCBS);
                    }

                    if (i == 1)
                    {
                        if ((!cargaPedidoPadrao.ImpostoInformadoPeloEmbarcador || calculoImpostosFilialEmissora) && !freteFilialEmissoraOperador)
                        {
                            if (!calculoImpostosFilialEmissora || !cargaPedidoPadrao.EmitirComplementarFilialEmissora)
                            {
                                regraICMSTotal = servicoIcms.BuscarRegraICMS(carga, cargaPedidoPadrao, empresa, remetente, destinatario, tomador, origem, destino, ref incluirICMS, ref percentualIncluir, valorBaseCalculo, cargaPedidoProdutos, unitOfWork, tipoServicoMultisoftware, configuracao, tabelaAliquotas, tomadoresFilialEmissora, tipoContratacao);

                                if (!incluirICMS)
                                    valorBaseCalculoIBSCBS -= Math.Round(regraICMSTotal.ValorICMS, 2, MidpointRounding.AwayFromZero);

                                impostoIBSCBSTotal = servicoImpostoIBSCBS.ObterImpostoIBSCBS(new Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBS { BaseCalculo = valorBaseCalculoIBSCBS, CodigoLocalidade = cargaPedido.Pedido.Destinatario.Localidade.Codigo, SiglaUF = cargaPedido.Pedido.Destinatario.Localidade.Estado.Sigla, CodigoTipoOperacao = carga.TipoOperacao?.Codigo ?? 0, Empresa = empresa });
                            }
                            else
                            {
                                regraICMSTotal = BuscarRegraICMSComplementoFilialEmissora(cargaPedidoPadrao, empresa, remetente, destinatario, tomador, origem, destino, ref incluirICMS, ref percentualIncluir, valorBaseCalculo, null, unitOfWork, tipoServicoMultisoftware);

                                if (!incluirICMS)
                                    valorBaseCalculoIBSCBS -= Math.Round(regraICMSTotal.ValorICMS, 2, MidpointRounding.AwayFromZero);

                                impostoIBSCBSTotal = servicoImpostoIBSCBS.ObterImpostoIBSCBSComTributacaoDefinidaFilialEmissora(cargaPedido, valorBaseCalculoIBSCBS);
                            }
                        }
                        else
                        {
                            regraICMSTotal = BuscarRegraICMSPeloEmbarcador(cargaPedidoPadrao, empresa, remetente, destinatario, tomador, origem, destino, ref incluirICMS, ref percentualIncluir, valorBaseCalculo, null, unitOfWork, tipoServicoMultisoftware);

                            if (!incluirICMS)
                                valorBaseCalculoIBSCBS -= Math.Round(regraICMSTotal.ValorICMS, 2, MidpointRounding.AwayFromZero);

                            impostoIBSCBSTotal = servicoImpostoIBSCBS.ObterImpostoIBSCBSComTributacaoDefinida(cargaPedido, valorBaseCalculoIBSCBS);
                        }

                        ValorRateioTotalPis = Math.Round(regraICMSTotal.ValorPis, 2, MidpointRounding.AwayFromZero);
                        ValorRateioTotalCofins = Math.Round(regraICMSTotal.ValorCofins, 2, MidpointRounding.AwayFromZero);
                        ValorRateioTotalICMS = Math.Round(regraICMSTotal.ValorICMS, 2, MidpointRounding.AwayFromZero);
                        ValorRateioTotalICMSIncluso = Math.Round(regraICMSTotal.ValorICMSIncluso, 2, MidpointRounding.AwayFromZero);
                        ValorRateioTotalBaseCalculo = Math.Round(regraICMSTotal.ValorBaseCalculoICMS, 2, MidpointRounding.AwayFromZero);
                        valorRateioTotalIBSEstadual = Math.Round(impostoIBSCBSTotal.ValorIBSEstadual, 3, MidpointRounding.AwayFromZero);
                        valorRateioTotalIBSMunicipal = Math.Round(impostoIBSCBSTotal.ValorIBSMunicipal, 3, MidpointRounding.AwayFromZero);
                        valorBaseCalculoIBSCBSTotal = Math.Round(impostoIBSCBSTotal.BaseCalculo, 3, MidpointRounding.AwayFromZero);
                        valorRateioTotalCBS = Math.Round(impostoIBSCBSTotal.ValorCBS, 3, MidpointRounding.AwayFromZero);
                    }

                    if (i == cargaPedidos.Count)
                    {
                        regraICMS.ValorPis = Math.Round(ValorRateioTotalPis, 2, MidpointRounding.AwayFromZero);
                        regraICMS.ValorCofins = Math.Round(ValorRateioTotalCofins, 2, MidpointRounding.AwayFromZero);
                        regraICMS.ValorICMS = Math.Round(ValorRateioTotalICMS, 2, MidpointRounding.AwayFromZero);
                        regraICMS.ValorICMSIncluso = Math.Round(ValorRateioTotalICMSIncluso, 2, MidpointRounding.AwayFromZero);
                        regraICMS.ValorBaseCalculoICMS = Math.Round(ValorRateioTotalBaseCalculo, 2, MidpointRounding.AwayFromZero);
                        regraICMS.ValorBaseCalculoPISCOFINS = Math.Round(ValorRateioTotalBaseCalculo, 2, MidpointRounding.AwayFromZero);

                        if (!configuracao.DesconsiderarSobraRateioParaBaseCalculoIBSCBS)
                        {
                            impostoIBSCBS.ValorIBSEstadual = Math.Round(valorRateioTotalIBSEstadual, 3, MidpointRounding.AwayFromZero);
                            impostoIBSCBS.ValorIBSMunicipal = Math.Round(valorRateioTotalIBSMunicipal, 3, MidpointRounding.AwayFromZero);
                            impostoIBSCBS.ValorCBS = Math.Round(valorRateioTotalCBS, 3, MidpointRounding.AwayFromZero);
                            impostoIBSCBS.BaseCalculo = Math.Round(valorBaseCalculoIBSCBSTotal, 3, MidpointRounding.AwayFromZero);
                        }
                    }

                    ValorRateioTotalPis -= Math.Round(regraICMS.ValorPis, 2, MidpointRounding.AwayFromZero);
                    ValorRateioTotalCofins -= Math.Round(regraICMS.ValorCofins, 2, MidpointRounding.AwayFromZero);
                    ValorRateioTotalICMS -= Math.Round(regraICMS.ValorICMS, 2, MidpointRounding.AwayFromZero);
                    ValorRateioTotalICMSIncluso -= Math.Round(regraICMS.ValorICMSIncluso, 2, MidpointRounding.AwayFromZero);
                    ValorRateioTotalBaseCalculo -= Math.Round(regraICMS.ValorBaseCalculoICMS, 2, MidpointRounding.AwayFromZero);
                    valorRateioTotalIBSEstadual -= Math.Round(impostoIBSCBS.ValorIBSEstadual, 3, MidpointRounding.AwayFromZero);
                    valorRateioTotalIBSMunicipal -= Math.Round(impostoIBSCBS.ValorIBSMunicipal, 3, MidpointRounding.AwayFromZero);
                    valorRateioTotalCBS -= Math.Round(impostoIBSCBS.ValorCBS, 3, MidpointRounding.AwayFromZero);
                    valorBaseCalculoIBSCBSTotal -= Math.Round(impostoIBSCBS.BaseCalculo, 3, MidpointRounding.AwayFromZero);

                    i++;

                    if (!calculoImpostosFilialEmissora)
                    {
                        cargaPedido.IncluirICMSBaseCalculo = incluirICMS;
                        cargaPedido.PercentualIncluirBaseCalculo = percentualIncluir;
                        cargaPedido.CFOP = regraICMS.ObjetoCFOP;
                        cargaPedido.CST = regraICMS.CST;
                        cargaPedido.ObservacaoRegraICMSCTe = regraICMS.ObservacaoCTe;
                        cargaPedido.BaseCalculoICMS = regraICMS.ValorBaseCalculoICMS;
                        cargaPedido.PercentualReducaoBC = regraICMS.PercentualReducaoBC;
                        cargaPedido.PercentualAliquota = regraICMS.Aliquota;
                        cargaPedido.AliquotaPis = regraICMS.AliquotaPis;
                        cargaPedido.AliquotaCofins = regraICMS.AliquotaCofins;
                        cargaPedido.PercentualAliquotaInternaDifal = regraICMS.AliquotaInternaDifal;
                        cargaPedido.DescontarICMSDoValorAReceber = regraICMS.DescontarICMSDoValorAReceber;
                        cargaPedido.NaoReduzirRetencaoICMSDoValorDaPrestacao = regraICMS.NaoReduzirRetencaoICMSDoValorDaPrestacao;
                        cargaPedido.NaoImprimirImpostosDACTE = regraICMS.NaoImprimirImpostosDACTE;
                        cargaPedido.NaoEnviarImpostoICMSNaEmissaoCte = regraICMS.NaoEnviarImpostoICMSNaEmissaoCte;
                        cargaPedido.ModeloDocumentoFiscal = cargaPedidoPadrao.ModeloDocumentoFiscal;

                        Servicos.Log.GravarInfo($"4 - CargaPedido: {cargaPedido.Codigo} -> Aliquota: {cargaPedido.PercentualAliquota}", "ProcessarAliquota");

                        if (cargaPedido.CST == "60")
                            cargaPedido.ICMSPagoPorST = true;

                        cargaPedido.ValorPis = Math.Round(regraICMS.ValorPis, 2, MidpointRounding.AwayFromZero);
                        cargaPedido.ValorCofins = Math.Round(regraICMS.ValorCofins, 2, MidpointRounding.AwayFromZero);
                        cargaPedido.ValorPis = Math.Round(regraICMS.ValorPis, 2, MidpointRounding.AwayFromZero);
                        cargaPedido.ValorCofins = Math.Round(regraICMS.ValorCofins, 2, MidpointRounding.AwayFromZero);
                        cargaPedido.ValorICMS = Math.Round(regraICMS.ValorICMS, 2, MidpointRounding.AwayFromZero);
                        cargaPedido.ValorICMSIncluso = Math.Round(regraICMS.ValorICMSIncluso, 2, MidpointRounding.AwayFromZero);

                        cargaPedido.PercentualCreditoPresumido = regraICMS.PercentualCreditoPresumido;
                        cargaPedido.ValorCreditoPresumido = Math.Round(regraICMS.ValorCreditoPresumido, 2, MidpointRounding.AwayFromZero);

                        serCargaPedido.PreencherCamposImpostoIBSCBS(cargaPedido, impostoIBSCBS);

                        if (cargaPedido.IncluirICMSBaseCalculo && cargaPedido.CST != "60")
                        {
                            if (cargaPedido.ValorICMSIncluso > 0)
                                cargaPedido.ValorFreteAPagar += cargaPedido.ValorICMSIncluso + cargaPedido.ValorPis + cargaPedido.ValorCofins;
                            else
                                cargaPedido.ValorFreteAPagar += cargaPedido.ValorICMS + cargaPedido.ValorPis + cargaPedido.ValorCofins;
                        }

                        decimal aliquotaPisCofins = regraICMS.AliquotaPis + regraICMS.AliquotaCofins;
                        ProcessarValoresComponenteICMSTabelaFrete(carga, cargaPedido, aliquotaPisCofins);

                        cargaPedido.SetarRegraICMS(regraICMS.CodigoRegra);

                        cargaPedido.PossuiCTe = true;
                    }
                    else
                    {
                        cargaPedido.IncluirICMSBaseCalculoFilialEmissora = incluirICMS;
                        cargaPedido.PercentualIncluirBaseCalculoFilialEmissora = percentualIncluir;
                        cargaPedido.CFOPFilialEmissora = regraICMS.ObjetoCFOP;
                        cargaPedido.CSTFilialEmissora = regraICMS.CST;
                        cargaPedido.ObservacaoRegraICMSCTeFilialEmissora = regraICMS.ObservacaoCTe;
                        cargaPedido.BaseCalculoICMSFilialEmissora = regraICMS.ValorBaseCalculoICMS;
                        cargaPedido.PercentualReducaoBCFilialEmissora = regraICMS.PercentualReducaoBC;
                        cargaPedido.PercentualAliquotaFilialEmissora = regraICMS.Aliquota;
                        cargaPedido.PercentualAliquotaFilialEmissoraInternaDifal = 0;
                        cargaPedido.DescontarICMSDoValorAReceber = regraICMS.DescontarICMSDoValorAReceber;
                        cargaPedido.NaoReduzirRetencaoICMSDoValorDaPrestacao = regraICMS.NaoReduzirRetencaoICMSDoValorDaPrestacao;
                        cargaPedido.NaoImprimirImpostosDACTE = regraICMS.NaoImprimirImpostosDACTE;
                        cargaPedido.NaoEnviarImpostoICMSNaEmissaoCte = regraICMS.NaoEnviarImpostoICMSNaEmissaoCte;

                        if (cargaPedido.CSTFilialEmissora == "60")
                            cargaPedido.ICMSPagoPorSTFilialEmissora = true;

                        cargaPedido.ValorICMSFilialEmissora = Math.Round(regraICMS.ValorICMS, 2, MidpointRounding.AwayFromZero);
                        cargaPedido.ValorICMSFilialEmissora = Math.Round(regraICMS.ValorICMSIncluso, 2, MidpointRounding.AwayFromZero);

                        cargaPedido.PercentualCreditoPresumidoFilialEmissora = regraICMS.PercentualCreditoPresumido;
                        cargaPedido.ValorCreditoPresumidoFilialEmissora = Math.Round(regraICMS.ValorCreditoPresumido, 2, MidpointRounding.AwayFromZero);

                        serCargaPedido.PreencherCamposImpostoIBSCBSFilialEmissora(cargaPedido, impostoIBSCBS);

                        if (cargaPedido.IncluirICMSBaseCalculoFilialEmissora && cargaPedido.CSTFilialEmissora != "60")
                            cargaPedido.ValorFreteAPagarFilialEmissora += cargaPedido.ValorICMSFilialEmissora;
                    }

                }
                else
                {
                    if (!calculoImpostosFilialEmissora)
                    {
                        cargaPedido.IncluirICMSBaseCalculo = false;
                        cargaPedido.PercentualIncluirBaseCalculo = 0m;
                        cargaPedido.ObservacaoRegraICMSCTe = "";
                        cargaPedido.BaseCalculoICMS = 0m;
                        cargaPedido.PercentualReducaoBC = 0m;
                        cargaPedido.PercentualAliquota = 0m;
                        cargaPedido.AliquotaPis = 0m;
                        cargaPedido.AliquotaCofins = 0m;
                        cargaPedido.ValorICMS = 0m;
                        cargaPedido.ValorPis = 0m;
                        cargaPedido.ValorCofins = 0m;
                        cargaPedido.ValorICMSIncluso = 0m;
                        cargaPedido.PercentualCreditoPresumido = 0m;
                        cargaPedido.ValorCreditoPresumido = 0m;
                        cargaPedido.PossuiCTe = false;
                        if (cargaPedido.ModeloDocumentoFiscal != null && cargaPedido.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                            cargaPedido.ModeloDocumentoFiscal = null;
                        cargaPedido.DescontarICMSDoValorAReceber = false;
                        cargaPedido.NaoReduzirRetencaoICMSDoValorDaPrestacao = false;
                        cargaPedido.NaoImprimirImpostosDACTE = false;
                        cargaPedido.NaoEnviarImpostoICMSNaEmissaoCte = false;
                        cargaPedido.RegraICMS = null;

                        serCargaPedido.ZerarCamposImpostoIBSCBS(cargaPedido);

                        Servicos.Log.GravarInfo($"5 - CargaPedido: {cargaPedido.Codigo} -> Aliquota: {cargaPedido.PercentualAliquota}", "ProcessarAliquota");
                    }
                    else
                    {
                        cargaPedido.IncluirICMSBaseCalculoFilialEmissora = false;
                        cargaPedido.PercentualIncluirBaseCalculoFilialEmissora = 0m;
                        cargaPedido.ObservacaoRegraICMSCTeFilialEmissora = "";
                        cargaPedido.BaseCalculoICMSFilialEmissora = 0m;
                        cargaPedido.PercentualReducaoBCFilialEmissora = 0m;
                        cargaPedido.PercentualAliquotaFilialEmissora = 0m;
                        cargaPedido.ValorICMSFilialEmissora = 0m;
                        cargaPedido.PercentualCreditoPresumidoFilialEmissora = 0m;
                        cargaPedido.ValorCreditoPresumidoFilialEmissora = 0m;
                        cargaPedido.DescontarICMSDoValorAReceber = false;
                        cargaPedido.NaoReduzirRetencaoICMSDoValorDaPrestacao = false;
                        cargaPedido.NaoEnviarImpostoICMSNaEmissaoCte = false;
                        cargaPedido.NaoImprimirImpostosDACTE = false;

                        serCargaPedido.ZerarCamposImpostoIBSCBSFilialEmissora(cargaPedido);
                    }
                }
                if (!calculoImpostosFilialEmissora)
                {
                    bool setarImportosISS = false;
                    if (possuiNFS)
                    {
                        setarImportosISS = true;
                        if (cargaPedido.ModeloDocumentoFiscal == null || cargaPedido.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                            cargaPedido.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.NFSe);

                        cargaPedido.PossuiNFS = true;
                    }
                    else
                    {
                        cargaPedido.PossuiNFS = false;
                        if (cargaPedido.ModeloDocumentoFiscal != null && cargaPedido.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                            cargaPedido.ModeloDocumentoFiscal = null;
                    }

                    if (possuiNFSManual)
                    {
                        setarImportosISS = true;
                        cargaPedido.PossuiNFSManual = true;
                        cargaPedido.ModeloDocumentoFiscal = null;
                        cargaPedido.ModeloDocumentoFiscalIntramunicipal = modeloDocumentoIntramunicipal;
                    }
                    else
                    {
                        cargaPedido.PossuiNFSManual = false;
                        cargaPedido.ModeloDocumentoFiscalIntramunicipal = null;
                    }

                    //Calcula ISS
                    if (setarImportosISS)
                    {
                        Servicos.Log.GravarInfo($"Carga: {carga.Codigo} - Inicio setarImportosISS", "LogCalculoISS");

                        Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS = serCargaISS.BuscarRegraISS(empresa, valorbaseRateado, cargaPedido.Destino, carga.TipoOperacao, tomador, null, carga?.TipoDeCarga?.NBS ?? "", unitOfWork);
                        if (regraISS != null)
                        {

                            decimal valorBaseIBSCBSRateada = valorbaseRateadoIBSCBS;
                            if (regraISS.IncluirISSBaseCalculo)
                                valorBaseIBSCBSRateada += Math.Round(regraISS.ValorISS, 2, MidpointRounding.AwayFromZero);

                            Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = servicoImpostoIBSCBS.ObterImpostoIBSCBS(new Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBS
                            {
                                BaseCalculo = valorBaseIBSCBSRateada,
                                ValoAbaterBaseCalculo = Math.Round(regraISS.ValorISS, 2, MidpointRounding.AwayFromZero),
                                CodigoLocalidade = cargaPedido.Pedido.Destinatario.Localidade.Codigo,
                                SiglaUF = cargaPedido.Pedido.Destinatario.Localidade.Estado.Sigla,
                                CodigoTipoOperacao = carga.TipoOperacao?.Codigo ?? 0,
                                Empresa = empresa
                            });

                            impostoIBSCBS.NBS = regraISS.NBS;

                            if (contadorISS == 1)
                            {
                                Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISSTotal = serCargaISS.BuscarRegraISS(empresa, valorBaseCalculo, cargaPedido.Destino, carga.TipoOperacao, tomador, null, carga?.TipoDeCarga?.NBS ?? "", unitOfWork);
                                Servicos.Log.GravarInfo($"Valor ISS pela regraISSTotal: {regraISSTotal.ValorISS}", "LogCalculoISS");
                                ValorRateioTotalISS = Math.Round(regraISSTotal.ValorISS, 2, MidpointRounding.AwayFromZero);
                                ValorRateioTotalBaseCalculoISS = Math.Round(regraISSTotal.ValorBaseCalculoISS, 2, MidpointRounding.AwayFromZero);
                                ValorRateioTotalRetencaoISS = Math.Round(regraISSTotal.ValorRetencaoISS, 2, MidpointRounding.AwayFromZero);

                                ValorRateioTotalIR = Math.Round(regraISSTotal.ValorIR, 2, MidpointRounding.AwayFromZero);
                                ValorRateioTotalBaseCalculoIR = Math.Round(regraISSTotal.BaseCalculoIR, 2, MidpointRounding.AwayFromZero);
                                ValorRateioTotalRetencaoIR = Math.Round(regraISSTotal.AliquotaIR, 2, MidpointRounding.AwayFromZero);

                                valorRateioTotalBaseCalculoIBSCBS = valorBaseCalculo;

                                if (regraISSTotal.IncluirISSBaseCalculo)
                                    valorRateioTotalBaseCalculoIBSCBS += Math.Round(regraISSTotal.ValorISS, 2, MidpointRounding.AwayFromZero);

                                impostoIBSCBSTotal = servicoImpostoIBSCBS.ObterImpostoIBSCBS(new Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBS
                                {
                                    BaseCalculo = valorRateioTotalBaseCalculoIBSCBS,
                                    ValoAbaterBaseCalculo = Math.Round(regraISSTotal.ValorISS, 2, MidpointRounding.AwayFromZero),
                                    CodigoLocalidade = cargaPedido.Pedido.Destinatario.Localidade.Codigo,
                                    SiglaUF = cargaPedido.Pedido.Destinatario.Localidade.Estado.Sigla,
                                    CodigoTipoOperacao = carga.TipoOperacao?.Codigo ?? 0,
                                    Empresa = empresa
                                });

                                impostoIBSCBSTotal.NBS = regraISSTotal.NBS;

                            }

                            if (contadorISS == cargaPedidos.Count)
                            {
                                Servicos.Log.GravarInfo($"Valor ISS pela regraISS: {regraISS.ValorISS}", "LogCalculoISS");
                                regraISS.ValorISS = Math.Round(ValorRateioTotalISS, 2, MidpointRounding.AwayFromZero);
                                regraISS.ValorBaseCalculoISS = Math.Round(ValorRateioTotalBaseCalculoISS, 2, MidpointRounding.AwayFromZero);
                                regraISS.ValorRetencaoISS = Math.Round(ValorRateioTotalRetencaoISS, 2, MidpointRounding.AwayFromZero);

                                regraISS.ValorIR = Math.Round(ValorRateioTotalIR, 2, MidpointRounding.AwayFromZero);
                                regraISS.BaseCalculoIR = Math.Round(ValorRateioTotalBaseCalculoIR, 2, MidpointRounding.AwayFromZero);
                                regraISS.AliquotaIR = Math.Round(ValorRateioTotalRetencaoIR, 2, MidpointRounding.AwayFromZero);

                                impostoIBSCBS.ValorCBS = Math.Round(impostoIBSCBSTotal.ValorCBS, 3, MidpointRounding.AwayFromZero);
                                impostoIBSCBS.ValorIBSEstadual = Math.Round(impostoIBSCBSTotal.ValorIBSEstadual, 3, MidpointRounding.AwayFromZero);
                                impostoIBSCBS.ValorIBSMunicipal = Math.Round(impostoIBSCBSTotal.ValorIBSMunicipal, 3, MidpointRounding.AwayFromZero);
                                impostoIBSCBS.BaseCalculo = Math.Round(impostoIBSCBSTotal.BaseCalculo, 3, MidpointRounding.AwayFromZero);
                            }

                            ValorRateioTotalISS -= Math.Round(regraISS.ValorISS, 2, MidpointRounding.AwayFromZero);
                            ValorRateioTotalBaseCalculoISS -= Math.Round(regraISS.ValorBaseCalculoISS, 2, MidpointRounding.AwayFromZero);
                            ValorRateioTotalRetencaoISS -= Math.Round(regraISS.ValorRetencaoISS, 2, MidpointRounding.AwayFromZero);

                            ValorRateioTotalIR -= Math.Round(regraISS.ValorIR, 2, MidpointRounding.AwayFromZero);
                            ValorRateioTotalBaseCalculoIR -= Math.Round(regraISS.BaseCalculoIR, 2, MidpointRounding.AwayFromZero);
                            ValorRateioTotalRetencaoIR -= Math.Round(regraISS.AliquotaIR, 2, MidpointRounding.AwayFromZero);
                            impostoIBSCBSTotal.ValorCBS -= Math.Round(impostoIBSCBS.ValorCBS, 3, MidpointRounding.AwayFromZero);
                            impostoIBSCBSTotal.ValorIBSEstadual -= Math.Round(impostoIBSCBS.ValorIBSEstadual, 3, MidpointRounding.AwayFromZero);
                            impostoIBSCBSTotal.ValorIBSMunicipal -= Math.Round(impostoIBSCBS.ValorIBSMunicipal, 3, MidpointRounding.AwayFromZero);
                            impostoIBSCBSTotal.BaseCalculo -= Math.Round(impostoIBSCBS.BaseCalculo, 3, MidpointRounding.AwayFromZero);

                            contadorISS++;

                            cargaPedido.ValorISS = Math.Round(regraISS.ValorISS, 2, MidpointRounding.AwayFromZero);
                            cargaPedido.BaseCalculoISS = regraISS.ValorBaseCalculoISS;
                            cargaPedido.PercentualAliquotaISS = regraISS.AliquotaISS;
                            cargaPedido.PercentualRetencaoISS = regraISS.PercentualRetencaoISS;
                            cargaPedido.IncluirISSBaseCalculo = regraISS.IncluirISSBaseCalculo;
                            cargaPedido.ValorRetencaoISS = Math.Round(regraISS.ValorRetencaoISS, 2, MidpointRounding.AwayFromZero);

                            cargaPedido.ReterIR = regraISS.ReterIR;
                            cargaPedido.AliquotaIR = regraISS.AliquotaIR;
                            cargaPedido.BaseCalculoIR = regraISS.BaseCalculoIR;
                            cargaPedido.ValorIR = Math.Round(regraISS.ValorIR, 2, MidpointRounding.AwayFromZero);

                            serCargaPedido.PreencherCamposImpostoIBSCBS(cargaPedido, impostoIBSCBS);

                        }

                        if (cargaPedido.IncluirISSBaseCalculo)
                            cargaPedido.ValorFreteAPagar += cargaPedido.ValorISS - cargaPedido.ValorRetencaoISS;
                    }
                    else
                    {
                        cargaPedido.ValorISS = 0;
                        cargaPedido.BaseCalculoISS = 0;
                        cargaPedido.PercentualAliquotaISS = 0;
                        cargaPedido.PercentualRetencaoISS = 0;
                        cargaPedido.IncluirISSBaseCalculo = false;
                        cargaPedido.ValorRetencaoISS = 0;

                        cargaPedido.ReterIR = false;
                        cargaPedido.AliquotaIR = 0;
                        cargaPedido.BaseCalculoIR = 0;
                        cargaPedido.ValorIR = 0;
                    }

                    var cargaPedidoEmissoa = cargaPedido;
                    serCargaPedido.VerificarFilialEmissaoCargaPedido(cargaPedidoEmissoa, configuracaoGeralCarga);
                }

                repCargaPedido.Atualizar(cargaPedido);

                if (!calculoImpostosFilialEmissora)
                {
                    Dominio.ObjetosDeValor.Embarcador.Carga.CargaValoresAcrescentar cargaValoresAcrescentar = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaValoresAcrescentar()
                    {
                        ValorFrete = cargaPedido.ValorFrete,
                        ValorFreteLiquido = 0m,
                        ValorFreteAPagar = cargaPedido.ValorFreteAPagar,
                        ValorICMS = cargaPedido.ValorICMS,
                        ValorPis = cargaPedido.ValorPis,
                        ValorCofins = cargaPedido.ValorCofins,
                        ValorISS = cargaPedido.ValorISS,
                        ValorRetencaoISS = cargaPedido.ValorRetencaoISS,
                        ValorTotalMoedaPagar = cargaPedido.ValorTotalMoedaPagar ?? 0m,
                        ValorIBSEstadual = cargaPedido.ValorIBSEstadual,
                        ValorIBSMunicipal = cargaPedido.ValorIBSMunicipal,
                        ValorCBS = cargaPedido.ValorCBS,
                    };

                    AcrescentarValoresDaCarga(carga, cargaValoresAcrescentar, cargaPedido);
                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Carga.CargaValoresAcrescentar cargaValoresAcrescentar = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaValoresAcrescentar()
                    {
                        ValorFrete = cargaPedido.ValorFreteFilialEmissora,
                        ValorFreteAPagar = cargaPedido.ValorFreteAPagarFilialEmissora,
                        ValorICMS = cargaPedido.ValorICMSFilialEmissora,
                        ValorIBSEstadual = cargaPedido.ValorIBSEstadualFilialEmissora,
                        ValorIBSMunicipal = cargaPedido.ValorIBSMunicipalFilialEmissora,
                        ValorCBS = cargaPedido.ValorCBSFilialEmissora,
                    };

                    AcrescentarValoresFilialEmissoraDaCarga(carga, cargaValoresAcrescentar);
                }
            }
        }

        public void CalcularImpostos(ref Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, decimal valorFretePedido, bool calculoImpostosFilialEmissora, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesFretes, List<Dominio.Entidades.Embarcador.ICMS.PedagioEstadoBaseCalculo> pedagioEstadosBaseCalculo, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos, List<Dominio.ObjetosDeValor.Embarcador.ICMS.TabelaAliquota> tabelaAliquotas, List<Dominio.Entidades.Cliente> tomadoresFilialEmissora, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga)
        {
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            Servicos.Embarcador.Carga.ICMS servicoIcms = new Servicos.Embarcador.Carga.ICMS(unitOfWork);
            Servicos.Embarcador.Carga.ISS serCargaISS = new Servicos.Embarcador.Carga.ISS(unitOfWork);
            Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);
            Servicos.Embarcador.Imposto.ImpostoIBSCBS servicoImpostoIBSCBS = new Servicos.Embarcador.Imposto.ImpostoIBSCBS(unitOfWork);

            Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = carga.TabelaFrete ?? null;

            decimal valorBaseCalculo = 0;
            if (!calculoImpostosFilialEmissora)
            {
                cargaPedido.ValorFrete = Math.Round(valorFretePedido, 2, MidpointRounding.AwayFromZero);
                valorBaseCalculo = cargaPedido.ValorFrete;
            }
            else
            {
                cargaPedido.ValorFreteFilialEmissora = Math.Round(valorFretePedido, 2, MidpointRounding.AwayFromZero);
                valorBaseCalculo = cargaPedido.ValorFreteFilialEmissora;
            }

            decimal valorBaseCalculoIBSCBS = valorBaseCalculo;

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesFretesSemFreteValor = (from obj in cargaPedidoComponentesFretes
                                                                                                                               where obj.CargaPedido.Codigo == cargaPedido.Codigo
                                                                                                                               && (obj.ComponenteFrete == null || !obj.ComponenteFrete.ComponentePertenceComposicaoFreteValor)
                                                                                                                               && !(obj.ComponenteFrete?.ComponenteApenasInformativoDocumentoEmitido ?? false)
                                                                                                                               && obj.ComponenteFilialEmissora == calculoImpostosFilialEmissora
                                                                                                                               && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS
                                                                                                                               && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS
                                                                                                                               && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS
                                                                                                                               && ((cargaPedido.ModeloDocumentoFiscal == null && (obj.ModeloDocumentoFiscal == null || (obj.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)))
                                                                                                                               || cargaPedido.ModeloDocumentoFiscal != null && (obj.ModeloDocumentoFiscal == null || obj.ModeloDocumentoFiscal.Codigo == cargaPedido.ModeloDocumentoFiscal.Codigo))
                                                                                                                               select obj).ToList();

            List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> componentes = new List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico>();
            Dominio.Entidades.Empresa empresa = calculoImpostosFilialEmissora ? cargaOrigem.EmpresaFilialEmissora : cargaOrigem.Empresa;

            for (int i = 0; i < cargaPedidoComponentesFretesSemFreteValor.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponenteFrete = cargaPedidoComponentesFretesSemFreteValor[i];
                valorBaseCalculo += servicoIcms.ObterValorIcmsComponenteFrete(cargaPedidoComponenteFrete, empresa, cargaPedido.Origem.Estado.Sigla, pedagioEstadosBaseCalculo, unitOfWork, tipoServicoMultisoftware);
                valorBaseCalculoIBSCBS += cargaPedidoComponenteFrete.ValorComponente;
            }

            if (tabelaFrete?.NaoAdicionarOValorDoComponenteABaseDeCalculoDoICMS ?? false)
            {
                valorBaseCalculo -= cargaPedidoComponentesFretesSemFreteValor
                    .Where(x => x.IncluirBaseCalculoICMS && x.ComponenteFrete.Codigo == tabelaFrete.ComponenteFreteDestacar.Codigo)
                    .Sum(obj => obj.ValorComponente);
            }

            bool naoIncluirValorICMSBasecalculoQuandoValorFreteInformadoOperador = carga.TipoOperacao?.ConfiguracaoCalculoFrete?.NaoIncluirValorICMSBasecalculoQuandoValorFreteInformadoOperador ?? false;

            Dominio.Entidades.Cliente tomador = cargaPedido.ObterTomador();

            if (tomador == null)
                throw new ServicoException("Tomador não encontrado");

            bool possuiCTe = false;
            bool possuiNFS = false;
            bool possuiNFSManual = false;
            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoIntramunicipal = null;

            serCargaPedido.VerificarQuaisDocumentosDeveEmitir(carga, cargaPedido, cargaPedido.Origem, cargaPedido.Destino, tipoServicoMultisoftware, unitOfWork, out possuiCTe, out possuiNFS, out possuiNFSManual, out modeloDocumentoIntramunicipal, configuracao, out bool sempreDisponibilizarDocumentoNFSManual);

            cargaPedido.DisponibilizarDocumentoNFSManual = sempreDisponibilizarDocumentoNFSManual;

            decimal valorTotalComponente = Math.Round((cargaPedidoComponentesFretesSemFreteValor.Sum(obj => obj.ValorComponente)), 2, MidpointRounding.AwayFromZero);

            if (tabelaFrete?.DescontarDoValorAReceberValorComponente ?? false)
            {
                valorTotalComponente -= cargaPedidoComponentesFretesSemFreteValor
                    .Where(x => x.ComponenteFrete.Codigo == tabelaFrete.ComponenteFreteDestacar.Codigo)
                    .Sum(obj => obj.ValorComponente);
            }

            if (!calculoImpostosFilialEmissora)
                cargaPedido.ValorFreteAPagar = Math.Round((cargaPedido.ValorFrete + valorTotalComponente), 2, MidpointRounding.AwayFromZero);
            else
                cargaPedido.ValorFreteAPagarFilialEmissora = Math.Round((cargaPedido.ValorFreteFilialEmissora + valorTotalComponente), 2, MidpointRounding.AwayFromZero);

            if (possuiCTe || (cargaPedido.ModeloDocumentoFiscal?.CalcularImpostos ?? false))//calcula ICMS
            {
                bool incluirICMS = !calculoImpostosFilialEmissora ? cargaPedido.IncluirICMSBaseCalculo : cargaPedido.IncluirICMSBaseCalculoFilialEmissora;
                decimal percentualIncluir = !calculoImpostosFilialEmissora ? cargaPedido.PercentualIncluirBaseCalculo : cargaPedido.PercentualIncluirBaseCalculoFilialEmissora;

                if (cargaPedido.ModeloDocumentoFiscal == null || cargaPedido.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                {
                    if (ModeloDocumentoFiscalCTePadrao == null)
                        ModeloDocumentoFiscalCTePadrao = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.CTe);

                    cargaPedido.ModeloDocumentoFiscal = ModeloDocumentoFiscalCTePadrao;
                }


                Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = null;
                Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = null;

                Dominio.Entidades.Cliente remetente = cargaPedido.Pedido.Remetente;
                Dominio.Entidades.Cliente destinatario = cargaPedido.Pedido.Destinatario;

                Dominio.Entidades.Localidade origem = cargaPedido.Origem;
                Dominio.Entidades.Localidade destino = cargaPedido.Destino;

                if (calculoImpostosFilialEmissora)
                {
                    if (cargaPedido.CargaPedidoFilialEmissora && (cargaPedido.CargaPedidoProximoTrecho != null || cargaPedido.ProximoTrechoComplementaFilialEmissora))
                        destino = destinatario.Localidade;
                }

                if (carga.TipoOperacao?.ConfiguracaoEmissaoDocumento?.EmitirDocumentoSempreOrigemDestinoPedido ?? false)
                {
                    origem = remetente.Localidade;
                    destino = destinatario.Localidade;
                }

                if (configuracao.UtilizarLocalidadePrestacaoPedido)
                {
                    if (cargaPedido.Pedido.LocalidadeInicioPrestacao != null)
                        origem = cargaPedido.Pedido.LocalidadeInicioPrestacao;

                    if (cargaPedido.Pedido.LocalidadeTerminoPrestacao != null)
                        destino = cargaPedido.Pedido.LocalidadeTerminoPrestacao;
                }

                if (!cargaPedido.ImpostoInformadoPeloEmbarcador || calculoImpostosFilialEmissora)
                {
                    if (!calculoImpostosFilialEmissora || !cargaPedido.EmitirComplementarFilialEmissora)
                    {
                        regraICMS = servicoIcms.BuscarRegraICMS(carga, cargaPedido, empresa, remetente, destinatario, tomador, origem, destino, ref incluirICMS, ref percentualIncluir, valorBaseCalculo, cargaPedidoProdutos, unitOfWork, tipoServicoMultisoftware, configuracao, tabelaAliquotas, tomadoresFilialEmissora);

                        if (!incluirICMS)
                            valorBaseCalculoIBSCBS -= Math.Round(regraICMS.ValorICMS, 2, MidpointRounding.AwayFromZero);

                        impostoIBSCBS = servicoImpostoIBSCBS.ObterImpostoIBSCBS(new Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBS { BaseCalculo = valorBaseCalculoIBSCBS, CodigoLocalidade = cargaPedido.Pedido.Destinatario.Localidade.Codigo, SiglaUF = cargaPedido.Pedido.Destinatario.Localidade.Estado.Sigla, CodigoTipoOperacao = carga.TipoOperacao?.Codigo ?? 0, Empresa = empresa });
                    }
                    else
                    {
                        regraICMS = BuscarRegraICMSComplementoFilialEmissora(cargaPedido, empresa, remetente, destinatario, tomador, origem, destino, ref incluirICMS, ref percentualIncluir, valorBaseCalculo, null, unitOfWork, tipoServicoMultisoftware);

                        if (!incluirICMS)
                            valorBaseCalculoIBSCBS -= Math.Round(regraICMS.ValorICMS, 2, MidpointRounding.AwayFromZero);

                        impostoIBSCBS = servicoImpostoIBSCBS.ObterImpostoIBSCBSComTributacaoDefinidaFilialEmissora(cargaPedido, valorBaseCalculoIBSCBS);
                    }
                }
                else
                {
                    regraICMS = BuscarRegraICMSPeloEmbarcador(cargaPedido, empresa, remetente, destinatario, tomador, origem, destino, ref incluirICMS, ref percentualIncluir, valorBaseCalculo, null, unitOfWork, tipoServicoMultisoftware);

                    if (!incluirICMS)
                        valorBaseCalculoIBSCBS -= Math.Round(regraICMS.ValorICMS, 2, MidpointRounding.AwayFromZero);

                    impostoIBSCBS = servicoImpostoIBSCBS.ObterImpostoIBSCBSComTributacaoDefinida(cargaPedido, valorBaseCalculoIBSCBS);
                }

                if (cargaPedido.Expedidor != null && (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor))
                    remetente = cargaPedido.Expedidor;

                if (cargaPedido.Recebedor != null && ((cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor) && !(cargaPedido.Carga.TipoOperacao?.UtilizarRecebedorApenasComoParticipante ?? false)))
                    destinatario = cargaPedido.Recebedor;

                if (!calculoImpostosFilialEmissora)
                {
                    cargaPedido.IncluirICMSBaseCalculo = incluirICMS;
                    cargaPedido.PercentualIncluirBaseCalculo = percentualIncluir;
                    cargaPedido.CFOP = regraICMS.ObjetoCFOP;
                    cargaPedido.CST = regraICMS.CST;
                    cargaPedido.ObservacaoRegraICMSCTe = regraICMS.ObservacaoCTe;
                    cargaPedido.BaseCalculoICMS = regraICMS.ValorBaseCalculoICMS;
                    cargaPedido.PercentualReducaoBC = regraICMS.PercentualReducaoBC;
                    cargaPedido.PercentualAliquota = regraICMS.Aliquota;
                    cargaPedido.AliquotaPis = regraICMS.AliquotaPis;
                    cargaPedido.AliquotaCofins = regraICMS.AliquotaCofins;
                    cargaPedido.PercentualAliquotaInternaDifal = regraICMS.AliquotaInternaDifal;
                    cargaPedido.DescontarICMSDoValorAReceber = regraICMS.DescontarICMSDoValorAReceber;
                    cargaPedido.NaoReduzirRetencaoICMSDoValorDaPrestacao = regraICMS.NaoReduzirRetencaoICMSDoValorDaPrestacao;
                    cargaPedido.NaoImprimirImpostosDACTE = regraICMS.NaoImprimirImpostosDACTE;
                    cargaPedido.NaoEnviarImpostoICMSNaEmissaoCte = regraICMS.NaoEnviarImpostoICMSNaEmissaoCte;

                    Servicos.Log.GravarInfo($"6 - CargaPedido: {cargaPedido.Codigo} -> Aliquota: {cargaPedido.PercentualAliquota}", "ProcessarAliquota");

                    if (cargaPedido.CST == "60")
                        cargaPedido.ICMSPagoPorST = true;

                    cargaPedido.ValorPis = Math.Round(regraICMS.ValorPis, 2, MidpointRounding.AwayFromZero);
                    cargaPedido.ValorCofins = Math.Round(regraICMS.ValorCofins, 2, MidpointRounding.AwayFromZero);
                    cargaPedido.ValorICMS = Math.Round(regraICMS.ValorICMS, 2, MidpointRounding.AwayFromZero);
                    cargaPedido.ValorICMSIncluso = Math.Round(regraICMS.ValorICMSIncluso, 2, MidpointRounding.AwayFromZero);

                    cargaPedido.PercentualCreditoPresumido = regraICMS.PercentualCreditoPresumido;
                    cargaPedido.ValorCreditoPresumido = Math.Round(regraICMS.ValorCreditoPresumido, 2, MidpointRounding.AwayFromZero);

                    serCargaPedido.PreencherCamposImpostoIBSCBS(cargaPedido, impostoIBSCBS);

                    if (cargaPedido.IncluirICMSBaseCalculo && cargaPedido.CST != "60" && !naoIncluirValorICMSBasecalculoQuandoValorFreteInformadoOperador)
                    {
                        if (cargaPedido.ValorICMSIncluso > 0)
                            cargaPedido.ValorFreteAPagar += cargaPedido.ValorICMSIncluso + cargaPedido.ValorPis + cargaPedido.ValorCofins;
                        else
                            cargaPedido.ValorFreteAPagar += cargaPedido.ValorICMS + cargaPedido.ValorPis + cargaPedido.ValorCofins;
                    }

                    decimal aliquotaPisCofins = regraICMS.AliquotaPis + regraICMS.AliquotaCofins;
                    ProcessarValoresComponenteICMSTabelaFrete(carga, cargaPedido, aliquotaPisCofins);

                    cargaPedido.SetarRegraICMS(regraICMS.CodigoRegra);

                    cargaPedido.PossuiCTe = true;
                }
                else
                {
                    cargaPedido.IncluirICMSBaseCalculoFilialEmissora = incluirICMS;
                    cargaPedido.PercentualIncluirBaseCalculoFilialEmissora = percentualIncluir;
                    cargaPedido.CFOPFilialEmissora = regraICMS.ObjetoCFOP;
                    cargaPedido.CSTFilialEmissora = regraICMS.CST;
                    cargaPedido.ObservacaoRegraICMSCTeFilialEmissora = regraICMS.ObservacaoCTe;
                    cargaPedido.BaseCalculoICMSFilialEmissora = regraICMS.ValorBaseCalculoICMS;
                    cargaPedido.PercentualReducaoBCFilialEmissora = regraICMS.PercentualReducaoBC;
                    cargaPedido.PercentualAliquotaFilialEmissora = regraICMS.Aliquota;
                    cargaPedido.PercentualAliquotaFilialEmissoraInternaDifal = 0;
                    cargaPedido.DescontarICMSDoValorAReceber = regraICMS.DescontarICMSDoValorAReceber;
                    cargaPedido.NaoReduzirRetencaoICMSDoValorDaPrestacao = regraICMS.NaoReduzirRetencaoICMSDoValorDaPrestacao;
                    cargaPedido.NaoImprimirImpostosDACTE = regraICMS.NaoImprimirImpostosDACTE;
                    cargaPedido.NaoEnviarImpostoICMSNaEmissaoCte = regraICMS.NaoEnviarImpostoICMSNaEmissaoCte;

                    if (cargaPedido.CSTFilialEmissora == "60")
                        cargaPedido.ICMSPagoPorSTFilialEmissora = true;

                    cargaPedido.ValorICMSFilialEmissora = Math.Round(regraICMS.ValorICMS, 2, MidpointRounding.AwayFromZero);

                    cargaPedido.PercentualCreditoPresumidoFilialEmissora = regraICMS.PercentualCreditoPresumido;
                    cargaPedido.ValorCreditoPresumidoFilialEmissora = Math.Round(regraICMS.ValorCreditoPresumido, 2, MidpointRounding.AwayFromZero);

                    serCargaPedido.PreencherCamposImpostoIBSCBSFilialEmissora(cargaPedido, impostoIBSCBS);

                    if (cargaPedido.IncluirICMSBaseCalculoFilialEmissora && cargaPedido.CSTFilialEmissora != "60")
                        cargaPedido.ValorFreteAPagarFilialEmissora += cargaPedido.ValorICMSFilialEmissora;

                    cargaPedido.PossuiCTe = true;
                }

            }
            else
            {
                if (!calculoImpostosFilialEmissora)
                {
                    cargaPedido.IncluirICMSBaseCalculo = false;
                    cargaPedido.PercentualIncluirBaseCalculo = 0m;
                    cargaPedido.ObservacaoRegraICMSCTe = "";
                    cargaPedido.BaseCalculoICMS = 0m;
                    cargaPedido.PercentualReducaoBC = 0m;
                    cargaPedido.PercentualAliquota = 0m;
                    cargaPedido.AliquotaCofins = 0m;
                    cargaPedido.AliquotaPis = 0m;
                    cargaPedido.ValorICMS = 0m;
                    cargaPedido.ValorPis = 0m;
                    cargaPedido.ValorCofins = 0m;
                    cargaPedido.ValorICMSIncluso = 0m;
                    cargaPedido.PercentualCreditoPresumido = 0m;
                    cargaPedido.ValorCreditoPresumido = 0m;
                    cargaPedido.PossuiCTe = false;
                    if (cargaPedido.ModeloDocumentoFiscal != null && cargaPedido.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                        cargaPedido.ModeloDocumentoFiscal = null;
                    cargaPedido.DescontarICMSDoValorAReceber = false;
                    cargaPedido.NaoEnviarImpostoICMSNaEmissaoCte = false;
                    cargaPedido.NaoReduzirRetencaoICMSDoValorDaPrestacao = false;
                    cargaPedido.NaoImprimirImpostosDACTE = false;

                    serCargaPedido.ZerarCamposImpostoIBSCBS(cargaPedido);
                }
                else
                {
                    cargaPedido.IncluirICMSBaseCalculoFilialEmissora = false;
                    cargaPedido.PercentualIncluirBaseCalculoFilialEmissora = 0m;
                    cargaPedido.ObservacaoRegraICMSCTeFilialEmissora = "";
                    cargaPedido.BaseCalculoICMSFilialEmissora = 0m;
                    cargaPedido.PercentualReducaoBCFilialEmissora = 0m;
                    cargaPedido.PercentualAliquotaFilialEmissora = 0m;
                    cargaPedido.ValorICMSFilialEmissora = 0m;
                    cargaPedido.ValorICMSIncluso = 0m;
                    cargaPedido.PercentualCreditoPresumidoFilialEmissora = 0m;
                    cargaPedido.ValorCreditoPresumidoFilialEmissora = 0m;
                    cargaPedido.DescontarICMSDoValorAReceber = false;
                    cargaPedido.NaoEnviarImpostoICMSNaEmissaoCte = false;
                    cargaPedido.NaoReduzirRetencaoICMSDoValorDaPrestacao = false;
                    cargaPedido.NaoImprimirImpostosDACTE = false;

                    serCargaPedido.ZerarCamposImpostoIBSCBSFilialEmissora(cargaPedido);
                }
            }

            if (!calculoImpostosFilialEmissora)
            {
                bool setarImportosISS = false;
                if (possuiNFS)
                {
                    setarImportosISS = true;
                    if (cargaPedido.ModeloDocumentoFiscal == null || cargaPedido.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                        cargaPedido.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.NFSe);

                    cargaPedido.PossuiNFS = true;
                }
                else
                {
                    cargaPedido.PossuiNFS = false;
                    if (cargaPedido.ModeloDocumentoFiscal != null && cargaPedido.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                        cargaPedido.ModeloDocumentoFiscal = null;
                }

                if (possuiNFSManual)
                {
                    setarImportosISS = true;
                    cargaPedido.PossuiNFSManual = true;
                    cargaPedido.ModeloDocumentoFiscal = null;
                    cargaPedido.ModeloDocumentoFiscalIntramunicipal = modeloDocumentoIntramunicipal;
                }
                else
                {
                    cargaPedido.PossuiNFSManual = false;
                    cargaPedido.ModeloDocumentoFiscalIntramunicipal = null;
                }

                //Calcula ISS
                if (setarImportosISS)
                {
                    Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS = serCargaISS.BuscarRegraISS(empresa, valorBaseCalculo, cargaPedido.Destino, carga.TipoOperacao, tomador, null, carga?.TipoDeCarga?.NBS ?? "", unitOfWork);

                    if (regraISS != null)
                    {
                        decimal valorBaseCalculoIBSCBSISS = valorBaseCalculoIBSCBS;

                        if (regraISS.IncluirISSBaseCalculo)
                            valorBaseCalculoIBSCBSISS += regraISS.ValorISS;

                        Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = servicoImpostoIBSCBS.ObterImpostoIBSCBS(new Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBS
                        {
                            BaseCalculo = valorBaseCalculoIBSCBSISS,
                            ValoAbaterBaseCalculo = regraISS.ValorISS,
                            CodigoLocalidade = cargaPedido.Pedido.Destinatario.Localidade.Codigo,
                            SiglaUF = cargaPedido.Pedido.Destinatario.Localidade.Estado.Sigla,
                            CodigoTipoOperacao = carga.TipoOperacao?.Codigo ?? 0,
                            Empresa = empresa
                        });

                        cargaPedido.ValorISS = Math.Round(regraISS.ValorISS, 2, MidpointRounding.AwayFromZero);
                        cargaPedido.BaseCalculoISS = regraISS.ValorBaseCalculoISS;
                        cargaPedido.PercentualAliquotaISS = regraISS.AliquotaISS;
                        cargaPedido.PercentualRetencaoISS = regraISS.PercentualRetencaoISS;
                        cargaPedido.IncluirISSBaseCalculo = regraISS.IncluirISSBaseCalculo;
                        cargaPedido.ValorRetencaoISS = Math.Round(regraISS.ValorRetencaoISS, 2, MidpointRounding.AwayFromZero);

                        cargaPedido.ReterIR = regraISS.ReterIR;
                        cargaPedido.AliquotaIR = regraISS.AliquotaIR;
                        cargaPedido.BaseCalculoIR = regraISS.BaseCalculoIR;
                        cargaPedido.ValorIR = Math.Round(regraISS.ValorIR, 2, MidpointRounding.AwayFromZero);
                        impostoIBSCBS.NBS = regraISS.NBS;

                        serCargaPedido.PreencherCamposImpostoIBSCBS(cargaPedido, impostoIBSCBS);

                    }

                    if (cargaPedido.IncluirISSBaseCalculo)
                        cargaPedido.ValorFreteAPagar += cargaPedido.ValorISS - cargaPedido.ValorRetencaoISS;
                }
                else
                {
                    cargaPedido.ValorISS = 0;
                    cargaPedido.BaseCalculoISS = 0;
                    cargaPedido.PercentualAliquotaISS = 0;
                    cargaPedido.PercentualRetencaoISS = 0;
                    cargaPedido.IncluirISSBaseCalculo = false;
                    cargaPedido.ValorRetencaoISS = 0;

                    cargaPedido.ReterIR = false;
                    cargaPedido.AliquotaIR = 0;
                    cargaPedido.BaseCalculoIR = 0;
                    cargaPedido.ValorIR = 0;
                }

                serCargaPedido.VerificarFilialEmissaoCargaPedido(cargaPedido, configuracaoGeralCarga);
            }

            repCargaPedido.Atualizar(cargaPedido);

            if (!calculoImpostosFilialEmissora)
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.CargaValoresAcrescentar cargaValoresAcrescentar = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaValoresAcrescentar()
                {
                    ValorFrete = cargaPedido.ValorFrete,
                    ValorFreteLiquido = 0m,
                    ValorFreteAPagar = cargaPedido.ValorFreteAPagar,
                    ValorICMS = cargaPedido.ValorICMS,
                    ValorPis = cargaPedido.ValorPis,
                    ValorCofins = cargaPedido.ValorCofins,
                    ValorISS = cargaPedido.ValorISS,
                    ValorRetencaoISS = cargaPedido.ValorRetencaoISS,
                    ValorIBSEstadual = cargaPedido.ValorIBSEstadual,
                    ValorIBSMunicipal = cargaPedido.ValorIBSMunicipal,
                    ValorCBS = cargaPedido.ValorCBS
                };
                AcrescentarValoresDaCarga(carga, cargaValoresAcrescentar);
            }
            else
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.CargaValoresAcrescentar cargaValoresAcrescentar = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaValoresAcrescentar()
                {
                    ValorFrete = cargaPedido.ValorFreteFilialEmissora,
                    ValorFreteAPagar = cargaPedido.ValorFreteAPagarFilialEmissora,
                    ValorICMS = cargaPedido.ValorICMSFilialEmissora,
                    ValorIBSEstadual = cargaPedido.ValorIBSEstadualFilialEmissora,
                    ValorIBSMunicipal = cargaPedido.ValorIBSMunicipalFilialEmissora,
                    ValorCBS = cargaPedido.ValorCBSFilialEmissora,
                };

                AcrescentarValoresFilialEmissoraDaCarga(carga, cargaValoresAcrescentar);
            }
        }

        public void AcrescentarValoresFilialEmissoraDaCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Carga.CargaValoresAcrescentar cargaValoresAcrescentar)
        {
            carga.ValorICMSFilialEmissora += cargaValoresAcrescentar.ValorICMS;
            carga.ValorFreteAPagarFilialEmissora += cargaValoresAcrescentar.ValorFreteAPagar;
            carga.ValorFreteFilialEmissora += cargaValoresAcrescentar.ValorFrete;

            carga.ValorIBSEstadualFilialEmissora += cargaValoresAcrescentar.ValorIBSEstadual;
            carga.ValorIBSMunicipalFilialEmissora += cargaValoresAcrescentar.ValorIBSMunicipal;
            carga.ValorCBSFilialEmissora += cargaValoresAcrescentar.ValorCBS;
        }

        public void AcrescentarValoresDaCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Carga.CargaValoresAcrescentar cargaValoresAcrescentar, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = null)
        {
            carga.ValorICMS += cargaValoresAcrescentar.ValorICMS;
            carga.ValorPis += cargaValoresAcrescentar.ValorPis;
            carga.ValorCofins += cargaValoresAcrescentar.ValorCofins;
            carga.ValorFreteAPagar += cargaValoresAcrescentar.ValorFreteAPagar;
            carga.ValorFrete += cargaValoresAcrescentar.ValorFrete;
            carga.ValorFreteLiquido += cargaValoresAcrescentar.ValorFreteLiquido;
            carga.ValorISS += cargaValoresAcrescentar.ValorISS;
            carga.ValorRetencaoISS += cargaValoresAcrescentar.ValorRetencaoISS;
            carga.ValorTotalMoedaPagar += cargaValoresAcrescentar.ValorTotalMoedaPagar;

            carga.ValorIBSEstadual += cargaValoresAcrescentar.ValorIBSEstadual;
            carga.ValorIBSMunicipal += cargaValoresAcrescentar.ValorIBSMunicipal;
            carga.ValorCBS += cargaValoresAcrescentar.ValorCBS;

            Servicos.Log.GravarInfo($"Método: AcrescentarValoresDaCarga -> Carga: {carga.Codigo} - CargaOrigem: {cargaPedido?.CargaOrigem?.Codigo ?? 0} - Valor ISS da Carga: {carga.ValorISS} - Valor ISS da CargaOrigem: {cargaValoresAcrescentar.ValorISS}", "LogCalculoISS");
        }

        public void AcrescentarValoresDaCargaAgrupada(Dominio.Entidades.Embarcador.Cargas.Carga carga, bool filialEmissora, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            if (!carga.CargaAgrupada)
                return;

            Servicos.Log.TratarErro($"Carga: {carga.Codigo} - Inicio método AcrescentarValoresDaCargaAgrupada", "LogCalculoISS");
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasAgrupadas = repCarga.BuscarCargasOriginais(carga.Codigo);
            foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaAgrupada in cargasAgrupadas)
            {
                Servicos.Log.TratarErro($"CargaAgrupada: {cargaAgrupada.Codigo} - CargaAgrupada", "LogCalculoISS");
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosAgrupamento = (from obj in cargaPedidos where obj.CargaOrigem.Codigo == cargaAgrupada.Codigo select obj).ToList();
                if (filialEmissora)
                {
                    cargaAgrupada.ValorICMSFilialEmissora = cargaPedidosAgrupamento.Sum(obj => obj.ValorICMSFilialEmissora);
                    cargaAgrupada.ValorFreteAPagarFilialEmissora = cargaPedidosAgrupamento.Sum(obj => obj.ValorFreteAPagarFilialEmissora);
                    cargaAgrupada.ValorFreteFilialEmissora = cargaPedidosAgrupamento.Sum(obj => obj.ValorFreteFilialEmissora);
                    cargaAgrupada.ValorIBSEstadualFilialEmissora = cargaPedidosAgrupamento.Sum(obj => obj.ValorIBSEstadualFilialEmissora);
                    cargaAgrupada.ValorIBSMunicipalFilialEmissora = cargaPedidosAgrupamento.Sum(obj => obj.ValorIBSMunicipalFilialEmissora);
                    cargaAgrupada.ValorCBSFilialEmissora = cargaPedidosAgrupamento.Sum(obj => obj.ValorCBSFilialEmissora);
                }
                else
                {
                    cargaAgrupada.ValorICMS = cargaPedidosAgrupamento.Sum(obj => obj.ValorICMS);
                    cargaAgrupada.ValorFreteAPagar = cargaPedidosAgrupamento.Sum(obj => obj.ValorFreteAPagar);
                    cargaAgrupada.ValorFrete = cargaPedidosAgrupamento.Sum(obj => obj.ValorFrete);
                    cargaAgrupada.ValorISS = cargaPedidosAgrupamento.Sum(obj => obj.ValorISS);
                    Servicos.Log.TratarErro($"Valor ISS da CargaAGrupada: {cargaPedidosAgrupamento.Sum(obj => obj.ValorISS)}", "LogCalculoISS");
                    cargaAgrupada.ValorRetencaoISS = cargaPedidosAgrupamento.Sum(obj => obj.ValorRetencaoISS);
                    cargaAgrupada.ValorPis = cargaPedidosAgrupamento.Sum(obj => obj.ValorPis);
                    cargaAgrupada.ValorCofins = cargaPedidosAgrupamento.Sum(obj => obj.ValorCofins);
                    cargaAgrupada.ValorIBSEstadual = cargaPedidosAgrupamento.Sum(obj => obj.ValorIBSEstadual);
                    cargaAgrupada.ValorIBSMunicipal = cargaPedidosAgrupamento.Sum(obj => obj.ValorIBSMunicipal);
                    cargaAgrupada.ValorCBS = cargaPedidosAgrupamento.Sum(obj => obj.ValorCBS);
                }

                repCarga.Atualizar(cargaAgrupada);
            }

            Servicos.Log.TratarErro($"Carga: {carga.Codigo} - Fim método AcrescentarValoresDaCargaAgrupada", "LogCalculoISS");
        }

        public void ZerarValoresDaCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, bool filialEmissora, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            if (!filialEmissora)
            {
                carga.ValorFrete = 0m;
                carga.ValorICMS = 0m;
                carga.ValorPis = 0m;
                carga.ValorCofins = 0m;
                carga.ValorISS = 0m;
                carga.ValorRetencaoISS = 0m;
                carga.ValorFreteAPagar = 0m;
                carga.ValorBaseFrete = 0m;
                carga.ValorFreteResidual = 0m;
                carga.ValorTotalMoedaPagar = 0m;
                carga.ValorIBSEstadual = 0m;
                carga.ValorIBSMunicipal = 0m;
                carga.ValorCBS = 0m;

                if (carga.CargaAgrupada)
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem in carga.CargasAgrupamento)
                    {
                        cargaOrigem.ValorFrete = 0m;
                        cargaOrigem.ValorICMS = 0m;
                        cargaOrigem.ValorPis = 0m;
                        cargaOrigem.ValorCofins = 0m;
                        cargaOrigem.ValorISS = 0m;
                        cargaOrigem.ValorRetencaoISS = 0m;
                        cargaOrigem.ValorFreteAPagar = 0m;
                        cargaOrigem.ValorIBSEstadual = 0m;
                        cargaOrigem.ValorIBSMunicipal = 0m;
                        cargaOrigem.ValorCBS = 0m;
                    }

                    repCarga.ZerarValoresCargaAgrupamento(carga.Codigo);
                }
            }
            else
            {
                carga.ValorFreteFilialEmissora = 0m;
                carga.ValorICMSFilialEmissora = 0m;
                carga.ValorFreteAPagarFilialEmissora = 0m;
                carga.ValorIBSEstadualFilialEmissora = 0m;
                carga.ValorIBSMunicipalFilialEmissora = 0m;
                carga.ValorCBSFilialEmissora = 0m;

                if (carga.CargaAgrupada)
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem in carga.CargasAgrupamento)
                    {
                        cargaOrigem.ValorFreteFilialEmissora = 0m;
                        cargaOrigem.ValorICMSFilialEmissora = 0m;
                        cargaOrigem.ValorFreteAPagarFilialEmissora = 0m;
                        cargaOrigem.ValorIBSEstadualFilialEmissora = 0m;
                        cargaOrigem.ValorIBSMunicipalFilialEmissora = 0m;
                        cargaOrigem.ValorCBSFilialEmissora = 0m;
                    }

                    repCarga.ZerarValoresCargaAgrupamentoFilialEmissora(carga.Codigo);
                }
            }
        }

        public void AdicionarValoresDaCargaNotasSemCobertura(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            carga.ValorICMS += cargaPedidos.Sum(obj => obj.ValorICMS);
            carga.ValorFreteAPagar += cargaPedidos.Sum(obj => obj.ValorFreteAPagar);
            carga.ValorFrete += cargaPedidos.Sum(obj => obj.ValorFrete);
            carga.ValorISS += cargaPedidos.Sum(obj => obj.ValorISS);
            carga.ValorRetencaoISS += cargaPedidos.Sum(obj => obj.ValorRetencaoISS);
            carga.ValorPis += cargaPedidos.Sum(obj => obj.ValorPis);
            carga.ValorCofins += cargaPedidos.Sum(obj => obj.ValorCofins);
            carga.ValorBaseFrete += cargaPedidos.Sum(obj => obj.ValorBaseFrete);
            carga.ValorFreteResidual += cargaPedidos.Sum(obj => obj.ValorFreteResidual);
            carga.ValorFreteLiquido += cargaPedidos.Sum(obj => obj.ValorFrete);

            carga.ValorIBSEstadual += cargaPedidos.Sum(obj => obj.ValorIBSEstadual);
            carga.ValorIBSMunicipal += cargaPedidos.Sum(obj => obj.ValorIBSMunicipal);
            carga.ValorCBS += cargaPedidos.Sum(obj => obj.ValorCBS);

            repCarga.Atualizar(carga);
        }

        public string InformarDadosContabeisCargaPedido(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            string retorno = "";
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return retorno;

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao repCargaPedidoContaContabilContabilizacao = new Repositorio.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao(unitOfWork);
            Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado serConfiguracaoCentroResultado = new ConfiguracaoContabil.ConfiguracaoCentroResultado();
            Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil serConfiguracaoContaContabil = new ConfiguracaoContabil.ConfiguracaoContaContabil();

            bool utilizaPEPCentroCusto = carga?.TipoOperacao?.TipoOperacaoUtilizaCentroDeCustoPEP ?? false;
            bool utilizaContaRazao = carga?.TipoOperacao?.TipoOperacaoUtilizaContaRazao ?? false;

            if (!utilizaContaRazao)
            {
                Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil configuracaoContaContabil = serConfiguracaoContaContabil.ObterConfiguracaoContaContabil(cargaPedido.Pedido.Remetente, cargaPedido.Pedido.Destinatario, cargaPedido.ObterTomador(), null, carga.Empresa, carga.TipoOperacao, carga.Rota, cargaPedido.ModeloDocumentoFiscal, null, unitOfWork);

                if ((configuracaoContaContabil != null) && (configuracaoContaContabil.ConfiguracaoContaContabilContabilizacoes?.Count > 0))
                {
                    foreach (Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilContabilizacao configuracaoContabilizacao in configuracaoContaContabil.ConfiguracaoContaContabilContabilizacoes)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao cargaPedidoContaContabilContabilizacao = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao();
                        cargaPedidoContaContabilContabilizacao.CargaPedido = cargaPedido;
                        if (configuracaoContabilizacao.CodigoPlanoConta > 0)
                            cargaPedidoContaContabilContabilizacao.PlanoConta = new Dominio.Entidades.Embarcador.Financeiro.PlanoConta() { Codigo = configuracaoContabilizacao.CodigoPlanoConta };
                        cargaPedidoContaContabilContabilizacao.TipoContabilizacao = configuracaoContabilizacao.TipoContabilizacao;
                        if (configuracaoContabilizacao.CodigoPlanoContaContraPartidaProvisao > 0)
                            cargaPedidoContaContabilContabilizacao.PlanoContaContraPartida = new Dominio.Entidades.Embarcador.Financeiro.PlanoConta() { Codigo = configuracaoContabilizacao.CodigoPlanoContaContraPartidaProvisao };
                        cargaPedidoContaContabilContabilizacao.TipoContaContabil = configuracaoContabilizacao.TipoContaContabil;
                        repCargaPedidoContaContabilContabilizacao.Inserir(cargaPedidoContaContabilContabilizacao);
                    }
                }
                else if ((carga?.TipoOperacao?.InserirDadosContabeisXCampoXTextCTe ?? false) || configuracao.CentroResultadoPedidoObrigatorio)
                {
                    retorno = $"Não foi localizada uma configuração contábil compatível com o pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador}";
                    cargaPedido.Carga.PossuiPendenciaConfiguracaoContabil = true;
                    repCarga.Atualizar(cargaPedido.Carga);
                }
            }

            if (!utilizaPEPCentroCusto)
            {
                bool logisticaReversa = carga.TipoOperacao?.LogisticaReversa ?? false;

                Dominio.Entidades.Cliente destinatario = !logisticaReversa ? cargaPedido.Pedido.Destinatario : cargaPedido.Pedido.Remetente;
                Dominio.Entidades.Cliente remetente = !logisticaReversa ? cargaPedido.Pedido.Remetente : cargaPedido.Pedido.Destinatario;
                Dominio.Entidades.Cliente expedidor = !logisticaReversa ? cargaPedido.Pedido.Expedidor : cargaPedido.Pedido.Recebedor;
                Dominio.Entidades.Cliente recebedor = !logisticaReversa ? cargaPedido.Pedido.Recebedor : cargaPedido.Pedido.Expedidor;
                Dominio.Entidades.Localidade origem = !logisticaReversa ? cargaPedido.Origem : cargaPedido.Destino;
                if (configuracao.ArmazenarCentroCustoDestinatario)
                    destinatario = null;

                Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado configuracaoCentroResultado = serConfiguracaoCentroResultado.ObterConfiguracaoCentroResultado(remetente, destinatario, expedidor, recebedor, cargaPedido.ObterTomador(), null, null, carga.Empresa, carga.TipoOperacao, null, carga.Rota, carga.Filial, origem, unitOfWork);
                if (configuracaoCentroResultado != null)
                {
                    cargaPedido.CentroResultado = configuracaoCentroResultado.CentroResultadoContabilizacao;
                    cargaPedido.ItemServico = configuracaoCentroResultado.ItemServico;
                    cargaPedido.ValorMaximoCentroContabilizacao = configuracaoCentroResultado.ValorMaximoCentroContabilizacao;
                    cargaPedido.CentroResultadoEscrituracao = configuracaoCentroResultado.CentroResultadoEscrituracao;
                    cargaPedido.CentroResultadoICMS = configuracaoCentroResultado.CentroResultadoICMS;
                    cargaPedido.CentroResultadoPIS = configuracaoCentroResultado.CentroResultadoPIS;
                    cargaPedido.CentroResultadoCOFINS = configuracaoCentroResultado.CentroResultadoCOFINS;
                }
                else
                {
                    cargaPedido.CentroResultado = null;
                    cargaPedido.CentroResultadoICMS = null;
                    cargaPedido.CentroResultadoPIS = null;
                    cargaPedido.CentroResultadoCOFINS = null;
                    cargaPedido.ItemServico = "";
                    cargaPedido.CentroResultadoEscrituracao = null;
                    cargaPedido.ValorMaximoCentroContabilizacao = 0;
                    if ((carga.TipoOperacao?.InserirDadosContabeisXCampoXTextCTe ?? false) || configuracao.CentroResultadoPedidoObrigatorio)
                    {
                        retorno = "Não foi localizada uma configuração de centro de resultado compatível com o pedido " + cargaPedido.Pedido.NumeroPedidoEmbarcador;
                        cargaPedido.Carga.PossuiPendenciaConfiguracaoContabil = true;
                        repCarga.Atualizar(cargaPedido.Carga);
                    }

                }

                if (configuracao.ArmazenarCentroCustoDestinatario)
                {
                    destinatario = !logisticaReversa ? cargaPedido.Pedido.Destinatario : cargaPedido.Pedido.Remetente;

                    Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado configuracaoCentroResultadoDestinatario = serConfiguracaoCentroResultado.ObterConfiguracaoCentroResultado(null, destinatario, null, null, cargaPedido.ObterTomador(), null, null, carga.Empresa, carga.TipoOperacao, null, carga.Rota, carga.Filial, origem, unitOfWork);
                    if (configuracaoCentroResultadoDestinatario != null)
                        cargaPedido.CentroResultadoDestinatario = configuracaoCentroResultadoDestinatario.CentroResultadoContabilizacao;
                    else
                    {
                        cargaPedido.CentroResultadoDestinatario = null;
                        if ((carga.TipoOperacao?.InserirDadosContabeisXCampoXTextCTe ?? false) || configuracao.CentroResultadoPedidoObrigatorio)
                        {
                            retorno = "Não foi localizada uma configuração de centro de resultado do destinatário compatível com o pedido " + cargaPedido.Pedido.NumeroPedidoEmbarcador;
                            cargaPedido.Carga.PossuiPendenciaConfiguracaoContabil = true;
                            repCarga.Atualizar(cargaPedido.Carga);
                        }
                    }
                }
            }

            return retorno;
        }

        #endregion

        #region Métodos Privados

        private void RatearFreteEntrePedidosTabelaComissao(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, bool rateioFreteFilialEmissora, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga)
        {
            Servicos.Embarcador.Carga.ComponetesFrete serCargaComponetesFrete = new Servicos.Embarcador.Carga.ComponetesFrete(unitOfWork);
            Servicos.Embarcador.Carga.ICMS serCargaICMS = new Servicos.Embarcador.Carga.ICMS(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
            Repositorio.Embarcador.ICMS.PedagioEstadoBaseCalculo repPedagioEstadoBaseCalculo = new Repositorio.Embarcador.ICMS.PedagioEstadoBaseCalculo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosLiberados = (from obj in cargaPedidos where !obj.PedidoSemNFe select obj).ToList();

            ZerarValoresDaCarga(carga, rateioFreteFilialEmissora, unitOfWork);
            repCargaPedido.ZerarValorFreteSemNota(carga.Codigo);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidosComponentesFreteCarga = repCargaPedidoComponenteFrete.BuscarPorCarga(carga.Codigo, rateioFreteFilialEmissora);
            List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFretesDiretos = new List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();
            List<Dominio.Entidades.Embarcador.ICMS.PedagioEstadoBaseCalculo> pedagioEstadosBaseCalculo = repPedagioEstadoBaseCalculo.BuscarPorEstados((from obj in cargaPedidosLiberados select obj.Origem.Estado.Sigla).Distinct().ToList());
            List<Dominio.ObjetosDeValor.Embarcador.ICMS.TabelaAliquota> tabelaAliquotas = new List<Dominio.ObjetosDeValor.Embarcador.ICMS.TabelaAliquota>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = serCargaICMS.ObterProdutosCargaContidosEmRegras(carga, unitOfWork);
            List<Dominio.Entidades.Cliente> tomadoresFilialEmissora = Servicos.Embarcador.Carga.FilialEmissora.ObterTomadoresFilialEmissora((from obj in cargaPedidos where obj.CargaPedidoFilialEmissora select obj.CargaOrigem.EmpresaFilialEmissora.CNPJ_SemFormato).Distinct().ToList(), unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigem = Servicos.Embarcador.Carga.Carga.ObterCargasOrigem(carga, unitOfWork);

            //if (!rateioFreteFilialEmissora)
            //    VerificarEZerarValorRateioCargaAnterior(carga, unitOfWork);

            bool abriuTransacao = false;
            if (!unitOfWork.IsActiveTransaction())
            {
                unitOfWork.Start();
                abriuTransacao = true;
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidosLiberados)
            {
                Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem = (from obj in cargasOrigem where obj.Codigo == cargaPedido.CargaOrigem.Codigo select obj).FirstOrDefault();

                decimal valorFrete = cargaPedido.ValorFrete;

                if (!rateioFreteFilialEmissora || (configuracao.CalcularFreteFilialEmissoraPorTabelaDeFrete || (carga.TipoOperacao?.CalculaFretePorTabelaFreteFilialEmissora ?? false)) || carga.TipoFreteEscolhido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador || carga.TipoFreteEscolhido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Operador)
                {
                    if (rateioFreteFilialEmissora)
                        valorFrete = cargaPedido.ValorFreteFilialEmissora;
                    else
                        InformarDadosContabeisCargaPedido(cargaPedido, cargaOrigem, configuracao, tipoServicoMultisoftware, unitOfWork);

                    CalcularImpostos(ref carga, cargaOrigem, cargaPedido, valorFrete, rateioFreteFilialEmissora, tipoServicoMultisoftware, unitOfWork, configuracao, cargaPedidosComponentesFreteCarga, pedagioEstadosBaseCalculo, cargaPedidoProdutos, tabelaAliquotas, tomadoresFilialEmissora, configuracaoGeralCarga);

                    cargaPedido.ValorBaseFrete = cargaPedido.ValorFreteAPagar;
                    if (carga.MaiorValorBaseFreteDosPedidos < cargaPedido.ValorBaseFrete)
                        carga.MaiorValorBaseFreteDosPedidos = cargaPedido.ValorBaseFrete;
                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Carga.CargaValoresAcrescentar cargaValoresAcrescentar = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaValoresAcrescentar()
                    {
                        ValorFrete = cargaPedido.ValorFreteFilialEmissora,
                        ValorFreteAPagar = cargaPedido.ValorFreteAPagarFilialEmissora,
                        ValorICMS = cargaPedido.ValorICMSFilialEmissora,
                        ValorIBSEstadual = cargaPedido.ValorIBSEstadualFilialEmissora,
                        ValorIBSMunicipal = cargaPedido.ValorIBSMunicipalFilialEmissora,
                        ValorCBS = cargaPedido.ValorCBSFilialEmissora,
                    };

                    AcrescentarValoresFilialEmissoraDaCarga(carga, cargaValoresAcrescentar);
                }

                //if (cargaPedido.ValorFreteAPagar > 0 && rateioFreteFilialEmissora)
                //    Servicos.Embarcador.Carga.FreteFilialEmissora.SetarFreteEmbarcadorFilialEmissora(ref carga, cargaPedido, tipoServicoMultisoftware, rateioFreteFilialEmissora, unitOfWork);
            }
            //Servicos.Embarcador.Carga.FreteFilialEmissora.VerificarCargaAguardaValorRedespacho(ref carga, unitOfWork);

            if (abriuTransacao)
                unitOfWork.CommitChanges();

            if (!rateioFreteFilialEmissora)
            {
                carga.ValorPis = Math.Round(carga.ValorPis, 2, MidpointRounding.AwayFromZero);
                carga.ValorCofins = Math.Round(carga.ValorCofins, 2, MidpointRounding.AwayFromZero);
                carga.ValorICMS = Math.Round(carga.ValorICMS, 2, MidpointRounding.AwayFromZero);
                carga.ValorISS = Math.Round(carga.ValorISS, 2, MidpointRounding.AwayFromZero);
                carga.ValorRetencaoISS = Math.Round(carga.ValorRetencaoISS, 2, MidpointRounding.AwayFromZero);
                carga.ValorFreteAPagar = Math.Round(carga.ValorFreteAPagar, 2, MidpointRounding.AwayFromZero);
                carga.ValorFrete = Math.Round(carga.ValorFrete, 2, MidpointRounding.AwayFromZero);
                carga.ValorFreteLiquido = carga.ValorFrete;

                carga.ValorIBSEstadual = Math.Round(carga.ValorIBSEstadual, 3, MidpointRounding.AwayFromZero);
                carga.ValorIBSMunicipal = Math.Round(carga.ValorIBSMunicipal, 3, MidpointRounding.AwayFromZero);
                carga.ValorCBS = Math.Round(carga.ValorCBS, 3, MidpointRounding.AwayFromZero);
            }
            else
            {
                carga.ValorICMSFilialEmissora = Math.Round(carga.ValorICMSFilialEmissora, 2, MidpointRounding.AwayFromZero);
                carga.ValorFreteAPagarFilialEmissora = Math.Round(carga.ValorFreteAPagarFilialEmissora, 2, MidpointRounding.AwayFromZero);
                carga.ValorFreteFilialEmissora = Math.Round(carga.ValorFreteFilialEmissora, 2, MidpointRounding.AwayFromZero);

                carga.ValorIBSEstadualFilialEmissora = Math.Round(carga.ValorIBSEstadualFilialEmissora, 3, MidpointRounding.AwayFromZero);
                carga.ValorIBSMunicipalFilialEmissora = Math.Round(carga.ValorIBSMunicipalFilialEmissora, 3, MidpointRounding.AwayFromZero);
                carga.ValorCBSFilialEmissora = Math.Round(carga.ValorCBSFilialEmissora, 3, MidpointRounding.AwayFromZero);
            }

            serCargaComponetesFrete.AdicionarComponentesCargaAgrupada(carga, rateioFreteFilialEmissora, cargaPedidosComponentesFreteCarga, unitOfWork);
            AcrescentarValoresDaCargaAgrupada(carga, rateioFreteFilialEmissora, cargaPedidos, unitOfWork);

            if (carga.ExigeNotaFiscalParaCalcularFrete)
            {
                RateioNotaFiscal serRateioNotaFiscal = new RateioNotaFiscal(unitOfWork);
                serRateioNotaFiscal.RatearFreteCargaPedidoEntreNotas(carga, cargaPedidos, cargaPedidosComponentesFreteCarga, rateioFreteFilialEmissora, tipoServicoMultisoftware, unitOfWork, configuracao);
            }
        }

        private List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> retornarPedidosPorDestino(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> pedidosCarga, Dominio.Entidades.Embarcador.Cargas.CargaPercurso cargaPercurso)
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosDestino = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoDestino in pedidosCarga)
            {
                if (cargaPedidoDestino.Pedido.TipoOperacaoEmissao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao.VendaComRedespacho)
                {
                    if (cargaPedidoDestino.Recebedor.Localidade.Codigo == cargaPercurso.Destino.Codigo)
                        cargaPedidosDestino.Add(cargaPedidoDestino);
                }
                else
                {
                    if (cargaPedidoDestino.Destino.Codigo == cargaPercurso.Destino.Codigo)
                        cargaPedidosDestino.Add(cargaPedidoDestino);
                }
            }

            return cargaPedidosDestino;
        }

        private Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS BuscarRegraICMSPeloEmbarcador(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Cliente remetente, Dominio.Entidades.Cliente destinatario, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino, ref bool incluiICMSBaseCalculo, ref decimal percentualICMSIncluirNoFrete, decimal valorParaBaseDeCalculo, Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Carga.ICMS serCargaICMS = new Servicos.Embarcador.Carga.ICMS(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS();
            Servicos.Embarcador.Imposto.ImpostoPisCofins servicoPisCofins = new Servicos.Embarcador.Imposto.ImpostoPisCofins();

            if (cargaPedido.ModeloDocumentoFiscal == null || cargaPedido.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
            {
                regraICMS.CFOP = cargaPedido.CFOP?.CodigoCFOP ?? 0;
                regraICMS.CST = cargaPedido.CST;
            }

            regraICMS.AliquotaCofins = cargaPedido.AliquotaCofins;
            regraICMS.AliquotaPis = cargaPedido.AliquotaPis;
            regraICMS.Aliquota = cargaPedido.PercentualAliquota;
            regraICMS.AliquotaInternaDifal = cargaPedido.PercentualAliquotaInternaDifal;
            regraICMS.PercentualReducaoBC = cargaPedido.PercentualReducaoBC;
            regraICMS.ObservacaoCTe = cargaPedido.ObservacaoRegraICMSCTe;
            regraICMS.PercentualInclusaoBC = cargaPedido.PercentualIncluirBaseCalculo;
            regraICMS.IncluirICMSBC = cargaPedido.IncluirICMSBaseCalculo;
            regraICMS.DescontarICMSDoValorAReceber = cargaPedido.DescontarICMSDoValorAReceber;
            regraICMS.NaoReduzirRetencaoICMSDoValorDaPrestacao = cargaPedido.NaoReduzirRetencaoICMSDoValorDaPrestacao;
            regraICMS.NaoEnviarImpostoICMSNaEmissaoCte = cargaPedido.NaoEnviarImpostoICMSNaEmissaoCte;
            regraICMS.NaoImprimirImpostosDACTE = cargaPedido.NaoImprimirImpostosDACTE;

            decimal aliquotaPisCofins = regraICMS.AliquotaPis + regraICMS.AliquotaCofins;
            if (aliquotaPisCofins > 0)
                regraICMS.IncluirPisCofinsBC = true;

            regraICMS.ValorBaseCalculoPISCOFINS = valorParaBaseDeCalculo;

            regraICMS.ValorICMSIncluso = serCargaICMS.CalcularICMSInclusoNoFrete(regraICMS.CST, ref valorParaBaseDeCalculo, regraICMS.Aliquota, percentualICMSIncluirNoFrete, regraICMS.PercentualReducaoBC, incluiICMSBaseCalculo, regraICMS.AliquotaInternaDifal, aliquotaPisCofins);
            regraICMS.ValorICMS = serCargaICMS.CalcularInclusaoICMSNoFrete(regraICMS.CST, ref valorParaBaseDeCalculo, regraICMS.Aliquota, percentualICMSIncluirNoFrete, regraICMS.PercentualReducaoBC, incluiICMSBaseCalculo, aliquotaPisCofins);
            regraICMS.ValorPis = servicoPisCofins.CalcularValorPis(regraICMS.AliquotaPis, regraICMS.ValorBaseCalculoPISCOFINS);
            regraICMS.ValorCofins = servicoPisCofins.CalcularValorCofins(regraICMS.AliquotaCofins, regraICMS.ValorBaseCalculoPISCOFINS);

            regraICMS.ValorBaseCalculoICMS = valorParaBaseDeCalculo;

            return regraICMS;
        }

        private Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS BuscarRegraICMSComplementoFilialEmissora(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Cliente remetente, Dominio.Entidades.Cliente destinatario, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino, ref bool incluiICMSBaseCalculo, ref decimal percentualICMSIncluirNoFrete, decimal valorParaBaseDeCalculo, Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Carga.ICMS serCargaICMS = new Servicos.Embarcador.Carga.ICMS(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS();

            if (cargaPedido.ModeloDocumentoFiscal == null || cargaPedido.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
            {
                regraICMS.CFOP = cargaPedido.CFOPFilialEmissora?.CodigoCFOP ?? 0;
                regraICMS.ObjetoCFOP = cargaPedido.CFOPFilialEmissora;
                regraICMS.CST = cargaPedido.CSTFilialEmissora;
            }

            regraICMS.Aliquota = cargaPedido.PercentualAliquotaFilialEmissora;
            regraICMS.AliquotaInternaDifal = cargaPedido.PercentualAliquotaFilialEmissoraInternaDifal;
            regraICMS.PercentualReducaoBC = cargaPedido.PercentualReducaoBCFilialEmissora;
            regraICMS.PercentualInclusaoBC = cargaPedido.PercentualIncluirBaseCalculoFilialEmissora;
            regraICMS.IncluirICMSBC = cargaPedido.IncluirICMSBaseCalculoFilialEmissora;

            regraICMS.ValorICMSIncluso = serCargaICMS.CalcularICMSInclusoNoFrete(regraICMS.CST, ref valorParaBaseDeCalculo, regraICMS.Aliquota, percentualICMSIncluirNoFrete, regraICMS.PercentualReducaoBC, incluiICMSBaseCalculo, regraICMS.AliquotaInternaDifal);
            regraICMS.ValorICMS = serCargaICMS.CalcularInclusaoICMSNoFrete(regraICMS.CST, ref valorParaBaseDeCalculo, regraICMS.Aliquota, percentualICMSIncluirNoFrete, regraICMS.PercentualReducaoBC, incluiICMSBaseCalculo);

            regraICMS.ValorBaseCalculoICMS = valorParaBaseDeCalculo;
            regraICMS.ValorBaseCalculoPISCOFINS = valorParaBaseDeCalculo;

            return regraICMS;
        }

        private void SetarObservacaoPedido(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork)
        {
            string observacaoCTe = string.Empty;
            string observacaoCTeTerceiro = string.Empty;

            if (carga.TipoOperacao?.UsarConfiguracaoEmissao ?? false)
            {
                observacaoCTe = carga.TipoOperacao.ObservacaoCTe;
                observacaoCTeTerceiro = carga.TipoOperacao.ObservacaoCTeTerceiro;
            }
            else
            {
                Dominio.Entidades.Cliente tomador = cargaPedido?.ObterTomador();

                if (tomador != null)
                {
                    if (tomador.NaoUsarConfiguracaoEmissaoGrupo)
                    {
                        observacaoCTe = tomador.ObservacaoCTe;
                        observacaoCTeTerceiro = tomador.ObservacaoCTeTerceiro;
                    }
                    else if (tomador.GrupoPessoas != null)
                    {
                        observacaoCTe = tomador.GrupoPessoas.ObservacaoCTe;
                        observacaoCTeTerceiro = tomador.GrupoPessoas.ObservacaoCTeTerceiro;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(observacaoCTe) || !string.IsNullOrWhiteSpace(observacaoCTeTerceiro))
            {
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

                if (!string.IsNullOrWhiteSpace(observacaoCTe))
                {
                    if (string.IsNullOrWhiteSpace(cargaPedido.Pedido.ObservacaoCTe))
                        cargaPedido.Pedido.ObservacaoCTe = observacaoCTe;
                    else if (!cargaPedido.Pedido.ObservacaoCTe.ToLower().Contains(observacaoCTe.ToLower()))
                        cargaPedido.Pedido.ObservacaoCTe += " / " + observacaoCTe;
                }

                if (!string.IsNullOrWhiteSpace(observacaoCTeTerceiro))
                {
                    if (string.IsNullOrWhiteSpace(cargaPedido.Pedido.ObservacaoCTeTerceiro))
                        cargaPedido.Pedido.ObservacaoCTeTerceiro = observacaoCTeTerceiro;
                    else if (!cargaPedido.Pedido.ObservacaoCTeTerceiro.ToLower().Contains(observacaoCTeTerceiro.ToLower()))
                        cargaPedido.Pedido.ObservacaoCTeTerceiro += " / " + observacaoCTeTerceiro;
                }

                cargaPedido.Pedido.ObservacaoCTeTerceiro = Utilidades.String.Left(cargaPedido.Pedido.ObservacaoCTeTerceiro, 2000);
                cargaPedido.Pedido.ObservacaoCTe = Utilidades.String.Left(cargaPedido.Pedido.ObservacaoCTe, 2000);

                repPedido.Atualizar(cargaPedido.Pedido);
            }
        }

        private void ProcessarValoresComponenteICMSTabelaFrete(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, decimal aliquotaPisCofins)
        {
            if (carga.TabelaFrete?.DescontarDoValorAReceberOICMSDoComponente ?? false)
            {
                Servicos.Embarcador.Carga.ICMS servicoIcms = new Servicos.Embarcador.Carga.ICMS(_unitOfWork);

                decimal valorComponente = ObterValorComponenteFrete(carga.Componentes, carga.TabelaFrete.ComponenteFreteDestacar.Codigo);
                decimal valorCalculadoIcmsComponente = servicoIcms.CalcularICMSInclusoNoFrete(cargaPedido.CST, ref valorComponente, cargaPedido.PercentualAliquota, 0, cargaPedido.PercentualReducaoBC, cargaPedido.IncluirICMSBaseCalculo, cargaPedido.PercentualAliquotaInternaDifal, aliquotaPisCofins, false);

                cargaPedido.ValorFreteAPagar -= valorCalculadoIcmsComponente;
                carga.TabelaFrete.ValorICMSComponenteDestacado = valorCalculadoIcmsComponente;
                Servicos.Log.GravarInfo($"CargaPedido: {cargaPedido.Codigo} -> Valor ICMS calculado do componente descontado do valor a receber: {valorCalculadoIcmsComponente}", "ValoresComponenteICMSTabelaFrete");
            }
        }

        private decimal ObterValorComponenteFrete(IList<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> componentes, int codigoComponente)
        {
            return componentes
                .Where(x => x.ComponenteFrete?.Codigo == codigoComponente)
                .Select(x => (decimal?)x.ValorComponente)
                .FirstOrDefault() ?? 0m;
        }

        public decimal ObterValorArredondadoIncidenciaISS(decimal numero)
        {
            if (numero == 0)
                return numero;

            return (((int)(numero * 1000) % 10) > 5) ? numero.RoundUp(2) : numero.RoundDown(2);
        }

        private decimal ObterValorComponenteDestacado(List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> componentes, int codigoComponenteDestaque)
        {
            return componentes
                .Where(x => x.ComponenteFrete?.Codigo == codigoComponenteDestaque)
                .Sum(obj => obj.ValorComponente);
        }

        #endregion
    }
}