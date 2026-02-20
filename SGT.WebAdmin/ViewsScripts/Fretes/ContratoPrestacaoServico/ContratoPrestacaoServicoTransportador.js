/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="ContratoPrestacaoServico.js" />

/*
* Declaração de Objetos Globais do Arquivo
*/

var _contratoPrestacaoServicoTransportador;
var _gridContratoPrestacaoServicoTransportador;

/*
* Declaração das Classes
*/

var ContratoPrestacaoServicoTransportador = function () {
    this.ListaTransportador = PropertyEntity({ type: types.map, required: false, text: "Adicionar Transportador", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridContratoPrestacaoServicoTransportador() {
    var linhasPorPaginas = 5;
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerContratoPrestacaoServicoTransportador, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "75%", className: "text-align-left" }
    ];

    _gridContratoPrestacaoServicoTransportador = new BasicDataTable(_contratoPrestacaoServicoTransportador.ListaTransportador.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

    new BuscarTransportadores(_contratoPrestacaoServicoTransportador.ListaTransportador, null, null, null, _gridContratoPrestacaoServicoTransportador);
    _contratoPrestacaoServicoTransportador.ListaTransportador.basicTable = _gridContratoPrestacaoServicoTransportador;

    _gridContratoPrestacaoServicoTransportador.CarregarGrid([]);
}

function loadContratoPrestacaoServicoTransportador() {
    _contratoPrestacaoServicoTransportador = new ContratoPrestacaoServicoTransportador();
    KoBindings(_contratoPrestacaoServicoTransportador, "knockoutContratoPrestacaoServicoTransportador");

    loadGridContratoPrestacaoServicoTransportador();
}

/*
 * Declaração das Funções Públicas
 */

function controlarCamposContratoPrestacaoServicoTransportador(habilitar) {
    _contratoPrestacaoServicoTransportador.ListaTransportador.visible(habilitar);
}

function isContratoPrestacaoServicoTransportadorInformado() {
    var listaTransportador = obterListaTransportador();

    return listaTransportador.length > 0;
}

function limparCamposContratoPrestacaoServicoTransportador() {
    _gridContratoPrestacaoServicoTransportador.CarregarGrid([]);
}

function obterContratoPrestacaoServicoTransportadorSalvar() {
    var listaTransportador = obterListaTransportador();
    var listaTransportadorRetornar = new Array();

    listaTransportador.forEach(function (filial) {
        listaTransportadorRetornar.push({
            Codigo: filial.Codigo
        });
    });

    return JSON.stringify(listaTransportadorRetornar);
}

function preencherContratoPrestacaoServicoTransportador(dadosTransportador) {
    _gridContratoPrestacaoServicoTransportador.CarregarGrid(dadosTransportador);
}

/*
 * Declaração das Funções
 */

function obterListaTransportador() {
    return _gridContratoPrestacaoServicoTransportador.BuscarRegistros();
}

function removerContratoPrestacaoServicoTransportador(registroSelecionado) {
    var listaTransportador = obterListaTransportador();

    for (var i = 0; i < listaTransportador.length; i++) {
        if (registroSelecionado.Codigo == listaTransportador[i].Codigo) {
            listaTransportador.splice(i, 1);
            break;
        }
    }

    _gridContratoPrestacaoServicoTransportador.CarregarGrid(listaTransportador);
}