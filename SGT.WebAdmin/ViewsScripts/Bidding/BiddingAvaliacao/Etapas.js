/// <reference path="../../Enumeradores/EnumStatusBidding.js" />
/// <reference path="../../Enumeradores/EnumTipoPreenchimentoChecklist.js" />

var _etapaBidding;

var EtapaBidding = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("25%") });

    this.Etapa1 = PropertyEntity({
        text: "Convite", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("Nessa etapa são mostrados os status dos convites."),
        tooltipTitle: ko.observable("Convite")
    });
    this.Etapa2 = PropertyEntity({
        text: "Checklist", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("Nessa etapa é onde verifica-se as perguntas."),
        tooltipTitle: ko.observable("Checklist")
    });
    this.Etapa3 = PropertyEntity({
        text: "Ofertas", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable("As ofertas são mostradas nessa etapa."),
        tooltipTitle: ko.observable("Ofertas")
    });
    this.Etapa4 = PropertyEntity({
        text: "Fechamento", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(4),
        tooltip: ko.observable("Nessa etapa é mostrado o resultado do convite."),
        tooltipTitle: ko.observable("Resultado")
    });
}

function LoadEtapasBidding() {
    _etapaBidding = new EtapaBidding();
    KoBindings(_etapaBidding, "knockoutEtapasBidding");
    Etapa1Aguardando();
    //SetarEtapaInicioRequisicao();
}

function SetarEtapasRequisicao(status, tipoPreenchimentoChecklist) {
    _CRUDduvidas.FinalizarEtapa.visible(true);

    if (tipoPreenchimentoChecklist == EnumTipoPreenchimentoChecklist.PreenchimentoDesabilitado) {
        _etapaBidding.TamanhoEtapa.val("33%");
        _etapaBidding.Etapa2.visible(false);
    }

    if (status == EnumStatusBidding.Aguardando) {
        Etapa1Aguardando();
    }
    else if (status == EnumStatusBidding.Checklist) {
        Etapa2Liberada();
    }
    else if (status == EnumStatusBidding.Ofertas) {
        Etapa3Liberada();
    }
    else if (status == EnumStatusBidding.Fechamento) {
        _CRUDduvidas.FinalizarEtapa.visible(false);
        Etapa4Liberada();
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
    _CRUDduvidas.NotificarInteressados.visible(true);
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
    _CRUDduvidas.FecharBidding.visible(true);
    _CRUDduvidas.NotificarInteressados.visible(true);
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