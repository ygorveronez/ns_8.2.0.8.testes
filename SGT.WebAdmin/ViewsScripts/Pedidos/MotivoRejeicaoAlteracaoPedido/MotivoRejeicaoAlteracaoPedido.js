/// <reference path="../../Enumeradores/EnumTipoMotivoRejeicaoAlteracaoPedido.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDMotivoRejeicaoAlteracaoPedido;
var _motivoRejeicaoAlteracaoPedido;
var _pesquisaMotivoRejeicaoAlteracaoPedido;
var _gridMotivoRejeicaoAlteracaoPedido;

/*
 * Declaração das Classes
 */

var CRUDMotivoRejeicaoAlteracaoPedido = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var MotivoRejeicaoAlteracaoPedido = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 2000 });
    this.Status = PropertyEntity({ text: "*Situação: ", issue: 557, val: ko.observable(true), options: _status, def: true });
    this.Tipo = PropertyEntity({ text: "*Tipo: ", val: ko.observable(EnumTipoMotivoRejeicaoAlteracaoPedido.Todos), options: EnumTipoMotivoRejeicaoAlteracaoPedido.obterOpcoes(), def: EnumTipoMotivoRejeicaoAlteracaoPedido.Todos });
}

var PesquisaMotivoRejeicaoAlteracaoPedido = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });
    this.Tipo = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumTipoMotivoRejeicaoAlteracaoPedido.Todos), options: EnumTipoMotivoRejeicaoAlteracaoPedido.obterOpcoes(), def: EnumTipoMotivoRejeicaoAlteracaoPedido.Todos });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridMotivoRejeicaoAlteracaoPedido, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridMotivoRejeicaoAlteracaoPedido() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "MotivoRejeicaoAlteracaoPedido/ExportarPesquisa", titulo: "Motivo de Rejeição de Alteração de Pedido" };

    _gridMotivoRejeicaoAlteracaoPedido = new GridViewExportacao(_pesquisaMotivoRejeicaoAlteracaoPedido.Pesquisar.idGrid, "MotivoRejeicaoAlteracaoPedido/Pesquisa", _pesquisaMotivoRejeicaoAlteracaoPedido, menuOpcoes, configuracoesExportacao);
    _gridMotivoRejeicaoAlteracaoPedido.CarregarGrid();
}

function loadMotivoRejeicaoAlteracaoPedido() {
    _motivoRejeicaoAlteracaoPedido = new MotivoRejeicaoAlteracaoPedido();
    KoBindings(_motivoRejeicaoAlteracaoPedido, "knockoutMotivoRejeicaoAlteracaoPedido");

    HeaderAuditoria("MotivoRejeicaoAlteracaoPedido", _motivoRejeicaoAlteracaoPedido);

    _CRUDMotivoRejeicaoAlteracaoPedido = new CRUDMotivoRejeicaoAlteracaoPedido();
    KoBindings(_CRUDMotivoRejeicaoAlteracaoPedido, "knockoutCRUDMotivoRejeicaoAlteracaoPedido");

    _pesquisaMotivoRejeicaoAlteracaoPedido = new PesquisaMotivoRejeicaoAlteracaoPedido();
    KoBindings(_pesquisaMotivoRejeicaoAlteracaoPedido, "knockoutPesquisaMotivoRejeicaoAlteracaoPedido", false, _pesquisaMotivoRejeicaoAlteracaoPedido.Pesquisar.id);

    loadGridMotivoRejeicaoAlteracaoPedido();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    Salvar(_motivoRejeicaoAlteracaoPedido, "MotivoRejeicaoAlteracaoPedido/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridMotivoRejeicaoAlteracaoPedido();
                limparCamposMotivoRejeicaoAlteracaoPedido();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_motivoRejeicaoAlteracaoPedido, "MotivoRejeicaoAlteracaoPedido/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridMotivoRejeicaoAlteracaoPedido();
                limparCamposMotivoRejeicaoAlteracaoPedido();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposMotivoRejeicaoAlteracaoPedido();
}

function editarClick(registroSelecionado) {
    limparCamposMotivoRejeicaoAlteracaoPedido();

    _motivoRejeicaoAlteracaoPedido.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_motivoRejeicaoAlteracaoPedido, "MotivoRejeicaoAlteracaoPedido/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaMotivoRejeicaoAlteracaoPedido.ExibirFiltros.visibleFade(false);

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
        ExcluirPorCodigo(_motivoRejeicaoAlteracaoPedido, "MotivoRejeicaoAlteracaoPedido/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridMotivoRejeicaoAlteracaoPedido();
                    limparCamposMotivoRejeicaoAlteracaoPedido();
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
 * Declaração das Funções Privadas
 */

function controlarBotoesHabilitados() {
    var isEdicao = _motivoRejeicaoAlteracaoPedido.Codigo.val() > 0;

    _CRUDMotivoRejeicaoAlteracaoPedido.Atualizar.visible(isEdicao);
    _CRUDMotivoRejeicaoAlteracaoPedido.Excluir.visible(isEdicao);
    _CRUDMotivoRejeicaoAlteracaoPedido.Cancelar.visible(isEdicao);
    _CRUDMotivoRejeicaoAlteracaoPedido.Adicionar.visible(!isEdicao);
}

function limparCamposMotivoRejeicaoAlteracaoPedido() {
    LimparCampos(_motivoRejeicaoAlteracaoPedido);
    controlarBotoesHabilitados();
}

function recarregarGridMotivoRejeicaoAlteracaoPedido() {
    _gridMotivoRejeicaoAlteracaoPedido.CarregarGrid();
}