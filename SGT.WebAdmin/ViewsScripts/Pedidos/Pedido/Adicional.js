/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/SerieTransportador.js" />
/// <reference path="../../Consultas/CanalEntrega.js" />
/// <reference path="../../Consultas/CentroResultado.js" />
/// <reference path="../../Consultas/PlanoConta.js" />
/// <reference path="../../Consultas/Deposito.js" />
/// <reference path="../../Consultas/PedidoTipoPagamento.js" />
/// <reference path="../../Consultas/CentroCustoViagem.js" />
/// <reference path="../../Consultas/Navio.js" />
/// <reference path="../../Enumeradores/EnumRequisitanteColeta.js" />
/// <reference path="../../Enumeradores/EnumTipoPagamento.js" />
/// <reference path="../../Enumeradores/EnumSimNao.js" />
/// <reference path="../../Enumeradores/EnumTransitoAduaneiro.js" />
/// <reference path="Pedido.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _adicional
var _resumoAdicional;
var _usuarioLogado, _usuarioLogadoCentroCustoViagem;

/*
 * Declaração das Classes
 */

var Adicional = function () {
    this.DataTerminoCarregamento = PropertyEntity({ text: ko.observable(Localization.Resources.Pedidos.Pedido.DataTerminoCarregamento.getFieldDescription()), getType: typesKnockout.dateTime, required: false, issue: 0, enable: ko.observable(false), visible: ko.observable(true) });
    this.DataAgendamento = PropertyEntity({ text: ko.observable(Localization.Resources.Pedidos.Pedido.DataAgendamento.getFieldDescription()), getType: typesKnockout.dateTime, required: false, issue: 0, enable: ko.observable(true), visible: ko.observable(true) });
    this.DataInicialViagemExecutada = PropertyEntity({ text: ko.observable(Localization.Resources.Pedidos.Pedido.DataInicialViajemExecutada.getFieldDescription()), getType: typesKnockout.dateTime, required: false, issue: 1980, enable: ko.observable(false), visible: ko.observable(false) });
    this.DataInicialViagemFaturada = PropertyEntity({ text: ko.observable(Localization.Resources.Pedidos.Pedido.DataInicialViajemFaturada.getFieldDescription()), getType: typesKnockout.dateTime, required: false, issue: 1981, enable: ko.observable(true), visible: ko.observable(false) });
    this.DataFinalViagemExecutada = PropertyEntity({ text: ko.observable(Localization.Resources.Pedidos.Pedido.DataFinalViajemExecutada.getFieldDescription()), getType: typesKnockout.dateTime, required: false, issue: 1980, enable: ko.observable(false), visible: ko.observable(false) });
    this.DataFinalViagemFaturada = PropertyEntity({ text: ko.observable(Localization.Resources.Pedidos.Pedido.DataFinalViajemFaturada.getFieldDescription()), getType: typesKnockout.dateTime, required: false, issue: 1981, enable: ko.observable(true), visible: ko.observable(false) });
    this.DataPrevisaoSaida = PropertyEntity({ text: ko.observable(Localization.Resources.Pedidos.Pedido.DataPrevisaoSaida.getFieldDescription()), getType: typesKnockout.dateTime, required: false, issue: 2 });
    this.DataPrevisaoEntrega = PropertyEntity({ text: ko.observable(Localization.Resources.Pedidos.Pedido.DataPrevisaoEntrega.getFieldDescription()), getType: typesKnockout.dateTime, required: false, issue: 2 });
    this.PrevisaoEntregaTransportador = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.PrevisaoEntregaTransportador, getType: typesKnockout.dateTime, required: false, visible: ko.observable(true) });
    this.ValorCustoFrete = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.ValorCustoFrete, getType: typesKnockout.decimal, required: false, visible: ko.observable(true) });


    this.DataValidade = PropertyEntity({ text: ko.observable(Localization.Resources.Pedidos.Pedido.DataValidade.getFieldDescription()), getType: typesKnockout.dateTime, required: false });
    this.DataInicioJanelaDescarga = PropertyEntity({ text: ko.observable(Localization.Resources.Pedidos.Pedido.DataInicioJanelaDescarga.getFieldDescription()), getType: typesKnockout.dateTime, required: false });
    this.CodigoPedidoCliente = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.NumeroPedidoCliente.getFieldDescription(), maxlength: 150, required: false });
    this.SenhaAgendamento = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.SenhaAgendamento.getFieldDescription(), maxlength: 150, required: false });
    this.SenhaAgendamentoCliente = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.SenhaAgendamentoCliente.getFieldDescription(), maxlength: 150, required: false });
    this.Requisitante = PropertyEntity({ val: ko.observable(EnumRequisitanteColeta.Remetente), options: EnumRequisitanteColeta.obterOpcoes(), def: EnumRequisitanteColeta.Remetente, text: Localization.Resources.Pedidos.Pedido.Requisitante.getFieldDescription(), required: false, issue: 308 });
    this.SerieCTe = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Pedidos.Pedido.SerieCTe.getFieldDescription(), issue: 756, idBtnSearch: guid(), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-12 col-lg-2"), visible: ko.observable(true) });

    this.ValorFreteNegociado = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 13, text: Localization.Resources.Pedidos.Pedido.ValorFreteNegociado.getFieldDescription(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.ValorFreteTransportadorTerceiro = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 13, text: Localization.Resources.Pedidos.Pedido.ValorFreteTerceiro.getFieldDescription(), required: ko.observable(false), enable: ko.observable(true), issue: (_CONFIGURACAO_TMS.GerarContratoTerceiroSemInformacaoDoFrete ? 24746 : 24748), visible: ko.observable(true) });
    this.ValorFreteToneladaNegociado = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 13, text: Localization.Resources.Pedidos.Pedido.ValorFreteNegociado.getFieldDescription(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.ValorFreteToneladaTerceiro = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 13, text: Localization.Resources.Pedidos.Pedido.ValorFreteTerceiro.getFieldDescription(), required: ko.observable(false), enable: ko.observable(true), issue: (_CONFIGURACAO_TMS.GerarContratoTerceiroSemInformacaoDoFrete ? 24746 : 24748), visible: ko.observable(false) });

    this.ValorPedagioRota = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 13, text: Localization.Resources.Pedidos.Pedido.ValorPedagioRota.getFieldDescription(), required: false, enable: ko.observable(true), visible: ko.observable(false) });
    this.ValorTotalNotasFiscais = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 13, text: Localization.Resources.Pedidos.Pedido.ValorTotalMercadoria.getFieldDescription(), required: false });
    this.TipoPagamento = PropertyEntity({ val: ko.observable(EnumTipoPagamento.Pago), options: EnumTipoPagamento.obterOpcoes(), def: EnumTipoPagamento.Pago, text: Localization.Resources.Pedidos.Pedido.TipoPagamento.getFieldDescription(), issue: 120, required: false, eventChange: tipoPagamentoChange });
    this.CategoriaOS = PropertyEntity({ val: ko.observable(""), options: EnumCategoriaOS.obterOpcoesCadastroPedido(), def: "", text: Localization.Resources.Pedidos.Pedido.CategoriaOS.getFieldDescription(), visible: ko.observable(_HabilitarFuncionalidadesProjetoGollum) });
    this.ValorTotalProvedor = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 13, text: "Valor Total Provedor:", required: false, enable: ko.observable(false), visible: ko.observable(_HabilitarFuncionalidadesProjetoGollum) });
    this.TipoOS = PropertyEntity({ val: ko.observable(EnumTipoOS.NaoInformado), options: EnumTipoOS.obterOpcoesPedido(), def: "", text: "Tipo OS:", visible: ko.observable(_HabilitarFuncionalidadesProjetoGollum) });
    this.TipoOSConvertido = PropertyEntity({ val: ko.observable(EnumTipoOSConvertido.NaoInformado), options: EnumTipoOSConvertido.obterOpcoesPedido(), def: "", text: "Tipo OS Convertido:", visible: ko.observable(_HabilitarFuncionalidadesProjetoGollum) });
    this.DataUltimaLiberacao = PropertyEntity({ text: ko.observable(Localization.Resources.Pedidos.Pedido.DataUltimaLiberacao.getFieldDescription()), getType: typesKnockout.dateTime, required: false });
    this.NumeroOrdem = PropertyEntity({ getType: typesKnockout.string, maxlength: 150, text: Localization.Resources.Pedidos.Pedido.NumeroOrdem.getFieldDescription(), required: false });
    this.NumeroControle = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.NumeroControle.getFieldDescription(), required: false, visible: ko.observable(true), maxlength: 100 });
    this.UsuarioCriacaoRemessa = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Pedidos.Pedido.UsuarioCriacaoRemessa.getFieldDescription(), val: ko.observable(""), def: "", maxlength: 12 });
    this.RotaEmbarcador = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Pedidos.Pedido.RotaEmbarcador.getFieldDescription(), val: ko.observable(""), def: "" });
    this.RegiaoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, fadeVisible: ko.observable(false), text: Localization.Resources.Pedidos.Pedido.RegiaoDestino.getFieldDescription(), idBtnSearch: guid() });
    this.NumeroPaletesPagos = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { precision: 3, allowZero: false }, maxlength: 13, text: Localization.Resources.Pedidos.Pedido.NumeroPalletsPagos.getFieldDescription(), required: false, enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) });
    this.NumeroSemiPaletes = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { precision: 3, allowZero: false }, maxlength: 13, text: Localization.Resources.Pedidos.Pedido.NumeroSemiPallets.getFieldDescription(), required: false, enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) });
    this.NumeroSemiPaletesPagos = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { precision: 3, allowZero: false }, maxlength: 13, text: Localization.Resources.Pedidos.Pedido.NumeroSemiPalletsPagos.getFieldDescription(), required: false, enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) });
    this.NumeroCombis = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { precision: 3, allowZero: false }, maxlength: 13, text: Localization.Resources.Pedidos.Pedido.NumeroCombis.getFieldDescription(), required: false, enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) });
    this.NumeroCombisPagas = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { precision: 3, allowZero: false }, maxlength: 13, text: Localization.Resources.Pedidos.Pedido.NumeroCombisPagas.getFieldDescription(), required: false, enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) });
    this.CodigoAgrupamentoCarregamento = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.CodigoAgrupador.getFieldDescription(), required: false, enable: ko.observable(false), visible: ko.observable(true) });
    this.NumeroContratoFreteCliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Pedidos.Pedido.NumeroContrato.getFieldDescription(), visible: ko.observable(true), idBtnSearch: guid() });
    this.PedidoBloqueado = PropertyEntity({ val: ko.observable(false), options: Global.ObterOpcoesBooleano(Localization.Resources.Enumeradores.SimNao.Sim, Localization.Resources.Enumeradores.SimNao.Nao), def: false, text: Localization.Resources.Pedidos.Pedido.PedidoBloqueado.getFieldDescription(), required: false });
    this.PedidoRestricaoData = PropertyEntity({ val: ko.observable(false), options: Global.ObterOpcoesBooleano(Localization.Resources.Enumeradores.SimNao.Sim, Localization.Resources.Enumeradores.SimNao.Nao), def: false, text: Localization.Resources.Pedidos.Pedido.PedidoRestricaoData.getFieldDescription(), required: false });
    this.CustoFrete = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { precision: 3, allowZero: false }, maxlength: 13, text: Localization.Resources.Pedidos.Pedido.CustoFrete.getFieldDescription(), required: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.ValorCobrancaFreteCombinado = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { precision: 3, allowZero: false }, maxlength: 13, text: Localization.Resources.Pedidos.Pedido.ValorCobrancaFreteCombinado.getFieldDescription(), required: false, enable: ko.observable(true), visible: ko.observable(true) });

    this.PesoTotalPaletes = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 15, text: Localization.Resources.Pedidos.Pedido.PesoTotalPallets.getFieldDescription(), required: false, visible: ko.observable(true) });
    this.LocalPaletizacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.LocalPaletizacao, idBtnSearch: guid(), required: false, enable: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-4 col-lg-4") });
    this.MercadoLivreRota = PropertyEntity({ getType: typesKnockout.int, maxlength: 11, text: Localization.Resources.Pedidos.Pedido.MercadoLivreRota.getFieldDescription(), required: false, configInt: { precision: 0, allowZero: false }, visible: ko.observable(_PossuiIntegracaoMercadoLivre), enable: ko.observable(true) });
    this.MercadoLivreFacility = PropertyEntity({ val: ko.observable(""), def: "", maxlength: 50, getType: typesKnockout.string, text: Localization.Resources.Pedidos.Pedido.MercadoLivreFacility.getFieldDescription(), visible: ko.observable(_PossuiIntegracaoMercadoLivre), enable: ko.observable(true) });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, fadeVisible: ko.observable(false), text: ko.observable(Localization.Resources.Pedidos.Pedido.Tomador.getFieldDescription()), idBtnSearch: guid(), issue: 52, cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-6 col-lg-6") });
    this.ObservacaoCTe = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.ObservacaoCTe.getFieldDescription(), maxlength: 2000, issue: 76 });
    this.ImprimirObservacaoCTe = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.Pedido.ImprimirObservacaoCTe, val: ko.observable(false), def: false, visible: true });
    this.Temperatura = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.Temperatura.getFieldDescription(), issue: 587, maxlength: 50, required: false });
    this.Vendedor = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.Vendedor.getFieldDescription(), maxlength: 150, enable: false });
    this.Ordem = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.Ordem.getFieldDescription(), maxlength: 50, enable: false });
    this.PortoSaida = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.PortoSaida.getFieldDescription(), maxlength: 150, enable: false });
    this.PortoChegada = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.PortoChegada.getFieldDescription(), maxlength: 150, enable: false });
    this.Companhia = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.Companhia.getFieldDescription(), maxlength: 150, enable: false });
    this.NumeroNavio = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.NumeroNavio.getFieldDescription(), maxlength: 1000, enable: false });
    this.Reserva = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.Reserva.getFieldDescription(), maxlength: 150, enable: false });
    this.Resumo = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.Resumo.getFieldDescription(), maxlength: 150, enable: false });
    this.TipoEmbarque = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.TipoEmbarque.getFieldDescription(), maxlength: 150, enable: false });
    this.DataETA = PropertyEntity({ text: ko.observable(Localization.Resources.Pedidos.Pedido.DataETA.getFieldDescription()), getType: typesKnockout.dateTime, enable: false });
    this.Pallets = PropertyEntity({ getType: typesKnockout.int, maxlength: 5, text: Localization.Resources.Pedidos.Pedido.NumeroPalletsControle.getFieldDescription(), configInt: { precision: 0, allowZero: true, thousands: "" }, required: false, visible: ko.observable(_CONFIGURACAO_TMS.UtilizarControlePallets) });
    this.GerarAutomaticamenteCargaDoPedido = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.Pedido.GerarUmaCargaAutomaticamenteParaEstePedido, val: ko.observable(true), def: true, visible: (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador ? true : false) });
    this.PedidoSubContratado = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.Pedido.UmaSubcontratacao, issue: 901, val: ko.observable(false), def: false, visible: true });
    this.ViagemJaOcorreu = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.Pedido.ViajemJaOcorreu, issue: 317, val: ko.observable(false), def: false, visible: true });
    this.PedidoTransbordo = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.Pedido.PedidoTransbordoRodoviario, issue: 900, val: ko.observable(false), def: false, visible: true });
    this.DisponibilizarPedidoParaColeta = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.Pedido.DisponibilizarPedidoParaColeta, val: ko.observable(false), def: false, visible: ko.observable(_CONFIGURACAO_TMS.ExibirPedidoDeColeta) });
    this.RecebedorColeta = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.Recebedor.getRequiredFieldDescription(), idBtnSearch: guid(), issue: 0, visible: ko.observable(false), required: ko.observable(false) });
    this.PesoTotalCargaTMS = PropertyEntity({ val: _pedido.PesoTotalCarga.val, getType: typesKnockout.decimal, configDecimal: { allowZero: true }, maxlength: 15, text: Localization.Resources.Pedidos.Pedido.PesoPedido.getFieldDescription(), required: false, visible: ko.observable(false), issue: 24747 });
    this.QtdEntregas = PropertyEntity({ getType: typesKnockout.int, configInt: { precision: 0, allowZero: true }, maxlength: 15, text: Localization.Resources.Pedidos.Pedido.QtdEntregas.getFieldDescription(), required: false, visible: ko.observable(false) });
    this.CubagemTotalTMS = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { precision: 3, allowZero: true }, maxlength: 15, text: Localization.Resources.Pedidos.Pedido.CubagemPedido.getFieldDescription(), required: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.NecessarioReentrega = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.Pedido.SeraNecessarioRefazerReentrega, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.EntregaAgendada = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.Pedido.EntregaEstaAgendadaNoDestino, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.QuebraMultiplosCarregamentos = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.Pedido.PermitirQuebraPedidoNaRoteirizacao, val: ko.observable(true), def: true, visible: ko.observable(true) });
    this.ExecaoCab = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.Pedido.ExecaoCab, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.PedidoPaletizado = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.Pedido.PedidoPaletizado, val: ko.observable(true), def: false, visible: ko.observable(_CONFIGURACAO_TMS.UsarFatorConversaoProdutoEmPedidoPaletizado) });
    this.EssePedidopossuiPedidoBonificacao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.Pedido.EssePedidopossuiPedidoBonificacao, val: ko.observable(false), def: false, visible: ko.observable(false), enable: ko.observable(true) });
    this.EssePedidopossuiPedidoVenda = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.Pedido.EssePedidopossuiPedidoVenda, val: ko.observable(false), def: false, visible: ko.observable(false), enable: ko.observable(true) });
    this.Distancia = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 15, text: Localization.Resources.Pedidos.Pedido.DistanciaKM.getFieldDescription(), required: false, visible: ko.observable(true) });
    this.ValorFreteInformativo = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 15, text: Localization.Resources.Pedidos.Pedido.ValorFreteInformativo.getFieldDescription(), required: false, visible: ko.observable(true) });
    this.ProcImportacao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Pedidos.Pedido.ProcImportação.getFieldDescription(), required: false, visible: ko.observable(true) });
    this.NumeroPedidoVinculado = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Pedidos.Pedido.NumeroPedidoVinculado.getFieldDescription(), required: false, visible: ko.observable(false), enable: ko.observable(true) });
    this.Safra = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Pedidos.Pedido.Safra.getFieldDescription(), required: false, visible: ko.observable(true) });
    this.PercentualAdiantamentoTerceiro = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: true }, maxlength: 15, text: Localization.Resources.Pedidos.Pedido.Adiantamento.getFieldDescription(), required: false, visible: ko.observable(true), val: ko.observable("") });
    this.PercentualMinimoAdiantamentoTerceiro = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: true }, maxlength: 15, text: Localization.Resources.Pedidos.Pedido.MinimoAdiantamento.getFieldDescription(), required: false, visible: ko.observable(true), val: ko.observable("") });
    this.PercentualMaximoAdiantamentoTerceiro = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: true }, maxlength: 15, text: Localization.Resources.Pedidos.Pedido.MáximoAdiantamento.getFieldDescription(), required: false, visible: ko.observable(true), val: ko.observable("") });
    this.Substituicao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.Pedido.Substituicao, val: ko.observable(false), def: false, visible: ko.observable(_CONFIGURACAO_TMS.PermiteInformarPedidoSubstituicao) });
    this.Rastreado = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.Pedido.SeraRastreado, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.GerenciamentoRisco = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.Pedido.TeraGerenciamentoDeRisco, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.EscoltaArmada = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Pedidos.Pedido.NecessarioEscolta), val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.EscoltaMunicipal = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.Pedido.NecessarioEscoltaMunicipal, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.DespachoTransitoAduaneiro = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.Pedido.DespachoTransitoAduaneiro.getFieldDescription(), val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.NumeroDTA = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.NumeroDTA.getFieldDescription(), val: ko.observable(""), def: "", maxlength: 100 });
    this.Cotacao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.Pedido.Cotacao, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.ValorFreteCotado = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.ValorDoFreteCotado, val: ko.observable(""), def: "", enable: ko.observable(false) });
    this.Seguro = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.Pedido.Seguro, val: ko.observable(false), def: false, visible: ko.observable(false), eventChange: seguroChange });
    this.Ajudante = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.Pedido.Ajudante, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.ResponsavelRedespacho = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, fadeVisible: ko.observable(false), text: ko.observable(Localization.Resources.Pedidos.Pedido.ResponsavelRedespacho.getFieldDescription()), idBtnSearch: guid(), issue: 52 });
    this.FuncionarioVendedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, fadeVisible: ko.observable(false), text: ko.observable(Localization.Resources.Pedidos.Pedido.Vendedor.getFieldDescription()), idBtnSearch: guid() });
    this.FuncionarioSupervisor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, fadeVisible: ko.observable(false), text: ko.observable(Localization.Resources.Pedidos.Pedido.Supervisor.getFieldDescription()), idBtnSearch: guid() });
    this.FuncionarioGerente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, fadeVisible: ko.observable(false), text: ko.observable(Localization.Resources.Pedidos.Pedido.Gerente.getFieldDescription()), idBtnSearch: guid() });
    this.ElementoPEP = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Pedidos.Pedido.ElementoPEP.getFieldDescription(), visible: ko.observable(true), enable: ko.observable(true), val: ko.observable(""), def: "", maxlength: 24 });
    this.CentroResultadoEmbarcador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.CentroResultado.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.ContaContabil = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Pedidos.Pedido.ContaContabil.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true), required: false });
    this.ElementoPEP.val.subscribe(VerificarCamposElementoPEPCentroResultadoPedidoAdicionais);
    this.CentroResultadoEmbarcador.val.subscribe(VerificarCamposElementoPEPCentroResultadoPedidoAdicionais);
    this.ISISReturn = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.ISISReturn.getFieldDescription(), def: 0, val: ko.observable(0), getType: typesKnockout.int, visible: ko.observable(false) });
    this.QtVolumes = PropertyEntity({ getType: typesKnockout.int, configInt: { precision: 0, allowZero: true }, text: Localization.Resources.Pedidos.Pedido.QtdVolumes.getFieldDescription(), required: false, visible: ko.observable(true), enable: ko.observable(true), val: ko.observable("") });
    this.QuantidadeVolumesPrevios = PropertyEntity({ getType: typesKnockout.int, configInt: { precision: 0, allowZero: true }, text: Localization.Resources.Pedidos.Pedido.QtdVolumePrevio.getFieldDescription(), required: false, visible: ko.observable(true), enable: ko.observable(true), val: ko.observable("") });
    this.DataOrder = PropertyEntity({ text: ko.observable(Localization.Resources.Pedidos.Pedido.OrderDate.getFieldDescription()), getType: typesKnockout.date, required: false, issue: 2, val: ko.observable("") });
    this.ColetaEmProdutorRural = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.UsarTipoTomadorPedido = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pedidos.Pedido.UsarTipoTomador, def: false, enable: ko.observable(true) });
    this.TipoTomador = PropertyEntity({ val: ko.observable(EnumTipoTomador.Remetente), options: EnumTipoTomador.obterOpcoesPadrao(), def: EnumTipoTomador.Remetente, text: Localization.Resources.Pedidos.Pedido.TipoTomador.getFieldDescription(), required: false, eventChange: tipoTomadorChange });
    this.PossuiCarregamento = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.Pedido.PossuiCargaInformeValor.getFieldDescription(), val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.ValorCarregamento = PropertyEntity({ getType: typesKnockout.decimal, maxlength: 15, text: Localization.Resources.Pedidos.Pedido.ValorCarga.getFieldDescription(), configDecimal: { allowZero: false }, required: false });
    this.PossuiDescarga = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.Pedido.PossuiDescargaInformeValor.getFieldDescription(), val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.ValorDescarga = PropertyEntity({ getType: typesKnockout.decimal, maxlength: 15, text: Localization.Resources.Pedidos.Pedido.ValorDescarga.getFieldDescription(), configDecimal: { allowZero: false }, required: false });
    this.PossuiDeslocamento = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.Pedido.DeslocamentoInformeQuantidade.getFieldDescription(), val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.ValorDeslocamento = PropertyEntity({ getType: typesKnockout.decimal, maxlength: 15, text: Localization.Resources.Pedidos.Pedido.ValorQuantidade.getFieldDescription(), configDecimal: { allowZero: false }, required: false });
    this.PossuiDiaria = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.Pedido.DiariasInformeQuantidade.getFieldDescription(), val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.ValorDiaria = PropertyEntity({ getType: typesKnockout.decimal, maxlength: 15, text: Localization.Resources.Pedidos.Pedido.ValorDiaria.getFieldDescription(), configDecimal: { allowZero: false }, required: false });
    this.QuantidadeNotasFiscais = PropertyEntity({ getType: typesKnockout.int, maxlength: 15, text: Localization.Resources.Pedidos.Pedido.QtdNotasFiscais.getFieldDescription(), configInt: { precision: 0, allowZero: true }, required: false, visible: ko.observable(false) });
    this.ObservacaoInterna = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.ObservacaoInterna.getFieldDescription(), maxlength: 2000, required: false, visible: ko.observable(true) });
    this.ObservacaoEntrega = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.ObservacaoEntrega.getFieldDescription(), maxlength: 500, required: false, visible: ko.observable(true), enable: ko.observable(false) });
    this.ValorTotalCarga = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 15, text: Localization.Resources.Pedidos.Pedido.SeguroInformeValorTotalDaCarga.getFieldDescription(), required: false, visible: ko.observable(true), enable: ko.observable(false) });
    this.QtdAjudantes = PropertyEntity({ getType: typesKnockout.int, configInt: { precision: 0, allowZero: false }, text: Localization.Resources.Pedidos.Pedido.AjudantesInformeQuantidade.getFieldDescription(), required: false, visible: ko.observable(true), enable: ko.observable(false) });
    this.PossuiIsca = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.PossuiIsca, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.QtdIsca = PropertyEntity({ getType: typesKnockout.int, configInt: { precision: 0, allowZero: false }, text: Localization.Resources.Pedidos.Pedido.QuantidadesIsca.getFieldDescription(), visible: ko.observable(obterVisivilidade()) });
    this.AjudanteCarga = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.Pedido.AjudantesNaCarga, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.QtdAjudantesCarga = PropertyEntity({ getType: typesKnockout.int, configInt: { precision: 0, allowZero: false }, text: Localization.Resources.Pedidos.Pedido.AjudantesNaCargaInformeQuantidade.getFieldDescription(), required: false, visible: ko.observable(obterVisivilidade()), enable: ko.observable(false) });
    this.AjudanteDescarga = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.Pedido.AjudanteNaDescarga, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.QtdAjudantesDescarga = PropertyEntity({ getType: typesKnockout.int, configInt: { precision: 0, allowZero: false }, text: Localization.Resources.Pedidos.Pedido.AjudantesNaDescargaInformeQuantidade.getFieldDescription(), required: false, visible: ko.observable(obterVisivilidade()), enable: ko.observable(false) });
    this.Escolta = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.Pedido.Escolta, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.QtdEscolta = PropertyEntity({ getType: typesKnockout.int, configInt: { precision: 0, allowZero: false }, text: Localization.Resources.Pedidos.Pedido.EscoltaInformeQuantidade.getFieldDescription(), required: false, visible: ko.observable(true), enable: ko.observable(false) });
    this.TagNumeroCTe = PropertyEntity({ eventClick: function (e) { InserirTag(_adicional.ObservacaoCTe.id, "#NumeroCTe"); }, type: types.event, text: Localization.Resources.Pedidos.Pedido.NumeroCTe });
    this.TagRemetente = PropertyEntity({ eventClick: function (e) { InserirTag(_adicional.ObservacaoCTe.id, "#NomeRemetente"); }, type: types.event, text: Localization.Resources.Gerais.Geral.Remetente });
    this.TagDestinatario = PropertyEntity({ eventClick: function (e) { InserirTag(_adicional.ObservacaoCTe.id, "#NomeDestinatario"); }, type: types.event, text: Localization.Resources.Gerais.Geral.Destinatario });
    this.TagNumeroPedido = PropertyEntity({ eventClick: function (e) { InserirTag(_adicional.ObservacaoCTe.id, "#NumeroPedido"); }, type: types.event, text: Localization.Resources.Pedidos.Pedido.NumeroPedido });
    this.TagNumeroCarga = PropertyEntity({ eventClick: function (e) { InserirTag(_adicional.ObservacaoCTe.id, "#NumeroCarga"); }, type: types.event, text: Localization.Resources.Pedidos.Pedido.NumeroCarga });
    this.TagLocalPrestacaoServico = PropertyEntity({ eventClick: function (e) { InserirTag(_adicional.ObservacaoCTe.id, "#LocalPrestacaoServico"); }, type: types.event, text: Localization.Resources.Pedidos.Pedido.LocalPrestacaoServico });

    this.CanalEntrega = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Pedidos.Pedido.CanalEntrega.getFieldDescription()), idBtnSearch: guid() });
    this.CanalVenda = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Pedidos.Pedido.CanalVenda.getFieldDescription()), idBtnSearch: guid() });
    this.Deposito = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Pedidos.Pedido.Deposito.getFieldDescription()), idBtnSearch: guid() });
    this.NumeroEXP = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.NumeroEXP.getFieldDescription(), maxlength: 150, enable: ko.observable(false), visible: ko.observable(false) });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.ObservacaoPedido.getFieldDescription(), maxlength: 2000, enable: ko.observable(true), val: ko.observable(""), getType: typesKnockout.string, visible: ko.observable(true) });
    this.ValorGross = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.ValorGross.getFieldDescription(), enable: ko.observable(true), visible: ko.observable(true), val: ko.observable(0), getType: typesKnockout.decimal });
    this.ValoNFe = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.ValorNFe.getFieldDescription(), enable: ko.observable(false), visible: ko.observable(true), getType: typesKnockout.decimal, val: ko.observable(0) });
    this.DataEmissaoNFe = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.DataEmissaoNFe.getFieldDescription(), enable: ko.observable(false), visible: ko.observable(true), getType: typesKnockout.dateTime, val: ko.observable("") });
    this.DataAlocacaoPedido = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.DataAlocacaoPedido.getFieldDescription(), enable: ko.observable(true), visible: ko.observable(true), getType: typesKnockout.dateTime, val: ko.observable("") });
    this.NumeroProtocoloIntegracaoPedido = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Pedidos.Pedido.NumeroProtocoloIntegracaoPedido.getFieldDescription(), visible: ko.observable(false), enable: ko.observable(false) });
    this.NumeroProtocoloIntegracaoCarga = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Pedidos.Pedido.NumeroProtocoloIntegracaoCarga.getFieldDescription(), visible: ko.observable(false), enable: ko.observable(false) });
    this.NumeroPedidoOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Pedidos.Pedido.NumeroPedidoOrigem.getFieldDescription(), visible: ko.observable(_CONFIGURACAO_TMS.HabilitarBIDTransportePedido), idBtnSearch: guid() });
    this.CentroDeCustoViagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Pedidos.Pedido.CentroDeCustoViagem.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) });
    this.Balsa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Pedidos.Pedido.Balsa.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(true) });
    //Multimodal
    this.TagNumeroPedidoCliente = PropertyEntity({ eventClick: function (e) { InserirTag(_adicional.ObservacaoCTe.id, "#NumeroPedidoCliente"); }, type: types.event, text: Localization.Resources.Pedidos.Pedido.NumeroPedidoDoCliente, visible: ko.observable(false) });
    this.TagNumeroBooking = PropertyEntity({ eventClick: function (e) { InserirTag(_adicional.ObservacaoCTe.id, "#NumeroBooking"); }, type: types.event, text: Localization.Resources.Pedidos.Pedido.NumeroDoBooking, visible: ko.observable(false) });
    this.TagNumeroContainer = PropertyEntity({ eventClick: function (e) { InserirTag(_adicional.ObservacaoCTe.id, "#NumeroContainer"); }, type: types.event, text: Localization.Resources.Pedidos.Pedido.NumeroDoContainer, visible: ko.observable(false) });
    this.TagPedidoViagemNavio = PropertyEntity({ eventClick: function (e) { InserirTag(_adicional.ObservacaoCTe.id, "#NavioViagemDirecao"); }, type: types.event, text: Localization.Resources.Pedidos.Pedido.NavioViajemDirecao, visible: ko.observable(false) });
    this.TagPortoOrigem = PropertyEntity({ eventClick: function (e) { InserirTag(_adicional.ObservacaoCTe.id, "#PortoOrigem"); }, type: types.event, text: Localization.Resources.Pedidos.Pedido.PortoOrigem, visible: ko.observable(false) });
    this.TagPortoDestino = PropertyEntity({ eventClick: function (e) { InserirTag(_adicional.ObservacaoCTe.id, "#PortoDestino"); }, type: types.event, text: Localization.Resources.Pedidos.Pedido.PortoDestino, visible: ko.observable(false) });
    this.TagQuantidadeETipoContainer = PropertyEntity({ eventClick: function (e) { InserirTag(_adicional.ObservacaoCTe.id, "#QuantidadeETipoContainer"); }, type: types.event, text: Localization.Resources.Pedidos.Pedido.QtdTipoContainer, visible: ko.observable(false) });

    this.TagNumeroOS = PropertyEntity({ eventClick: function (e) { InserirTag(_adicional.ObservacaoCTe.id, "#NumeroOS"); }, type: types.event, text: Localization.Resources.Pedidos.Pedido.NumeroOS, visible: ko.observable(false) });
    this.TagValorNota = PropertyEntity({ eventClick: function (e) { InserirTag(_adicional.ObservacaoCTe.id, "#ValorNota"); }, type: types.event, text: Localization.Resources.Pedidos.Pedido.ValorNota, visible: ko.observable(false) });
    this.TagValorADValorem = PropertyEntity({ eventClick: function (e) { InserirTag(_adicional.ObservacaoCTe.id, "#ValorADValorem"); }, type: types.event, text: Localization.Resources.Pedidos.Pedido.ValorADValorem, visible: ko.observable(false) });

    this.ValorAdValorem = PropertyEntity({ getType: typesKnockout.decimal, maxlength: 13, text: Localization.Resources.Pedidos.Pedido.PorcentagemADValorem.getFieldDescription(), required: false, visible: ko.observable(false), configDecimal: { precision: 3, allowZero: true, allowNegative: false } });
    this.IDBAF = PropertyEntity({ getType: typesKnockout.int, maxlength: 15, text: Localization.Resources.Pedidos.Pedido.IDdoBAF.getFieldDescription(), configInt: { precision: 0, allowZero: true }, required: false, visible: ko.observable(false) });
    this.ValorBAF = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 13, text: Localization.Resources.Pedidos.Pedido.ValorBAF.getFieldDescription(), required: false, visible: ko.observable(false) });
    this.NumeroProposta = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.NumeroProposta.getFieldDescription(), required: false, visible: ko.observable(false), maxlength: 100 });
    this.ProvedorOS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Pedidos.Pedido.ProvedorOS.getFieldDescription()), issue: 52, idBtnSearch: guid(), visible: ko.observable(false) });
    this.PedidoViagemNavio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Pedidos.Pedido.NavioViajemDirecao.getFieldDescription()), issue: 52, idBtnSearch: guid(), visible: ko.observable(false) });
    this.PedidoEmpresaResponsavel = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Pedidos.Pedido.EmpresaResponsavel.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(false) });
    this.PedidoCentroCusto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Pedidos.Pedido.CentroCusto.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(false) });
    this.NecessitaAverbacaoAutomatica = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.Pedido.NecessitaAverbacaoAutomatica, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.PossuiCargaPerigosa = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.Pedido.PossuiCargaPerigosa, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.PedidoLiberadoPortalRetira = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.Pedido.PedidoLiberadoPortalRetira, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.ContemCargaRefrigerada = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.Pedido.ContemCargaRefrigerada, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.NecessitaEnergiaContainerRefrigerado = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.Pedido.NecessitaEnergiaContainerRefrigerado, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.ValidarDigitoVerificadorContainer = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.Pedido.SeraUtilizadoContainerProprio, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.PedidoSVM = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.Pedido.EstePedidoDeSVMInterno, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.PedidoDeSVMTerceiro = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.Pedido.EstePedidoDeSVMTerceiro, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.ContainerADefinir = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.Pedido.ContainerDefinir, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.VisibleEmbarcador = PropertyEntity({ required: false, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) });
    this.VisibleTMS = PropertyEntity({ required: false, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) });
    this.FormaAverbacaoCTE = PropertyEntity({ val: ko.observable(EnumFormaAverbacaoCTE.Definitiva), options: EnumFormaAverbacaoCTE.obterOpcoes(), def: EnumFormaAverbacaoCTE.Definitiva, text: Localization.Resources.Pedidos.Pedido.FormaAverbacao.getFieldDescription(), required: false, visible: ko.observable(false) });
    this.PedidoTipoPagamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.CondicaoPedido.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });
    this.DataBaseCRT = PropertyEntity({ text: ko.observable(Localization.Resources.Pedidos.Pedido.DataBaseCRT.getFieldDescription()), getType: typesKnockout.dateTime, required: false, visible: ko.observable(_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) });
    this.ProdutoPredominante = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.ProdutoPredominante.getFieldDescription(), maxlength: 150, visible: ko.observable(false) });
    this.PontoPartida = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Pedidos.Pedido.PontoPartida.getFieldDescription(), idBtnSearch: guid(), issue: 52 });
    this.LocalidadeInicioPrestacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Pedidos.Pedido.LocalidadeInicioPrestacao.getFieldDescription(), idBtnSearch: guid(), issue: 0, visible: ko.observable(_CONFIGURACAO_TMS.UtilizarLocalidadePrestacaoPedido) });
    this.LocalidadeTerminoPrestacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Pedidos.Pedido.LocalidadeTerminoPrestacao.getFieldDescription(), idBtnSearch: guid(), issue: 0, visible: ko.observable(_CONFIGURACAO_TMS.UtilizarLocalidadePrestacaoPedido) });

    this.NumeroPedidoICT = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Pedidos.Pedido.NumeroPedidoICT.getFieldDescription(), val: ko.observable(""), def: "", maxlength: 50 });
    this.CondicaoExpedicao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Pedidos.Pedido.CondicaoExpedicao.getFieldDescription(), val: ko.observable(""), def: "", maxlength: 50 });
    this.GrupoFreteMaterial = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Pedidos.Pedido.GrupoFreteMaterial.getFieldDescription(), val: ko.observable(""), def: "", maxlength: 50 });
    this.RestricaoEntrega = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Pedidos.Pedido.RestricaoEntrega.getFieldDescription(), val: ko.observable(""), def: "", maxlength: 100 });
    this.DataCriacaoRemessa = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.DataCriacaoRemessa.getFieldDescription(), enable: ko.observable(true), visible: ko.observable(true), getType: typesKnockout.dateTime, val: ko.observable("") });
    this.DataCriacaoVenda = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.DataCriacaoVenda.getFieldDescription(), enable: ko.observable(true), visible: ko.observable(true), getType: typesKnockout.dateTime, val: ko.observable("") });
    this.IndicadorPOF = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Pedidos.Pedido.IndicadorPOF.getFieldDescription(), val: ko.observable(""), def: "", maxlength: 50 });
    this.NumeroRastreioCorreios = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Pedidos.Pedido.NumeroRastreioCorreios.getFieldDescription(), val: ko.observable(""), def: "", maxlength: 150, enable: false });
    this.ProcessamentoEspecial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, fadeVisible: ko.observable(false), text: Localization.Resources.Pedidos.Pedido.ProcessamentoEspecial.getFieldDescription(), idBtnSearch: guid() });
    this.PeriodoEntrega = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, fadeVisible: ko.observable(false), text: Localization.Resources.Pedidos.Pedido.PeriodoEntrega.getFieldDescription(), idBtnSearch: guid() });
    this.HorarioEntrega = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, fadeVisible: ko.observable(false), text: Localization.Resources.Pedidos.Pedido.HorarioEntrega.getFieldDescription(), idBtnSearch: guid() });
    this.DetalheEntrega = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, fadeVisible: ko.observable(false), text: Localization.Resources.Pedidos.Pedido.DetalheEntrega.getFieldDescription(), idBtnSearch: guid() });
    this.RestricaoDiasEntrega = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Pedidos.Pedido.RestricaoDiasEntrega.getFieldDescription(), val: ko.observable(""), def: "", maxlength: 150, enable: false });
    this.IndicativoColetaEntrega = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.IndicativoColetaEntrega.getFieldDescription(), options: EnumIndicativoColetaEntrega.obterOpcoes(), val: ko.observable(EnumIndicativoColetaEntrega.NaoInformado), def: EnumIndicativoColetaEntrega.NaoInformado, visible: ko.observable(false) });
    this.TipoServico = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Pedidos.Pedido.TipoServico.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.NumeroAutorizacaoColetaEntrega = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Pedidos.Pedido.NumeroAutorizacaoColetaEntrega.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.ClientePropostaComercial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.ClientePropostaComercial.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.TipoSeguro = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Pedidos.Pedido.TipoSeguro.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.NumeroOSMae = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Pedidos.Pedido.NumeroOSMae.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.LinkRastreio = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Pedidos.Pedido.LinkRastreio.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(false), enable: ko.observable(false) });
    this.DataPrevisaoTerminoCarregamento = PropertyEntity({ text: ko.observable(Localization.Resources.Pedidos.Pedido.DataPrevisaoTerminoCarregamento.getFieldDescription()), getType: typesKnockout.dateTime, required: false, issue: 0, enable: ko.observable(true), visible: ko.observable(true) });

    this.ContainerADefinir.val.subscribe(function (novoValor) {
        if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) {
            if (novoValor) {
                _importacao.Container.required = false;
                _importacao.Container.text(Localization.Resources.Pedidos.Pedido.Container.getRequiredFieldDescription());
            }
            else {
                _importacao.Container.required = true;
                _importacao.Container.text(Localization.Resources.Pedidos.Pedido.Container.getFieldDescription());
            }
        }
    });

    this.TipoTomador.val.subscribe(function (novoValor) {
        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS && _adicional.UsarTipoTomadorPedido.val()) {
            if (novoValor == EnumTipoTomador.Remetente || novoValor == EnumTipoTomador.Expedidor)
                _adicional.TipoPagamento.val(EnumTipoPagamento.Pago);
            else if (novoValor == EnumTipoTomador.Recebedor || novoValor == EnumTipoTomador.Destinatario)
                _adicional.TipoPagamento.val(EnumTipoPagamento.A_Pagar);
            else if (novoValor == EnumTipoTomador.Outros || novoValor == EnumTipoTomador.Intermediario)
                _adicional.TipoPagamento.val(EnumTipoPagamento.Outros);

            tipoPagamentoChange();
        }
    });

    this.DataFinalViagemExecutada.dateRangeInit = this.DataInicialViagemExecutada;
    this.DataInicialViagemExecutada.dateRangeLimit = this.DataFinalViagemExecutada;

    this.DataFinalViagemFaturada.dateRangeInit = this.DataInicialViagemFaturada;
    this.DataInicialViagemFaturada.dateRangeLimit = this.DataFinalViagemFaturada;

    this.DataPrevisaoEntrega.dateRangeInit = this.DataPrevisaoSaida;
    this.DataPrevisaoSaida.dateRangeLimit = this.DataPrevisaoEntrega;

    this.NecessitaAverbacaoAutomatica.val.subscribe(function (novoValor) {
        _adicional.FormaAverbacaoCTE.visible(novoValor);
    });

    this.PossuiCarregamento.val.subscribe(function (novoValor) {
        _adicional.ValorCarregamento.required = novoValor;
        //_adicional.ValorCarregamento.val("");
    });

    this.PossuiDescarga.val.subscribe(function (novoValor) {
        _adicional.ValorDescarga.required = novoValor;
        //_adicional.ValorDescarga.val("");
    });

    this.Ajudante.val.subscribe(function (novoValor) {
        _adicional.QtdAjudantes.required = novoValor;
        //_adicional.QtdAjudantes.val("");
    });

    this.DisponibilizarPedidoParaColeta.val.subscribe(function (valor) {
        _adicional.RecebedorColeta.visible(valor == true);
        _adicional.RecebedorColeta.required(valor == true);
    });

    this.PossuiDeslocamento.val.subscribe(function (novoValor) {
        _adicional.ValorDeslocamento.required = novoValor;
        _adicional.ValorDeslocamento.val("");
    });

    this.PossuiDiaria.val.subscribe(function (novoValor) {
        _adicional.ValorDiaria.required = novoValor;
        _adicional.ValorDiaria.val("");
    });

    this.EssePedidopossuiPedidoBonificacao.val.subscribe(function (novoValor) {
        _adicional.EssePedidopossuiPedidoVenda.enable(!_adicional.EssePedidopossuiPedidoVenda.enable());
        _adicional.NumeroPedidoVinculado.required = novoValor;
    });

    this.EssePedidopossuiPedidoVenda.val.subscribe(function (novoValor) {
        _adicional.EssePedidopossuiPedidoBonificacao.enable(!_adicional.EssePedidopossuiPedidoBonificacao.enable());
        _adicional.NumeroPedidoVinculado.required = novoValor;
    });

    loadDadosUsuarioLogado();
};

