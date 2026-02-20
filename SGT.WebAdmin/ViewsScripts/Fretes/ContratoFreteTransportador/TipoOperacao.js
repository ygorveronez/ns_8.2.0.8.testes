/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="ContratoFreteTransportador.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _tipoOperacaoContrato;
var _gridTipoOperacaoContratoTipoOperacao;
var _gridTipoOperacaoContratoTipoCarga;

var TipoOperacaoContrato = function () {
    this.AdicionarTipoOperacao = PropertyEntity({ type: types.event, text: "Adicionar", visible: ko.observable(true), idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), required: true, idGrid: guid()});
    this.TipoOperacao.val.subscribe(function () {
        _contratoFreteTransportador.TipoOperacao.val(JSON.stringify(_tipoOperacaoContrato.TipoOperacao.val()));
        RenderizarGridTipoOperacaoContratoTipoOperacao();
    });
}

//*******EVENTOS*******

function LoadTipoOperacaoContrato() {
    _tipoOperacaoContrato = new TipoOperacaoContrato();
    KoBindings(_tipoOperacaoContrato, "knockoutTipoOperacaoContrato");

    LoadGridsTipoOperacaoContrato();

    // Buscas
    new BuscarTiposOperacao(_tipoOperacaoContrato.AdicionarTipoOperacao, AdicionarTipoOperacaoContratoFactory("TipoOperacao"), null, null, _gridTipoOperacaoContratoTipoOperacao);
}

//*******MÉTODOS*******
function LoadGridTipoOperacaoContratoFactory(gridName, id, name) {
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
                    RemoverTipoOperacaoContratoFactory(window[gridName], name, data);
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


function AdicionarTipoOperacaoContratoFactory(name) {
    return function AdicionarTipoOperacaoContratoAGrid(data) {
        // Pega registros
        var dataGrid = _tipoOperacaoContrato[name].val();

        data.forEach(function (item) {
            // Objeto TipoOperacaoContrato
            var obj = {
                Codigo: item.Codigo,
                Descricao: item.Descricao
            };

            // Adiciona a lista e atualiza a grid
            dataGrid.push(obj);
        });

        _tipoOperacaoContrato[name].val(dataGrid);
    }
}

function RemoverTipoOperacaoContratoFactory(grid, name, data) {
    var dataGrid = grid.BuscarRegistros();

    for (var i = 0; i < dataGrid.length; i++) {
        if (data.Codigo == dataGrid[i].Codigo) {
            dataGrid.splice(i, 1);
            break;
        }
    }

    _tipoOperacaoContrato[name].val(dataGrid);
}

function EditarTipoOperacaoContrato(data) {
    _tipoOperacaoContrato.TipoOperacao.val(data.TipoOperacao);
    RenderizarGridsTipoOperacaoContrato();
}

function LimparCamposTipoOperacaoContrato() {
    LimparCampos(_tipoOperacaoContrato);
    _tipoOperacaoContrato.TipoOperacao.val([]);
    RenderizarGridsTipoOperacaoContrato();
}

function LoadGridsTipoOperacaoContrato() {
    LoadGridTipoOperacaoContratoFactory("_gridTipoOperacaoContratoTipoOperacao", _tipoOperacaoContrato.TipoOperacao.idGrid, "TipoOperacao");
}

function RenderizarGridsTipoOperacaoContrato() {
    RenderizarGridTipoOperacaoContratoTipoOperacao();
}

function RenderizarGridTipoOperacaoContratoTipoOperacao() {
    var data = _tipoOperacaoContrato.TipoOperacao.val();
    _gridTipoOperacaoContratoTipoOperacao.CarregarGrid(data);
}

