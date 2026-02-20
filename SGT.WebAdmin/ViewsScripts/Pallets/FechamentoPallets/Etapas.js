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
/// <reference path="../../Enumeradores/EnumSituacaoFechamentoFrete.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _etapaFechamento;

var EtapaFechamento = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("33%") });

    this.Etapa1 = PropertyEntity({
        text: "Dados", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("É onde informa os dados do fechamento."),
        tooltipTitle: ko.observable("Dados")
    });
    this.Etapa2 = PropertyEntity({
        text: "Composição do Fechamento", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("É onde se altera as informações que compõe o fechamento."),
        tooltipTitle: ko.observable("Composição do Fechamento")
    });
    this.Etapa3 = PropertyEntity({
        text: "Finalizar", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable("Etapa onde se finaliza o fechamento."),
        tooltipTitle: ko.observable("Finalizar")
    });
}


//*******EVENTOS*******

function LoadEtapasFechamento() {
    _etapaFechamento = new EtapaFechamento();
    KoBindings(_etapaFechamento, "knockoutEtapasFechamento");
    Etapa1FechamentoLiberada();
    SetarEtapaInicioFechamento();
}

function SetarEtapaInicioFechamento() {
    DesabilitarTodasEtapasFechamento();
    Etapa1FechamentoLiberada();
    $("#" + _etapaFechamento.Etapa1.idTab).click();
}

function SetarEtapasFechamento() {
    var situacao = _fechamentoPallets.Situacao.val();

    if (situacao == EnumSituacaoFechamentoPallets.Aberto)
        Etapa3FechamentoLiberada();
    //else if (situacao == EnumSituacaoFechamentoFrete.Cancelado)
    //    Etapa2FechamentoLiberada();
    //else if (situacao == EnumSituacaoFechamentoFrete.EmEmissaoComplemento)
    //    Etapa3FechamentoAguardando();
    else if (situacao == EnumSituacaoFechamentoPallets.Finalizado)
        Etapa3FechamentoAprovada();
}

function DesabilitarTodasEtapasFechamento() {
    Etapa2FechamentoDesabilitada();
    Etapa3FechamentoDesabilitada();
}

//*******Etapa 1*******

function Etapa1FechamentoLiberada() {
    $("#" + _etapaFechamento.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamento.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2FechamentoDesabilitada();
}

function Etapa1FechamentoAprovada() {
    $("#" + _etapaFechamento.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamento.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2FechamentoDesabilitada() {
    $("#" + _etapaFechamento.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaFechamento.Etapa2.idTab + " .step").attr("class", "step");
    Etapa3FechamentoDesabilitada();
}

function Etapa2FechamentoAprovada() {
    $("#" + _etapaFechamento.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamento.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1FechamentoAprovada();
}

function Etapa2FechamentoReprovada() {
    $("#" + _etapaFechamento.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamento.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1FechamentoAprovada();
}

function Etapa2FechamentoLiberada() {
    $("#" + _etapaFechamento.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamento.Etapa2.idTab + " .step").attr("class", "step yellow");
    //$("#" + _etapaFechamento.Etapa2.idTab).click();
    Etapa1FechamentoAprovada();
}


//*******Etapa 3*******

function Etapa3FechamentoDesabilitada() {
    $("#" + _etapaFechamento.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaFechamento.Etapa3.idTab + " .step").attr("class", "step");
}

function Etapa3FechamentoAguardando() {
    $("#" + _etapaFechamento.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamento.Etapa3.idTab + " .step").attr("class", "step yellow");
    Etapa2FechamentoAprovada();
}


function Etapa3FechamentoAprovada() {
    $("#" + _etapaFechamento.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamento.Etapa3.idTab + " .step").attr("class", "step green");
    Etapa2FechamentoAprovada();
}

function Etapa3FechamentoReprovada() {
    $("#" + _etapaFechamento.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamento.Etapa3.idTab + " .step").attr("class", "step red");
    Etapa2FechamentoAprovada();
}

function Etapa3FechamentoLiberada() {
    $("#" + _etapaFechamento.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamento.Etapa3.idTab + " .step").attr("class", "step yellow");
    setTimeout(function () {
        $("#" + _etapaFechamento.Etapa2.idTab).click();
    }, 100);

    Etapa2FechamentoAprovada();
}