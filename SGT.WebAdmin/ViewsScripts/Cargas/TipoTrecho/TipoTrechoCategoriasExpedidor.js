/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/CategoriaPessoa.js" />

var _gridTipoTrechoCategoriasExpedidor;

var CategoriasExpedidor = function () {
    this.ListaCategoriasExpedidor = PropertyEntity({ type: types.map, required: false, text: "Adicionar Categoria Expedidor", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) });
}

function loadTipoTrechoCategoriasExpedidor() {
    _categoriasExpedidorTipoTrecho = new CategoriasExpedidor();
    KoBindings(_categoriasExpedidorTipoTrecho, "knockoutTipoTrechoCategoriasExpedidor");

    loadGridTipoTrechoCategoriasExpedidor();
}

function loadGridTipoTrechoCategoriasExpedidor() {
    var linhasPorPaginas = 5;
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerCategoriaExpedidor, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "75%", className: "text-align-left" }
    ];

    _gridTipoTrechoCategoriasExpedidor = new BasicDataTable(_categoriasExpedidorTipoTrecho.ListaCategoriasExpedidor.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

    new BuscarCategoriaPessoa(_categoriasExpedidorTipoTrecho.ListaCategoriasExpedidor, null, _gridTipoTrechoCategoriasExpedidor);
    _categoriasExpedidorTipoTrecho.ListaCategoriasExpedidor.basicTable = _gridTipoTrechoCategoriasExpedidor;

    _gridTipoTrechoCategoriasExpedidor.CarregarGrid([]);
}


function obterListaCategoriasExpedidor() {
    return _gridTipoTrechoCategoriasExpedidor.BuscarRegistros();
}

function obterCodigosCategoriasExpedidor() {
    var listaCategoriasExpedidor = obterListaCategoriasExpedidor();
    var listaCategoriasExpedidorRetornar = new Array();

    listaCategoriasExpedidor.forEach(function (setor) {
        listaCategoriasExpedidorRetornar.push(Number(setor.Codigo))
    });

    return JSON.stringify(listaCategoriasExpedidorRetornar);
}

function removerCategoriaExpedidor(registroSelecionado) {
    var listaCategoriasExpedidor = obterListaCategoriasExpedidor();

    for (var i = 0; i < listaCategoriasExpedidor.length; i++) {
        if (registroSelecionado.Codigo == listaCategoriasExpedidor[i].Codigo) {
            listaCategoriasExpedidor.splice(i, 1);
            break;
        }
    }

    _gridTipoTrechoCategoriasExpedidor.CarregarGrid(listaCategoriasExpedidor);
}

function recarregarGridTipoTrechoCategoriasExpedidor() {
    _gridTipoTrechoCategoriasExpedidor.CarregarGrid();
}