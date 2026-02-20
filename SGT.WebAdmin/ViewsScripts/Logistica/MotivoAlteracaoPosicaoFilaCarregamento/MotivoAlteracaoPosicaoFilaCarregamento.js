/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDMotivoAlteracaoPosicaoFilaCarregamento;
var _gridMotivoAlteracaoPosicaoFilaCarregamento;
var _motivoAlteracaoPosicaoFilaCarregamento;
var _pesquisaMotivoAlteracaoPosicaoFilaCarregamento;

/*
 * Declaração das Classes
 */

var CRUDMotivoAlteracaoPosicaoFilaCarregamento = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar / Novo", visible: true });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var MotivoAlteracaoPosicaoFilaCarregamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 2000 });
    this.Status = PropertyEntity({ text: "*Situação: ", issue: 557, val: ko.observable(true), options: _status, def: true });
}

var PesquisaMotivoAlteracaoPosicaoFilaCarregamento = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridMotivoAlteracaoPosicaoFilaCarregamento, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridMotivoAlteracaoPosicaoFilaCarregamento() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "MotivoAlteracaoPosicaoFilaCarregamento/ExportarPesquisa", titulo: "Motivo da Alteração de Posição da Fila de Carregamento" };

    _gridMotivoAlteracaoPosicaoFilaCarregamento = new GridViewExportacao(_pesquisaMotivoAlteracaoPosicaoFilaCarregamento.Pesquisar.idGrid, "MotivoAlteracaoPosicaoFilaCarregamento/Pesquisa", _pesquisaMotivoAlteracaoPosicaoFilaCarregamento, menuOpcoes, configuracoesExportacao);
    _gridMotivoAlteracaoPosicaoFilaCarregamento.CarregarGrid();
}

function loadMotivoAlteracaoPosicaoFilaCarregamento() {
    _motivoAlteracaoPosicaoFilaCarregamento = new MotivoAlteracaoPosicaoFilaCarregamento();
    KoBindings(_motivoAlteracaoPosicaoFilaCarregamento, "knockoutMotivoAlteracaoPosicaoFilaCarregamento");

    HeaderAuditoria("MotivoAlteracaoPosicaoFilaCarregamento", _motivoAlteracaoPosicaoFilaCarregamento);

    _CRUDMotivoAlteracaoPosicaoFilaCarregamento = new CRUDMotivoAlteracaoPosicaoFilaCarregamento();
    KoBindings(_CRUDMotivoAlteracaoPosicaoFilaCarregamento, "knockoutCRUDMotivoAlteracaoPosicaoFilaCarregamento");

    _pesquisaMotivoAlteracaoPosicaoFilaCarregamento = new PesquisaMotivoAlteracaoPosicaoFilaCarregamento();
    KoBindings(_pesquisaMotivoAlteracaoPosicaoFilaCarregamento, "knockoutPesquisaMotivoAlteracaoPosicaoFilaCarregamento", false, _pesquisaMotivoAlteracaoPosicaoFilaCarregamento.Pesquisar.id);

    loadGridMotivoAlteracaoPosicaoFilaCarregamento();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    Salvar(_motivoAlteracaoPosicaoFilaCarregamento, "MotivoAlteracaoPosicaoFilaCarregamento/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridMotivoAlteracaoPosicaoFilaCarregamento();
                limparCamposMotivoAlteracaoPosicaoFilaCarregamento();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_motivoAlteracaoPosicaoFilaCarregamento, "MotivoAlteracaoPosicaoFilaCarregamento/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridMotivoAlteracaoPosicaoFilaCarregamento();
                limparCamposMotivoAlteracaoPosicaoFilaCarregamento();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposMotivoAlteracaoPosicaoFilaCarregamento();
}

function editarClick(registroSelecionado) {
    limparCamposMotivoAlteracaoPosicaoFilaCarregamento();

    _motivoAlteracaoPosicaoFilaCarregamento.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_motivoAlteracaoPosicaoFilaCarregamento, "MotivoAlteracaoPosicaoFilaCarregamento/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaMotivoAlteracaoPosicaoFilaCarregamento.ExibirFiltros.visibleFade(false);

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
        ExcluirPorCodigo(_motivoAlteracaoPosicaoFilaCarregamento, "MotivoAlteracaoPosicaoFilaCarregamento/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridMotivoAlteracaoPosicaoFilaCarregamento();
                    limparCamposMotivoAlteracaoPosicaoFilaCarregamento();
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
    var isEdicao = _motivoAlteracaoPosicaoFilaCarregamento.Codigo.val() > 0;

    _CRUDMotivoAlteracaoPosicaoFilaCarregamento.Atualizar.visible(isEdicao);
    _CRUDMotivoAlteracaoPosicaoFilaCarregamento.Excluir.visible(isEdicao);
    _CRUDMotivoAlteracaoPosicaoFilaCarregamento.Adicionar.visible(!isEdicao);
}

function limparCamposMotivoAlteracaoPosicaoFilaCarregamento() {
    LimparCampos(_motivoAlteracaoPosicaoFilaCarregamento);
    controlarBotoesHabilitados();
}

function recarregarGridMotivoAlteracaoPosicaoFilaCarregamento() {
    _gridMotivoAlteracaoPosicaoFilaCarregamento.CarregarGrid();
}