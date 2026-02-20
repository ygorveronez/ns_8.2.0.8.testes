/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCargaJanelaDescarregamentoAdicional.js" />

// #region Objetos Globais do Arquivo

var _CRUDJanelaDescarregamentoSituacao;
var _janelaDescarregamentoSituacao;
var _pesquisaJanelaDescarregamentoSituacao;
var _gridJanelaDescarregamentoSituacao;

// #endregion Objetos Globais do Arquivo

// #region Classes

var CRUDJanelaDescarregamentoSituacao = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var JanelaDescarregamentoSituacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Cor = PropertyEntity({ text: "*Cor:", required: true });
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true });
    this.Situacao = PropertyEntity({ text: "*Situação: ", val: ko.observable(""), options: EnumSituacaoCargaJanelaDescarregamentoAdicional.obterOpcoes(), def: "" });
}

var PesquisaJanelaDescarregamentoSituacao = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoCargaJanelaDescarregamentoAdicional.Todas), options: EnumSituacaoCargaJanelaDescarregamentoAdicional.obterOpcoesPesquisa(), def: EnumSituacaoCargaJanelaDescarregamentoAdicional.Todas });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridJanelaDescarregamentoSituacao, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadGridJanelaDescarregamentoSituacao() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "JanelaDescargaSituacao/ExportarPesquisa", titulo: "Situações da janela de Descarregamento" };

    _gridJanelaDescarregamentoSituacao = new GridViewExportacao(_pesquisaJanelaDescarregamentoSituacao.Pesquisar.idGrid, "JanelaDescargaSituacao/Pesquisa", _pesquisaJanelaDescarregamentoSituacao, menuOpcoes, configuracoesExportacao);
    _gridJanelaDescarregamentoSituacao.CarregarGrid();
}

function loadJanelaDescarregamentoSituacao() {
    _janelaDescarregamentoSituacao = new JanelaDescarregamentoSituacao();
    KoBindings(_janelaDescarregamentoSituacao, "knockoutJanelaDescarregamentoSituacao");

    HeaderAuditoria("JanelaDescarregamentoSituacao", _janelaDescarregamentoSituacao);

    _CRUDJanelaDescarregamentoSituacao = new CRUDJanelaDescarregamentoSituacao();
    KoBindings(_CRUDJanelaDescarregamentoSituacao, "knockoutCRUDJanelaDescarregamentoSituacao");

    _pesquisaJanelaDescarregamentoSituacao = new PesquisaJanelaDescarregamentoSituacao();
    KoBindings(_pesquisaJanelaDescarregamentoSituacao, "knockoutPesquisaJanelaDescarregamentoSituacao", false, _pesquisaJanelaDescarregamentoSituacao.Pesquisar.id);

    loadGridJanelaDescarregamentoSituacao();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarClick(e, sender) {
    Salvar(_janelaDescarregamentoSituacao, "JanelaDescargaSituacao/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridJanelaDescarregamentoSituacao();
                limparCamposJanelaDescarregamentoSituacao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_janelaDescarregamentoSituacao, "JanelaDescargaSituacao/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridJanelaDescarregamentoSituacao();
                limparCamposJanelaDescarregamentoSituacao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposJanelaDescarregamentoSituacao();
}

function editarClick(registroSelecionado) {
    limparCamposJanelaDescarregamentoSituacao();

    _janelaDescarregamentoSituacao.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_janelaDescarregamentoSituacao, "JanelaDescargaSituacao/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaJanelaDescarregamentoSituacao.ExibirFiltros.visibleFade(false);

                controlarBotoesHabilitados();
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
        ExcluirPorCodigo(_janelaDescarregamentoSituacao, "JanelaDescargaSituacao/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridJanelaDescarregamentoSituacao();
                    limparCamposJanelaDescarregamentoSituacao();
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

// #region Funções

function controlarBotoesHabilitados() {
    var isEdicao = _janelaDescarregamentoSituacao.Codigo.val() > 0;

    _CRUDJanelaDescarregamentoSituacao.Atualizar.visible(isEdicao);
    _CRUDJanelaDescarregamentoSituacao.Excluir.visible(isEdicao);
    _CRUDJanelaDescarregamentoSituacao.Cancelar.visible(isEdicao);
    _CRUDJanelaDescarregamentoSituacao.Adicionar.visible(!isEdicao);
}

function limparCamposJanelaDescarregamentoSituacao() {
    LimparCampos(_janelaDescarregamentoSituacao);
    controlarBotoesHabilitados();
}

function recarregarGridJanelaDescarregamentoSituacao() {
    _gridJanelaDescarregamentoSituacao.CarregarGrid();
}

// #endregion Funções
