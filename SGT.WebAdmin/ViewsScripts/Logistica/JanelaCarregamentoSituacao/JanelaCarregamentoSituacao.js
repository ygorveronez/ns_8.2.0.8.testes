/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCargaJanelaCarregamentoAdicional.js" />

// #region Objetos Globais do Arquivo

var _CRUDJanelaCarregamentoSituacao;
var _janelaCarregamentoSituacao;
var _pesquisaJanelaCarregamentoSituacao;
var _gridJanelaCarregamentoSituacao;

// #endregion Objetos Globais do Arquivo

// #region Classes

var CRUDJanelaCarregamentoSituacao = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
}

var JanelaCarregamentoSituacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Cor = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Cor.getRequiredFieldDescription(), required: true });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getRequiredFieldDescription(), required: true });
    this.Situacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Situacao.getRequiredFieldDescription(), val: ko.observable(""), options: EnumSituacaoCargaJanelaCarregamentoAdicional.obterOpcoes(), def: "" });
}

var PesquisaJanelaCarregamentoSituacao = function () {
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), val: ko.observable(EnumSituacaoCargaJanelaCarregamentoAdicional.Todas), options: EnumSituacaoCargaJanelaCarregamentoAdicional.obterOpcoesPesquisa(), def: EnumSituacaoCargaJanelaCarregamentoAdicional.Todas });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: Localization.Resources.Gerais.Geral.FiltroPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridJanelaCarregamentoSituacao, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadGridJanelaCarregamentoSituacao() {
    var opcaoEditar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "JanelaCarregamentoSituacao/ExportarPesquisa", titulo: Localization.Resources.Logistica.JanelaCarregamentoSituacao.JanelaCaregamentoSituacao };

    _gridJanelaCarregamentoSituacao = new GridViewExportacao(_pesquisaJanelaCarregamentoSituacao.Pesquisar.idGrid, "JanelaCarregamentoSituacao/Pesquisa", _pesquisaJanelaCarregamentoSituacao, menuOpcoes, configuracoesExportacao);
    _gridJanelaCarregamentoSituacao.CarregarGrid();
}

function loadJanelaCarregamentoSituacao() {
    _janelaCarregamentoSituacao = new JanelaCarregamentoSituacao();
    KoBindings(_janelaCarregamentoSituacao, "knockoutJanelaCarregamentoSituacao");

    HeaderAuditoria("JanelaCarregamentoSituacao", _janelaCarregamentoSituacao);

    _CRUDJanelaCarregamentoSituacao = new CRUDJanelaCarregamentoSituacao();
    KoBindings(_CRUDJanelaCarregamentoSituacao, "knockoutCRUDJanelaCarregamentoSituacao");

    _pesquisaJanelaCarregamentoSituacao = new PesquisaJanelaCarregamentoSituacao();
    KoBindings(_pesquisaJanelaCarregamentoSituacao, "knockoutPesquisaJanelaCarregamentoSituacao", false, _pesquisaJanelaCarregamentoSituacao.Pesquisar.id);

    loadGridJanelaCarregamentoSituacao();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarClick(e, sender) {
    Salvar(_janelaCarregamentoSituacao, "JanelaCarregamentoSituacao/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Successo, Localization.Resources.Gerais.Geral.CadastradoComSucesso);

                recarregarGridJanelaCarregamentoSituacao();
                limparCamposJanelaCarregamentoSituacao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_janelaCarregamentoSituacao, "JanelaCarregamentoSituacao/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AtualizadoComSucesso);

                recarregarGridJanelaCarregamentoSituacao();
                limparCamposJanelaCarregamentoSituacao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposJanelaCarregamentoSituacao();
}

function editarClick(registroSelecionado) {
    limparCamposJanelaCarregamentoSituacao();

    _janelaCarregamentoSituacao.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_janelaCarregamentoSituacao, "JanelaCarregamentoSituacao/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaJanelaCarregamentoSituacao.ExibirFiltros.visibleFade(false);

                controlarBotoesHabilitados();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    }, null);
}

function excluirClick() {   
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Gerais.Geral.DesejaExcluirRegistro, function () {
        ExcluirPorCodigo(_janelaCarregamentoSituacao, "JanelaCarregamentoSituacao/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);

                    recarregarGridJanelaCarregamentoSituacao();
                    limparCamposJanelaCarregamentoSituacao();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }, null);
    });
}

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

// #endregion Funções Associadas a Eventos

// #region Funções

function controlarBotoesHabilitados() {
    var isEdicao = _janelaCarregamentoSituacao.Codigo.val() > 0;

    _CRUDJanelaCarregamentoSituacao.Atualizar.visible(isEdicao);
    _CRUDJanelaCarregamentoSituacao.Excluir.visible(isEdicao);
    _CRUDJanelaCarregamentoSituacao.Cancelar.visible(isEdicao);
    _CRUDJanelaCarregamentoSituacao.Adicionar.visible(!isEdicao);
}

function limparCamposJanelaCarregamentoSituacao() {
    LimparCampos(_janelaCarregamentoSituacao);
    controlarBotoesHabilitados();
}

function recarregarGridJanelaCarregamentoSituacao() {
    _gridJanelaCarregamentoSituacao.CarregarGrid();
}

// #endregion Funções
