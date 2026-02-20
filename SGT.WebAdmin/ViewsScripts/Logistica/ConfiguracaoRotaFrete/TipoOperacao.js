/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />

var _gridTipoOperacao;
var _tipoOperacao;

var TipoOperacao = function () {
    this.TipoOperacao = PropertyEntity({ type: types.event, text: "Adicionar Tipo de Operação", idBtnSearch: guid(), idGrid: guid() });
}

function loadTipoOperacao() {
    _tipoOperacao = new TipoOperacao();
    KoBindings(_tipoOperacao, "knockoutTipoOperacao");

    loadGridTipoOperacao();
}

function loadGridTipoOperacao() {
    var opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: excluirTipoOperacaoClick };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [opcaoExcluir] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridTipoOperacao = new BasicDataTable(_tipoOperacao.TipoOperacao.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTiposOperacao(_tipoOperacao.TipoOperacao, null, null, null, _gridTipoOperacao);

    _tipoOperacao.TipoOperacao.basicTable = _gridTipoOperacao;
    _tipoOperacao.TipoOperacao.basicTable.CarregarGrid([]);
}

function excluirTipoOperacaoClick(registroSelecionado) {
    var listaTipoOperacao = obterListaTipoOperacao();

    for (var i = 0; i < listaTipoOperacao.length; i++) {
        if (registroSelecionado.Codigo == listaTipoOperacao[i].Codigo) {
            listaTipoOperacao.splice(i, 1);
            break;
        }
    }

    _gridTipoOperacao.CarregarGrid(listaTipoOperacao);
}


function limparCamposTipoOperacao() {
    LimparCampos(_tipoOperacao);
    _gridTipoOperacao.CarregarGrid([]);
}

function preencherTipoOperacao(tiposOperacao) {
    _gridTipoOperacao.CarregarGrid(tiposOperacao);
}

function preencherTipoOperacaoSalvar(configuracaoRotaFrete) {
    configuracaoRotaFrete["TiposOperacao"] = obterListaTipoOperacaoSalvar();
}

function obterListaTipoOperacao() {
    return _tipoOperacao.TipoOperacao.basicTable.BuscarRegistros();
}

function obterListaTipoOperacaoSalvar() {
    var listaTipoOperacao = obterListaTipoOperacao();

    return JSON.stringify(listaTipoOperacao);
}