var ResumoAdicional = function () {
    this.FormaPagamento = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.FormaPagamento.getFieldDescription(), visible: ko.observable(true) });
    this.DiasDePrazoFatura = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.DiasDePrazoFaturamento.getFieldDescription(), visible: ko.observable(true) });

    this.BuscarValoresTabelaFrete = PropertyEntity({ type: types.event, eventClick: BuscarValoresTabelaFreteClick, text: ko.observable(Localization.Resources.Pedidos.Pedido.BuscarValoresTabelaFrete), visible: ko.observable(false), enable: ko.observable(true) });
};


/*
 * Declaração das Funções de Inicialização  
 */

function loadAdicional() {

    _adicional = new Adicional();
    KoBindings(_adicional, "knockoutAdicionais");

    _resumoAdicional = new ResumoAdicional();
    KoBindings(_resumoAdicional, "knockoutResumoAdicionais");

    $("#tabAdicionais").show();
    $("#tabPercursoMDFe").show();

    BuscarSeriesCTeTransportador(_adicional.SerieCTe);
    BuscarClientes(_adicional.Tomador, RetornoTomador);
    BuscarClientes(_adicional.ProvedorOS);
    BuscarClientes(_adicional.ResponsavelRedespacho);
    BuscarClientes(_adicional.PontoPartida);
    BuscarClientes(_adicional.LocalPaletizacao, RetornoLocalPaletizacao);
    BuscarFuncionario(_adicional.FuncionarioVendedor);
    BuscarFuncionario(_adicional.FuncionarioSupervisor);
    BuscarFuncionario(_adicional.FuncionarioGerente);
    BuscarCanaisEntrega(_adicional.CanalEntrega);
    BuscarCanaisVenda(_adicional.CanalVenda);
    BuscarDeposito(_adicional.Deposito);
    BuscarPedidoViagemNavio(_adicional.PedidoViagemNavio, RetornoPedidoViagemNavio);
    BuscarPedidoEmpresaResponsavel(_adicional.PedidoEmpresaResponsavel);
    BuscarPedidoCentroCusto(_adicional.PedidoCentroCusto);
    BuscarPedidoTipoPagamento(_adicional.PedidoTipoPagamento, RetornoPedidoTipoPagamento);
    BuscarLocalidades(_adicional.LocalidadeInicioPrestacao);
    BuscarLocalidades(_adicional.LocalidadeTerminoPrestacao);
    BuscarCentroResultado(_adicional.CentroResultadoEmbarcador);
    BuscarPlanoConta(_adicional.ContaContabil);
    BuscarRegioes(_adicional.RegiaoDestino);
    BuscarClientes(_adicional.RecebedorColeta);
    BuscarClientes(_adicional.ClientePropostaComercial);
    BuscarTiposDetalhe(_adicional.ProcessamentoEspecial, null, null, EnumTipoTipoDetalhe.ProcessamentoEspecial);
    BuscarTiposDetalhe(_adicional.HorarioEntrega, null, null, EnumTipoTipoDetalhe.HorarioEntrega);
    BuscarTiposDetalhe(_adicional.PeriodoEntrega, null, null, EnumTipoTipoDetalhe.PeriodoEntrega);
    BuscarTiposDetalhe(_adicional.DetalheEntrega, null, null, EnumTipoTipoDetalhe.DetalheEntrega);
    BuscarCentroCustoViagem(_adicional.CentroDeCustoViagem);
    BuscarContratoFreteCliente(_adicional.NumeroContratoFreteCliente, null, null, false);
    BuscarPedidos(_adicional.NumeroPedidoOrigem);
    BuscarNavios(_adicional.Balsa, null, null, EnumTipoEmbarcacao.Balsa);

    configurarLayoutPorTipoSistema();

    if (_CONFIGURACAO_TMS.SolicitarValorFretePorTonelada) {
        _adicional.ValorFreteNegociado.visible(false);
        _adicional.ValorFreteTransportadorTerceiro.visible(false);
        _adicional.ValorFreteToneladaNegociado.required(true);
        _adicional.ValorFreteToneladaTerceiro.required(true);
        _adicional.ValorFreteToneladaNegociado.visible(true);
        _adicional.ValorFreteToneladaTerceiro.visible(true);
        _resumoAdicional.BuscarValoresTabelaFrete.visible(true);
    }
    else {
        _adicional.ValorFreteNegociado.visible(true);
        _adicional.ValorFreteTransportadorTerceiro.visible(true);
        _adicional.ValorFreteToneladaNegociado.required(false);
        _adicional.ValorFreteToneladaTerceiro.required(false);
        _adicional.ValorFreteToneladaNegociado.visible(false);
        _adicional.ValorFreteToneladaTerceiro.visible(false);
    }

    if (_CONFIGURACAO_TMS.ExibirCamposRecebimentoPedidoIntegracao) {
        _adicional.IndicativoColetaEntrega.visible(true);
        _adicional.NumeroAutorizacaoColetaEntrega.visible(true);
        _adicional.ClientePropostaComercial.visible(true);
        _adicional.TipoSeguro.visible(true);
    } else {
        _adicional.IndicativoColetaEntrega.visible(false);
        _adicional.NumeroAutorizacaoColetaEntrega.visible(false);
        _adicional.ClientePropostaComercial.visible(false);
        _adicional.TipoSeguro.visible(false);
    }

    if (!_CONFIGURACAO_TMS.UsuarioAdministrador) {
        if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Pedido_PermiteAlterarTipoTomador, _PermissoesPersonalizadas))
            _adicional.UsarTipoTomadorPedido.enable(false);
        if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Pedido_PermitePreencherValoresFrete, _PermissoesPersonalizadas))
            _adicional.ValorFreteNegociado.enable(false);
    }

    if (_CONFIGURACAO_TMS.GerarCargaAutomaticamenteNoPedido) {
        _adicional.GerarAutomaticamenteCargaDoPedido.val(false);
    } else {
        _adicional.GerarAutomaticamenteCargaDoPedido.val(true)
    }

    if (_CONFIGURACAO_TMS.AtualizarNumeroPedidoVinculado) {
        _adicional.EssePedidopossuiPedidoBonificacao.visible(true)
        _adicional.EssePedidopossuiPedidoVenda.visible(true)
        _adicional.NumeroPedidoVinculado.visible(true)
    }
}

