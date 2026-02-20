//*******MAPEAMENTO KNOUCKOUT*******

var _etapaAutorizacaoPagamentoContratoFrete;

var EtapaAutorizacaoPagamentoContratoFrete = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("50%") });

    this.Etapa1 = PropertyEntity({
        text: "Contrato Frete", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("Esta etapa é destinada à seleção de contrato de frete."),
        tooltipTitle: ko.observable("Contrato de Frete")
    });

    this.Etapa2 = PropertyEntity({
        text: "Pagamento CIOT", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("Esta etapa é destinada a integração de Pagamento do CIOT."),
        tooltipTitle: ko.observable("Pagamento do CIOT")
    });

    this.Etapa3 = PropertyEntity({
        text: "Integração", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("Esta etapa é destinada as integrações adicionais."),
        tooltipTitle: ko.observable("Integração")
    });
};

//*******EVENTOS*******

function LoadEtapasAutorizacaoPagamentoContratoFrete() {
    _etapaAutorizacaoPagamentoContratoFrete = new EtapaAutorizacaoPagamentoContratoFrete();
    KoBindings(_etapaAutorizacaoPagamentoContratoFrete, "knockoutEtapaAutorizacaoPagamentoContratoFrete");

    Etapa1Liberada();
}

function SetarEtapaInicio() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaAutorizacaoPagamentoContratoFrete.Etapa1.idTab).click();
}

function SetarEtapasAutorizacaoPagamentoContratoFrete() {
    if (_autorizacaoPagamentoContratoFrete.Codigo.val() == 0) {
        Etapa1Liberada();
        Etapa2Desabilitada();
        Etapa3Desabilitada();

        $("#" + _etapaAutorizacaoPagamentoContratoFrete.Etapa1.idTab).click();
    }
    else if (_autorizacaoPagamentoContratoFrete.ContratoFretePagamentoCIOTIntegracaoPendente.val()) {
        Etapa1Aprovada();
        Etapa2Aguardando();
        Etapa3Desabilitada();

        $("#" + _etapaAutorizacaoPagamentoContratoFrete.Etapa2.idTab).click();
    }
    else if (!_autorizacaoPagamentoContratoFrete.ContratoFretePagamentoCIOTIntegracaoPendente.val() && _autorizacaoPagamentoContratoFrete.ContratoFretePagamentoCONTRATOIntegracaoPendente.val()) {
        Etapa1Aprovada();
        Etapa2Aprovada();
        Etapa3Aguardando();

        $("#" + _etapaAutorizacaoPagamentoContratoFrete.Etapa3.idTab).click();
    }
    else if (!_autorizacaoPagamentoContratoFrete.ContratoFretePagamentoCIOTIntegracaoPendente.val() && !_autorizacaoPagamentoContratoFrete.ContratoFretePagamentoCONTRATOIntegracaoPendente.val()) {
        Etapa1Aprovada();
        Etapa2Aprovada();
        Etapa3Aprovada();
    }
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
    Etapa3Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaAutorizacaoPagamentoContratoFrete.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAutorizacaoPagamentoContratoFrete.Etapa1.idTab + " .step").attr("class", "step yellow");

    _etapaAutorizacaoPagamentoContratoFrete.Etapa1.eventClick = function () { };
}

function Etapa1Aprovada() {
    $("#" + _etapaAutorizacaoPagamentoContratoFrete.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAutorizacaoPagamentoContratoFrete.Etapa1.idTab + " .step").attr("class", "step green");

    _etapaAutorizacaoPagamentoContratoFrete.Etapa1.eventClick = function () { };
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    $("#" + _etapaAutorizacaoPagamentoContratoFrete.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaAutorizacaoPagamentoContratoFrete.Etapa2.idTab + " .step").attr("class", "step");

    _etapaAutorizacaoPagamentoContratoFrete.Etapa2.eventClick = function () { };
}

function Etapa2Aguardando() {
    $("#" + _etapaAutorizacaoPagamentoContratoFrete.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAutorizacaoPagamentoContratoFrete.Etapa2.idTab + " .step").attr("class", "step yellow");

    _etapaAutorizacaoPagamentoContratoFrete.Etapa2.eventClick = GridPagamentoCIOT();
}

function Etapa2Reprovada() {
    $("#" + _etapaAutorizacaoPagamentoContratoFrete.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAutorizacaoPagamentoContratoFrete.Etapa2.idTab + " .step").attr("class", "step red");

    _etapaAutorizacaoPagamentoContratoFrete.Etapa2.eventClick = GridPagamentoCIOT();
}

function Etapa2Aprovada() {
    $("#" + _etapaAutorizacaoPagamentoContratoFrete.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAutorizacaoPagamentoContratoFrete.Etapa2.idTab + " .step").attr("class", "step green");

    _etapaAutorizacaoPagamentoContratoFrete.Etapa2.eventClick = GridPagamentoCIOT();
}

//*******Etapa 3*******
function Etapa3Desabilitada() {
    $("#" + _etapaAutorizacaoPagamentoContratoFrete.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaAutorizacaoPagamentoContratoFrete.Etapa3.idTab + " .step").attr("class", "step");

    _etapaAutorizacaoPagamentoContratoFrete.Etapa3.eventClick = function () { };
}

function Etapa3Aguardando() {
    $("#" + _etapaAutorizacaoPagamentoContratoFrete.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAutorizacaoPagamentoContratoFrete.Etapa3.idTab + " .step").attr("class", "step yellow");

    _etapaAutorizacaoPagamentoContratoFrete.Etapa3.eventClick = GridPagamentoIntegracao();
}

function Etapa3Reprovada() {
    $("#" + _etapaAutorizacaoPagamentoContratoFrete.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAutorizacaoPagamentoContratoFrete.Etapa3.idTab + " .step").attr("class", "step red");

    _etapaAutorizacaoPagamentoContratoFrete.Etapa3.eventClick = GridPagamentoIntegracao();
}

function Etapa3Aprovada() {
    $("#" + _etapaAutorizacaoPagamentoContratoFrete.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAutorizacaoPagamentoContratoFrete.Etapa3.idTab + " .step").attr("class", "step green");

    _etapaAutorizacaoPagamentoContratoFrete.Etapa3.eventClick = GridPagamentoIntegracao();
}