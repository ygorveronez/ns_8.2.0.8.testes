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
/// <reference path="Pedido.js" />
/// <reference path="../../Enumeradores/EnumSituacaoPedido.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _etapaPedido;

var EtapaPedido = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("33%") });

    this.Etapa1 = PropertyEntity({
        text: Localization.Resources.Pedidos.Pedido.DescricaoPedido, type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable(Localization.Resources.Pedidos.Pedido.OndeSeInformaOsDados),
        tooltipTitle: ko.observable(Localization.Resources.Pedidos.Pedido.DescricaoPedido)
    });
    this.Etapa2 = PropertyEntity({
        text: Localization.Resources.Gerais.Geral.Aprovacao, type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable(Localization.Resources.Pedidos.Pedido.CasoPedidoNecessiteDeAprovacao),
        tooltipTitle: ko.observable(Localization.Resources.Gerais.Geral.Aprovacao)
    });
    this.Etapa3 = PropertyEntity({
        text: Localization.Resources.Gerais.Geral.Integracao, type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable(Localization.Resources.Pedidos.Pedido.CasoPedidoNecessiteDeAprovacao),
        tooltipTitle: ko.observable(Localization.Resources.Gerais.Geral.Integracao)
    });
}


//*******EVENTOS*******

function loadEtapaPedido() {
    _etapaPedido = new EtapaPedido();
    KoBindings(_etapaPedido, "knockoutEtapaPedido");
    Etapa1Liberada();
}

function setarEtapaInicioPedido() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaPedido.Etapa1.idTab).click();
}

function setarEtapasPedido() {
    var situacaoPedido = _pedido.SituacaoPedido.val();

    if (situacaoPedido == EnumSituacaoPedido.AgAprovacao) {
        Etapa2Aguardando();
        Etapa3Aguardando();
    }
    else if (situacaoPedido == EnumSituacaoPedido.AutorizacaoPendente) {
        Etapa2Aguardando();
        Etapa3Aguardando();
    }
    else if (situacaoPedido == EnumSituacaoPedido.Rejeitado) {
        Etapa2Reprovada();
        Etapa3Reprovada();
    }
    else if (situacaoPedido == EnumSituacaoPedido.Aberto) {
        Etapa2Aprovada();
        Etapa3Aprovada();
    }
    else if (situacaoPedido == EnumSituacaoPedido.Aberto) {
        Etapa2Aprovada();
        Etapa3Aprovada();
    }
    else
        Etapa1Aguardando();
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
    Etapa3Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaPedido.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaPedido.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
    Etapa3Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaPedido.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaPedido.Etapa1.idTab + " .step").attr("class", "step green");
}

function Etapa1Aguardando() {
    $("#" + _etapaPedido.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaPedido.Etapa1.idTab + " .step").attr("class", "step yellow");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    $("#" + _etapaPedido.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaPedido.Etapa2.idTab + " .step").attr("class", "step");
}

function Etapa2Liberada() {
    $("#" + _etapaPedido.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaPedido.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
}

function Etapa2Aguardando() {
    $("#" + _etapaPedido.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaPedido.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
}

function Etapa2Aprovada() {
    $("#" + _etapaPedido.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaPedido.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    $("#" + _etapaPedido.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaPedido.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
}

//*******Etapa 3*******

function Etapa3Desabilitada() {
    $("#" + _etapaPedido.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaPedido.Etapa3.idTab + " .step").attr("class", "step");
}

function Etapa3Liberada() {
    $("#" + _etapaPedido.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaPedido.Etapa3.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
}

function Etapa3Aguardando() {
    $("#" + _etapaPedido.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaPedido.Etapa3.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();    
}

function Etapa3Aprovada() {
    $("#" + _etapaPedido.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaPedido.Etapa3.idTab + " .step").attr("class", "step green");
    Etapa1Aprovada();
}

function Etapa3Reprovada() {
    $("#" + _etapaPedido.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaPedido.Etapa3.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
}