function BuscarValoresTabelaFreteClick(e, sender) {
    let data = {
        DataColeta: _pedido.DataColeta.val(),
        DataFinalViagem: _adicional.DataPrevisaoEntrega.val(),
        DataInicialViagem: _adicional.DataPrevisaoSaida.val(),
        DataVigencia: _pedido.DataColeta.val(),
        Destino: _localidadeDestino.Localidade.codEntity(),
        CNPJClienteAtivo: _pedido.Remetente.codEntity(),
        CodigoLocalidadeDestino: _localidadeDestino.Localidade.codEntity(),
        CodigoLocalidadeOrigem: _pedido.Origem.codEntity(),
        CodigoGrupoPessoa: _pedido.GrupoPessoa.codEntity(),
        CodigoModeloVeicular: _pedido.ModeloVeicularCarga.codEntity(),
        Distancia: _adicional.Distancia.val(),
        EscoltaArmada: _adicional.EscoltaArmada.val(),
        GerenciamentoRisco: _adicional.GerenciamentoRisco.val(),
        NumeroAjudantes: _adicional.QtdAjudantes.val(),
        NumeroDeslocamento: _adicional.ValorDeslocamento.val(),
        NumeroDiarias: _adicional.ValorDiaria.val(),
        NumeroEntregas: _adicional.QtdEntregas.val(),
        NumeroPallets: _pedido.PalletsFracionado.val(),
        NumeroPedidos: 1,
        Origem: _pedido.Origem.codEntity(),
        Peso: _pedido.PesoTotalCarga.val(),
        PesoCubado: _pedido.PesoTotalCarga.val(),
        QuantidadeNotasFiscais: 0,
        Rastreado: _adicional.Rastreado.val(),
        CNPJDestinatario: _pedido.Destinatario.codEntity(),
        CodigoTipoDeCarga: _pedido.TipoCarga.codEntity(),
        CodigoTipoOperacao: _pedido.TipoOperacao.codEntity(),
        ValorNotasFiscais: _adicional.ValorTotalNotasFiscais.val(),
        Volumes: _adicional.QtVolumes.val(),
        PesoTotal: _pedido.PesoTotalCarga.val(),
        PagamentoTerceiro: false
    };

    executarReST("CotacaoPedido/BuscarValoresTabelafrete", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (arg.Data.FreteCalculado === false)
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Pedidos.Pedido.NaoFoiPossivelCalcularValores, arg.Data.MensagemRetorno);

                let valorFrete = Globalize.parseFloat(arg.Data.ValorFrete);
                if (isNaN(valorFrete))
                    valorFrete = 0;

                if (arg.Data.Componentes != null) {
                    $.each(arg.Data.Componentes, function (i, componente) {
                        let valorComponente = Globalize.parseFloat(componente.ValorTotal);
                        if (isNaN(valorComponente))
                            valorComponente = 0;

                        valorFrete = valorFrete + valorComponente;
                    });
                }
                if (valorFrete > 0)
                    _adicional.ValorFreteNegociado.val(Globalize.format(valorFrete, "n2"));
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Data.MensagemRetorno);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
        }
    }, BuscarValorTerceiro());
}

