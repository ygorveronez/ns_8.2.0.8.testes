/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />

// #region Objetos Globais do Arquivo

var _CRUDPortfolio;
var _gridPortfolio;
var _Portfolio;
var _pesquisaPortfolio;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaPortfolio = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", val: ko.observable(""), def: "", maxlength: 100});
    this.CodigoIntegracao = PropertyEntity({ text: "Código de Integração:", val: ko.observable(""), def: "", maxlength: 50});

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridPortfolio, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

var Portfolio = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", val: ko.observable(""), def: "", required: ko.observable(true) });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de Integração:", val: ko.observable(""), def: "" });
}

var CRUDPortfolio = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadPortfolio() {

    _pesquisaPortfolio = new PesquisaPortfolio();
    KoBindings(_pesquisaPortfolio, "knockoutPesquisaPortfolio", false, _pesquisaPortfolio.Pesquisar.id);

    _Portfolio = new Portfolio();
    KoBindings(_Portfolio, "knockoutPortfolio");

    HeaderAuditoria("Portfolio", _Portfolio);

    _CRUDPortfolio = new CRUDPortfolio();
    KoBindings(_CRUDPortfolio, "knockoutCRUDPortfolio");

    loadGridPortfolio();
}

function loadGridPortfolio() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: function (item) { editarClick(item); }, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

    _gridPortfolio = new GridView(_pesquisaPortfolio.Pesquisar.idGrid, "PortfolioModuloControle/Pesquisa", _pesquisaPortfolio, menuOpcoes);
    _gridPortfolio.CarregarGrid();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarClick(e, sender) {
    Salvar(_Portfolio, "PortfolioModuloControle/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridPortfolio();
                limparCamposPortfolio();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_Portfolio, "PortfolioModuloControle/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridPortfolio();
                limparCamposPortfolio();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposPortfolio();
}

function editarClick(registroSelecionado) {
    limparCamposPortfolio();
   
    _Portfolio.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_Portfolio, "PortfolioModuloControle/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaPortfolio.ExibirFiltros.visibleFade(false);

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
        ExcluirPorCodigo(_Portfolio, "PortfolioModuloControle/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridPortfolio();
                    limparCamposPortfolio();
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
    _CRUDPortfolio.Atualizar.visible(isEdicao);
    _CRUDPortfolio.Excluir.visible(isEdicao);
    _CRUDPortfolio.Cancelar.visible(isEdicao);
    _CRUDPortfolio.Adicionar.visible(!isEdicao);
}

function limparCamposPortfolio() {
    var isEdicao = false;

    controlarBotoesHabilitados(isEdicao);
    LimparCampos(_Portfolio);
    exibirFiltros();
}

function recarregarGridPortfolio() {
    _gridPortfolio.CarregarGrid();
}

function exibirFiltros() {
    if (!_pesquisaPortfolio.ExibirFiltros.visibleFade())
        _pesquisaPortfolio.ExibirFiltros.visibleFade(true);
}

// #endregion Métodos privados