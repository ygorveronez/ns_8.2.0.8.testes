/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumMonitoramentoTratativaAutomatica.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _monitoramentoEventoTratativaAutomatica;

/*
 * Declaração das Classes
 */

var MonitoramentoEventoTratativaAutomatica = function () {
    this.Codigo = PropertyEntity({ text: "", getType: typesKnockout.int, val: ko.observable(false) });
    this.TratativaAutomatica = PropertyEntity({ text: "Habilitar tratativa automática?", getType: typesKnockout.bool, val: ko.observable(false) });
    _monitoramentoEvento.TratativaAutomatica = this.TratativaAutomatica;
    this.GatilhoTratativaAutomatica = PropertyEntity({ text: "*Tratar automaticamente no", val: ko.observable(""), options: EnumMonitoramentoTratativaAutomatica.obterOpcoes(), required: ko.observable(false), visible: ko.observable(false) });
    this.TratativaAutomatica.val.subscribe(function (checked) {
        _monitoramentoEventoTratativaAutomatica.GatilhoTratativaAutomatica.visible(checked);
        _monitoramentoEventoTratativaAutomatica.GatilhoTratativaAutomatica.required(checked);
    });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadMonitoramentoEventoTratativaAutomatica() {
    _monitoramentoEventoTratativaAutomatica = new MonitoramentoEventoTratativaAutomatica();
    KoBindings(_monitoramentoEventoTratativaAutomatica, "knockoutMonitoramentoEventoTratativaAutomatica");
}

/*
 * Declaração das Funções Públicas
 */

function limparCamposMonitoramentoEventoTratativaAutomatica() {
    LimparCampos(_monitoramentoEventoTratativaAutomatica);
}

function obterMonitoramentoEventoTratativaAutomaticaSalvar() {
    return JSON.stringify(RetornarObjetoPesquisa(_monitoramentoEventoTratativaAutomatica));
}

function preencherMonitoramentoEventoTratativaAutomatica(dados) {
    if (dados.length > 0 && dados[0].TratativaAutomatica) {
        _monitoramentoEventoTratativaAutomatica.TratativaAutomatica.val(true);
        _monitoramentoEventoTratativaAutomatica.GatilhoTratativaAutomatica.val(dados[0].GatilhoTratativaAutomatica);
        _monitoramentoEventoTratativaAutomatica.Codigo.val(dados[0].Codigo);
    } else {
        _monitoramentoEventoTratativaAutomatica.TratativaAutomatica.val(false);
        _monitoramentoEventoTratativaAutomatica.GatilhoTratativaAutomatica.val("");
        _monitoramentoEventoTratativaAutomatica.Codigo.val(0);
    }
    PreencherObjetoKnout(_monitoramentoEventoTratativaAutomatica, { Data: dados[0] })
}

function validarCamposObrigatoriosMonitoramentoEventoTratativaAutomatica() {
    return ValidarCamposObrigatorios(_monitoramentoEventoTratativaAutomatica);
}
