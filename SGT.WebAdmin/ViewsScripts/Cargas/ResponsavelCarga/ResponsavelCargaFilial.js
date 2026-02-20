/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="ResponsavelCarga.js" />
/// <reference path="ControleEntregaSetor.js" />

/*
* Declaração de Objetos Globais do Arquivo
*/

var _responsavelCargaFilial;
var _gridResponsavelCargaFilial;

/*
* Declaração das Classes
*/

var ResponsavelCargaFilial = function () {
    this.ListaFilial = PropertyEntity({ type: types.map, required: false, text: "Adicionar Filial", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridResponsavelCargaFilial() {
    var linhasPorPaginas = 5;
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerResponsavelCargaFilial, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "75%", className: "text-align-left" }
    ];

    _gridResponsavelCargaFilial = new BasicDataTable(_responsavelCargaFilial.ListaFilial.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

    new BuscarFilial(_responsavelCargaFilial.ListaFilial, null, _gridResponsavelCargaFilial);
    _responsavelCargaFilial.ListaFilial.basicTable = _gridResponsavelCargaFilial;

    _gridResponsavelCargaFilial.CarregarGrid([]);
}

function loadResponsavelCargaFilial() {
    _responsavelCargaFilial = new ResponsavelCargaFilial();
    KoBindings(_responsavelCargaFilial, "knockoutResponsavelCargaFilial");

    loadGridResponsavelCargaFilial();
}

/*
 * Declaração das Funções Públicas
 */

function limparCamposResponsavelCargaFilial() {
    _gridResponsavelCargaFilial.CarregarGrid([]);
}

function obterResponsavelCargaFilialSalvar() {
    var listaFilial = obterListaFilial();
    var listaFilialRetornar = new Array();

    for (var i = 0; i < listaFilial.length; i++) {
        listaFilialRetornar.push({
            Codigo: listaFilial[i].Codigo
        });
    }

    return JSON.stringify(listaFilialRetornar);
}

function preencherResponsavelCargaFilial(dadosFilial) {
    _gridResponsavelCargaFilial.CarregarGrid(dadosFilial);
}

/*
 * Declaração das Funções
 */

function obterListaFilial() {
    return _gridResponsavelCargaFilial.BuscarRegistros();
}

function removerResponsavelCargaFilial(registroSelecionado) {
    var listaFilial = obterListaFilial();

    for (var i = 0; i < listaFilial.length; i++) {
        if (registroSelecionado.Codigo == listaFilial[i].Codigo) {
            listaFilial.splice(i, 1);
            break;
        }
    }

    _gridResponsavelCargaFilial.CarregarGrid(listaFilial);
}