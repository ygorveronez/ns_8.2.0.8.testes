//*******MAPEAMENTO KNOUCKOUT*******

var _etapaLoteEscrituracaoCancelamento;

var EtapaLoteEscrituracaoCancelamento = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("50%") });

    this.Etapa1 = PropertyEntity({
        text: "Documentos", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("Esta etapa é destinada à seleção se documentos."),
        tooltipTitle: ko.observable("Documentos")
    });
    this.Etapa2 = PropertyEntity({
        text: "Integração", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("Caso tenha integração, nessa etapa é possível acompanhar."),
        tooltipTitle: ko.observable("Integração")
    });
};

//*******EVENTOS*******

function LoadEtapasLoteEscrituracaoCancelamento() {
    _etapaLoteEscrituracaoCancelamento = new EtapaLoteEscrituracaoCancelamento();
    KoBindings(_etapaLoteEscrituracaoCancelamento, "knockoutEtapaLoteEscrituracaoCancelamento");
    Etapa1Liberada();
}

function SetarEtapaInicio() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaLoteEscrituracaoCancelamento.Etapa1.idTab).click();
}

function SetarEtapasLoteEscrituracaoCancelamento() {
    var situacao = _loteEscrituracaoCancelamento.Situacao.val();

    if (situacao == "")
        Etapa1Liberada();
    else if (situacao == EnumSituacaoLoteEscrituracaoCancelamento.EmCriacao)
        Etapa1Liberada();
    else if (situacao == EnumSituacaoLoteEscrituracaoCancelamento.AgIntegracao)
        Etapa2Aguardando();
    else if (situacao == EnumSituacaoLoteEscrituracaoCancelamento.Finalizado)
        Etapa2Aprovada();
    else if (situacao == EnumSituacaoLoteEscrituracaoCancelamento.FalhaIntegracao)
        Etapa2Reprovada();
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaLoteEscrituracaoCancelamento.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLoteEscrituracaoCancelamento.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaLoteEscrituracaoCancelamento.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLoteEscrituracaoCancelamento.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    $("#" + _etapaLoteEscrituracaoCancelamento.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaLoteEscrituracaoCancelamento.Etapa2.idTab + " .step").attr("class", "step");
    _etapaLoteEscrituracaoCancelamento.Etapa2.eventClick = function () { };
}
function Etapa2Aguardando() {
    $("#" + _etapaLoteEscrituracaoCancelamento.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLoteEscrituracaoCancelamento.Etapa2.idTab + " .step").attr("class", "step yellow");
    _etapaLoteEscrituracaoCancelamento.Etapa2.eventClick = BuscarDadosIntegracoesLoteEscrituracaoCancelamento;
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    $("#" + _etapaLoteEscrituracaoCancelamento.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLoteEscrituracaoCancelamento.Etapa2.idTab + " .step").attr("class", "step red");
    _etapaLoteEscrituracaoCancelamento.Etapa2.eventClick = BuscarDadosIntegracoesLoteEscrituracaoCancelamento;
    Etapa1Aprovada();
}

function Etapa2Aprovada() {
    $("#" + _etapaLoteEscrituracaoCancelamento.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLoteEscrituracaoCancelamento.Etapa2.idTab + " .step").attr("class", "step green");
    _etapaLoteEscrituracaoCancelamento.Etapa2.eventClick = BuscarDadosIntegracoesLoteEscrituracaoCancelamento;
    Etapa1Aprovada();
}