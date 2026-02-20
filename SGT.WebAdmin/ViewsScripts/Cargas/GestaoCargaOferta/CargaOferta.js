/// <reference path="../../../wwwroot/js/Global/Auditoria.js" />
/// <reference path="../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../wwwroot/js/Global/Rest.js" />
/// <reference path="../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../wwwroot/js/bootstrap/bootstrap.js" />
/// <reference path="../../../wwwroot/js/libs/jquery.blockui.js" />
/// <reference path="../../../wwwroot/js/Global/knoutViewsSlides.js" />
/// <reference path="../../../wwwroot/js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Global/ModeloFiltroPesquisa/ConfiguracaoModeloFiltroPesquisa.js" />
/// <reference path="../../Enumeradores/EnumSituacaoIntegracao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCargaOferta.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/Cliente.js" />

// #region Objetos Globais do Arquivo

var _pesquisaCargaOferta;
var _gridCargaOferta;
var _botoes;
var _gridHistoricoIntegracaoCargaOferta;
var _pesquisaHistoricoIntegracaoCargaOferta;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaCargaOferta = function () {
    var self = this;
    var dataInicial = moment().add(-3, 'days').format("DD/MM/YYYY");
    var dataFinal = moment().add(1, 'days').format("DD/MM/YYYY");

    this.DataInicio = PropertyEntity({ text: "Data Inicial", getType: typesKnockout.date, val: ko.observable(dataInicial), required: ko.observable(false) });
    this.DataFim = PropertyEntity({ text: "Data Final", dateRangeInit: this.DataInicio, getType: typesKnockout.date, val: ko.observable(dataFinal), required: ko.observable(false) });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.Filiais = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filiais", issue: 70, idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.CodigosCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Carga", val: ko.observable(""), idBtnSearch: guid(), def: "", visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Tipo de Operação", issue: 121, visible: ko.observable(true), idBtnSearch: guid(), cssClass: "col col-sm-3 col-md-3 col-lg-3" });

    this.Transportadores = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Transportador", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TiposCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Carga", idBtnSearch: guid(), visible: ko.observable(true) });

    this.SituacaoIntegracao = PropertyEntity({ text: "Situação da Integração", val: ko.observable(EnumSituacaoIntegracao.Todas), options: EnumSituacaoIntegracao.obterOpcoesPesquisa(), def: EnumSituacaoIntegracao.Todas });
    this.SituacaoOferta = PropertyEntity({ text: "Situação", val: ko.observable(EnumSituacaoCargaOferta.EmOferta), options: EnumSituacaoCargaOferta.obterOpcoesPesquisa(), def: EnumSituacaoCargaOferta.EmOferta });

    this.ModeloFiltrosPesquisa = PropertyEntity({
        type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.ModeloDeFiltroDePesquisa, idBtnSearch: guid(),
        tipoFiltroPesquisa: EnumCodigoFiltroPesquisa.CargaOferta,
        callbackRetornoPesquisa: function () {
            $("#" + _pesquisaCargaOferta.Filiais.id).trigger("change");
            $("#" + _pesquisaCargaOferta.TipoOperacao.id).trigger("change");
            $("#" + _pesquisaCargaOferta.Transportadores.id).trigger("change");
            $("#" + _pesquisaCargaOferta.SituacaoOferta.id).trigger("change");
            $("#" + _pesquisaCargaOferta.SituacaoIntegracao.id).trigger("change");
        }
    });

    this.ConfiguracaoModeloFiltroPesquisa = PropertyEntity({ eventClick: function (e) { abrirConfiguracaoModeloFiltroPesquisa(EnumCodigoFiltroPesquisa.CargaOferta, _pesquisaCargaOferta) }, type: types.event, text: Localization.Resources.Gerais.Geral.ConfiguracaoDeFiltros, visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function () {
            carregarGridCargaOferta();
            limparGridListaCargaOfertaSelecionados();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaCargaOferta.ExibirFiltrosAvancados.visibleFade()) {
                _pesquisaCargaOferta.ExibirFiltrosAvancados.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaCargaOferta.ExibirFiltrosAvancados.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Avancada, icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });

    this.ExibirFiltrosAvancados = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });
    this.ListaCargaOfertaSelecionados = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
};

