/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="ContratoFreteTransportador.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _canalEntrega;
var _gridCanalEntregaContrato;

var CanalEntregaContrato = function () {
    this.AdicionarCanalEntrega = PropertyEntity({ type: types.event, text: "Adicionar Canal Entrega", visible: ko.observable(true), idBtnSearch: guid() });
    this.CanalEntrega = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), required: false, idGrid: guid() });
    this.CanalEntrega.val.subscribe(function () {
        _contratoFreteTransportador.CanalEntrega.val(JSON.stringify(_canalEntrega.CanalEntrega.val()));
        RenderizarGridCanalEntregaContrato();
    });
}

//*******EVENTOS*******

function LoadCanalEntregaContrato() {
    _canalEntrega = new CanalEntregaContrato();
    KoBindings(_canalEntrega, "knockoutCanalEntregaContrato");

    LoadGridsCanalEntregaContrato();

    new BuscarCanaisEntrega(_canalEntrega.AdicionarCanalEntrega, AdicionarCanalEntregaContrato("CanalEntrega"), _gridCanalEntregaContrato);
}

//*******MÉTODOS*******
function LoadGridCanalEntregaContratoFactory(gridName, id, name) {
    //-- Grid
    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [
            {
                descricao: "Excluir",
                id: guid(),
                evento: "onclick",
                tamanho: "10",
                icone: "",
                metodo: function (data) {
                    if (_CAMPOS_BLOQUEADOS) return;
                    RemoverCanalEntregaContrato(window[gridName], name, data);
                }
            }
        ]
    };

    // Cabecalho
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%", className: "text-align-left" },
    ];

    // Grid
    window[gridName] = new BasicDataTable(id, header, menuOpcoes, null, null, 10);
    window[gridName].CarregarGrid([]);
}


function AdicionarCanalEntregaContrato(name) {
    return function AdicionarCanalEntregaContratoAGrid(data) {
        var dataGrid = _canalEntrega[name].val();

        data.forEach(function (item) {
            var obj = {
                Codigo: item.Codigo,
                Descricao: item.Descricao
            };

            // Adiciona a lista e atualiza a grid
            dataGrid.push(obj);
        });

        _canalEntrega[name].val(dataGrid);
    }
}

function RemoverCanalEntregaContrato(grid, name, data) {
    var dataGrid = grid.BuscarRegistros();

    for (var i = 0; i < dataGrid.length; i++) {
        if (data.Codigo == dataGrid[i].Codigo) {
            dataGrid.splice(i, 1);
            break;
        }
    }

    _canalEntrega[name].val(dataGrid);
}

function EditarCanalEntregaContrato(data) {
    _canalEntrega.CanalEntrega.val(data.CanalEntrega);
    RenderizarGridsCanalEntregaContrato();
}

function LimparCamposCanalEntregaContrato() {
    LimparCampos(_canalEntrega);
    _canalEntrega.CanalEntrega.val([]);
    RenderizarGridsCanalEntregaContrato();
}

function LoadGridsCanalEntregaContrato() {
    LoadGridCanalEntregaContratoFactory("_gridCanalEntregaContrato", _canalEntrega.CanalEntrega.idGrid, "CanalEntrega");
}

function RenderizarGridsCanalEntregaContrato() {
    RenderizarGridCanalEntregaContrato();
}

function RenderizarGridCanalEntregaContrato() {
    var data = _canalEntrega.CanalEntrega.val();
    _gridCanalEntregaContrato.CarregarGrid(data);
}