function BuscarValorTerceiro() {
    let data = {
        DataColeta: _pedido.DataColeta.val(),
        DataFinalViagem: _adicional.DataPrevisaoEntrega.val(),
        DataInicialViagem: _adicional.DataPrevisaoSaida.val(),
        DataVigencia: _pedido.DataColeta.val(),
        Destino: _localidadeDestino.Localidade.codEntity(),
        CNPJClienteAtivo: _pedido.Remetente.codEntity(),
        CodigoLocalidadeDestino: _localidadeDestino.Localidade.codEntity(),
        CodigoLocalidadeOrigem: _pedido.Origem.codEntity(),
        CodigoGrupoPessoa: _pedido.GrupoPessoa.codEntity(),
        CodigoModeloVeicular: _pedido.ModeloVeicularCarga.codEntity(),
        Distancia: _adicional.Distancia.val(),
        EscoltaArmada: _adicional.EscoltaArmada.val(),
        GerenciamentoRisco: _adicional.GerenciamentoRisco.val(),
        NumeroAjudantes: _adicional.QtdAjudantes.val(),
        NumeroDeslocamento: _adicional.ValorDeslocamento.val(),
        NumeroDiarias: _adicional.ValorDiaria.val(),
        NumeroEntregas: _adicional.QtdEntregas.val(),
        NumeroPallets: _pedido.PalletsFracionado.val(),
        NumeroPedidos: 1,
        Origem: _pedido.Origem.codEntity(),
        Peso: _pedido.PesoTotalCarga.val(),
        PesoCubado: _pedido.PesoTotalCarga.val(),
        QuantidadeNotasFiscais: 0,
        Rastreado: _adicional.Rastreado.val(),
        CNPJDestinatario: _pedido.Destinatario.codEntity(),
        CodigoTipoDeCarga: _pedido.TipoCarga.codEntity(),
        CodigoTipoOperacao: _pedido.TipoOperacao.codEntity(),
        ValorNotasFiscais: _adicional.ValorTotalNotasFiscais.val(),
        Volumes: _adicional.QtVolumes.val(),
        PesoTotal: _pedido.PesoTotalCarga.val(),
        PagamentoTerceiro: true
    };

    executarReST("CotacaoPedido/BuscarValoresTabelafrete", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (arg.Data.FreteCalculado === false)
                    return;

                let valorFrete = Globalize.parseFloat(arg.Data.ValorFrete);
                if (isNaN(valorFrete))
                    valorFrete = 0;

                if (arg.Data.Componentes != null) {
                    $.each(arg.Data.Componentes, function (i, componente) {
                        let valorComponente = Globalize.parseFloat(componente.ValorTotal);
                        if (isNaN(valorComponente))
                            valorComponente = 0;

                        valorFrete = valorFrete + valorComponente;
                    });
                }
                if (valorFrete > 0)
                    _adicional.ValorFreteTransportadorTerceiro.val(Globalize.format(valorFrete, "n2"));
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Data.MensagemRetorno);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
        }
    });
}

