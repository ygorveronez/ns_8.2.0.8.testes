/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDSituacaoEstoquePedido;
var _situacaoEstoquePedido;
var _pesquisaSituacaoEstoquePedido;
var _gridSituacaoEstoquePedido;

var _corPadrao = '#2E8B57';

var _ListaCores = [
    { value: '#FFFFFF' },
    { value: '#ED6464' },
    { value: '#ED8664' },
    { value: '#EDA864' },
    { value: '#EDCB64' },
    { value: '#EDED64' },
    { value: '#CBED64' },
    { value: '#A8ED64' },
    { value: '#86ED64' },
    { value: '#64ED64' },
    { value: '#64ED86' },
    { value: '#64EDA8' },
    { value: '#64EDCB' },
    { value: '#64EDED' },
    { value: '#64CBED' },
    { value: '#64A8ED' },
    { value: '#6495ED' },
    { value: '#6486ED' },
    { value: '#6464ED' },
    { value: '#8664ED' },
    { value: '#A864ED' },
    { value: '#CB64ED' },
    { value: '#ED64ED' },
    { value: '#ED64CB' },
    { value: '#ED64A8' },
    { value: '#ED6486' },
    { value: '#8B4513' },
    { value: '#E06F1F' },
    { value: '#EDA978' },
    { value: '#F9E2D2' },
    { value: '#000000' },
    { value: '#708090' },
    { value: '#9AA6B1' },
    { value: '#C5CCD3' },
    { value: '#F1F2F4' },
    { value: '#2E8B57' },
    { value: '#00BFFF' },
    { value: '#808080' },
    { value: '#A020F0' }
];

/*
 * Declaração das Classes
 */

var PesquisaSituacaoEstoquePedido = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.CodigoIntegracao = PropertyEntity({ text: "Codigo Integração:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.BloqueiaPedido = PropertyEntity({ text: "Bloqueia o pedido: ", val: ko.observable(2), options: EnumSimNaoPesquisa.obterOpcoesPesquisa2() });
    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridSituacaoEstoquePedido, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

var SituacaoEstoquePedido = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.CodigoIntegracao = PropertyEntity({ text: "Codigo de Integração:", required: false, getType: typesKnockout.string, val: ko.observable(""), maxlength: 50 });
    this.BloqueiaPedido = PropertyEntity({ text: "Bloqueia o pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 2000 });
    this.Cor = PropertyEntity({ text: "Cor: ", val: ko.observable(_corPadrao), options: _ListaCores });
}

var CRUDSituacaoEstoque = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar / Nova" });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridSituacaoEstoquePedido() {
    const opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    const menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    const configuracoesExportacao = { url: "SituacaoEstoquePedido/ExportarPesquisa", titulo: "Situação estoque dos Pedidos" };

    _gridSituacaoEstoquePedido = new GridViewExportacao(_pesquisaSituacaoEstoquePedido.Pesquisar.idGrid, "SituacaoEstoquePedido/Pesquisa", _pesquisaSituacaoEstoquePedido, menuOpcoes, configuracoesExportacao);
    _gridSituacaoEstoquePedido.CarregarGrid();
}

function loadSituacaoEstoquePedido() {
    _situacaoEstoquePedido = new SituacaoEstoquePedido();
    KoBindings(_situacaoEstoquePedido, "knockoutSituacaoEstoquePedido");

    _CRUDSituacaoEstoquePedido = new CRUDSituacaoEstoque();
    KoBindings(_CRUDSituacaoEstoquePedido, "knockoutCRUDSituacaoEstoquePedido");

    _pesquisaSituacaoEstoquePedido = new PesquisaSituacaoEstoquePedido();
    KoBindings(_pesquisaSituacaoEstoquePedido, "knockoutPesquisaSituacaoEstoquePedido", false, _pesquisaSituacaoEstoquePedido.Pesquisar.id);

    loadGridSituacaoEstoquePedido();

    loadCores();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    Salvar(_situacaoEstoquePedido, "SituacaoEstoquePedido/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridSituacaoEstoquePedido();
                limparCamposSituacaoEstoquePedido();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_situacaoEstoquePedido, "SituacaoEstoquePedido/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridSituacaoEstoquePedido();
                limparCamposSituacaoEstoquePedido();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposSituacaoEstoquePedido();
}

function editarClick(registroSelecionado) {
    limparCamposSituacaoEstoquePedido();

    _situacaoEstoquePedido.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_situacaoEstoquePedido, "SituacaoEstoquePedido/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaSituacaoEstoquePedido.ExibirFiltros.visibleFade(false);
                controlarBotoesHabilitados();
                setarCor(retorno.Data.Cor);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function excluirClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_situacaoEstoquePedido, "SituacaoEstoquePedido/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridSituacaoEstoquePedido();
                    limparCamposSituacaoEstoquePedido();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

/*
 * Declaração das Funções
 */

function controlarBotoesHabilitados() {
    const isEdicao = _situacaoEstoquePedido.Codigo.val() > 0;

    _CRUDSituacaoEstoquePedido.Atualizar.visible(isEdicao);
    _CRUDSituacaoEstoquePedido.Excluir.visible(isEdicao);
    _CRUDSituacaoEstoquePedido.Adicionar.visible(!isEdicao);
}

function limparCamposSituacaoEstoquePedido() {
    controlarBotoesHabilitados();
    LimparCampos(_situacaoEstoquePedido);
    setarCor(_corPadrao);
}

function recarregarGridSituacaoEstoquePedido() {
    _gridSituacaoEstoquePedido.CarregarGrid();
}

function loadCores() {
    $("#" + _situacaoEstoquePedido.Cor.id).colorselector({
        callback: function (value) {
            _situacaoEstoquePedido.Cor.val(value);
        }
    });
    setarCor(_corPadrao);
}

function setarCor(cor) {
    if (cor == null)
        cor = _corPadrao;
    $("#" + _situacaoEstoquePedido.Cor.id).colorselector("setValue", cor);
}