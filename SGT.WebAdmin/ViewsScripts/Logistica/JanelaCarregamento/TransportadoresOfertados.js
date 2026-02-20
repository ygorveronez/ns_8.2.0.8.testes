/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="TransportadoresOfertadosHistorico.js" />

// #region Objetos Globais do Arquivo

var _gridTransportadoresOfertados;
var _legendaTransportadoresOfertados;
var _transportadoresOfertados;

// #endregion Objetos Globais do Arquivo

// #region Classes

var LegendaTransportadoresOfertados = function () {
    this.Bloqueado = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Bloqueado, visible: ko.observable(false) });
    this.OfertaForaRota = PropertyEntity({ text: Localization.Resources.Cargas.Carga.OfertaForaDaRota, visible: ko.observable(false) });
    this.OfertaNormal = PropertyEntity({ text: Localization.Resources.Cargas.Carga.OfertaNormal, visible: ko.observable(false) });
    this.OfertaPorMenorValorFrete = PropertyEntity({ text: Localization.Resources.Cargas.Carga.OfertaPorMenorValorDeFrete, visible: ko.observable(false) });
    this.OfertaPorRota = PropertyEntity({ text: Localization.Resources.Cargas.Carga.OfertaPorRota, visible: ko.observable(false) });
}

var TransportadoresOfertados = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

// #endregion Classes

// #region Funções de Inicialização

function loadTransportadoresOfertados() {
    _transportadoresOfertados = new TransportadoresOfertados();

    _legendaTransportadoresOfertados = new LegendaTransportadoresOfertados();
    KoBindings(_legendaTransportadoresOfertados, "knockoutLegendaTransportadoresOfertados");

    loadGridTransportadoresOfertados();
    loadTransportadoresOfertadosHistorico();
}

function loadGridTransportadoresOfertados() {
    var quantidadePorPagina = 10;
    var opcaoDetalhes = { descricao: Localization.Resources.Gerais.Geral.Detalhes, id: "clasEditar", evento: "onclick", metodo: visualizarTransportadoresOfertadosHistoricoClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoDetalhes] };

    _gridTransportadoresOfertados = new GridView("grid-transportadores-ofertados", "JanelaCarregamento/ObterTransportadoresOfertados", _transportadoresOfertados, menuOpcoes, null, quantidadePorPagina, undefined, undefined, undefined, undefined, 99999);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function visualizarTransportadoresOfertadosHistoricoClick(registroSelecionado) {
    visualizarTransportadoresOfertadosHistorico(registroSelecionado.Codigo)
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function visualizarTransportadoresOfertados(codigoJanelaCarregamento) {
    _transportadoresOfertados.Codigo.val(codigoJanelaCarregamento);

    _legendaTransportadoresOfertados.Bloqueado.visible(_centroCarregamentoAtual.ExibirTransportadoresOfertadosComMenorValorFreteTabela);
    _legendaTransportadoresOfertados.OfertaForaRota.visible(_centroCarregamentoAtual.ExibirTransportadoresOfertadosPorPrioridadeDeRota);
    _legendaTransportadoresOfertados.OfertaNormal.visible(_centroCarregamentoAtual.ExibirTransportadoresOfertadosComMenorValorFreteTabela || _centroCarregamentoAtual.ExibirTransportadoresOfertadosPorPrioridadeDeRota);
    _legendaTransportadoresOfertados.OfertaPorMenorValorFrete.visible(_centroCarregamentoAtual.ExibirTransportadoresOfertadosComMenorValorFreteTabela);
    _legendaTransportadoresOfertados.OfertaPorRota.visible(_centroCarregamentoAtual.ExibirTransportadoresOfertadosPorPrioridadeDeRota);

    recarregarGridTransportadoresOfertados();
    exibirModalTransportadoresOfertados();
}

// #endregion Funções Públicas

// #region Funções Privadas

function exibirModalTransportadoresOfertados() {
    Global.abrirModal('divModalTransportadoresOfertados');
}

function recarregarGridTransportadoresOfertados() {
    _gridTransportadoresOfertados.CarregarGrid();
}

// #endregion Funções Privadas
