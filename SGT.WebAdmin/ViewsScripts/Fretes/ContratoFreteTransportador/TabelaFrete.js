
/// <reference path="../../Consultas/TabelaFrete.js" />

//#region Variaveis Globais
var _tabelaFreteContrato;
var _gridTabelaFrete;
//#endregion

//#region Construtores
var TabelaFreteContrato = function () {
    this.AdicionarTabelaFrete = PropertyEntity({ type: types.event, text: "Adicionar Tabela", visible: ko.observable(true), idBtnSearch: guid() });
    this.TabelasFrete = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), required: false, idGrid: guid() });
}

//#endregion

//#region Funções Inicializadoras
function LoadTabelaFreteContrato() {
    _tabelaFreteContrato = new TabelaFreteContrato();
    KoBindings(_tabelaFreteContrato, "knockoutTabelaFreteContrato");

    loadGridTabelaFreteContrato();

    new BuscarTabelasDeFrete(_tabelaFreteContrato.AdicionarTabelaFrete, null, null, _gridTabelaFrete);
}

function loadGridTabelaFreteContrato() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [
            {
                descricao: "Excluir",
                id: guid(),
                evento: "onclick",
                tamanho: "10",
                icone: "",
                metodo: removerItemGridTabela
            }
        ]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%", className: "text-align-left" },
    ];
    _gridTabelaFrete = new BasicDataTable(_tabelaFreteContrato.TabelasFrete.idGrid, header, menuOpcoes, null, null, 10);
    CarregarListaGridTabela();
}
//#endregion

//#region Funções Auxiliares
function removerItemGridTabela(e) {
    let listaRegistrosGrid = ObterListRegistrosGrid();
    for (var i = 0; i < listaRegistrosGrid.length; i++) {
        if (e.Codigo != listaRegistrosGrid[i].Codigo)
            continue;

        listaRegistrosGrid.splice(i, 1);
        break;
    }
    CarregarListaGridTabela(listaRegistrosGrid);
}

function ObterListRegistrosGrid() {
    return _gridTabelaFrete.BuscarRegistros();
}

function CarregarListaGridTabela(registros = []) {
    return _gridTabelaFrete.CarregarGrid(registros);
}

function LimparTabelasFreteCliente() {
    CarregarListaGridTabela();
    _contratoFreteTransportador.TabelasFrete.val("");
}

function ObterValorTabelaFrete() {
    return _contratoFreteTransportador.TabelasFrete.val(JSON.stringify(ObterListRegistrosGrid()));
}
//#endregion