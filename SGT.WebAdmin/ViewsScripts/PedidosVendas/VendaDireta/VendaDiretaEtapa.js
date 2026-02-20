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
/// <reference path="../../Enumeradores/EnumStatusVendaDireta.js" />
/// <reference path="VendaDireta.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _etapaVendaDireta;

var EtapaVendaDireta = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("50%") });

    this.Etapa1 = PropertyEntity({
        text: "Venda", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("É onde informa os dados da venda."),
        tooltipTitle: ko.observable("Dados")
    });
    this.Etapa2 = PropertyEntity({
        text: "Boleto", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("Nessa etapa é possível acompanhar a geração dos boletos da venda."),
        tooltipTitle: ko.observable("Aprovação")
    });
}

//*******EVENTOS*******

function LoadEtapasVendaDireta() {
    _etapaVendaDireta = new EtapaVendaDireta();
    KoBindings(_etapaVendaDireta, "knockoutEtapaVendaDireta");
    Etapa1Liberada();
    SetarEtapaInicioVendaDireta();
}

function SetarEtapaInicioVendaDireta() {
    DesabilitarTodasEtapasVendaDireta();
    Etapa1Liberada();
    $("#" + _etapaVendaDireta.Etapa1.idTab).click();
}

function SetarEtapasVendaDireta() {
    var situacao = _vendaDireta.Status.val();

    if (situacao == EnumStatusVendaDireta.Finalizado)
        Etapa2Aprovada();
    else if (situacao == EnumStatusVendaDireta.Cancelado)
        Etapa2Reprovada();
}

function DesabilitarTodasEtapasVendaDireta() {
    Etapa2Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaVendaDireta.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaVendaDireta.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaVendaDireta.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaVendaDireta.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    $("#" + _etapaVendaDireta.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaVendaDireta.Etapa2.idTab + " .step").attr("class", "step");
}

function Etapa2Aprovada() {
    $("#" + _etapaVendaDireta.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaVendaDireta.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    $("#" + _etapaVendaDireta.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaVendaDireta.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
}