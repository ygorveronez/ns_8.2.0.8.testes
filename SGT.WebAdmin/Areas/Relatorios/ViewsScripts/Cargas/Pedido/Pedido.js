/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CentroCarregamento.js" />
/// <reference path="../../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/Rest.js" />
/// <reference path="../../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../../js/Global/Grid.js" />
/// <reference path="../../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Localidade.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Carga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/PedidoViagemNavio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Porto.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CentroCustoViagem.js" />
/// <reference path="../../../../../js/app.config.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoPedido.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoLocalPrestacao.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoEntrega.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoPropostaMultimodal.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoCargaMercante.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridPedido, _pesquisaPedido, _CRUDRelatorio, _relatorioPedido, _CRUDFiltrosRelatorio;

//var _situacaoCargaEmbarcador = [
//    { text: Localization.Resources.Cargas.Pedido.ComLogistica, value: EnumSituacoesCarga.NaLogistica },
//    { text: Localization.Resources.Cargas.Pedido.DadosDaCarga, value: EnumSituacoesCarga.Nova },
//    { text: "NF-e", value: EnumSituacoesCarga.AgNFe },
//    { text: Localization.Resources.Cargas.Pedido.CalculoFrete, value: EnumSituacoesCarga.CalculoFrete },
//    { text: Localization.Resources.Gerais.Geral.Transportador, value: EnumSituacoesCarga.AgTransportador },
//    { text: Localization.Resources.Cargas.Pedido.EmissaoDocumentos, value: EnumSituacoesCarga.PendeciaDocumentos },
//    { text: Localization.Resources.Cargas.Pedido.Integracao, value: EnumSituacoesCarga.AgIntegracao },
//    { text: Localization.Resources.Cargas.Pedido.Impressao, value: EnumSituacoesCarga.AgImpressaoDocumentos },
//    { text: Localization.Resources.Cargas.Pedido.EmTransporte, value: EnumSituacoesCarga.EmTransporte },
//    { text: Localization.Resources.Cargas.Pedido.Encerrada, value: EnumSituacoesCarga.Encerrada },
//    { text: Localization.Resources.Cargas.Pedido.PagamentoLiberado, value: EnumSituacoesCarga.LiberadoPagamento },
//    { text: Localization.Resources.Cargas.Pedido.Canceladas, value: EnumSituacoesCarga.Cancelada }
//];

//var _situacaoCargaTMS = [
//    { text: Localization.Resources.Cargas.Pedido.EmAndamento, value: EnumSituacoesCarga.NaLogistica },
//    { text: Localization.Resources.Cargas.Pedido.EtapaUmCarga, value: EnumSituacoesCarga.Nova },
//    { text: Localization.Resources.Cargas.Pedido.EtapaDoisNFe, value: EnumSituacoesCarga.AgNFe },
//    { text: Localization.Resources.Cargas.Pedido.EtapaTresFrete, value: EnumSituacoesCarga.CalculoFrete },
//    { text: Localization.Resources.Cargas.Pedido.EtapaQuatroCincoDoc, value: EnumSituacoesCarga.PendeciaDocumentos },
//    { text: Localization.Resources.Cargas.Pedido.EtapaSeisIntegracao, value: EnumSituacoesCarga.AgIntegracao },
//    { text: Localization.Resources.Cargas.Pedido.EmTransporte, value: EnumSituacoesCarga.EmTransporte },
//    { text: Localization.Resources.Cargas.Pedido.Finalizadas, value: EnumSituacoesCarga.Encerrada },
//    { text: Localization.Resources.Cargas.Pedido.Cancelada, value: EnumSituacoesCarga.Cancelada },
//    { text: Localization.Resources.Cargas.Pedido.Anulada, value: EnumSituacoesCarga.Anulada }
//];

