/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../Enumeradores/EnumSimNao.js" />
/// <reference path="../../Consultas/PortfolioModuloControle.js" />

// #region Objetos Globais do Arquivo

var _CRUDCorVeiculo;
var _gridCorVeiculo;
var _CorVeiculo;
var _pesquisaCorVeiculo;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaCorVeiculo = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", val: ko.observable(""), def: "", maxlentgh: 100 });
    this.Situacao = PropertyEntity({ getType: typesKnockout.select, def: 0, options: _statusFemPesquisa, val: ko.observable(0), text: "Situação:" });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridCorVeiculo, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

var CorVeiculo = function () {
    this.CodigoIntegracao = PropertyEntity({ text: "Código Integração:", val: ko.observable(""), getType: typesKnockout.string });
    this.Descricao = PropertyEntity({ text: "*Descrição", val: ko.observable(""), def: "", maxlentgh: 100, required: ko.observable(true) });
    this.Situacao = PropertyEntity({ getType: typesKnockout.select, def: 1, options: _statusFem, val: ko.observable(1), text: "*Situação:", required: ko.observable(true) });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

var CRUDCorVeiculo = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadCorVeiculo() {

    _pesquisaCorVeiculo = new PesquisaCorVeiculo();
    KoBindings(_pesquisaCorVeiculo, "knockoutPesquisaCorVeiculo", false, _pesquisaCorVeiculo.Pesquisar.id);

    _CorVeiculo = new CorVeiculo();
    KoBindings(_CorVeiculo, "knockoutCorVeiculo");

    HeaderAuditoria("CorVeiculo", _CorVeiculo);

    _CRUDCorVeiculo = new CRUDCorVeiculo();
    KoBindings(_CRUDCorVeiculo, "knockoutCRUDCorVeiculo");


    loadGridCorVeiculo();
}

function loadGridCorVeiculo() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: function (item) { editarClick(item); }, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

    _gridCorVeiculo = new GridView(_pesquisaCorVeiculo.Pesquisar.idGrid, "CorVeiculo/Pesquisa", _pesquisaCorVeiculo, menuOpcoes);
    _gridCorVeiculo.CarregarGrid();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarClick(e, sender) {
    Salvar(_CorVeiculo, "CorVeiculo/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridCorVeiculo();
                limparCamposCorVeiculo();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_CorVeiculo, "CorVeiculo/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridCorVeiculo();
                limparCamposCorVeiculo();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposCorVeiculo();
}

function editarClick(registroSelecionado) {
    limparCamposCorVeiculo();
   
    _CorVeiculo.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_CorVeiculo, "CorVeiculo/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaCorVeiculo.ExibirFiltros.visibleFade(false);

                var isEdicao = true;

                controlarBotoesHabilitados(isEdicao);
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
        ExcluirPorCodigo(_CorVeiculo, "CorVeiculo/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridCorVeiculo();
                    limparCamposCorVeiculo();
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

// #endregion Funções Associadas a Eventos

// #region Métodos privados

function controlarBotoesHabilitados(isEdicao) {
    _CRUDCorVeiculo.Atualizar.visible(isEdicao);
    _CRUDCorVeiculo.Excluir.visible(isEdicao);
    _CRUDCorVeiculo.Cancelar.visible(isEdicao);
    _CRUDCorVeiculo.Adicionar.visible(!isEdicao);
}

function limparCamposCorVeiculo() {
    var isEdicao = false;

    controlarBotoesHabilitados(isEdicao);
    LimparCampos(_CorVeiculo);
    exibirFiltros();
}

function recarregarGridCorVeiculo() {
    _gridCorVeiculo.CarregarGrid();
}

function exibirFiltros() {
    if (!_pesquisaCorVeiculo.ExibirFiltros.visibleFade())
        _pesquisaCorVeiculo.ExibirFiltros.visibleFade(true);
}

// #endregion Métodos privados