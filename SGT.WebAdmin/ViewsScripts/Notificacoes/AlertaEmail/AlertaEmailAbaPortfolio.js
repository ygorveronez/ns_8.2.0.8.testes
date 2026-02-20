/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/PortfolioModuloControle.js" />


var _gridAlertaEmailPortfolio;

var Portfolio = function () {
    this.ListaPortfolio = PropertyEntity({ type: types.map, required: false, text: "Adicionar Portfólio", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) });
}

function loadGridAlertaEmailPortfolio() {
    var linhasPorPaginas = 5;
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerAlertaEmailPortfolio, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "75%", className: "text-align-left" }
    ];

    _gridAlertaEmailPortfolio = new BasicDataTable(_portfolioAlertaEmail.ListaPortfolio.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

    new BuscarPortfolioModuloControle(_portfolioAlertaEmail.ListaPortfolio, null, _gridAlertaEmailPortfolio);
    _portfolioAlertaEmail.ListaPortfolio.basicTable = _gridAlertaEmailPortfolio;

    _gridAlertaEmailPortfolio.CarregarGrid([]);
}

function obterListaPortfolio() {
    return _gridAlertaEmailPortfolio.BuscarRegistros();
}

function obterListaPortfolioSalvar() {
    var listaPortfolio = obterListaPortfolio();
    var listaPortfolioRetornar = new Array();

    listaPortfolio.forEach(function (setor) {
        listaPortfolioRetornar.push(Number(setor.Codigo))
    });

    return JSON.stringify(listaPortfolioRetornar);
}

function removerAlertaEmailPortfolio(registroSelecionado) {
    var listaPortfolio = obterListaPortfolio();

    for (var i = 0; i < listaPortfolio.length; i++) {
        if (registroSelecionado.Codigo == listaPortfolio[i].Codigo) {
            listaPortfolio.splice(i, 1);
            break;
        }
    }

    _gridAlertaEmailPortfolio.CarregarGrid(listaPortfolio);
}

function recarregarGridAlertaEmail() {
    _gridAlertaEmail.CarregarGrid();
}