/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="ContratoFreteTransportador.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _franquia;
var _gridFranquiaTipoOperacao;
var _gridFranquiaTipoCarga;

var Franquia = function () {
    //this.AdicionarTipoOperacao = PropertyEntity({ type: types.event, text: "Adicionar", visible: ko.observable(true), idBtnSearch: guid() });
    //this.TipoOperacao = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), required: true, idGrid: guid()});
    //this.TipoOperacao.val.subscribe(function () {
    //    _contratoFreteTransportador.TipoOperacao.val(JSON.stringify(_franquia.TipoOperacao.val()));
    //    RenderizarGridFranquiaTipoOperacao();
    //});

    this.AdicionarTipoCarga = PropertyEntity({ type: types.event, text: "Adicionar", visible: ko.observable(true), idBtnSearch: guid() });
    this.TipoCarga = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), required: true, idGrid: guid() });
    this.TipoCarga.val.subscribe(function () {
        _contratoFreteTransportador.TipoCarga.val(JSON.stringify(_franquia.TipoCarga.val()));
        RenderizarGridFranquiaTipoCarga();
    });

    this.TotalPorCavalo = PropertyEntity({ text: "Total KM por Cavalo:", val: ko.observable("0"), def: "0", enable: ko.observable(true), type: types.map, getType: typesKnockout.int, eventChange: RecalcularFranquia });
    this.TotalPorCavalo.configInt.allowZero = true;

    this.TotalKm = PropertyEntity({ text: "Total KM:", val: ko.observable(0), def: 0, type: types.map, getType: typesKnockout.decimal, enable: false });
    this.TotalKm.val.subscribe(RenderizaResumoFranquia);

    this.ContratoMensal = PropertyEntity({ text: "Valor do Contrato Mensal:", val: ko.observable("0,00"), type: types.map, getType: typesKnockout.string, enable: false });
    this.ValorKm = PropertyEntity({ text: "Valor por KM:", val: ko.computed(CalculoValorKmFranquia, this), type: types.map, getType: typesKnockout.string, enable: false });
    this.ValorKm.val.subscribe(RenderizaResumoFranquia);

    this.ValorKmExcedente = PropertyEntity({ text: "Valor por KM Excedente:", val: ko.observable("0,00"), def: "0,00", enable: ko.observable(true), type: types.map, getType: typesKnockout.decimal });
    this.ValorKmExcedente.configDecimal.allowZero = true;
    this.ValorKmExcedente.val.subscribe(RenderizaResumoFranquia);

    this.ValorKmUtilizado = PropertyEntity({ text: "KM Consumido:", val: ko.observable("0"), def: "0", type: types.map, enable: false });
    this.ValorPago = PropertyEntity({ text: "Valor Pago:", val: ko.observable("0,00"), def: "0,00", type: types.map, enable: false });
}

//*******EVENTOS*******

function LoadFranquia() {
    _franquia = new Franquia();
    KoBindings(_franquia, "knockoutFranquia");

    LoadGridsFranquia();

    // Buscas
    //new BuscarTiposOperacao(_franquia.AdicionarTipoOperacao, AdicionarFranquiaFactory("TipoOperacao"), null, null, _gridFranquiaTipoOperacao);
    new BuscarTiposdeCarga(_franquia.AdicionarTipoCarga, AdicionarFranquiaFactory("TipoCarga"), null, _gridFranquiaTipoCarga);
}

//*******MÉTODOS*******
function LoadGridFranquiaFactory(gridName, id, name) {
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
                    RemoverFranquiaFactory(window[gridName], name, data);
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

function limparCamposFranquia() {
    $("#tabFranquiaFranquiaKM").click();
}

function AdicionarFranquiaFactory(name) {
    return function AdicionarFranquiaAGrid(data) {
        // Pega registros
        var dataGrid = _franquia[name].val();

        data.forEach(function (item) {
            // Objeto Franquia
            var obj = {
                Codigo: item.Codigo,
                Descricao: item.Descricao
            };

            // Adiciona a lista e atualiza a grid
            dataGrid.push(obj);
        });

        _franquia[name].val(dataGrid);
    }
}

function RemoverFranquiaFactory(grid, name, data) {
    var dataGrid = grid.BuscarRegistros();

    for (var i = 0; i < dataGrid.length; i++) {
        if (data.Codigo == dataGrid[i].Codigo) {
            dataGrid.splice(i, 1);
            break;
        }
    }

    _franquia[name].val(dataGrid);
}

function EditarFranquia(data) {
    _franquia.TipoCarga.val(data.TipoCarga);
    //_franquia.TipoOperacao.val(data.TipoOperacao);
    RenderizarGridsFranquia();
    RecalcularFranquia();
}

function LimparCamposFranquia() {
    LimparCampos(_franquia);
    _franquia.TipoCarga.val([]);
    //_franquia.TipoOperacao.val([]);
    RenderizarGridsFranquia();
}

function LoadGridsFranquia() {
    //LoadGridFranquiaFactory("_gridFranquiaTipoOperacao", _franquia.TipoOperacao.idGrid, "TipoOperacao");
    LoadGridFranquiaFactory("_gridFranquiaTipoCarga", _franquia.TipoCarga.idGrid, "TipoCarga");
}

function RenderizarGridsFranquia() {
    //RenderizarGridFranquiaTipoOperacao();
    RenderizarGridFranquiaTipoCarga();
}

//function RenderizarGridFranquiaTipoOperacao() {
//    var data = _franquia.TipoOperacao.val();
//    _gridFranquiaTipoOperacao.CarregarGrid(data);
//}

function RenderizarGridFranquiaTipoCarga() {
    var data = _franquia.TipoCarga.val();
    _gridFranquiaTipoCarga.CarregarGrid(data);
}

function RecalcularFranquia() {
    var arrayAcordosTodasAbas = _contratoFreteTransportador.Acordos.list.slice();
    var quantidadeDezenasMes = _acordoConfiguracao.QuantidadeAbas.val();

    var quantidadeTotalVeiculos = arrayAcordosTodasAbas
        .filter(function (item) { return item.FranquiaPorKm.val}) // Filtra apenas o que sao franquia SIM
        .map(function (item) { return _ParseIntHelper(item.Quantidade.val)}) // Converte quantidade
        .reduce(function (a, b) {return a + b }, 0); // Sumariza

    var totalKm = (_ParseIntHelper(_franquia.TotalPorCavalo.val()) * quantidadeTotalVeiculos) / quantidadeDezenasMes;
    _franquia.TotalKm.val(_ParseIntHelper(totalKm.toString().replace(".", ",")));
}

function CalculoValorKmFranquia() {
    var contrato = _ParseFloatHelper(this.ContratoMensal.val()) || 0;
    var total = _ParseIntHelper(this.TotalKm.val()) || 0;
    var totalFranquia = (total > 0) ? (contrato / total) : 0;

    return _FormatHelper(totalFranquia);
}