var Botoes = function () {
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(true), enable: ko.observable(true) });
    this.Acoes = PropertyEntity({ val: ko.observable(false), def: false, text: "Ações", visible: ko.observable(true), enable: ko.observable(true) });
    this.Ofertar = PropertyEntity({ val: ko.observable(false), def: false, type: types.event, getType: typesKnockout.bool, text: "Ofertar", visible: ko.observable(true), enable: ko.observable(true), eventClick: ofertarClick });
    this.Cancelar = PropertyEntity({ val: ko.observable(false), def: false, type: types.event, getType: typesKnockout.bool, text: "Cancelar", visible: ko.observable(true), enable: ko.observable(true), eventClick: cancelarClick });
}

var PesquisaHistoricoIntegracaoCargaOferta = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

// #endregion Classes

// #region Funções de Inicialização

function loadFiltroPesquisa() {
    var data = { TipoFiltro: EnumCodigoFiltroPesquisa.CargaOferta };

    executarReST("ModeloFiltroPesquisa/ObterFiltroPesquisaPadrao", data, function (res) {
        if (res.Success && Boolean(res.Data)) {
            PreencherJsonFiltroPesquisa(_pesquisaCargaOferta, res.Data.Dados);
            _pesquisaCargaOferta.ModeloFiltrosPesquisa.codEntity(res.Data.Codigo);
            _pesquisaCargaOferta.ModeloFiltrosPesquisa.val(res.Data.Descricao);

            if (_pesquisaCargaOferta.ModeloFiltrosPesquisa.callbackRetornoPesquisa instanceof Function)
                _pesquisaCargaOferta.ModeloFiltrosPesquisa.callbackRetornoPesquisa();
        }

        carregarGridCargaOferta();
    });
}

function loadCargaOferta() {
    _pesquisaCargaOferta = new PesquisaCargaOferta();
    KoBindings(_pesquisaCargaOferta, "knockoutPesquisaCargaOferta", false, _pesquisaCargaOferta.Pesquisar.id);

    _botoes = new Botoes();
    KoBindings(_botoes, "knoutBotoes");

    BuscarFilial(_pesquisaCargaOferta.Filiais);
    BuscarTiposOperacao(_pesquisaCargaOferta.TipoOperacao);
    BuscarCargas(_pesquisaCargaOferta.CodigosCarga);
    BuscarTransportadores(_pesquisaCargaOferta.Transportadores);
    BuscarTiposdeCarga(_pesquisaCargaOferta.TiposCarga);

    loadGridCargaOferta();
    loadFiltroPesquisa();
}

function loadGridCargaOferta() {
    const opcaoIntegracoes = { descricao: "Integrações", id: guid(), metodo: buscarIntegracoesOfertaClick, icone: "", visibilidade: true };
    const auditoria = { descricao: "Auditoria", id: guid(), metodo: OpcaoAuditoria("CargaOferta", null, _pesquisaCargaOferta, null, true), icone: "", visibilidade: true };

    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [opcaoIntegracoes, auditoria], tamanho: 10 };

    const multiplaEscolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        callbackNaoSelecionado: naoSelecionadoCallback,
        callbackSelecionado: selecionadoCallback,
        somenteLeitura: false,
        SelecionarTodosKnout: _botoes.SelecionarTodos,
    }

    var configExportacao = {
        url: "CargaOferta/ExportarPesquisa",
        titulo: "Ofertas de Cargas"
    };

    _gridCargaOferta = new GridViewExportacao(
        "grid-carga-oferta",
        "CargaOferta/Pesquisa",
        _pesquisaCargaOferta,
        menuOpcoes,
        configExportacao,
        null,
        null,
        multiplaEscolha
    );

    _gridCargaOferta.SetQuantidadeLinhasPorPagina(12);
    _gridCargaOferta.SetPermitirEdicaoColunas(true);
    _gridCargaOferta.SetSalvarPreferenciasGrid(true);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function buscarIntegracoesOfertaClick(registroSelecionado) {

    buscarIntegracaoCargaOferta(registroSelecionado);

    Global.abrirModal("divModalIntegracaoCargaOferta");
}

function historicoIntegracaoOfertaClick(registroSelecionado) {

    buscarHistoricoIntegracaoCargaOferta(registroSelecionado);

    Global.abrirModal("divModalHistoricoIntegracaoCargaOferta");
}

