/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../Enumeradores/EnumSimNao.js" />
/// <reference path="../../Consultas/Irregularidade.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDMotivoDesacordo;
var _motivoDesacordo;
var _pesquisaMotivoDesacordo;
var _gridMotivoDesacordo;

/*
 * Declaração das Classes
 */

var CRUDMotivoDesacordo = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var MotivoDesacordo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 300 });
    this.Situacao = PropertyEntity({ text: "*Situação: ", issue: 557, val: ko.observable(true), options: _status, def: true });
    this.SubstituiCTe = PropertyEntity({ text: "*Substitui CT-e: ", val: ko.observable(true), options: EnumSimNao.obterOpcoes(), def: true });
    this.Irregularidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Irregularidade:", required: true, idBtnSearch: guid() });
}

var PesquisaMotivoDesacordo = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusFemPesquisa, def: 0 });
    this.SubstituiCTe = PropertyEntity({ text: "Substitui CT-e: ", val: ko.observable(true), options: EnumSimNao.obterOpcoes(), def: true });
    this.Irregularidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Irregularidade:", idBtnSearch: guid() });
    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridMotivoDesacordo, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridMotivoDesacordo() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "MotivoDesacordo/ExportarPesquisa", titulo: "Motivo de Desacordo" };

    _gridMotivoDesacordo = new GridViewExportacao(_pesquisaMotivoDesacordo.Pesquisar.idGrid, "MotivoDesacordo/Pesquisa", _pesquisaMotivoDesacordo, menuOpcoes, configuracoesExportacao);
    _gridMotivoDesacordo.CarregarGrid();
}

function loadMotivoDesacordo() {
    _motivoDesacordo = new MotivoDesacordo();
    KoBindings(_motivoDesacordo, "knockoutMotivoDesacordo");

    HeaderAuditoria("MotivoDesacordo", _motivoDesacordo);

    _CRUDMotivoDesacordo = new CRUDMotivoDesacordo();
    KoBindings(_CRUDMotivoDesacordo, "knockoutCRUDMotivoDesacordo");

    _pesquisaMotivoDesacordo = new PesquisaMotivoDesacordo();
    KoBindings(_pesquisaMotivoDesacordo, "knockoutPesquisaMotivoDesacordo", false, _pesquisaMotivoDesacordo.Pesquisar.id);

    BuscarIrregularidades(_motivoDesacordo.Irregularidade);
    BuscarIrregularidades(_pesquisaMotivoDesacordo.Irregularidade);

    loadGridMotivoDesacordo();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    Salvar(_motivoDesacordo, "MotivoDesacordo/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridMotivoDesacordo();
                limparCamposMotivoDesacordo();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_motivoDesacordo, "MotivoDesacordo/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridMotivoDesacordo();
                limparCamposMotivoDesacordo();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposMotivoDesacordo();
}

function editarClick(registroSelecionado) {
    limparCamposMotivoDesacordo();

    _motivoDesacordo.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_motivoDesacordo, "MotivoDesacordo/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaMotivoDesacordo.ExibirFiltros.visibleFade(false);

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
        ExcluirPorCodigo(_motivoDesacordo, "MotivoDesacordo/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridMotivoDesacordo();
                    limparCamposMotivoDesacordo();
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
    var isEdicao = _motivoDesacordo.Codigo.val() > 0;

    _CRUDMotivoDesacordo.Atualizar.visible(isEdicao);
    _CRUDMotivoDesacordo.Excluir.visible(isEdicao);
    _CRUDMotivoDesacordo.Cancelar.visible(isEdicao);
    _CRUDMotivoDesacordo.Adicionar.visible(!isEdicao);
}

function limparCamposMotivoDesacordo() {
    LimparCampos(_motivoDesacordo);
    controlarBotoesHabilitados();
}

function recarregarGridMotivoDesacordo() {
    _gridMotivoDesacordo.CarregarGrid();
}