function RetornoTomador(data) {
    if (data.GrupoPessoasBloqueado) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pedidos.Pedido.GrupoPessoasBloqueadas, Localization.Resources.Pedidos.Pedido.GrupoPessoasClienteBloqueadas.format(data.GrupoPessoasMotivoBloqueio));

        _adicional.Tomador.codEntity(0);
        _adicional.Tomador.val('');

        return;
    }

    if (data.Bloqueado) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pedidos.Pedido.PessoaBloqueada, Localization.Resources.Pedidos.Pedido.ClienteBloqueado.format(data.MotivoBloqueio));

        _adicional.Tomador.codEntity(0);
        _adicional.Tomador.val('');

        return;
    }

    _adicional.Tomador.codEntity(data.Codigo);
    _adicional.Tomador.val(data.Descricao);

    obterCondicaoPedidoPorTomador();
}

function RetornoLocalPaletizacao(data) {
    _adicional.LocalPaletizacao.codEntity(data.Codigo);
    _adicional.LocalPaletizacao.val(data.Descricao);
}

function RetornoPedidoViagemNavio(data) {
    _adicional.PedidoViagemNavio.codEntity(data.Codigo);
    _adicional.PedidoViagemNavio.val(data.Descricao);

    if (data.CodigoNavio > 0) {
        _importacao.Navio.codEntity(data.CodigoNavio);
        _importacao.Navio.val(data.DescricaoNavio);
    }
    _importacao.DirecaoViagemMultimodal.val(data.Direcao);
}

