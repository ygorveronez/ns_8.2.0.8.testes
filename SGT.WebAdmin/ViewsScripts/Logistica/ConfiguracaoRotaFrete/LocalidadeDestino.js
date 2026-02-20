/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Localidade.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridLocalidadeDestino;
var _localidadeDestino;

/*
 * Declaração das Classes
 */

var LocalidadeDestino = function () {
    this.LocalidadeDestino = PropertyEntity({ type: types.event, text: "Adicionar Cidade", idBtnSearch: guid(), idGrid: guid() });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadLocalidadeDestino() {
    _localidadeDestino = new LocalidadeDestino();
    KoBindings(_localidadeDestino, "knockoutLocalidadeDestino");

    loadGridLocalidadeDestino();
}

function loadGridLocalidadeDestino() {
    var opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: excluirLocalidadeDestinoClick };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [opcaoExcluir] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridLocalidadeDestino = new BasicDataTable(_localidadeDestino.LocalidadeDestino.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarLocalidades(_localidadeDestino.LocalidadeDestino, null, null, null, _gridLocalidadeDestino);

    _localidadeDestino.LocalidadeDestino.basicTable = _gridLocalidadeDestino;
    _localidadeDestino.LocalidadeDestino.basicTable.CarregarGrid([]);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function excluirLocalidadeDestinoClick(registroSelecionado) {
    var listaLocalidadeDestino = obterListaLocalidadeDestino();

    for (var i = 0; i < listaLocalidadeDestino.length; i++) {
        if (registroSelecionado.Codigo == listaLocalidadeDestino[i].Codigo) {
            listaLocalidadeDestino.splice(i, 1);
            break;
        }
    }

    _gridLocalidadeDestino.CarregarGrid(listaLocalidadeDestino);
}

/*
 * Declaração das Funções Públicas
 */

function limparCamposLocalidadeDestino() {
    LimparCampos(_localidadeDestino);
    _gridLocalidadeDestino.CarregarGrid([]);
}

function preencherLocalidadeDestino(localidadesDestino) {
    _gridLocalidadeDestino.CarregarGrid(localidadesDestino);
}

function preencherLocalidadeDestinoSalvar(configuracaoRotaFrete) {
    configuracaoRotaFrete["LocalidadesDestino"] = obterListaLocalidadeDestinoSalvar();
}

/*
 * Declaração das Funções Privadas
 */

function obterListaLocalidadeDestino() {
    return _localidadeDestino.LocalidadeDestino.basicTable.BuscarRegistros();
}

function obterListaLocalidadeDestinoSalvar() {
    var listaLocalidadeDestino = obterListaLocalidadeDestino();

    return JSON.stringify(listaLocalidadeDestino);
}