var PesquisaCarga = function () {
    var situacaoCargaPesquisa = _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador ? EnumSituacoesCarga.obterOpcoesEmbarcador() : EnumSituacoesCarga.obterOpcoesTMS();
    var situacoesMarcadasPorPadrao = !_CONFIGURACAO_TMS.UtilizarRelatorioPedidoComoStatusEntrega ? new Array() :
        [EnumSituacoesCarga.NaLogistica, EnumSituacoesCarga.Nova, EnumSituacoesCarga.AgNFe, EnumSituacoesCarga.CalculoFrete, EnumSituacoesCarga.AgTransportador, EnumSituacoesCarga.PendeciaDocumentos, EnumSituacoesCarga.AgIntegracao,
        EnumSituacoesCarga.AgImpressaoDocumentos, EnumSituacoesCarga.EmTransporte, EnumSituacoesCarga.Encerrada, EnumSituacoesCarga.LiberadoPagamento];

    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Pedido.TipoRelatorio, issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: Localization.Resources.Gerais.Geral.DataInicial.getFieldDescription(), getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataFinal = PropertyEntity({ text: Localization.Resources.Gerais.Geral.DataFinal.getFieldDescription(), getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Gerais.Geral.Transportador.getFieldDescription()), issue: 69, idBtnSearch: guid() });
    this.Pedido = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Gerais.Geral.Pedido.getFieldDescription()), idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Filial.getFieldDescription(), issue: 63, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Remetente = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Remetente.getFieldDescription(), issue: 52, idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Destinatario.getFieldDescription(), issue: 52, idBtnSearch: guid() });
    this.UFOrigem = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.UFOrigem.getFieldDescription(), issue: 16, idBtnSearch: guid() });
    this.Origem = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Origem.getFieldDescription(), issue: 16, idBtnSearch: guid() });
    this.UFDestino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.UFDestino.getFieldDescription(), issue: 16, idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Destino.getFieldDescription(), issue: 16, idBtnSearch: guid() });
    this.GrupoPessoas = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.GrupoPessoas.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.TipoCarga.getFieldDescription(), issue: 53, idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.TipoOperacao.getFieldDescription(), issue: 121, idBtnSearch: guid(), visible: ko.observable(true) });
    this.ModeloVeiculo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.ModeloVeiculo.getFieldDescription(), issue: 156, idBtnSearch: guid() });
    this.RotaFrete = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.RotaFrete.getFieldDescription(), idBtnSearch: guid() });
    this.Restricao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Restricao.getFieldDescription(), idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Veiculo.getFieldDescription(), issue: 143, idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Motorista.getFieldDescription(), issue: 145, idBtnSearch: guid() });
    this.TipoLocalPrestacao = PropertyEntity({ val: ko.observable(EnumTipoLocalPrestacao.todos), options: EnumTipoLocalPrestacao.obterOpcoesPesquisa(), def: EnumTipoLocalPrestacao.todos, text: Localization.Resources.Gerais.Geral.TipoLocalPrestacao.getFieldDescription() });
    this.ExibirProdutos = PropertyEntity({ text: Localization.Resources.Gerais.Geral.ExibirProdutos.getFieldDescription(), val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.PedidosSemCarga = PropertyEntity({ text: Localization.Resources.Gerais.Geral.PedidosSemCarga.getFieldDescription(), val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.DataCarregamentoInicial = PropertyEntity({ text: Localization.Resources.Cargas.Pedido.DataCarregamentoInicial.getFieldDescription(), getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataCarregamentoFinal = PropertyEntity({ text: Localization.Resources.Cargas.Pedido.DataCarregamentoFinal.getFieldDescription(), getType: typesKnockout.date, val: ko.observable(""), def: "" });

    this.DataInicioJanela = PropertyEntity({ text: Localization.Resources.Cargas.Pedido.DataInicioJanela.getFieldDescription(), getType: typesKnockout.date, val: ko.observable(""), def: "" });

    this.SituacoesPedido = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, text: Localization.Resources.Cargas.Pedido.SituacoesPedido.getFieldDescription(), options: EnumSituacaoPedido.obterOpcoes(), visible: ko.observable(true) });
    this.SituacoesEntrega = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, text: Localization.Resources.Cargas.Pedido.SituacoesEntrega.getFieldDescription(), options: EnumSituacaoEntrega.obterOpcoes(), visible: ko.observable(true) });
    this.Situacoes = PropertyEntity({ val: ko.observable(situacoesMarcadasPorPadrao), def: situacoesMarcadasPorPadrao, getType: typesKnockout.selectMultiple, text: Localization.Resources.Cargas.Pedido.Situacoes.getFieldDescription(), issue: 553, options: ko.observable(situacaoCargaPesquisa), visible: ko.observable(true) });
    this.SituacaoCargaMercante = PropertyEntity({ text: Localization.Resources.Gerais.Geral.SituacaoCargaMercante, getType: typesKnockout.selectMultiple, val: ko.observable([]), options: EnumSituacaoCargaMercante.obterOpcoes(), def: [], visible: ko.observable(false) });

    this.DeliveryTerm = PropertyEntity({ text: Localization.Resources.Cargas.Pedido.DeliveryTerm.getFieldDescription(), maxlength: 150 });
    this.DataCriacaoPedidoInicial = PropertyEntity({ text: Localization.Resources.Cargas.Pedido.DataCriacaoPedidoInicial, getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataCriacaoPedidoFinal = PropertyEntity({ text: Localization.Resources.Cargas.Pedido.DataCriacaoPedidoFinal, getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.IdAutorizacao = PropertyEntity({ text: Localization.Resources.Cargas.Pedido.IdAutorizacao.getFieldDescription(), maxlength: 150 });
    this.SomenteComReserva = PropertyEntity({ text: Localization.Resources.Cargas.Pedido.SomenteComReserva.getFieldDescription(), type: types.bool, val: ko.observable(false), def: false });
    this.SomentePedidosCanceladosAposVincularCarga = PropertyEntity({ text: Localization.Resources.Cargas.Pedido.SomentePedidosCanceladosAposVincularCarga, type: types.bool, val: ko.observable(false), def: false });
    this.SomentePedidosDeIntegracao = PropertyEntity({ text: Localization.Resources.Cargas.Pedido.SomentePedidosDeIntegracao, type: types.bool, val: ko.observable(false), def: false });
    this.UtilizarDadosDasCargasAgrupadas = PropertyEntity({ text: Localization.Resources.Cargas.Pedido.UtilizarDadosDasCargasAgrupadas, type: types.bool, val: ko.observable(_CONFIGURACAO_TMS.UtilizarRelatorioPedidoComoStatusEntrega), def: _CONFIGURACAO_TMS.UtilizarRelatorioPedidoComoStatusEntrega });
    this.NumeroCarga = PropertyEntity({ text: Localization.Resources.Cargas.Pedido.NumeroCarga.getFieldDescription(), maxlength: 50 });
    this.NumeroPedido = PropertyEntity({ text: Localization.Resources.Cargas.Pedido.NumeroPedido.getFieldDescription(), maxlength: 50 });
    this.Expedidor = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Expedidor.getFieldDescription(), idBtnSearch: guid() });
    this.Gerente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Gerente.getFieldDescription(), idBtnSearch: guid() });
    this.Supervisor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Supervisor.getFieldDescription(), idBtnSearch: guid() });
    this.Vendedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Vendedor.getFieldDescription(), idBtnSearch: guid() });
    this.NumeroPedidoCliente = PropertyEntity({ text: Localization.Resources.Gerais.Geral.NumeroPedidoCliente.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.ExibirCargasAgrupadas = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Gerais.Geral.ExibirCargasAgrupadas.getFieldDescription(), visible: ko.observable(false) });
    this.TipoPropostaMultiModal = PropertyEntity({ val: ko.observable(EnumTipoPropostaMultimodal.Todos), def: EnumTipoPropostaMultimodal.Todos, getType: typesKnockout.selectMultiple, text: Localization.Resources.Gerais.Geral.TipoPropostaMultiModal.getFieldDescription(), options: ko.observable(EnumTipoPropostaMultimodal.obterOpcoes()), visible: ko.observable(true) });
    this.NumeroViagemNavio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.NumeroViagemNavio.getFieldDescription(), idBtnSearch: guid() });
    this.PortoOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.PortoOrigem.getFieldDescription(), idBtnSearch: guid() });
    this.PortoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.PortoDestino.getFieldDescription(), idBtnSearch: guid() });

    this.PossuiRecebedor = PropertyEntity({ text: Localization.Resources.Gerais.Geral.PossuiRecebedor.getFieldDescription(), val: ko.observable(null), options: Global.ObterOpcoesPesquisaBooleano(Localization.Resources.Gerais.Geral.Sim, Localization.Resources.Gerais.Geral.Nao), def: null });
    this.PossuiExpedidor = PropertyEntity({ text: Localization.Resources.Gerais.Geral.PossuiExpedidor.getFieldDescription(), val: ko.observable(null), options: Global.ObterOpcoesPesquisaBooleano(Localization.Resources.Gerais.Geral.Sim, Localization.Resources.Gerais.Geral.Nao), def: null });
    this.AguardandoIntegracao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.AguardandoIntegracao.getFieldDescription(), val: ko.observable(EnumAguardandoIntegracao.Ambos), options: EnumAguardandoIntegracao.obterOpcoesPesquisa() });

    this.PrevisaoDataInicial = PropertyEntity({ text: Localization.Resources.Cargas.Pedido.PrevisaoDataInicial.getFieldDescription(), getType: typesKnockout.date });
    this.PrevisaoDataFinal = PropertyEntity({ text: Localization.Resources.Cargas.Pedido.PrevisaoDataFinal, getType: typesKnockout.date });
    this.DataInclusaoBookingInicial = PropertyEntity({ text: Localization.Resources.Cargas.Pedido.DataInclusaoBookingInicial.getFieldDescription(), getType: typesKnockout.date });
    this.DataInclusaoBookingLimite = PropertyEntity({ text: Localization.Resources.Cargas.Pedido.DataInclusaoBookingLimite, getType: typesKnockout.date });
    this.DataInclusaoPCPInicial = PropertyEntity({ text: Localization.Resources.Cargas.Pedido.DataInclusaoPCPInicial.getFieldDescription(), getType: typesKnockout.date });
    this.DataInclusaoPCPLimite = PropertyEntity({ text: Localization.Resources.Cargas.Pedido.DataInclusaoPCPLimite.getFieldDescription(), getType: typesKnockout.date });
    this.DataCriacaoPedidoERP = PropertyEntity({ text: Localization.Resources.Cargas.Pedido.DataCriacaoPedidoERP.getFieldDescription(), getType: typesKnockout.date, visible: ko.observable(_CONFIGURACAO_TMS.PermiteReceberDataCriacaoPedidoERP) });
    this.DataInicioViagemInicial = PropertyEntity({ text: Localization.Resources.Cargas.Pedido.DataInicioViagemInicial.getFieldDescription(), getType: typesKnockout.date });
    this.DataInicioViagemFinal = PropertyEntity({ text: Localization.Resources.Cargas.Pedido.DataInicioViagemFinal.getFieldDescription(), getType: typesKnockout.date });
    this.DataEntregaInicial = PropertyEntity({ text: Localization.Resources.Cargas.Pedido.DataEntregaInicial.getFieldDescription(), getType: typesKnockout.date });
    this.DataEntregaFinal = PropertyEntity({ text: Localization.Resources.Cargas.Pedido.DataEntregaFinal.getFieldDescription(), getType: typesKnockout.date });

    this.PrevisaoEntregaPedidoDataInicial = PropertyEntity({ text: Localization.Resources.Cargas.Pedido.PrevisaoEntregaPedidoDataInicial.getFieldDescription(), getType: typesKnockout.date });
    this.PrevisaoEntregaPedidoDataFinal = PropertyEntity({ text: Localization.Resources.Cargas.Pedido.PrevisaoEntregaPedidoDataFinal.getFieldDescription(), getType: typesKnockout.date });

    this.DataETAPortoOrigemInicial = PropertyEntity({ text: Localization.Resources.Cargas.Pedido.DataETAPortoOrigemInicial.getFieldDescription(), getType: typesKnockout.date, visible: ko.observable(false) });
    this.DataETAPortoOrigemFinal = PropertyEntity({ text: Localization.Resources.Cargas.Pedido.DataETAPortoOrigemFinal.getFieldDescription(), getType: typesKnockout.date, visible: ko.observable(false) });
    this.DataETSPortoOrigemInicial = PropertyEntity({ text: Localization.Resources.Cargas.Pedido.DataETSPortoOrigemInicial.getFieldDescription(), getType: typesKnockout.date, visible: ko.observable(false) });
    this.DataETSPortoOrigemFinal = PropertyEntity({ text: Localization.Resources.Cargas.Pedido.DataETSPortoOrigemFinal.getFieldDescription(), getType: typesKnockout.date, visible: ko.observable(false) });
    this.DataETAPortoDestinoInicial = PropertyEntity({ text: Localization.Resources.Cargas.Pedido.DataETAPortoDestinoInicial.getFieldDescription(), getType: typesKnockout.date, visible: ko.observable(false) });
    this.DataETAPortoDestinoFinal = PropertyEntity({ text: Localization.Resources.Cargas.Pedido.DataETAPortoDestinoFinal.getFieldDescription(), getType: typesKnockout.date, visible: ko.observable(false) });
    this.DataETSPortoDestinoInicial = PropertyEntity({ text: Localization.Resources.Cargas.Pedido.DataETSPortoDestinoInicial.getFieldDescription(), getType: typesKnockout.date, visible: ko.observable(false) });
    this.DataETSPortoDestinoFinal = PropertyEntity({ text: Localization.Resources.Cargas.Pedido.DataETSPortoDestinoFinal.getFieldDescription(), getType: typesKnockout.date, visible: ko.observable(false) });
    this.DataInclusaoPedidoInicial = PropertyEntity({ text: Localization.Resources.Cargas.Pedido.DataInclusaoPedidoInicial.getFieldDescription(), getType: typesKnockout.date, visible: ko.observable(false) });
    this.DataInclusaoPedidoFinal = PropertyEntity({ text: Localization.Resources.Cargas.Pedido.DataInclusaoPedidoFinal.getFieldDescription(), getType: typesKnockout.date, visible: ko.observable(false) });
    this.OperadorPedido = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Pedido.OperadorPedido.getFieldDescription(), idBtnSearch: guid() });
    this.CentroResultado = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Pedido.CentroResultado.getFieldDescription(), idBtnSearch: guid() });
    this.CentroDeCustoViagemCodigo = PropertyEntity({ type: types.entity, codEntity: ko.observable(null), required: false, text: "Centro de Custo Viagem", idBtnSearch: guid(), visible: ko.observable(true) });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;
    this.DataCarregamentoInicial.dateRangeLimit = this.DataCarregamentoFinal;
    this.DataCarregamentoFinal.dateRangeInit = this.DataCarregamentoInicial;
    this.DataInclusaoBookingInicial.dateRangeLimit = this.DataInclusaoBookingLimite;
    this.DataInclusaoBookingLimite.dateRangeInit = this.DataInclusaoBookingInicial;
    this.DataInclusaoPCPInicial.dateRangeLimit = this.DataInclusaoPCPLimite;
    this.DataInclusaoPCPLimite.dateRangeInit = this.DataInclusaoPCPInicial;
    this.DataInicioViagemInicial.dateRangeLimit = this.DataInicioViagemFinal;
    this.DataInicioViagemFinal.dateRangeInit = this.DataInicioViagemInicial;
    this.DataEntregaInicial.dateRangeLimit = this.DataEntregaFinal;
    this.DataEntregaFinal.dateRangeInit = this.DataEntregaInicial;
    this.PrevisaoDataInicial.dateRangeLimit = this.PrevisaoDataFinal;
    this.PrevisaoDataFinal.dateRangeInit = this.PrevisaoDataInicial;
    this.PrevisaoEntregaPedidoDataInicial.dateRangeLimit = this.PrevisaoEntregaPedidoDataFinal;
    this.PrevisaoEntregaPedidoDataFinal.dateRangeInit = this.PrevisaoEntregaPedidoDataInicial;
    this.DataETAPortoOrigemInicial.dateRangeLimit = this.DataETAPortoOrigemFinal;
    this.DataETAPortoOrigemFinal.dateRangeInit = this.DataETAPortoOrigemInicial;
    this.DataETSPortoOrigemInicial.dateRangeLimit = this.DataETSPortoOrigemFinal;
    this.DataETSPortoOrigemFinal.dateRangeInit = this.DataETSPortoOrigemInicial;
    this.DataETAPortoDestinoInicial.dateRangeLimit = this.DataETAPortoDestinoFinal;
    this.DataETAPortoDestinoFinal.dateRangeInit = this.DataETAPortoDestinoInicial;
    this.DataETSPortoDestinoInicial.dateRangeLimit = this.DataETSPortoDestinoFinal;
    this.DataETSPortoDestinoFinal.dateRangeInit = this.DataETSPortoDestinoInicial;
    this.DataInclusaoPedidoInicial.dateRangeLimit = this.DataInclusaoPedidoFinal;
    this.DataInclusaoPedidoFinal.dateRangeInit = this.DataInclusaoPedidoInicial;
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridPedido.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Cargas.Pedido.Preview, idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaPedido.Visible.visibleFade()) {
                _pesquisaPedido.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaPedido.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: Localization.Resources.Cargas.Pedido.Avancada, icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: Localization.Resources.Gerais.Geral.GerarPDF });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: Localization.Resources.Gerais.Geral.GerarPlanilhaExcel, idGrid: guid() });
};

