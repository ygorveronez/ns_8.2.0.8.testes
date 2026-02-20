/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="ContratoFreteTransportador.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _filial;
var _gridFilial;

var Filial = function () {
    this.ContratoFreteTransportador = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Filiais = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), required: true, idGrid: guid() });
    this.Filiais.val.subscribe(function () {
        _contratoFreteTransportador.Filiais.val(JSON.stringify(_filial.Filiais.val()));
        RenderizarGridFilial();
    });

    this.Adicionar = PropertyEntity({ type: types.event, text: "Adicionar", visible: ko.observable(true), idBtnSearch: guid() });
}

//*******EVENTOS*******

function LoadFilial() {
    _filial = new Filial();
    KoBindings(_filial, "knockoutFiliais");

    LoadGridFilial();

    // Buscas
    new BuscarFilial(_filial.Adicionar, AdicionarFilialAGrid, _gridFilial);
}

function AdicionarFilialAGrid(data) {
    // Pega registros
    var dataGrid = _filial.Filiais.val();

    data.forEach(function (item) {
        // Objeto filial
        var obj = {
            Codigo: item.Codigo,
            Descricao: item.Descricao
        };

        // Adiciona a lista e atualiza a grid
        dataGrid.push(obj);
    });

    _filial.Filiais.val(dataGrid);
}

function RemoverFilialClick(data) {
    var dataGrid = _gridFilial.BuscarRegistros();

    for (var i = 0; i < dataGrid.length; i++) {
        if (data.Codigo == dataGrid[i].Codigo) {
            dataGrid.splice(i, 1);
            break;
        }
    }

    _filial.Filiais.val(dataGrid);
}

//*******MÉTODOS*******

function EditarFilial(data) {
    _filial.ContratoFreteTransportador.val(_contratoFreteTransportador.Codigo.val());

    _filial.Filiais.val(data.Filiais);
    RenderizarGridFilial();
}

function LimparCamposFilial() {
    LimparCampos(_filial);
    _filial.Filiais.val([]);
    RenderizarGridFilial();
}

function LoadGridFilial() {
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
                    RemoverFilialClick(data);
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
    _gridFilial = new BasicDataTable(_filial.Filiais.idGrid, header, menuOpcoes, null, null, 10);
    _gridFilial.CarregarGrid([]);
}

function RenderizarGridFilial() {
    var filials = _filial.Filiais.val();

    _gridFilial.CarregarGrid(filials);
}