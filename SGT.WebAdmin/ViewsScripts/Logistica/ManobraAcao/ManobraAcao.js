/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumTipoManobraAcao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDManobraAcao;
var _manobraAcao;
var _pesquisaManobraAcao;
var _gridManobraAcao;

/*
 * Declaração das Classes
 */

var CRUDManobraAcao = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var ManobraAcao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.LocalDestinoObrigatorio = PropertyEntity({ text: "Local de Destino Obrigatório", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoManobraAcao.NaoInformado), options: EnumTipoManobraAcao.obterOpcoes(), def: EnumTipoManobraAcao.NaoInformado, text: "*Tipo: ", required: true });
}

var PesquisaManobraAcao = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridManobraAcao, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridManobraAcao() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "ManobraAcao/ExportarPesquisa", titulo: "Ações de Manobra" };

    _gridManobraAcao = new GridViewExportacao(_pesquisaManobraAcao.Pesquisar.idGrid, "ManobraAcao/Pesquisa", _pesquisaManobraAcao, menuOpcoes, configuracoesExportacao);
    _gridManobraAcao.CarregarGrid();
}

function loadManobraAcao() {
    _manobraAcao = new ManobraAcao();
    KoBindings(_manobraAcao, "knockoutManobraAcao");

    HeaderAuditoria("ManobraAcao", _manobraAcao);

    _CRUDManobraAcao = new CRUDManobraAcao();
    KoBindings(_CRUDManobraAcao, "knockoutCRUDManobraAcao");

    _pesquisaManobraAcao = new PesquisaManobraAcao();
    KoBindings(_pesquisaManobraAcao, "knockoutPesquisaManobraAcao", false, _pesquisaManobraAcao.Pesquisar.id);

    loadGridManobraAcao();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    Salvar(_manobraAcao, "ManobraAcao/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridManobraAcao();
                limparCamposManobraAcao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_manobraAcao, "ManobraAcao/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridManobraAcao();
                limparCamposManobraAcao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposManobraAcao();
}

function editarClick(registroSelecionado) {
    limparCamposManobraAcao();

    _manobraAcao.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_manobraAcao, "ManobraAcao/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaManobraAcao.ExibirFiltros.visibleFade(false);

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
        ExcluirPorCodigo(_manobraAcao, "ManobraAcao/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridManobraAcao();
                    limparCamposManobraAcao();
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
    var isEdicao = _manobraAcao.Codigo.val() > 0;

    _CRUDManobraAcao.Atualizar.visible(isEdicao);
    _CRUDManobraAcao.Excluir.visible(isEdicao);
    _CRUDManobraAcao.Cancelar.visible(isEdicao);
    _CRUDManobraAcao.Adicionar.visible(!isEdicao);
}

function limparCamposManobraAcao() {
    LimparCampos(_manobraAcao);
    controlarBotoesHabilitados();
}

function recarregarGridManobraAcao() {
    _gridManobraAcao.CarregarGrid();
}