/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="ItemNaoConformidade.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _tipoOperacao;
var _gridTipoOperacao;

var TipoOperacao = function () {
    this.ItemNaoConformidade = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.TipoOperacao = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), required: true, idGrid: guid() });
    this.TipoOperacao.val.subscribe(function () {
        _itemNaoConformidade.TipoOperacao.val(JSON.stringify(_tipoOperacao.TipoOperacao.val().map(function (r) { return r.Codigo; })));
        RenderizarGridTipoOperacao();
    });

    this.Adicionar = PropertyEntity({ type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), idBtnSearch: guid() });
}

//*******EVENTOS*******

function LoadTipoOperacao() {
    _tipoOperacao = new TipoOperacao();
    KoBindings(_tipoOperacao, "knockoutTipoOperacao");

    LoadGridTipoOperacao();

    // Buscas
    new BuscarTiposOperacao(_tipoOperacao.Adicionar, AdicionarTipoOperacaoAGrid);
}

function AdicionarTipoOperacaoAGrid(data) {
    var dataGrid = _tipoOperacao.TipoOperacao.val();
    
    var tipoOpe = {
        Codigo: data.Codigo,
        Descricao: data.Descricao,
    };

    dataGrid.push(tipoOpe);

    _tipoOperacao.TipoOperacao.val(dataGrid);
}

function RemoverTipoOperacaoClick(data) {
    var dataGrid = _gridTipoOperacao.BuscarRegistros();

    for (var i = 0; i < dataGrid.length; i++) {
        if (data.Codigo == dataGrid[i].Codigo) {
            dataGrid.splice(i, 1);
            break;
        }
    }

    _tipoOperacao.TipoOperacao.val(dataGrid);
}


//*******MÉTODOS*******
function ListarTipoOperacao(data) {
    _tipoOperacao.ItemNaoConformidade.val(_itemNaoConformidade.Codigo.val());

    _tipoOperacao.TipoOperacao.val(data.TipoOperacao);
    RenderizarGridTipoOperacao();
}

function LimparCamposTipoOperacao() {
    LimparCampos(_tipoOperacao);
    _tipoOperacao.TipoOperacao.val([]);
    RenderizarGridTipoOperacao();
}

function LoadGridTipoOperacao() {
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
                    RemoverTipoOperacaoClick(data);
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
    _gridTipoOperacao = new BasicDataTable(_tipoOperacao.TipoOperacao.idGrid, header, menuOpcoes, null, null, 10);
    _gridTipoOperacao.CarregarGrid([]);
}

function RenderizarGridTipoOperacao() {
    var tipoOperacao = _tipoOperacao.TipoOperacao.val();

    _gridTipoOperacao.CarregarGrid(tipoOperacao);
}