/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDMotivoAdvertenciaTransportador;
var _gridMotivoAdvertenciaTransportador;
var _motivoAdvertenciaTransportador;
var _pesquisaMotivoAdvertenciaTransportador;

/*
 * Declaração das Classes
 */

var CRUDMotivoAdvertenciaTransportador = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar / Novo", visible: true });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var MotivoAdvertenciaTransportador = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 2000 });
    this.Pontuacao = PropertyEntity({ val: ko.observable(""), text: "*Desconto Pontuação:", def: "", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, required: true, maxlength: 10 });
    this.Status = PropertyEntity({ text: "*Situação: ", issue: 557, val: ko.observable(true), options: _status, def: true });
}

var PesquisaMotivoAdvertenciaTransportador = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridMotivoAdvertenciaTransportador, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridMotivoAdvertenciaTransportador() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "MotivoAdvertenciaTransportador/ExportarPesquisa", titulo: "Motivo de Advertência do Transportador" };

    _gridMotivoAdvertenciaTransportador = new GridViewExportacao(_pesquisaMotivoAdvertenciaTransportador.Pesquisar.idGrid, "MotivoAdvertenciaTransportador/Pesquisa", _pesquisaMotivoAdvertenciaTransportador, menuOpcoes, configuracoesExportacao);
    _gridMotivoAdvertenciaTransportador.CarregarGrid();
}

function loadMotivoAdvertenciaTransportador() {
    _motivoAdvertenciaTransportador = new MotivoAdvertenciaTransportador();
    KoBindings(_motivoAdvertenciaTransportador, "knockoutMotivoAdvertenciaTransportador");

    HeaderAuditoria("MotivoAdvertenciaTransportador", _motivoAdvertenciaTransportador);

    _CRUDMotivoAdvertenciaTransportador = new CRUDMotivoAdvertenciaTransportador();
    KoBindings(_CRUDMotivoAdvertenciaTransportador, "knockoutCRUDMotivoAdvertenciaTransportador");

    _pesquisaMotivoAdvertenciaTransportador = new PesquisaMotivoAdvertenciaTransportador();
    KoBindings(_pesquisaMotivoAdvertenciaTransportador, "knockoutPesquisaMotivoAdvertenciaTransportador", false, _pesquisaMotivoAdvertenciaTransportador.Pesquisar.id);

    loadGridMotivoAdvertenciaTransportador();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    Salvar(_motivoAdvertenciaTransportador, "MotivoAdvertenciaTransportador/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridMotivoAdvertenciaTransportador();
                limparCamposMotivoAdvertenciaTransportador();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_motivoAdvertenciaTransportador, "MotivoAdvertenciaTransportador/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridMotivoAdvertenciaTransportador();
                limparCamposMotivoAdvertenciaTransportador();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposMotivoAdvertenciaTransportador();
}

function editarClick(registroSelecionado) {
    limparCamposMotivoAdvertenciaTransportador();

    _motivoAdvertenciaTransportador.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_motivoAdvertenciaTransportador, "MotivoAdvertenciaTransportador/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaMotivoAdvertenciaTransportador.ExibirFiltros.visibleFade(false);

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
        ExcluirPorCodigo(_motivoAdvertenciaTransportador, "MotivoAdvertenciaTransportador/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridMotivoAdvertenciaTransportador();
                    limparCamposMotivoAdvertenciaTransportador();
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
    var isEdicao = _motivoAdvertenciaTransportador.Codigo.val() > 0;

    _CRUDMotivoAdvertenciaTransportador.Atualizar.visible(isEdicao);
    _CRUDMotivoAdvertenciaTransportador.Excluir.visible(isEdicao);
    _CRUDMotivoAdvertenciaTransportador.Adicionar.visible(!isEdicao);
}

function limparCamposMotivoAdvertenciaTransportador() {
    LimparCampos(_motivoAdvertenciaTransportador);
    controlarBotoesHabilitados();
}

function recarregarGridMotivoAdvertenciaTransportador() {
    _gridMotivoAdvertenciaTransportador.CarregarGrid();
}