//*******EVENTOS*******

function LoadRelatorioPedido() {
    _pesquisaPedido = new PesquisaCarga();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridPedido = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "/Relatorios/Pedido/Pesquisa", _pesquisaPedido, null, null, 10, null, null, null, null, 20);
    _gridPedido.SetPermitirEdicaoColunas(true);

    _relatorioPedido = new RelatorioGlobal("Relatorios/Pedido/BuscarDadosRelatorio", _gridPedido, function () {
        _relatorioPedido.loadRelatorio(function () {
            KoBindings(_pesquisaPedido, "knockoutPesquisaPedido");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaPedido");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaPedidos");

            new BuscarTransportadores(_pesquisaPedido.Transportador, null, null, true);
            new BuscarVeiculos(_pesquisaPedido.Veiculo);
            new BuscarMotoristas(_pesquisaPedido.Motorista);
            new BuscarTiposdeCarga(_pesquisaPedido.TipoCarga);
            new BuscarModelosVeicularesCarga(_pesquisaPedido.ModeloVeiculo);
            new BuscarFilial(_pesquisaPedido.Filial);
            new BuscarGruposPessoas(_pesquisaPedido.GrupoPessoas, null, null, null, EnumTipoGrupoPessoas.Clientes);
            new BuscarClientes(_pesquisaPedido.Remetente);
            new BuscarClientes(_pesquisaPedido.Destinatario);
            new BuscarClientes(_pesquisaPedido.Expedidor);
            new BuscarLocalidades(_pesquisaPedido.Origem);
            new BuscarLocalidades(_pesquisaPedido.Destino);
            new BuscarEstados(_pesquisaPedido.UFOrigem);
            new BuscarEstados(_pesquisaPedido.UFDestino);
            new BuscarTiposOperacao(_pesquisaPedido.TipoOperacao);
            new BuscarPedidos(_pesquisaPedido.Pedido);
            new BuscarRotasFrete(_pesquisaPedido.RotaFrete);
            new BuscarRestricaoEntrega(_pesquisaPedido.Restricao);
            new BuscarFuncionario(_pesquisaPedido.Gerente);
            new BuscarFuncionario(_pesquisaPedido.Supervisor);
            new BuscarFuncionario(_pesquisaPedido.Vendedor);
            new BuscarPedidoViagemNavio(_pesquisaPedido.NumeroViagemNavio);
            new BuscarPorto(_pesquisaPedido.PortoOrigem);
            new BuscarPorto(_pesquisaPedido.PortoDestino);
            new BuscarOperador(_pesquisaPedido.OperadorPedido);
            new BuscarCentroResultado(_pesquisaPedido.CentroResultado);
            new BuscarCentroCustoViagem(_pesquisaPedido.CentroDeCustoViagemCodigo);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador)
                _pesquisaPedido.ExibirCargasAgrupadas.visible(true);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
                _pesquisaPedido.Filial.visible(false);
                _pesquisaPedido.DataETAPortoOrigemInicial.visible(true);
                _pesquisaPedido.DataETAPortoOrigemFinal.visible(true);
                _pesquisaPedido.DataETSPortoOrigemInicial.visible(true);
                _pesquisaPedido.DataETSPortoOrigemFinal.visible(true);
                _pesquisaPedido.DataETAPortoDestinoInicial.visible(true);
                _pesquisaPedido.DataETAPortoDestinoFinal.visible(true);
                _pesquisaPedido.DataETSPortoDestinoInicial.visible(true);
                _pesquisaPedido.DataETSPortoDestinoFinal.visible(true);
                _pesquisaPedido.DataInclusaoPedidoInicial.visible(true);
                _pesquisaPedido.DataInclusaoPedidoFinal.visible(true);
                _pesquisaPedido.Transportador.text("Empresa/Filial:");
            }

            if (_CONFIGURACAO_TMS.AtivarNovosFiltrosConsultaCarga) {
                _pesquisaPedido.Situacoes.visible(false);
                _pesquisaPedido.Situacoes.val(new Array());
                _pesquisaPedido.SituacaoCargaMercante.visible(true);
            }

        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaPedido);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioPedido.gerarRelatorio("Relatorios/Pedido/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioPedido.gerarRelatorio("Relatorios/Pedido/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
