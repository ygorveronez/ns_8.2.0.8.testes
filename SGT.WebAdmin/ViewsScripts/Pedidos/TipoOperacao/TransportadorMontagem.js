/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../Consultas/Tranportador.js" />

/*
* Declaração de Objetos Globais do Arquivo
*/

var _gridTransportadorMontagem;

/*
 * Declaração das Funções de Inicialização
 */

function loadGridTransportadorMontagem() {
    var linhasPorPaginas = 5;
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerTransportador, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "75%", className: "text-align-left" }
    ];

    _gridTransportadorMontagem = new BasicDataTable(_tipoOperacao.AdicionarTransportadoresMontagem.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    new BuscarTransportadores(_tipoOperacao.AdicionarTransportadoresMontagem, null, null, null, _gridTransportadorMontagem);
    _gridTransportadorMontagem.CarregarGrid([]);
}

/*
 * Declaração das Funções Públicas
 */

function limparCamposTipoOperacaoTransportadorMontagem() {
    _gridTransportadorMontagem.CarregarGrid([]);
}

function obterTipoOperacaoTransportadorMontagemSalvar() {
    var listaTransportador = obterListaTransportador();
    var listaTransportadorRetornar = new Array();

    for (var i = 0; i < listaTransportador.length; i++)
        listaTransportadorRetornar.push(listaTransportador[i].Codigo);

    return JSON.stringify(listaTransportadorRetornar);
}

function preencherTipoOperacaoTranportadorMontagem(dadosTransportador) {
    _gridTransportadorMontagem.CarregarGrid(dadosTransportador);
}

/*
 * Declaração das Funções
 */

function obterListaTransportador() {
    return _gridTransportadorMontagem.BuscarRegistros();
}

function removerTransportador(registroSelecionado) {
    var listaTransportador = obterListaTransportador();

    for (var i = 0; i < listaTransportador.length; i++) {
        if (registroSelecionado.Codigo == listaTransportador[i].Codigo) {
            listaTransportador.splice(i, 1);
            break;
        }
    }
    _gridTransportadorMontagem.CarregarGrid(listaTransportador);
}