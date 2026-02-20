//*******MAPEAMENTO KNOUCKOUT*******

var _etapaLoteCliente;

var EtapaLoteCliente = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("50%") });

    this.Etapa1 = PropertyEntity({
        text: "Clientes", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("Esta etapa é destinada à seleção de clientes."),
        tooltipTitle: ko.observable("Documentos")
    });
    this.Etapa2 = PropertyEntity({
        text: "Integração", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("Esta etapa é destinada às integrações dos clientes, caso disponível."),
        tooltipTitle: ko.observable("Integração")
    });
};

//*******EVENTOS*******

function LoadEtapasLoteCliente() {
    _etapaLoteCliente = new EtapaLoteCliente();
    KoBindings(_etapaLoteCliente, "knockoutEtapaLoteCliente");

    Etapa1Liberada();
}

function SetarEtapaInicio() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaLoteCliente.Etapa1.idTab).click();
}

function SetarEtapasLoteCliente() {
    var situacao = _loteCliente.Situacao.val();

    if (situacao === EnumSituacaoLoteCliente.EmCriacao)
        Etapa1Liberada();
    else if (situacao === EnumSituacaoLoteCliente.AgIntegracao)
        Etapa2Aguardando();
    else if (situacao === EnumSituacaoLoteCliente.Finalizado)
        Etapa2Aprovada();
    else if (situacao === EnumSituacaoLoteCliente.FalhaIntegracao)
        Etapa2Reprovada();
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaLoteCliente.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLoteCliente.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaLoteCliente.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLoteCliente.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    $("#" + _etapaLoteCliente.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaLoteCliente.Etapa2.idTab + " .step").attr("class", "step");
}

function Etapa2Aguardando() {
    $("#" + _etapaLoteCliente.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLoteCliente.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    $("#" + _etapaLoteCliente.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLoteCliente.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
}

function Etapa2Aprovada() {
    $("#" + _etapaLoteCliente.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLoteCliente.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1Aprovada();
}