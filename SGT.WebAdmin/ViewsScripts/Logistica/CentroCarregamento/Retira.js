/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/MotivoAdvertenciaTransportador.js" />
/// <reference path="../../Enumeradores/EnumTipoManobraAcao.js" />
/// <reference path="CentroCarregamento.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _knockoutRetira;

/*
 * Declaração das Classes
 */

var Retira = function () {
    this.ObservacaoRetira = PropertyEntity({ type: types.string, codEntity: ko.observable(0), text: Localization.Resources.Logistica.CentroCarregamento.Observacao.getFieldDescription(), val: ko.observable("") });
}

function loadRetira() {
    _knockoutRetira = new Retira();
    KoBindings(_knockoutRetira, "knockoutRetira");
}

/*
 * Funções
 */

function limparCamposRetira() {
    LimparCampos(_knockoutRetira);
}

function preencherRetiraSalvar(centroCarregamento) {
    centroCarregamento["ObservacaoRetira"] = _knockoutRetira.ObservacaoRetira.val();
}

function preencherRetira(dadosRetira) {
    _knockoutRetira.ObservacaoRetira.val(dadosRetira);
}