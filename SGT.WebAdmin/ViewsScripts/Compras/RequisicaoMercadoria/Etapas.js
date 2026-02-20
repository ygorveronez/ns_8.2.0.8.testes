/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _etapaRequisicao;

var EtapaRequisicao = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("50%") });

    this.Etapa1 = PropertyEntity({
        text: "Mercadorias", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("É onde informa os dados da requisição."),
        tooltipTitle: ko.observable("Dados")
    });
    this.Etapa2 = PropertyEntity({
        text: "Aprovação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("Nessa etapa é possível acompanhar a aprovação da requisição."),
        tooltipTitle: ko.observable("Aprovação")
    });
}


//*******EVENTOS*******

function LoadEtapasRequisicao() {
    _etapaRequisicao = new EtapaRequisicao();
    KoBindings(_etapaRequisicao, "knockoutEtapasRequisicao");
    Etapa1Liberada();
    SetarEtapaInicioRequisicao();
}

function SetarEtapaInicioRequisicao() {
    DesabilitarTodasEtapasRequisicao();
    Etapa1Liberada();
    Global.ResetarSteps();
}

function SetarEtapasRequisicao() {
    var situacao = _requisicaoMercadoria.Situacao.val();

    if (situacao == EnumSituacaoRequisicaoMercadoria.AgAprovacao)
        Etapa2Aguardando();
    else if (situacao == EnumSituacaoRequisicaoMercadoria.Aprovada)
        Etapa2Aprovada();
    else if (situacao == EnumSituacaoRequisicaoMercadoria.Rejeitada)
        Etapa2Reprovada();
    else if (situacao == EnumSituacaoRequisicaoMercadoria.SemRegra)
        Etapa2Aguardando();
}

function DesabilitarTodasEtapasRequisicao() {
    Etapa2Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaRequisicao.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaRequisicao.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaRequisicao.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaRequisicao.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    $("#" + _etapaRequisicao.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaRequisicao.Etapa2.idTab + " .step").attr("class", "step");
}

function Etapa2Aprovada() {
    $("#" + _etapaRequisicao.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaRequisicao.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    $("#" + _etapaRequisicao.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaRequisicao.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
}

function Etapa2Aguardando() {
    $("#" + _etapaRequisicao.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaRequisicao.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
}

