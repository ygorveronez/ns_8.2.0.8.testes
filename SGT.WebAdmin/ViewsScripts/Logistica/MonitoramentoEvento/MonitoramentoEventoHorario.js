/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _monitoramentoEventoHorario;

/*
 * Declaração das Classes
 */

var MonitoramentoEventoHorario = function () {
    this.RestringirHorario = PropertyEntity({ text: "Restringir horário de ação?", getType: typesKnockout.bool, val: ko.observable(false) });
    _monitoramentoEvento.RestringirHorario = this.RestringirHorario;
    this.HoraInicio = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.time, text: "*Horário inicial:", required: ko.observable(false), visible: ko.observable(false) });
    this.HoraFim = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.time, text: "*Horário final:", required: ko.observable(false), visible: ko.observable(false) });
    this.RestringirHorario.val.subscribe(function (checked) {
        _monitoramentoEventoHorario.HoraInicio.visible(checked);
        _monitoramentoEventoHorario.HoraFim.visible(checked);
        _monitoramentoEventoHorario.HoraInicio.required(checked);
        _monitoramentoEventoHorario.HoraFim.required(checked);
    });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadMonitoramentoEventoHorario() {
    _monitoramentoEventoHorario = new MonitoramentoEventoHorario();
    KoBindings(_monitoramentoEventoHorario, "knockoutMonitoramentoEventoHorario");
}

/*
 * Declaração das Funções Públicas
 */

function limparCamposMonitoramentoEventoHorario() {
    LimparCampos(_monitoramentoEventoHorario);
}

function obterMonitoramentoEventoHorarioSalvar() {
    return JSON.stringify(RetornarObjetoPesquisa(_monitoramentoEventoHorario));
}

function preencherMonitoramentoEventoHorario(dados) {
    if (dados.RestringirHorario) {
        _monitoramentoEventoHorario.RestringirHorario.val(true);
    } else {
        _monitoramentoEventoHorario.RestringirHorario.val(false);
    }
    PreencherObjetoKnout(_monitoramentoEventoHorario, { Data: dados })
}

function validarCamposObrigatoriosMonitoramentoEventoHorario() {
    return ValidarCamposObrigatorios(_monitoramentoEventoHorario);
}
