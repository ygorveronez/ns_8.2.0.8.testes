/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />

var _gridTipoTrechoClientesDestino;

var ClientesDestino = function () {
    this.ListaClientesDestino = PropertyEntity({ type: types.map, required: false, text: "Adicionar Clientes Destino", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) });
}

function loadTipoTrechoClientesDestino() {
    _ClientesDestinoTipoTrecho = new ClientesDestino();
    KoBindings(_ClientesDestinoTipoTrecho, "knockoutTipoTrechoClientesDestino");

    loadGridTipoTrechoClientesDestino();
}

function loadGridTipoTrechoClientesDestino() {
    var linhasPorPaginas = 5;
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerModeloVeicular, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "75%", className: "text-align-left" }
    ];

    _gridTipoTrechoClientesDestino = new BasicDataTable(_ClientesDestinoTipoTrecho.ListaClientesDestino.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

    new BuscarClientes(_ClientesDestinoTipoTrecho.ListaClientesDestino, null, null, null, null, _gridTipoTrechoClientesDestino);
    _ClientesDestinoTipoTrecho.ListaClientesDestino.basicTable = _gridTipoTrechoClientesDestino;

    _gridTipoTrechoClientesDestino.CarregarGrid([]);
}


function obterListaClientesDestino() {
    return _gridTipoTrechoClientesDestino.BuscarRegistros();
}

function obterCodigosClientesDestino() {
    var listaClientesDestino = obterListaClientesDestino();
    var listaClientesDestinoRetornar = new Array();

    listaClientesDestino.forEach(function (setor) {
        listaClientesDestinoRetornar.push(Number(setor.Codigo))
    });

    return JSON.stringify(listaClientesDestinoRetornar);
}

function removerModeloVeicular(registroSelecionado) {
    var listaClientesDestino = obterListaClientesDestino();

    for (var i = 0; i < listaClientesDestino.length; i++) {
        if (registroSelecionado.Codigo == listaClientesDestino[i].Codigo) {
            listaClientesDestino.splice(i, 1);
            break;
        }
    }

    _gridTipoTrechoClientesDestino.CarregarGrid(listaClientesDestino);
}

function recarregarGridTipoTrechoClientesDestino() {
    _gridTipoTrechoClientesDestino.CarregarGrid();
}