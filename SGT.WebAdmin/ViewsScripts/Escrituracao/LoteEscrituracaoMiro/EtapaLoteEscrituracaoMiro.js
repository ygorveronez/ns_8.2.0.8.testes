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
/// <reference path="../../Enumeradores/EnumSituacaoLoteEscrituracao.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _etapaLoteEscrituracaoMiro;

var EtapaLoteEscrituracaoMiro = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("50%") });

    this.Etapa1 = PropertyEntity({
        text: "Documentos", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("Esta etapa é destinada à seleção se documentos."),
        tooltipTitle: ko.observable("Documentos")
    });
    this.Etapa2 = PropertyEntity({
        text: "Integração", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("Caso tenha integração, nessa etapa é possível acompanhar."),
        tooltipTitle: ko.observable("Integração")
    });
}


//*******EVENTOS*******

function loadEtapasLoteEscrituracaoMiro() {
    _etapaLoteEscrituracaoMiro = new EtapaLoteEscrituracaoMiro();
    KoBindings(_etapaLoteEscrituracaoMiro, "knockoutEtapaLoteEscrituracaoMiro");
    Etapa1Liberada();
}

function SetarEtapaInicio() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaLoteEscrituracaoMiro.Etapa1.idTab).click();
}

function SetarEtapasLoteEscrituracao() {
    var situacao = _loteEscrituracao.Situacao.val();

    //if (situacao == EnumSituacaoLoteEscrituracao.Cancelada)
        //situacao = _etapaLoteEscrituracaoMiro.SituacaoNoCancelamento.val();

    if (situacao == EnumSituacaoLoteEscrituracao.Todas)
        Etapa1Liberada();
    else if (situacao == EnumSituacaoLoteEscrituracao.EmCriacao)
        Etapa1Liberada();
    else if (situacao == EnumSituacaoLoteEscrituracao.AgIntegracao)
        Etapa2Aguardando();
    else if (situacao == EnumSituacaoLoteEscrituracao.Finalizado)
        Etapa2Aprovada();
    else if (situacao == EnumSituacaoLoteEscrituracao.FalhaIntegracao)
        Etapa2Reprovada();
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaLoteEscrituracaoMiro.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLoteEscrituracaoMiro.Etapa1.idTab + " .step").attr("class", "step yellow");
    //Etapa2Desabilitada();
    Etapa2Aguardando();
}

function Etapa1Aprovada() {
    $("#" + _etapaLoteEscrituracaoMiro.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLoteEscrituracaoMiro.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    $("#" + _etapaLoteEscrituracaoMiro.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaLoteEscrituracaoMiro.Etapa2.idTab + " .step").attr("class", "step");
    _etapaLoteEscrituracaoMiro.Etapa2.eventClick = function () { };
}
function Etapa2Aguardando() {
    $("#" + _etapaLoteEscrituracaoMiro.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLoteEscrituracaoMiro.Etapa2.idTab + " .step").attr("class", "step yellow");
    _etapaLoteEscrituracaoMiro.Etapa2.eventClick = BuscarDadosIntegracoesLoteEscrituracaoMiro;
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    $("#" + _etapaLoteEscrituracaoMiro.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLoteEscrituracaoMiro.Etapa2.idTab + " .step").attr("class", "step red");
    _etapaLoteEscrituracaoMiro.Etapa2.eventClick = BuscarDadosIntegracoesLoteEscrituracaoMiro;
    Etapa1Aprovada();
}

function Etapa2Aprovada() {
    $("#" + _etapaLoteEscrituracaoMiro.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLoteEscrituracaoMiro.Etapa2.idTab + " .step").attr("class", "step green");
    _etapaLoteEscrituracaoMiro.Etapa2.eventClick = BuscarDadosIntegracoesLoteEscrituracaoMiro;
    Etapa1Aprovada();
}