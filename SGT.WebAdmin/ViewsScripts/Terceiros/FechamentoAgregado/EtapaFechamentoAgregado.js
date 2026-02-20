/// <reference path="Etapa1SelecaoCIOT.js" />
/// <reference path="Etapa2Consolidacao.js" />
/// <reference path="Etapa3Integracao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _etapaFechamentoAgregado;

var EtapaFechamentoAgregado = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("50%") });

    this.Etapa1 = PropertyEntity({
        text: "CIOT", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("Esta etapa é destinada à seleção do CIOT."),
        tooltipTitle: ko.observable("CIOT")
    });

    this.Etapa2 = PropertyEntity({
        text: "Consolidação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("Esta etapa é destinada a consolidação do dados."),
        tooltipTitle: ko.observable("Consolidacao")
    });

    this.Etapa3 = PropertyEntity({
        text: "Integração", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("Esta etapa é destinada a integrações."),
        tooltipTitle: ko.observable("Integração")
    });
};

//*******EVENTOS*******

function LoadEtapasFechamentoAgregado() {
    _etapaFechamentoAgregado = new EtapaFechamentoAgregado();
    KoBindings(_etapaFechamentoAgregado, "knockoutEtapaFechamentoAgregado");

    Etapa1FechamentoAgregadoLiberada();
}

function SetarEtapasFechamentoAgregado() {
    if (_fechamentoAgregado.Codigo.val() == 0) {
        Etapa1FechamentoAgregadoLiberada();
        Etapa2FechamentoAgregadoDesabilitada();
        Etapa3FechamentoAgregadoDesabilitada();

        $("#" + _etapaFechamentoAgregado.Etapa1.idTab).click();
        $("#" + _etapaFechamentoAgregado.Etapa1.idTab).tab("show");
    }
    else if (_fechamentoAgregado.Codigo.val() > 0 && !_etapa2Consolidacao.Consolidado.val()) {
        Etapa1FechamentoAgregadoAprovada();
        Etapa2FechamentoAgregadoAguardando();
        Etapa3FechamentoAgregadoDesabilitada();

        $("#" + _etapaFechamentoAgregado.Etapa2.idTab).click();
        $("#" + _etapaFechamentoAgregado.Etapa2.idTab).tab("show");
    }
    else if (_fechamentoAgregado.Codigo.val() > 0 && _etapa2Consolidacao.Consolidado.val() && _etapa1SelecaoCIOT.SituacaoCIOT.val() == EnumSituacaoCIOT.Encerrado) {
        Etapa1FechamentoAgregadoAprovada();
        Etapa2FechamentoAgregadoAprovada();
        Etapa3FechamentoAgregadoDesabilitada();

        $("#" + _etapaFechamentoAgregado.Etapa2.idTab).click();
        $("#" + _etapaFechamentoAgregado.Etapa2.idTab).tab("show");
    }
    else if (_fechamentoAgregado.Codigo.val() > 0 && _etapa2Consolidacao.Consolidado.val()) {
        Etapa1FechamentoAgregadoAprovada();
        Etapa2FechamentoAgregadoReprovada();
        Etapa3FechamentoAgregadoDesabilitada();

        $("#" + _etapaFechamentoAgregado.Etapa2.idTab).click();
        $("#" + _etapaFechamentoAgregado.Etapa2.idTab).tab("show");
    }
}

function DesabilitarTodasEtapasFechamentoAgregado() {
    Etapa2FechamentoAgregadoDesabilitada();
    Etapa3FechamentoAgregadoDesabilitada();
}

//*******Etapa 1*******

function Etapa1FechamentoAgregadoLiberada() {
    $("#" + _etapaFechamentoAgregado.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamentoAgregado.Etapa1.idTab + " .step").attr("class", "step yellow");

    _etapaFechamentoAgregado.Etapa1.eventClick = function () { };
}

function Etapa1FechamentoAgregadoAprovada() {
    $("#" + _etapaFechamentoAgregado.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamentoAgregado.Etapa1.idTab + " .step").attr("class", "step green");

    _etapaFechamentoAgregado.Etapa1.eventClick = function () { };
}

//*******Etapa 2*******

function Etapa2FechamentoAgregadoDesabilitada() {
    $("#" + _etapaFechamentoAgregado.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaFechamentoAgregado.Etapa2.idTab + " .step").attr("class", "step");

    _etapaFechamentoAgregado.Etapa2.eventClick = function () { };
}

function Etapa2FechamentoAgregadoAguardando() {
    $("#" + _etapaFechamentoAgregado.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamentoAgregado.Etapa2.idTab + " .step").attr("class", "step yellow");

    _etapaFechamentoAgregado.Etapa2.eventClick = function () { }; //BuscarFechamentoAgregadoPorCodigo(_fechamentoAgregado.Codigo.val(), false);
}

function Etapa2FechamentoAgregadoReprovada() {
    $("#" + _etapaFechamentoAgregado.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamentoAgregado.Etapa2.idTab + " .step").attr("class", "step red");

    _etapaFechamentoAgregado.Etapa2.eventClick = function () { }; //BuscarFechamentoAgregadoPorCodigo(_fechamentoAgregado.Codigo.val(), false);
}

function Etapa2FechamentoAgregadoAprovada() {
    $("#" + _etapaFechamentoAgregado.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamentoAgregado.Etapa2.idTab + " .step").attr("class", "step green");

    _etapaFechamentoAgregado.Etapa2.eventClick = function () { }; //BuscarFechamentoAgregadoPorCodigo(_fechamentoAgregado.Codigo.val(), false);
}

//*******Etapa 3*******
function Etapa3FechamentoAgregadoDesabilitada() {
    $("#" + _etapaFechamentoAgregado.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaFechamentoAgregado.Etapa3.idTab + " .step").attr("class", "step");

    _etapaFechamentoAgregado.Etapa3.eventClick = function () { };
}

function Etapa3FechamentoAgregadoAguardando() {
    $("#" + _etapaFechamentoAgregado.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamentoAgregado.Etapa3.idTab + " .step").attr("class", "step yellow");

    _etapaFechamentoAgregado.Etapa3.eventClick = GridIntegracao();
}

function Etapa3FechamentoAgregadoReprovada() {
    $("#" + _etapaFechamentoAgregado.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamentoAgregado.Etapa3.idTab + " .step").attr("class", "step red");

    _etapaFechamentoAgregado.Etapa3.eventClick = GridIntegracao();
}

function Etapa3FechamentoAgregadoAprovada() {
    $("#" + _etapaFechamentoAgregado.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamentoAgregado.Etapa3.idTab + " .step").attr("class", "step green");

    _etapaFechamentoAgregado.Etapa3.eventClick = GridIntegracao();
}