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
/// <reference path="../../Enumeradores/EnumSituacaoDescarteLoteProdutoEmbarcador.js" />
/// <reference path="DescarteLoteProduto.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _etapaDescarteLote;

var EtapaDescarteLote = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("50%") });

    this.Etapa1 = PropertyEntity({
        text: "Descarte", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("Esta etapa é destinada ao preenchimento de dados do descarte."),
        tooltipTitle: ko.observable("Descarte")
    });
    this.Etapa2 = PropertyEntity({
        text: "Aprovacao", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("Lista de aprovadores."),
        tooltipTitle: ko.observable("Aprovacao")
    });
}


//*******EVENTOS*******

function loadEtapaDescarteLote() {
    _etapaDescarteLote = new EtapaDescarteLote();
    KoBindings(_etapaDescarteLote, "knockoutEtapaDescarteLote");
    Etapa1Liberada();
}

function SetarEtapaInicioLancamento() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaDescarteLote.Etapa1.idTab).click();
    $("#" + _etapaDescarteLote.Etapa1.idTab).tab("show");
}

function SetarEtapaDescarteLote() {
    var situacao = _descarte.Situacao.val();
    
    if (situacao == EnumSituacaoDescarteLoteProdutoEmbarcador.AgAprovacao)
        Etapa2Aguardando();
    else if (situacao == EnumSituacaoDescarteLoteProdutoEmbarcador.Finalizado)
        Etapa2Aprovada();
    else if (situacao == EnumSituacaoDescarteLoteProdutoEmbarcador.Rejeitada)
        Etapa2Reprovada();
    else if (situacao == EnumSituacaoDescarteLoteProdutoEmbarcador.SemRegra)
        Etapa2Reprovada();
    else
        Etapa1Liberada();
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaDescarteLote.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaDescarteLote.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaDescarteLote.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaDescarteLote.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    $("#" + _etapaDescarteLote.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaDescarteLote.Etapa2.idTab + " .step").attr("class", "step");
}

function Etapa2Aprovada() {
    $("#" + _etapaDescarteLote.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaDescarteLote.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1Aprovada();
}

function Etapa2Aguardando() {
    $("#" + _etapaDescarteLote.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaDescarteLote.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
    $("#" + _etapaDescarteLote.Etapa2.idTab).click();
    $("#" + _etapaDescarteLote.Etapa2.idTab).tab("show");
}

function Etapa2Reprovada() {
    $("#" + _etapaDescarteLote.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaDescarteLote.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
}