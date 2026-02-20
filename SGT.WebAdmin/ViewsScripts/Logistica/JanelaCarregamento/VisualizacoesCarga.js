/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />

// #region Objetos Globais do Arquivo

var _visualizacoesCarga;
var _gridvisualizacoesCarga;

// #endregion Objetos Globais do Arquivo

// #region Classes

var VisualizacoesCarga = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.Transportador = PropertyEntity({ val: ko.observable(0), def: 0 });
};

// #endregion Classes

// #region Funções de Inicialização

function loadVisualizacoesCarga() {
    _visualizacoesCarga = new VisualizacoesCarga();
    KoBindings(_visualizacoesCarga, "divModalVisualizacoesCarga");

    loadGridVisualizacoesCarga();
}

function loadGridVisualizacoesCarga() {
    var multiplaescolha = {
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        permitirSelecionarSomenteUmRegistro: true,
        somenteLeitura: false,
        callbackSelecionado: function (e, registroSelecionado) {
            _visualizacoesCarga.Transportador.val(registroSelecionado.CodigoTransportador);
            _visualizacoesCarga.SelecionarTransportador.enable(true);
            _visualizacoesCarga.ImprimirCotacao.visible(true);
        },
        callbackNaoSelecionado: function () {
            _visualizacoesCarga.Transportador.val(0);
            _visualizacoesCarga.SelecionarTransportador.enable(false);
            _visualizacoesCarga.ImprimirCotacao.visible(false);
        }
    };

    _gridVisualizacoesCarga = new GridView("grid-visualizacoes-carga", "JanelaCarregamento/ObterVisualizacoesCarga", _visualizacoesCarga, null, null, 10, null, null, null, multiplaescolha, 9999999);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos



// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function AbrirTelaVisualizacoesCarga(carga, sender) {
    if (sender)
        sender.stopPropagation();

    _visualizacoesCarga.Carga.val(carga);
    _gridVisualizacoesCarga.CarregarGrid();

    Global.abrirModal("divModalVisualizacoesCarga");
}

// #endregion Funções Públicas