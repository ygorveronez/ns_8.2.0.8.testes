/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumTipoClienteCotacaoPedido.js" />
/// <reference path="../../Enumeradores/EnumStatusCotacaoPedido.js" />
/// <reference path="../../Enumeradores/EnumTipoModal.js" />
/// <reference path="../../Enumeradores/EnumTipoPagamento.js" />
/// <reference path="CotacaoPedido.js" />

var _cotacaoPedidoAbaResumoPedidos;
var _cotacaoPedidoAbaHistoricoPedidos;
var _pedidoFracionadoCRUD;
var _gridDetalhesPedidosFracionados;
var _gridHistoricoPedidosFracionados;

var CotacaoPedidoAbaResumoPedidos = function () {
    this.CodigoCotacaoPedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataColetaPedidoFracionado = PropertyEntity({ text: ko.observable("Data Coleta:"), getType: typesKnockout.date, required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.PesoLiquidoPedidoFracionado = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 15, text: ko.observable("Peso Bruto:"), required: false, visible: ko.observable(true), enable: ko.observable(false) });
    this.NumeroPaletesPedidoFracionado = PropertyEntity({ getType: typesKnockout.int, maxlength: 5, text: ko.observable("Nº Pallets:"), configInt: { precision: 0, allowZero: false, thousands: "" }, required: false, visible: ko.observable(true), enable: ko.observable(false) });
    this.CubagemPedidoFracionado = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { precision: 3, allowZero: false, allowNegative: false }, maxlength: 15, text: "Cubagem do Pedido (M³):", required: false, visible: ko.observable(true), enable: ko.observable(false) });
    this.UnidadesPedidoFracionado = PropertyEntity({ getType: typesKnockout.int, configInt: { precision: 0, allowZero: false }, text: ko.observable("Unidades:"), required: false, visible: ko.observable(true), enable: ko.observable(false) });

    this.CriarPedidoFracionado = PropertyEntity({ type: types.event, eventClick: adicionarPedidoFracionadoClick, text: ko.observable("Adicionar"), visible: ko.observable(true), enable: ko.observable(true) });
    this.CancelarPedidoFracionado = PropertyEntity({ eventClick: cancelarPedidoFracionadoClick, type: types.event, text: "Cancelar / Limpar", visible: ko.observable(true) });

    this.DetalhesPedidoFracionado = PropertyEntity({ idGrid: guid(), visible: ko.observable(true), id: guid() });
};

var CotacaoPedidoAbaHistoricoPedidos = function () {
    this.HistoricoPedidoFracionado = PropertyEntity({ idGrid: guid(), visible: ko.observable(true), id: guid() });
};

function loadCotacaoPedidoPedidos() {
    _cotacaoPedidoAbaResumoPedidos = new CotacaoPedidoAbaResumoPedidos();
    KoBindings(_cotacaoPedidoAbaResumoPedidos, "tabResumo");

    _cotacaoPedidoAbaHistoricoPedidos = new CotacaoPedidoAbaHistoricoPedidos();
    KoBindings(_cotacaoPedidoAbaHistoricoPedidos, "tabHistoricoPedidos");
}

function controlaCamposPedidoFracionado() {
    if (parseFloat(_cotacaoPedidoAdicional.PesoTotal.val()) > 0) {
        _cotacaoPedidoAbaResumoPedidos.PesoLiquidoPedidoFracionado.enable(true);
        _cotacaoPedidoAbaResumoPedidos.NumeroPaletesPedidoFracionado.enable(false);
        _cotacaoPedidoAbaResumoPedidos.CubagemPedidoFracionado.enable(false);
        _cotacaoPedidoAbaResumoPedidos.UnidadesPedidoFracionado.enable(false);
    }

    if (parseInt(_cotacaoPedidoAdicional.NumeroPaletes.val()) > 0) {
        _cotacaoPedidoAbaResumoPedidos.NumeroPaletesPedidoFracionado.enable(true);
        _cotacaoPedidoAbaResumoPedidos.PesoLiquidoPedidoFracionado.enable(false);
        _cotacaoPedidoAbaResumoPedidos.CubagemPedidoFracionado.enable(false);
        _cotacaoPedidoAbaResumoPedidos.UnidadesPedidoFracionado.enable(false);
    }

    if (parseFloat(_cotacaoPedidoAdicional.TotalPesoCubado.val()) > 0) {
        _cotacaoPedidoAbaResumoPedidos.CubagemPedidoFracionado.enable(true);
        _cotacaoPedidoAbaResumoPedidos.PesoLiquidoPedidoFracionado.enable(false);
        _cotacaoPedidoAbaResumoPedidos.NumeroPaletesPedidoFracionado.enable(false);
        _cotacaoPedidoAbaResumoPedidos.UnidadesPedidoFracionado.enable(false);
    }

    if (parseInt(_cotacaoPedidoAdicional.QtdEntregas.val()) > 0) {
        _cotacaoPedidoAbaResumoPedidos.UnidadesPedidoFracionado.enable(true);
        _cotacaoPedidoAbaResumoPedidos.PesoLiquidoPedidoFracionado.enable(false);
        _cotacaoPedidoAbaResumoPedidos.NumeroPaletesPedidoFracionado.enable(false);
        _cotacaoPedidoAbaResumoPedidos.CubagemPedidoFracionado.enable(false);
    }
}

function cancelarPedidoFracionadoClick(e) {
    LimparCampos(_cotacaoPedidoAbaResumoPedidos);
}

function adicionarPedidoFracionadoClick(e, sender) {
    e.CodigoCotacaoPedido = _cotacaoPedido.Codigo;

    Salvar(e, "CotacaoPedido/CriarPedidoFracionado", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridCotacaoPedido.CarregarGrid();
                limparCamposCotacaoPedido();
                LimparCampos(e);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function ExibirDetalhesPedidosFracionados(registroSelecionado) {
    _gridDetalhesPedidosFracionados = new GridViewExportacao(_cotacaoPedidoAbaResumoPedidos.DetalhesPedidoFracionado.idGrid, "CotacaoPedido/BuscarDetalhesPedidosFracionados", _cotacaoPedido);
    _gridDetalhesPedidosFracionados.CarregarGrid();

    _gridHistoricoPedidosFracionados = new GridViewExportacao(_cotacaoPedidoAbaHistoricoPedidos.HistoricoPedidoFracionado.idGrid, "CotacaoPedido/BuscarHistoricoPedidosFracionados", _cotacaoPedido);
    _gridHistoricoPedidosFracionados.CarregarGrid();
}