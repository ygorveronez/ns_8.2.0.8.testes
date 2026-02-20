/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />

// #region Objetos Globais do Arquivo

var _historicoTransportadoresInteressadosCarga;
var _gridHistoricoTransportadoresInteressadosCarga;

// #endregion Objetos Globais do Arquivo

// #region Classes

var HistoricoTransportadoresInteressadosCarga = function () {
    this.Carga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.InteresseAtualOuHistorico = PropertyEntity({ val: ko.observable(true), def: true });
};

// #endregion Classes

// #region Funções de Inicialização

function loadHistoricoTransportadoresInteressadosCarga() {
    _historicoTransportadoresInteressadosCarga = new HistoricoTransportadoresInteressadosCarga();
    KoBindings(_historicoTransportadoresInteressadosCarga, "divModalHistoricoTransportadoresInteressadosCarga");

    loadGridHistoricoTransportadoresInteressadosCarga();
}

function loadGridHistoricoTransportadoresInteressadosCarga() {
    _gridHistoricoTransportadoresInteressadosCarga = new GridView("grid-historico-transportadores-interessados-carga", "JanelaCarregamento/ObterInteressadosCarga", _historicoTransportadoresInteressadosCarga, null, null, 10);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function AbrirTelaHistoricoTransportadoresInteressadosCarga(carga) {
    _historicoTransportadoresInteressadosCarga.Carga.val(carga);

    _gridHistoricoTransportadoresInteressadosCarga.CarregarGrid();

    Global.abrirModal("divModalHistoricoTransportadoresInteressadosCarga");
}

// #endregion Funções Públicas