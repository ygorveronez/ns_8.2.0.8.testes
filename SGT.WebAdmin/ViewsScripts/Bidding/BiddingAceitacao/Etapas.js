/// <reference path="../../Enumeradores/EnumStatusBidding.js" />
/// <reference path="../../Enumeradores/EnumTipoPreenchimentoChecklist.js" />

var _etapaBidding;

var EtapaBidding = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("25%") });

    this.Etapa1 = PropertyEntity({
        text: "Convite", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("Nessa etapa são mostradas as informações gerais do convite."),
        tooltipTitle: ko.observable("Convite")
    });
    this.Etapa2 = PropertyEntity({
        text: "Checklist", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("Nessa etapa é onde se responde as perguntas."),
        tooltipTitle: ko.observable("Checklist")
    });
    this.Etapa3 = PropertyEntity({
        text: "Ofertas", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable("As ofertas são mostradas nessa etapa."),
        tooltipTitle: ko.observable("Ofertas")
    });
    this.Etapa4 = PropertyEntity({
        text: "Resultado", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(4),
        tooltip: ko.observable("Nessa etapa é mostrado o resultado do convite."),
        tooltipTitle: ko.observable("Resultado")
    });
}

function LoadEtapasBidding() {
    _etapaBidding = new EtapaBidding();
    KoBindings(_etapaBidding, "knockoutEtapasBidding");
    Etapa1Aguardando();
}

function SetarEtapasRequisicao(status, removerEtapaAceite, tipoPreenchimentoChecklist) {
    _biddingAceitacaoChecklist.Descritivo.visible(false);

    if (tipoPreenchimentoChecklist == EnumTipoPreenchimentoChecklist.PreenchimentoDesabilitado) {
        _biddingAceitacaoChecklist.Descritivo.visible(true);
        _etapaBidding.TamanhoEtapa.val("33%");
        _etapaBidding.Etapa2.visible(false);
        _etapaBidding.Etapa1.step(1);
        _etapaBidding.Etapa3.step(2);
        _etapaBidding.Etapa4.step(3);
    }
    else {
        _etapaBidding.TamanhoEtapa.val("25%");
        _etapaBidding.Etapa2.visible(true);
        _etapaBidding.Etapa2.step(2);
        _etapaBidding.Etapa3.step(3);
        _etapaBidding.Etapa4.step(4);
    }

    switch (status) {
        case EnumStatusBidding.Aguardando:
            Etapa1Aguardando();
            break;

        case EnumStatusBidding.Checklist:
            Etapa2Liberada();
            break;

        case EnumStatusBidding.Ofertas:
            Etapa3Liberada();
            break;

        case EnumStatusBidding.Fechamento:
            Etapa4Liberada();
            break;

        default: break;
    }


}

function Etapa1Aguardando() {
    $("#" + _etapaBidding.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBidding.Etapa1.idTab + " .step").attr("class", "step yellow");
    $("#" + _etapaBidding.Etapa1.idTab).tab("show");
    Etapa2Desabilitada();
    Etapa3Desabilitada();
    Etapa4Desabilitada();
}

function Etapa1Aceita() {
    $("#" + _etapaBidding.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBidding.Etapa1.idTab + " .step").attr("class", "step green");
}

function Etapa2Liberada() {
    Etapa1Aceita();
    Etapa3Desabilitada();
    Etapa4Desabilitada();
    $("#" + _etapaBidding.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBidding.Etapa2.idTab + " .step").attr("class", "step yellow");
    $("#" + _etapaBidding.Etapa2.idTab).tab("show");
}

function Etapa2Aceita() {
    $("#" + _etapaBidding.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBidding.Etapa2.idTab + " .step").attr("class", "step green");
}

function Etapa3Liberada() {
    Etapa1Aceita();
    Etapa2Aceita();
    Etapa4Desabilitada();
    $("#" + _etapaBidding.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBidding.Etapa3.idTab + " .step").attr("class", "step yellow");
    $("#" + _etapaBidding.Etapa3.idTab).tab("show");
}

function Etapa3Aceita() {
    $("#" + _etapaBidding.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBidding.Etapa3.idTab + " .step").attr("class", "step green");
}

function Etapa4Liberada() {
    Etapa1Aceita();
    Etapa2Aceita();
    Etapa3Aceita();
    $("#" + _etapaBidding.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBidding.Etapa4.idTab + " .step").attr("class", "step green");
    $("#" + _etapaBidding.Etapa4.idTab).tab("show");
}

function Etapa2Desabilitada() {
    $("#" + _etapaBidding.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaBidding.Etapa2.idTab + " .step").attr("class", "step");
}

function Etapa3Desabilitada() {
    $("#" + _etapaBidding.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaBidding.Etapa3.idTab + " .step").attr("class", "step");
}

function Etapa4Desabilitada() {
    $("#" + _etapaBidding.Etapa4.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaBidding.Etapa4.idTab + " .step").attr("class", "step");
}