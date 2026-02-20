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


//*******MAPEAMENTO KNOUCKOUT*******

var _etapaCancelamentoPagamento;

var EtapaPagamento = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("33%") });

    this.Etapa1 = PropertyEntity({
        text: "Documentos", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("Esta etapa é destinada à seleção se documentos."),
        tooltipTitle: ko.observable("Documentos")
    });
    this.Etapa2 = PropertyEntity({
        text: "Fechamento", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("É a etapa onde a provisão é confirmada antes da integração."),
        tooltipTitle: ko.observable("Fechamento")
    });
    this.Etapa3 = PropertyEntity({
        text: "Integração", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable("Caso tenha integração, nessa etapa é possível acompanhar."),
        tooltipTitle: ko.observable("Integração")
    });
}


//*******EVENTOS*******

function loadEtapasPagamento() {
    _etapaCancelamentoPagamento = new EtapaPagamento();
    KoBindings(_etapaCancelamentoPagamento, "knockoutEtapaCancelamentoPagamento");
    Etapa1Liberada();
}

function SetarEtapaInicio() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaCancelamentoPagamento.Etapa1.idTab).click();
    $("#" + _etapaCancelamentoPagamento.Etapa1.idTab).tab("show")
}

function SetarEtapasPagamento() {
    var situacao = _cancelamentoPagamento.Situacao.val();

    if (situacao === 0)
        Etapa1Liberada();
    else if (situacao === EnumSituacaoCancelamentoPagamento.EmCancelamento)
        Etapa2Aguardando();
    else if (situacao === EnumSituacaoCancelamentoPagamento.PendenciaCancelamento)
        Etapa2Reprovada();
    else if (situacao === EnumSituacaoCancelamentoPagamento.AgIntegracao)
        Etapa3Aguardando();
    else if (situacao === EnumSituacaoCancelamentoPagamento.Cancelado)
        Etapa3Aprovada();
    else if (situacao === EnumSituacaoCancelamentoPagamento.FalhaIntegracao)
        Etapa3Reprovada();
    else if (situacao === EnumSituacaoCancelamentoPagamento.EmIntegracao)
        Etapa3Aguardando();
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaCancelamentoPagamento.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamentoPagamento.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaCancelamentoPagamento.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamentoPagamento.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    $("#" + _etapaCancelamentoPagamento.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaCancelamentoPagamento.Etapa2.idTab + " .step").attr("class", "step");
    _etapaCancelamentoPagamento.Etapa2.eventClick = function () { };
    Etapa3Desabilitada();
}
function Etapa2Aguardando() {
    $("#" + _etapaCancelamentoPagamento.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamentoPagamento.Etapa2.idTab + " .step").attr("class", "step yellow");
    $("#" + _etapaCancelamentoPagamento.Etapa2.idTab).tab("show")
    _etapaCancelamentoPagamento.Etapa2.eventClick = buscarDadosFechamentoCancelamentoPagamento;
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    $("#" + _etapaCancelamentoPagamento.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamentoPagamento.Etapa2.idTab + " .step").attr("class", "step red");
    _etapaCancelamentoPagamento.Etapa2.eventClick = buscarDadosFechamentoCancelamentoPagamento;
    Etapa1Aprovada();
}

function Etapa2Aprovada() {
    $("#" + _etapaCancelamentoPagamento.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamentoPagamento.Etapa2.idTab + " .step").attr("class", "step green");
    _etapaCancelamentoPagamento.Etapa2.eventClick = buscarDadosFechamentoCancelamentoPagamento;
    Etapa1Aprovada();
}

//*******Etapa 3*******

function Etapa3Desabilitada() {
    $("#" + _etapaCancelamentoPagamento.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaCancelamentoPagamento.Etapa3.idTab + " .step").attr("class", "step");
    _etapaCancelamentoPagamento.Etapa3.eventClick = function () { };
}
function Etapa3Aguardando() {
    $("#" + _etapaCancelamentoPagamento.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamentoPagamento.Etapa3.idTab + " .step").attr("class", "step yellow");
    _etapaCancelamentoPagamento.Etapa3.eventClick = CarregaIntegracao;
    Etapa2Aprovada();
}

function Etapa3Reprovada() {
    $("#" + _etapaCancelamentoPagamento.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamentoPagamento.Etapa3.idTab + " .step").attr("class", "step red");
    _etapaCancelamentoPagamento.Etapa3.eventClick = CarregaIntegracao;
    Etapa2Aprovada();
}

function Etapa3Aprovada() {
    $("#" + _etapaCancelamentoPagamento.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamentoPagamento.Etapa3.idTab + " .step").attr("class", "step green");
    _etapaCancelamentoPagamento.Etapa3.eventClick = CarregaIntegracao;
    Etapa2Aprovada();
}