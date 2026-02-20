/// <reference path="../../../ViewsScripts/Enumeradores/EnumStatusAgendamentoColetaPallet.js" />

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
        etapa: EnumStatusAgendamentoColetaPallet.Todos,
        visible: ko.observable(true),
        isActive: ko.observable(false),
        situacao: ko.observable(0)
    });

    this.Etapa2 = PropertyEntity({
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
        etapa: EnumStatusAgendamentoColetaPallet.EmAndamento,
        visible: ko.observable(true),
        isActive: ko.observable(false),
        situacao: ko.observable(1)
    });

    this.Etapa3 = PropertyEntity({
        text: ko.observable("Finalizado"),
        type: types.local,
        enable: ko.observable(false),
        eventClick: null,
        idGrid: guid(),
        idTab: guid(),
        icon: "fal fa-barcode",
        class: ko.observable("step"),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable(""),
        etapa: EnumStatusAgendamentoColetaPallet.Finalizado,
        visible: ko.observable(true),
        isActive: ko.observable(false),
        situacao: ko.observable(2)
    });

    this.Etapas = PropertyEntity({ def: [], val: ko.observableArray([]) });
}
//#endregion Classes

// #region Funções de Inicialização
function carregarEtapasAgendamentoColetaPallet() {
    _etapas = new Etapas();
    KoBindings(_etapas, "knockoutEtapasAgendamentoColetaPallet");

    incializarEtapas();
}

function incializarEtapas() {
    _etapas.Etapas.val.push(_etapas.EtapaAgendamento);
    _etapas.Etapas.val.push(_etapas.EtapaAcompanhamento);
    _etapas.Etapas.val.push(_etapas.EtapaFinalizado);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos
// #endregion Funções Associadas a Eventos

// #region Funções Públicas

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

// #endregion Funções Públicas
