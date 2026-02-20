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
/// <reference path="../../Enumeradores/EnumEtapaCarga.js" />
/// <reference path="../../Enumeradores/EnumTipoIntegracao.js" />
/// <reference path="../../Consultas/Carga.js" />

// #region Objetos Globais do Arquivo

var _pesquisa;
var _gridIntegracoesComFalha;
var _botoes;

// #endregion Objetos Globais do Arquivo

// #region Classes

var Pesquisa = function () {
    var self = this;
    var dataInicial = moment().add(-60, 'days').format("DD/MM/YYYY");
    var dataFinal = moment().format("DD/MM/YYYY");

    this.DataInicio = PropertyEntity({ text: "Data Inicial", getType: typesKnockout.date, val: ko.observable(dataInicial), required: ko.observable(false) });
    this.DataFim = PropertyEntity({ text: "Data Final", getType: typesKnockout.date, val: ko.observable(dataFinal), required: ko.observable(false) });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.CodigosCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Carga", val: ko.observable(""), idBtnSearch: guid(), def: "", visible: ko.observable(true) });
    this.EtapaCarga = PropertyEntity({ , text: "Etapa da carga", val: ko.observable(EnumSituacoesCarga.Todas), options: EnumSituacoesCarga.obterOpcoesPesquisa(), def: EnumSituacoesCarga.Todas });

    this.TipoIntegracao = PropertyEntity({ text: "Tipo da Integração", val: ko.observable(""), def: "", options: ko.observable([]), required: true });

    this.ModeloFiltrosPesquisa = PropertyEntity({
        type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.ModeloDeFiltroDePesquisa, idBtnSearch: guid(),
        tipoFiltroPesquisa: EnumCodigoFiltroPesquisa.IntegracoesComFalha,
        callbackRetornoPesquisa: function () {
            $("#" + _pesquisa.EtapaCarga.id).trigger("change");
            $("#" + _pesquisa.TipoIntegracao.id).trigger("change");
        }
    });

    this.ConfiguracaoModeloFiltroPesquisa = PropertyEntity({ eventClick: function (e) { abrirConfiguracaoModeloFiltroPesquisa(EnumCodigoFiltroPesquisa.IntegracoesComFalha, _pesquisa) }, type: types.event, text: Localization.Resources.Gerais.Geral.ConfiguracaoDeFiltros, visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function () {
            carregarGrid();
            limparGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
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
    this.ListaSelecionados = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
};

var Botoes = function () {
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(true), enable: ko.observable(true) });
    this.Acoes = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Ações", visible: ko.observable(true), enable: ko.observable(true) });
    this.Reenviar = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Reenviar", visible: ko.observable(true), enable: ko.observable(true), eventClick: reenviarIntegracoes });
    this.DownloadArquivos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Download arquivos", visible: ko.observable(true), enable: ko.observable(true), eventClick: downloadArquivos });
    this.Atualizar = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Atualizar", visible: ko.observable(true), enable: ko.observable(true), eventClick: atualizar });
}

var PesquisaHistoricoIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}
// #endregion Classes

function loadFiltroPesquisaIntegracoes() {
    var data = { TipoFiltro: EnumCodigoFiltroPesquisa.IntegracoesComFalha };

    executarReST("ModeloFiltroPesquisa/ObterFiltroPesquisaPadrao", data, function (res) {
        if (res.Success && Boolean(res.Data)) {
            PreencherJsonFiltroPesquisa(_pesquisa, res.Data.Dados);
            _pesquisa.ModeloFiltrosPesquisa.codEntity(res.Data.Codigo);
            _pesquisa.ModeloFiltrosPesquisa.val(res.Data.Descricao);

            if (_pesquisa.ModeloFiltrosPesquisa.callbackRetornoPesquisa instanceof Function)
                _pesquisa.ModeloFiltrosPesquisa.callbackRetornoPesquisa();
        }

        carregarGrid();
    });
}

function loadIntegracoesComFalha() {
    _pesquisa = new Pesquisa();
    KoBindings(_pesquisa, "knockoutPesquisa", false, _pesquisa.Pesquisar.id);

    _botoes = new Botoes();
    KoBindings(_botoes, "knoutBotoes");

    BuscarCargas(_pesquisa.CodigosCarga);

    loadGridIntegracoes();
    loadFiltroPesquisaIntegracoes();
    buscarIntegracoes();
}

