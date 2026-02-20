/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../Enumeradores/EnumSimNao.js" />
/// <reference path="../../Consultas/PortfolioModuloControle.js" />

// #region Objetos Globais do Arquivo

var _CRUDJustificativa;
var _gridJustificativa;
var _Justificativa;
var _pesquisaJustificativa;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaJustificativa = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", val: ko.observable(""), def: "", maxlentgh: 100, required: true });
    this.Situacao = PropertyEntity({ getType: typesKnockout.select, def: 0, options: _statusFemPesquisa, val: ko.observable(0), text: "Situação:", required: true });
    this.Observacao = PropertyEntity({ text: "Observacão:", val: ko.observable(""), def: "", maxlentgh: 500 });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridJustificativa, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

var Justificativa = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição", val: ko.observable(""), def: "", maxlentgh: 100, required: ko.observable(true) });
    this.Situacao = PropertyEntity({ getType: typesKnockout.select, def: 1, options: _statusFem, val: ko.observable(1), text: "*Situação:", required: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observacão:", val: ko.observable(""), def: "", maxlentgh: 500 });
}

var CRUDJustificativa = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadJustificativaCancelamentoAgendamento() {

    _pesquisaJustificativa = new PesquisaJustificativa();
    KoBindings(_pesquisaJustificativa, "knockoutPesquisaJustificativa", false, _pesquisaJustificativa.Pesquisar.id);

    _Justificativa = new Justificativa();
    KoBindings(_Justificativa, "knockoutJustificativa");

    HeaderAuditoria("Justificativa", _Justificativa);

    _CRUDJustificativa = new CRUDJustificativa();
    KoBindings(_CRUDJustificativa, "knockoutCRUDJustificativa");

    loadGridJustificativa();
}

function loadGridJustificativa() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: function (item) { editarClick(item); }, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

    _gridJustificativa = new GridView(_pesquisaJustificativa.Pesquisar.idGrid, "JustificativaCancelamentoAgendamento/Pesquisa", _pesquisaJustificativa, menuOpcoes);
    _gridJustificativa.CarregarGrid();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarClick(e, sender) {
    Salvar(_Justificativa, "JustificativaCancelamentoAgendamento/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridJustificativa();
                limparCamposJustificativa();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_Justificativa, "JustificativaCancelamentoAgendamento/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridJustificativa();
                limparCamposJustificativa();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposJustificativa();
}

function editarClick(registroSelecionado) {
    limparCamposJustificativa();
   
    _Justificativa.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_Justificativa, "JustificativaCancelamentoAgendamento/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaJustificativa.ExibirFiltros.visibleFade(false);

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
        ExcluirPorCodigo(_Justificativa, "JustificativaCancelamentoAgendamento/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridJustificativa();
                    limparCamposJustificativa();
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
    _CRUDJustificativa.Atualizar.visible(isEdicao);
    _CRUDJustificativa.Excluir.visible(isEdicao);
    _CRUDJustificativa.Cancelar.visible(isEdicao);
    _CRUDJustificativa.Adicionar.visible(!isEdicao);
}

function limparCamposJustificativa() {
    var isEdicao = false;

    controlarBotoesHabilitados(isEdicao);
    LimparCampos(_Justificativa);
    exibirFiltros();
}

function recarregarGridJustificativa() {
    _gridJustificativa.CarregarGrid();
}

function exibirFiltros() {
    if (!_pesquisaJustificativa.ExibirFiltros.visibleFade())
        _pesquisaJustificativa.ExibirFiltros.visibleFade(true);
}

// #endregion Métodos privados