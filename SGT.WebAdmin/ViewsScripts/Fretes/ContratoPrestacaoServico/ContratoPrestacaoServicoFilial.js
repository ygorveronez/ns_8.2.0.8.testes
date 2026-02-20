/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="ContratoPrestacaoServico.js" />

/*
* Declaração de Objetos Globais do Arquivo
*/

var _contratoPrestacaoServicoFilial;
var _gridContratoPrestacaoServicoFilial;

/*
* Declaração das Classes
*/

var ContratoPrestacaoServicoFilial = function () {
    this.ListaFilial = PropertyEntity({ type: types.map, required: false, text: "Adicionar Filial", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridContratoPrestacaoServicoFilial() {
    var linhasPorPaginas = 5;
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerContratoPrestacaoServicoFilial, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "75%", className: "text-align-left" }
    ];

    _gridContratoPrestacaoServicoFilial = new BasicDataTable(_contratoPrestacaoServicoFilial.ListaFilial.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

    new BuscarFilial(_contratoPrestacaoServicoFilial.ListaFilial, null, _gridContratoPrestacaoServicoFilial);
    _contratoPrestacaoServicoFilial.ListaFilial.basicTable = _gridContratoPrestacaoServicoFilial;

    _gridContratoPrestacaoServicoFilial.CarregarGrid([]);
}

function loadContratoPrestacaoServicoFilial() {
    _contratoPrestacaoServicoFilial = new ContratoPrestacaoServicoFilial();
    KoBindings(_contratoPrestacaoServicoFilial, "knockoutContratoPrestacaoServicoFilial");

    loadGridContratoPrestacaoServicoFilial();
}

/*
 * Declaração das Funções Públicas
 */

function controlarCamposContratoPrestacaoServicoFilial(habilitar) {
    _contratoPrestacaoServicoFilial.ListaFilial.visible(habilitar);
}

function limparCamposContratoPrestacaoServicoFilial() {
    _gridContratoPrestacaoServicoFilial.CarregarGrid([]);
}

function obterContratoPrestacaoServicoFilialSalvar() {
    var listaFilial = obterListaFilial();
    var listaFilialRetornar = new Array();

    listaFilial.forEach(function (filial) {
        listaFilialRetornar.push({
            Codigo: filial.Codigo
        });
    });

    return JSON.stringify(listaFilialRetornar);
}

function preencherContratoPrestacaoServicoFilial(dadosFilial) {
    _gridContratoPrestacaoServicoFilial.CarregarGrid(dadosFilial);
}

/*
 * Declaração das Funções
 */

function obterListaFilial() {
    return _gridContratoPrestacaoServicoFilial.BuscarRegistros();
}

function removerContratoPrestacaoServicoFilial(registroSelecionado) {
    var listaFilial = obterListaFilial();

    for (var i = 0; i < listaFilial.length; i++) {
        if (registroSelecionado.Codigo == listaFilial[i].Codigo) {
            listaFilial.splice(i, 1);
            break;
        }
    }

    _gridContratoPrestacaoServicoFilial.CarregarGrid(listaFilial);
}