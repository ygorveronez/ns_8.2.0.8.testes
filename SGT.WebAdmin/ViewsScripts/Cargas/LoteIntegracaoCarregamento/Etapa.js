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
/// <reference path="loteintegracaocarregamento.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _etapaLoteIntegracao;

var EtapaLoteIntegracao = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("50%") });

    this.Etapa1 = PropertyEntity({
        text: "Carregamento", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("É onde se informar os carregamentos para integração."),
        tooltipTitle: ko.observable("Pedido")
    });
    this.Etapa2 = PropertyEntity({
        text: "Integração", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("Integração lote carregamento."),
        tooltipTitle: ko.observable("Aprovação")
    });
}


//*******EVENTOS*******

function loadEtapaLoteIntegracao() {
    _etapaLoteIntegracao = new EtapaLoteIntegracao();
    KoBindings(_etapaLoteIntegracao, "knockoutEtapaLoteIntegracao");
    Etapa1Liberada();
}

function setarEtapaInicio() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaLoteIntegracao.Etapa1.idTab).click();
}

function setarEtapaIntegracao() {
    DesabilitarTodasEtapas();
    Etapa2Liberada();
    $("#" + _etapaLoteIntegracao.Etapa2.idTab).click();
}

function setarEtapas(situacao) {
    //var situacaoPedido = _pedido.SituacaoPedido.val();

    //if (situacao == "") {
    //    Etapa2Aguardando();
    //}
    //else if (situacaoPedido == EnumSituacaoPedido.AutorizacaoPendente) {
    //    Etapa2Aguardando();
    //}
    //else if (situacaoPedido == EnumSituacaoPedido.Rejeitado) {
    //    Etapa2Reprovada();
    //}
    //else if (situacaoPedido == EnumSituacaoPedido.Aberto) {
    //    Etapa2Aprovada();
    //}
    //else if (situacaoPedido == EnumSituacaoPedido.Aberto) {
    //    Etapa2Aprovada();
    //}
    //else
    //    Etapa1Aguardando();
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaLoteIntegracao.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLoteIntegracao.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aguardando() {
    $("#" + _etapaLoteIntegracao.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLoteIntegracao.Etapa1.idTab + " .step").attr("class", "step yellow");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    $("#" + _etapaLoteIntegracao.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaLoteIntegracao.Etapa2.idTab + " .step").attr("class", "step");
}

function Etapa2Liberada() {
    $("#" + _etapaLoteIntegracao.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLoteIntegracao.Etapa2.idTab + " .step").attr("class", "step yellow");
}

function Etapa2Reprovada() {
    $("#" + _etapaLoteIntegracao.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLoteIntegracao.Etapa2.idTab + " .step").attr("class", "step red");
}
