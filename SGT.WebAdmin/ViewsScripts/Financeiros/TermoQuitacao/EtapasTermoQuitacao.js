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
/// <reference path="../../Enumeradores/EnumSituacaoTermoQuitacaoFinanciero.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _etapaTermoQuitacaoFinanceiro;

var EtapaTermoQuitacaoFinanceiro = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("16%") });

    this.Etapa1 = PropertyEntity({
        text: "Termo Quitação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("Etapa Inicial para termo de quitação"),
        tooltipTitle: ko.observable("Avaria")
    });
    this.Etapa2 = PropertyEntity({
        text: "Aprovação Provisão", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable("Caso a solicitação necessite de aprovação ela será realizada nessa etapa."),
        tooltipTitle: ko.observable("Produtos Avariados")
    });
    this.Etapa3 = PropertyEntity({
        text: "Aprovação Transportador", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable("Caso a solicitação necessite de aprovação ela será realizada nessa etapa."),
        tooltipTitle: ko.observable("Aprovação")
    });
}


//*******EVENTOS*******

function loadEtapasTermoQuitacaoFinanceiro() {
    _etapaTermoQuitacaoFinanceiro = new EtapaTermoQuitacaoFinanceiro();
    KoBindings(_etapaTermoQuitacaoFinanceiro, "knockoutEtapaTermoQuitacaoFinanceiro");
    Etapa1Liberada();
}

function setarEtapaInicioTermoQuitacao() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaTermoQuitacaoFinanceiro.Etapa1.idTab)[0].click();
}

function setarEtapasSetarEtapaTermoQuitacao() {
    DesabilitarTodasEtapas();
    if (_termoQuitacao.Situacao.val() == EnumSituacaoTermoQuitacaoFinanceiro.Novo)
        Etapa1Aprovada();
    else if (_termoQuitacao.Situacao.val() == EnumSituacaoTermoQuitacaoFinanceiro.AguardandoAprovacaoProvisao)
        Etapa2Liberada();
    else if (_termoQuitacao.Situacao.val() == EnumSituacaoTermoQuitacaoFinanceiro.RejeitadoProvisao)
        Etapa2Reprovada();
    else if (_termoQuitacao.Situacao.val() == EnumSituacaoTermoQuitacaoFinanceiro.SemRegraProvisao)
        Etapa2SemRegra();
    else if (_termoQuitacao.Situacao.val() == EnumSituacaoTermoQuitacaoFinanceiro.AguardandoAprovacaoTransportador)
        Etapa3Liberada();
    else if (_termoQuitacao.Situacao.val() == EnumSituacaoTermoQuitacaoFinanceiro.RejeitadoTransportador)
        Etapa3Reprovada();

}

function DefinirTab() {
    if (_termoQuitacao.Situacao.val() == EnumSituacaoTermoQuitacaoFinanceiro.Dados || _termoQuitacao.Situacao.val() == EnumSituacaoTermoQuitacaoFinanceiro.Finalizado)
        $("#" + _etapaTermoQuitacaoFinanceiro.Etapa1.idTab).tab("show");
    else if (_termoQuitacao.Situacao.val() == EnumSituacaoTermoQuitacaoFinanceiro.Produtos)
        $("#" + _etapaTermoQuitacaoFinanceiro.Etapa2.idTab).tab("show");
    else if (_termoQuitacao.Situacao.val() == EnumSituacaoTermoQuitacaoFinanceiro.AgAprovacao || _termoQuitacao.Situacao.val() == EnumSituacaoTermoQuitacaoFinanceiro.SemRegraAprovacao || _termoQuitacao.Situacao.val() == EnumSituacaoTermoQuitacaoFinanceiro.RejeitadaAutorizacao)
        $("#" + _etapaTermoQuitacaoFinanceiro.Etapa3.idTab).tab("show");
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
    Etapa3Desabilitada();
}

function EtapaSemRegra() {
    exibirMensagem(tipoMensagem.aviso, "Regras da etapa", "Nenhuma regra encontrada. A solicitação permanecerá aguardando autorização.");
    _crudTermoQuitacao.Reprocessar.visible(true);
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaTermoQuitacaoFinanceiro.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTermoQuitacaoFinanceiro.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaTermoQuitacaoFinanceiro.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTermoQuitacaoFinanceiro.Etapa1.idTab + " .step").attr("class", "step green");
}

function Etapa1Reprovada() {
    $("#" + _etapaTermoQuitacaoFinanceiro.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTermoQuitacaoFinanceiro.Etapa1.idTab + " .step").attr("class", "step red");

    Etapa2Desabilitada();
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    $("#" + _etapaTermoQuitacaoFinanceiro.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaTermoQuitacaoFinanceiro.Etapa2.idTab + " .step").attr("class", "step");
    Etapa3Desabilitada()
}

function Etapa2Aguardando() {
    $("#" + _etapaTermoQuitacaoFinanceiro.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTermoQuitacaoFinanceiro.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
    Etapa3Desabilitada();
}

function Etapa2Aprovada() {
    $("#" + _etapaTermoQuitacaoFinanceiro.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTermoQuitacaoFinanceiro.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    $("#" + _etapaTermoQuitacaoFinanceiro.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTermoQuitacaoFinanceiro.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
}

function Etapa2SemRegra() {
    $("#" + _etapaTermoQuitacaoFinanceiro.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTermoQuitacaoFinanceiro.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
    EtapaSemRegra();
}

function Etapa2Liberada() {
    $("#" + _etapaTermoQuitacaoFinanceiro.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTermoQuitacaoFinanceiro.Etapa2.idTab + " .step").attr("class", "step blue");
    $("#" + _etapaTermoQuitacaoFinanceiro.Etapa2.idTab).tab("show");
    Etapa1Aprovada();
}


//*******Etapa 3*******

function Etapa3Desabilitada() {
    $("#" + _etapaTermoQuitacaoFinanceiro.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaTermoQuitacaoFinanceiro.Etapa3.idTab + " .step").attr("class", "step");
}

function Etapa3Aguardando() {
    $("#" + _etapaTermoQuitacaoFinanceiro.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTermoQuitacaoFinanceiro.Etapa3.idTab + " .step").attr("class", "step yellow");
    Etapa2Aprovada();
}

function Etapa3Aprovada() {
    $("#" + _etapaTermoQuitacaoFinanceiro.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTermoQuitacaoFinanceiro.Etapa3.idTab + " .step").attr("class", "step green");
    Etapa2Aprovada();
}

function Etapa3Reprovada() {
    $("#" + _etapaTermoQuitacaoFinanceiro.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTermoQuitacaoFinanceiro.Etapa3.idTab + " .step").attr("class", "step red");
    Etapa2Aprovada();
}

function Etapa3SemRegra() {
    Etapa3Aguardando();
    EtapaSemRegra();
}

function Etapa3Liberada() {
    $("#" + _etapaTermoQuitacaoFinanceiro.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTermoQuitacaoFinanceiro.Etapa3.idTab + " .step").attr("class", "step blue");
    $("#" + _etapaTermoQuitacaoFinanceiro.Etapa3.idTab).tab("show");
    Etapa2Aprovada();
}
