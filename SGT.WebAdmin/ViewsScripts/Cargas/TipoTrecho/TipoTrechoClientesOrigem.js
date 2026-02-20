/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />

var _gridTipoTrechoClientesOrigem;

var ClientesOrigem = function () {
    this.ListaClientesOrigem = PropertyEntity({ type: types.map, required: false, text: "Adicionar Clientes Origem", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) });
}

function loadTipoTrechoClientesOrigem() {
    _ClientesOrigemTipoTrecho = new ClientesOrigem();
    KoBindings(_ClientesOrigemTipoTrecho, "knockoutTipoTrechoClientesOrigem");

    loadGridTipoTrechoClientesOrigem();
}

function loadGridTipoTrechoClientesOrigem() {
    var linhasPorPaginas = 5;
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerModeloVeicular, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "75%", className: "text-align-left" }
    ];

    _gridTipoTrechoClientesOrigem = new BasicDataTable(_ClientesOrigemTipoTrecho.ListaClientesOrigem.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

    new BuscarClientes(_ClientesOrigemTipoTrecho.ListaClientesOrigem, null, null, null, null, _gridTipoTrechoClientesOrigem);
    _ClientesOrigemTipoTrecho.ListaClientesOrigem.basicTable = _gridTipoTrechoClientesOrigem;

    _gridTipoTrechoClientesOrigem.CarregarGrid([]);
}


function obterListaClientesOrigem() {
    return _gridTipoTrechoClientesOrigem.BuscarRegistros();
}

function obterCodigosClientesOrigem() {
    var listaClientesOrigem = obterListaClientesOrigem();
    var listaClientesOrigemRetornar = new Array();

    listaClientesOrigem.forEach(function (setor) {
        listaClientesOrigemRetornar.push(Number(setor.Codigo))
    });

    return JSON.stringify(listaClientesOrigemRetornar);
}

function removerModeloVeicular(registroSelecionado) {
    var listaClientesOrigem = obterListaClientesOrigem();

    for (var i = 0; i < listaClientesOrigem.length; i++) {
        if (registroSelecionado.Codigo == listaClientesOrigem[i].Codigo) {
            listaClientesOrigem.splice(i, 1);
            break;
        }
    }

    _gridTipoTrechoClientesOrigem.CarregarGrid(listaClientesOrigem);
}

function recarregarGridTipoTrechoClientesOrigem() {
    _gridTipoTrechoClientesOrigem.CarregarGrid();
}