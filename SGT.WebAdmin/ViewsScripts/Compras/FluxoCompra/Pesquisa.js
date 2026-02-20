/// <reference path="../../Consultas/OrdemCompra.js" />
/// <reference path="../../Consultas/CotacaoCompra.js" />
/// <reference path="../../Consultas/RequisicaoCompra.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacaoFluxoCompra.js" />
/// <reference path="../../Enumeradores/EnumEtapaFluxoCompra.js" />
/// <reference path="FluxoCompra.js"/>

//*******MAPEAMENTO KNOUCKOUT*******

var _gridFluxoCompra;
var _pesquisaFluxoCompra;

var PesquisaFluxoCompra = function () {
    this.NumeroInicial = PropertyEntity({ text: "Número Inicial:", getType: typesKnockout.int, maxlength: 10, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.NumeroFinal = PropertyEntity({ text: "Número Final:", getType: typesKnockout.int, maxlength: 10, configInt: { precision: 0, allowZero: false, thousands: "" } });

    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.SituacaoTratativa = PropertyEntity({ text: "Tratativa:", val: ko.observable(EnumTratativaFluxoCompra.Todos), options: EnumTratativaFluxoCompra.obterOpcoesPesquisa(), def: EnumTratativaFluxoCompra.Todos });
    this.Situacao = PropertyEntity({ text: "Situação:", val: ko.observable(EnumSituacaoFluxoCompra.Todos), options: EnumSituacaoFluxoCompra.obterOpcoesPesquisa(), def: EnumSituacaoFluxoCompra.Todos });
    this.EtapaAtual = PropertyEntity({ text: "Etapa Atual:", val: ko.observable(EnumEtapaFluxoCompra.Todos), options: EnumEtapaFluxoCompra.obterOpcoesPesquisa(), def: EnumEtapaFluxoCompra.Todos });

    this.OrdemCompra = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Ordem de Compra:", idBtnSearch: guid() });
    this.CotacaoCompra = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cotação:", idBtnSearch: guid() });
    this.RequisicaoMercadoria = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Requisição de Mercadoria:", idBtnSearch: guid() });
    this.Operador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador:", idBtnSearch: guid() });

    this.Produto = PropertyEntity({ text: "Produto:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid() });
    this.Fornecedor = PropertyEntity({ text: "Fornecedor:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridFluxoCompra.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

//*******EVENTOS*******

function LoadPesquisaFluxoCompra() {
    _pesquisaFluxoCompra = new PesquisaFluxoCompra();
    KoBindings(_pesquisaFluxoCompra, "knockoutPesquisaFluxoCompra", false, _pesquisaFluxoCompra.Pesquisar.id);

    new BuscarOrdemCompra(_pesquisaFluxoCompra.OrdemCompra);
    new BuscarCotacaoCompra(_pesquisaFluxoCompra.CotacaoCompra, RetornoCotacaoCompra);
    new BuscarRequisicaoCompra(_pesquisaFluxoCompra.RequisicaoMercadoria);
    new BuscarFuncionario(_pesquisaFluxoCompra.Operador);
    new BuscarClientes(_pesquisaFluxoCompra.Fornecedor);
    new BuscarProdutoTMS(_pesquisaFluxoCompra.Produto);

    BuscarFluxosCompra();
}

//*******MÉTODOS*******

function RetornoCotacaoCompra(data) {
    _pesquisaFluxoCompra.CotacaoCompra.codEntity(data.Codigo);
    _pesquisaFluxoCompra.CotacaoCompra.val(data.Numero);
}

function BuscarFluxosCompra() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarFluxoCompra, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridFluxoCompra = new GridView(_pesquisaFluxoCompra.Pesquisar.idGrid, "FluxoCompra/Pesquisa", _pesquisaFluxoCompra, menuOpcoes);
    _gridFluxoCompra.CarregarGrid();
}

function RecarregarGridPesquisa() {
    _gridFluxoCompra.CarregarGrid();
}