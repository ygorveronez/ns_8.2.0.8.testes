/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/CategoriaPessoa.js" />

var _gridTipoTrechoCategoriasDestino;

var CategoriasDestino = function () {
    this.ListaCategoriasDestino = PropertyEntity({ type: types.map, required: false, text: "Adicionar Categoria Destino", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) });
}

function loadTipoTrechoCategoriasDestino() {
    _categoriasDestinoTipoTrecho = new CategoriasDestino();
    KoBindings(_categoriasDestinoTipoTrecho, "knockoutTipoTrechoCategoriasDestino");

    loadGridTipoTrechoCategoriasDestino();
}

function loadGridTipoTrechoCategoriasDestino() {
    var linhasPorPaginas = 5;
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerCategoriaDestino, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "75%", className: "text-align-left" }
    ];

    _gridTipoTrechoCategoriasDestino = new BasicDataTable(_categoriasDestinoTipoTrecho.ListaCategoriasDestino.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

    new BuscarCategoriaPessoa(_categoriasDestinoTipoTrecho.ListaCategoriasDestino, null, _gridTipoTrechoCategoriasDestino);
    _categoriasDestinoTipoTrecho.ListaCategoriasDestino.basicTable = _gridTipoTrechoCategoriasDestino;

    _gridTipoTrechoCategoriasDestino.CarregarGrid([]);
}


function obterListaCategoriasDestino() {
    return _gridTipoTrechoCategoriasDestino.BuscarRegistros();
}

function obterCodigosCategoriasDestino() {
    var listaCategoriasDestino = obterListaCategoriasDestino();
    var listaCategoriasDestinoRetornar = new Array();

    listaCategoriasDestino.forEach(function (setor) {
        listaCategoriasDestinoRetornar.push(Number(setor.Codigo))
    });

    return JSON.stringify(listaCategoriasDestinoRetornar);
}

function removerCategoriaDestino(registroSelecionado) {
    var listaCategoriasDestino = obterListaCategoriasDestino();

    for (var i = 0; i < listaCategoriasDestino.length; i++) {
        if (registroSelecionado.Codigo == listaCategoriasDestino[i].Codigo) {
            listaCategoriasDestino.splice(i, 1);
            break;
        }
    }

    _gridTipoTrechoCategoriasDestino.CarregarGrid(listaCategoriasDestino);
}

function recarregarGridTipoTrechoCategoriasDestino() {
    _gridTipoTrechoCategoriasDestino.CarregarGrid();
}