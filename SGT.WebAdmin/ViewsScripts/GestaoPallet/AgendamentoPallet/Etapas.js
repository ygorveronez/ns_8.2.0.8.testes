/// <reference path="../../../ViewsScripts/Enumeradores/EnumEtapaAgendamentoPallet.js" />

//#region Objetos Globais do Arquivo
var _etapas;
// #endregion Objetos Globais do Arquivo

//#region Classes
var Etapas = function () {
    this.Etapa1 = PropertyEntity({
        text: ko.observable("Agendamento"),
        type: types.local,
        enable: ko.observable(true),
        eventClick: null,
        idGrid: guid(),
        idTab: guid(),
        step: ko.observable(1),
        icon: "fal fa-boxes",
        class: ko.observable("step yellow"),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable(""),
        etapa: EnumEtapaAgendamentoPallet.Agendamento,
        visible: ko.observable(true),
        isActive: ko.observable(false),
        situacao: ko.observable(0)
    });

    this.Etapa2 = PropertyEntity({
        text: ko.observable("NFe"),
        type: types.local,
        enable: ko.observable(true),
        eventClick: null,
        idGrid: guid(),
        idTab: guid(),
        icon: "fal fa-pencil-alt",
        class: ko.observable("step"),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable(""),
        etapa: EnumEtapaAgendamentoPallet.NFe,
        visible: ko.observable(true),
        isActive: ko.observable(false),
        situacao: ko.observable(1)
    });

    this.Etapa3 = PropertyEntity({
        text: ko.observable("Acompanhamento"),
        type: types.local,
        enable: ko.observable(true),
        eventClick: null,
        idGrid: guid(),
        idTab: guid(),
        icon: "fal fa-pencil-alt",
        class: ko.observable("step"),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable(""),
        etapa: EnumEtapaAgendamentoPallet.Acompanhamento,
        visible: ko.observable(true),
        isActive: ko.observable(false),
        situacao: ko.observable(2)
    });

    this.Etapas = PropertyEntity({ def: [], val: ko.observableArray([]) });
}
//#endregion Classes

// #region Funções de Inicialização
function carregarEtapasAgendamentoPallet() {
    _etapas = new Etapas();
    KoBindings(_etapas, "knockoutEtapasAgendamentoPallet");
}

// #endregion Funções de Inicialização

function controlarAcoesContainerPrincipal(situacaoEtapa, objetoKnockout) {
    for (var prop in objetoKnockout) {
        if (objetoKnockout.hasOwnProperty(prop)) {
            var property = objetoKnockout[prop];
            if (property.hasOwnProperty('enable') && situacaoEtapa == 1) {
                if (typeof property.enable === 'function')
                    property.enable(false);
                else if (typeof property.enable === 'boolean')
                    property.enable = false;
            }
        }
    }
}

function SetarEtapasRequisicaoAgendamentoPallet(status) {
    if (status == EnumEtapaAgendamentoPallet.Agendamento) {
        EtapaAgendamentoPallet();
    }
    else if (status == EnumEtapaAgendamentoPallet.NFe) {
        EtapaDadosNFePalletLiberada();
    }
    else if (status == EnumEtapaAgendamentoPallet.Acompanhamento) {
        EtapaDadosAcompanhamentoPalletLiberada();
    }
    else
        EtapaAgendamentoPallet();
}

function VerificarSituacaoPallet(codigoSituacao) {
    if (EnumSituacaoAgendamentoPallet.Cancelado == codigoSituacao) {
        SetarEnableCamposKnockout(_etapaAgendamentoPallet, false);
        SetarEnableCamposKnockout(_retornoAcompanhamentoPallet, false);

        _etapaAgendamentoPallet.Adicionar.visible(false);
    }
}

function SetarCorEtapa(id) {
    $("#" + id).prop("disabled", false);

    if (_etapaAgendamentoPallet.Situacao.val() == EnumSituacaoAgendamentoPallet.Agendamento || _etapaAgendamentoPallet.Situacao == EnumSituacaoAgendamentoPallet.Acompanhamento)
        $("#" + id + " .step").attr("class", "step yellow");
    else if (_etapaAgendamentoPallet.Situacao.val() == EnumSituacaoAgendamentoPallet.Cancelado)
        $("#" + id + " .step").attr("class", "step red");
    else if (_etapaAgendamentoPallet.Situacao.val() == EnumSituacaoAgendamentoPallet.Finalizado)
        $("#" + id + " .step").attr("class", "step green");
    else
        $("#" + id + " .step").attr("class", "step yellow");
}

// Etapa Agendamento
function EtapaDadosAgendamentoPalletDesabilitada() {
    $("#" + _etapas.Etapa1.idTab).prop("disabled", true);
    $("#" + _etapas.Etapa1.idTab + " .step").attr("class", "step");
}

function EtapaDadosAgendamentoPalletAceita() {
    $("#" + _etapas.Etapa1.idTab).prop("disabled", false);
    $("#" + _etapas.Etapa1.idTab + " .step").attr("class", "step green");
}

function EtapaDadosAgendamentoPalletReprovada() {
    $("#" + _etapas.Etapa1.idTab).prop("disabled", false);
    $("#" + _etapas.Etapa1.idTab + " .step").attr("class", "step red");
}
// Etapa Agendamento

// Etapa NFe
function EtapaDadosNFeDesabilitada() {
    $("#" + _etapas.Etapa2.idTab).prop("disabled", true);
    $("#" + _etapas.Etapa2.idTab + " .step").attr("class", "step");
}

function EtapaDadosNFePalletAceita() {
    $("#" + _etapas.Etapa2.idTab).prop("disabled", false);
    $("#" + _etapas.Etapa2.idTab + " .step").attr("class", "step green");
}

function EtapaDadosNFePalletReprovada() {
    $("#" + _etapas.Etapa2.idTab).prop("disabled", false);
    $("#" + _etapas.Etapa2.idTab + " .step").attr("class", "step red");
}
// Etapa NFe

// Etapa Acompanhamento
function EtapaDadosAcompanhamentoDesabilitada() {
    $("#" + _etapas.Etapa3.idTab).prop("disabled", true);
    $("#" + _etapas.Etapa3.idTab + " .step").attr("class", "step");
}

function EtapaDadosAcompanhamentoPalletAceita() {
    $("#" + _etapas.Etapa3.idTab).prop("disabled", false);
    $("#" + _etapas.Etapa3.idTab + " .step").attr("class", "step green");
}

function EtapaDadosAcompanhamentoPalletReprovada() {
    $("#" + _etapas.Etapa3.idTab).prop("disabled", false);
    $("#" + _etapas.Etapa3.idTab + " .step").attr("class", "step red");
}
// Etapa Acompanhamento