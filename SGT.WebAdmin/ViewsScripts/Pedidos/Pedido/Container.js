/// <reference path="Pedido.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _container;

/*
 * Declaração das Classes
 */

var Container = function () {
    this.ContainerTipoReservaFluxoContainer = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Pedidos.Pedido.TipoContainerReserva.getFieldDescription(), visible: ko.observable(true), idBtnSearch: guid() });
    this.NumeroContainer = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.NumeroContainer.getFieldDescription(), required: false, visible: ko.observable(true), maxlength: 1000 });
    this.LacreContainerUm = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.LacreContainerum.getFieldDescription(), required: false, visible: ko.observable(true), maxlength: 100 });
    this.LacreContainerDois = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.LacreContainerdois.getFieldDescription(), required: false, visible: ko.observable(true), maxlength: 100 });
    this.LacreContainerTres = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.LacreContainertres.getFieldDescription(), required: false, visible: ko.observable(true), maxlength: 100 });
    this.TaraContainer = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.TaraContainer.getFieldDescription(), required: false, visible: ko.observable(true), maxlength: 100 });
    this.DataChip = PropertyEntity({ text: ko.observable(Localization.Resources.Pedidos.Pedido.ChipDate.getFieldDescription()), getType: typesKnockout.date, required: false, issue: 2, val: ko.observable("") });
    this.DataCancel = PropertyEntity({ text: ko.observable(Localization.Resources.Pedidos.Pedido.CancelDate.getFieldDescription()), getType: typesKnockout.date, required: false, issue: 2, val: ko.observable("") });
    this.NumeroBooking = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.NumeroBooking.getFieldDescription(), required: false, visible: ko.observable(false), maxlength: 100 });
    this.NumeroOS = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.OrdemServico.getFieldDescription(), required: false, visible: ko.observable(false), maxlength: 100 });

}

/*
 * Declaração das Funções de Inicialização
 */

function loadContainer() {
    _container = new Container();
    KoBindings(_container, "knockoutContainer");

    configurarLayoutPorTipoSistemaContainer();
    new BuscarTiposContainer(_container.ContainerTipoReservaFluxoContainer);

}


function configurarLayoutPorTipoSistemaContainer() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {

        _container.NumeroBooking.visible(true);
        _container.NumeroOS.visible(true);
       
    }

    if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) {
        //_container.TagNumeroBooking.visible(true);
        //_container.TagNumeroContainer.visible(true);
        //_container.TagNumeroOS.visible(true);

        _container.NumeroOS.visible(true);
        _container.NumeroBooking.visible(true);
        _container.NumeroContainer.visible(false);
        _container.LacreContainerUm.visible(false);
        _container.LacreContainerDois.visible(false);
        _container.LacreContainerTres.visible(false);
        _container.TaraContainer.visible(false);

    }
}
function preencherContainerSalvar(pedido) {
    let container = RetornarObjetoPesquisa(_container);
    $.extend(pedido, container);
}
function limparCamposContainer() {
    LimparCampos(_container);

}

function preencherContainer(container) {

    PreencherObjetoKnout(_container, { Data: container });

}