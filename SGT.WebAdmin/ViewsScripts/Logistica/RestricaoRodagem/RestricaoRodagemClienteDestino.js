/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="RestricaoRodagem.js" />

/*
* Declaração de Objetos Globais do Arquivo
*/

var _restricaoRodagemClienteDestino;
var _gridRestricaoRodagemClienteDestino;

/*
* Declaração das Classes
*/

var RestricaoRodagemClienteDestino = function () {
    this.ListaClienteDestino = PropertyEntity({ type: types.map, required: false, text: "Adicionar Cliente", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridRestricaoRodagemClienteDestino() {
    var linhasPorPaginas = 5;
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerRestricaoRodagemClienteDestino, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "75%", className: "text-align-left" }
    ];

    _gridRestricaoRodagemClienteDestino = new BasicDataTable(_restricaoRodagemClienteDestino.ListaClienteDestino.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

    new BuscarClientes(_restricaoRodagemClienteDestino.ListaClienteDestino, null, false, null, null, _gridRestricaoRodagemClienteDestino);

    _restricaoRodagemClienteDestino.ListaClienteDestino.basicTable = _gridRestricaoRodagemClienteDestino;

    _gridRestricaoRodagemClienteDestino.CarregarGrid([]);
}

function loadRestricaoRodagemClienteDestino() {
    _restricaoRodagemClienteDestino = new RestricaoRodagemClienteDestino();
    KoBindings(_restricaoRodagemClienteDestino, "knockoutRestricaoRodagemClienteDestino");

    loadGridRestricaoRodagemClienteDestino();
}

/*
 * Declaração das Funções Públicas
 */

function obterRestricaoRodagemClienteDestinoSalvar() {
    var listaClienteDestino = obterListaClienteDestino();
    var listaClienteDestinoRetornar = new Array();

    listaClienteDestino.forEach(function (clienteDestino) {
        listaClienteDestinoRetornar.push({
            Codigo: clienteDestino.Codigo
        });
    });

    return JSON.stringify(listaClienteDestinoRetornar);
}

function preencherRestricaoRodagemClienteDestino(dadosClienteDestino) {
    _gridRestricaoRodagemClienteDestino.CarregarGrid(dadosClienteDestino);
}

/*
 * Declaração das Funções
 */

function limparCamposRestricaoRodagemClienteDestino() {
    _gridRestricaoRodagemClienteDestino.CarregarGrid([]);
}

function obterListaClienteDestino() {
    return _gridRestricaoRodagemClienteDestino.BuscarRegistros();
}

function removerRestricaoRodagemClienteDestino(registroSelecionado) {
    var listaClienteDestino = obterListaClienteDestino();

    for (var i = 0; i < listaClienteDestino.length; i++) {
        if (registroSelecionado.Codigo == listaClienteDestino[i].Codigo) {
            listaClienteDestino.splice(i, 1);
            break;
        }
    }

    _gridRestricaoRodagemClienteDestino.CarregarGrid(listaClienteDestino);
}