function ofertarClick() {
    var rowsSelecionadas = _gridCargaOferta.ObterMultiplosSelecionados();

    var codigos = _pesquisaCargaOferta.ListaCargaOfertaSelecionados.val();
    var data = { Codigos: codigos };

    executarReST("CargaOferta/Ofertar", data, function (res) {
        if (res.Success) {
            if (res.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, res.Msg);
                _gridCargaOferta.CarregarGrid();
            }
            else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, res.Msg);
                _gridCargaOferta.CarregarGrid();
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, res.Msg)
        }
    });
}

function cancelarClick() {
    var codigos = _pesquisaCargaOferta.ListaCargaOfertaSelecionados.val();
    var data = { Codigos: codigos };
    executarReST("CargaOferta/Cancelar", data, function (res) {
        if (res.Success) {
            if (res.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, res.Msg)
            }
            else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, res.Msg)
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, res.Msg)
        }
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Privadas

function buscarRowsSelecionadasCargaOferta() {
    var rowsSelecionadas = null;

    if (_botoes.SelecionarTodos.val())
        rowsSelecionadas = _gridCargaOferta.ObterMultiplosNaoSelecionados();
    else
        rowsSelecionadas = _gridCargaOferta.ObterMultiplosSelecionados();

    var codigosRowsSelecionadas = obterCodigosRowsSelecionadas(rowsSelecionadas);

    if (codigosRowsSelecionadas && (codigosRowsSelecionadas.length > 0 || _botoes.SelecionarTodos.val()))
        _pesquisaCargaOferta.ListaCargaOfertaSelecionados.val(JSON.stringify(codigosRowsSelecionadas));
    else
        _pesquisaCargaOferta.ListaCargaOfertaSelecionados.val("");
}

function obterCodigosRowsSelecionadas(rowsSelecionadas) {
    var codigos = new Array();

    for (var i = 0; i < rowsSelecionadas.length; i++)
        codigos.push(rowsSelecionadas[i].Codigo);

    return codigos;
}

function naoSelecionadoCallback() {
    buscarRowsSelecionadasCargaOferta();
}

function selecionadoCallback() {
    buscarRowsSelecionadasCargaOferta();
}

function carregarGridCargaOferta() {
    _gridCargaOferta.CarregarGrid(function (grid) {
        var temDados = grid.data.length > 0;

        _botoes.SelecionarTodos.visible(temDados);
        _botoes.SelecionarTodos.val(false);
        _botoes.Acoes.visible(temDados);
        _botoes.Acoes.val(false);

        _gridCargaOferta.AtualizarRegistrosSelecionados([]);
        _gridCargaOferta.AtualizarRegistrosNaoSelecionados([]);

    });
}

function limparGridListaCargaOfertaSelecionados() {
    _pesquisaCargaOferta.ListaCargaOfertaSelecionados.val("");
}

function buscarIntegracaoCargaOferta(registroSelecionado) {
    _pesquisaHistoricoIntegracaoCargaOferta = new PesquisaHistoricoIntegracaoCargaOferta();
    _pesquisaHistoricoIntegracaoCargaOferta.Codigo.val(registroSelecionado.Codigo);

    let historico = { descricao: "Arquivos Integração", id: guid(), evento: "onclick", metodo: historicoIntegracaoOfertaClick, tamanho: "20", icone: "" };

    let menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [historico]
    };

    let _gridIntegracoesCargaOferta = new GridView("tblIntegracaoCargaOferta", "CargaOferta/ConsultarIntegracoes", _pesquisaHistoricoIntegracaoCargaOferta, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridIntegracoesCargaOferta.CarregarGrid();
}

function buscarHistoricoIntegracaoCargaOferta(registroSelecionado) {
    _pesquisaHistoricoIntegracaoCargaOferta = new PesquisaHistoricoIntegracaoCargaOferta();
    _pesquisaHistoricoIntegracaoCargaOferta.Codigo.val(registroSelecionado.Codigo);

    let download = { descricao: Localization.Resources.Gerais.Geral.DownloadArquivos, id: guid(), evento: "onclick", metodo: downloadArquivosHistoricoIntegracaoCargaOferta, tamanho: "20", icone: "" };

    let menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoCargaOferta = new GridView("tblHistoricoIntegracaoCargaOferta", "CargaOferta/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracaoCargaOferta, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoCargaOferta.CarregarGrid();
};

function downloadArquivosHistoricoIntegracaoCargaOferta(data) {
    executarDownload("CargaOferta/DownloadArquivosIntegracao", { Codigo: data.Codigo });
}

// #endregion Funções Privadas
