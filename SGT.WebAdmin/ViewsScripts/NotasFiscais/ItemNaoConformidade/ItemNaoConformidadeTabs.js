
/// <reference path="../../../../../ViewsScripts/Consultas/Filiais.js" />

//#region Variaives Globais
var _tabCfop;
var _tabTipoOperacao;
var _tabFilial;
var _tabFornecedores;
var _gridCfop;
var _gridTipoOperacao;
var _gridFilial;
var _gridFornecedores;
//#endregion


//#region Construtores
function TabCFOP() {
    this.Adicionar = PropertyEntity({ type: types.event, text: "Adicionar", visible: ko.observable(true), idBtnSearch: guid() });
    this.CFOP = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), required: true, idGrid: guid() });
}
function TabTipoOperacao() {
    this.Adicionar = PropertyEntity({ type: types.event, text: "Adicionar", visible: ko.observable(true), idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), required: true, idGrid: guid() });
}

function TabFilial() {
    this.Adicionar = PropertyEntity({ type: types.event, text: "Adicionar", visible: ko.observable(true), idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), required: true, idGrid: guid() });
}
function TabFornecedor() {
    this.Adicionar = PropertyEntity({ type: types.event, text: "Adicionar", visible: ko.observable(true), idBtnSearch: guid() });
    this.Fornecedor = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), required: true, idGrid: guid() });
}
//#endregion


//#region Carregamento
function loadTabsItemNaoConformidades() {

    _tabCfop = new TabCFOP();
    KoBindings(_tabCfop, "knockoutCFOPItem");

    _tabTipoOperacao = new TabTipoOperacao();
    KoBindings(_tabTipoOperacao, "knockoutTipoOperacaoItem");

    _tabFilial = new TabFilial();
    KoBindings(_tabFilial, "knockoutFilialItem");

    _tabFornecedores = new TabFornecedor();
    KoBindings(_tabFornecedores, "knockoutFornecedorItem");


    loadGridCFOP();
    loadGridFornecedor();
    loadGridFilial();
    loadGridTipoOperacao();

    new BuscarClientes(_tabFornecedores.Adicionar, null, null, null, null, _gridFornecedores);
    new BuscarTiposOperacao(_tabTipoOperacao.Adicionar, null, null, null, _gridTipoOperacao );
    new BuscarFilial(_tabFilial.Adicionar, null, _gridFilial, null,);
    new BuscarCFOPNotaFiscal(_tabCfop.Adicionar, null, null, null,null,null,null,null, _gridCfop);
}
//#endregion


//#region Loads
function loadGridCFOP() {

    let header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descricao", width: "30%", className: "text-align-center" }
    ];

    // Grid
    _gridCfop = CriarGridBasicTab(_tabCfop.CFOP.idGrid, header, RemoverCFOP);
    _gridCfop.CarregarGrid([]);
}

function loadGridTipoOperacao() {

    let header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descricao", width: "30%", className: "text-align-center" }
    ];

    _gridTipoOperacao = CriarGridBasicTab(_tabTipoOperacao.TipoOperacao.idGrid, header, RemoverTipoOperacao);
    _gridTipoOperacao.CarregarGrid([]);
}

function loadGridFilial() {

    let header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descricao", width: "30%", className: "text-align-center" }
    ];

    _gridFilial = CriarGridBasicTab(_tabFilial.Filial.idGrid, header, RemoverFilial);
    _gridFilial.CarregarGrid([]);
}

function loadGridFornecedor() {

    let header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descricao", width: "30%", className: "text-align-center" }
    ];

    _gridFornecedores = CriarGridBasicTab(_tabFornecedores.Fornecedor.idGrid, header, RemoverFornecedor);
    _gridFornecedores.CarregarGrid([]);
}
//#endregion



//#region Auxiliares

function CriarGridBasicTab(idGrid, headers, callbackRemover) {

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [
            {
                descricao: "Excluir",
                id: guid(),
                evento: "onclick",
                tamanho: "10",
                icone: "",
                metodo: callbackRemover
            }
        ]
    };

    return new BasicDataTable(idGrid, headers, menuOpcoes, null, null, 10);
}

function RemoverFornecedor(item) {
    _gridFornecedores.CarregarGrid(RemoverRegistroGrid(_gridFornecedores.BuscarRegistros(),item.Codigo))
}
function RemoverFilial(item) {
    _gridFilial.CarregarGrid(RemoverRegistroGrid(_gridFilial.BuscarRegistros(), item.Codigo))
}
function RemoverCFOP(item) {
    _gridCfop.CarregarGrid(RemoverRegistroGrid(_gridCfop.BuscarRegistros(), item.Codigo))
}
function RemoverTipoOperacao(item) {
    _gridTipoOperacao.CarregarGrid(RemoverRegistroGrid(_gridTipoOperacao.BuscarRegistros(), item.Codigo))
}
function RemoverRegistroGrid(list, codigoRemover) {

    for (var i = 0; i < list.length; i++) {
        let item = list[i];

        if (item.Codigo != codigoRemover)
            continue;

        list.splice(i, 1);
        break;
    }
    return list;
}

function PreecherValoresTabs() {

    _itemNaoConformidade.Filiais.val(RetornarRegistrosConvertidos(_gridFilial.BuscarRegistros()))
    _itemNaoConformidade.Fornecedor.val(RetornarRegistrosConvertidos(_gridFornecedores.BuscarRegistros()))
    _itemNaoConformidade.CFOP.val(RetornarRegistrosConvertidos(_gridCfop.BuscarRegistros()))
    _itemNaoConformidade.TipoOperacao.val(RetornarRegistrosConvertidos(_gridTipoOperacao.BuscarRegistros()))
}
function RetornarRegistrosConvertidos(list) {
    let codigos = list.map(x => x.Codigo);
    return JSON.stringify(codigos);
}
function CarregarGridsTabs(retorno) {
    let listfiliais = retorno.Filiais;
    let listFornecedor = retorno.Fornecedor;
    let listCFOP = retorno.CFOP;
    let listTipoOperacao = retorno.TipoOperacao;

    PreecherListasGrids(listfiliais, _gridFilial);
    PreecherListasGrids(listFornecedor, _gridFornecedores);
    PreecherListasGrids(listCFOP, _gridCfop);
    PreecherListasGrids(listTipoOperacao, _gridTipoOperacao);
}

function PreecherListasGrids(list, grid) {
    grid.CarregarGrid(list);
}

function limparGridsTabs() {
    _gridFilial.CarregarGrid([])
    _gridFornecedores.CarregarGrid([])
    _gridCfop.CarregarGrid([])
    _gridTipoOperacao.CarregarGrid([])
}
//#endregion

