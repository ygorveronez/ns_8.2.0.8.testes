/// <reference path="MDFe.js" />
/// <reference path="CTe.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCancelamentoDocumentoCarga.js" />
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
/// <reference path="../../Enumeradores/EnumTipoCancelamentoCarga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _etapaCancelamento;
var _integracaoMDFeManual = false;

var EtapaCancelamento = function () {
    //this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("50%") });

    this.Etapa1 = PropertyEntity({
        text: "Cancelamento", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("É onde se gera o cancelamento."),
        tooltipTitle: ko.observable("Cancelamento")
    });
    this.Etapa2 = PropertyEntity({
        text: "MDF-e", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("É a etapa de cancelamento dos MDF-es."),
        tooltipTitle: ko.observable("MDF-e")
    });
    this.Etapa3 = PropertyEntity({
        text: "Integração", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable("Quando configurado, será gerado integrações"),
        tooltipTitle: ko.observable("Integração")
    });

    var etapas = 0;
    for (var i in this) if (/(Etapa)[0-9]+/.test(i)) etapas++;
    /*
    if (!_integracaoMDFeManual)
        etapas -= 1;
        */
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable((100 / etapas).toFixed(2) + "%") });
}

//*******EVENTOS*******

function LoadEtapaCancelamento() {
    _etapaCancelamento = new EtapaCancelamento();
    KoBindings(_etapaCancelamento, "knockoutEtapaCancelamento");
    Etapa1Liberada();
}

function SetarEtapaInicioCancelamento() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaCancelamento.Etapa1.idTab).click();
}

function SetarEtapaCancelamento() {
    var situacaoCancelamento = _cancelamento.Situacao.val();

    if (situacaoCancelamento == EnumSituacaoMDFeManualCancelamento.EmCancelamento)
        Etapa2Aguardando();
    else if (situacaoCancelamento == EnumSituacaoMDFeManualCancelamento.Cancelada) {
        Etapa2Aprovada();
        Etapa3Aprovada();
    }
    else if (situacaoCancelamento == EnumSituacaoMDFeManualCancelamento.CancelamentoRejeitado)
        Etapa2Reprovada();
    else if (situacaoCancelamento == EnumSituacaoMDFeManualCancelamento.AgIntegracoes)
        Etapa3Aguardando();
    else if (situacaoCancelamento == EnumSituacaoMDFeManualCancelamento.FalhaIntegracao)
        Etapa3Reprovada();
    else {
        SetarEtapaInicioCancelamento();
    }
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
    Etapa3Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaCancelamento.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
    Etapa3Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaCancelamento.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    _etapaCancelamento.Etapa2.eventClick = function () { };
    $("#" + _etapaCancelamento.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaCancelamento.Etapa2.idTab + " .step").attr("class", "step");
    $("#" + _etapaCancelamento.Etapa1.idTab).click();
}

function Etapa2Liberada() {
    _etapaCancelamento.Etapa2.eventClick = ConsultarMDFesCarga;
    $("#" + _etapaCancelamento.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
}

function Etapa2Aguardando() {
    _etapaCancelamento.Etapa2.eventClick = ConsultarMDFesCarga;
    $("#" + _etapaCancelamento.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
    $("#" + _etapaCancelamento.Etapa2.idTab).click();
}

function Etapa2Aprovada() {
    _etapaCancelamento.Etapa2.eventClick = ConsultarMDFesCarga;
    $("#" + _etapaCancelamento.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    _etapaCancelamento.Etapa2.eventClick = ConsultarMDFesCarga;
    $("#" + _etapaCancelamento.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
}

//*******Etapa 3*******

function Etapa3Desabilitada() {
    _etapaCancelamento.Etapa3.eventClick = function () { };
    $("#" + _etapaCancelamento.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaCancelamento.Etapa3.idTab + " .step").attr("class", "step");
    $("#" + _etapaCancelamento.Etapa1.idTab).click();
}

function Etapa3Liberada() {
    _etapaCancelamento.Etapa3.eventClick = carregarIntegracoesMDFeManual;
    $("#" + _etapaCancelamento.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa3.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
    Etapa2Aprovada();
}

function Etapa3Aguardando() {
    _etapaCancelamento.Etapa3.eventClick = carregarIntegracoesMDFeManual;
    $("#" + _etapaCancelamento.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa3.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
    Etapa2Aprovada();
    $("#" + _etapaCancelamento.Etapa3.idTab).click();
}

function Etapa3Aprovada() {
    _etapaCancelamento.Etapa3.eventClick = carregarIntegracoesMDFeManual;
    $("#" + _etapaCancelamento.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa3.idTab + " .step").attr("class", "step green");
    Etapa1Aprovada();
    Etapa2Aprovada();
}

function Etapa3Reprovada() {
    _etapaCancelamento.Etapa3.eventClick = carregarIntegracoesMDFeManual;
    $("#" + _etapaCancelamento.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa3.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
    Etapa2Aprovada();
}

// ** CONSULTA INTEGRAÇÃO MDFE AQUAVIARIO ** //

function ConsultarIntegracaoMDFeAquaviarioManual() {
    var p = new promise.Promise();

    executarReST("ConfiguracaoIntercab/ConsultarIntegracaoMDFeAquaviarioManual", {}, function (r) {
        if (r.Success && r.Data) {
            _integracaoMDFeManual = true;
        }
        p.done();
    });

    return p;
}