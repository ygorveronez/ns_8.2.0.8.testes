/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="GerarEscala.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridDadosEscalaProduto;
var _dadosEscalaProduto;

/*
 * Declaração das Classes
 */

var DadosEscalaProduto = function () {
    this.ListaProduto = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });

    this.ListaProduto.val.subscribe(function () {
        recarregarGridDadosEscalaProduto();
    });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadDadosEscalaProduto() {
    _dadosEscalaProduto = new DadosEscalaProduto();
    KoBindings(_dadosEscalaProduto, "knockoutDadosEscalaProduto");

    loadGridDadosEscalaProduto();
}

function loadGridDadosEscalaProduto() {
    var linhasPorPaginas = 5;
    var ordenacao = { column: 1, dir: orderDir.asc };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "100%" }
    ];

    _gridDadosEscalaProduto = new BasicDataTable(_dadosEscalaProduto.ListaProduto.idGrid, header, null, ordenacao, null, linhasPorPaginas);
    _gridDadosEscalaProduto.CarregarGrid([]);
}

/*
 * Declaração das Funções Públicas
 */

function carregarProdutosPorDataEscala(dataEscala) {
    if (Boolean(dataEscala) && isPermitirEditarDadosEscala())
        buscarProdutosPorDataEscala(dataEscala);
    else
        limparCamposDadosEscalaProduto();
}

function isDadosEscalaProdutoInformado() {
    var listaDadosEscalaProduto = obterListaDadosEscalaProduto();

    return (listaDadosEscalaProduto.length > 0);
}

function limparCamposDadosEscalaProduto() {
    preencherDadosEscalaProduto([]);
}

function obterDadosEscalaProdutoSalvar() {
    var listaDadosEscalaProduto = obterListaDadosEscalaProduto();
    var listaDadosEscalaProdutoSalvar = new Array();

    for (var i = 0; i < listaDadosEscalaProduto.length; i++) {
        listaDadosEscalaProdutoSalvar.push({
            Codigo: listaDadosEscalaProduto[i].Codigo
        });
    }

    return JSON.stringify(listaDadosEscalaProdutoSalvar);
}

function preencherDadosEscalaProduto(dadosEscalaProduto) {
    _dadosEscalaProduto.ListaProduto.val(dadosEscalaProduto);
}

/*
 * Declaração das Funções Privadas
 */

function buscarProdutosPorDataEscala(dataEscala) {
    executarReST("GerarEscala/BuscarProdutosPorDataEscala", { DataEscala: dataEscala }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data)
                preencherDadosEscalaProduto(retorno.Data);
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function obterListaDadosEscalaProduto() {
    return _dadosEscalaProduto.ListaProduto.val().slice();
}

function recarregarGridDadosEscalaProduto() {
    var listaDadosEscalaProduto = obterListaDadosEscalaProduto();

    _gridDadosEscalaProduto.CarregarGrid(listaDadosEscalaProduto);
}
