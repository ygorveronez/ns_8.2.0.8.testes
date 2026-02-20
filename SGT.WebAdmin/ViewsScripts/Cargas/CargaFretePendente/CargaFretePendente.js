/*CargaFretePendente.js*/
/// <reference path="../../../js/Global/Buscas.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />

var _gridCargasPendentes;
var _pesquisaCargasPendentes;

/*
 * Declaração das Classes
 */

var PesquisaCargasPendentes = function () {
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            if (ValidarCamposObrigatorios(_pesquisaCargasPendentes)) {
                _pesquisaCargasPendentes.ExibirFiltros.visibleFade(false);
                _gridCargasPendentes.CarregarGrid();
            }
            else
                exibirMensagemCamposObrigatorio();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.DataInicial = PropertyEntity({ text: "Data Inicio: ", getType: typesKnockout.date, val: ko.observable(null), visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date, val: ko.observable(null), visible: ko.observable(true) });
    this.DataInicial.dateRangeLimit = this.DataFinalColeta;
    this.DataFinal.dateRangeInit = this.DataInicialColeta;
    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Nº da Carga", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo Operação", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Transportador", idBtnSearch: guid(), visible: ko.observable(true) });

    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Selecionar Todas", visible: ko.observable(true) });
    this.ProcessarSelecionados = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Reprocessar frete cargas selecionadas", eventClick: processarSelecionadosClick, visible: ko.observable(false) });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });

};


function loadPesquisaCargasPendentes() {
    _pesquisaCargasPendentes = new PesquisaCargasPendentes();
    KoBindings(_pesquisaCargasPendentes, "knockoutPesquisaCargas", false, _pesquisaCargasPendentes.Pesquisar.id);
}


function loadGridCargas() {
    var draggableRows = false;
    var draggableRows = false;
    var limiteRegistros = 50;
    var totalRegistrosPorPagina = 50;

    var multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        somenteLeitura: false,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        SelecionarTodosKnout: _pesquisaCargasPendentes.SelecionarTodos,
    };

    _pesquisaCargasPendentes.SelecionarTodos.val(false);

    _gridCargasPendentes = new GridView("grid-cargas_pendentes", "CargaFrete/PesquisaCargaFretePendente", _pesquisaCargasPendentes, null, null, totalRegistrosPorPagina, null, true, draggableRows, multiplaescolha, limiteRegistros, undefined, null);
    _gridCargasPendentes.CarregarGrid();
}

function loadCargaFretePendente() {
    loadPesquisaCargasPendentes();
    loadGridCargas();

    new BuscarFilial(_pesquisaCargasPendentes.Filial);
    new BuscarTiposOperacao(_pesquisaCargasPendentes.TipoOperacao);
    new BuscarTransportadores(_pesquisaCargasPendentes.Transportador);
}

function exibirMultiplasOpcoes() {
    _pesquisaCargasPendentes.ProcessarSelecionados.visible(false);

    var existemRegistrosSelecionados = _gridCargasPendentes.ObterMultiplosSelecionados().length > 0;

    if (existemRegistrosSelecionados) {
        _pesquisaCargasPendentes.ProcessarSelecionados.visible(true);
    }
}

function processarSelecionadosClick() {
    var cargasSelecionadas = null;

    exibirConfirmacao("Confirmação", "Você realmente deseja reprocessar o frete de todas as cargas selecionadas? ", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaCargasPendentes);
        dados.SelecionarTodos = _pesquisaCargasPendentes.SelecionarTodos.val();

        if (_pesquisaCargasPendentes.SelecionarTodos.val()) {
            cargasSelecionadas = _gridCargasPendentes.ObterMultiplosNaoSelecionados();
        } else {
            cargasSelecionadas = _gridCargasPendentes.ObterMultiplosSelecionados();
        }

        var codigosCargas = new Array();
        for (var i = 0; i < cargasSelecionadas.length; i++)
            codigosCargas.push(cargasSelecionadas[i].DT_RowId);

        if (codigosCargas && (codigosCargas.length > 0 || _pesquisaCargasPendentes.SelecionarTodos.val())) {
            dados.ItensSelecionados = JSON.stringify(codigosCargas);
        }

        console.log(dados);
        executarReST("CargaFrete/ReprocessarFreteCargas", dados, function (retorno) {
            if (retorno.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cargas enviadas para reprocessamento de frete");
                _gridCargasPendentes.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        })
    });

}

