/// <reference path="../SolicitacaoAvaria/SolicitacaoAvaria.js" />
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

var EtapaFluxoAvaria = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("16%") });

    this.Etapa1 = PropertyEntity({
        text: "Avaria", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("É onde se informar os dados iniciais para a geração de uma solicitação de avaria."),
        tooltipTitle: ko.observable("Avaria")
    });
    this.Etapa2 = PropertyEntity({
        text: "Produtos Avariados", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable("Esta etapa é onde são informados os produtos avariados."),
        tooltipTitle: ko.observable("Produtos Avariados")
    });
    this.Etapa3 = PropertyEntity({
        text: "Aprovação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable("Caso a solicitação necessite de aprovação ela será realizada nessa etapa."),
        tooltipTitle: ko.observable("Aprovação")
    });
    this.Etapa4 = PropertyEntity({
        text: "Termo", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable("Esta é a etapa do termo da avaria."),
        tooltipTitle: ko.observable("Termo")
    });
    this.Etapa5 = PropertyEntity({
        text: "Lote", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(5),
        tooltip: ko.observable("Esta etapa é onde os lotes são gerados em caso de necessidade."),
        tooltipTitle: ko.observable("Lote")
    });
    this.Etapa6 = PropertyEntity({
        text: "Destinação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(5),
        tooltip: ko.observable("Esta é a etapa de destinação."),
        tooltipTitle: ko.observable("Destinação")
    });
}


//*******EVENTOS*******

function loadEtapasFluxoAvaria() {
    _etapaAvaria = new EtapaFluxoAvaria();
    KoBindings(_etapaAvaria, "knockoutEtapaAvaria");
    Etapa1Liberada();
}

function setarEtapaInicioAvaria() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaAvaria.Etapa1.idTab)[0].click();
}

function setarEtapasFluxoAvaria() {
    DesabilitarTodasEtapas();

    //Aguardando desenvolvimento do fluxo de etapas
    if (_fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.Dados)
        Etapa1Liberada();
    else if (_fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.Produtos)
        Etapa2Liberada();
    else if (_fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.AgAprovacao)
        Etapa3Aguardando();
    else if (_fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.SemRegraAprovacao)
        Etapa3SemRegra();
    else if (_fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.RejeitadaAutorizacao)
        Etapa3Reprovada();
    else if (_fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.Termo)
        Etapa4Liberada();
    else if (_fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.AgLote)
        Etapa5Liberada();
    else if (_fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.LoteGerado)
        Etapa5Aguardando();
    else if (_fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.Destinacao)
        Etapa6Aguardando();
    else if (_fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.Finalizado)
        Etapa6Aprovada();
}

function DefinirTab() {
    if (_fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.Dados || _fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.Finalizado)
        $("#" + _etapaAvaria.Etapa1.idTab).tab("show");
    else if (_fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.Produtos)
        $("#" + _etapaAvaria.Etapa2.idTab).tab("show");
    else if (_fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.AgAprovacao || _fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.SemRegraAprovacao || _fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.RejeitadaAutorizacao)
        $("#" + _etapaAvaria.Etapa3.idTab).tab("show");
    else if (_fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.Termo)
        $("#" + _etapaAvaria.Etapa4.idTab).tab("show");
    else if (_fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.AgLote || _fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.LoteGerado)
        $("#" + _etapaAvaria.Etapa5.idTab).tab("show");
    else if (_fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.Destinacao)
        $("#" + _etapaAvaria.Etapa6.idTab).tab("show");
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
    Etapa3Desabilitada();
    Etapa4Desabilitada();
    Etapa5Desabilitada();
    Etapa6Desabilitada();
}

