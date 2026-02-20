/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Localidade.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridLocalidadeOrigem;
var _localidadeOrigem;

/*
 * Declaração das Classes
 */

var LocalidadeOrigem = function () {
    this.LocalidadeOrigem = PropertyEntity({ type: types.event, text: "Adicionar Cidade", idBtnSearch: guid(), idGrid: guid() });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadLocalidadeOrigem() {
    _localidadeOrigem = new LocalidadeOrigem();
    KoBindings(_localidadeOrigem, "knockoutLocalidadeOrigem");

    loadGridLocalidadeOrigem();
}

function loadGridLocalidadeOrigem() {
    var opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: excluirLocalidadeOrigemClick };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [opcaoExcluir] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridLocalidadeOrigem = new BasicDataTable(_localidadeOrigem.LocalidadeOrigem.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarLocalidades(_localidadeOrigem.LocalidadeOrigem, null, null, null, _gridLocalidadeOrigem);

    _localidadeOrigem.LocalidadeOrigem.basicTable = _gridLocalidadeOrigem;
    _localidadeOrigem.LocalidadeOrigem.basicTable.CarregarGrid([]);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function excluirLocalidadeOrigemClick(registroSelecionado) {
    var listaLocalidadeOrigem = obterListaLocalidadeOrigem();

    for (var i = 0; i < listaLocalidadeOrigem.length; i++) {
        if (registroSelecionado.Codigo == listaLocalidadeOrigem[i].Codigo) {
            listaLocalidadeOrigem.splice(i, 1);
            break;
        }
    }

    _gridLocalidadeOrigem.CarregarGrid(listaLocalidadeOrigem);
}

/*
 * Declaração das Funções Públicas
 */

function limparCamposLocalidadeOrigem() {
    LimparCampos(_localidadeOrigem);
    _gridLocalidadeOrigem.CarregarGrid([]);
}

function preencherLocalidadeOrigem(localidadesOrigem) {
    _gridLocalidadeOrigem.CarregarGrid(localidadesOrigem);
}

function preencherLocalidadeOrigemSalvar(configuracaoRotaFrete) {
    configuracaoRotaFrete["LocalidadesOrigem"] = obterListaLocalidadeOrigemSalvar();
}

/*
 * Declaração das Funções Privadas
 */

function obterListaLocalidadeOrigem() {
    return _localidadeOrigem.LocalidadeOrigem.basicTable.BuscarRegistros();
}

function obterListaLocalidadeOrigemSalvar() {
    var listaLocalidadeOrigem = obterListaLocalidadeOrigem();

    return JSON.stringify(listaLocalidadeOrigem);
}
