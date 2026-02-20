/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/Estado.js" />
/// <reference path="../../Consultas/Pais.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/CanalEntrega.js" />
/// <reference path="../../Consultas/TipoDetalhe.js" />
/// <reference path="Bloco.js" />
/// <reference path="Carga.js" />
/// <reference path="Carregamento.js" />
/// <reference path="CarregamentoCarga.js" />
/// <reference path="CarregamentoPedido.js" />
/// <reference path="Carregamentos.js" />
/// <reference path="CarregamentoTransporte.js" />
/// <reference path="DirecoesGoogleMaps.js" />
/// <reference path="Distancia.js" />
/// <reference path="GoogleMaps.js" />
/// <reference path="OrigemDestino.js" />
/// <reference path="Pedido.js" />
/// <reference path="PedidoProduto.js" />
/// <reference path="PedidosMapa.js" />
/// <reference path="Roteirizador.js" />
/// <reference path="SimulacaoFrete.js" />
/// <reference path="../../enumeradores/enumtipotipodetalhe.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumSimNaoPesquisa.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaMontegemCarga;
var _centralizarMapaNosPontos = null;
var _objPesquisaMontagem = {};

var PesquisaMontegemCarga = function () {
    var dataInicial = moment().add(-1, 'days').format("DD/MM/YYYY");
    var dataFinal = moment().add(1, 'days').format("DD/MM/YYYY");

    if (_CONFIGURACAO_TMS.MontagemCarga.FiltroPeriodoVazioAoIniciar) {
        dataInicial = '';
        dataFinal = '';
        var dataCriacaoPedidoInicial = moment().add(-1, 'days').format("DD/MM/YYYY");
        var dataCriacaoPedidoFinal = moment().add(1, 'days').format("DD/MM/YYYY");
    }

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataInicio = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DataInicio.getFieldDescription()), getType: typesKnockout.date, val: ko.observable(dataInicial) });
    this.DataFim = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DataFim.getFieldDescription()), getType: typesKnockout.date, val: ko.observable(dataFinal) });
    this.CodigoCargaEmbarcador = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.NumeroCarga.getFieldDescription()), val: ko.observable(""), def: "", visible: ko.observable(true), cssClass: ko.observable("col col-xs-6 col-sm-6 col-md-2 col-lg-2") });
    this.CodigoPedidoEmbarcador = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.CodigoPedidoEmbarcador.getFieldDescription()), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.CodigoPedidoEmbarcadorDe = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.CodigoPedidoEmbarcadorDe.getFieldDescription()), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.CodigoPedidoEmbarcadorAte = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.CodigoPedidoEmbarcadorAte.getFieldDescription()), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.NumeroPedido = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.NumeroPedido.getFieldDescription()), val: ko.observable(""), def: "", visible: ko.observable(true), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: '' }, cssClass: ko.observable("col col-xs-6 col-sm-6 col-md-2 col-lg-2") });
    this.NumeroBooking = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.NumeroBooking.getFieldDescription()), val: ko.observable(""), def: "", maxlength: 150, visible: ko.observable(_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) });
    this.NumeroOS = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.NumeroOS.getFieldDescription()), val: ko.observable(""), def: "", visible: ko.observable(_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) });
    this.PedidoViagemNavio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.PedidoViagemNavio.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Empresa.getFieldDescription()), issue: 69, idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Filial.getFieldDescription()), issue: 70, idBtnSearch: guid(), visible: ko.observable(true) });
    this.FilialVenda = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.FilialVenda.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Pedidos = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Pedidos.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Veiculo.getFieldDescription()), issue: 0, idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataCriacaoPedidoInicio = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DataCriacaoPedidoInicio.getFieldDescription()), getType: typesKnockout.dateTime, val: ko.observable(dataCriacaoPedidoInicial) });
    this.DataCriacaoPedidoLimite = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DataCriacaoPedidoLimite.getFieldDescription()), dateRangeInit: this.DataCriacaoPedidoInicio, getType: typesKnockout.dateTime, val: ko.observable(dataCriacaoPedidoFinal) });
    this.RegiaoDestino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.RegiaoDestino.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(true) });

    this.ComSessaoRoteirizador = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: false });
    this.OpcaoSessaoRoteirizador = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.bool, def: 0, visible: false });
    this.SessaoRoteirizador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.SessaoRoteirizador.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(false) });

    this.Expedidor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Expedidor.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Recebedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Recebedor.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(true) });
    this.NotaFiscal = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.NotaFiscal.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Remetente.getFieldDescription()), issue: 52, idBtnSearch: guid() });
    this.Rota = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Rota.getFieldDescription()), idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Destinatario.getFieldDescription()), issue: 52, idBtnSearch: guid() });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Origem.getFieldDescription()), issue: 16, idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Destino.getFieldDescription()), issue: 16, idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Cargas.MontagemCarga.TipoOperacao.getFieldDescription()), issue: 121, visible: ko.observable(true), idBtnSearch: guid(), cssClass: "col col-sm-3 col-md-3 col-lg-3" });
    this.EstadoOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.EstadoOrigem.getFieldDescription()), idBtnSearch: guid() });
    this.EstadosDestino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.EstadosDestino.getFieldDescription()), idBtnSearch: guid() });
    this.GrupoPessoaRemetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.GrupoPessoaRemetente.getFieldDescription()), idBtnSearch: guid() });
    this.GerarCargasDeRedespacho = PropertyEntity({ visible: ko.observable(true), val: ko.observable(false), getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Cargas.MontagemCarga.GerarCargasDeRedespacho), def: false });
    this.GerarCargasDeColeta = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Cargas.MontagemCarga.GerarCargasDeColeta.getFieldDescription()), def: false, visible: ko.observable(_CONFIGURACAO_TMS.ExibirPedidoDeColeta) });
    this.Ordem = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Ordem.getFieldDescription()), maxlength: 50 });
    this.PortoSaida = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.PortoSaida.getFieldDescription()), maxlength: 150 });
    this.TipoEmbarque = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.TipoEmbarque.getFieldDescription()), maxlength: 150 });
    this.PaisDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.PaisDestino.getFieldDescription()), idBtnSearch: guid() });
    this.Reserva = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Reserva.getFieldDescription()), maxlength: 150 });
    this.DeliveryTerm = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DeliveryTerm.getFieldDescription()), maxlength: 150 });
    this.IdAutorizacao = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.IdAutorizacao.getFieldDescription()), maxlength: 150 });
    this.SomenteComReserva = PropertyEntity({ type: types.bool, val: ko.observable(false), def: false, text: ko.observable(Localization.Resources.Cargas.MontagemCarga.SomenteComReserva) });
    this.ExigeAgendamento = PropertyEntity({ visible: ko.observable(true), val: ko.observable(false), getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Cargas.MontagemCarga.ExigeAgendamento), def: false });
    this.DataInclusaoBookingInicial = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DataInclusaoBookingInicial.getFieldDescription()), getType: typesKnockout.date });
    this.DataInclusaoBookingLimite = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DataInclusaoBookingLimite.getFieldDescription()), dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataInclusaoPCPInicial = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DataInclusaoPCPInicial.getFieldDescription()), getType: typesKnockout.date });
    this.DataInclusaoPCPLimite = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DataInclusaoPCPLimite.getFieldDescription()), dateRangeInit: this.DataInicio, getType: typesKnockout.date });

    if (!_CONFIGURACAO_TMS.MontagemCarga.FiltroPeriodoVazioAoIniciar) {
        this.DataInicio.dateRangeLimit = this.DataFim;
        this.DataFim.dateRangeInit = this.DataInicio;
    }

    this.DataInclusaoBookingInicial.dateRangeLimit = this.DataInclusaoBookingLimite;
    this.DataInclusaoBookingLimite.dateRangeInit = this.DataInclusaoBookingInicial;
    this.DataInclusaoPCPInicial.dateRangeLimit = this.DataInclusaoPCPLimite;
    this.DataInclusaoPCPLimite.dateRangeInit = this.DataInclusaoPCPInicial;
    this.DataCriacaoPedidoInicio.dateRangeLimit = this.DataCriacaoPedidoLimite;
    this.DataCriacaoPedidoLimite.dateRangeInit = this.DataCriacaoPedidoInicio;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            BuscarDadosMontagemCarga();
        }, type: types.event, text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Pesquisar), idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: ko.observable(Localization.Resources.Cargas.MontagemCarga.FiltroDePesquisa), idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade() == true) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Avancada, idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    //#4429
    this.PesoDe = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.PesoDe.getFieldDescription()), val: ko.observable(""), def: "0,00", visible: ko.observable(true), getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.PesoAte = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.PesoAte.getFieldDescription()), val: ko.observable(""), def: "0,00", visible: ko.observable(true), getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.PalletDe = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.PalletDe.getFieldDescription()), val: ko.observable(""), def: "0,00", visible: ko.observable(true), getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.PalletAte = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.PalletAte.getFieldDescription()), val: ko.observable(""), def: "0,00", visible: ko.observable(true), getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.VolumeDe = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.VolumeDe.getFieldDescription()), val: ko.observable(""), def: "0,00", visible: ko.observable(true), getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.VolumeAte = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.VolumeAte.getFieldDescription()), val: ko.observable(""), def: "0,00", visible: ko.observable(true), getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    //#34971
    this.ValorDe = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.ValorDe.getFieldDescription()), val: ko.observable(""), def: "0,00", visible: ko.observable(true), getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.ValorAte = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.ValorAte.getFieldDescription()), val: ko.observable(""), def: "0,00", visible: ko.observable(true), getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });

    this.TipoDeCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.TipoDeCarga.getFieldDescription()), idBtnSearch: guid() });
    this.NaoRecebeCargaCompartilhada = PropertyEntity({ visible: ko.observable(true), val: ko.observable(false), getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Cargas.MontagemCarga.NaoRecebeCargaCompartilhada), def: false });
    this.SomentePedidosComNota = PropertyEntity({ visible: ko.observable(true), val: ko.observable(false), getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Cargas.MontagemCarga.SomentePedidosComNota), def: false  });
    this.SomentePedidosSemNota = PropertyEntity({ visible: ko.observable(true), val: ko.observable(false), getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Cargas.MontagemCarga.SomentePedidosSemNota), def: false });

    this.CodigoProcessamentoEspecial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.CodigoProcessamentoEspecial.getFieldDescription()), idBtnSearch: guid() });
    this.CanalEntrega = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.CanalEntrega.getFieldDescription()), type: types.multiplesEntities, idBtnSearch: guid() });
    this.CodigoHorarioEntrega = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.CodigoHorarioEntrega.getFieldDescription()), idBtnSearch: guid() });
    this.RestricaoDiasEntrega = PropertyEntity({ val: ko.observable(new Array()), getType: typesKnockout.selectMultiple, options: EnumDiaSemana.obterOpcoes(), def: new Array(), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.RestricaoDiasEntrega.getFieldDescription()) });

    this.PedidosBloqueados = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.PedidoBloqueado.getFieldDescription(), options: EnumSimNaoPesquisa.obterOpcoesPesquisa2(), val: ko.observable(EnumSimNaoPesquisa.Todos2), def: EnumSimNaoPesquisa.Todos2, visible: ko.observable(true) });
    this.PedidosRestricaoData = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.PedidoRestricaoData.getFieldDescription(), options: EnumSimNaoPesquisa.obterOpcoesPesquisa2(), val: ko.observable(EnumSimNaoPesquisa.Todos2), def: EnumSimNaoPesquisa.Todos2, visible: ko.observable(true) });
    this.PedidosRestricaoPercentual = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.PedidoRestricaoPercentual.getFieldDescription(), options: EnumSimNaoPesquisa.obterOpcoesPesquisa2(), val: ko.observable(EnumSimNaoPesquisa.Todos2), def: EnumSimNaoPesquisa.Todos2, visible: ko.observable(true) });

    this.DataAgendamentoInicial = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.DataAgendamentoInicial.getFieldDescription(), getType: typesKnockout.date });
    this.DataAgendamentoFinal = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.DataAgendamentoFinal.getFieldDescription(), dateRangeInit: this.DataInicio, getType: typesKnockout.date });

    //this.UsarTipoTomadorPedido = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Cargas.MontagemCarga.UsarTipoTomador, def: false, enable: ko.observable(true) });
    this.TiposTomador = PropertyEntity({ val: ko.observable(new Array()), getType: typesKnockout.selectMultiple, options: EnumTipoTomador.obterOpcoes(), def: new Array(), text: Localization.Resources.Cargas.MontagemCarga.TipoTomador.getFieldDescription(), required: false });
    this.Ordenacao = PropertyEntity({ val: ko.observable(EnumOrdenacaoFiltroPesquisaPedido.Padrao), options: EnumOrdenacaoFiltroPesquisaPedido.obterOpcoes(), def: EnumOrdenacaoFiltroPesquisaPedido.Padrao, text: Localization.Resources.Cargas.MontagemCarga.Ordenacao.getFieldDescription(), required: false });

    this.PedidosParaReentrega = PropertyEntity({ visible: ko.observable(true), val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pedidos.Pedido.PedidosParaReentrega, def: false });

    this.DataAgendamentoInicial.dateRangeLimit = this.DataAgendamentoFinal;
    this.DataAgendamentoFinal.dateRangeInit = this.DataAgendamentoInicial;

    this.SomentePedidosSemNota.val.subscribe(function (valor) {
        _pesquisaMontegemCarga.SomentePedidosComNota.val(!valor);
    });

    this.SomentePedidosComNota.val.subscribe(function (valor) {
        _pesquisaMontegemCarga.SomentePedidosSemNota.val(!valor);
    });

    this.Regiao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Regiao.getFieldDescription()), idBtnSearch: guid() });
    this.Mesorregiao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Mesorregiao.getFieldDescription()), idBtnSearch: guid() });
    this.TendenciaEntrega = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.TendenciaEntrega.getFieldDescription(), val: ko.observable(EnumFiltroTendenciaPrazoEntrega.Nenhum), options: EnumFiltroTendenciaPrazoEntrega.obterOpcoes(), def: EnumFiltroTendenciaPrazoEntrega.Nenhum });
}