function EtapaSemRegra() {
    exibirMensagem(tipoMensagem.aviso, "Regras da etapa", "Nenhuma regra encontrada. A solicitação permanecerá aguardando autorização.");
    _CRUDAvaria.Reprocessar.visible(true);

    _detalhesFluxoAprovacao.MensagemEtapaSemRegra.visible(true);
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

function Etapa2Liberada() {
    $("#" + _etapaAvaria.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvaria.Etapa2.idTab + " .step").attr("class", "step blue");
    Etapa1Aprovada();
}


//*******Etapa 3*******

function Etapa3Desabilitada() {
    $("#" + _etapaAvaria.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaAvaria.Etapa3.idTab + " .step").attr("class", "step");
    Etapa4Desabilitada()
}

function Etapa3Aguardando() {
    $("#" + _etapaAvaria.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvaria.Etapa3.idTab + " .step").attr("class", "step yellow");
    Etapa2Aprovada();
    Etapa4Desabilitada();
}

function Etapa3Aprovada() {
    $("#" + _etapaAvaria.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvaria.Etapa3.idTab + " .step").attr("class", "step green");
    Etapa2Aprovada();
}

function Etapa3Reprovada() {
    $("#" + _etapaAvaria.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvaria.Etapa3.idTab + " .step").attr("class", "step red");
    Etapa2Aprovada();
}

function Etapa3SemRegra() {
    Etapa3Aguardando();
    EtapaSemRegra();
}

function Etapa3Liberada() {
    $("#" + _etapaAvaria.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvaria.Etapa3.idTab + " .step").attr("class", "step blue");
    Etapa2Aprovada();
}

//*******Etapa 4*******

function Etapa4Desabilitada() {
    $("#" + _etapaAvaria.Etapa4.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaAvaria.Etapa4.idTab + " .step").attr("class", "step");
    Etapa5Desabilitada()
}

function Etapa4Aguardando() {
    $("#" + _etapaAvaria.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvaria.Etapa4.idTab + " .step").attr("class", "step yellow");
    Etapa3Aprovada();
    Etapa5Desabilitada();
}

function Etapa4Aprovada() {
    $("#" + _etapaAvaria.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvaria.Etapa4.idTab + " .step").attr("class", "step green");
    Etapa3Aprovada();
}

function Etapa4Reprovada() {
    $("#" + _etapaAvaria.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvaria.Etapa4.idTab + " .step").attr("class", "step red");
    Etapa3Aprovada();
}

function Etapa4SemRegra() {
    $("#" + _etapaAvaria.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvaria.Etapa4.idTab + " .step").attr("class", "step red");
    Etapa3Aprovada();
    EtapaSemRegra();
}

function Etapa4Liberada() {
    $("#" + _etapaAvaria.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvaria.Etapa4.idTab + " .step").attr("class", "step blue");
    Etapa3Aprovada();
    Etapa5Desabilitada();
}

//*******Etapa 5*******

function Etapa5Desabilitada() {
    $("#" + _etapaAvaria.Etapa5.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaAvaria.Etapa5.idTab + " .step").attr("class", "step");
    Etapa6Desabilitada();
}

function Etapa5Aguardando() {
    $("#" + _etapaAvaria.Etapa5.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvaria.Etapa5.idTab + " .step").attr("class", "step yellow");
    Etapa4Aprovada();
    Etapa6Desabilitada();
}

function Etapa5Aprovada() {
    $("#" + _etapaAvaria.Etapa5.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvaria.Etapa5.idTab + " .step").attr("class", "step green");
    Etapa4Aprovada();
}

function Etapa5Problema() {
    $("#" + _etapaAvaria.Etapa5.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvaria.Etapa5.idTab + " .step").attr("class", "step red");
    Etapa4Aprovada();
    Etapa6Desabilitada();
}

function Etapa5SemRegra() {
    $("#" + _etapaAvaria.Etapa5.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvaria.Etapa5.idTab + " .step").attr("class", "step red");
    Etapa4Aprovada();
    EtapaSemRegra();
}

function Etapa5Reprovada() {
    $("#" + _etapaAvaria.Etapa5.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvaria.Etapa5.idTab + " .step").attr("class", "step red");
    Etapa4Aprovada();
    Etapa6Desabilitada();
}

function Etapa5Liberada() {
    $("#" + _etapaAvaria.Etapa5.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvaria.Etapa5.idTab + " .step").attr("class", "step blue");
    Etapa4Aprovada();
    Etapa6Desabilitada();
}

//*******Etapa 6*******

function Etapa6Desabilitada() {
    $("#" + _etapaAvaria.Etapa6.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaAvaria.Etapa6.idTab + " .step").attr("class", "step");
}

function Etapa6Aguardando() {
    $("#" + _etapaAvaria.Etapa6.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvaria.Etapa6.idTab + " .step").attr("class", "step yellow");
    Etapa5Aprovada();
}

function Etapa6Aprovada() {
    $("#" + _etapaAvaria.Etapa6.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvaria.Etapa6.idTab + " .step").attr("class", "step green");
    Etapa5Aprovada();
}

function Etapa6Problema() {
    $("#" + _etapaAvaria.Etapa6.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvaria.Etapa6.idTab + " .step").attr("class", "step red");
    Etapa5Aprovada();
}

function Etapa6SemRegra() {
    $("#" + _etapaAvaria.Etapa6.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvaria.Etapa6.idTab + " .step").attr("class", "step red");
    Etapa5Aprovada();
    EtapaSemRegra();
}

function Etapa6Reprovada() {
    $("#" + _etapaAvaria.Etapa6.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvaria.Etapa6.idTab + " .step").attr("class", "step red");
    Etapa5Aprovada();
}

function Etapa6Liberada() {
    $("#" + _etapaAvaria.Etapa6.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvaria.Etapa6.idTab + " .step").attr("class", "step blue");
    Etapa5Aprovada();
}