function RetornoPedidoTipoPagamento(data) {
    _adicional.PedidoTipoPagamento.codEntity(data.Codigo);
    _adicional.PedidoTipoPagamento.val(data.Descricao);
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        if (data.ObservacaoPedido != "") {
            _adicional.ObservacaoCTe.val(data.ObservacaoPedido);
            _adicional.ImprimirObservacaoCTe.val(true);
        } else {
            _adicional.ObservacaoCTe.val("");
            _adicional.ImprimirObservacaoCTe.val(false);
        }
    }
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function seguroChange() {
    if ($("#" + _adicional.Seguro.id).is(':checked'))
        _adicional.ValorTotalCarga.enable(true);
    else {
        _adicional.ValorTotalCarga.enable(false);
        _adicional.ValorTotalCarga.val("");
    }
}

function tipoPagamentoChange() {
    if (_adicional.TipoPagamento.val() == EnumTipoPagamento.Outros) {
        _adicional.Tomador.required = true;
        _adicional.Tomador.text(Localization.Resources.Pedidos.Pedido.Tomador.getRequiredFieldDescription());
    }
    else {
        _adicional.Tomador.required = false;
        _adicional.Tomador.text(Localization.Resources.Pedidos.Pedido.Tomador.getFieldDescription());
    }

    obterCondicaoPedidoPorTomador();
}