//*******EVENTOS*******
function DataInicioModificada(val) {
    var dataInicio = moment(val, "DD/MM/YYYY");
    var dataFim = moment(_pesquisaMontegemCarga.DataFim.val(), "DD/MM/YYYY");
    var diferenca = dataFim.diff(dataInicio, 'days', true);

    if (diferenca > 7) {
        dataFim = dataInicio.add(7, 'days').format("DD/MM/YYYY");
        _pesquisaMontegemCarga.DataFim.val(dataFim);
        _pesquisaMontegemCarga.DataFim.get$().change()
    }
}

function DataFimModificada(val) {
    var dataInicio = moment(_pesquisaMontegemCarga.DataInicio.val(), "DD/MM/YYYY");
    var dataFim = moment(val, "DD/MM/YYYY");
    var diferenca = dataFim.diff(dataInicio, 'days', true);

    if (diferenca > 7) {
        dataInicio = dataFim.add(-7, 'days').format("DD/MM/YYYY");
        _pesquisaMontegemCarga.DataInicio.val(dataInicio);
        _pesquisaMontegemCarga.DataInicio.get$().change()
    }
}

function ObterDetalhesCargaMontagem(codigo) {
    var data = { Codigo: codigo };
    executarReST("MontagemCargaCarga/BuscarPorCodigoCarga", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                $("#fdsCarga").html('<button type="button" class="btn-close" data-bs-dismiss="modal" aria-hidden="true" style="position: absolute; z-index: 9999; right: 18px; top: 6px;"><i class="fal fa-times"></i></button>');
                var knoutCarga = GerarTagHTMLDaCarga("fdsCarga", arg.Data, false);
                $("#fdsCarga .container-carga").addClass("mb-0");
                _cargaAtual = knoutCarga;
                Global.abrirModal("divModalDetalhesCarga");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function loadMontagemCarga() {
    buscarDetalhesOperador(function () {
        carregarConteudosHTML(function () {
            _pesquisaMontegemCarga = new PesquisaMontegemCarga();
            KoBindings(_pesquisaMontegemCarga, "knoutPesquisaMontagemCarga");

            _objPesquisaMontagem = RetornarObjetoPesquisa(_pesquisaMontegemCarga);

            new BuscarClientes(_pesquisaMontegemCarga.Remetente);
            new BuscarCanaisEntrega(_pesquisaMontegemCarga.CanalEntrega);
            new BuscarLocalidades(_pesquisaMontegemCarga.Origem);
            new BuscarClientes(_pesquisaMontegemCarga.Destinatario);
            new BuscarLocalidades(_pesquisaMontegemCarga.Destino);
            new BuscarFilial(_pesquisaMontegemCarga.Filial);
            new BuscarFilial(_pesquisaMontegemCarga.FilialVenda);
            new BuscarPedidosDisponiveis(_pesquisaMontegemCarga.Pedidos);
            new BuscarRegioes(_pesquisaMontegemCarga.RegiaoDestino);
            new BuscarTransportadores(_pesquisaMontegemCarga.Empresa);
            new BuscarTiposOperacao(_pesquisaMontegemCarga.TipoOperacao);
            new BuscarGruposPessoas(_pesquisaMontegemCarga.GrupoPessoaRemetente);
            new BuscarEstados(_pesquisaMontegemCarga.EstadosDestino);
            new BuscarEstados(_pesquisaMontegemCarga.EstadoOrigem);
            new BuscarClientes(_pesquisaMontegemCarga.Recebedor);
            new BuscarXMLNotaFiscal(_pesquisaMontegemCarga.NotaFiscal);
            new BuscarClientes(_pesquisaMontegemCarga.Expedidor);
            new BuscarPedidoViagemNavio(_pesquisaMontegemCarga.PedidoViagemNavio);
            new BuscarPaises(_pesquisaMontegemCarga.PaisDestino);
            new BuscarVeiculos(_pesquisaMontegemCarga.Veiculo);
            new BuscarRotasFrete(_pesquisaMontegemCarga.Rota);
            new BuscarRegioes(_pesquisaMontegemCarga.Regiao);
            new BuscarMesoRegiao(_pesquisaMontegemCarga.Mesorregiao);

            new BuscarTiposDetalhe(_pesquisaMontegemCarga.CodigoProcessamentoEspecial, null, null, EnumTipoTipoDetalhe.ProcessamentoEspecial);
            new BuscarTiposDetalhe(_pesquisaMontegemCarga.CodigoHorarioEntrega, null, null, EnumTipoTipoDetalhe.HorarioEntrega);

            //Tarefa MARFRIG, #9391 vamos filtrar somente os tipos de carga da filial.
            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
                new BuscarTiposdeCargaPorFilial(_pesquisaMontegemCarga.TipoDeCarga, null, _pesquisaMontegemCarga.Filial);
            } else {
                //#4429
                new BuscarTiposdeCarga(_pesquisaMontegemCarga.TipoDeCarga);
            }

            loadCarregamento();
            loadPedidoProduto();
            loadDetalhesPedido();
            loadDetalhesCarga(buscarCargasMontagem);
            loadDetalhePedidoNotaFiscal();
            loadDetalhesCarregamento();
            loadRoteirizadorCarregamento();
            loadSimulacao();
            loadImportacao();
            loadCapacidadeJanelaCarregamento();
            loadPeriodoCarregamento();
            loadAreaNotaFiscal();

            if (_CONFIGURACAO_TMS.PermiteAdicionarNotaManualmente) {
                var $tabMapa = $("#liMapa");

                $tabMapa.show();
                $tabMapa.on('shown.bs.tab', 'a', function () {
                    if (_centralizarMapaNosPontos != null) {
                        _map.fitBounds(_centralizarMapaNosPontos);
                        _map.panToBounds(_centralizarMapaNosPontos);
                        _centralizarMapaNosPontos = null;
                    }
                });
                $(window).one('hashchange', function (e) {
                    $tabMapa.off('shown.bs.tab', 'a');
                });
            }

            if (_CONFIGURACAO_TMS.TipoMontagemCargaPadrao !== EnumTipoMontagemCarga.Todos) {
                _carregamento.TipoMontagemCarga.val(_CONFIGURACAO_TMS.TipoMontagemCargaPadrao);
                _carregamento.TipoMontagemCarga.def = _CONFIGURACAO_TMS.TipoMontagemCargaPadrao;

                _carregamento.TipoMontagemCarga.visible(false);
                buscarInformacoesTipoMontagem();
            }

            VisibilidadeCampos();
            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
                BuscarDadosMontagemCarga();
            else
                CarregarDadosMontagemCarga();
        });
    });
}

function VisibilidadeCampos() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        _pesquisaMontegemCarga.Filial.visible(false);
        _pesquisaMontegemCarga.FilialVenda.visible(false);
        _pesquisaMontegemCarga.Empresa.text("Empresa/Filial:");
        _pesquisaMontegemCarga.CodigoCargaEmbarcador.cssClass("col col-xs-6 col-sm-6 col-md-3 col-lg-3");
        _pesquisaMontegemCarga.NumeroPedido.cssClass("col col-xs-6 col-sm-6 col-md-3 col-lg-3");
    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _pesquisaMontegemCarga.NumeroPedido.visible(false);

        _carregamento.DataPrevisaoSaida.visible(_CONFIGURACAO_TMS.PadraoVisualizacaoOperadorLogistico);
        _carregamento.DataPrevisaoRetorno.visible(_CONFIGURACAO_TMS.PadraoVisualizacaoOperadorLogistico);
    }

    _pesquisaMontegemCarga.GerarCargasDeRedespacho.visible(!_CONFIGURACAO_TMS.NaoGerarCarregamentoRedespacho);
}

function BuscarDadosMontagemCarga() {
    _objPesquisaMontagem = RetornarObjetoPesquisa(_pesquisaMontegemCarga);
    _gerarCargasDeColeta = _pesquisaMontegemCarga.GerarCargasDeColeta.val();
    BuscarPedidos(function () {
        ObterPontosPedidos();
        PesquisarPedidos();
        PesquisarCargas();
        PesquisarCarregamentos();
        PesquisarNotasFiscais();
        setConfiguracaoColeta();
    });
}

function CarregarDadosMontagemCarga() {
    _objPesquisaMontagem = RetornarObjetoPesquisa(_pesquisaMontegemCarga);
    _gerarCargasDeColeta = _pesquisaMontegemCarga.GerarCargasDeColeta.val();
    CarregarListaPedidos(function () {
        ObterPontosPedidos();
        PesquisarPedidos();
        PesquisarCargas();
        PesquisarCarregamentos();
        setConfiguracaoColeta();
    });
}
