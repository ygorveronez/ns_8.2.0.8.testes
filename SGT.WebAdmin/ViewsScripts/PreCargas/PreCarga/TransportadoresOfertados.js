/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="TransportadoresOfertadosHistorico.js" />

// #region Objetos Globais do Arquivo

var _gridPreCargaTransportadoresOfertados;
var _legendaPreCargaTransportadoresOfertados;
var _preCargaTransportadoresOfertados;

// #endregion Objetos Globais do Arquivo

// #region Classes

var LegendaPreCargaTransportadoresOfertados = function () {
    this.Bloqueado = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Bloqueado });
    this.OfertaForaRota = PropertyEntity({ text: Localization.Resources.Cargas.Carga.OfertaPorRota });
    this.OfertaNormal = PropertyEntity({ text: Localization.Resources.Cargas.Carga.OfertaNormal });
    this.OfertaPorRota = PropertyEntity({ text: Localization.Resources.Cargas.Carga.OfertaPorRota });
}

var PreCargaTransportadoresOfertados = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

// #endregion Classes

// #region Funções de Inicialização

function loadPreCargaTransportadoresOfertados() {
    _preCargaTransportadoresOfertados = new PreCargaTransportadoresOfertados();

    _legendaPreCargaTransportadoresOfertados = new LegendaPreCargaTransportadoresOfertados();
    KoBindings(_legendaPreCargaTransportadoresOfertados, "knockoutLegendaPreCargaTransportadoresOfertados");

    loadGridPreCargaTransportadoresOfertados();
    loadPreCargaTransportadoresOfertadosHistorico();
}

function loadGridPreCargaTransportadoresOfertados() {
    var quantidadePorPagina = 10;
    var opcaoDetalhes = { descricao: "Detalhes", id: "clasEditar", evento: "onclick", metodo: visualizarPreCargaTransportadoresOfertadosHistoricoClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoDetalhes] };

    _gridPreCargaTransportadoresOfertados = new GridView("grid-pre-carga-transportadores-ofertados", "PreCarga/ObterTransportadoresOfertados", _preCargaTransportadoresOfertados, menuOpcoes, null, quantidadePorPagina, undefined, undefined, undefined, undefined, 99999);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function visualizarPreCargaTransportadoresOfertadosHistoricoClick(registroSelecionado) {
    visualizarPreCargaTransportadoresOfertadosHistorico(registroSelecionado.Codigo)
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function visualizarPreCargaTransportadoresOfertados(codigoPreCarga) {
    _preCargaTransportadoresOfertados.Codigo.val(codigoPreCarga);

    recarregarGridPreCargaTransportadoresOfertados();
    exibirModalPreCargaTransportadoresOfertados();
}

// #endregion Funções Públicas

// #region Funções Privadas

function exibirModalPreCargaTransportadoresOfertados() {
    Global.abrirModal('divModalPreCargaTransportadoresOfertados');
}

function recarregarGridPreCargaTransportadoresOfertados() {
    _gridPreCargaTransportadoresOfertados.CarregarGrid();
}

// #endregion Funções Privadas
