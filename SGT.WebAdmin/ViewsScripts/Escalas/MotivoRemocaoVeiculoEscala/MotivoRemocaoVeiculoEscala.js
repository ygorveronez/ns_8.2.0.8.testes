/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDMotivoRemocaoVeiculoEscala;
var _gridMotivoRemocaoVeiculoEscala;
var _motivoRemocaoVeiculoEscala;
var _pesquisaMotivoRemocaoVeiculoEscala;

/*
 * Declaração das Classes
 */

var CRUDMotivoRemocaoVeiculoEscala = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar / Novo", visible: true });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var MotivoRemocaoVeiculoEscala = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 2000 });
    this.Status = PropertyEntity({ text: "*Situação: ", issue: 557, val: ko.observable(true), options: _status, def: true });
}

var PesquisaMotivoRemocaoVeiculoEscala = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridMotivoRemocaoVeiculoEscala, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridMotivoRemocaoVeiculoEscala() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "MotivoRemocaoVeiculoEscala/ExportarPesquisa", titulo: "Motivo de Remoção do Veículo da Escala" };

    _gridMotivoRemocaoVeiculoEscala = new GridViewExportacao(_pesquisaMotivoRemocaoVeiculoEscala.Pesquisar.idGrid, "MotivoRemocaoVeiculoEscala/Pesquisa", _pesquisaMotivoRemocaoVeiculoEscala, menuOpcoes, configuracoesExportacao);
    _gridMotivoRemocaoVeiculoEscala.CarregarGrid();
}

function loadMotivoRemocaoVeiculoEscala() {
    _motivoRemocaoVeiculoEscala = new MotivoRemocaoVeiculoEscala();
    KoBindings(_motivoRemocaoVeiculoEscala, "knockoutMotivoRemocaoVeiculoEscala");

    HeaderAuditoria("MotivoRemocaoVeiculoEscala", _motivoRemocaoVeiculoEscala);

    _CRUDMotivoRemocaoVeiculoEscala = new CRUDMotivoRemocaoVeiculoEscala();
    KoBindings(_CRUDMotivoRemocaoVeiculoEscala, "knockoutCRUDMotivoRemocaoVeiculoEscala");

    _pesquisaMotivoRemocaoVeiculoEscala = new PesquisaMotivoRemocaoVeiculoEscala();
    KoBindings(_pesquisaMotivoRemocaoVeiculoEscala, "knockoutPesquisaMotivoRemocaoVeiculoEscala", false, _pesquisaMotivoRemocaoVeiculoEscala.Pesquisar.id);

    loadGridMotivoRemocaoVeiculoEscala();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    Salvar(_motivoRemocaoVeiculoEscala, "MotivoRemocaoVeiculoEscala/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridMotivoRemocaoVeiculoEscala();
                limparCamposMotivoRemocaoVeiculoEscala();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_motivoRemocaoVeiculoEscala, "MotivoRemocaoVeiculoEscala/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridMotivoRemocaoVeiculoEscala();
                limparCamposMotivoRemocaoVeiculoEscala();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposMotivoRemocaoVeiculoEscala();
}

function editarClick(registroSelecionado) {
    limparCamposMotivoRemocaoVeiculoEscala();

    _motivoRemocaoVeiculoEscala.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_motivoRemocaoVeiculoEscala, "MotivoRemocaoVeiculoEscala/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaMotivoRemocaoVeiculoEscala.ExibirFiltros.visibleFade(false);

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
        ExcluirPorCodigo(_motivoRemocaoVeiculoEscala, "MotivoRemocaoVeiculoEscala/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridMotivoRemocaoVeiculoEscala();
                    limparCamposMotivoRemocaoVeiculoEscala();
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
    var isEdicao = _motivoRemocaoVeiculoEscala.Codigo.val() > 0;

    _CRUDMotivoRemocaoVeiculoEscala.Atualizar.visible(isEdicao);
    _CRUDMotivoRemocaoVeiculoEscala.Excluir.visible(isEdicao);
    _CRUDMotivoRemocaoVeiculoEscala.Adicionar.visible(!isEdicao);
}

function limparCamposMotivoRemocaoVeiculoEscala() {
    LimparCampos(_motivoRemocaoVeiculoEscala);
    controlarBotoesHabilitados();
}

function recarregarGridMotivoRemocaoVeiculoEscala() {
    _gridMotivoRemocaoVeiculoEscala.CarregarGrid();
}
