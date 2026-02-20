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
/// <reference path="../../Enumeradores/EnumSituacaoChamado.js" />

var _gestaoOcorrenciaEtapas;

var EtapasGestaoOcorrencia = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("50%") });
    
    this.Etapa1 = PropertyEntity({
        text: "Ocorrência", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("Nessa etapa é realizado o lançamento da ocorrência."),
        tooltipTitle: ko.observable("Ocorrência")
    });
    
    this.Etapa2 = PropertyEntity({
        text: "Atendimento", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("Nessa etapa são visualizadas as informações sobre o andamento do atendimento."),
        tooltipTitle: ko.observable("Atendimento")
    });
}

function loadGestaoOcorrenciaEtapas() {
    _gestaoOcorrenciaEtapas = new EtapasGestaoOcorrencia();
    KoBindings(_gestaoOcorrenciaEtapas, "knockoutEtapasGestaoOcorrencia");

    etapaOcorrenciaAguardando();
}

function etapaOcorrenciaAguardando() {
    $("#" + _gestaoOcorrenciaEtapas.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _gestaoOcorrenciaEtapas.Etapa1.idTab + " .step").attr("class", "step yellow");

    _gestaoOcorrencia.Adicionar.visible(true);
    SetarEnableCamposKnockout(_gestaoOcorrencia, true);

    focarEtapaOcorrenciaAguardando();
    etapaAtendimentoDesabilitada();
}

function focarEtapaOcorrenciaAguardando() {
    $("#tabOcorrencia").click();
}

function etapaAtendimentoDesabilitada() {
    $("#" + _gestaoOcorrenciaEtapas.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _gestaoOcorrenciaEtapas.Etapa2.idTab + " .step").attr("class", "step");
}

function etapaAtendimentoAguardando() {
    $("#" + _gestaoOcorrenciaEtapas.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _gestaoOcorrenciaEtapas.Etapa2.idTab + " .step").attr("class", "step yellow");

    _gestaoOcorrencia.Adicionar.visible(false);
    SetarEnableCamposKnockout(_gestaoOcorrencia, false);
    
    etapaOcorrenciaConcluida();
}

function focarEtapaAtendimento() {
    $("#tabAtendimento").click();
}

function etapaOcorrenciaConcluida() {
    $("#" + _gestaoOcorrenciaEtapas.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _gestaoOcorrenciaEtapas.Etapa1.idTab + " .step").attr("class", "step green");

    focarEtapaAtendimento();
}

function etapaAtendimentoConcluida() {
    $("#" + _gestaoOcorrenciaEtapas.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _gestaoOcorrenciaEtapas.Etapa2.idTab + " .step").attr("class", "step green");

    _gestaoOcorrencia.Adicionar.visible(false);
    SetarEnableCamposKnockout(_gestaoOcorrencia, false);

    etapaOcorrenciaConcluida();
}

function etapaAtendimentoCancelada() {
    $("#" + _gestaoOcorrenciaEtapas.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _gestaoOcorrenciaEtapas.Etapa2.idTab + " .step").attr("class", "step red");

     _gestaoOcorrencia.Adicionar.visible(false);
    SetarEnableCamposKnockout(_gestaoOcorrencia, false);
    
    etapaOcorrenciaConcluida();
}

function setarStatusEtapaAtendimento(situacao) {
    if (situacao == null || situacao === EnumSituacaoChamado.Finalizado)
        etapaAtendimentoConcluida();
    else if (situacao === EnumSituacaoChamado.Cancelada)
        etapaAtendimentoCancelada();
    else
        etapaAtendimentoAguardando();
}