/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />

// #region Objetos Globais do Arquivo

var _pesquisaCargaAdicionarFluxoPatio;
var _gridCargaAdicionarFluxoPatio;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaCargaAdicionarFluxoPatio = function () {
    var dataDiaAnterior = moment().add(-1, 'days').format("DD/MM/YYYY");

    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número da Carga:", maxlength: 20 });
    this.NumeroPedidoEmbarcador = PropertyEntity({ text: "Número do Pedido:", maxlength: 20 });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", val: ko.observable(dataDiaAnterior), def: dataDiaAnterior, getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.Destinatario = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.Remetente = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid() });
    this.TipoCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Carga:", idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid() });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridCargaAdicionarFluxoPatio, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadGridCargaAdicionarFluxoPatio() {
    var opcaoDetalhes = { descricao: "Detalhes", id: guid(), metodo: exibirDetalhesClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 10, opcoes: [opcaoDetalhes] };
    var configuracoesExportacao = { url: "AdicionarCargaFluxoPatio/ExportarPesquisa", titulo: "Cargas para Adicionar ao Fluxo de Pátio" };
    var quantidadePorPagina = 20;

    _gridCargaAdicionarFluxoPatio = new GridViewExportacao(_pesquisaCargaAdicionarFluxoPatio.Pesquisar.idGrid, "AdicionarCargaFluxoPatio/Pesquisa", _pesquisaCargaAdicionarFluxoPatio, menuOpcoes, configuracoesExportacao, undefined, quantidadePorPagina);
    _gridCargaAdicionarFluxoPatio.CarregarGrid();
}

function loadAdicionarCargaFluxoPatio() {
    _pesquisaCargaAdicionarFluxoPatio = new PesquisaCargaAdicionarFluxoPatio();
    KoBindings(_pesquisaCargaAdicionarFluxoPatio, "knockoutPesquisaCargaAdicionarFluxoPatio", false, _pesquisaCargaAdicionarFluxoPatio.Pesquisar.id);

    new BuscarFilial(_pesquisaCargaAdicionarFluxoPatio.Filial);
    new BuscarTiposOperacao(_pesquisaCargaAdicionarFluxoPatio.TipoOperacao);
    new BuscarClientes(_pesquisaCargaAdicionarFluxoPatio.Destinatario);
    new BuscarClientes(_pesquisaCargaAdicionarFluxoPatio.Remetente);
    new BuscarTiposdeCarga(_pesquisaCargaAdicionarFluxoPatio.TipoCarga);

    loadInformarDadosCarga();
    loadGridCargaAdicionarFluxoPatio();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function exibirDetalhesClick(registroSelecionado) {
    informarDadosCarga(registroSelecionado.Codigo);
}

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function recarregarGridCargaAdicionarFluxoPatio() {
    _gridCargaAdicionarFluxoPatio.CarregarGrid();
}

// #endregion Funções Públicas
