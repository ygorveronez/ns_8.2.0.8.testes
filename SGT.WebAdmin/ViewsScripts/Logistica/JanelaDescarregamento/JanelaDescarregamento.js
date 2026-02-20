/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/CentrosDescarregamento.js" />
/// <reference path="../../Enumeradores/EnumSituacaoJanelaDescarregamento.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _pesquisaJanelasDescarregamentos;
var _gridJanelasDescarregamentos;

/*
 * Declaração das Classes
 */

var PesquisaJanelasDescarregamentos = function () {
    this.CentroDescarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Descarregamento:", idBtnSearch: guid() });
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFim = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoJanelaDescarregamento.Todas), options: EnumSituacaoJanelaDescarregamento.obterOpcoesPesquisa(), def: EnumSituacaoJanelaDescarregamento.Todas, text: "Situação: " });

    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridJanelasDescarregamentos.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridJanelaDescarregamento() {
    var opcaoDetalhes = { descricao: "Detalhes", id: "clasEditar", evento: "onclick", metodo: function () { }, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [] };

    _gridJanelasDescarregamentos = new GridView(_pesquisaJanelasDescarregamentos.Pesquisar.idGrid, "JanelaDescarregamento/Pesquisa", _pesquisaJanelasDescarregamentos, menuOpcoes, null, 25);
    _gridJanelasDescarregamentos.CarregarGrid();
}

function loadJanelaDescarregamento() {
    _pesquisaJanelasDescarregamentos = new PesquisaJanelasDescarregamentos();
    KoBindings(_pesquisaJanelasDescarregamentos, "knockoutPesquisaJanelasDescarregamentos");

    new BuscarCentrosDescarregamento(_pesquisaJanelasDescarregamentos.CentroDescarregamento);

    loadGridJanelaDescarregamento();
}
