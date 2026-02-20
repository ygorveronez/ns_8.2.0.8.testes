/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../Enumeradores/EnumSimNao.js" />
/// <reference path="../../Consultas/PortfolioModuloControle.js" />

// #region Objetos Globais do Arquivo

var _CRUDSIF;
var _gridSIF;
var _SIF;
var _pesquisaSIF;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaSIF = function () {
    this.CodigoSIF = PropertyEntity({ text: "Código SIF:", val: ko.observable(""), def: "", maxlentgh: 100 });
    this.Descricao = PropertyEntity({ text: "Descrição:", val: ko.observable(""), def: "", maxlentgh: 100 });
    this.Status = PropertyEntity({ getType: typesKnockout.select, def: 0, options: _statusFemPesquisa, val: ko.observable(0), text: "Status:" });
    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridSIF, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

var SIF = function () {
    this.CodigoIntegracao = PropertyEntity({ text: "Código Integração:", val: ko.observable(""), getType: typesKnockout.string });
    this.Descricao = PropertyEntity({ text: "*Descrição:", val: ko.observable(""), def: "", maxlentgh: 100, required: ko.observable(true) });
    this.Situacao = PropertyEntity({ getType: typesKnockout.select, def: 1, options: _statusFem, val: ko.observable(1), text: "Status:", required: ko.observable(true) });
    this.CodigoSIF = PropertyEntity({ text: "*Código SIF:", val: ko.observable(), def: "", getType: typesKnockout.string, required: ko.observable(true) });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });


}

var CRUDSIF = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadSIF() {

    _pesquisaSIF = new PesquisaSIF();
    KoBindings(_pesquisaSIF, "knockoutPesquisaSIF", false, _pesquisaSIF.Pesquisar.id);

    _SIF = new SIF();
    KoBindings(_SIF, "knockoutSIF");


    _CRUDSIF = new CRUDSIF();
    KoBindings(_CRUDSIF, "knockoutCRUDSIF");


    loadGridSIF();
}

function loadGridSIF() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: function (item) { editarClick(item); }, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

    _gridSIF = new GridView(_pesquisaSIF.Pesquisar.idGrid, "ServicoInspecaoFederal/Pesquisa", _pesquisaSIF, menuOpcoes);
    _gridSIF.CarregarGrid();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarClick(e, sender) {
    Salvar(_SIF, "ServicoInspecaoFederal/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridSIF();
                limparCamposSIF();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_SIF, "ServicoInspecaoFederal/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridSIF();
                limparCamposSIF();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposSIF();
}

function editarClick(registroSelecionado) {
    limparCamposSIF();

    _SIF.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_SIF, "ServicoInspecaoFederal/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaSIF.ExibirFiltros.visibleFade(false);

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
        ExcluirPorCodigo(_SIF, "ServicoInspecaoFederal/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridSIF();
                    limparCamposSIF();
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
    _CRUDSIF.Atualizar.visible(isEdicao);
    _CRUDSIF.Excluir.visible(isEdicao);
    _CRUDSIF.Cancelar.visible(isEdicao);
    _CRUDSIF.Adicionar.visible(!isEdicao);
}

function limparCamposSIF() {
    var isEdicao = false;

    controlarBotoesHabilitados(isEdicao);
    LimparCampos(_SIF);
    exibirFiltros();
}

function recarregarGridSIF() {
    _gridSIF.CarregarGrid();
}

function exibirFiltros() {
    if (!_pesquisaSIF.ExibirFiltros.visibleFade())
        _pesquisaSIF.ExibirFiltros.visibleFade(true);
}

// #endregion Métodos privados