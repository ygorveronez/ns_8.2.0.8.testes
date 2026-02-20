/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />

var _gridTipoTrechoModelosVeiculares;

var ModelosVeiculares = function () {
    this.ListaModelosVeiculares = PropertyEntity({ type: types.map, required: false, text: "Adicionar Modelo Veicular", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) });
}

function loadTipoTrechoModelosVeiculares() {
    _modelosVeicularesTipoTrecho = new ModelosVeiculares();
    KoBindings(_modelosVeicularesTipoTrecho, "knockoutTipoTrechoModelosVeiculares");

    loadGridTipoTrechoModelosVeiculares();
}

function loadGridTipoTrechoModelosVeiculares() {
    var linhasPorPaginas = 5;
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerModeloVeicular, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "75%", className: "text-align-left" }
    ];

    _gridTipoTrechoModelosVeiculares = new BasicDataTable(_modelosVeicularesTipoTrecho.ListaModelosVeiculares.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

    new BuscarModelosVeicularesCarga(_modelosVeicularesTipoTrecho.ListaModelosVeiculares, null, null, null, null, null, _gridTipoTrechoModelosVeiculares);
    _modelosVeicularesTipoTrecho.ListaModelosVeiculares.basicTable = _gridTipoTrechoModelosVeiculares;

    _gridTipoTrechoModelosVeiculares.CarregarGrid([]);
}


function obterListaModelosVeiculares() {
    return _gridTipoTrechoModelosVeiculares.BuscarRegistros();
}

function obterCodigosModelosVeiculares() {
    var listaModelosVeiculares = obterListaModelosVeiculares();
    var listaModelosVeicularesRetornar = new Array();

    listaModelosVeiculares.forEach(function (setor) {
        listaModelosVeicularesRetornar.push(Number(setor.Codigo))
    });

    return JSON.stringify(listaModelosVeicularesRetornar);
}

function removerModeloVeicular(registroSelecionado) {
    var listaModelosVeiculares = obterListaModelosVeiculares();

    for (var i = 0; i < listaModelosVeiculares.length; i++) {
        if (registroSelecionado.Codigo == listaModelosVeiculares[i].Codigo) {
            listaModelosVeiculares.splice(i, 1);
            break;
        }
    }

    _gridTipoTrechoModelosVeiculares.CarregarGrid(listaModelosVeiculares);
}

function recarregarGridTipoTrechoModelosVeiculares() {
    _gridTipoTrechoModelosVeiculares.CarregarGrid();
}