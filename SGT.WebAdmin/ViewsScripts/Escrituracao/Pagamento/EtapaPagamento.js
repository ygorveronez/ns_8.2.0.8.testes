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
/// <reference path="../../Enumeradores/EnumSituacaoPagamento.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _etapaPagamento;

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
    _etapaPagamento = new EtapaPagamento();
    KoBindings(_etapaPagamento, "knockoutEtapaPagamento");
    Etapa1Liberada();
}

function EtapaSemRegra() {
    exibirMensagem(tipoMensagem.aviso, "Regras da etapa", "Nenhuma regra encontrada. A solicitação permanecerá aguardando autorização.");
}

function SetarEtapaInicio() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaPagamento.Etapa1.idTab).click();
    $("#" + _etapaPagamento.Etapa1.idTab).tab("show");
}

function SetarEtapasPagamento() {
    var situacao = _pagamento.Situacao.val();

    switch (situacao) {
        case EnumSituacaoPagamento.Todas:
            Etapa1Liberada();
            break;

        case EnumSituacaoPagamento.EmFechamento:
        case EnumSituacaoPagamento.AguardandoAprovacao:
            Etapa2Aguardando();
            break;

        case EnumSituacaoPagamento.PendenciaFechamento:
        case EnumSituacaoPagamento.Reprovado:
            Etapa2Reprovada();
            break;

        case EnumSituacaoPagamento.AguardandoIntegracao:
        case EnumSituacaoPagamento.EmIntegracao:
            Etapa3Aguardando();
            break;

        case EnumSituacaoPagamento.Finalizado:
            Etapa3Aprovada();
            break;

        case EnumSituacaoPagamento.FalhaIntegracao:
            Etapa3Reprovada();
            break;

        case EnumSituacaoPagamento.SemRegraAprovacao:
            Etapa2SemRegraAprovacao();
            break;

        case EnumSituacaoPagamento.Cancelado:
            EtapaCancelada();
            break;
    }
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaPagamento.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaPagamento.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaPagamento.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaPagamento.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    $("#" + _etapaPagamento.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaPagamento.Etapa2.idTab + " .step").attr("class", "step");
    _etapaPagamento.Etapa2.eventClick = function () { };
    Etapa3Desabilitada();
}

function Etapa2Aguardando() {
    $("#" + _etapaPagamento.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaPagamento.Etapa2.idTab + " .step").attr("class", "step yellow");
    _etapaPagamento.Etapa2.eventClick = buscarDadosFechamentoPagamento;
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    $("#" + _etapaPagamento.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaPagamento.Etapa2.idTab + " .step").attr("class", "step red");
    _etapaPagamento.Etapa2.eventClick = buscarDadosFechamentoPagamento;
    Etapa1Aprovada();
}

function Etapa2Aprovada() {
    $("#" + _etapaPagamento.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaPagamento.Etapa2.idTab + " .step").attr("class", "step green");
    _etapaPagamento.Etapa2.eventClick = buscarDadosFechamentoPagamento;
    Etapa1Aprovada();
}

function Etapa2SemRegraAprovacao() {
    $("#" + _etapaPagamento.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaPagamento.Etapa2.idTab + " .step").attr("class", "step red");
    _etapaPagamento.Etapa2.eventClick = buscarDadosFechamentoPagamento;
    Etapa1Aprovada();
    EtapaSemRegra();
}

//*******Etapa 3*******

function Etapa3Desabilitada() {
    $("#" + _etapaPagamento.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaPagamento.Etapa3.idTab + " .step").attr("class", "step");
    _etapaPagamento.Etapa3.eventClick = function () { };
}

function Etapa3Aguardando() {
    $("#" + _etapaPagamento.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaPagamento.Etapa3.idTab + " .step").attr("class", "step yellow");
    _etapaPagamento.Etapa3.eventClick = CarregaIntegracao;
    Etapa2Aprovada();
    ocultarBotoesFechamento();
}

function Etapa3Reprovada() {
    $("#" + _etapaPagamento.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaPagamento.Etapa3.idTab + " .step").attr("class", "step red");
    _etapaPagamento.Etapa3.eventClick = CarregaIntegracao;
    Etapa2Aprovada();
    ocultarBotoesFechamento();
}

function Etapa3Aprovada() {
    $("#" + _etapaPagamento.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaPagamento.Etapa3.idTab + " .step").attr("class", "step green");
    _etapaPagamento.Etapa3.eventClick = CarregaIntegracao;
    Etapa2Aprovada();
    ocultarBotoesFechamento();
}

function EtapaCancelada() {
    _etapaPagamento.Etapa3.eventClick = CarregaIntegracao;
    $("#" + _etapaPagamento.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaPagamento.Etapa3.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
    Etapa2Aprovada();
}