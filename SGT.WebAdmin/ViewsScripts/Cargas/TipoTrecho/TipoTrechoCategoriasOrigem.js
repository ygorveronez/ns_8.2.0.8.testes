/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/CategoriaPessoa.js" />

var _gridTipoTrechoCategoriasOrigem;

var CategoriasOrigem = function () {
    this.ListaCategoriasOrigem = PropertyEntity({ type: types.map, required: false, text: "Adicionar Categoria Origem", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) });
}

function loadTipoTrechoCategoriasOrigem() {
    _categoriasOrigemTipoTrecho = new CategoriasOrigem();
    KoBindings(_categoriasOrigemTipoTrecho, "knockoutTipoTrechoCategoriasOrigem");

    loadGridTipoTrechoCategoriasOrigem();
}

function loadGridTipoTrechoCategoriasOrigem() {
    var linhasPorPaginas = 5;
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerCategoriaOrigem, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "75%", className: "text-align-left" }
    ];

    _gridTipoTrechoCategoriasOrigem = new BasicDataTable(_categoriasOrigemTipoTrecho.ListaCategoriasOrigem.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

    new BuscarCategoriaPessoa(_categoriasOrigemTipoTrecho.ListaCategoriasOrigem, null, _gridTipoTrechoCategoriasOrigem);
    _categoriasOrigemTipoTrecho.ListaCategoriasOrigem.basicTable = _gridTipoTrechoCategoriasOrigem;

    _gridTipoTrechoCategoriasOrigem.CarregarGrid([]);
}


function obterListaCategoriasOrigem() {
    return _gridTipoTrechoCategoriasOrigem.BuscarRegistros();
}

function obterCodigosCategoriasOrigem() {
    var listaCategoriasOrigem = obterListaCategoriasOrigem();
    var listaCategoriasOrigemRetornar = new Array();

    listaCategoriasOrigem.forEach(function (setor) {
        listaCategoriasOrigemRetornar.push(Number(setor.Codigo))
    });

    return JSON.stringify(listaCategoriasOrigemRetornar);
}

function removerCategoriaOrigem(registroSelecionado) {
    var listaCategoriasOrigem = obterListaCategoriasOrigem();

    for (var i = 0; i < listaCategoriasOrigem.length; i++) {
        if (registroSelecionado.Codigo == listaCategoriasOrigem[i].Codigo) {
            listaCategoriasOrigem.splice(i, 1);
            break;
        }
    }

    _gridTipoTrechoCategoriasOrigem.CarregarGrid(listaCategoriasOrigem);
}

function recarregarGridTipoTrechoCategoriasOrigem() {
    _gridTipoTrechoCategoriasOrigem.CarregarGrid();
}