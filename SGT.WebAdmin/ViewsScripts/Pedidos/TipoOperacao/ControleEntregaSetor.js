/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../Consultas/SetorFuncionario.js" />

/*
* Declaração de Objetos Globais do Arquivo
*/

var _gridControleEntregaSetor;

/*
 * Declaração das Funções de Inicialização
 */

function loadGridTipoOperacaoControleEntregaSetor() {
    var linhasPorPaginas = 5;
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerTipoOperacao, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "75%", className: "text-align-left" }
    ];

    _gridControleEntregaSetor = new BasicDataTable(_tipoOperacao.Setor.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    new BuscarSetorFuncionario(_tipoOperacao.Setor, null, null, _gridControleEntregaSetor);
    _gridControleEntregaSetor.CarregarGrid([]);
}

function loadTipoOperacaoControleEntregaSetor() {
    loadGridTipoOperacaoControleEntregaSetor();
}

/*
 * Declaração das Funções Públicas
 */

function limparCamposTipoOperacaoControleEntregaSetor() {
    _gridControleEntregaSetor.CarregarGrid([]);
}

function obterTipoOperacaoControleEntregaSetorSalvar() {
    var listaSetor = obterListaSetor();
    var listaSetorRetornar = new Array();

    for (var i = 0; i < listaSetor.length; i++)
        listaSetorRetornar.push(listaSetor[i].Codigo);

    return JSON.stringify(listaSetorRetornar);
}

function preencherTipoOperacaoControleEntregaSetor(dadosSetor) {
    _gridControleEntregaSetor.CarregarGrid(dadosSetor);
}

/*
 * Declaração das Funções
 */

function obterListaSetor() {
    return _gridControleEntregaSetor.BuscarRegistros();
}

function removerTipoOperacao(registroSelecionado) {
    var listaSetor = obterListaSetor();

    for (var i = 0; i < listaSetor.length; i++) {
        if (registroSelecionado.Codigo == listaSetor[i].Codigo) {
            listaSetor.splice(i, 1);
            break;
        }
    }
    _gridControleEntregaSetor.CarregarGrid(listaSetor);
}