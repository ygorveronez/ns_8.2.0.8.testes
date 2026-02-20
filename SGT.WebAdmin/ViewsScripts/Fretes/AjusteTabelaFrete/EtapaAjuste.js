/// <reference path="Aprovadores.js" />
/// <reference path="AjusteTabelaFrete.js" />
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
/// <reference path="../../Enumeradores/EnumSituacaoAjusteTabelaFrete.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _etapaAjuste;

var EtapaAjuste = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("33%") });

    this.Etapa1 = PropertyEntity({
        text: "Ajuste", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("É onde se informar os dados do reajuste da tabela."),
        tooltipTitle: ko.observable("Avaria")
    });
    this.Etapa2 = PropertyEntity({
        text: "Aprovação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("Caso o reajuste necessite de aprovação ela será realizada nessa etapa."),
        tooltipTitle: ko.observable("Aprovação")
    });
    this.Etapa3 = PropertyEntity({
        text: "Integração", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable("Esta etapa é onde é efetuada a integração."),
        tooltipTitle: ko.observable("Integração")
    });
}


//*******EVENTOS*******

function loadEtapaAjuste() {
    _etapaAjuste = new EtapaAjuste();
    KoBindings(_etapaAjuste, "knockoutEtapaAjuste");
    Etapa1Liberada();
}

function setarEtapaInicioAjuste() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaAjuste.Etapa1.idTab).click();
}

function setarEtapaAjuste() {
    var situacao = _ajusteTabelaFrete.Situacao.val();

    if (situacao == EnumSituacaoAjusteTabelaFrete.Todas)
        Etapa1Liberada();
    else if (situacao == EnumSituacaoAjusteTabelaFrete.ProblemaProcessamento)
        Etapa1Reprovada();
    else if (situacao == EnumSituacaoAjusteTabelaFrete.ProblemaAjuste)
        Etapa1Reprovada();
    else if (situacao == EnumSituacaoAjusteTabelaFrete.ProblemaCriacao)
        Etapa1Reprovada();
    else if (situacao == EnumSituacaoAjusteTabelaFrete.Pendente)
        Etapa1Liberada();
    else if (situacao == EnumSituacaoAjusteTabelaFrete.EmCriacao)
        Etapa1Aguardando();
    else if (situacao == EnumSituacaoAjusteTabelaFrete.AgAprovacao)
        Etapa2Aguardando();
    else if (situacao == EnumSituacaoAjusteTabelaFrete.SemRegraAprovacao)
        Etapa2SemRegra();
    else if (situacao == EnumSituacaoAjusteTabelaFrete.RejeitadaAutorizacao)
        Etapa2Rejeitada();
    else if (situacao == EnumSituacaoAjusteTabelaFrete.AgIntegracao)
        Etapa3Aguardando();
    else if (situacao == EnumSituacaoAjusteTabelaFrete.Cancelado)
        Etapa1Reprovada();
    else if (situacao == EnumSituacaoAjusteTabelaFrete.Finalizado)
        Etapa3Aprovada();
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
    Etapa3Desabilitada();
}

function EtapaSemRegra() {
    exibirMensagem(tipoMensagem.aviso, "Regras da etapa", "Nenhuma regra encontrada. O reajuste permanecerá aguardando autorização.");

    _detalhesAjuste.MensagemEtapaSemRegra.visible(true);
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaAjuste.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAjuste.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaAjuste.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAjuste.Etapa1.idTab + " .step").attr("class", "step green");
}

function Etapa1Reprovada() {
    $("#" + _etapaAjuste.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAjuste.Etapa1.idTab + " .step").attr("class", "step red");
    Etapa2Desabilitada();
}

function Etapa1Aguardando() {
    $("#" + _etapaAjuste.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAjuste.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    $("#" + _etapaAjuste.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaAjuste.Etapa2.idTab + " .step").attr("class", "step");
    Etapa3Desabilitada()
}

function Etapa2Aguardando() {
    $("#" + _etapaAjuste.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAjuste.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
    Etapa3Desabilitada();
}

function Etapa2Aprovada() {
    $("#" + _etapaAjuste.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAjuste.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    $("#" + _etapaAjuste.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAjuste.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
}

function Etapa2SemRegra() {
    $("#" + _etapaAjuste.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAjuste.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
    EtapaSemRegra();
}

function Etapa2Rejeitada() {
    $("#" + _etapaAjuste.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAjuste.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
}


//*******Etapa 3*******

function Etapa3Desabilitada() {
    $("#" + _etapaAjuste.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaAjuste.Etapa3.idTab + " .step").attr("class", "step");
}

function Etapa3Aguardando() {
    $("#" + _etapaAjuste.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAjuste.Etapa3.idTab + " .step").attr("class", "step yellow");
    Etapa2Aprovada();
}

function Etapa3Aprovada() {
    $("#" + _etapaAjuste.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAjuste.Etapa3.idTab + " .step").attr("class", "step green");
    Etapa2Aprovada();
}

function Etapa3Problema() {
    $("#" + _etapaAjuste.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAjuste.Etapa3.idTab + " .step").attr("class", "step red");
    Etapa2Aprovada();
}

function Etapa3SemRegra() {
    $("#" + _etapaAjuste.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAjuste.Etapa3.idTab + " .step").attr("class", "step red");
    Etapa2Aprovada();
    EtapaSemRegra();
}

function Etapa3Reprovada() {
    $("#" + _etapaAjuste.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAjuste.Etapa3.idTab + " .step").attr("class", "step red");
    Etapa2Aprovada();
}

function Etapa3Liberada() {
    $("#" + _etapaAjuste.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAjuste.Etapa3.idTab + " .step").attr("class", "step blue");
    Etapa2Aprovada();
}