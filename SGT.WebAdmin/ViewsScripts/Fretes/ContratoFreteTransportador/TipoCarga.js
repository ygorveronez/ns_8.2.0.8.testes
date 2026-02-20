/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="ContratoFreteTransportador.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _tipoCargaContrato;
var _gridTipoCargaContratoTipoCarga;

var TipoCargaContrato = function () {
    this.AdicionarTipoCarga = PropertyEntity({ type: types.event, text: "Adicionar", visible: ko.observable(true), idBtnSearch: guid() });
    this.TipoCarga = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), required: false, idGrid: guid()});
    this.TipoCarga.val.subscribe(function () {
        _contratoFreteTransportador.TipoCarga.val(JSON.stringify(_tipoCargaContrato.TipoCarga.val()));
        RenderizarGridTipoCargaContratoTipoCarga();
    });
}

//*******EVENTOS*******

function LoadTipoCargaContrato() {
    _tipoCargaContrato = new TipoCargaContrato();
    KoBindings(_tipoCargaContrato, "knockoutTipoCargaContrato");

    LoadGridsTipoCargaContrato();

    // Buscas
    new BuscarTiposdeCarga(_tipoCargaContrato.AdicionarTipoCarga, AdicionarTipoCargaContratoFactory("TipoCarga"), null, _gridTipoCargaContratoTipoCarga);
}

//*******MÉTODOS*******
function LoadGridTipoCargaContratoFactory(gridName, id, name) {
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
                    RemoverTipoCargaContratoFactory(window[gridName], name, data);
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


function AdicionarTipoCargaContratoFactory(name) {
    return function AdicionarTipoCargaContratoAGrid(data) {
        // Pega registros
        var dataGrid = _tipoCargaContrato[name].val();

        data.forEach(function (item) {
            // Objeto TipoCargaContrato
            var obj = {
                Codigo: item.Codigo,
                Descricao: item.Descricao
            };

            // Adiciona a lista e atualiza a grid
            dataGrid.push(obj);
        });

        _tipoCargaContrato[name].val(dataGrid);
    }
}

function RemoverTipoCargaContratoFactory(grid, name, data) {
    var dataGrid = grid.BuscarRegistros();

    for (var i = 0; i < dataGrid.length; i++) {
        if (data.Codigo == dataGrid[i].Codigo) {
            dataGrid.splice(i, 1);
            break;
        }
    }

    _tipoCargaContrato[name].val(dataGrid);
}

function EditarTipoCargaContrato(data) {
    _tipoCargaContrato.TipoCarga.val(data.TipoCarga);
    RenderizarGridsTipoCargaContrato();
}

function LimparCamposTipoCargaContrato() {
    LimparCampos(_tipoCargaContrato);
    _tipoCargaContrato.TipoCarga.val([]);
    RenderizarGridsTipoCargaContrato();
}

function LoadGridsTipoCargaContrato() {
    LoadGridTipoCargaContratoFactory("_gridTipoCargaContratoTipoCarga", _tipoCargaContrato.TipoCarga.idGrid, "TipoCarga");
}

function RenderizarGridsTipoCargaContrato() {
    RenderizarGridTipoCargaContratoTipoCarga();
}

function RenderizarGridTipoCargaContratoTipoCarga() {
    var data = _tipoCargaContrato.TipoCarga.val();
    _gridTipoCargaContratoTipoCarga.CarregarGrid(data);
}

