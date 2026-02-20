/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/CategoriaPessoa.js" />

var _gridTipoTrechoCategoriasRecebedor;

var CategoriasRecebedor = function () {
    this.ListaCategoriasRecebedor = PropertyEntity({ type: types.map, required: false, text: "Adicionar Categoria Recebedor", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) });
}

function loadTipoTrechoCategoriasRecebedor() {
    _categoriasRecebedorTipoTrecho = new CategoriasRecebedor();
    KoBindings(_categoriasRecebedorTipoTrecho, "knockoutTipoTrechoCategoriasRecebedor");

    loadGridTipoTrechoCategoriasRecebedor();
}

function loadGridTipoTrechoCategoriasRecebedor() {
    var linhasPorPaginas = 5;
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerCategoriaRecebedor, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "75%", className: "text-align-left" }
    ];

    _gridTipoTrechoCategoriasRecebedor = new BasicDataTable(_categoriasRecebedorTipoTrecho.ListaCategoriasRecebedor.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

    new BuscarCategoriaPessoa(_categoriasRecebedorTipoTrecho.ListaCategoriasRecebedor, null, _gridTipoTrechoCategoriasRecebedor);
    _categoriasRecebedorTipoTrecho.ListaCategoriasRecebedor.basicTable = _gridTipoTrechoCategoriasRecebedor;

    _gridTipoTrechoCategoriasRecebedor.CarregarGrid([]);
}


function obterListaCategoriasRecebedor() {
    return _gridTipoTrechoCategoriasRecebedor.BuscarRegistros();
}

function obterCodigosCategoriasRecebedor() {
    var listaCategoriasRecebedor = obterListaCategoriasRecebedor();
    var listaCategoriasRecebedorRetornar = new Array();

    listaCategoriasRecebedor.forEach(function (setor) {
        listaCategoriasRecebedorRetornar.push(Number(setor.Codigo))
    });

    return JSON.stringify(listaCategoriasRecebedorRetornar);
}

function removerCategoriaRecebedor(registroSelecionado) {
    var listaCategoriasRecebedor = obterListaCategoriasRecebedor();

    for (var i = 0; i < listaCategoriasRecebedor.length; i++) {
        if (registroSelecionado.Codigo == listaCategoriasRecebedor[i].Codigo) {
            listaCategoriasRecebedor.splice(i, 1);
            break;
        }
    }

    _gridTipoTrechoCategoriasRecebedor.CarregarGrid(listaCategoriasRecebedor);
}

function recarregarGridTipoTrechoCategoriasRecebedor() {
    _gridTipoTrechoCategoriasRecebedor.CarregarGrid();
}