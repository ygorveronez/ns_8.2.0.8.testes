/// <reference path="../../Enumeradores/EnumSituacaoLoteLiberacaoComercialPedido.js" />
/// <reference path="LoteLiberacaoComercialPedido.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _etapaEtapaLoteLiberacaoComercialPedido;

var EtapaLoteLiberacaoComercialPedido = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("50%") });

    this.Etapa1 = PropertyEntity({
        text: "Pedidos Bloqueados", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("É onde realiza a liberação comercial."),
        tooltipTitle: ko.observable("Pedidos Bloqueados")
    });
    this.Etapa2 = PropertyEntity({
        text: "Integração", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("Esta etapa é onde a integração dos dados são feitas."),
        tooltipTitle: ko.observable("Integração")
    });
};

//*******EVENTOS*******

function LoadEtapaLoteLiberacaoComercialPedido() {
    _etapaEtapaLoteLiberacaoComercialPedido = new EtapaLoteLiberacaoComercialPedido();
    KoBindings(_etapaEtapaLoteLiberacaoComercialPedido, "knockoutEtapaLoteLiberacaoComercialPedido");

    Etapa1Liberada();
}

function SetarEtapaInicioLoteLiberacaoComercialPedido() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaEtapaLoteLiberacaoComercialPedido.Etapa1.idTab).click();
    $("#" + _etapaEtapaLoteLiberacaoComercialPedido.Etapa1.idTab).tab("show")
}

function SetarEtapaLoteLiberacaoComercialPedido() {
    let situacao = _pesquisaCadastroLoteLiberacaoComercialPedido.Situacao.val();

    if (situacao === EnumSituacaoLoteLiberacaoComercialPedido.EmIntegracao)
        Etapa2Aguardando();
    else if (situacao === EnumSituacaoLoteLiberacaoComercialPedido.Finalizado)
        Etapa2Aprovada();
    else if (situacao === EnumSituacaoLoteLiberacaoComercialPedido.FalhaNaIntegracao)
        Etapa2Reprovada();
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaEtapaLoteLiberacaoComercialPedido.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaEtapaLoteLiberacaoComercialPedido.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaEtapaLoteLiberacaoComercialPedido.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaEtapaLoteLiberacaoComercialPedido.Etapa1.idTab + " .step").attr("class", "step green");
}

function Etapa1Desabilitada() {
    $("#" + _etapaEtapaLoteLiberacaoComercialPedido.Etapa1.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaEtapaLoteLiberacaoComercialPedido.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    _etapaEtapaLoteLiberacaoComercialPedido.Etapa2.eventClick = function () { };
    $("#" + _etapaEtapaLoteLiberacaoComercialPedido.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaEtapaLoteLiberacaoComercialPedido.Etapa2.idTab + " .step").attr("class", "step");
}

function Etapa2Liberada() {
    _etapaEtapaLoteLiberacaoComercialPedido.Etapa2.eventClick = EtapaLoteLiberacaoComercialPedidoClick;
    $("#" + _etapaEtapaLoteLiberacaoComercialPedido.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaEtapaLoteLiberacaoComercialPedido.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
}

function Etapa2Aguardando() {
    _etapaEtapaLoteLiberacaoComercialPedido.Etapa2.eventClick = EtapaLoteLiberacaoComercialPedidoClick;
    $("#" + _etapaEtapaLoteLiberacaoComercialPedido.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaEtapaLoteLiberacaoComercialPedido.Etapa2.idTab + " .step").attr("class", "step yellow");
    $("#" + _etapaEtapaLoteLiberacaoComercialPedido.Etapa2.idTab).click();
    $("#" + _etapaEtapaLoteLiberacaoComercialPedido.Etapa2.idTab).tab("show")
    Etapa1Aprovada();
}

function Etapa2Aprovada() {
    _etapaEtapaLoteLiberacaoComercialPedido.Etapa2.eventClick = EtapaLoteLiberacaoComercialPedidoClick;
    $("#" + _etapaEtapaLoteLiberacaoComercialPedido.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaEtapaLoteLiberacaoComercialPedido.Etapa2.idTab + " .step").attr("class", "step green");
    $("#" + _etapaEtapaLoteLiberacaoComercialPedido.Etapa2.idTab).click();
    $("#" + _etapaEtapaLoteLiberacaoComercialPedido.Etapa2.idTab).tab("show")
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    _etapaEtapaLoteLiberacaoComercialPedido.Etapa2.eventClick = EtapaLoteLiberacaoComercialPedidoClick;
    $("#" + _etapaEtapaLoteLiberacaoComercialPedido.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaEtapaLoteLiberacaoComercialPedido.Etapa2.idTab + " .step").attr("class", "step red");
    $("#" + _etapaEtapaLoteLiberacaoComercialPedido.Etapa2.idTab).click();
    $("#" + _etapaEtapaLoteLiberacaoComercialPedido.Etapa2.idTab).tab("show")
    Etapa1Aprovada();
}