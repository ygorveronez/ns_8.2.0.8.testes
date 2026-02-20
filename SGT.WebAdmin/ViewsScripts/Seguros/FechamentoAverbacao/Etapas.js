/// <reference path="DadosFechamento.js" />
/// <reference path="FechamentoAverbacao.js" />
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
/// <reference path="../../Enumeradores/EnumSituacaoFechamentoAverbacoes.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _etapaFechamentoAverbacao;

var EtapaFechamentoAverbacao = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("50%") });

    this.Etapa1 = PropertyEntity({
        text: "Dados Fechamento", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("Esta etapa é destinada aos dados do fechamento."),
        tooltipTitle: ko.observable("Dados")
    });
    this.Etapa2 = PropertyEntity({
        text: "Averbações", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("É a listagem das averbações referente ao fechamento.."),
        tooltipTitle: ko.observable("Averbações")
    });
}


//*******EVENTOS*******

function loadEtapasFechamentoAverbacao() {
    _etapaFechamentoAverbacao = new EtapaFechamentoAverbacao();
    KoBindings(_etapaFechamentoAverbacao, "knockoutEtapaFechamentoAverbacao");
    Etapa1Liberada();
}

function SetarEtapaInicioFechamento() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaFechamentoAverbacao.Etapa1.idTab).click();
}

function SetarEtapaFechamento() {
    var situacao = _fechamentoAverbacao.Situacao.val();

    if (situacao == EnumSituacaoFechamentoAverbacoes.Todas)
        Etapa1Liberada();
    else if (situacao == EnumSituacaoFechamentoAverbacoes.Aberta)
        Etapa2Liberada();
    else if (situacao == EnumSituacaoFechamentoAverbacoes.Fechada)
        Etapa2Aprovada();
    else if (situacao == EnumSituacaoFechamentoAverbacoes.Cancelada)
        Etapa2Rejeitada();
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaFechamentoAverbacao.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamentoAverbacao.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaFechamentoAverbacao.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamentoAverbacao.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    $("#" + _etapaFechamentoAverbacao.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaFechamentoAverbacao.Etapa2.idTab + " .step").attr("class", "step");
}

function Etapa2Aprovada() {
    $("#" + _etapaFechamentoAverbacao.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamentoAverbacao.Etapa2.idTab + " .step").attr("class", "step green");
    $("#" + _etapaFechamentoAverbacao.Etapa2.idTab).click();
    Etapa1Aprovada();
}

function Etapa2Liberada() {
    $("#" + _etapaFechamentoAverbacao.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamentoAverbacao.Etapa2.idTab + " .step").attr("class", "step yellow");
    $("#" + _etapaFechamentoAverbacao.Etapa2.idTab).click();
    Etapa1Aprovada();
}

function Etapa2Rejeitada() {
    $("#" + _etapaFechamentoAverbacao.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamentoAverbacao.Etapa2.idTab + " .step").attr("class", "step red");
    $("#" + _etapaFechamentoAverbacao.Etapa2.idTab).click();
    Etapa1Aprovada();
}