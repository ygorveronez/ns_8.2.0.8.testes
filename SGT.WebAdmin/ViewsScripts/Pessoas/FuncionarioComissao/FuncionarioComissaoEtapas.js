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
/// <reference path="../../Enumeradores/EnumSituacaoFuncionarioComissao.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _etapaFuncionarioComissao;

var EtapaFuncionarioComissao = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("50%") });

    this.Etapa1 = PropertyEntity({
        text: "Títulos", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("É onde informa os dados da comissão."),
        tooltipTitle: ko.observable("Dados")
    });
    this.Etapa2 = PropertyEntity({
        text: "Aprovação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("Nessa etapa é possível acompanhar a aprovação da comissão."),
        tooltipTitle: ko.observable("Aprovação")
    });
}

//*******EVENTOS*******

function LoadEtapasFuncionarioComissao() {
    _etapaFuncionarioComissao = new EtapaFuncionarioComissao();
    KoBindings(_etapaFuncionarioComissao, "knockoutEtapasFuncionarioComissao");
    Etapa1Liberada();
    SetarEtapaInicioFuncionarioComissao();
}

function SetarEtapaInicioFuncionarioComissao() {
    DesabilitarTodasEtapasFuncionarioComissao();
    Etapa1Liberada();
    $("#" + _etapaFuncionarioComissao.Etapa1.idTab).click();
}

function SetarEtapasFuncionarioComissao() {
    var situacao = _funcionarioComissao.Situacao.val();

    if (situacao == EnumSituacaoFuncionarioComissao.AgAprovacao)
        Etapa2Aguardando();
    else if (situacao == EnumSituacaoFuncionarioComissao.Aprovada)
        Etapa2Aprovada();
    else if (situacao == EnumSituacaoFuncionarioComissao.Rejeitada || situacao == EnumSituacaoFuncionarioComissao.Cancelado)
        Etapa2Reprovada();
    else if (situacao == EnumSituacaoFuncionarioComissao.SemRegra)
        Etapa2Aguardando();
}

function DesabilitarTodasEtapasFuncionarioComissao() {
    Etapa2Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaFuncionarioComissao.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFuncionarioComissao.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaFuncionarioComissao.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFuncionarioComissao.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    $("#" + _etapaFuncionarioComissao.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaFuncionarioComissao.Etapa2.idTab + " .step").attr("class", "step");
}

function Etapa2Aprovada() {
    $("#" + _etapaFuncionarioComissao.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFuncionarioComissao.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    $("#" + _etapaFuncionarioComissao.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFuncionarioComissao.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
}

function Etapa2Aguardando() {
    $("#" + _etapaFuncionarioComissao.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFuncionarioComissao.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
}