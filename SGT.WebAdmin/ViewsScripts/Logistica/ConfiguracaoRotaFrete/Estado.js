/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Estado.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridEstado;
var _estado;

/*
 * Declaração das Classes
 */

var Estado = function () {
    this.Estado = PropertyEntity({ type: types.event, text: "Adicionar Estado", idBtnSearch: guid(), idGrid: guid() });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadEstado() {
    _estado = new Estado();
    KoBindings(_estado, "knockoutEstado");

    loadGridEstado();
}

function loadGridEstado() {
    var opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: excluirEstadoClick };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [opcaoExcluir] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridEstado = new BasicDataTable(_estado.Estado.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarEstados(_estado.Estado, null, _gridEstado);

    _estado.Estado.basicTable = _gridEstado;
    _estado.Estado.basicTable.CarregarGrid([]);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function excluirEstadoClick(registroSelecionado) {
    var listaEstado = obterListaEstado();

    for (var i = 0; i < listaEstado.length; i++) {
        if (registroSelecionado.Codigo == listaEstado[i].Codigo) {
            listaEstado.splice(i, 1);
            break;
        }
    }

    _gridEstado.CarregarGrid(listaEstado);
}

/*
 * Declaração das Funções Públicas
 */

function limparCamposEstado() {
    LimparCampos(_estado);
    _gridEstado.CarregarGrid([]);
}

function preencherEstado(estados) {
    _gridEstado.CarregarGrid(estados);
}

function preencherEstadoSalvar(configuracaoRotaFrete) {
    configuracaoRotaFrete["Estados"] = obterListaEstadoSalvar();
}

/*
 * Declaração das Funções Privadas
 */

function obterListaEstado() {
    return _estado.Estado.basicTable.BuscarRegistros();
}

function obterListaEstadoSalvar() {
    var listaEstado = obterListaEstado();

    return JSON.stringify(listaEstado);
}
