//*******MAPEAMENTO KNOUCKOUT*******

var _etapaLoteContabilizacao;

var EtapaLoteContabilizacao = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("50%") });

    this.Etapa1 = PropertyEntity({
        text: "Movimentos", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("Esta etapa é destinada à seleção de movimentos."),
        tooltipTitle: ko.observable("Documentos")
    });
    this.Etapa2 = PropertyEntity({
        text: "Integração", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("Esta etapa é destinada às integrações dos movimentos, caso disponível."),
        tooltipTitle: ko.observable("Integração")
    });
};

//*******EVENTOS*******

function LoadEtapasLoteContabilizacao() {
    _etapaLoteContabilizacao = new EtapaLoteContabilizacao();
    KoBindings(_etapaLoteContabilizacao, "knockoutEtapaLoteContabilizacao");

    Etapa1Liberada();
}

function SetarEtapaInicio() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaLoteContabilizacao.Etapa1.idTab).click();
}

function SetarEtapasLoteContabilizacao() {
    var situacao = _loteContabilizacao.Situacao.val();

    if (situacao == EnumSituacaoLoteContabilizacao.EmCriacao)
        Etapa1Liberada();
    else if (situacao == EnumSituacaoLoteContabilizacao.AgIntegracao)
        Etapa2Aguardando();
    else if (situacao == EnumSituacaoLoteContabilizacao.Finalizado)
        Etapa2Aprovada();
    else if (situacao == EnumSituacaoLoteContabilizacao.FalhaIntegracao)
        Etapa2Reprovada();
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaLoteContabilizacao.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLoteContabilizacao.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaLoteContabilizacao.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLoteContabilizacao.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    $("#" + _etapaLoteContabilizacao.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaLoteContabilizacao.Etapa2.idTab + " .step").attr("class", "step");

    _etapaLoteContabilizacao.Etapa2.eventClick = function () { };
}
function Etapa2Aguardando() {
    $("#" + _etapaLoteContabilizacao.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLoteContabilizacao.Etapa2.idTab + " .step").attr("class", "step yellow");

    _etapaLoteContabilizacao.Etapa2.eventClick = BuscarDadosIntegracoesLoteContabilizacao;

    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    $("#" + _etapaLoteContabilizacao.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLoteContabilizacao.Etapa2.idTab + " .step").attr("class", "step red");

    _etapaLoteContabilizacao.Etapa2.eventClick = BuscarDadosIntegracoesLoteContabilizacao;

    Etapa1Aprovada();
}

function Etapa2Aprovada() {
    $("#" + _etapaLoteContabilizacao.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLoteContabilizacao.Etapa2.idTab + " .step").attr("class", "step green");

    _etapaLoteContabilizacao.Etapa2.eventClick = BuscarDadosIntegracoesLoteContabilizacao;

    Etapa1Aprovada();
}