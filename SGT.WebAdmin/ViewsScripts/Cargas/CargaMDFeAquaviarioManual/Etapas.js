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
/// <reference path="Cargas.js" />
/// <reference path="CTes.js" />
/// <reference path="Impressao.js" />
/// <reference path="MDFe.js" />
/// <reference path="SignalR.js" />
/// <reference path="Terminais.js" />
/// <reference path="CargaMDFeAquaviarioManual.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _etapa;
var _integracaoMDFeManual = false;

var Etapas = function () {
    this.Etapa1 = PropertyEntity({
        text: "Dados MDF-e", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("Esta etapa é onde os dados do MDF-e são inclusos."),
        tooltipTitle: ko.observable("Abertura")
    });
    this.Etapa2 = PropertyEntity({
        text: "Emissão", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("É onde ocorre a emissão do MDF-e."),
        tooltipTitle: ko.observable("Análise")
    });
    this.Etapa3 = PropertyEntity({
        text: "Impressão", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable("É utilizado para controle de impressão dos documentos"),
        tooltipTitle: ko.observable("Ocorrência")
    });
    this.Etapa4 = PropertyEntity({
        text: "Integração", type: types.local, enable: ko.observable(true), visible: ko.observable(_integracaoMDFeManual), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(4),
        tooltip: ko.observable("Quando configurado, será gerado integrações"),
        tooltipTitle: ko.observable("Integração")
    });

    var etapas = 0;
    for (var i in this) if (/(Etapa)[0-9]+/.test(i)) etapas++;

    if (!_integracaoMDFeManual)
        etapas -= 1;

    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable((100 / etapas).toFixed(2) + "%") });
}


//*******EVENTOS*******

function loadEtapasMDFe() {
    _etapa = new Etapas();
    KoBindings(_etapa, "knockoutEtapaMDFe");
    Etapa1Liberada();
}

function SetarEtapaInicio() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapa.Etapa1.idTab).click();
}

function SetarEtapaMDFe() {
    var situacao = _cargaMDFeAquaviario.Situacao.val();

    if (situacao == EnumSituacaoMDFeManual.Cancelado)
        situacao = _cargaMDFeAquaviario.SituacaoCancelamento.val();

    if (situacao == EnumSituacaoMDFeManual.EmDigitacao)
        Etapa1Liberada();
    else if (situacao == EnumSituacaoMDFeManual.EmEmissao)
        Etapa2Aguardando();
    else if (situacao == EnumSituacaoMDFeManual.Rejeicao)
        Etapa2Reprovada();
    else if (situacao == EnumSituacaoMDFeManual.AgImpressao)
        Etapa3Liberada();
    else if (situacao == EnumSituacaoMDFeManual.Finalizado) {
        Etapa3Aprovada();
        Etapa4Aprovada();
    }
    else if (situacao === EnumSituacaoMDFeManual.AgIntegracao)
        Etapa4Aguardando();
    else if (situacao === EnumSituacaoMDFeManual.FalhaIntegracao)
        Etapa4Reprovada();
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
    Etapa3Desabilitada();
    Etapa4Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapa.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapa.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
    $("#" + _etapa.Etapa1.idTab).click();
}

function Etapa1Aprovada() {
    $("#" + _etapa.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapa.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    $("#" + _etapa.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapa.Etapa2.idTab + " .step").attr("class", "step");
    _etapa.Etapa2.eventClick = function () { };
    Etapa3Desabilitada();
}

function Etapa2Aprovada() {
    $("#" + _etapa.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapa.Etapa2.idTab + " .step").attr("class", "step green");
    _etapa.Etapa2.eventClick = buscarMDFeClick;
    Etapa1Aprovada();
}

function Etapa2Liberada() {
    $("#" + _etapa.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapa.Etapa2.idTab + " .step").attr("class", "step yellow");
    $("#" + _etapa.Etapa2.idTab).click();
    _etapa.Etapa2.eventClick = buscarMDFeClick;
    Etapa1Aprovada();
    $("#" + _etapa.Etapa2.idTab).click();
}


function Etapa2Reprovada() {
    $("#" + _etapa.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapa.Etapa2.idTab + " .step").attr("class", "step red");
    _etapa.Etapa2.eventClick = buscarMDFeClick;
    Etapa1Aprovada();
    $("#" + _etapa.Etapa2.idTab).click();
}

function Etapa2Aguardando() {
    $("#" + _etapa.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapa.Etapa2.idTab + " .step").attr("class", "step yellow");
    _etapa.Etapa2.eventClick = buscarMDFeClick;
    Etapa1Aprovada();
    $("#" + _etapa.Etapa2.idTab).click();
}


//*******Etapa 3*******

function Etapa3Desabilitada() {
    $("#" + _etapa.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapa.Etapa3.idTab + " .step").attr("class", "step");
    _etapa.Etapa3.eventClick = function () { };
    Etapa4Desabilitada();
}

function Etapa3Aguardando() {
    $("#" + _etapa.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapa.Etapa3.idTab + " .step").attr("class", "step yellow");
    Etapa2Aprovada();
    _etapa.Etapa3.eventClick = verificarEtapaImpressaoClick;
    $("#" + _etapa.Etapa3.idTab).click();
}

function Etapa3Aprovada() {
    $("#" + _etapa.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapa.Etapa3.idTab + " .step").attr("class", "step green");
    _etapa.Etapa3.eventClick = verificarEtapaImpressaoClick;
    Etapa2Aprovada();
    $("#" + _etapa.Etapa3.idTab).click();
}

function Etapa3Reprovada() {
    $("#" + _etapa.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapa.Etapa3.idTab + " .step").attr("class", "step red");
    _etapa.Etapa3.eventClick = verificarEtapaImpressaoClick;
    Etapa2Aprovada();
    $("#" + _etapa.Etapa3.idTab).click();
}

function Etapa3Liberada() {
    $("#" + _etapa.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapa.Etapa3.idTab + " .step").attr("class", "step blue");
    _etapa.Etapa3.eventClick = verificarEtapaImpressaoClick;
    Etapa2Aprovada();
    $("#" + _etapa.Etapa3.idTab).click();
}

//*******Etapa 4*******

function Etapa4Desabilitada() {
    _etapa.Etapa4.eventClick = function () { };
    $("#" + _etapa.Etapa4.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapa.Etapa4.idTab + " .step").attr("class", "step");
}

function Etapa4Liberada() {
    _etapa.Etapa4.eventClick = recarregarMDFeAquaviarioManualIntegracoes;
    $("#" + _etapa.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapa.Etapa4.idTab + " .step").attr("class", "step yellow");
    Etapa3Aprovada();
}

function Etapa4Aguardando() {
    _etapa.Etapa4.eventClick = recarregarMDFeAquaviarioManualIntegracoes;
    $("#" + _etapa.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapa.Etapa4.idTab + " .step").attr("class", "step yellow");
    $("#" + _etapa.Etapa4.idTab).click();
    Etapa3Aprovada();
}

function Etapa4Aprovada() {
    _etapa.Etapa4.eventClick = recarregarMDFeAquaviarioManualIntegracoes;
    $("#" + _etapa.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapa.Etapa4.idTab + " .step").attr("class", "step green");
    $("#" + _etapa.Etapa4.idTab).click();
    Etapa3Aprovada();
}

function Etapa4Reprovada() {
    _etapa.Etapa4.eventClick = recarregarMDFeAquaviarioManualIntegracoes;
    $("#" + _etapa.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapa.Etapa4.idTab + " .step").attr("class", "step red");
    $("#" + _etapa.Etapa4.idTab).click();
    Etapa3Aprovada();
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