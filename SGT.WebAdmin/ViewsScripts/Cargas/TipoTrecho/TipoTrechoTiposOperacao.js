/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />

var _gridTipoTrechoTiposOperacao;

var TiposOperacao = function () {
    this.ListaTiposOperacao = PropertyEntity({ type: types.map, required: false, text: "Adicionar Tipo Operacao", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) });
}

function loadTipoTrechoTiposOperacao() {

    _tiposOperacaoTipoTrecho = new TiposOperacao();
    KoBindings(_tiposOperacaoTipoTrecho, "knockoutTipoTrechoTiposOperacao");

    loadGridTipoTrechoTiposOperacao();
}

function loadGridTipoTrechoTiposOperacao() {
    var linhasPorPaginas = 5;
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerTipoOperacao, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "75%", className: "text-align-left" }
    ];

    _gridTipoTrechoTiposOperacao = new BasicDataTable(_tiposOperacaoTipoTrecho.ListaTiposOperacao.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

    new BuscarTiposOperacao(_tiposOperacaoTipoTrecho.ListaTiposOperacao, null, null, null, _gridTipoTrechoTiposOperacao);
    _tiposOperacaoTipoTrecho.ListaTiposOperacao.basicTable = _gridTipoTrechoTiposOperacao;

    _gridTipoTrechoTiposOperacao.CarregarGrid([]);
}


function obterListaTiposOperacao() {
    return _gridTipoTrechoTiposOperacao.BuscarRegistros();
}

function obterCodigosTiposOperacao() {
    var listaTiposOperacao = obterListaTiposOperacao();
    var listaTiposOperacaoRetornar = new Array();

    listaTiposOperacao.forEach(function (setor) {
        listaTiposOperacaoRetornar.push(Number(setor.Codigo))
    });

    return JSON.stringify(listaTiposOperacaoRetornar);
}

function removerTipoOperacao(registroSelecionado) {
    var listaTiposOperacao = obterListaTiposOperacao();

    for (var i = 0; i < listaTiposOperacao.length; i++) {
        if (registroSelecionado.Codigo == listaTiposOperacao[i].Codigo) {
            listaTiposOperacao.splice(i, 1);
            break;
        }
    }

    _gridTipoTrechoTiposOperacao.CarregarGrid(listaTiposOperacao);
}

function recarregarGridTipoTrechoTiposOperacao() {
    _gridTipoTrechoTiposOperacao.CarregarGrid();
}