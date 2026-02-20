//*******MAPEAMENTO KNOUCKOUT*******

var _etapaCargaCTeAgrupado;

var EtapaCargaCTeAgrupado = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("50%") });

    this.Etapa1 = PropertyEntity({
        text: "Cargas", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("Esta etapa é destinada à seleção de cargas."),
        tooltipTitle: ko.observable("Cargas")
    });

    this.Etapa2 = PropertyEntity({
        text: "Documentos", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("Esta etapa é destinada aos documentos emitidos."),
        tooltipTitle: ko.observable("Documentos")
    });

    this.Etapa3 = PropertyEntity({
        text: Localization.Resources.Gerais.Geral.Integracao, type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        tooltip: ko.observable("Integrações entre sistemas"),
        tooltipTitle: ko.observable("Integração")
    });
};

//*******EVENTOS*******

function LoadEtapasCargaCTeAgrupado() {
    _etapaCargaCTeAgrupado = new EtapaCargaCTeAgrupado();
    KoBindings(_etapaCargaCTeAgrupado, "knockoutEtapaCTeAgrupado");

    Etapa1Liberada();
}

function SetarEtapaInicio() {
    DesabilitarTodasEtapasCTeAgrupado();
    Etapa1Liberada();
    $("#" + _etapaCargaCTeAgrupado.Etapa1.idTab).click();
}

function SetarEtapasCargaCTeAgrupado() {
    var situacao = _cargaCTeAgrupado.Situacao.val();

    if (situacao === "")
        Etapa1Liberada();
    else if (situacao === EnumSituacaoCargaCTeAgrupado.EmEmissao)
        Etapa2Aguardando();
    else if (situacao === EnumSituacaoCargaCTeAgrupado.Finalizado || situacao === EnumSituacaoCargaCTeAgrupado.EmCancelamento || situacao === EnumSituacaoCargaCTeAgrupado.Cancelado)
        Etapa3Aprovada();
    else if (situacao === EnumSituacaoCargaCTeAgrupado.Rejeitado)
        Etapa2Reprovada();
    else if (situacao === EnumSituacaoCargaCTeAgrupado.AgIntegracao)
        Etapa3Aguardando();
    else if (situacao === EnumSituacaoCargaCTeAgrupado.FalhaIntegracao)
        Etapa3Reprovada();

}

function DesabilitarTodasEtapasCTeAgrupado() {
    Etapa2Desabilitada();
    Etapa3Desabilitada();

    $("#" + _etapaCargaCTeAgrupado.Etapa1.idTab)[0].click();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaCargaCTeAgrupado.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCargaCTeAgrupado.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaCargaCTeAgrupado.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCargaCTeAgrupado.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    $("#" + _etapaCargaCTeAgrupado.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaCargaCTeAgrupado.Etapa2.idTab + " .step").attr("class", "step");

    Etapa3Desabilitada();
    _etapaCargaCTeAgrupado.Etapa2.eventClick = function () { };
}
function Etapa2Aguardando() {
    $("#" + _etapaCargaCTeAgrupado.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCargaCTeAgrupado.Etapa2.idTab + " .step").attr("class", "step yellow");

    _etapaCargaCTeAgrupado.Etapa2.eventClick = BuscarDocumentosCargaCTeAgrupado;

    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    $("#" + _etapaCargaCTeAgrupado.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCargaCTeAgrupado.Etapa2.idTab + " .step").attr("class", "step red");

    _etapaCargaCTeAgrupado.Etapa2.eventClick = BuscarDocumentosCargaCTeAgrupado;

    Etapa1Aprovada();
}

function Etapa2Aprovada() {
    $("#" + _etapaCargaCTeAgrupado.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCargaCTeAgrupado.Etapa2.idTab + " .step").attr("class", "step green");

    _etapaCargaCTeAgrupado.Etapa2.eventClick = BuscarDocumentosCargaCTeAgrupado;

    Etapa1Aprovada();
}


//*******Etapa 3*******

function Etapa3Desabilitada() {
    _etapaCargaCTeAgrupado.Etapa3.eventClick = function () { };
    $("#" + _etapaCargaCTeAgrupado.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaCargaCTeAgrupado.Etapa3.idTab + " .step").attr("class", "step");
}

function Etapa3Liberada() {
    _etapaCargaCTeAgrupado.Etapa3.eventClick = BuscarDadosIntegracoesCTeAgrupado;
    $("#" + _etapaCargaCTeAgrupado.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCargaCTeAgrupado.Etapa3.idTab + " .step").attr("class", "step yellow");
    Etapa2Aprovada();
}

function Etapa3Aguardando() {
    _etapaCargaCTeAgrupado.Etapa3.eventClick = BuscarDadosIntegracoesCTeAgrupado;
    $("#" + _etapaCargaCTeAgrupado.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCargaCTeAgrupado.Etapa3.idTab + " .step").attr("class", "step yellow");
    Etapa2Aprovada();
}

function Etapa3Aprovada() {
    _etapaCargaCTeAgrupado.Etapa3.eventClick = BuscarDadosIntegracoesCTeAgrupado;
    $("#" + _etapaCargaCTeAgrupado.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCargaCTeAgrupado.Etapa3.idTab + " .step").attr("class", "step green");
    Etapa2Aprovada();
}

function Etapa3Reprovada() {
    _etapaCargaCTeAgrupado.Etapa3.eventClick = BuscarDadosIntegracoesCTeAgrupado;
    $("#" + _etapaCargaCTeAgrupado.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCargaCTeAgrupado.Etapa3.idTab + " .step").attr("class", "step red");
    Etapa2Aprovada();
}