function tipoTomadorChange() {
    obterCondicaoPedidoPorTomador();
}

/*
 * Declaração das Funções Públicas
 */

function limparCamposDadosAdicionais() {
    LimparCampos(_adicional);
    LimparCampos(_resumoAdicional);

    _adicional.Tomador.required = false;
    _adicional.Tomador.text(Localization.Resources.Pedidos.Pedido.Tomador.getRequiredFieldDescription());
    _adicional.ValorTotalCarga.enable(false);
    _adicional.ValorTotalCarga.val("");
    _adicional.Seguro.val(false);
    _adicional.DespachoTransitoAduaneiro.val(false);

    _adicional.PercentualAdiantamentoTerceiro.val(_CONFIGURACAO_TMS.PercentualAdiantamentoTerceiroPadrao);
    _adicional.PercentualMinimoAdiantamentoTerceiro.val(_CONFIGURACAO_TMS.PercentualMinimoAdiantamentoTerceiroPadrao);
    _adicional.PercentualMaximoAdiantamentoTerceiro.val(_CONFIGURACAO_TMS.PercentualMaximoAdiantamentoTerceiroPadrao);

    if (_CONFIGURACAO_TMS.AtualizarNumeroPedidoVinculado) {
        _adicional.EssePedidopossuiPedidoBonificacao.enable(true)
        _adicional.EssePedidopossuiPedidoVenda.enable(true)
        _adicional.NumeroPedidoVinculado.enable(true)
    }

    loadDadosUsuarioLogado();
}

function limparCamposDadosAdicionaisDuplicar() {
    _adicional.DataPrevisaoSaida.val("");
    _adicional.DataPrevisaoEntrega.val("");
    _adicional.DataInicialViagemExecutada.val("");
    _adicional.DataFinalViagemExecutada.val("");
    _adicional.DataInicialViagemFaturada.val("");
    _adicional.DataFinalViagemFaturada.val("");
    _adicional.DataAgendamento.val("");
    loadDadosUsuarioLogado();
}

function preencherDadosAdicionais(dadosAdicionais) {
    const objetosEstender = ['DatasHorarios', 'Validacoes'];

    for (let c = 0; c < objetosEstender.length; c++)
        $.extend(dadosAdicionais, dadosAdicionais[objetosEstender[c]]);

    // Caso o Centro de Custo de viagem não esteja preenchido, busca os dados do usuário logado. Somente para o MultiTMS.
    if ((_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) && (dadosAdicionais.CentroDeCustoViagem && dadosAdicionais.CentroDeCustoViagem.Codigo == 0)) {
        loadDadosUsuarioLogado();
    }

    PreencherObjetoKnout(_adicional, { Data: dadosAdicionais });

    if (!_CONFIGURACAO_TMS.ExibirPedidoDeColeta)
        _adicional.DisponibilizarPedidoParaColeta.val(false);

    seguroChange();
    //tipoPagamentoChange();

    _adicional.EssePedidopossuiPedidoBonificacao.enable(false);
    _adicional.EssePedidopossuiPedidoVenda.enable(false);
    _adicional.NumeroPedidoVinculado.enable(false);
}

function preencherDadosAdicionaisSalvar(pedido) {
    let adicional = RetornarObjetoPesquisa(_adicional);
    $.extend(pedido, adicional);
}

function verificarDadosAdicionais() {
    if (_pedido.TelaResumida.val())
        return true;

    if (ValidarCamposObrigatorios(_adicional))
        return true;

    $("#myTab a:eq(4)").tab("show");
    exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CampoObrigatorio, Localization.Resources.Pedidos.Pedido.InformeCampoObrigatorio);
    return false;
}

