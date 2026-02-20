/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="ContratoFreteTransportador.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _valoresVeiculos;
var _gridValoresVeiculos;

var ValoresVeiculos = function () {
    this.ContratoFreteTransportador = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.ModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Modelo Veicular:", idBtnSearch: guid(), enable: ko.observable(true), required: true });
    this.ValorDiaria = PropertyEntity({ text: "Valor Diária:", type: types.map, getType: typesKnockout.decimal, enable: ko.observable(true) });
    this.ValorQuinzena = PropertyEntity({ text: "Valor Quinzena:", type: types.map, getType: typesKnockout.decimal, enable: ko.observable(true) });

    this.ValoresVeiculos = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), required: true, idGrid: guid() });
    this.ValoresVeiculos.val.subscribe(function () {
        _contratoFreteTransportador.ValoresVeiculos.val(JSON.stringify(_valoresVeiculos.ValoresVeiculos.val()));
        RenderizarGridValoresVeiculos();
    });

    this.Adicionar = PropertyEntity({ type: types.event, eventClick: AdicionarValoresVeiculos, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true), idBtnSearch: guid() });
}

//*******EVENTOS*******

function LoadValoresVeiculos() {
    _valoresVeiculos = new ValoresVeiculos();
    KoBindings(_valoresVeiculos, "knockoutValoresVeiculos");

    LoadGridValoresVeiculos();

    // Buscas
    new BuscarModelosVeicularesCarga(_valoresVeiculos.ModeloVeicular);
}

function RemoverValorVeiculoClick(data) {
    var dataGrid = _valoresVeiculos.ValoresVeiculos.val();

    for (var i = 0; i < dataGrid.length; i++) {
        if (data.Codigo == dataGrid[i].Codigo) {
            dataGrid.splice(i, 1);
            break;
        }
    }

    _valoresVeiculos.ValoresVeiculos.val(dataGrid);
}

//*******MÉTODOS*******
function AdicionarValoresVeiculos() {
    if (!ValidaVeiculoValor())
        return;

    var dataGrid = _valoresVeiculos.ValoresVeiculos.val();

    var valorVeiculo = {
        Codigo: _valoresVeiculos.ModeloVeicular.codEntity(),
        ModeloVeicular: _valoresVeiculos.ModeloVeicular.val(),
        ValorDiaria: _valoresVeiculos.ValorDiaria.val(),
        ValorQuinzena: _valoresVeiculos.ValorQuinzena.val(),
    };

    dataGrid.push(valorVeiculo);
    _valoresVeiculos.ValoresVeiculos.val(dataGrid);

    LimparCamposAdicionarValoresVeiculos();
}

function ValidaVeiculoValor() {
    var msg = "";

    if (_valoresVeiculos.ModeloVeicular.val() == "" || _valoresVeiculos.ModeloVeicular.codEntity() == 0)
        msg = "Modelo Veicular é obrigatório";

    var _ValorDiaria = (_valoresVeiculos.ValorDiaria.val() != "") ? Globalize.parseFloat(_valoresVeiculos.ValorDiaria.val()) : 0;
    var _ValorQuinzena = (_valoresVeiculos.ValorQuinzena.val() != "") ? Globalize.parseFloat(_valoresVeiculos.ValorQuinzena.val()) : 0;
    
    if (_ValorDiaria == 0 && _ValorQuinzena == 0)
        msg = "É preciso informar um dos campos de valor";

    var dataGrid = _valoresVeiculos.ValoresVeiculos.val();
    for (var i = 0; i < dataGrid.length; i++) {
        if (dataGrid[i].Codigo == _valoresVeiculos.ModeloVeicular.codEntity()) {
            msg = "Já existe uma configuração com o Modelo Veicular informado";
            break;
        }
    }

    if (msg != "")
        exibirMensagem(tipoMensagem.atencao, "Configuração Inválida", msg);

    return msg == "";
}

function LoadGridValoresVeiculos() {
    //-- Grid
    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [
            {
                descricao: "Excluir",
                id: guid(),
                evento: "onclick",
                tamanho: "25",
                icone: "",
                metodo: function (data) {
                    if (_CAMPOS_BLOQUEADOS) return;
                    RemoverValorVeiculoClick(data);
                }
            }
        ]
    };

    // Cabecalho
    var header = [
        { data: "Codigo", visible: false },
        { data: "ModeloVeicular", title: "Modelo", width: "25%", className: "text-align-right" },
        { data: "ValorDiaria", title: "Valor Diária", width: "25%", className: "text-align-left" },
        { data: "ValorQuinzena", title: "Valor Quinzena", width: "25%", className: "text-align-left" },
    ];

    // Grid
    _gridValoresVeiculos = new BasicDataTable(_valoresVeiculos.ValoresVeiculos.idGrid, header, menuOpcoes, null, null, 10);
    _gridValoresVeiculos.CarregarGrid([]);
}

function RenderizarGridValoresVeiculos() {
    var itens = _valoresVeiculos.ValoresVeiculos.val();

    _gridValoresVeiculos.CarregarGrid(itens);
}

function EditarValoresVeiculos(data) {
    _valoresVeiculos.ContratoFreteTransportador.val(_contratoFreteTransportador.Codigo.val());

    _valoresVeiculos.ValoresVeiculos.val(data.ValoresVeiculos);
}

function LimparCamposValoresVeiculos() {
    LimparCampos(_valoresVeiculos);
    _valoresVeiculos.ValoresVeiculos.val([]);
    RenderizarGridValoresVeiculos();
}

function LimparCamposAdicionarValoresVeiculos() {
    _valoresVeiculos.ModeloVeicular.val("");
    _valoresVeiculos.ModeloVeicular.codEntity(0);
    _valoresVeiculos.ValorQuinzena.val("");
    _valoresVeiculos.ValorDiaria.val("");
}