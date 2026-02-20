/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/GrupoProduto.js" />
/// <reference path="ModeloVeicularCarga.js" />

/*
* Declaração de Objetos Globais do Arquivo
*/

var _modeloVeicularCargaGrupoProduto;
var _gridModeloVeicularCargaGrupoProduto;

/*
* Declaração das Classes
*/

var ModeloVeicularCargaGrupoProduto = function () {
    this.ListaGrupoProduto = PropertyEntity({ type: types.map, required: false, text: Localization.Resources.Cargas.ModeloVeicularCarga.AdicionarGrupoProduto, getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridModeloVeicularCargaGrupoProduto() {
    var linhasPorPaginas = 5;
    var opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerModeloVeicularCargaGrupoProduto, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "75%", className: "text-align-left" }
    ];

    _gridModeloVeicularCargaGrupoProduto = new BasicDataTable(_modeloVeicularCargaGrupoProduto.ListaGrupoProduto.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    
    new BuscarGruposProdutos(_modeloVeicularCargaGrupoProduto.ListaGrupoProduto, null, _gridModeloVeicularCargaGrupoProduto);
    _modeloVeicularCargaGrupoProduto.ListaGrupoProduto.basicTable = _gridModeloVeicularCargaGrupoProduto;

    _gridModeloVeicularCargaGrupoProduto.CarregarGrid([]);
}

function loadModeloVeicularCargaGrupoProduto() {
    _modeloVeicularCargaGrupoProduto = new ModeloVeicularCargaGrupoProduto();
    KoBindings(_modeloVeicularCargaGrupoProduto, "knockoutGrupoProduto");
    
    loadGridModeloVeicularCargaGrupoProduto();
}

/*
 * Declaração das Funções Públicas
 */

function limparCamposModeloVeicularCargaGrupoProduto() {
    _gridModeloVeicularCargaGrupoProduto.CarregarGrid([]);
}

function obterModeloVeicularCargaGrupoProdutoSalvar() {
    var listaGrupoProduto = obterListaGrupoProduto();
    var listaGrupoProdutoRetornar = new Array();

    for (var i = 0; i < listaGrupoProduto.length; i++) {
        listaGrupoProdutoRetornar.push({
            Codigo: listaGrupoProduto[i].Codigo
        });
    }

    return JSON.stringify(listaGrupoProdutoRetornar);
}

function preencherModeloVeicularCargaGrupoProduto(dadosGrupoProduto) {
    _gridModeloVeicularCargaGrupoProduto.CarregarGrid(dadosGrupoProduto);
}

/*
 * Declaração das Funções Privadas
 */

function obterListaGrupoProduto() {
    return _gridModeloVeicularCargaGrupoProduto.BuscarRegistros();
}

function removerModeloVeicularCargaGrupoProduto(registroSelecionado) {
    var listaGrupoProduto = obterListaGrupoProduto();

    for (var i = 0; i < listaGrupoProduto.length; i++) {
        if (registroSelecionado.Codigo == listaGrupoProduto[i].Codigo) {
            listaGrupoProduto.splice(i, 1);
            break;

        }
    }

    _gridModeloVeicularCargaGrupoProduto.CarregarGrid(listaGrupoProduto);
}
