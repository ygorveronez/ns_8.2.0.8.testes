/// <reference path="../../Enumeradores/EnumSituacaoImportacaoProgramacaoColeta.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _etapaImportacaoProgramacaoColeta;

var EtapaImportacaoProgramacaoColeta = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("50%") });

    this.Etapa1 = PropertyEntity({
        text: "Dados importação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("É onde se inicia o cadastro."),
        tooltipTitle: ko.observable("Dados importação")
    });
    this.Etapa2 = PropertyEntity({
        text: "Importação Pedidos", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("É a etapa de importação."),
        tooltipTitle: ko.observable("Importação Pedidos")
    });
};

//*******EVENTOS*******

function LoadEtapaImportacaoProgramacaoColeta() {
    _etapaImportacaoProgramacaoColeta = new EtapaImportacaoProgramacaoColeta();
    KoBindings(_etapaImportacaoProgramacaoColeta, "knockoutEtapaImportacaoProgramacaoColeta");

    Etapa1Liberada();
}

function SetarEtapaInicioImportacaoProgramacaoColeta() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaImportacaoProgramacaoColeta.Etapa1.idTab).click();
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
}

function SetarEtapaImportacaoProgramacaoColeta() {
    var situacao = _importacaoProgramacaoColeta.Situacao.val();

    if (situacao === EnumSituacaoImportacaoProgramacaoColeta.EmCriacao)
        Etapa2Aguardando();
    else if (situacao === EnumSituacaoImportacaoProgramacaoColeta.EmAndamento || situacao === EnumSituacaoImportacaoProgramacaoColeta.Finalizado)
        Etapa2Aprovada();
    else if (situacao === EnumSituacaoImportacaoProgramacaoColeta.FalhaNaGeracao)
        Etapa2Reprovada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaImportacaoProgramacaoColeta.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaImportacaoProgramacaoColeta.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaImportacaoProgramacaoColeta.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaImportacaoProgramacaoColeta.Etapa1.idTab + " .step").attr("class", "step green");
}

function Etapa1Desabilitada() {
    $("#" + _etapaImportacaoProgramacaoColeta.Etapa1.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaImportacaoProgramacaoColeta.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    $("#" + _etapaImportacaoProgramacaoColeta.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaImportacaoProgramacaoColeta.Etapa2.idTab + " .step").attr("class", "step");
}

function Etapa2Liberada() {
    $("#" + _etapaImportacaoProgramacaoColeta.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaImportacaoProgramacaoColeta.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
}

function Etapa2Aguardando() {
    $("#" + _etapaImportacaoProgramacaoColeta.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaImportacaoProgramacaoColeta.Etapa2.idTab + " .step").attr("class", "step yellow");
    $("#" + _etapaImportacaoProgramacaoColeta.Etapa2.idTab).click();
    Etapa1Aprovada();
}

function Etapa2Aprovada() {
    $("#" + _etapaImportacaoProgramacaoColeta.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaImportacaoProgramacaoColeta.Etapa2.idTab + " .step").attr("class", "step green");
    $("#" + _etapaImportacaoProgramacaoColeta.Etapa2.idTab).click();
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    $("#" + _etapaImportacaoProgramacaoColeta.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaImportacaoProgramacaoColeta.Etapa2.idTab + " .step").attr("class", "step red");
    $("#" + _etapaImportacaoProgramacaoColeta.Etapa2.idTab).click();
    Etapa1Aprovada();
}