function obterCondicaoPedidoPorTomador() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        let tomador = RetornoTomadorPedido();
        if (tomador == null)
            return;
        let codigoTomador = RetornoTomadorPedido().codEntity();
        if (codigoTomador > 0) {
            executarReST("Pedido/ObterCondicaoPedidoPorTomador", { Tomador: codigoTomador, TipoOperacao: _pedido.TipoOperacao.codEntity() }, function (arg) {
                if (arg.Success) {
                    if (arg.Data) {
                        let data = arg.Data;
                        _adicional.PedidoTipoPagamento.codEntity(data.Codigo);
                        _adicional.PedidoTipoPagamento.val(data.Descricao);

                        _resumoAdicional.FormaPagamento.val(data.FormaPagamento);
                        _resumoAdicional.DiasDePrazoFatura.val(data.DiasDePrazoFatura);
                    } else {
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.falha, arg.Msg);
                }
            });
        } else {
            _adicional.PedidoTipoPagamento.codEntity(0);
            _adicional.PedidoTipoPagamento.val("");
        }
    }
}

/*
 * Declaração das Funções Privadas
 */

function VerificarCamposElementoPEPCentroResultadoPedidoAdicionais() {
    _adicional.ElementoPEP.enable(true);
    _adicional.CentroResultadoEmbarcador.enable(true);

    if (_adicional.CentroResultadoEmbarcador.val() != "" && _adicional.CentroResultadoEmbarcador.codEntity() > 0) {
        _adicional.ElementoPEP.enable(false);
        _adicional.ElementoPEP.val("");
    }
    if (_adicional.ElementoPEP.val() != "") {
        _adicional.CentroResultadoEmbarcador.enable(false);
        _adicional.CentroResultadoEmbarcador.val("");
    }
}

function configurarLayoutPorTipoSistema() {

    _adicional.NumeroProtocoloIntegracaoPedido.visible(true);
    _adicional.NumeroProtocoloIntegracaoCarga.visible(true);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _adicional.NumeroEXP.visible(true);
        _resumoAdicional.BuscarValoresTabelaFrete.visible(false);
        _adicional.Safra.visible(false);
        _adicional.PercentualAdiantamentoTerceiro.visible(false);
        _adicional.PercentualMinimoAdiantamentoTerceiro.visible(false);
        _adicional.PercentualMaximoAdiantamentoTerceiro.visible(false);
        _adicional.ObservacaoEntrega.visible(true);
        _adicional.ObservacaoEntrega.enable(false);
    }
    else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {

        if (_CONFIGURACAO_TMS.InformarDataViagemExecutadaPedido) {
            _adicional.DataFinalViagemExecutada.enable(true);
            _adicional.DataInicialViagemExecutada.enable(true);
        }

        if (_CONFIGURACAO_TMS.SolicitarValorFretePorTonelada) {
            _adicional.PercentualAdiantamentoTerceiro.visible(true);
            _adicional.PercentualMinimoAdiantamentoTerceiro.visible(true);
            _adicional.PercentualMaximoAdiantamentoTerceiro.visible(true);
            _adicional.PercentualAdiantamentoTerceiro.val(_CONFIGURACAO_TMS.PercentualAdiantamentoTerceiroPadrao);
            _adicional.PercentualMinimoAdiantamentoTerceiro.val(_CONFIGURACAO_TMS.PercentualMinimoAdiantamentoTerceiroPadrao);
            _adicional.PercentualMaximoAdiantamentoTerceiro.val(_CONFIGURACAO_TMS.PercentualMaximoAdiantamentoTerceiroPadrao);
            _resumoAdicional.BuscarValoresTabelaFrete.visible(true);
        }
        else {
            _adicional.PercentualAdiantamentoTerceiro.visible(false);
            _adicional.PercentualMinimoAdiantamentoTerceiro.visible(false);
            _adicional.PercentualMaximoAdiantamentoTerceiro.visible(false);
        }

        if (_CONFIGURACAO_TMS.PermitirBuscarValoresTabelaFrete) {
            _resumoAdicional.BuscarValoresTabelaFrete.visible(true);
        }

        _adicional.Safra.visible(true);
        _adicional.ProdutoPredominante.visible(true);
        _adicional.ValorPedagioRota.visible(true);
        _adicional.SerieCTe.visible(false);
        _adicional.SerieCTe.cssClass("col col-xs-12 col-sm-12 col-md-12 col-lg-4");
        _adicional.Tomador.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-6");
        _adicional.DataPrevisaoEntrega.text("Data previsão de entrega/retorno:");
        _adicional.PesoTotalCargaTMS.visible(true);
        _adicional.CubagemTotalTMS.visible(true);
        _adicional.QtdEntregas.visible(true);

        _adicional.PedidoTipoPagamento.visible(true);
        _adicional.NecessarioReentrega.visible(true);
        _adicional.Rastreado.visible(true);
        _adicional.GerenciamentoRisco.visible(true);
        _adicional.Seguro.visible(true);
        _adicional.ValorTotalCarga.visible(true);
        _adicional.DespachoTransitoAduaneiro.visible(true);
        _adicional.Cotacao.visible(true);
        _adicional.DataInicialViagemExecutada.visible(true);
        _adicional.DataFinalViagemExecutada.visible(true);
        _adicional.DataInicialViagemFaturada.visible(true);
        _adicional.DataFinalViagemFaturada.visible(true);
        _adicional.ObservacaoInterna.visible(true);
        _adicional.PossuiCarregamento.visible(true);
        _adicional.PossuiDescarga.visible(true);

        if (_CONFIGURACAO_TMS.CamposSecundariosObrigatoriosPedido) {
            _adicional.DataAgendamento.required = true;
            _adicional.ObservacaoInterna.required = true;

            _adicional.PossuiCarregamento.val(true);
            _adicional.PossuiCarregamento.def = true;

            _adicional.PossuiDescarga.val(true);
            _adicional.PossuiDescarga.def = true;

            _adicional.Requisitante.val("");
            _adicional.Requisitante.def = "";
            _adicional.Requisitante.required = true;

            _adicional.TipoPagamento.val("");
            _adicional.TipoPagamento.def = "";
            _adicional.TipoPagamento.required = true;

            _adicional.PesoTotalCargaTMS.required = true;
            _adicional.QtdEntregas.required = true;
            _adicional.CubagemTotalTMS.required = true;
            _adicional.QtVolumes.required = true;

            _adicional.Ajudante.val(true);
            _adicional.Ajudante.def = true;

            _adicional.QuantidadeNotasFiscais.visible(true);
            _adicional.QuantidadeNotasFiscais.required = true;

            _adicional.PedidoTipoPagamento.required = true;
        }

        $("#knockoutResumoAdicionais").show();
    }

    if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) {
        _adicional.TagNumeroPedidoCliente.visible(true);
        _adicional.TagPedidoViagemNavio.visible(true);
        _adicional.TagQuantidadeETipoContainer.visible(true);
        _adicional.TagPortoOrigem.visible(true);
        _adicional.TagPortoDestino.visible(true);
        _adicional.TagValorNota.visible(true);
        _adicional.TagValorADValorem.visible(true);

        _adicional.ValorAdValorem.visible(true);
        _adicional.IDBAF.visible(true);
        _adicional.ValorBAF.visible(true);
        _adicional.NumeroProposta.visible(true);
        _adicional.PedidoEmpresaResponsavel.visible(true);
        _adicional.PedidoCentroCusto.visible(true);

        _adicional.NecessitaAverbacaoAutomatica.visible(true);
        _adicional.ValidarDigitoVerificadorContainer.visible(true);
        _adicional.PedidoSVM.visible(true);
        _adicional.PedidoDeSVMTerceiro.visible(true);
        _adicional.ContainerADefinir.visible(true);

        _adicional.Distancia.visible(false);

        SetarCampoAbaAdicionais(_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal);
    }
}

var obterVisivilidade = () => (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)

var loadDadosUsuarioLogado = function () {
    executarReST("Usuario/DadosUsuarioLogado", {}, function (arg) {
        if (arg.Success && _adicional.CentroDeCustoViagem)
            if (arg.Data !== false && arg.Data != null) {
                _usuarioLogado = arg.Data;
                loadDadosComplementaresUsuarioLogado();
            }
    });
}

// Carrega todos os campos que precisarem incluir o usuário logado automaticamente
var loadDadosComplementaresUsuarioLogado = function () {
    executarReST("Usuario/BuscarPorCodigo", { Codigo: _usuarioLogado.Codigo }, function (arg) {
        if (arg.Success && _adicional.CentroDeCustoViagem) {
            if (arg.Data !== false && arg.Data != null && arg.Data.CentroDeCustoSetorTurno) {
                _usuarioLogadoCentroCustoViagem = arg.Data.CentroDeCustoSetorTurno;
                _adicional.CentroDeCustoViagem.val(_usuarioLogadoCentroCustoViagem.Descricao);
                _adicional.CentroDeCustoViagem.codEntity(_usuarioLogadoCentroCustoViagem.Codigo);
            }
        }
    });

}

function SetarCampoAbaAdicionais(exibir) {
    _adicional.ProvedorOS.visible(exibir);
    _adicional.PedidoViagemNavio.visible(exibir);
    _adicional.NecessitaEnergiaContainerRefrigerado.visible(exibir);
    _adicional.NecessitaAverbacaoAutomatica.visible(exibir);
}