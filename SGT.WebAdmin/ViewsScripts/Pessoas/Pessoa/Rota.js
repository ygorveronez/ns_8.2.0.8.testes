/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="ContratoFreteTransportador.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _rota;
var _gridRota;

var Rota = function () {
    this.Pessoa = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Rota = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), required: true, idGrid: guid() });
    this.Rota.val.subscribe(function () {
        _pessoa.Rota.val(JSON.stringify(_rota.Rota.val().map(function (r) { return r.Codigo; })));
        RenderizarGridRota();
    });

    this.Adicionar = PropertyEntity({ type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), idBtnSearch: guid() });
}

//*******EVENTOS*******

function LoadRota() {
    _rota = new Rota();
    KoBindings(_rota, "knockoutRota");

    LoadGridRota();

    // Buscas
    new BuscarRotasFrete(_rota.Adicionar, AdicionarRotaAGrid, _gridRota);
}

function AdicionarRotaAGrid(data) {
    var dataGrid = _rota.Rota.val();

    data.forEach(function (r) {
        var rota = {
            Codigo: r.Codigo,
            Descricao: r.Descricao,
        };

        dataGrid.push(rota);
    });

    _rota.Rota.val(dataGrid);
}

function RemoverRotaClick(data) {
    var dataGrid = _gridRota.BuscarRegistros();

    for (var i = 0; i < dataGrid.length; i++) {
        if (data.Codigo == dataGrid[i].Codigo) {
            dataGrid.splice(i, 1);
            break;
        }
    }

    _rota.Rota.val(dataGrid);
}


//*******MÉTODOS*******
function ListarRotas(data) {
    _rota.Pessoa.val(_pessoa.Codigo.val());

    _rota.Rota.val(data.Rota);
    RenderizarGridRota();
}

function LimparCamposRota() {
    LimparCampos(_rota);
    _rota.Rota.val([]);
    RenderizarGridRota();
}

function LoadGridRota() {
    //-- Grid
    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [
            {
                descricao: Localization.Resources.Gerais.Geral.Excluir,
                id: guid(),
                evento: "onclick",
                tamanho: "10",
                icone: "",
                metodo: function (data) {
                    RemoverRotaClick(data);
                }
            }
        ]
    };

    // Cabecalho
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "90%", className: "text-align-left" },
    ];

    // Grid
    _gridRota = new BasicDataTable(_rota.Rota.idGrid, header, menuOpcoes, null, null, 10);
    _gridRota.CarregarGrid([]);
}

function RenderizarGridRota() {
    var Rota = _rota.Rota.val();

    _gridRota.CarregarGrid(Rota);
}