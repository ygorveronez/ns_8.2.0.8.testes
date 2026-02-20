/// <reference path="../../Enumeradores/EnumIncotermPedido.js" />
/// <reference path="../../Enumeradores/EnumTransitoAduaneiro.js" />
/// <reference path="Pedido.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _crtMicDta;

/*
 * Declaração das Classes
 */

var CrtMicDta = function () {
    this.Incoterm = PropertyEntity({ val: ko.observable(""), options: EnumIncotermPedido.obterOpcoes(), def: "", text: "Incoterm:", visible: ko.observable(true), required: false });
    this.TransitoAduaneiro = PropertyEntity({ val: ko.observable(EnumTransitoAduaneiro.Sim), options: EnumTransitoAduaneiro.obterOpcoes(), def: EnumTransitoAduaneiro.Sim, text: Localization.Resources.Pedidos.Pedido.TransitoAduaneiro.getFieldDescription(), visible: ko.observable(true), required: false });
    this.NotificacaoCRT = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Pedidos.Pedido.NotificacaoCRT.getFieldDescription(), idBtnSearch: guid(), issue: 52 });
    this.DtaRotaPrazoTransporte = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.DtaRotaPrazoTransporte.getFieldDescription(), maxlength: 1000, enable: ko.observable(true), val: ko.observable(""), getType: typesKnockout.string, visible: ko.observable(true) });
    this.DeclaracaoObservacaoCRT = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.DeclaracaoObservacaoCRT.getFieldDescription(), maxlength: 2000, required: false, visible: ko.observable(true) });
    this.TipoEmbalagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.TipoEmbalagem.getFieldDescription(), idBtnSearch: guid(), required: false, visible: ko.observable(true) });
    this.DetalheMercadoria = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.DetalheMercadoria.getFieldDescription(), maxlength: 500, enable: ko.observable(true), val: ko.observable(""), getType: typesKnockout.string, visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização  
 */

function loadCrtMicDta() {
    _crtMicDta = new CrtMicDta();
    KoBindings(_crtMicDta, "knockoutCrtMicDta");

    BuscarClientes(_crtMicDta.NotificacaoCRT);
    BuscarTipoEmbalagem(_crtMicDta.TipoEmbalagem);
}

/*
 * Declaração das Funções Públicas
 */

function preencherDadosCrtMicDta(crtMicDta) {
    PreencherObjetoKnout(_crtMicDta, { Data: crtMicDta });
}

function preencherDadosCrtMicDtaSalvar(pedido) {
    let crtMicDta = RetornarObjetoPesquisa(_crtMicDta);
    $.extend(pedido, crtMicDta);
}

function limparCamposCrtMicDta() {
    LimparCampos(_crtMicDta);
}



