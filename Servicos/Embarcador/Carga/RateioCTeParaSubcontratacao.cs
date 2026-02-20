using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Servicos.Embarcador.Carga
{
    public class RateioCTeParaSubcontratacao : ServicoBase
    {

        public RateioCTeParaSubcontratacao(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }
        #region Métodos Públicos

        private List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoContaContabilContabilizacao> InformarDadosContabeisCTeSubContratacao(Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTesParaSubContratacao, bool emissaoPorCTe, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao> cargaPedidoContaContabilContabilizacaosCarga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoContaContabilContabilizacao> pedidosCTeParaSubContratacaoContaContabilContabilizacao = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoContaContabilContabilizacao>();
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return pedidosCTeParaSubContratacaoContaContabilContabilizacao;


            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoContaContabilContabilizacao repPedidoCTeParaSubContratacaoContaContabilContabilizacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoContaContabilContabilizacao(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            if (emissaoPorCTe)
            {
                Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado serConfiguracaoCentroResultado = new ConfiguracaoContabil.ConfiguracaoCentroResultado();
                Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil serConfiguracaoContaContabil = new ConfiguracaoContabil.ConfiguracaoContaContabil();

                Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaPedido.CargaOrigem;

                Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil configuracaoContaContabil = serConfiguracaoContaContabil.ObterConfiguracaoContaContabil(pedidoCTesParaSubContratacao.CTeTerceiro.Remetente.Cliente, pedidoCTesParaSubContratacao.CTeTerceiro.Destinatario.Cliente, cargaPedido.ObterTomador(), null, carga.Empresa, carga.TipoOperacao, carga.Rota, cargaPedido.ModeloDocumentoFiscal, null, unitOfWork);

                if ((configuracaoContaContabil != null) && (configuracaoContaContabil.ConfiguracaoContaContabilContabilizacoes?.Count > 0))
                {
                    foreach (Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilContabilizacao configuracaoContabilizacao in configuracaoContaContabil.ConfiguracaoContaContabilContabilizacoes)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoContaContabilContabilizacao pedidoCTeParaSubContratacaoContaContabilContabilizacao = new Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoContaContabilContabilizacao();

                        pedidoCTeParaSubContratacaoContaContabilContabilizacao.PedidoCTeParaSubContratacao = pedidoCTesParaSubContratacao;
                        if (configuracaoContabilizacao.CodigoPlanoConta > 0)
                            pedidoCTeParaSubContratacaoContaContabilContabilizacao.PlanoConta = new Dominio.Entidades.Embarcador.Financeiro.PlanoConta() { Codigo = configuracaoContabilizacao.CodigoPlanoConta };
                        pedidoCTeParaSubContratacaoContaContabilContabilizacao.TipoContabilizacao = configuracaoContabilizacao.TipoContabilizacao;
                        if (configuracaoContabilizacao.CodigoPlanoContaContraPartidaProvisao > 0)
                            pedidoCTeParaSubContratacaoContaContabilContabilizacao.PlanoContaContraPartida = new Dominio.Entidades.Embarcador.Financeiro.PlanoConta() { Codigo = configuracaoContabilizacao.CodigoPlanoContaContraPartidaProvisao };
                        pedidoCTeParaSubContratacaoContaContabilContabilizacao.TipoContaContabil = configuracaoContabilizacao.TipoContaContabil;
                        repPedidoCTeParaSubContratacaoContaContabilContabilizacao.Inserir(pedidoCTeParaSubContratacaoContaContabilContabilizacao);
                        pedidosCTeParaSubContratacaoContaContabilContabilizacao.Add(pedidoCTeParaSubContratacaoContaContabilContabilizacao);
                    }
                }
                else if ((carga.TipoOperacao?.InserirDadosContabeisXCampoXTextCTe ?? false) || configuracao.CentroResultadoPedidoObrigatorio)
                {
                    cargaPedido.Carga.PossuiPendenciaConfiguracaoContabil = true;
                    repCarga.Atualizar(cargaPedido.Carga);
                }

                bool logisticaReversa = carga.TipoOperacao?.LogisticaReversa ?? false;

                Dominio.Entidades.Cliente destinatario = logisticaReversa ? pedidoCTesParaSubContratacao.CTeTerceiro.Destinatario.Cliente : pedidoCTesParaSubContratacao.CTeTerceiro.Remetente.Cliente;
                Dominio.Entidades.Cliente remetente = logisticaReversa ? pedidoCTesParaSubContratacao.CTeTerceiro.Remetente.Cliente : pedidoCTesParaSubContratacao.CTeTerceiro.Destinatario.Cliente;
                Dominio.Entidades.Cliente expedidor = logisticaReversa ? pedidoCTesParaSubContratacao.CTeTerceiro.Recebedor?.Cliente : pedidoCTesParaSubContratacao.CTeTerceiro.Expedidor?.Cliente;
                Dominio.Entidades.Cliente recebedor = logisticaReversa ? pedidoCTesParaSubContratacao.CTeTerceiro.Expedidor?.Cliente : pedidoCTesParaSubContratacao.CTeTerceiro.Recebedor?.Cliente;
                Dominio.Entidades.Localidade origem = logisticaReversa ? pedidoCTesParaSubContratacao.CTeTerceiro.LocalidadeInicioPrestacao : pedidoCTesParaSubContratacao.CTeTerceiro.LocalidadeTerminoPrestacao;
                if (configuracao.ArmazenarCentroCustoDestinatario)
                    destinatario = null;

                Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado configuracaoCentroResultado = serConfiguracaoCentroResultado.ObterConfiguracaoCentroResultado(remetente, destinatario, expedidor, recebedor, cargaPedido.ObterTomador(), null, null, carga.Empresa, carga.TipoOperacao, null, carga.Rota, carga.Filial, origem, unitOfWork);
                if (configuracaoCentroResultado != null)
                {
                    pedidoCTesParaSubContratacao.CentroResultado = configuracaoCentroResultado.CentroResultadoContabilizacao;
                    pedidoCTesParaSubContratacao.ItemServico = configuracaoCentroResultado.ItemServico;
                    pedidoCTesParaSubContratacao.ValorMaximoCentroContabilizacao = configuracaoCentroResultado.ValorMaximoCentroContabilizacao;
                    pedidoCTesParaSubContratacao.CentroResultadoEscrituracao = configuracaoCentroResultado.CentroResultadoEscrituracao;
                    pedidoCTesParaSubContratacao.CentroResultadoICMS = configuracaoCentroResultado.CentroResultadoICMS;
                    pedidoCTesParaSubContratacao.CentroResultadoPIS = configuracaoCentroResultado.CentroResultadoPIS;
                    pedidoCTesParaSubContratacao.CentroResultadoCOFINS = configuracaoCentroResultado.CentroResultadoCOFINS;
                }
                else
                {
                    pedidoCTesParaSubContratacao.CentroResultado = null;
                    pedidoCTesParaSubContratacao.ItemServico = "";
                    pedidoCTesParaSubContratacao.CentroResultadoEscrituracao = null;
                    pedidoCTesParaSubContratacao.CentroResultadoICMS = null;
                    pedidoCTesParaSubContratacao.CentroResultadoPIS = null;
                    pedidoCTesParaSubContratacao.CentroResultadoCOFINS = null;
                    pedidoCTesParaSubContratacao.ValorMaximoCentroContabilizacao = 0;
                    if ((carga.TipoOperacao?.InserirDadosContabeisXCampoXTextCTe ?? false) || configuracao.CentroResultadoPedidoObrigatorio)
                    {
                        cargaPedido.Carga.PossuiPendenciaConfiguracaoContabil = true;
                        repCarga.Atualizar(cargaPedido.Carga);
                    }
                }


                if (configuracao.ArmazenarCentroCustoDestinatario)
                {
                    destinatario = logisticaReversa ? pedidoCTesParaSubContratacao.CTeTerceiro.Destinatario.Cliente : pedidoCTesParaSubContratacao.CTeTerceiro.Remetente.Cliente;

                    Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado configuracaoCentroResultadoDestinatario = serConfiguracaoCentroResultado.ObterConfiguracaoCentroResultado(null, destinatario, null, null, cargaPedido.ObterTomador(), null, null, carga.Empresa, carga.TipoOperacao, null, carga.Rota, carga.Filial, origem, unitOfWork);
                    if (configuracaoCentroResultadoDestinatario != null)
                        pedidoCTesParaSubContratacao.CentroResultadoDestinatario = configuracaoCentroResultadoDestinatario.CentroResultadoContabilizacao;
                    else
                    {
                        pedidoCTesParaSubContratacao.CentroResultadoDestinatario = null;
                        if ((carga.TipoOperacao?.InserirDadosContabeisXCampoXTextCTe ?? false) || configuracao.CentroResultadoPedidoObrigatorio)
                        {
                            //retorno = "Não foi localizada uma configuração de centro de resultado de destino compatível com o ct-e anterior " + pedidoCTesParaSubContratacao.CTeTerceiro.Numero.ToString();
                            cargaPedido.Carga.PossuiPendenciaConfiguracaoContabil = true;
                            repCarga.Atualizar(cargaPedido.Carga);
                        }
                    }

                }
            }
            else
            {
                pedidoCTesParaSubContratacao.CentroResultado = cargaPedido.CentroResultado;
                pedidoCTesParaSubContratacao.ItemServico = cargaPedido.ItemServico;
                pedidoCTesParaSubContratacao.CentroResultadoEscrituracao = cargaPedido.CentroResultadoEscrituracao;
                pedidoCTesParaSubContratacao.CentroResultadoICMS = cargaPedido.CentroResultadoICMS;
                pedidoCTesParaSubContratacao.CentroResultadoPIS = cargaPedido.CentroResultadoPIS;
                pedidoCTesParaSubContratacao.CentroResultadoCOFINS = cargaPedido.CentroResultadoCOFINS;
                pedidoCTesParaSubContratacao.ValorMaximoCentroContabilizacao = cargaPedido.ValorMaximoCentroContabilizacao;
                pedidoCTesParaSubContratacao.CentroResultadoDestinatario = cargaPedido.CentroResultadoDestinatario;
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao> cargaPedidoContaContabilContabilizacaos = (from obj in cargaPedidoContaContabilContabilizacaosCarga where obj.CargaPedido.Codigo == cargaPedido.Codigo select obj).ToList();
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao cargaPedidoContaContabilContabilizacao in cargaPedidoContaContabilContabilizacaos)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoContaContabilContabilizacao pedidoCTeParaSubContratacaoContaContabilContabilizacao = new Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoContaContabilContabilizacao();

                    pedidoCTeParaSubContratacaoContaContabilContabilizacao.PedidoCTeParaSubContratacao = pedidoCTesParaSubContratacao;
                    pedidoCTeParaSubContratacaoContaContabilContabilizacao.PlanoConta = cargaPedidoContaContabilContabilizacao.PlanoConta;
                    pedidoCTeParaSubContratacaoContaContabilContabilizacao.TipoContabilizacao = cargaPedidoContaContabilContabilizacao.TipoContabilizacao;
                    pedidoCTeParaSubContratacaoContaContabilContabilizacao.PlanoContaContraPartida = cargaPedidoContaContabilContabilizacao.PlanoContaContraPartida;
                    pedidoCTeParaSubContratacaoContaContabilContabilizacao.TipoContaContabil = cargaPedidoContaContabilContabilizacao.TipoContaContabil;
                    repPedidoCTeParaSubContratacaoContaContabilContabilizacao.Inserir(pedidoCTeParaSubContratacaoContaContabilContabilizacao);
                    pedidosCTeParaSubContratacaoContaContabilContabilizacao.Add(pedidoCTeParaSubContratacaoContaContabilContabilizacao);
                }
            }
            return pedidosCTeParaSubContratacaoContaContabilContabilizacao;
        }

        public void RatearFreteCargaCTesParaSubcontratacao(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                Repositorio.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao repCargaPedidoContaContabilContabilizacao = new Repositorio.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal repCTeParaSubContratacaoPedidoNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete repPedidoXMLNotaFiscalComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubcontratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao> cargaPedidoContaContabilContabilizacaos = repCargaPedidoContaContabilContabilizacao.BuscarPorCarga(carga.Codigo);
                Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteICMS = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS);
                Dominio.Entidades.Embarcador.Frete.ComponenteFrete componentePisCofins = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> pedidoCTeParaSubContratacaoNotasFiscais = repCTeParaSubContratacaoPedidoNotaFiscal.BuscarPorCarga(carga.Codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> cargaCTesParaSubContratacao = repPedidoCTeParaSubcontratacao.BuscarPorCarga(carga.Codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> componentesICMSXMLNotaFiscalExistenteCarga = repPedidoXMLNotaFiscalComponenteFrete.BuscarPorCargaETipo(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS, null, false);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> componentesPisConfisXMLNotaFiscalExistenteCarga = repPedidoXMLNotaFiscalComponenteFrete.BuscarPorCargaETipo(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS, null, false);


                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in carga.Pedidos)
                {
                    if (cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SVMTerceiro
                        || cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada
                        || cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho
                        || cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SVMProprio
                        || cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario)
                    {
                        Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponentesFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);

                        List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> componentes = new List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico>();
                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesFrete = repCargaPedidoComponentesFrete.BuscarPorCargaPedido(cargaPedido.Codigo, false, null, false);

                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponenteFrete in cargaPedidoComponentesFrete)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico componenteFreteDinamico = cargaPedidoComponenteFrete.ConvertarParaComponenteDinamico();
                            componentes.Add(componenteFreteDinamico);
                        }

                        bool cteAnteriorFilialEmissora = false;
                        RatearValorCTeSucontratacaoDoPedido(carga, cargaPedido, cargaPedido.ValorFrete, cargaPedido.ValorTotalMoeda ?? 0m, componentes, cargaPedido.FormulaRateio, cteAnteriorFilialEmissora, cargaPedidoContaContabilContabilizacaos, tipoServicoMultisoftware, unitOfWork, componenteICMS, componentePisCofins, pedidoCTeParaSubContratacaoNotasFiscais, cargaCTesParaSubContratacao, componentesICMSXMLNotaFiscalExistenteCarga, componentesPisConfisXMLNotaFiscalExistenteCarga);
                    }
                }
                AjustarValoresImpostosEntrePedidos(carga, tipoServicoMultisoftware, unitOfWork);
            }
        }

        public void AjustarValoresImpostosEntrePedidos(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            if (configuracaoTMS.UtilizaEmissaoMultimodal)
            {
                if (carga == null || carga.Pedidos == null || carga.Pedidos.Count == 0)
                    return;

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidoCTesParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarPorCarga(carga.Codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorCarga(carga.Codigo);
                if (pedidoCTesParaSubContratacao == null || pedidoCTesParaSubContratacao.Count == 0 && (pedidosXMLNotaFiscal == null || pedidosXMLNotaFiscal.Count == 0))
                    return;

                decimal totalFretePagar = Math.Round(carga.Pedidos.Sum(o => o.ValorFreteAPagar), 2, MidpointRounding.AwayFromZero);
                decimal totalBaseICMS = Math.Round(carga.Pedidos.Sum(o => o.BaseCalculoICMS), 2, MidpointRounding.AwayFromZero);
                decimal aliquotaICMS = Math.Round(carga.Pedidos.Select(o => o.PercentualAliquota).FirstOrDefault(), 2, MidpointRounding.AwayFromZero);
                decimal aliquotaICMSInternaDifal = Math.Round(carga.Pedidos.Select(o => o.PercentualAliquotaInternaDifal).FirstOrDefault(), 2, MidpointRounding.AwayFromZero);
                decimal totalICMSIncluso = Math.Round(carga.Pedidos.Sum(o => o.ValorICMSIncluso), 2, MidpointRounding.AwayFromZero);
                string cst = carga.Pedidos.FirstOrDefault().CST;

                if (totalBaseICMS <= 0 || aliquotaICMS <= 0 || cst == "060" || cst == "60")
                    return;

                totalBaseICMS = Math.Round(carga.Pedidos.Sum(o => o.ValorFreteAPagar), 2, MidpointRounding.AwayFromZero);
                decimal totalICMS = Math.Round((totalBaseICMS * (aliquotaICMS / 100)), 2, MidpointRounding.AwayFromZero);
                decimal totalPis = Math.Round(carga.Pedidos.Sum(o => o.ValorPis), 2, MidpointRounding.AwayFromZero);
                decimal totalCofins = Math.Round(carga.Pedidos.Sum(o => o.ValorCofins), 2, MidpointRounding.AwayFromZero);

                decimal baseICMSPedidos = 0;
                decimal icmsPedidos = 0;
                decimal icmsInclusoPedidos = 0;
                decimal valorFretePagar = 0;
                decimal pisPedidos = 0;
                decimal cofinsPedidos = 0;
                decimal percentualReducaoICMS = 0;

                if (pedidoCTesParaSubContratacao != null && pedidoCTesParaSubContratacao.Count > 0)
                {
                    baseICMSPedidos = Math.Round(pedidoCTesParaSubContratacao.Sum(o => o.BaseCalculoICMS), 2, MidpointRounding.AwayFromZero);
                    icmsPedidos = Math.Round(pedidoCTesParaSubContratacao.Sum(o => o.ValorICMS), 2, MidpointRounding.AwayFromZero);
                    icmsInclusoPedidos = Math.Round(pedidoCTesParaSubContratacao.Sum(o => o.ValorICMSIncluso), 2, MidpointRounding.AwayFromZero);
                    valorFretePagar = Math.Round(pedidoCTesParaSubContratacao.Sum(o => o.ValorFrete), 2, MidpointRounding.AwayFromZero);
                    pisPedidos = Math.Round(pedidoCTesParaSubContratacao.Sum(o => o.ValorPis), 2, MidpointRounding.AwayFromZero);
                    cofinsPedidos = Math.Round(pedidoCTesParaSubContratacao.Sum(o => o.ValorCofins), 2, MidpointRounding.AwayFromZero);
                    percentualReducaoICMS = Math.Round(pedidoCTesParaSubContratacao.Sum(o => o.PercentualReducaoBC), 2, MidpointRounding.AwayFromZero);
                }
                else if (pedidosXMLNotaFiscal != null && pedidosXMLNotaFiscal.Count > 0)
                {
                    baseICMSPedidos = Math.Round(pedidosXMLNotaFiscal.Sum(o => o.BaseCalculoICMS), 2, MidpointRounding.AwayFromZero);
                    icmsPedidos = Math.Round(pedidosXMLNotaFiscal.Sum(o => o.ValorICMS), 2, MidpointRounding.AwayFromZero);
                    icmsInclusoPedidos = Math.Round(pedidosXMLNotaFiscal.Sum(o => o.ValorICMSIncluso), 2, MidpointRounding.AwayFromZero);
                    valorFretePagar = Math.Round(pedidosXMLNotaFiscal.Sum(o => o.ValorFrete), 2, MidpointRounding.AwayFromZero);
                    pisPedidos = Math.Round(pedidosXMLNotaFiscal.Sum(o => o.ValorPis), 2, MidpointRounding.AwayFromZero);
                    cofinsPedidos = Math.Round(pedidosXMLNotaFiscal.Sum(o => o.ValorCofins), 2, MidpointRounding.AwayFromZero);
                    percentualReducaoICMS = Math.Round(pedidosXMLNotaFiscal.Sum(o => o.PercentualReducaoBC), 2, MidpointRounding.AwayFromZero);
                }

                if (percentualReducaoICMS > 0)
                    return;

                decimal diferencaBaseICMS = Math.Round(totalBaseICMS - baseICMSPedidos, 2, MidpointRounding.AwayFromZero);
                decimal diferencaICMS = Math.Round(totalICMS - icmsPedidos, 2, MidpointRounding.AwayFromZero);
                decimal diferencaICMSIncluso = Math.Round(totalICMSIncluso - icmsInclusoPedidos, 2, MidpointRounding.AwayFromZero);
                decimal diferencaPis = Math.Round(totalPis - pisPedidos, 2, MidpointRounding.AwayFromZero);
                decimal diferencaCofins = Math.Round(totalCofins - cofinsPedidos, 2, MidpointRounding.AwayFromZero);

                if (pedidoCTesParaSubContratacao != null && pedidoCTesParaSubContratacao.Count > 0)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao primeiroPedidoCTesParaSubContratacao = pedidoCTesParaSubContratacao.FirstOrDefault();

                    if (diferencaBaseICMS != 0 && primeiroPedidoCTesParaSubContratacao.BaseCalculoICMS > 0)
                        primeiroPedidoCTesParaSubContratacao.BaseCalculoICMS += diferencaBaseICMS;
                    if (diferencaICMS != 0 && diferencaBaseICMS != 0 && primeiroPedidoCTesParaSubContratacao.ValorICMS > 0)
                        primeiroPedidoCTesParaSubContratacao.ValorICMS += diferencaICMS;
                    if (diferencaBaseICMS != 0 && diferencaICMSIncluso != 0 && primeiroPedidoCTesParaSubContratacao.ValorICMSIncluso > 0 && totalICMSIncluso > 0)
                        primeiroPedidoCTesParaSubContratacao.ValorICMSIncluso += diferencaICMSIncluso;
                    if (diferencaPis != 0 && diferencaBaseICMS != 0 && primeiroPedidoCTesParaSubContratacao.ValorPis > 0)
                        primeiroPedidoCTesParaSubContratacao.ValorPis += diferencaPis;
                    if (diferencaCofins != 0 && diferencaBaseICMS != 0 && primeiroPedidoCTesParaSubContratacao.ValorCofins > 0)
                        primeiroPedidoCTesParaSubContratacao.ValorPis += diferencaCofins;

                    repPedidoCTeParaSubContratacao.Atualizar(primeiroPedidoCTesParaSubContratacao);
                }
                else if (pedidosXMLNotaFiscal != null && pedidosXMLNotaFiscal.Count > 0)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal primeiroPedidoXMLNotaFiscal = pedidosXMLNotaFiscal.FirstOrDefault();

                    if (diferencaBaseICMS != 0 && primeiroPedidoXMLNotaFiscal.BaseCalculoICMS > 0)
                        primeiroPedidoXMLNotaFiscal.BaseCalculoICMS += diferencaBaseICMS;
                    if (diferencaICMS != 0 && diferencaBaseICMS != 0 && primeiroPedidoXMLNotaFiscal.ValorICMS > 0)
                        primeiroPedidoXMLNotaFiscal.ValorICMS += diferencaICMS;
                    if (diferencaBaseICMS != 0 && diferencaICMSIncluso != 0 && primeiroPedidoXMLNotaFiscal.ValorICMSIncluso > 0 && totalICMSIncluso > 0)
                        primeiroPedidoXMLNotaFiscal.ValorICMSIncluso += diferencaICMSIncluso;
                    if (diferencaPis != 0 && diferencaBaseICMS != 0 && primeiroPedidoXMLNotaFiscal.ValorPis > 0)
                        primeiroPedidoXMLNotaFiscal.ValorPis += diferencaPis;
                    if (diferencaCofins != 0 && diferencaBaseICMS != 0 && primeiroPedidoXMLNotaFiscal.ValorCofins > 0)
                        primeiroPedidoXMLNotaFiscal.ValorPis += diferencaCofins;

                    repPedidoXMLNotaFiscal.Atualizar(primeiroPedidoXMLNotaFiscal);
                }
            }
        }

        public void RatearFreteCargaPedidoEntreCTesParaSubcontratacao(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Cargas.Carga carga, decimal valorSubContratacao, decimal valorTotalMoeda, List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> componentes, Dominio.Entidades.Embarcador.Rateio.RateioFormula formulaRateio, bool rateioFreteFilialEmissora, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao> cargaPedidoContaContabilContabilizacaosCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteICMS, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componentePisCofins, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> cargapedidoCTeParaSubContratacaoNotasFiscais, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> cargaCTesParaSubContratacao, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> componentesICMSXMLNotaFiscalExistenteCarga, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> componentesPisConfisXMLNotaFiscalExistenteCarga)
        {
            RatearValorCTeSucontratacaoDoPedido(carga, cargaPedido, valorSubContratacao, valorTotalMoeda, componentes, formulaRateio, rateioFreteFilialEmissora, cargaPedidoContaContabilContabilizacaosCarga, tipoServicoMultisoftware, unitOfWork, componenteICMS, componentePisCofins, cargapedidoCTeParaSubContratacaoNotasFiscais, cargaCTesParaSubContratacao, componentesICMSXMLNotaFiscalExistenteCarga, componentesPisConfisXMLNotaFiscalExistenteCarga);
        }

        public void RatearValorCTeSucontratacaoDoPedido(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, decimal valorSubContratacao, decimal valorTotalMoeda, List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> componentes, Dominio.Entidades.Embarcador.Rateio.RateioFormula formulaRateio, bool rateioFreteFilialEmissora, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao> cargaPedidoContaContabilContabilizacaosCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteICMS, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componentePisCofins, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> cargapedidoCTeParaSubContratacaoNotasFiscais, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> cargaCTesParaSubContratacao, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> componentesICMSXMLNotaFiscalExistenteCarga, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> componentesPisConfisXMLNotaFiscalExistenteCarga)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete repPedidoCteParaSubContratacaoComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroComponenteFrete repCTeTerceiroComponenteFrete = new Repositorio.Embarcador.CTe.CTeTerceiroComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoContaContabilContabilizacao repPedidoCTeParaSubContratacaoContaContabilContabilizacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoContaContabilContabilizacao(unitOfWork);

            if (!rateioFreteFilialEmissora)
                repPedidoCTeParaSubContratacaoContaContabilContabilizacao.DeletarPorCarga(carga.Codigo);

            Servicos.Embarcador.Carga.RateioFormula serRateioFormula = new Servicos.Embarcador.Carga.RateioFormula(unitOfWork);
            Servicos.Embarcador.Carga.CTeSubContratacao serCTeSubContratacao = new Servicos.Embarcador.Carga.CTeSubContratacao(unitOfWork);
            Servicos.Embarcador.Carga.RateioNotaFiscal serRateioNotaFiscal = new RateioNotaFiscal(unitOfWork);
            Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);
            Servicos.Embarcador.Pedido.PedidoCTeParaSubContratacao servicoPedidoCTeParaSubContratacao = new Servicos.Embarcador.Pedido.PedidoCTeParaSubContratacao(unitOfWork);
            Servicos.Embarcador.Imposto.ImpostoIBSCBS servicoImpostoIBSCBS = new Servicos.Embarcador.Imposto.ImpostoIBSCBS(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moeda = cargaPedido.Moeda ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real;
            decimal cotacaoMoeda = cargaPedido.ValorCotacaoMoeda ?? 0m;

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            //string descricaoItemPeso = serCTeSubContratacao.ObterDescricaoItemPeso(cargaPedido, unitOfWork, out bool utilizarPrimeiraUnidadeMedidaPeso);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete> pedidoCteParaSubContratacaoComponenteFreteExistentes = repPedidoCteParaSubContratacaoComponenteFrete.BuscarTodosdoCargaPedido(cargaPedido.Codigo, rateioFreteFilialEmissora);
            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete pedidoCteParaSubContratacaoComponenteFreteExistente in pedidoCteParaSubContratacaoComponenteFreteExistentes)
                repPedidoCteParaSubContratacaoComponenteFrete.Deletar(pedidoCteParaSubContratacaoComponenteFreteExistente);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidoCTesParaSubContratacao = null;

            if (cargaPedido.Carga.UtilizarCTesAnterioresComoCTeFilialEmissora && rateioFreteFilialEmissora)
                pedidoCTesParaSubContratacao = (from obj in cargaCTesParaSubContratacao where obj.CargaPedido.Codigo == cargaPedido.Codigo select obj).ToList();
            else
                pedidoCTesParaSubContratacao = (from obj in cargaCTesParaSubContratacao where obj.CargaPedido.Codigo == cargaPedido.Codigo && obj.CteSubContratacaoFilialEmissora == rateioFreteFilialEmissora select obj).ToList();

            int numeroCTes = pedidoCTesParaSubContratacao.Count();
            if (numeroCTes <= 0)
                return;

            decimal somaICMSIncluso = 0m, somaICMS = 0m, somaBaseCalculoICMS = 0m, somaPis = 0m, somaCofins = 0m;

            decimal somaBaseCalculoIBSCBS = 0m;
            decimal somaValorIBSEstadual = 0m;
            decimal somaValorIBSMunicipal = 0m;
            decimal somaValorCBS = 0m;

            bool emissaoAgrupadaPedido = configuracaoTMS.AtivarEmissaoSubcontratacaoAgrupado &&
                                         (cargaPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoAgrupado ||
                                          cargaPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoIndividual);

            Dominio.ObjetosDeValor.Embarcador.CTe.ConfiguracaoCTeEmitidoEmbarcador configuracao = null;

            if (carga.TipoOperacao?.UtilizarValorFreteOriginalSubcontratacao ?? false)
            {
                Servicos.Embarcador.CTe.CTEsImportados svcCTesImportados = new Embarcador.CTe.CTEsImportados(unitOfWork);

                configuracao = svcCTesImportados.ObterConfiguracoesComponentes(cargaPedido);
            }

            int numeroNotasRateio = 1;
            if (formulaRateio?.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porNotaFiscal)
            {
                numeroNotasRateio = pedidoCTesParaSubContratacao.Sum(obj => obj.CTeTerceiro.NumeroTotalDocumentos);
                if (numeroNotasRateio == 0)
                    numeroNotasRateio = 1;
            }


            decimal pesoTotal = pedidoCTesParaSubContratacao.Sum(obj => obj.CTeTerceiro.Peso);
            if (configuracaoTMS.UtilizaEmissaoMultimodal && cargaPedido.Peso > 0m && (cargaPedido.TipoServicoMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal.VinculadoMultimodalTerceiro))
                pesoTotal = cargaPedido.Peso;
            decimal valorTotalNF = pedidoCTesParaSubContratacao.Sum(obj => obj.CTeTerceiro.ValorTotalMercadoria);
            decimal valorTotalCTe = pedidoCTesParaSubContratacao.Sum(obj => obj.CTeTerceiro.ValorAReceber);

            if (!rateioFreteFilialEmissora)
            {
                if (!emissaoAgrupadaPedido)
                {
                    cargaPedido.ValorICMS = 0m;
                    cargaPedido.ValorPis = 0m;
                    cargaPedido.ValorCofins = 0m;
                    cargaPedido.BaseCalculoICMS = 0m;

                    serCargaPedido.ZerarCamposImpostoIBSCBS(cargaPedido, true);
                }
            }
            else
            {
                cargaPedido.ValorICMSFilialEmissora = 0m;
                cargaPedido.BaseCalculoICMSFilialEmissora = 0m;

                serCargaPedido.ZerarCamposImpostoIBSCBSFilialEmissora(cargaPedido, true);
            }

            decimal valorTotalFreteCTe = 0m, valorTotalMoedaCTe = 0m;

            Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao ultimoCTeParaSubContratacao = pedidoCTesParaSubContratacao.LastOrDefault();

            Dominio.Entidades.Embarcador.Rateio.RateioFormula formulaRateioSubContratacao = formulaRateio;

            decimal percentualTabelaSubcontratacao = 0m;
            if (formulaRateioSubContratacao == null)//quando não tem formula de rateio definida usa por padrão a formula por valor de CTe
            {
                formulaRateioSubContratacao = new Dominio.Entidades.Embarcador.Rateio.RateioFormula();
                formulaRateioSubContratacao.ParametroRateioFormula = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porValorCTe;
                Repositorio.Embarcador.Cargas.CargaTabelaFreteSubContratacao repCargaTabelaFreteSubContratacao = new Repositorio.Embarcador.Cargas.CargaTabelaFreteSubContratacao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteSubContratacao cargaTabelaFreteSubContratacao = repCargaTabelaFreteSubContratacao.BuscarPorCarga(carga.Codigo);
                if (cargaTabelaFreteSubContratacao != null)
                    percentualTabelaSubcontratacao = cargaTabelaFreteSubContratacao.PercentualCobrado;
            }

            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscalCte = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.CTe);
            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscalNFSe = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.NFSe);

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao in pedidoCTesParaSubContratacao)
            {
                decimal valorRateioOriginal = 0;
                decimal peso = pedidoCTeParaSubContratacao.CTeTerceiro.Peso;
                decimal valorTotalMercadoria = pedidoCTeParaSubContratacao.CTeTerceiro.ValorTotalMercadoria;
                decimal valorCTe = pedidoCTeParaSubContratacao.CTeTerceiro.ValorAReceber;

                if (moeda != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real)
                {
                    pedidoCTeParaSubContratacao.ValorTotalMoeda = serRateioFormula.AplicarFormulaRateio(formulaRateioSubContratacao, valorTotalMoeda, numeroNotasRateio, numeroCTes, pesoTotal, peso, valorTotalMercadoria, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, valorCTe, percentualTabelaSubcontratacao, valorTotalCTe);

                    if (formulaRateioSubContratacao != null && formulaRateioSubContratacao.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porNotaFiscal)
                        pedidoCTeParaSubContratacao.ValorTotalMoeda = Math.Floor((pedidoCTeParaSubContratacao.ValorTotalMoeda ?? 0m) * pedidoCTeParaSubContratacao.CTeTerceiro.NumeroTotalDocumentos * 100) / 100;

                    pedidoCTeParaSubContratacao.ValorFrete = Math.Round((pedidoCTeParaSubContratacao.ValorTotalMoeda ?? 0m) * cotacaoMoeda, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    pedidoCTeParaSubContratacao.ValorFrete = serRateioFormula.AplicarFormulaRateio(formulaRateioSubContratacao, valorSubContratacao, numeroNotasRateio, numeroCTes, pesoTotal, peso, valorTotalMercadoria, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, valorCTe, percentualTabelaSubcontratacao, valorTotalCTe);

                    if (formulaRateioSubContratacao != null && formulaRateioSubContratacao.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porNotaFiscal)
                        pedidoCTeParaSubContratacao.ValorFrete = Math.Floor((pedidoCTeParaSubContratacao.ValorFrete * pedidoCTeParaSubContratacao.CTeTerceiro.NumeroTotalDocumentos) * 100) / 100;
                }

                valorTotalFreteCTe += pedidoCTeParaSubContratacao.ValorFrete;
                valorTotalMoedaCTe += pedidoCTeParaSubContratacao.ValorTotalMoeda ?? 0m;

                if (pedidoCTeParaSubContratacao.Equals(ultimoCTeParaSubContratacao))
                {
                    if (moeda != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real)
                    {
                        pedidoCTeParaSubContratacao.ValorTotalMoeda += valorTotalMoeda - valorTotalMoedaCTe;
                        pedidoCTeParaSubContratacao.ValorFrete = Math.Round((pedidoCTeParaSubContratacao.ValorTotalMoeda ?? 0m) * cotacaoMoeda, 2, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        pedidoCTeParaSubContratacao.ValorFrete += valorSubContratacao - valorTotalFreteCTe;
                    }
                }

                Servicos.Embarcador.Carga.ICMS svcICMS = new ICMS(unitOfWork);

                Dominio.Entidades.Localidade inicioPrestacao = pedidoCTeParaSubContratacao.CTeTerceiro.LocalidadeInicioPrestacao;
                Dominio.Entidades.Localidade terminoPrestacao = pedidoCTeParaSubContratacao.CTeTerceiro.LocalidadeTerminoPrestacao;

                Dominio.Entidades.Cliente remetente = pedidoCTeParaSubContratacao.CTeTerceiro.Remetente.Cliente;
                Dominio.Entidades.Cliente destinatario = pedidoCTeParaSubContratacao.CTeTerceiro.Destinatario.Cliente;
                //Dominio.Entidades.Cliente tomador = pedidoCTeParaSubContratacao.CTeTerceiro.Tomador.Cliente;

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor && cargaPedido.Expedidor != null)
                    {
                        inicioPrestacao = cargaPedido.Expedidor.Localidade;
                        remetente = cargaPedido.Expedidor;
                    }

                    if ((cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor) && cargaPedido.Recebedor != null && !(cargaPedido.Carga.TipoOperacao?.UtilizarRecebedorApenasComoParticipante ?? false))
                    {
                        terminoPrestacao = cargaPedido.Recebedor.Localidade;
                        destinatario = cargaPedido.Recebedor;
                    }
                }
                else
                {
                    if (pedidoCTeParaSubContratacao.CTeTerceiro.Recebedor != null && cargaPedido.TipoEmissaoCTeParticipantes != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor && cargaPedido.TipoEmissaoCTeParticipantes != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor)
                        terminoPrestacao = destinatario.Localidade;

                    if ((cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada))
                    {
                        if (cargaPedido.Expedidor != null)
                            inicioPrestacao = cargaPedido.Expedidor.Localidade;

                        if (cargaPedido.Recebedor != null && !(cargaPedido.Carga.TipoOperacao?.UtilizarRecebedorApenasComoParticipante ?? false))
                            terminoPrestacao = cargaPedido.Recebedor.Localidade;
                    }
                }

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete> pedidosCteParaSubContratacaoComponentesFrete = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete>();

                pedidoCTeParaSubContratacao.ValorTotalComponentes = 0m;
                pedidoCTeParaSubContratacao.ValorTotalMoedaComponentes = 0m;

                if (carga.TipoOperacao?.UtilizarValorFreteOriginalSubcontratacao ?? false)
                {
                    List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroComponenteFrete> componentesFrete = repCTeTerceiroComponenteFrete.BuscarPorCTeParaSubContratacao(pedidoCTeParaSubContratacao.CTeTerceiro.Codigo);

                    foreach (Dominio.Entidades.Embarcador.CTe.CTeTerceiroComponenteFrete componente in componentesFrete)
                    {
                        if (configuracao.DescricaoComponenteFreteLiquido?.ToLower() == componente.Descricao.ToLower())
                        {
                            pedidoCTeParaSubContratacao.ValorFrete = componente.Valor;
                            continue;
                        }

                        Dominio.ObjetosDeValor.Embarcador.CTe.ConfiguracaoCTeEmitidoEmbarcadorComponente configuracaoComponente = configuracao.Componentes.Where(o => o.OutraDescricaoCTe.ToLower() == componente.Descricao.ToLower()).FirstOrDefault();

                        Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = null;

                        if (configuracaoComponente != null)
                            componenteFrete = repComponenteFrete.BuscarPorCodigo(configuracaoComponente.Codigo);

                        if (componenteFrete == null)
                            componenteFrete = repComponenteFrete.BuscarPorDescricao(componente.Descricao);

                        if (componenteFrete == null)
                            throw new Exception($"CodigoCargaEmbarcador = {carga.CodigoCargaEmbarcador}. O componente {componente.Descricao} não foi encontrado nas configurações da operação e dos componentes do sistema para ser vinculado à carga.");

                        if (componenteFrete.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS || componenteFrete.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS)
                            continue;

                        Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete pedidoCteParaSubContratacaoComponenteFrete = new Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete
                        {
                            ComponenteFrete = componenteFrete,
                            PedidoCTeParaSubContratacao = pedidoCTeParaSubContratacao,
                            TipoComponenteFrete = componenteFrete.TipoComponenteFrete,
                            ValorComponente = componente.Valor,
                            TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor,
                            IncluirBaseCalculoICMS = configuracaoComponente?.IncluirICMS ?? true,
                            OutraDescricaoCTe = componente.Descricao,
                            ModeloDocumentoFiscal = cargaPedido.ModeloDocumentoFiscal,
                            IncluirIntegralmenteContratoFreteTerceiro = false
                        };

                        repPedidoCteParaSubContratacaoComponenteFrete.Inserir(pedidoCteParaSubContratacaoComponenteFrete);

                        pedidosCteParaSubContratacaoComponentesFrete.Add(pedidoCteParaSubContratacaoComponenteFrete);

                        pedidoCTeParaSubContratacao.ValorTotalComponentes += pedidoCteParaSubContratacaoComponenteFrete.ValorComponente;
                    }
                }
                else
                {
                    foreach (Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico componenteDinamico in componentes)
                    {
                        Dominio.Entidades.Embarcador.Rateio.RateioFormula formulaComponente = componenteDinamico.RateioFormula != null ? componenteDinamico.RateioFormula : formulaRateioSubContratacao;
                        decimal valorComponente = 0m, valorMoedaComponente = 0m;

                        if (moeda != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real)
                        {
                            decimal valorMoedaComponenteRateado = serRateioFormula.AplicarFormulaRateio(formulaRateioSubContratacao, componenteDinamico.ValorTotalMoeda ?? 0m, numeroNotasRateio, numeroCTes, pesoTotal, peso, valorTotalMercadoria, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, valorCTe, percentualTabelaSubcontratacao, valorTotalCTe);

                            if (formulaRateioSubContratacao != null && formulaRateioSubContratacao.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porNotaFiscal)
                                valorMoedaComponenteRateado = Math.Floor(valorMoedaComponenteRateado * pedidoCTeParaSubContratacao.CTeTerceiro.NumeroTotalDocumentos * 100) / 100;

                            if (pedidoCTeParaSubContratacao.Equals(ultimoCTeParaSubContratacao))
                            {
                                decimal valorTotalMoedaComponente = componentes.Where(obj => obj.TipoComponenteFrete == componenteDinamico.TipoComponenteFrete && (componenteDinamico.ComponenteFrete == null || obj.ComponenteFrete.Equals(componenteDinamico.ComponenteFrete))).Sum(obj => obj.ValorTotalMoeda ?? 0m);
                                decimal valorTotalMoedaCargaPedidoComponente = repPedidoCteParaSubContratacaoComponenteFrete.BuscarTotalMoedaCargaPedidoPorComponente(pedidoCTeParaSubContratacao.CargaPedido.Codigo, componenteDinamico.TipoComponenteFrete, componenteDinamico.ComponenteFrete) + valorMoedaComponente;

                                valorMoedaComponente += valorTotalMoedaComponente - valorTotalMoedaCargaPedidoComponente;
                            }

                            valorComponente = Math.Round(valorMoedaComponente * cotacaoMoeda, 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            decimal valorComponenteRateado = serRateioFormula.AplicarFormulaRateio(formulaRateioSubContratacao, componenteDinamico.ValorComponente, numeroNotasRateio, numeroCTes, pesoTotal, peso, valorTotalMercadoria, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, valorCTe, percentualTabelaSubcontratacao, valorTotalCTe);
                            if (formulaRateioSubContratacao != null && formulaRateioSubContratacao.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porNotaFiscal)
                                valorComponenteRateado = Math.Floor((valorComponenteRateado * pedidoCTeParaSubContratacao.CTeTerceiro.NumeroTotalDocumentos) * 100) / 100;

                            valorComponente = Math.Round(valorComponenteRateado, 2, MidpointRounding.AwayFromZero);

                            if (pedidoCTeParaSubContratacao.Equals(ultimoCTeParaSubContratacao))
                            {
                                decimal valorTotalComponente = (from obj in componentes where obj.TipoComponenteFrete == componenteDinamico.TipoComponenteFrete && (componenteDinamico.ComponenteFrete == null || obj.ComponenteFrete.Equals(componenteDinamico.ComponenteFrete)) select obj.ValorComponente).Sum();
                                decimal valorTotalCargaPedidoComponente = repPedidoCteParaSubContratacaoComponenteFrete.BuscarTotalCargaPedidoPorCompomente(pedidoCTeParaSubContratacao.CargaPedido.Codigo, componenteDinamico.TipoComponenteFrete, componenteDinamico.ComponenteFrete) + valorComponente;
                                valorComponente += valorTotalComponente - valorTotalCargaPedidoComponente;
                            }
                        }

                        Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete pedidoCteParaSubContratacaoComponenteFrete = new Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete
                        {
                            ComponenteFrete = componenteDinamico.ComponenteFrete,
                            PedidoCTeParaSubContratacao = pedidoCTeParaSubContratacao,
                            TipoComponenteFrete = componenteDinamico.TipoComponenteFrete,
                            ValorComponente = valorComponente,
                            Percentual = componenteDinamico.Percentual,
                            TipoValor = componenteDinamico.TipoValor,
                            ComponenteFilialEmissora = rateioFreteFilialEmissora,
                            DescontarValorTotalAReceber = componenteDinamico.DescontarValorTotalAReceber,
                            AcrescentaValorTotalAReceber = componenteDinamico.AcrescentaValorTotalAReceber,
                            NaoSomarValorTotalAReceber = componenteDinamico.NaoSomarValorTotalAReceber,
                            DescontarDoValorAReceberValorComponente = componenteDinamico.DescontarDoValorAReceberValorComponente,
                            DescontarDoValorAReceberOICMSDoComponente = componenteDinamico.DescontarDoValorAReceberOICMSDoComponente,
                            ValorICMSComponenteDestacado = componenteDinamico.ValorICMSComponenteDestacado,
                            NaoSomarValorTotalPrestacao = componenteDinamico.NaoSomarValorTotalPrestacao,
                            RateioFormula = componenteDinamico.RateioFormula,
                            IncluirBaseCalculoICMS = componenteDinamico.IncluirBaseCalculoImposto,
                            OutraDescricaoCTe = componenteDinamico.OutraDescricaoCTe,
                            ModeloDocumentoFiscal = componenteDinamico.ModeloDocumentoFiscal,
                            Moeda = moeda,
                            ValorCotacaoMoeda = cotacaoMoeda,
                            ValorTotalMoeda = valorMoedaComponente,
                            IncluirIntegralmenteContratoFreteTerceiro = componenteDinamico.IncluirIntegralmenteContratoFreteTerceiro
                        };

                        repPedidoCteParaSubContratacaoComponenteFrete.Inserir(pedidoCteParaSubContratacaoComponenteFrete);

                        pedidosCteParaSubContratacaoComponentesFrete.Add(pedidoCteParaSubContratacaoComponenteFrete);

                        pedidoCTeParaSubContratacao.ValorTotalComponentes += valorComponente;
                        pedidoCTeParaSubContratacao.ValorTotalMoedaComponentes += valorMoedaComponente;
                    }
                }

                serCargaPedido.VerificarQuaisDocumentosDeveEmitir(carga, cargaPedido, inicioPrestacao, terminoPrestacao, tipoServicoMultisoftware, unitOfWork, out bool possuiCTe, out bool possuiNFS, out bool possuiNFSManual, out Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscalIntramunicipal, configuracaoTMS, out bool sempreDisponibilizarDocumentoNFSManual);

                if (possuiCTe)
                {
                    if (pedidoCTeParaSubContratacao.ModeloDocumentoFiscal == null ||
                        pedidoCTeParaSubContratacao.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe ||
                        pedidoCTeParaSubContratacao.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)
                        pedidoCTeParaSubContratacao.ModeloDocumentoFiscal = modeloDocumentoFiscalCte;

                    if ((cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SVMTerceiro || (cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada && !carga.EmitirCTeComplementar) ||
                        cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada) && (!cargaPedido.EmitirComplementarFilialEmissora || !rateioFreteFilialEmissora))
                    {
                        if (!string.IsNullOrWhiteSpace(carga.Empresa?.Configuracao?.ObservacaoCTeSubcontratacao ?? ""))
                            pedidoCTeParaSubContratacao.ObservacaoRegraICMSCTe = carga.Empresa.Configuracao.ObservacaoCTeSubcontratacao;
                        else if (!string.IsNullOrWhiteSpace(cargaPedido.ObservacaoRegraICMSCTe))
                            pedidoCTeParaSubContratacao.ObservacaoRegraICMSCTe = cargaPedido.ObservacaoRegraICMSCTe;

                        string cstCargaPedido = rateioFreteFilialEmissora ? cargaPedido.CSTFilialEmissora : cargaPedido.CST;
                        Dominio.Entidades.CFOP cfopCargaPedido = rateioFreteFilialEmissora ? cargaPedido.CFOPFilialEmissora : cargaPedido.CFOP;

                        if (configuracaoTMS.UtilizarRegraICMSCTeSubcontratacao)
                        {
                            bool incluirICMSBase = false;
                            decimal percentualIncluiFrete = 0m;
                            decimal valorBaseCalculo = 0m;

                            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = svcICMS.BuscarRegraICMS(carga, cargaPedido, carga.Empresa, remetente, destinatario, pedidoCTeParaSubContratacao.CTeTerceiro.Emitente.Cliente, inicioPrestacao, terminoPrestacao, ref incluirICMSBase, ref percentualIncluiFrete, valorBaseCalculo, null, unitOfWork, tipoServicoMultisoftware, configuracaoTMS, cargaPedido.TipoContratacaoCarga);

                            bool utilizouRegraICMS = false;

                            if (!string.IsNullOrWhiteSpace(carga.Empresa?.Configuracao?.CSTCTeSubcontratacao))
                                pedidoCTeParaSubContratacao.CST = carga.Empresa?.Configuracao.CSTCTeSubcontratacao;
                            else if (!string.IsNullOrWhiteSpace(configuracaoTMS.CSTCTeSubcontratacao))
                                pedidoCTeParaSubContratacao.CST = configuracaoTMS.CSTCTeSubcontratacao;
                            else if (regraICMS != null)
                            {
                                pedidoCTeParaSubContratacao.CST = regraICMS.CST;
                                utilizouRegraICMS = true;
                            }
                            else if (!string.IsNullOrWhiteSpace(cstCargaPedido))
                                pedidoCTeParaSubContratacao.CST = cstCargaPedido;
                            else
                                pedidoCTeParaSubContratacao.CST = "40";

                            if (carga.Empresa?.Configuracao?.CFOPCTeSubcontratacao != null)
                                pedidoCTeParaSubContratacao.CFOP = carga.Empresa?.Configuracao.CFOPCTeSubcontratacao;
                            else if (regraICMS != null && regraICMS.CFOP > 0)
                            {
                                pedidoCTeParaSubContratacao.CFOP = repCFOP.BuscarPorNumero(regraICMS.CFOP);
                                utilizouRegraICMS = true;
                            }
                            else
                            {
                                Dominio.Entidades.Cliente tomador = Servicos.Embarcador.Carga.CTeSubContratacao.ObterTomadorCTeParaSubcontratacao(carga, cargaPedido, pedidoCTeParaSubContratacao, configuracaoTMS, tipoServicoMultisoftware);

                                Dominio.Entidades.Aliquota aliquota = svcICMS.ObterAliquota(carga.Empresa.Localidade.Estado, inicioPrestacao.Estado, terminoPrestacao.Estado, tomador.Atividade, pedidoCTeParaSubContratacao.CTeTerceiro.Destinatario.Atividade, unitOfWork);

                                pedidoCTeParaSubContratacao.CFOP = aliquota?.CFOP;
                            }

                            if (utilizouRegraICMS)
                                pedidoCTeParaSubContratacao.SetarRegraICMS(regraICMS.CodigoRegra);
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(carga.Empresa.Configuracao?.CSTCTeSubcontratacao))
                                pedidoCTeParaSubContratacao.CST = carga.Empresa.Configuracao.CSTCTeSubcontratacao;
                            else if (!string.IsNullOrWhiteSpace(configuracaoTMS.CSTCTeSubcontratacao))
                                pedidoCTeParaSubContratacao.CST = configuracaoTMS.CSTCTeSubcontratacao;
                            else if (!string.IsNullOrWhiteSpace(cstCargaPedido))
                                pedidoCTeParaSubContratacao.CST = cstCargaPedido;
                            else
                                pedidoCTeParaSubContratacao.CST = "40";

                            if (carga.Empresa.Configuracao?.CFOPCTeSubcontratacao != null)
                                pedidoCTeParaSubContratacao.CFOP = carga.Empresa.Configuracao.CFOPCTeSubcontratacao;
                            else if (cfopCargaPedido != null)
                                pedidoCTeParaSubContratacao.CFOP = cfopCargaPedido;
                            else
                            {
                                Dominio.Entidades.Cliente tomador = Servicos.Embarcador.Carga.CTeSubContratacao.ObterTomadorCTeParaSubcontratacao(carga, cargaPedido, pedidoCTeParaSubContratacao, configuracaoTMS, tipoServicoMultisoftware);

                                Dominio.Entidades.Aliquota aliquota = svcICMS.ObterAliquota(carga.Empresa.Localidade.Estado, inicioPrestacao.Estado, terminoPrestacao.Estado, tomador.Atividade, pedidoCTeParaSubContratacao.CTeTerceiro.Destinatario.Atividade, unitOfWork);

                                pedidoCTeParaSubContratacao.CFOP = aliquota?.CFOP;
                            }
                        }

                        pedidoCTeParaSubContratacao.ValorPis = 0m;
                        pedidoCTeParaSubContratacao.ValorCofins = 0m;
                        pedidoCTeParaSubContratacao.ValorICMS = 0m;
                        pedidoCTeParaSubContratacao.ValorICMSIncluso = 0m;
                        pedidoCTeParaSubContratacao.ModeloDocumentoFiscal = cargaPedido.ModeloDocumentoFiscal;//todo: está fixo por cargaPedido, depois podemos ver para ser dinamico igual é a nota.
                        pedidoCTeParaSubContratacao.PercentualAliquota = 0m;
                        pedidoCTeParaSubContratacao.AliquotaPis = 0m;
                        pedidoCTeParaSubContratacao.AliquotaCofins = 0m;
                        pedidoCTeParaSubContratacao.PercentualAliquotaInternaDifal = 0m;
                        pedidoCTeParaSubContratacao.PercentualIncluirBaseCalculo = 100m;
                        pedidoCTeParaSubContratacao.PercentualReducaoBC = 0m;

                        if (pedidoCTeParaSubContratacao.CST == "60" && configuracaoTMS.UtilizaEmissaoMultimodal)
                            pedidoCTeParaSubContratacao.PercentualReducaoBC = 100m;

                        servicoPedidoCTeParaSubContratacao.ZerarCamposImpostoIBSCBS(pedidoCTeParaSubContratacao);
                    }
                    else
                    {
                        decimal aliquotaPis = cargaPedido.AliquotaPis;
                        decimal aliquotaCofins = cargaPedido.AliquotaCofins;
                        decimal aliquota = cargaPedido.PercentualAliquota;
                        decimal aliquotaInternaDifal = cargaPedido.PercentualAliquotaInternaDifal;
                        decimal percentualReducaoBC = cargaPedido.PercentualReducaoBC;
                        decimal percentualIncluirBaseCalculo = cargaPedido.PercentualIncluirBaseCalculo;
                        bool incluirICMSBaseCalculo = cargaPedido.IncluirICMSBaseCalculo;
                        string cst = cargaPedido.CST;
                        decimal baseCalculoIBSCBS = pedidoCTeParaSubContratacao.ValorFrete;

                        Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = servicoImpostoIBSCBS.ObterImpostoIBSCBSComTributacaoDefinida(cargaPedido, baseCalculoIBSCBS);

                        if (rateioFreteFilialEmissora)
                        {
                            aliquota = cargaPedido.PercentualAliquotaFilialEmissora;
                            aliquotaInternaDifal = cargaPedido.PercentualAliquotaFilialEmissoraInternaDifal;
                            percentualReducaoBC = cargaPedido.PercentualReducaoBCFilialEmissora;
                            percentualIncluirBaseCalculo = cargaPedido.PercentualIncluirBaseCalculoFilialEmissora;
                            incluirICMSBaseCalculo = cargaPedido.IncluirICMSBaseCalculoFilialEmissora;
                            cst = cargaPedido.CSTFilialEmissora;

                            impostoIBSCBS = servicoImpostoIBSCBS.ObterImpostoIBSCBSComTributacaoDefinidaFilialEmissora(cargaPedido, baseCalculoIBSCBS);
                        }

                        if (!(cargaPedido.Carga.TipoOperacao.ConfiguracaoCalculoFrete.ConsiderarInclusaoIcmsCargaPedidoNaEmissaoCteComplementar ?? false) && (cargaPedido.Carga.EmitirCTeComplementar || (cargaPedido.EmitirComplementarFilialEmissora && rateioFreteFilialEmissora)))
                        {
                            aliquota = pedidoCTeParaSubContratacao.CTeTerceiro.AliquotaICMS;
                            cst = pedidoCTeParaSubContratacao.CTeTerceiro.CST;
                            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                            {
                                incluirICMSBaseCalculo = pedidoCTeParaSubContratacao.IncluirICMSBaseCalculo;
                                percentualIncluirBaseCalculo = pedidoCTeParaSubContratacao.PercentualIncluirBaseCalculo;
                                percentualReducaoBC = pedidoCTeParaSubContratacao.PercentualReducaoBC;
                            }

                            impostoIBSCBS = servicoImpostoIBSCBS.ObterImpostoIBSCBSComTributacaoDefinida(pedidoCTeParaSubContratacao, baseCalculoIBSCBS);
                        }

                        decimal baseCalculoICMS = 0m, icmsIncluso = 0m, icms = 0m, pis = 0m, cofins = 0m;
                        decimal valorIBSEstadual = 0m;
                        decimal valorIBSMunicipal = 0m;
                        decimal valorCBS = 0m;

                        if (emissaoAgrupadaPedido)
                        {
                            if (pedidoCTeParaSubContratacao.Equals(ultimoCTeParaSubContratacao))
                            {
                                baseCalculoICMS = cargaPedido.BaseCalculoICMS - somaBaseCalculoICMS;
                                icms = cargaPedido.ValorICMS - somaICMS;
                                icmsIncluso = cargaPedido.ValorICMSIncluso - somaICMSIncluso;
                                pis = cargaPedido.ValorPis - somaPis;
                                cofins = cargaPedido.ValorCofins - somaCofins;

                                baseCalculoIBSCBS = cargaPedido.BaseCalculoIBSCBS - somaBaseCalculoIBSCBS;
                                valorIBSEstadual = cargaPedido.ValorIBSEstadual - somaValorIBSEstadual;
                                valorIBSMunicipal = cargaPedido.ValorIBSMunicipal - somaValorIBSMunicipal;
                                valorCBS = cargaPedido.ValorCBS - somaValorCBS;
                            }
                            else
                            {
                                decimal valorRateioICMSOriginal = 0m;

                                baseCalculoICMS = serRateioFormula.AplicarFormulaRateio(formulaRateioSubContratacao, cargaPedido.BaseCalculoICMS, numeroNotasRateio, numeroCTes, pesoTotal, peso, valorTotalMercadoria, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioICMSOriginal, valorCTe, percentualTabelaSubcontratacao, valorTotalCTe);
                                icms = serRateioFormula.AplicarFormulaRateio(formulaRateioSubContratacao, cargaPedido.ValorICMS, numeroNotasRateio, numeroCTes, pesoTotal, peso, valorTotalMercadoria, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioICMSOriginal, valorCTe, percentualTabelaSubcontratacao, valorTotalCTe);
                                icmsIncluso = serRateioFormula.AplicarFormulaRateio(formulaRateioSubContratacao, cargaPedido.ValorICMSIncluso, numeroNotasRateio, numeroCTes, pesoTotal, peso, valorTotalMercadoria, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioICMSOriginal, valorCTe, percentualTabelaSubcontratacao, valorTotalCTe);
                                pis = serRateioFormula.AplicarFormulaRateio(formulaRateioSubContratacao, cargaPedido.ValorPis, numeroNotasRateio, numeroCTes, pesoTotal, peso, valorTotalMercadoria, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioICMSOriginal, valorCTe, percentualTabelaSubcontratacao, valorTotalCTe);
                                cofins = serRateioFormula.AplicarFormulaRateio(formulaRateioSubContratacao, cargaPedido.ValorCofins, numeroNotasRateio, numeroCTes, pesoTotal, peso, valorTotalMercadoria, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioICMSOriginal, valorCTe, percentualTabelaSubcontratacao, valorTotalCTe);

                                baseCalculoIBSCBS = serRateioFormula.AplicarFormulaRateio(formulaRateioSubContratacao, cargaPedido.BaseCalculoIBSCBS, numeroNotasRateio, numeroCTes, pesoTotal, peso, valorTotalMercadoria, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioICMSOriginal, valorCTe, percentualTabelaSubcontratacao, valorTotalCTe);
                                valorIBSEstadual = serRateioFormula.AplicarFormulaRateio(formulaRateioSubContratacao, cargaPedido.ValorIBSEstadual, numeroNotasRateio, numeroCTes, pesoTotal, peso, valorTotalMercadoria, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioICMSOriginal, valorCTe, percentualTabelaSubcontratacao, valorTotalCTe);
                                valorIBSMunicipal = serRateioFormula.AplicarFormulaRateio(formulaRateioSubContratacao, cargaPedido.ValorIBSMunicipal, numeroNotasRateio, numeroCTes, pesoTotal, peso, valorTotalMercadoria, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioICMSOriginal, valorCTe, percentualTabelaSubcontratacao, valorTotalCTe);
                                valorCBS = serRateioFormula.AplicarFormulaRateio(formulaRateioSubContratacao, cargaPedido.ValorCBS, numeroNotasRateio, numeroCTes, pesoTotal, peso, valorTotalMercadoria, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioICMSOriginal, valorCTe, percentualTabelaSubcontratacao, valorTotalCTe);
                            }
                        }
                        else
                        {
                            baseCalculoICMS = pedidoCTeParaSubContratacao.ValorFrete;
                            ObterBaseCalculo(ref baseCalculoICMS, carga, cargaPedido, pedidoCTeParaSubContratacao, unitOfWork, tipoServicoMultisoftware, pedidosCteParaSubContratacaoComponentesFrete);

                            decimal aliquotaPiscofins = aliquotaPis + aliquotaCofins;
                            icmsIncluso = CalcularICMSInclusoNoFrete(cst, ref baseCalculoICMS, aliquota, percentualIncluirBaseCalculo, percentualReducaoBC, incluirICMSBaseCalculo, aliquotaInternaDifal, aliquotaPiscofins, unitOfWork);
                            icms = CalcularInclusaoICMSNoFrete(cst, ref baseCalculoICMS, aliquota, percentualIncluirBaseCalculo, percentualReducaoBC, incluirICMSBaseCalculo, aliquotaPiscofins);
                            if (aliquotaPiscofins > 0)
                            {
                                Servicos.Embarcador.Imposto.ImpostoPisCofins servicoPisCofins = new Servicos.Embarcador.Imposto.ImpostoPisCofins();
                                pis = servicoPisCofins.CalcularValorPis(aliquotaPis, baseCalculoICMS);
                                cofins = servicoPisCofins.CalcularValorCofins(aliquotaCofins, baseCalculoICMS);
                            }

                            baseCalculoIBSCBS = impostoIBSCBS.BaseCalculo;
                            valorIBSEstadual = impostoIBSCBS.ValorIBSEstadual;
                            valorIBSMunicipal = impostoIBSCBS.ValorIBSMunicipal;
                            valorCBS = impostoIBSCBS.ValorCBS;
                        }

                        somaBaseCalculoICMS += baseCalculoICMS;
                        somaICMS += icms;
                        somaICMSIncluso += icmsIncluso;
                        somaPis += pis;
                        somaCofins += cofins;

                        somaBaseCalculoIBSCBS += baseCalculoIBSCBS;
                        somaValorIBSEstadual += valorIBSEstadual;
                        somaValorIBSMunicipal += valorIBSMunicipal;
                        somaValorCBS += valorCBS;

                        pedidoCTeParaSubContratacao.BaseCalculoICMS = baseCalculoICMS;
                        pedidoCTeParaSubContratacao.IncluirICMSBaseCalculo = incluirICMSBaseCalculo;
                        pedidoCTeParaSubContratacao.PercentualReducaoBC = percentualReducaoBC;
                        pedidoCTeParaSubContratacao.PercentualIncluirBaseCalculo = percentualIncluirBaseCalculo;
                        pedidoCTeParaSubContratacao.ValorICMS = Math.Round(icms, 2, MidpointRounding.AwayFromZero);
                        pedidoCTeParaSubContratacao.ValorICMSIncluso = Math.Round(icmsIncluso, 2, MidpointRounding.AwayFromZero);
                        pedidoCTeParaSubContratacao.ValorPis = Math.Round(pis, 2, MidpointRounding.AwayFromZero);
                        pedidoCTeParaSubContratacao.ValorCofins = Math.Round(cofins, 2, MidpointRounding.AwayFromZero);

                        if (!cargaPedido.Carga.EmitirCTeComplementar || (cargaPedido.EmitirComplementarFilialEmissora && rateioFreteFilialEmissora))
                        {
                            if (pedidoCTeParaSubContratacao.CteSubContratacaoFilialEmissora)
                            {
                                pedidoCTeParaSubContratacao.CFOP = cargaPedido.CFOPFilialEmissora;
                                pedidoCTeParaSubContratacao.CST = cargaPedido.CSTFilialEmissora;
                            }
                            else
                            {
                                pedidoCTeParaSubContratacao.CFOP = cargaPedido.CFOP;
                                pedidoCTeParaSubContratacao.CST = cargaPedido.CST;
                            }
                        }
                        else
                        {
                            pedidoCTeParaSubContratacao.CFOP = pedidoCTeParaSubContratacao.CTeTerceiro.CFOP;
                            pedidoCTeParaSubContratacao.CST = pedidoCTeParaSubContratacao.CTeTerceiro.CST;
                        }

                        pedidoCTeParaSubContratacao.ModeloDocumentoFiscal = cargaPedido.ModeloDocumentoFiscal;
                        pedidoCTeParaSubContratacao.PercentualAliquota = aliquota;
                        pedidoCTeParaSubContratacao.AliquotaPis = aliquotaPis;
                        pedidoCTeParaSubContratacao.AliquotaCofins = aliquotaCofins;
                        pedidoCTeParaSubContratacao.PercentualAliquotaInternaDifal = aliquotaInternaDifal;

                        pedidoCTeParaSubContratacao.ObservacaoRegraICMSCTe = cargaPedido.ObservacaoRegraICMSCTe;
                        if (cargaPedido.RegraICMS != null)
                            pedidoCTeParaSubContratacao.RegraICMS = cargaPedido.RegraICMS;

                        servicoPedidoCTeParaSubContratacao.PreencherCamposImpostoIBSCBS(pedidoCTeParaSubContratacao, impostoIBSCBS);
                        servicoPedidoCTeParaSubContratacao.PreencherValoresImpostoIBSCBS(pedidoCTeParaSubContratacao, baseCalculoIBSCBS, valorIBSEstadual, valorIBSMunicipal, valorCBS, true);

                        if (!rateioFreteFilialEmissora)
                        {
                            if (!emissaoAgrupadaPedido)
                            {
                                cargaPedido.ValorPis += pedidoCTeParaSubContratacao.ValorPis;
                                cargaPedido.ValorCofins += pedidoCTeParaSubContratacao.ValorCofins;
                                cargaPedido.ValorICMS += pedidoCTeParaSubContratacao.ValorICMS;
                                cargaPedido.BaseCalculoICMS += baseCalculoICMS;

                                serCargaPedido.PreencherValoresImpostoIBSCBS(cargaPedido, baseCalculoIBSCBS, valorIBSEstadual, valorIBSMunicipal, valorCBS, true);
                            }
                        }
                        else
                        {
                            cargaPedido.ValorICMSFilialEmissora += pedidoCTeParaSubContratacao.ValorICMS;
                            cargaPedido.BaseCalculoICMSFilialEmissora += baseCalculoICMS;

                            serCargaPedido.PreencherValoresImpostoIBSCBSFilialEmissora(cargaPedido, baseCalculoIBSCBS, valorIBSEstadual, valorIBSMunicipal, valorCBS, true);
                        }

                    }

                    pedidoCTeParaSubContratacao.PossuiCTe = true;
                    pedidoCTeParaSubContratacao.PossuiNFS = false;
                    pedidoCTeParaSubContratacao.PossuiNFSManual = false;

                    if (!rateioFreteFilialEmissora)
                        cargaPedido.PossuiCTe = true;
                }
                else
                {
                    pedidoCTeParaSubContratacao.IncluirICMSBaseCalculo = false;
                    pedidoCTeParaSubContratacao.PercentualIncluirBaseCalculo = 0m;
                    pedidoCTeParaSubContratacao.BaseCalculoICMS = 0m;
                    pedidoCTeParaSubContratacao.PercentualReducaoBC = 0m;
                    pedidoCTeParaSubContratacao.PercentualAliquota = 0m;
                    pedidoCTeParaSubContratacao.AliquotaPis = 0m;
                    pedidoCTeParaSubContratacao.AliquotaCofins = 0m;
                    pedidoCTeParaSubContratacao.PercentualAliquotaInternaDifal = 0m;
                    pedidoCTeParaSubContratacao.ValorICMS = 0m;
                    pedidoCTeParaSubContratacao.ValorPis = 0m;
                    pedidoCTeParaSubContratacao.ValorCofins = 0m;
                    pedidoCTeParaSubContratacao.ValorICMSIncluso = 0m;
                    pedidoCTeParaSubContratacao.PossuiCTe = false;

                    servicoPedidoCTeParaSubContratacao.ZerarCamposImpostoIBSCBS(pedidoCTeParaSubContratacao);

                    if (pedidoCTeParaSubContratacao.ModeloDocumentoFiscal != null && pedidoCTeParaSubContratacao.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                        pedidoCTeParaSubContratacao.ModeloDocumentoFiscal = cargaPedido.ModeloDocumentoFiscal;
                }

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoContaContabilContabilizacao> pedidoCTeParaSubContratacaoContaContabilContabilizacaos = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoContaContabilContabilizacao>();

                if (!rateioFreteFilialEmissora)
                {
                    if (possuiNFS)
                    {
                        if (pedidoCTeParaSubContratacao.ModeloDocumentoFiscal == null ||
                            pedidoCTeParaSubContratacao.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ||
                            pedidoCTeParaSubContratacao.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)
                            pedidoCTeParaSubContratacao.ModeloDocumentoFiscal = modeloDocumentoFiscalNFSe;

                        pedidoCTeParaSubContratacao.PossuiCTe = false;
                        pedidoCTeParaSubContratacao.PossuiNFS = true;
                        pedidoCTeParaSubContratacao.PossuiNFSManual = false;

                        cargaPedido.PossuiNFS = true;
                    }
                    else
                    {
                        pedidoCTeParaSubContratacao.PossuiNFS = false;
                        if (pedidoCTeParaSubContratacao.ModeloDocumentoFiscal != null && pedidoCTeParaSubContratacao.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                            pedidoCTeParaSubContratacao.ModeloDocumentoFiscal = cargaPedido.ModeloDocumentoFiscal;
                    }

                    if (possuiNFSManual)
                    {
                        pedidoCTeParaSubContratacao.ModeloDocumentoFiscal = null;
                        pedidoCTeParaSubContratacao.PossuiNFSManual = true;

                        cargaPedido.PossuiNFSManual = true;
                        cargaPedido.ModeloDocumentoFiscalIntramunicipal = modeloDocumentoFiscalIntramunicipal;
                    }
                    else
                    {
                        pedidoCTeParaSubContratacao.PossuiNFSManual = false;
                    }

                    if (!rateioFreteFilialEmissora)
                        pedidoCTeParaSubContratacaoContaContabilContabilizacaos = InformarDadosContabeisCTeSubContratacao(pedidoCTeParaSubContratacao, false, cargaPedido, cargaPedidoContaContabilContabilizacaosCarga, configuracaoTMS, tipoServicoMultisoftware, unitOfWork);
                }


                repPedidoCTeParaSubContratacao.Atualizar(pedidoCTeParaSubContratacao);

                if (!rateioFreteFilialEmissora && (pedidoCTeParaSubContratacao.CTeTerceiro.ObterCargaCTe(cargaPedido.Carga.Codigo) == null || (cargaPedido.CargaPedidoTrechoAnterior == null && !(carga.TipoOperacao?.ExclusivaDeSubcontratacaoOuRedespacho ?? false) && !(carga.TipoOperacao?.NaoExigeVeiculoParaEmissao ?? false))))//apenas por CT-e anterior se não foi criado por filial emissora, senão gera por nota.
                    Servicos.Embarcador.Escrituracao.DocumentoProvisao.AdicionarDocumentoParaProvisao(pedidoCTeParaSubContratacao, false, tipoServicoMultisoftware, unitOfWork);

                if (!rateioFreteFilialEmissora)
                    RatearValorFreteCTeSubcontratacaoEntreNotasDoCTeDeSubcontratacao(cargaPedido, pedidoCTeParaSubContratacao, pedidosCteParaSubContratacaoComponentesFrete, formulaRateio, peso, valorTotalMercadoria, pedidoCTeParaSubContratacaoContaContabilContabilizacaos, tipoServicoMultisoftware, unitOfWork, componenteICMS, componentePisCofins, cargapedidoCTeParaSubContratacaoNotasFiscais, componentesICMSXMLNotaFiscalExistenteCarga, componentesPisConfisXMLNotaFiscalExistenteCarga);
            }

            //cargaPedido.ValorICMSIncluso = decimal.Round(cargaPedido.ValorICMSIncluso, 2, MidpointRounding.AwayFromZero);

            repCargaPedido.Atualizar(cargaPedido);
        }


        #endregion

        #region Métodos Privados

        private decimal CalcularInclusaoICMSNoFrete(string CST, ref decimal valorBaseCalculoICMS, decimal aliquota, decimal percentualICMSIncluirNoFrete, decimal percentualReducaoBaseCalculoICMS, bool incluirICMSBase, decimal aliquotaPISCOFINS)
        {
            incluirICMSBase = false;
            //valorBaseCalculoICMS -= valorBaseCalculoICMS * (percentualReducaoBaseCalculoICMS / 100);
            decimal percentualAliquota;
            percentualAliquota = CST != "40" && CST != "41" && CST != "51" && CST != "" ? aliquota : 0;
            decimal percentualICMSRecolhido = incluirICMSBase ? percentualICMSIncluirNoFrete : 0;
            valorBaseCalculoICMS += incluirICMSBase ? (percentualAliquota > 0 || aliquotaPISCOFINS > 0 ? ((valorBaseCalculoICMS / ((100 - percentualAliquota - aliquotaPISCOFINS) / 100)) - valorBaseCalculoICMS) : 0) : 0;
            decimal valorICMS = valorBaseCalculoICMS * (percentualAliquota / 100);
            valorBaseCalculoICMS = decimal.Round(valorBaseCalculoICMS, 2, MidpointRounding.AwayFromZero);
            return valorICMS;
        }

        private decimal CalcularICMSInclusoNoFrete(string CST, ref decimal valorBaseCalculoICMS, decimal aliquota, decimal percentualICMSIncluirNoFrete, decimal percentualReducaoBaseCalculoICMS, bool incluirICMSBase, decimal aliquotaInternaDifal, decimal aliquotaPISCOFINS, Repositorio.UnitOfWork unitOfWork)
        {
            valorBaseCalculoICMS -= valorBaseCalculoICMS * (percentualReducaoBaseCalculoICMS / 100);
            decimal percentualAliquota;

            if (aliquotaInternaDifal > 0)
                percentualAliquota = CST != "40" && CST != "41" && CST != "51" && CST != "" ? aliquotaInternaDifal : 0;
            else
                percentualAliquota = CST != "40" && CST != "41" && CST != "51" && CST != "" ? aliquota : 0;
            decimal percentualICMSRecolhido = incluirICMSBase ? percentualICMSIncluirNoFrete : 0;
            valorBaseCalculoICMS += incluirICMSBase ? (percentualAliquota > 0 || aliquotaPISCOFINS > 0 ? ((valorBaseCalculoICMS / ((100 - percentualAliquota - aliquotaPISCOFINS) / 100)) - valorBaseCalculoICMS) : 0) : 0;
            decimal valorICMS = valorBaseCalculoICMS * (percentualAliquota / 100);
            valorBaseCalculoICMS = decimal.Round(valorBaseCalculoICMS, 2, MidpointRounding.AwayFromZero);
            return valorICMS;
        }

        private void ObterBaseCalculo(ref decimal valorBaseCalculo, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubcontratacao, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete> pedidoCtesParaSubContratacaoComponentesFrete)
        {
            Servicos.Embarcador.Carga.ICMS servicoIcms = new Servicos.Embarcador.Carga.ICMS(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete repPedidoXMLNotaFiscalComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete(unidadeTrabalho);

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete pedidoCteParaSubContratacaoComponenteFrete in pedidoCtesParaSubContratacaoComponentesFrete)
                valorBaseCalculo += servicoIcms.ObterValorIcmsComponenteFrete(pedidoCteParaSubContratacaoComponenteFrete, carga.Empresa, cargaPedido.Origem.Estado.Sigla, unidadeTrabalho, tipoServicoMultisoftware);
        }

        public void CalcularImpostos(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Rateio.RateioFormula formulaRateio, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidoCTesParaSubContratacao, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete> pedidoCTeParaSubContratacaoComponentesFrete, decimal valorBaseCalculo, bool incluirICMSBase, decimal percentualIncluiFrete, string descricaoItemPeso, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao> cargaPedidoContaContabilContabilizacaosCarga, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteICMS, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componentePisCofins, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> cargapedidoCTeParaSubContratacaoNotasFiscais, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> componentesICMSXMLNotaFiscalExistenteCarga, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> componentesPisConfisXMLNotaFiscalExistenteCarga)
        {
            Servicos.Embarcador.Carga.CargaPedido svcCargaPedido = new CargaPedido(unitOfWork);
            Servicos.Embarcador.Carga.CTeSubContratacao svcCTeSubContratacao = new Servicos.Embarcador.Carga.CTeSubContratacao(unitOfWork);
            Servicos.Embarcador.Carga.ICMS svcICMS = new ICMS(unitOfWork);
            Servicos.Embarcador.Carga.ISS svcISS = new Servicos.Embarcador.Carga.ISS(unitOfWork);
            Servicos.Embarcador.Carga.RateioFormula svcRateio = new RateioFormula(unitOfWork);
            Servicos.Embarcador.Pedido.PedidoCTeParaSubContratacao servicoPedidoCTeParaSubContratacao = new Servicos.Embarcador.Pedido.PedidoCTeParaSubContratacao(unitOfWork);
            Servicos.Embarcador.Imposto.ImpostoIBSCBS servicoImpostoIBSCBS = new Servicos.Embarcador.Imposto.ImpostoIBSCBS(unitOfWork);

            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete repPedidoCTeParaSubContratacaoComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Rateio.RateioFormula repRateioFormula = new Repositorio.Embarcador.Rateio.RateioFormula(unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubcontratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao primeiroPedidoCTeParaSubcontratacao = pedidoCTesParaSubContratacao.FirstOrDefault();

            Dominio.Entidades.Localidade inicioPrestacao = primeiroPedidoCTeParaSubcontratacao.CTeTerceiro.LocalidadeInicioPrestacao;
            Dominio.Entidades.Localidade terminoPrestacao = primeiroPedidoCTeParaSubcontratacao.CTeTerceiro.LocalidadeTerminoPrestacao;

            Dominio.Entidades.Cliente remetente = primeiroPedidoCTeParaSubcontratacao.CTeTerceiro.Remetente.Cliente;
            Dominio.Entidades.Cliente destinatario = primeiroPedidoCTeParaSubcontratacao.CTeTerceiro.Destinatario.Cliente;

            Dominio.Entidades.Cliente tomador = primeiroPedidoCTeParaSubcontratacao.CTeTerceiro.Emitente.Cliente;

            if (tomador == null)
                throw new Exception("O tomador do CT-e para subcontratação " + primeiroPedidoCTeParaSubcontratacao.CTeTerceiro.Numero + " da carga " + carga.CodigoCargaEmbarcador + " é nulo.");

            if (primeiroPedidoCTeParaSubcontratacao.CTeTerceiro.Expedidor != null)
            {
                inicioPrestacao = primeiroPedidoCTeParaSubcontratacao.CTeTerceiro.Expedidor.Cliente.Localidade;
                remetente = primeiroPedidoCTeParaSubcontratacao.CTeTerceiro.Expedidor.Cliente;
            }
            else if (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor && cargaPedido.Expedidor != null)
            {
                inicioPrestacao = cargaPedido.Expedidor.Localidade;
                remetente = cargaPedido.Expedidor;
            }

            if (primeiroPedidoCTeParaSubcontratacao.CTeTerceiro.Recebedor != null)
            {
                terminoPrestacao = primeiroPedidoCTeParaSubcontratacao.CTeTerceiro.Recebedor.Cliente.Localidade;
                destinatario = primeiroPedidoCTeParaSubcontratacao.CTeTerceiro.Recebedor.Cliente;
            }
            else if ((cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor) && cargaPedido.Recebedor != null && !(cargaPedido.Carga.TipoOperacao?.UtilizarRecebedorApenasComoParticipante ?? false))
            {
                terminoPrestacao = cargaPedido.Recebedor.Localidade;
                destinatario = cargaPedido.Recebedor;
            }

            svcCargaPedido.VerificarQuaisDocumentosDeveEmitir(carga, cargaPedido, remetente.Localidade, destinatario.Localidade, tipoServicoMultisoftware, unitOfWork, out bool possuiCTe, out bool possuiNFS, out bool possuiNFSManual, out Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscalIntramunicipal, configuracao, out bool sempreDisponibilizarDocumentoNFSManual);

            if (formulaRateio != null)
                formulaRateio = repRateioFormula.BuscarPorCodigo(formulaRateio.Codigo);

            int totalCTes = pedidoCTesParaSubContratacao.Count;
            int numeroNotasRateio = pedidoCTesParaSubContratacao.Sum(obj => obj.CTeTerceiro.NumeroTotalDocumentos);
            decimal pesoTotal = pedidoCTesParaSubContratacao.Sum(obj => obj.CTeTerceiro.Peso /*svcCTeSubContratacao.ObterPesoDaSubContratacao(obj, descricaoItemPeso, unitOfWork)*/);
            decimal valorTotalNF = pedidoCTesParaSubContratacao.Sum(obj => obj.CTeTerceiro.ValorTotalMercadoria);
            decimal valorTotalCTe = pedidoCTesParaSubContratacao.Sum(obj => obj.CTeTerceiro.ValorAReceber);
            decimal baseCalculoIBSCBS = valorBaseCalculo;

            decimal totalBaseCalculoICMSRateada = 0m,
                    totalCreditoPresumidoRateado = 0m,
                    totalBaseCalculoISSRateada = 0m,
                    totalICMSRateado = 0m,
                    totalICMSInclusoRateado = 0m,
                    totalISSRateado = 0m,
                    totalRetencaoISSRateado = 0m,
                    totalPisRateado = 0m,
                    totalCofinsRateado = 0m,
                    totalBaseCalculoIBSCBSRateado = 0m,
                    totalValorIBSEstadualRateado = 0m,
                    totalValorIBSMunicipalRateado = 0m,
                    totalValorCBSRateado = 0m;

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete> pedidoCTeParaSubcontratacaoCompontesFreteAgrupada = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete>();
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete> cargaPedidosCTeParaSubcontratacaoComponentesFrete = repPedidoCTeParaSubContratacaoComponenteFrete.BuscarTodosdoCargaPedido(cargaPedido.Codigo, false);

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao in pedidoCTesParaSubContratacao)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete> pedidoCTeParaSubcontratacaoComponentesFrete = (from obj in cargaPedidosCTeParaSubcontratacaoComponentesFrete where obj.PedidoCTeParaSubContratacao.Codigo == pedidoCTeParaSubContratacao.Codigo && (obj.ModeloDocumentoFiscal == null || obj.ModeloDocumentoFiscal.Numero == "57") select obj).ToList();
                ObterBaseCalculo(ref valorBaseCalculo, carga, cargaPedido, pedidoCTeParaSubContratacao, unitOfWork, tipoServicoMultisoftware, pedidoCTeParaSubcontratacaoComponentesFrete);

                pedidoCTeParaSubcontratacaoCompontesFreteAgrupada.AddRange(pedidoCTeParaSubcontratacaoComponentesFrete);
            }

            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = null;
            Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS = null;
            Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = null;
            Dominio.Entidades.Empresa empresa = carga.Empresa;

            if (possuiNFS || possuiNFSManual)
            {
                regraISS = svcISS.BuscarRegraISS(carga.Empresa, valorBaseCalculo, destinatario.Localidade, carga.TipoOperacao, tomador, null, carga?.TipoDeCarga?.NBS ?? "", unitOfWork);

                if (regraISS != null)
                {
                    regraISS.ValorISS = Math.Round(regraISS.ValorISS, 2, MidpointRounding.AwayFromZero);
                    regraISS.ValorRetencaoISS = Math.Round(regraISS.ValorRetencaoISS, 2, MidpointRounding.AwayFromZero);
                    regraISS.ValorIR = Math.Round(regraISS.ValorIR, 2, MidpointRounding.AwayFromZero);

                    if (regraISS.IncluirISSBaseCalculo)
                        baseCalculoIBSCBS += regraISS.ValorISS;

                    impostoIBSCBS = servicoImpostoIBSCBS.ObterImpostoIBSCBSComValoresArredondados(new Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBS
                    {
                        BaseCalculo = baseCalculoIBSCBS,
                        ValoAbaterBaseCalculo = regraISS.ValorISS,
                        CodigoLocalidade = destinatario.Localidade.Codigo,
                        SiglaUF = destinatario.Localidade.Estado.Sigla,
                        CodigoTipoOperacao = carga.TipoOperacao?.Codigo ?? 0,
                        Empresa = empresa
                    });
                }

            }

            if (possuiCTe)
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga tipoContratacao = cargaPedido.TipoContratacaoCarga;

                regraICMS = svcICMS.BuscarRegraICMS(carga, cargaPedido, empresa, remetente, destinatario, tomador, remetente.Localidade, destinatario.Localidade, ref incluirICMSBase, ref percentualIncluiFrete, valorBaseCalculo, null, unitOfWork, tipoServicoMultisoftware, configuracao, tipoContratacao);
                regraICMS.ValorICMS = Math.Round(regraICMS.ValorICMS, 2, MidpointRounding.AwayFromZero);
                regraICMS.ValorPis = Math.Round(regraICMS.ValorPis, 2, MidpointRounding.AwayFromZero);
                regraICMS.ValorCofins = Math.Round(regraICMS.ValorCofins, 2, MidpointRounding.AwayFromZero);
                regraICMS.ValorICMSIncluso = Math.Round(regraICMS.ValorICMSIncluso, 2, MidpointRounding.AwayFromZero);
                regraICMS.ValorCreditoPresumido = Math.Round(regraICMS.ValorCreditoPresumido, 2, MidpointRounding.AwayFromZero);

                if (!incluirICMSBase)
                    baseCalculoIBSCBS -= Math.Round(regraICMS.ValorICMS, 2, MidpointRounding.AwayFromZero);

                impostoIBSCBS = servicoImpostoIBSCBS.ObterImpostoIBSCBSComValoresArredondados(new Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBS { BaseCalculo = baseCalculoIBSCBS, CodigoLocalidade = destinatario.Localidade.Codigo, SiglaUF = destinatario.Localidade.Estado.Sigla, CodigoTipoOperacao = carga.TipoOperacao?.Codigo ?? 0, Empresa = empresa });
            }

            for (int i = 0; i < totalCTes; i++)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = pedidoCTesParaSubContratacao[i];

                decimal peso = pedidoCTeParaSubContratacao.CTeTerceiro.Peso; //svcCTeSubContratacao.ObterPesoDaSubContratacao(pedidoCTeParaSubContratacao, descricaoItemPeso, unitOfWork);

                if (possuiCTe)
                {
                    decimal valorBaseCalculoICMS = regraICMS.ValorBaseCalculoICMS;
                    decimal valorICMS = regraICMS.ValorICMS;
                    decimal valorPis = regraICMS.ValorPis;
                    decimal valorCofins = regraICMS.ValorCofins;
                    decimal valorICMSIncluso = regraICMS.ValorICMSIncluso;
                    decimal valorCreditoPresumido = regraICMS.ValorCreditoPresumido;
                    decimal valorRateioOriginal = 0;

                    decimal valorbaseCalculoIBSCBS = impostoIBSCBS.BaseCalculo;
                    decimal valorIBSEstadual = impostoIBSCBS.ValorIBSEstadual;
                    decimal valorIBSMunicipal = impostoIBSCBS.ValorIBSMunicipal;
                    decimal valorCBS = impostoIBSCBS.ValorCBS;

                    if (formulaRateio != null && formulaRateio.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porCTe)
                    {
                        valorBaseCalculoICMS = regraICMS.ValorBaseCalculoICMS / totalCTes;
                        valorICMS = regraICMS.ValorICMS / totalCTes;
                        valorICMSIncluso = regraICMS.ValorICMSIncluso / totalCTes;
                        valorCreditoPresumido = regraICMS.ValorCreditoPresumido / totalCTes;
                        valorPis = regraICMS.ValorPis / totalCTes;
                        valorCofins = regraICMS.ValorCofins / totalCTes;

                        valorbaseCalculoIBSCBS = impostoIBSCBS.BaseCalculo / totalCTes;
                        valorIBSEstadual = impostoIBSCBS.ValorIBSEstadual / totalCTes;
                        valorIBSMunicipal = impostoIBSCBS.ValorIBSMunicipal / totalCTes;
                        valorCBS = impostoIBSCBS.ValorCBS / totalCTes;
                    }

                    decimal baseCalculoRateada = svcRateio.AplicarFormulaRateio(formulaRateio, valorBaseCalculoICMS, numeroNotasRateio, totalCTes, pesoTotal, peso, pedidoCTeParaSubContratacao.CTeTerceiro.ValorTotalMercadoria, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, pedidoCTeParaSubContratacao.CTeTerceiro.ValorAReceber, 0, valorTotalCTe);
                    decimal icmsRateado = svcRateio.AplicarFormulaRateio(formulaRateio, valorICMS, numeroNotasRateio, totalCTes, pesoTotal, peso, pedidoCTeParaSubContratacao.CTeTerceiro.ValorTotalMercadoria, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, pedidoCTeParaSubContratacao.CTeTerceiro.ValorAReceber, 0, valorTotalCTe);
                    decimal icmsInclusoRateado = svcRateio.AplicarFormulaRateio(formulaRateio, valorICMSIncluso, numeroNotasRateio, totalCTes, pesoTotal, peso, pedidoCTeParaSubContratacao.CTeTerceiro.ValorTotalMercadoria, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, pedidoCTeParaSubContratacao.CTeTerceiro.ValorAReceber, 0, valorTotalCTe);
                    decimal valorCreditoPresumidoRateado = svcRateio.AplicarFormulaRateio(formulaRateio, valorCreditoPresumido, numeroNotasRateio, totalCTes, pesoTotal, peso, pedidoCTeParaSubContratacao.CTeTerceiro.ValorTotalMercadoria, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, pedidoCTeParaSubContratacao.CTeTerceiro.ValorAReceber, 0, valorTotalCTe);
                    decimal pisRateada = svcRateio.AplicarFormulaRateio(formulaRateio, valorPis, numeroNotasRateio, totalCTes, pesoTotal, peso, pedidoCTeParaSubContratacao.CTeTerceiro.ValorTotalMercadoria, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, pedidoCTeParaSubContratacao.CTeTerceiro.ValorAReceber, 0, valorTotalCTe);
                    decimal cofinsRateado = svcRateio.AplicarFormulaRateio(formulaRateio, valorCofins, numeroNotasRateio, totalCTes, pesoTotal, peso, pedidoCTeParaSubContratacao.CTeTerceiro.ValorTotalMercadoria, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, pedidoCTeParaSubContratacao.CTeTerceiro.ValorAReceber, 0, valorTotalCTe);

                    decimal valorbaseCalculoIBSCBSRateado = svcRateio.AplicarFormulaRateio(formulaRateio, valorbaseCalculoIBSCBS, numeroNotasRateio, totalCTes, pesoTotal, peso, pedidoCTeParaSubContratacao.CTeTerceiro.ValorTotalMercadoria, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, pedidoCTeParaSubContratacao.CTeTerceiro.ValorAReceber, 0, valorTotalCTe);
                    decimal valorIBSEstadualRateado = svcRateio.AplicarFormulaRateio(formulaRateio, valorIBSEstadual, numeroNotasRateio, totalCTes, pesoTotal, peso, pedidoCTeParaSubContratacao.CTeTerceiro.ValorTotalMercadoria, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, pedidoCTeParaSubContratacao.CTeTerceiro.ValorAReceber, 0, valorTotalCTe);
                    decimal valorIBSMunicipalRateado = svcRateio.AplicarFormulaRateio(formulaRateio, valorIBSMunicipal, numeroNotasRateio, totalCTes, pesoTotal, peso, pedidoCTeParaSubContratacao.CTeTerceiro.ValorTotalMercadoria, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, pedidoCTeParaSubContratacao.CTeTerceiro.ValorAReceber, 0, valorTotalCTe);
                    decimal valorCBSRateado = svcRateio.AplicarFormulaRateio(formulaRateio, valorCBS, numeroNotasRateio, totalCTes, pesoTotal, peso, pedidoCTeParaSubContratacao.CTeTerceiro.ValorTotalMercadoria, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, pedidoCTeParaSubContratacao.CTeTerceiro.ValorAReceber, 0, valorTotalCTe);

                    if (i == totalCTes - 1)
                    {
                        pisRateada = regraICMS.ValorPis - totalPisRateado;
                        valorCofins = regraICMS.ValorCofins - totalCofinsRateado;
                        icmsRateado = regraICMS.ValorICMS - totalICMSRateado;
                        icmsInclusoRateado = regraICMS.ValorICMSIncluso - totalICMSInclusoRateado;
                        baseCalculoRateada = regraICMS.ValorBaseCalculoICMS - totalBaseCalculoICMSRateada;
                        valorCreditoPresumidoRateado = regraICMS.ValorCreditoPresumido - totalCreditoPresumidoRateado;

                        if (!configuracao.DesconsiderarSobraRateioParaBaseCalculoIBSCBS)
                        {
                            valorbaseCalculoIBSCBSRateado = impostoIBSCBS.BaseCalculo - totalBaseCalculoIBSCBSRateado;
                            valorIBSEstadualRateado = impostoIBSCBS.ValorIBSEstadual - totalValorIBSEstadualRateado;
                            valorIBSMunicipalRateado = impostoIBSCBS.ValorIBSMunicipal - totalValorIBSMunicipalRateado;
                            valorCBSRateado = impostoIBSCBS.ValorCBS - totalValorCBSRateado;
                        }
                    }

                    totalPisRateado += pisRateada;
                    totalCofinsRateado += valorCofins;
                    totalICMSRateado += icmsRateado;
                    totalICMSInclusoRateado += icmsInclusoRateado;
                    totalBaseCalculoICMSRateada += baseCalculoRateada;
                    totalCreditoPresumidoRateado += valorCreditoPresumidoRateado;

                    totalBaseCalculoIBSCBSRateado += valorbaseCalculoIBSCBSRateado;
                    totalValorIBSEstadualRateado += valorIBSEstadualRateado;
                    totalValorIBSMunicipalRateado += valorIBSMunicipalRateado;
                    totalValorCBSRateado += valorCBSRateado;

                    pedidoCTeParaSubContratacao.CFOP = repCFOP.BuscarPorNumero(regraICMS.CFOP);
                    pedidoCTeParaSubContratacao.CST = regraICMS.CST;
                    pedidoCTeParaSubContratacao.IncluirICMSBaseCalculo = incluirICMSBase;
                    pedidoCTeParaSubContratacao.PercentualIncluirBaseCalculo = percentualIncluiFrete;
                    pedidoCTeParaSubContratacao.PercentualReducaoBC = regraICMS.PercentualReducaoBC;
                    pedidoCTeParaSubContratacao.ObservacaoRegraICMSCTe = regraICMS.ObservacaoCTe;
                    pedidoCTeParaSubContratacao.PercentualAliquota = regraICMS.Aliquota;
                    pedidoCTeParaSubContratacao.AliquotaPis = regraICMS.AliquotaPis;
                    pedidoCTeParaSubContratacao.AliquotaCofins = regraICMS.AliquotaCofins;
                    pedidoCTeParaSubContratacao.PercentualAliquotaInternaDifal = regraICMS.AliquotaInternaDifal;
                    pedidoCTeParaSubContratacao.BaseCalculoICMS = baseCalculoRateada;
                    pedidoCTeParaSubContratacao.SetarRegraICMS(regraICMS.CodigoRegra);

                    pedidoCTeParaSubContratacao.PossuiCTe = true;
                    pedidoCTeParaSubContratacao.PossuiNFS = false;
                    pedidoCTeParaSubContratacao.PossuiNFSManual = false;

                    if (pedidoCTeParaSubContratacao.ModeloDocumentoFiscal == null ||
                        pedidoCTeParaSubContratacao.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe ||
                        pedidoCTeParaSubContratacao.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)
                        pedidoCTeParaSubContratacao.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.CTe);

                    //if (pedidoCTeParaSubContratacao.CST == "60")
                    //    pedidoCTeParaSubContratacao.ICMSPagoPorST = true;

                    pedidoCTeParaSubContratacao.ValorCofins = cofinsRateado;
                    pedidoCTeParaSubContratacao.ValorPis = pisRateada;
                    pedidoCTeParaSubContratacao.ValorICMS = icmsRateado;
                    pedidoCTeParaSubContratacao.ValorICMSIncluso = icmsInclusoRateado;

                    servicoPedidoCTeParaSubContratacao.PreencherCamposImpostoIBSCBS(pedidoCTeParaSubContratacao, impostoIBSCBS);
                    servicoPedidoCTeParaSubContratacao.PreencherValoresImpostoIBSCBS(pedidoCTeParaSubContratacao, valorbaseCalculoIBSCBSRateado, valorIBSEstadualRateado, valorIBSMunicipalRateado, valorCBSRateado);

                    //GerarComponenteICMS(pedidoCTeParaSubContratacao, unitOfWork);
                }
                else
                {
                    pedidoCTeParaSubContratacao.IncluirICMSBaseCalculo = false;
                    pedidoCTeParaSubContratacao.PercentualIncluirBaseCalculo = 0m;
                    pedidoCTeParaSubContratacao.ObservacaoRegraICMSCTe = "";
                    pedidoCTeParaSubContratacao.BaseCalculoICMS = 0m;
                    pedidoCTeParaSubContratacao.PercentualReducaoBC = 0m;
                    pedidoCTeParaSubContratacao.PercentualAliquota = 0m;
                    pedidoCTeParaSubContratacao.AliquotaPis = 0m;
                    pedidoCTeParaSubContratacao.AliquotaCofins = 0m;
                    pedidoCTeParaSubContratacao.PercentualAliquotaInternaDifal = 0m;
                    pedidoCTeParaSubContratacao.ValorICMS = 0m;
                    pedidoCTeParaSubContratacao.ValorICMSIncluso = 0m;
                    //pedidoCTeParaSubContratacao.ValorCreditoPresumido = 0m;
                    //pedidoCTeParaSubContratacao.PercentualCreditoPresumido = 0m;
                    pedidoCTeParaSubContratacao.PossuiCTe = false;

                    servicoPedidoCTeParaSubContratacao.ZerarCamposImpostoIBSCBS(pedidoCTeParaSubContratacao);

                    if (pedidoCTeParaSubContratacao.ModeloDocumentoFiscal != null && pedidoCTeParaSubContratacao.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                        pedidoCTeParaSubContratacao.ModeloDocumentoFiscal = cargaPedido.ModeloDocumentoFiscal;
                }

                if (possuiNFS || possuiNFSManual)
                {
                    if (regraISS != null)
                    {
                        decimal valorRateioOriginal = 0;
                        decimal baseCalculoRateada = Math.Round(svcRateio.AplicarFormulaRateio(formulaRateio, regraISS.ValorBaseCalculoISS, numeroNotasRateio, totalCTes, pesoTotal, peso, pedidoCTeParaSubContratacao.CTeTerceiro.ValorTotalMercadoria, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, pedidoCTeParaSubContratacao.CTeTerceiro.ValorAReceber, 0, valorTotalCTe), 2, MidpointRounding.AwayFromZero);
                        decimal baseCalculoIBSCBSRateada = Math.Round(svcRateio.AplicarFormulaRateio(formulaRateio, impostoIBSCBS.BaseCalculo, numeroNotasRateio, totalCTes, pesoTotal, peso, pedidoCTeParaSubContratacao.CTeTerceiro.ValorTotalMercadoria, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, pedidoCTeParaSubContratacao.CTeTerceiro.ValorAReceber, 0, valorTotalCTe), 2, MidpointRounding.AwayFromZero);
                        decimal valorCBSRateado = Math.Round(svcRateio.AplicarFormulaRateio(formulaRateio, impostoIBSCBS.ValorCBS, numeroNotasRateio, totalCTes, pesoTotal, peso, pedidoCTeParaSubContratacao.CTeTerceiro.ValorTotalMercadoria, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, pedidoCTeParaSubContratacao.CTeTerceiro.ValorAReceber, 0, valorTotalCTe), 2, MidpointRounding.AwayFromZero);
                        decimal valorIBSMunicipalRateado = Math.Round(svcRateio.AplicarFormulaRateio(formulaRateio, impostoIBSCBS.ValorIBSMunicipal, numeroNotasRateio, totalCTes, pesoTotal, peso, pedidoCTeParaSubContratacao.CTeTerceiro.ValorTotalMercadoria, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, pedidoCTeParaSubContratacao.CTeTerceiro.ValorAReceber, 0, valorTotalCTe), 2, MidpointRounding.AwayFromZero);
                        decimal valorIBSEstadualRateado = Math.Round(svcRateio.AplicarFormulaRateio(formulaRateio, impostoIBSCBS.ValorIBSEstadual, numeroNotasRateio, totalCTes, pesoTotal, peso, pedidoCTeParaSubContratacao.CTeTerceiro.ValorTotalMercadoria, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, pedidoCTeParaSubContratacao.CTeTerceiro.ValorAReceber, 0, valorTotalCTe), 2, MidpointRounding.AwayFromZero);
                        decimal issRateado = Math.Round(svcRateio.AplicarFormulaRateio(formulaRateio, regraISS.ValorISS, numeroNotasRateio, totalCTes, pesoTotal, peso, pedidoCTeParaSubContratacao.CTeTerceiro.ValorTotalMercadoria, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, pedidoCTeParaSubContratacao.CTeTerceiro.ValorAReceber, 0, valorTotalCTe), 2, MidpointRounding.AwayFromZero);
                        decimal retencaoISSRateado = Math.Round(svcRateio.AplicarFormulaRateio(formulaRateio, regraISS.ValorRetencaoISS, numeroNotasRateio, totalCTes, pesoTotal, peso, pedidoCTeParaSubContratacao.CTeTerceiro.ValorTotalMercadoria, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, pedidoCTeParaSubContratacao.CTeTerceiro.ValorAReceber, 0, valorTotalCTe), 2, MidpointRounding.AwayFromZero);

                        if (i == totalCTes - 1)
                        {
                            issRateado = regraISS.ValorISS - totalISSRateado;
                            retencaoISSRateado = regraISS.ValorRetencaoISS - totalRetencaoISSRateado;
                            baseCalculoRateada = regraISS.ValorBaseCalculoISS - totalBaseCalculoISSRateada;
                            baseCalculoIBSCBSRateada = impostoIBSCBS.BaseCalculo - totalBaseCalculoIBSCBSRateado;
                            valorIBSMunicipalRateado = impostoIBSCBS.ValorIBSMunicipal - totalValorIBSMunicipalRateado;
                            valorIBSEstadualRateado = impostoIBSCBS.ValorIBSEstadual - totalValorIBSEstadualRateado;
                            valorCBSRateado = impostoIBSCBS.ValorCBS - totalValorCBSRateado;
                        }

                        totalISSRateado += issRateado;
                        totalRetencaoISSRateado += retencaoISSRateado;
                        totalBaseCalculoISSRateada += baseCalculoRateada;
                        totalBaseCalculoIBSCBSRateado += baseCalculoIBSCBSRateada;
                        totalValorCBSRateado += valorCBSRateado;
                        totalValorIBSEstadualRateado += valorIBSEstadualRateado;
                        totalValorIBSMunicipalRateado += valorIBSMunicipalRateado;

                        pedidoCTeParaSubContratacao.ValorISS = issRateado;
                        pedidoCTeParaSubContratacao.BaseCalculoISS = baseCalculoRateada;
                        pedidoCTeParaSubContratacao.PercentualAliquotaISS = regraISS.AliquotaISS;
                        pedidoCTeParaSubContratacao.PercentualRetencaoISS = regraISS.PercentualRetencaoISS;
                        pedidoCTeParaSubContratacao.IncluirISSBaseCalculo = regraISS.IncluirISSBaseCalculo;
                        pedidoCTeParaSubContratacao.ValorRetencaoISS = retencaoISSRateado;

                        servicoPedidoCTeParaSubContratacao.PreencherCamposImpostoIBSCBS(pedidoCTeParaSubContratacao, impostoIBSCBS);
                        servicoPedidoCTeParaSubContratacao.PreencherValoresImpostoIBSCBS(pedidoCTeParaSubContratacao, baseCalculoIBSCBS, valorIBSEstadualRateado, valorIBSMunicipalRateado, valorCBSRateado);

                    }

                    pedidoCTeParaSubContratacao.PossuiCTe = false;

                    if (possuiNFS)
                    {
                        pedidoCTeParaSubContratacao.PossuiNFS = true;
                        pedidoCTeParaSubContratacao.PossuiNFSManual = false;

                        if (pedidoCTeParaSubContratacao.ModeloDocumentoFiscal == null || pedidoCTeParaSubContratacao.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                            pedidoCTeParaSubContratacao.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.NFSe);
                    }
                    else
                    {
                        pedidoCTeParaSubContratacao.PossuiNFS = false;
                        pedidoCTeParaSubContratacao.PossuiNFSManual = true;
                        pedidoCTeParaSubContratacao.ModeloDocumentoFiscal = null;
                    }

                    GerarComponenteISS(pedidoCTeParaSubContratacao, unitOfWork);
                }
                else
                {
                    pedidoCTeParaSubContratacao.ValorISS = 0;
                    pedidoCTeParaSubContratacao.BaseCalculoISS = 0;
                    pedidoCTeParaSubContratacao.PercentualAliquotaISS = 0;
                    pedidoCTeParaSubContratacao.PercentualRetencaoISS = 0;
                    pedidoCTeParaSubContratacao.IncluirISSBaseCalculo = false;
                    pedidoCTeParaSubContratacao.ValorRetencaoISS = 0;
                    pedidoCTeParaSubContratacao.PossuiNFS = false;
                    pedidoCTeParaSubContratacao.PossuiNFSManual = false;

                    if (pedidoCTeParaSubContratacao.ModeloDocumentoFiscal != null && pedidoCTeParaSubContratacao.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                        pedidoCTeParaSubContratacao.ModeloDocumentoFiscal = cargaPedido.ModeloDocumentoFiscal;
                }

                repPedidoCTeParaSubcontratacao.Atualizar(pedidoCTeParaSubContratacao);
                pedidoCTeParaSubContratacao.ValorTotalComponentes = 0;

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete> pedidoCTeParaSubcontratacaoComponentesFrete = (from obj in pedidoCTeParaSubcontratacaoCompontesFreteAgrupada where obj.PedidoCTeParaSubContratacao.Codigo == pedidoCTeParaSubContratacao.Codigo select obj).ToList();
                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete pedidoCTeParaSubcontratacaoComponenteFrete in pedidoCTeParaSubcontratacaoComponentesFrete)
                {
                    if (pedidoCTeParaSubcontratacaoComponenteFrete.ModeloDocumentoFiscal == null || pedidoCTeParaSubcontratacaoComponenteFrete.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe || pedidoCTeParaSubcontratacaoComponenteFrete.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                    {
                        pedidoCTeParaSubcontratacaoComponenteFrete.ModeloDocumentoFiscal = pedidoCTeParaSubContratacao.ModeloDocumentoFiscal;

                        repPedidoCTeParaSubContratacaoComponenteFrete.Atualizar(pedidoCTeParaSubcontratacaoComponenteFrete);
                    }

                    pedidoCTeParaSubContratacao.ValorTotalComponentes += pedidoCTeParaSubcontratacaoComponenteFrete.ValorComponente;
                }


                if (pedidoCTeParaSubContratacao.CTeTerceiro.ObterCargaCTe(carga.Codigo) == null) //apenas por CT-e anterior se não foi criado por filial emissora, senão gera por nota.
                    Servicos.Embarcador.Escrituracao.DocumentoProvisao.AdicionarDocumentoParaProvisao(pedidoCTeParaSubContratacao, false, tipoServicoMultisoftware, unitOfWork);

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoContaContabilContabilizacao> pedidoCTeParaSubContratacaoContaContabilContabilizacaos = InformarDadosContabeisCTeSubContratacao(pedidoCTeParaSubContratacao, true, cargaPedido, cargaPedidoContaContabilContabilizacaosCarga, configuracao, tipoServicoMultisoftware, unitOfWork);

                RatearValorFreteCTeSubcontratacaoEntreNotasDoCTeDeSubcontratacao(cargaPedido, pedidoCTeParaSubContratacao, pedidoCTeParaSubcontratacaoComponentesFrete, formulaRateio, peso, pedidoCTeParaSubContratacao.CTeTerceiro.ValorTotalMercadoria, pedidoCTeParaSubContratacaoContaContabilContabilizacaos, tipoServicoMultisoftware, unitOfWork, componenteICMS, componentePisCofins, cargapedidoCTeParaSubContratacaoNotasFiscais, componentesICMSXMLNotaFiscalExistenteCarga, componentesPisConfisXMLNotaFiscalExistenteCarga);

                pedidoCTesParaSubContratacao[i] = pedidoCTeParaSubContratacao;
            }

            if (possuiCTe)
            {
                cargaPedido.PercentualAliquota = regraICMS.Aliquota;
                cargaPedido.AliquotaPis = regraICMS.AliquotaPis;
                cargaPedido.AliquotaCofins = regraICMS.AliquotaCofins;
                cargaPedido.PercentualAliquotaInternaDifal = regraICMS.AliquotaInternaDifal;
                cargaPedido.CST = regraICMS.CST;
                cargaPedido.PercentualReducaoBC = regraICMS.PercentualReducaoBC;
                cargaPedido.ObservacaoRegraICMSCTe = regraICMS.ObservacaoCTe;
                cargaPedido.PercentualAliquota = regraICMS.Aliquota;
                cargaPedido.BaseCalculoICMS = regraICMS.ValorBaseCalculoICMS;
                cargaPedido.ValorCreditoPresumido = regraICMS.ValorCreditoPresumido;
                cargaPedido.PercentualCreditoPresumido = regraICMS.PercentualCreditoPresumido;
                cargaPedido.PossuiCTe = true;
                cargaPedido.DescontarICMSDoValorAReceber = regraICMS.DescontarICMSDoValorAReceber;
                cargaPedido.NaoReduzirRetencaoICMSDoValorDaPrestacao = regraICMS.NaoReduzirRetencaoICMSDoValorDaPrestacao;
                cargaPedido.NaoImprimirImpostosDACTE = regraICMS.NaoImprimirImpostosDACTE;
                cargaPedido.NaoEnviarImpostoICMSNaEmissaoCte = regraICMS.NaoEnviarImpostoICMSNaEmissaoCte;

                svcCargaPedido.PreencherCamposImpostoIBSCBS(cargaPedido, impostoIBSCBS, true);
            }

            if (possuiNFS)
                cargaPedido.PossuiNFS = true;

            if (possuiNFSManual)
            {
                cargaPedido.PossuiNFSManual = true;
                cargaPedido.ModeloDocumentoFiscalIntramunicipal = modeloDocumentoFiscalIntramunicipal;
            }

            if ((possuiNFS || possuiNFSManual) && regraISS != null)
            {
                cargaPedido.PercentualAliquotaISS = regraISS.AliquotaISS;
                cargaPedido.PercentualRetencaoISS = regraISS.PercentualRetencaoISS;
                cargaPedido.IncluirISSBaseCalculo = regraISS.IncluirISSBaseCalculo;

                svcCargaPedido.PreencherCamposImpostoIBSCBS(cargaPedido, impostoIBSCBS, true);

            }
        }


        private void GerarComponenteISS(Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete repPedidoCteParaSubContratacaoComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete(unitOfWork);

            Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS);
            Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete pedidoCteParaSubContratacaoComponenteFrete = repPedidoCteParaSubContratacaoComponenteFrete.BuscarPorPedidoCteParaSubContratacaoETipo(pedidoCTeParaSubContratacao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS, null);

            if (pedidoCTeParaSubContratacao.IncluirISSBaseCalculo && pedidoCTeParaSubContratacao.ValorISS > 0m)
            {
                if (pedidoCteParaSubContratacaoComponenteFrete == null)
                    pedidoCteParaSubContratacaoComponenteFrete = new Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete();

                pedidoCteParaSubContratacaoComponenteFrete.NaoSomarValorTotalAReceber = false;
                pedidoCteParaSubContratacaoComponenteFrete.NaoSomarValorTotalPrestacao = false;
                pedidoCteParaSubContratacaoComponenteFrete.AcrescentaValorTotalAReceber = false;
                pedidoCteParaSubContratacaoComponenteFrete.PedidoCTeParaSubContratacao = pedidoCTeParaSubContratacao;
                pedidoCteParaSubContratacaoComponenteFrete.ComponenteFrete = componenteFrete;
                pedidoCteParaSubContratacaoComponenteFrete.DescontarValorTotalAReceber = false;
                pedidoCteParaSubContratacaoComponenteFrete.TipoComponenteFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS;
                pedidoCteParaSubContratacaoComponenteFrete.ValorComponente = pedidoCTeParaSubContratacao.ValorISS;


                if (pedidoCteParaSubContratacaoComponenteFrete.Codigo > 0)
                    repPedidoCteParaSubContratacaoComponenteFrete.Atualizar(pedidoCteParaSubContratacaoComponenteFrete);
                else
                    repPedidoCteParaSubContratacaoComponenteFrete.Inserir(pedidoCteParaSubContratacaoComponenteFrete);
            }
            else if (pedidoCteParaSubContratacaoComponenteFrete != null)
            {
                repPedidoCteParaSubContratacaoComponenteFrete.Deletar(pedidoCteParaSubContratacaoComponenteFrete);
            }
        }

        private void RatearValorFreteCTeSubcontratacaoEntreNotasDoCTeDeSubcontratacao(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete> pedidosCteParaSubContratacaoComponentesFrete, Dominio.Entidades.Embarcador.Rateio.RateioFormula formulaRateio, decimal peso, decimal valorTotalMercadoria, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoContaContabilContabilizacao> pedidoCTeParaSubContratacaoContaContabilContabilizacaos, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteICMS, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componentePisCofins, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> cargapedidoCTeParaSubContratacaoNotasFiscais, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> componentesICMSXMLNotaFiscalExistenteCarga, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> componentesPisConfisXMLNotaFiscalExistenteCarga)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);


            Servicos.Embarcador.Carga.RateioFormula svcRateioFormula = new Servicos.Embarcador.Carga.RateioFormula(unitOfWork);
            Servicos.Embarcador.Carga.RateioNotaFiscal svcRateioNotaFiscal = new RateioNotaFiscal(unitOfWork);

            //aqui faz o rateio do valor de cada CT-e de subcontratação entre as notas do CT-e de Subcontratação.
            //Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal repCTeParaSubContratacaoPedidoNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal(unitOfWork);
            //List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> pedidoCTeParaSubContratacaoNotasFiscais = repCTeParaSubContratacaoPedidoNotaFiscal.BuscarPorPedidoCTeParaSubcontratacao(pedidoCTeParaSubContratacao.Codigo);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> pedidoCTeParaSubContratacaoNotasFiscais = (from obj in cargapedidoCTeParaSubContratacaoNotasFiscais where obj.PedidoCTeParaSubContratacao.Codigo == pedidoCTeParaSubContratacao.Codigo select obj).ToList();
            Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal ultimaPedidoCTeParaSubContratacaoPedidoNotaFiscal = pedidoCTeParaSubContratacaoNotasFiscais.LastOrDefault();


            decimal totalFreteCTe = 0m;
            decimal totalBaseCalculoCte = 0m;
            decimal totalICMSFreteCTe = 0m;
            decimal totalICMSInclusoFreteCTe = 0m;
            decimal totalMoedaCargaPedidoNotas = 0m;
            decimal totalPisFreteCTe = 0m;
            decimal totalCofinsFreteCTe = 0m;

            decimal totalBaseCalculoIBSCBSCargaPedidoNotas = 0m;
            decimal totalValorIBSEstadualCargaPedidoNotas = 0m;
            decimal totalValorIBSMunicipalCargaPedidoNotas = 0m;
            decimal totalValorCBSCargaPedidoNotas = 0m;

            List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> componentesDaNota = new List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico>();

            int numeroNotasFiscais = pedidoCTeParaSubContratacaoNotasFiscais.Count;

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moeda = pedidoCTeParaSubContratacao.Moeda ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real;
            decimal cotacaoMoeda = pedidoCTeParaSubContratacao.ValorCotacaoMoeda ?? 0m;
            decimal valorTotalMoeda = pedidoCTeParaSubContratacao.ValorTotalMoeda ?? 0m;

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoPedidoNotaFiscal in pedidoCTeParaSubContratacaoNotasFiscais)
            {
                decimal SomaValorTotalPorNota = 0m;
                decimal SomaValorICMSTotalPorNota = 0m;
                decimal SomaValorICMSInclusoTotalPorNota = 0m;
                decimal SomaBaseCalculoTotalPorNota = 0m;
                decimal somaValorTotalMoedaPorCTe = 0m;
                decimal SomaValorTotalPorCTeAgrupado = 0m;
                decimal valorFreteCTeOriginal = 0m;
                decimal SomaValorPisTotalPorNota = 0m;
                decimal SomaValorCofinsTotalPorNota = 0m;

                decimal SomaBaseCalculoIBSCBSTotalPorCTe = 0m;
                decimal SomaValorIBSEstadualTotalPorCTe = 0m;
                decimal SomaValorIBSMunicipalTotalPorCTe = 0m;
                decimal SomaValorCBSTotalPorCTe = 0m;

                Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXmlNotaFiscal = pedidoCTeParaSubContratacaoPedidoNotaFiscal.PedidoXMLNotaFiscal;

                svcRateioNotaFiscal.AplicarRateioNoPedidoNotaFiscal(pedidoCTeParaSubContratacao.ValorFrete, 0, 1, numeroNotasFiscais, peso, valorTotalMercadoria, numeroNotasFiscais, ref SomaValorTotalPorNota, ref totalFreteCTe, pedidoXmlNotaFiscal, formulaRateio, ultimaPedidoCTeParaSubContratacaoPedidoNotaFiscal.PedidoXMLNotaFiscal, null, null, false, 0m, 0m, 0m, 0, moeda, cotacaoMoeda, valorTotalMoeda, ref somaValorTotalMoedaPorCTe, ref totalMoedaCargaPedidoNotas, ref SomaValorTotalPorCTeAgrupado, ref valorFreteCTeOriginal);

                AplicarRateioICMSNoPedidoNotaFiscal(pedidoCTeParaSubContratacao, 1, numeroNotasFiscais, peso, valorTotalMercadoria, numeroNotasFiscais, ref SomaValorICMSTotalPorNota, ref SomaValorPisTotalPorNota, ref SomaValorCofinsTotalPorNota, ref totalICMSFreteCTe, ref totalPisFreteCTe, ref totalCofinsFreteCTe, ref SomaBaseCalculoTotalPorNota, ref totalBaseCalculoCte, pedidoXmlNotaFiscal, formulaRateio, ultimaPedidoCTeParaSubContratacaoPedidoNotaFiscal.PedidoXMLNotaFiscal, componenteICMS, componentePisCofins, unitOfWork, ref SomaValorICMSInclusoTotalPorNota, ref totalICMSInclusoFreteCTe, componentesICMSXMLNotaFiscalExistenteCarga, componentesPisConfisXMLNotaFiscalExistenteCarga, ref SomaBaseCalculoIBSCBSTotalPorCTe, ref SomaValorIBSEstadualTotalPorCTe, ref SomaValorIBSMunicipalTotalPorCTe, ref SomaValorCBSTotalPorCTe, ref totalBaseCalculoIBSCBSCargaPedidoNotas, ref totalValorIBSEstadualCargaPedidoNotas, ref totalValorIBSMunicipalCargaPedidoNotas, ref totalValorCBSCargaPedidoNotas);

                //usa lista para fazer o armazenamento temporario da soma dos componentes para arredondamento na ultima nota de cada CT-e
                pedidoXmlNotaFiscal.ValorTotalComponentes = 0m;
                pedidoXmlNotaFiscal.ValorTotalMoedaComponentes = 0m;

                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete pedidoCteParaSubContratacaoComponenteFrete in pedidosCteParaSubContratacaoComponentesFrete)
                {
                    Dominio.Entidades.Embarcador.Rateio.RateioFormula formulaComponente = pedidoCteParaSubContratacaoComponenteFrete.RateioFormula != null ? pedidoCteParaSubContratacaoComponenteFrete.RateioFormula : formulaRateio;

                    decimal valorComponente = 0m, valorMoedaComponente = 0m;
                    decimal valorRateioOriginal = 0;

                    if (moeda != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real)
                    {
                        valorMoedaComponente = svcRateioFormula.AplicarFormulaRateio(formulaComponente, pedidoCteParaSubContratacaoComponenteFrete.ValorTotalMoeda ?? 0m, numeroNotasFiscais, 1, peso, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.Valor, valorTotalMercadoria, pedidoCteParaSubContratacaoComponenteFrete.Percentual, pedidoCteParaSubContratacaoComponenteFrete.TipoValor, 0, 0, ref valorRateioOriginal);

                        if (formulaRateio != null && formulaRateio.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porCTe)// quando o rateio é por cte é necessário dividir o valor por nota para chegar ao valor de cada nota fiscal do CT-e. 
                            valorMoedaComponente = Math.Floor(valorMoedaComponente / numeroNotasFiscais * 100) / 100;

                        valorComponente = Math.Round(valorMoedaComponente * cotacaoMoeda, 2, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        valorComponente = svcRateioFormula.AplicarFormulaRateio(formulaComponente, pedidoCteParaSubContratacaoComponenteFrete.ValorComponente, numeroNotasFiscais, 1, peso, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.Valor, valorTotalMercadoria, pedidoCteParaSubContratacaoComponenteFrete.Percentual, pedidoCteParaSubContratacaoComponenteFrete.TipoValor, 0, 0, ref valorRateioOriginal);

                        if (formulaRateio != null && formulaRateio.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porCTe)// quando o rateio é por cte é necessário dividir o valor por nota para chegar ao valor de cada nota fiscal do CT-e. 
                            valorComponente = Math.Floor((valorComponente / numeroNotasFiscais) * 100) / 100;
                    }

                    if (pedidoCTeParaSubContratacaoPedidoNotaFiscal.Equals(ultimaPedidoCTeParaSubContratacaoPedidoNotaFiscal))
                    {
                        if (moeda != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real)
                        {
                            decimal valorTotalMoedaComponente = pedidosCteParaSubContratacaoComponentesFrete.Where(obj => obj.TipoComponenteFrete == pedidoCteParaSubContratacaoComponenteFrete.TipoComponenteFrete && (pedidoCteParaSubContratacaoComponenteFrete.ComponenteFrete == null || obj.ComponenteFrete.Equals(pedidoCteParaSubContratacaoComponenteFrete.ComponenteFrete))).Sum(obj => obj.ValorTotalMoeda ?? 0m);
                            decimal valorTotalMoedaCargaPedidoComponente = componentesDaNota.Where(obj => obj.TipoComponenteFrete == pedidoCteParaSubContratacaoComponenteFrete.TipoComponenteFrete && (pedidoCteParaSubContratacaoComponenteFrete.ComponenteFrete == null || obj.ComponenteFrete.Equals(pedidoCteParaSubContratacaoComponenteFrete.ComponenteFrete))).Sum(o => o.ValorTotalMoeda ?? 0m) + valorMoedaComponente;

                            valorMoedaComponente += valorTotalMoedaComponente - valorTotalMoedaCargaPedidoComponente;

                            valorComponente = Math.Round(valorMoedaComponente * cotacaoMoeda, 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            decimal valorTotalComponente = (from obj in pedidosCteParaSubContratacaoComponentesFrete where obj.TipoComponenteFrete == pedidoCteParaSubContratacaoComponenteFrete.TipoComponenteFrete && (pedidoCteParaSubContratacaoComponenteFrete.ComponenteFrete == null || obj.ComponenteFrete.Equals(pedidoCteParaSubContratacaoComponenteFrete.ComponenteFrete)) select obj.ValorComponente).Sum();
                            decimal valorTotalCargaPedidoComponente = (from obj in componentesDaNota where obj.TipoComponenteFrete == pedidoCteParaSubContratacaoComponenteFrete.TipoComponenteFrete && (pedidoCteParaSubContratacaoComponenteFrete.ComponenteFrete == null || obj.ComponenteFrete.Equals(pedidoCteParaSubContratacaoComponenteFrete.ComponenteFrete)) select obj.ValorComponente).Sum() + valorComponente;
                            valorComponente += valorTotalComponente - valorTotalCargaPedidoComponente;
                        }
                    }
                    else
                    {
                        //aqui como não tem como consultar no banco de dados cria a lista na memória.
                        Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico componenteDaNota = (from obj in componentesDaNota where obj.TipoComponenteFrete == pedidoCteParaSubContratacaoComponenteFrete.TipoComponenteFrete && (pedidoCteParaSubContratacaoComponenteFrete.ComponenteFrete == null || obj.ComponenteFrete.Equals(pedidoCteParaSubContratacaoComponenteFrete.ComponenteFrete)) select obj).FirstOrDefault();
                        if (componenteDaNota == null)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico componenteDaNotaDinamico = pedidoCteParaSubContratacaoComponenteFrete.ConvertarParaComponenteDinamico();
                            componenteDaNotaDinamico.ValorComponente = valorComponente;
                            componenteDaNotaDinamico.ValorTotalMoeda = valorMoedaComponente;
                            componentesDaNota.Add(componenteDaNotaDinamico);
                        }
                        else
                        {
                            componenteDaNota.ValorComponente += valorComponente;
                            componenteDaNota.ValorTotalMoeda += valorMoedaComponente;
                        }
                    }

                    Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico cargaPedidoComponenteFreteDinamico = pedidoCteParaSubContratacaoComponenteFrete.ConvertarParaComponenteDinamico();
                    cargaPedidoComponenteFreteDinamico.ValorComponente = valorComponente;
                    cargaPedidoComponenteFreteDinamico.ValorTotalMoeda = valorMoedaComponente;

                    svcRateioNotaFiscal.AplicarRateioNosComponentesPedidoNotaFiscal(cargaPedidoComponenteFreteDinamico, pedidoXmlNotaFiscal, unitOfWork);

                    pedidoXmlNotaFiscal.ValorTotalComponentes += cargaPedidoComponenteFreteDinamico.ValorComponente;
                    pedidoXmlNotaFiscal.ValorTotalMoedaComponentes += cargaPedidoComponenteFreteDinamico.ValorTotalMoeda ?? 0m;
                }

                repPedidoXMLNotaFiscal.Atualizar(pedidoXmlNotaFiscal);

                if (pedidoCTeParaSubContratacao.CTeTerceiro.ObterCargaCTe(cargaPedido.Carga.Codigo) != null && (cargaPedido.CargaPedidoTrechoAnterior != null || ((cargaPedido.Carga.TipoOperacao?.ExclusivaDeSubcontratacaoOuRedespacho ?? false) && (cargaPedido.Carga.TipoOperacao?.NaoExigeVeiculoParaEmissao ?? false))))
                    Servicos.Embarcador.Escrituracao.DocumentoProvisao.AdicionarDocumentoParaProvisao(pedidoXmlNotaFiscal, false, tipoServicoMultisoftware, unitOfWork);
            }
        }

        private void AplicarRateioICMSNoPedidoNotaFiscal(Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao, int totalCTes, int numeroNotasRateio, decimal pesoTotal, decimal valorTotalNF, int numeroNotasFiscaisCTes, ref decimal SomaICMSValorTotalPorCTe, ref decimal SomaPisValorTotalPorCTe, ref decimal SomaCofinsValorTotalPorCTe, ref decimal totalICMSFreteCargaPedidoNotas, ref decimal totalPisFreteCargaPedidoNotas, ref decimal totalCofinsFreteCargaPedidoNotas, ref decimal SomaBaseCalculoTotalPorCTe, ref decimal totalBaseCalculoCargaPedidoNotas, Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXmlNotaFiscal, Dominio.Entidades.Embarcador.Rateio.RateioFormula formulaRateio, Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal ultimoPedidoXmlNotaFiscal, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteICMS, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componentePisCofins, Repositorio.UnitOfWork unitOfWork, ref decimal SomaICMSInclusoTotalPorNota, ref decimal totalICMSInclusoFreteCargaPedidoNotas, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> componentesICMSXMLNotaFiscalExistenteCarga, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> componentesPisConfisXMLNotaFiscalExistenteCarga,
            ref decimal SomaBaseCalculoIBSCBSTotalPorCTe, ref decimal SomaValorIBSEstadualTotalPorCTe, ref decimal SomaValorIBSMunicipalTotalPorCTe, ref decimal SomaValorCBSTotalPorCTe, ref decimal totalBaseCalculoIBSCBSCargaPedidoNotas, ref decimal totalValorIBSEstadualCargaPedidoNotas, ref decimal totalValorIBSMunicipalCargaPedidoNotas, ref decimal totalValorCBSCargaPedidoNotas)
        {
            Servicos.Embarcador.Carga.RateioFormula serRateioFormula = new Servicos.Embarcador.Carga.RateioFormula(unitOfWork);
            Servicos.Embarcador.Carga.RateioNotaFiscal svcRateioNotaFiscal = new RateioNotaFiscal(unitOfWork);
            Servicos.Embarcador.Pedido.PedidoXMLNotaFiscal servicoPedidoXMLNotaFiscal = new Servicos.Embarcador.Pedido.PedidoXMLNotaFiscal(unitOfWork);

            decimal valorICMSIncluso = pedidoCTeParaSubContratacao.ValorICMSIncluso;
            decimal valorICMS = pedidoCTeParaSubContratacao.ValorICMS;
            decimal baseCalculo = pedidoCTeParaSubContratacao.BaseCalculoICMS;
            decimal valorPis = pedidoCTeParaSubContratacao.ValorPis;
            decimal valorCofins = pedidoCTeParaSubContratacao.ValorCofins;

            decimal baseCalculoIBSCBS = pedidoCTeParaSubContratacao.BaseCalculoIBSCBS;
            decimal valorIBSEstadual = pedidoCTeParaSubContratacao.ValorIBSEstadual;
            decimal valorIBSMunicipal = pedidoCTeParaSubContratacao.ValorIBSMunicipal;
            decimal valorCBS = pedidoCTeParaSubContratacao.ValorCBS;

            decimal valorICMSInclusoFreteAplicadoRateio = 0m, valorICMSFreteAplicadoRateio = 0m, valorBaseCalculoAplicadoRateio = 0m, valorPisFreteAplicadoRateio = 0m, valorCofinsFreteAplicadoRateio = 0m;
            decimal valorRateioOriginal = 0;

            valorICMSInclusoFreteAplicadoRateio = serRateioFormula.AplicarFormulaRateio(formulaRateio, valorICMSIncluso, numeroNotasRateio, totalCTes, pesoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.Valor, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal);
            valorICMSFreteAplicadoRateio = serRateioFormula.AplicarFormulaRateio(formulaRateio, valorICMS, numeroNotasRateio, totalCTes, pesoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.Valor, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal);
            valorBaseCalculoAplicadoRateio = serRateioFormula.AplicarFormulaRateio(formulaRateio, baseCalculo, numeroNotasRateio, totalCTes, pesoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.Valor, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal);
            valorPisFreteAplicadoRateio = serRateioFormula.AplicarFormulaRateio(formulaRateio, valorPis, numeroNotasRateio, totalCTes, pesoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.Valor, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal);
            valorCofinsFreteAplicadoRateio = serRateioFormula.AplicarFormulaRateio(formulaRateio, valorCofins, numeroNotasRateio, totalCTes, pesoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.Valor, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal);

            decimal baseCalculoIBSCBSAplicadoRateio = serRateioFormula.AplicarFormulaRateio(formulaRateio, baseCalculoIBSCBS, numeroNotasRateio, totalCTes, pesoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.Valor, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal);
            decimal valorIBSEstadualAplicadoRateio = serRateioFormula.AplicarFormulaRateio(formulaRateio, valorIBSEstadual, numeroNotasRateio, totalCTes, pesoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.Valor, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal);
            decimal valorIBSMunicipalAplicadoRateio = serRateioFormula.AplicarFormulaRateio(formulaRateio, valorIBSMunicipal, numeroNotasRateio, totalCTes, pesoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.Valor, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal);
            decimal valorCBSAplicadoRateio = serRateioFormula.AplicarFormulaRateio(formulaRateio, valorCBS, numeroNotasRateio, totalCTes, pesoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.Valor, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal);

            if (formulaRateio != null && formulaRateio.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porCTe)// quando o rateio é por cte é necessário dividir o valor por nota para chegar ao valor de cada nota fiscal do CT-e. 
            {
                valorICMSInclusoFreteAplicadoRateio = Math.Floor((valorICMSInclusoFreteAplicadoRateio / numeroNotasFiscaisCTes) * 100) / 100;
                valorICMSFreteAplicadoRateio = Math.Floor((valorICMSFreteAplicadoRateio / numeroNotasFiscaisCTes) * 100) / 100;
                valorBaseCalculoAplicadoRateio = Math.Floor((valorBaseCalculoAplicadoRateio / numeroNotasFiscaisCTes) * 100) / 100;
                valorPisFreteAplicadoRateio = Math.Floor((valorPisFreteAplicadoRateio / numeroNotasFiscaisCTes) * 100) / 100;
                valorCofinsFreteAplicadoRateio = Math.Floor((valorCofinsFreteAplicadoRateio / numeroNotasFiscaisCTes) * 100) / 100;

                baseCalculoIBSCBSAplicadoRateio = Math.Floor((baseCalculoIBSCBSAplicadoRateio / numeroNotasFiscaisCTes) * 100) / 100;
                valorIBSEstadualAplicadoRateio = Math.Floor((valorIBSEstadualAplicadoRateio / numeroNotasFiscaisCTes) * 100) / 100;
                valorIBSMunicipalAplicadoRateio = Math.Floor((valorIBSMunicipalAplicadoRateio / numeroNotasFiscaisCTes) * 100) / 100;
                valorCBSAplicadoRateio = Math.Floor((valorCBSAplicadoRateio / numeroNotasFiscaisCTes) * 100) / 100;
            }

            pedidoXmlNotaFiscal.ValorICMSIncluso = valorICMSInclusoFreteAplicadoRateio;
            pedidoXmlNotaFiscal.ValorICMS = valorICMSFreteAplicadoRateio;
            pedidoXmlNotaFiscal.ValorPis = valorPisFreteAplicadoRateio;
            pedidoXmlNotaFiscal.ValorCofins = valorCofinsFreteAplicadoRateio;
            pedidoXmlNotaFiscal.BaseCalculoICMS = valorBaseCalculoAplicadoRateio;

            servicoPedidoXMLNotaFiscal.PreencherValoresImpostoIBSCBSRateado(pedidoXmlNotaFiscal, baseCalculoIBSCBSAplicadoRateio, valorIBSEstadualAplicadoRateio, valorIBSMunicipalAplicadoRateio, valorCBSAplicadoRateio);

            SomaICMSInclusoTotalPorNota += valorICMSInclusoFreteAplicadoRateio;
            SomaICMSValorTotalPorCTe += valorICMSFreteAplicadoRateio;
            SomaBaseCalculoTotalPorCTe += valorBaseCalculoAplicadoRateio;
            SomaPisValorTotalPorCTe += valorPisFreteAplicadoRateio;
            SomaCofinsValorTotalPorCTe += valorCofinsFreteAplicadoRateio;

            SomaBaseCalculoIBSCBSTotalPorCTe += baseCalculoIBSCBSAplicadoRateio;
            SomaValorIBSEstadualTotalPorCTe += valorIBSEstadualAplicadoRateio;
            SomaValorIBSMunicipalTotalPorCTe += valorIBSMunicipalAplicadoRateio;
            SomaValorCBSTotalPorCTe += valorCBSAplicadoRateio;


            totalICMSFreteCargaPedidoNotas += pedidoXmlNotaFiscal.ValorICMS;
            totalICMSInclusoFreteCargaPedidoNotas += pedidoXmlNotaFiscal.ValorICMSIncluso;
            totalBaseCalculoCargaPedidoNotas += pedidoXmlNotaFiscal.BaseCalculoICMS;
            totalPisFreteCargaPedidoNotas += pedidoXmlNotaFiscal.ValorPis;
            totalCofinsFreteCargaPedidoNotas += pedidoXmlNotaFiscal.ValorCofins;

            totalBaseCalculoIBSCBSCargaPedidoNotas += pedidoXmlNotaFiscal.BaseCalculoIBSCBS;
            totalValorIBSEstadualCargaPedidoNotas += pedidoXmlNotaFiscal.ValorIBSEstadual;
            totalValorIBSMunicipalCargaPedidoNotas += pedidoXmlNotaFiscal.ValorIBSMunicipal;
            totalValorCBSCargaPedidoNotas += pedidoXmlNotaFiscal.ValorCBS;


            pedidoXmlNotaFiscal.PossuiCTe = pedidoCTeParaSubContratacao.PossuiCTe;
            pedidoXmlNotaFiscal.PossuiNFSManual = pedidoCTeParaSubContratacao.PossuiNFSManual;
            pedidoXmlNotaFiscal.PossuiNFS = pedidoCTeParaSubContratacao.PossuiNFS;
            pedidoXmlNotaFiscal.CFOP = pedidoCTeParaSubContratacao.CFOP;
            pedidoXmlNotaFiscal.CST = pedidoCTeParaSubContratacao.CST;
            pedidoXmlNotaFiscal.PercentualAliquota = pedidoCTeParaSubContratacao.PercentualAliquota;
            pedidoXmlNotaFiscal.AliquotaPis = pedidoCTeParaSubContratacao.AliquotaPis;
            pedidoXmlNotaFiscal.AliquotaCofins = pedidoCTeParaSubContratacao.AliquotaCofins;
            pedidoXmlNotaFiscal.PercentualAliquotaInternaDifal = pedidoCTeParaSubContratacao.PercentualAliquotaInternaDifal;
            pedidoXmlNotaFiscal.PercentualIncluirBaseCalculo = pedidoCTeParaSubContratacao.PercentualIncluirBaseCalculo;
            pedidoXmlNotaFiscal.PercentualReducaoBC = pedidoCTeParaSubContratacao.PercentualReducaoBC;
            pedidoXmlNotaFiscal.IncluirICMSBaseCalculo = pedidoCTeParaSubContratacao.IncluirICMSBaseCalculo;
            pedidoXmlNotaFiscal.ICMSPagoPorST = pedidoCTeParaSubContratacao.CST == "60" ? true : false;
            pedidoXmlNotaFiscal.DescontarICMSDoValorAReceber = pedidoCTeParaSubContratacao.CTeTerceiro != null && pedidoCTeParaSubContratacao.CTeTerceiro.CST == "60" ? (pedidoCTeParaSubContratacao.CTeTerceiro.ValorAReceber + pedidoCTeParaSubContratacao.CTeTerceiro.ValorICMS == pedidoCTeParaSubContratacao.CTeTerceiro.ValorPrestacaoServico) : false;

            servicoPedidoXMLNotaFiscal.PreencherCamposImpostoIBSCBSComTributacaoDefinida(pedidoXmlNotaFiscal, pedidoCTeParaSubContratacao);

            if (ultimoPedidoXmlNotaFiscal != null && pedidoXmlNotaFiscal.Equals(ultimoPedidoXmlNotaFiscal))
            {
                pedidoXmlNotaFiscal.ValorPis += valorPis - totalPisFreteCargaPedidoNotas;
                pedidoXmlNotaFiscal.ValorCofins += valorCofins - totalCofinsFreteCargaPedidoNotas;
                pedidoXmlNotaFiscal.ValorICMS += valorICMS - totalICMSFreteCargaPedidoNotas;
                pedidoXmlNotaFiscal.ValorICMSIncluso += valorICMSIncluso - totalICMSInclusoFreteCargaPedidoNotas;
                pedidoXmlNotaFiscal.BaseCalculoICMS += baseCalculo - totalBaseCalculoCargaPedidoNotas;

                servicoPedidoXMLNotaFiscal.AcrescentarValoresImpostoIBSCBS(pedidoXmlNotaFiscal, baseCalculoIBSCBS - totalBaseCalculoIBSCBSCargaPedidoNotas, valorIBSEstadual - totalValorIBSEstadualCargaPedidoNotas, valorIBSMunicipal - totalValorIBSMunicipalCargaPedidoNotas, valorCBS - totalValorCBSCargaPedidoNotas);
            }

            svcRateioNotaFiscal.GerarComponenteICMS(pedidoXmlNotaFiscal, false, unitOfWork, componenteICMS, componentesICMSXMLNotaFiscalExistenteCarga);
            svcRateioNotaFiscal.GerarComponentePisCofins(pedidoXmlNotaFiscal, unitOfWork, componentePisCofins, componentesPisConfisXMLNotaFiscalExistenteCarga);
        }

        #endregion
    }
}