function loadGridIntegracoes() {
    const opcaoReenviar = { descricao: "Reenviar", id: guid(), metodo: _botoes.Reenviar.eventClick, icone: "", visibilidade: true };
    const opcaoDownloadArquivos = { descricao: "Download de arquivos", id: guid(), metodo: downloadArquivos, icone: "", visibilidade: true };

    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [opcaoReenviar, opcaoDownloadArquivos], tamanho: 10 };

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
        url: "IntegracoesComFalha/ExportarPesquisa",
        titulo: "IntegracoesComFalha"
    };

    _gridIntegracoesComFalha = new GridViewExportacao(
        "grid-integracoes_falhas",
        "IntegracoesComFalha/Pesquisa",
        _pesquisa,
        menuOpcoes,
        configExportacao,
        null,
        null,
        multiplaEscolha
    );

    _gridIntegracoesComFalha.SetQuantidadeLinhasPorPagina(12);
    _gridIntegracoesComFalha.SetPermitirEdicaoColunas(true);
    _gridIntegracoesComFalha.SetSalvarPreferenciasGrid(true);
}

function buscarRowsSelecionadas() {
    var rowsSelecionadas = null;

    rowsSelecionadas = _gridIntegracoesComFalha.ObterMultiplosSelecionados();

    var codigosRowsSelecionadas = obterObjetoRowsSelecionadas(rowsSelecionadas);

    if (codigosRowsSelecionadas && (codigosRowsSelecionadas.length > 0 || _botoes.SelecionarTodos.val()))
        _pesquisa.ListaSelecionados.val(JSON.stringify(codigosRowsSelecionadas));
    else
        _pesquisa.ListaSelecionados.val("");
}

function obterObjetoRowsSelecionadas(rowsSelecionadas) {
    var codigos = new Array();

    for (var i = 0; i < rowsSelecionadas.length; i++) {
        var obj = criarObjetoRowSelecionada(rowsSelecionadas[i]);
        codigos.push(obj);
    }

    return codigos;
}

function criarObjetoRowSelecionada(linha) {
    var obj =
    {
        codigo: linha.Codigo,
        tabelaOrigem: linha.TabelaOrigem,
    }

    return obj;
}

function obterLinhaAtual(linha) {
    if (_pesquisa.ListaSelecionados.val().length == 0) {
        var obj = criarObjetoRowSelecionada(linha);
        var objToArray = [obj]; // o objeto passado precisa ser um array, se não o parse não funciona
        _pesquisa.ListaSelecionados.val(JSON.stringify(objToArray));
    }
}

function naoSelecionadoCallback() {
    buscarRowsSelecionadas();
}

function selecionadoCallback() {
    buscarRowsSelecionadas();
}

function carregarGrid() {
    _gridIntegracoesComFalha.CarregarGrid(function (grid) {
        var temDados = grid.data.length > 0;

        _botoes.SelecionarTodos.visible(temDados);
        _botoes.SelecionarTodos.val(false);
        _botoes.Acoes.visible(temDados);
        _botoes.Acoes.val(false);

        _gridIntegracoesComFalha.AtualizarRegistrosSelecionados([]);
        _gridIntegracoesComFalha.AtualizarRegistrosNaoSelecionados([]);

    });
}

function limparGrid() {
    _pesquisa.ListaSelecionados.val("");
}

function buscarIntegracoes() {
    return new Promise(function (resolve) {
        executarReST("TipoOperacao/BuscarIntegracoes", {}, function (retorno) {
            if (retorno.Success && retorno.Data) {
                var integracoes = retorno.Data.Integracoes.map(function (d) { return { value: d.Tipo, text: d.Descricao } });
                _pesquisa.TipoIntegracao.options(integracoes);
            }
            resolve();
        });
    });
}


function downloadArquivos(e) {
    obterLinhaAtual(e);
    executarDownload("IntegracoesComFalha/DownloadArquivosIntegracao", { ListaSelecionados: _pesquisa.ListaSelecionados.val() });
    limparGrid();
}

function reenviarIntegracoes(e) {
    exibirConfirmacao("Confirmação", "Realmente deseja realizar o reenvio?",
        function () {
            console.log(e);
            obterLinhaAtual(e);
            console.log(_pesquisa.ListaSelecionados.val());

            var data = { ListaSelecionados: _pesquisa.ListaSelecionados.val() };

            executarReST("IntegracoesComFalha/Reenviar", data, function (res) {
                if (res.Success) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, res.Mensagem);
                    _gridIntegracoesComFalha.CarregarGrid();
                }
                else
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Sucesso, res.Mensagem);

                limparGrid();
                _botoes.SelecionarTodos.val(false);
            });
        }, null);
}

function atualizar() {
    loadFiltroPesquisaIntegracoes();
}
