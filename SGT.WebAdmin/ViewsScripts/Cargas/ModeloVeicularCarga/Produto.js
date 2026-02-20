/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="ModeloVeicularCarga.js" />

/*
* Declaração de Objetos Globais do Arquivo
*/

var _modeloVeicularCargaProduto;
var _gridModeloVeicularCargaProduto;

/*
* Declaração das Classes
*/

var ModeloVeicularCargaProduto = function () {
    this.ListaProduto = PropertyEntity({ type: types.map, required: false, text: Localization.Resources.Cargas.ModeloVeicularCarga.AdicionarProduto, getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridModeloVeicularCargaProduto() {
    var linhasPorPaginas = 5;
    var opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerModeloVeicularCargaProduto, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "75%", className: "text-align-left" }
    ];

    _gridModeloVeicularCargaProduto = new BasicDataTable(_modeloVeicularCargaProduto.ListaProduto.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

    new BuscarProdutos(_modeloVeicularCargaProduto.ListaProduto, undefined, undefined, undefined, undefined, undefined, undefined, _gridModeloVeicularCargaProduto);
    _modeloVeicularCargaProduto.ListaProduto.basicTable = _gridModeloVeicularCargaProduto;

    _gridModeloVeicularCargaProduto.CarregarGrid([]);
}

function loadModeloVeicularCargaProduto() {
    _modeloVeicularCargaProduto = new ModeloVeicularCargaProduto();
    KoBindings(_modeloVeicularCargaProduto, "knockoutProduto");

    loadGridModeloVeicularCargaProduto();
}

/*
 * Declaração das Funções Públicas
 */

function limparCamposModeloVeicularCargaProduto() {
    _gridModeloVeicularCargaProduto.CarregarGrid([]);
}

function obterModeloVeicularCargaProdutoSalvar() {
    var listaProduto = obterListaProduto();
    var listaProdutoRetornar = new Array();

    for (var i = 0; i < listaProduto.length; i++) {
        listaProdutoRetornar.push({
            Codigo: listaProduto[i].Codigo
        });
    }

    return JSON.stringify(listaProdutoRetornar);
}

function preencherModeloVeicularCargaProduto(dadosProduto) {
    _gridModeloVeicularCargaProduto.CarregarGrid(dadosProduto);
}

/*
 * Declaração das Funções Privadas
 */

function obterListaProduto() {
    return _gridModeloVeicularCargaProduto.BuscarRegistros();
}

function removerModeloVeicularCargaProduto(registroSelecionado) {
    var listaProduto = obterListaProduto();

    for (var i = 0; i < listaProduto.length; i++) {
        if (registroSelecionado.Codigo == listaProduto[i].Codigo) {
            listaProduto.splice(i, 1);
            break;

        }
    }

    _gridModeloVeicularCargaProduto.CarregarGrid(listaProduto);
}
