/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../Enumeradores/EnumSimNao.js" />
/// <reference path="../../Enumeradores/EnumTipoJustificativa.js" />
/// <reference path="../../Consultas/PortfolioModuloControle.js" />

// #region Objetos Globais do Arquivo

var _CRUDFechamentoJustificativaAcrescimoDesconto;
var _gridFechamentoJustificativaAcrescimoDesconto;
var _FechamentoJustificativaAcrescimoDesconto;
var _pesquisaFechamentoJustificativaAcrescimoDesconto;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaFechamentoJustificativaAcrescimoDesconto = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", val: ko.observable(""), def: "", maxlentgh: 100 });
    this.Situacao = PropertyEntity({ getType: typesKnockout.select, def: 0, options: _statusPesquisa, val: ko.observable(0), text: "Situação:" });
    this.TipoJustificativa = PropertyEntity({ val: ko.observable(""), options: EnumTipoJustificativa.obterOpcoesPesquisa(), def: "", text: "Tipo da Justificativa:" });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridFechamentoJustificativaAcrescimoDesconto, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

var FechamentoJustificativaAcrescimoDesconto = function () {
    this.Descricao = PropertyEntity({ text: "*Descrição", val: ko.observable(""), def: "", maxlentgh: 100, required: ko.observable(true) });
    this.Situacao = PropertyEntity({ getType: typesKnockout.select, def: 1, options: _status, val: ko.observable(1), text: "*Situação:", required: ko.observable(true) });
    this.TipoJustificativa = PropertyEntity({ val: ko.observable(EnumTipoJustificativa.Acrescimo), options: EnumTipoJustificativa.obterOpcoes(), def: EnumTipoJustificativa.Acrescimo, text: "Tipo da Justificativa:" });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

var CRUDFechamentoJustificativaAcrescimoDesconto = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadFechamentoJustificativaAcrescimoDesconto() {

    _pesquisaFechamentoJustificativaAcrescimoDesconto = new PesquisaFechamentoJustificativaAcrescimoDesconto();
    KoBindings(_pesquisaFechamentoJustificativaAcrescimoDesconto, "knockoutPesquisaFechamentoJustificativaAcrescimoDesconto", false, _pesquisaFechamentoJustificativaAcrescimoDesconto.Pesquisar.id);

    _FechamentoJustificativaAcrescimoDesconto = new FechamentoJustificativaAcrescimoDesconto();
    KoBindings(_FechamentoJustificativaAcrescimoDesconto, "knockoutFechamentoJustificativaAcrescimoDesconto");

    HeaderAuditoria("FechamentoJustificativaAcrescimoDesconto", _FechamentoJustificativaAcrescimoDesconto);

    _CRUDFechamentoJustificativaAcrescimoDesconto = new CRUDFechamentoJustificativaAcrescimoDesconto();
    KoBindings(_CRUDFechamentoJustificativaAcrescimoDesconto, "knockoutCRUDFechamentoJustificativaAcrescimoDesconto");

    loadGridFechamentoJustificativaAcrescimoDesconto();
}

function loadGridFechamentoJustificativaAcrescimoDesconto() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: function (item) { editarClick(item); }, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

    _gridFechamentoJustificativaAcrescimoDesconto = new GridView(_pesquisaFechamentoJustificativaAcrescimoDesconto.Pesquisar.idGrid, "FechamentoJustificativaAcrescimoDesconto/Pesquisa", _pesquisaFechamentoJustificativaAcrescimoDesconto, menuOpcoes);
    _gridFechamentoJustificativaAcrescimoDesconto.CarregarGrid();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarClick(e, sender) {
    Salvar(_FechamentoJustificativaAcrescimoDesconto, "FechamentoJustificativaAcrescimoDesconto/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridFechamentoJustificativaAcrescimoDesconto();
                limparCamposFechamentoJustificativaAcrescimoDesconto();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_FechamentoJustificativaAcrescimoDesconto, "FechamentoJustificativaAcrescimoDesconto/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridFechamentoJustificativaAcrescimoDesconto();
                limparCamposFechamentoJustificativaAcrescimoDesconto();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposFechamentoJustificativaAcrescimoDesconto();
}

function editarClick(registroSelecionado) {
    limparCamposFechamentoJustificativaAcrescimoDesconto();

    _FechamentoJustificativaAcrescimoDesconto.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_FechamentoJustificativaAcrescimoDesconto, "FechamentoJustificativaAcrescimoDesconto/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaFechamentoJustificativaAcrescimoDesconto.ExibirFiltros.visibleFade(false);

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
        ExcluirPorCodigo(_FechamentoJustificativaAcrescimoDesconto, "FechamentoJustificativaAcrescimoDesconto/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridFechamentoJustificativaAcrescimoDesconto();
                    limparCamposFechamentoJustificativaAcrescimoDesconto();
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
    _CRUDFechamentoJustificativaAcrescimoDesconto.Atualizar.visible(isEdicao);
    _CRUDFechamentoJustificativaAcrescimoDesconto.Excluir.visible(isEdicao);
    _CRUDFechamentoJustificativaAcrescimoDesconto.Cancelar.visible(isEdicao);
    _CRUDFechamentoJustificativaAcrescimoDesconto.Adicionar.visible(!isEdicao);
}

function limparCamposFechamentoJustificativaAcrescimoDesconto() {
    var isEdicao = false;

    controlarBotoesHabilitados(isEdicao);
    LimparCampos(_FechamentoJustificativaAcrescimoDesconto);
    exibirFiltros();
}

function recarregarGridFechamentoJustificativaAcrescimoDesconto() {
    _gridFechamentoJustificativaAcrescimoDesconto.CarregarGrid();
}

function exibirFiltros() {
    if (!_pesquisaFechamentoJustificativaAcrescimoDesconto.ExibirFiltros.visibleFade())
        _pesquisaFechamentoJustificativaAcrescimoDesconto.ExibirFiltros.visibleFade(true);
}

// #endregion Métodos privados