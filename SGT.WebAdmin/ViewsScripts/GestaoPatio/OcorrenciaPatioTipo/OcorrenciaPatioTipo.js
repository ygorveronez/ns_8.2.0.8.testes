/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumTipoOcorrenciaPatio.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDOcorrenciaPatioTipo;
var _gridOcorrenciaPatioTipo;
var _ocorrenciaPatioTipo;
var _pesquisaOcorrenciaPatioTipo;

/*
 * Declaração das Classes
 */

var CRUDOcorrenciaPatioTipo = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar / Novo", visible: true });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var OcorrenciaPatioTipo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable(""), maxlength: 200 });
    this.Status = PropertyEntity({ text: "*Situação: ", issue: 557, val: ko.observable(true), options: _status, def: true, required: true });
    this.Tipo = PropertyEntity({ text: "*Tipo: ", val: ko.observable(EnumTipoOcorrenciaPatio.NaoInformado), options: EnumTipoOcorrenciaPatio.obterOpcoes(), def: EnumTipoOcorrenciaPatio.NaoInformado, required: true });
}

var PesquisaOcorrenciaPatioTipo = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });
    this.Tipo = PropertyEntity({ text: "Tipo: ", val: ko.observable(EnumTipoOcorrenciaPatio.Todos), options: EnumTipoOcorrenciaPatio.obterOpcoesPesquisa(), def: EnumTipoOcorrenciaPatio.Todos });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridOcorrenciaPatioTipo, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridOcorrenciaPatioTipo() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "OcorrenciaPatioTipo/ExportarPesquisa", titulo: "Tipos de Ocorrência de Pátio" };

    _gridOcorrenciaPatioTipo = new GridViewExportacao(_pesquisaOcorrenciaPatioTipo.Pesquisar.idGrid, "OcorrenciaPatioTipo/Pesquisa", _pesquisaOcorrenciaPatioTipo, menuOpcoes, configuracoesExportacao);
    _gridOcorrenciaPatioTipo.CarregarGrid();
}

function loadOcorrenciaPatioTipo() {
    _ocorrenciaPatioTipo = new OcorrenciaPatioTipo();
    KoBindings(_ocorrenciaPatioTipo, "knockoutOcorrenciaPatioTipo");

    HeaderAuditoria("OcorrenciaPatioTipo", _ocorrenciaPatioTipo);

    _CRUDOcorrenciaPatioTipo = new CRUDOcorrenciaPatioTipo();
    KoBindings(_CRUDOcorrenciaPatioTipo, "knockoutCRUDOcorrenciaPatioTipo");

    _pesquisaOcorrenciaPatioTipo = new PesquisaOcorrenciaPatioTipo();
    KoBindings(_pesquisaOcorrenciaPatioTipo, "knockoutPesquisaOcorrenciaPatioTipo", false, _pesquisaOcorrenciaPatioTipo.Pesquisar.id);

    loadGridOcorrenciaPatioTipo();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    Salvar(_ocorrenciaPatioTipo, "OcorrenciaPatioTipo/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridOcorrenciaPatioTipo();
                limparCamposOcorrenciaPatioTipo();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_ocorrenciaPatioTipo, "OcorrenciaPatioTipo/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridOcorrenciaPatioTipo();
                limparCamposOcorrenciaPatioTipo();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposOcorrenciaPatioTipo();
}

function editarClick(registroSelecionado) {
    limparCamposOcorrenciaPatioTipo();

    _ocorrenciaPatioTipo.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_ocorrenciaPatioTipo, "OcorrenciaPatioTipo/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaOcorrenciaPatioTipo.ExibirFiltros.visibleFade(false);

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
        ExcluirPorCodigo(_ocorrenciaPatioTipo, "OcorrenciaPatioTipo/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridOcorrenciaPatioTipo();
                    limparCamposOcorrenciaPatioTipo();
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
    var isEdicao = _ocorrenciaPatioTipo.Codigo.val() > 0;

    _CRUDOcorrenciaPatioTipo.Atualizar.visible(isEdicao);
    _CRUDOcorrenciaPatioTipo.Excluir.visible(isEdicao);
    _CRUDOcorrenciaPatioTipo.Adicionar.visible(!isEdicao);
}

function limparCamposOcorrenciaPatioTipo() {
    LimparCampos(_ocorrenciaPatioTipo);
    controlarBotoesHabilitados();
}

function recarregarGridOcorrenciaPatioTipo() {
    _gridOcorrenciaPatioTipo.CarregarGrid();
}