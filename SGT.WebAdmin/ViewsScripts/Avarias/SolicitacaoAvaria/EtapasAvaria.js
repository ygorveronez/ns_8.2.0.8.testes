/// <reference path="SolicitacaoAvaria.js" />
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
/// <reference path="../../Enumeradores/EnumSituacaoAvaria.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _etapaAvaria;

var EtapaAvaria = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("33%") });

    this.Etapa1 = PropertyEntity({
        text: "Avaria", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("É onde se informar os dados iniciais para a geração de uma solicitação de avaria."),
        tooltipTitle: ko.observable("Avaria")
    });
    this.Etapa2 = PropertyEntity({
        text: "Aprovação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("Caso a solicitação necessite de aprovação ela será realizada nessa etapa."),
        tooltipTitle: ko.observable("Aprovação")
    });
    this.Etapa3 = PropertyEntity({
        text: "Lote", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable("Esta etapa é onde os lotes são gerados em caso de necessidade."),
        tooltipTitle: ko.observable("Lote")
    });
}


//*******EVENTOS*******

function loadEtapasAvaria() {
    _etapaAvaria = new EtapaAvaria();
    KoBindings(_etapaAvaria, "knockoutEtapaAvaria");
    Etapa1Liberada();
}

function setarEtapaInicioAvaria() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaAvaria.Etapa1.idTab).click();
}

function setarEtapasAvaria() {
    var situacaoAvaria = _solicitacaoAvaria.Situacao.val();

    if (situacaoAvaria == EnumSituacaoAvaria.Todas)
        Etapa1Liberada();
    else if (situacaoAvaria == EnumSituacaoAvaria.EmCriacao)
        Etapa1Liberada();
    else if (situacaoAvaria == EnumSituacaoAvaria.AgAprovacao)
        Etapa2Aguardando();
    else if (situacaoAvaria == EnumSituacaoAvaria.SemRegraAprovacao)
        Etapa2Aguardando();
    else if (situacaoAvaria == EnumSituacaoAvaria.SemRegraLote)
        Etapa3Reprovada();
    else if (situacaoAvaria == EnumSituacaoAvaria.AgLote)
        Etapa3Aguardando();
    else if (situacaoAvaria == EnumSituacaoAvaria.AgIntegracao)
        Etapa3Liberada();
    else if (situacaoAvaria == EnumSituacaoAvaria.LoteGerado)
        Etapa3Aguardando();
    else if (situacaoAvaria == EnumSituacaoAvaria.Finalizada)
        Etapa3Aprovada();
    else if (situacaoAvaria == EnumSituacaoAvaria.RejeitadaAutorizacao)
        Etapa2Reprovada();
    else if (situacaoAvaria == EnumSituacaoAvaria.RejeitadaIntegracao)
        Etapa3Reprovada();
    else if (situacaoAvaria == EnumSituacaoAvaria.Cancelada)
        Etapa1Reprovada();
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
    Etapa3Desabilitada();
}

function EtapaSemRegra() {
    exibirMensagem(tipoMensagem.aviso, "Regras da etapa", "Nenhuma regra encontrada. A solicitação permanecerá aguardando autorização.");
    _CRUDAvaria.Reprocessar.visible(true);

    _detalhesSolicitacao.MensagemEtapaSemRegra.visible(true);
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaAvaria.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvaria.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaAvaria.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvaria.Etapa1.idTab + " .step").attr("class", "step green");
}

function Etapa1Reprovada() {
    $("#" + _etapaAvaria.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvaria.Etapa1.idTab + " .step").attr("class", "step red");

    Etapa2Desabilitada();
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    $("#" + _etapaAvaria.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaAvaria.Etapa2.idTab + " .step").attr("class", "step");
    Etapa3Desabilitada()
}

function Etapa2Aguardando() {
    $("#" + _etapaAvaria.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvaria.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
    Etapa3Desabilitada();
}

function Etapa2Aprovada() {
    $("#" + _etapaAvaria.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvaria.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    $("#" + _etapaAvaria.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvaria.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
}

function Etapa2SemRegra() {
    $("#" + _etapaAvaria.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvaria.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
    EtapaSemRegra();
}


//*******Etapa 3*******

function Etapa3Desabilitada() {
    $("#" + _etapaAvaria.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaAvaria.Etapa3.idTab + " .step").attr("class", "step");
}

function Etapa3Aguardando() {
    $("#" + _etapaAvaria.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvaria.Etapa3.idTab + " .step").attr("class", "step yellow");
    Etapa2Aprovada();
}

function Etapa3Aprovada() {
    $("#" + _etapaAvaria.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvaria.Etapa3.idTab + " .step").attr("class", "step green");
    Etapa2Aprovada();
}

function Etapa3Problema() {
    $("#" + _etapaAvaria.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvaria.Etapa3.idTab + " .step").attr("class", "step red");
    Etapa2Aprovada();
}

function Etapa3SemRegra() {
    $("#" + _etapaAvaria.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvaria.Etapa3.idTab + " .step").attr("class", "step red");
    Etapa2Aprovada();
    EtapaSemRegra();
}

function Etapa3Reprovada() {
    $("#" + _etapaAvaria.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvaria.Etapa3.idTab + " .step").attr("class", "step red");
    Etapa2Aprovada();
}

function Etapa3Liberada() {
    $("#" + _etapaAvaria.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvaria.Etapa3.idTab + " .step").attr("class", "step blue");
    Etapa2Aprovada();
}