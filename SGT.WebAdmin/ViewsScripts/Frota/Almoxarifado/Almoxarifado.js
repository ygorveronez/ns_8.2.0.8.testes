/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Consultas/Usuario.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDAlmoxarifado;
var _almoxarifado;
var _pesquisaAlmoxarifado;
var _gridAlmoxarifado;

/*
 * Declaração das Classes
 */

var Almoxarifado = function () {
    this.Codigo = PropertyEntity({ text: "Código:", val: ko.observable(0), def: 0, getType: typesKnockout.int, enable: false, visible: true });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, getType: typesKnockout.string, val: ko.observable(""), maxlength: 200, required: ko.observable(true) });
    this.Email = PropertyEntity({ text: "E-mail:", getType: typesKnockout.multiplesEmails, val: ko.observable(""), maxlength: 300 });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Empresa:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true) });
    this.FuncionarioResponsavel = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Funcionário Responsável:", idBtnSearch: guid(), cssClass: ko.observable("col col-xs-12 col-md-4"), required: ko.observable(true) });
    this.Status = PropertyEntity({ text: "*Situação: ", issue: 557, val: ko.observable(true), options: _status, def: true });
}

var CRUDAlmoxarifado = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var PesquisaAlmoxarifado = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable(""), maxlength: 200, cssClass: ko.observable("col col-xs-8 col-md-4") });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Status = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridAlmoxarifado, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridAlmoxarifado() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "Almoxarifado/ExportarPesquisa", titulo: "Almoxarifado" };

    _gridAlmoxarifado = new GridViewExportacao(_pesquisaAlmoxarifado.Pesquisar.idGrid, "Almoxarifado/Pesquisa", _pesquisaAlmoxarifado, menuOpcoes, configuracoesExportacao);
    _gridAlmoxarifado.CarregarGrid();
}

function loadAlmoxarifado() {
    _almoxarifado = new Almoxarifado();
    KoBindings(_almoxarifado, "knockoutAlmoxarifado");

    HeaderAuditoria("Almoxarifado", _almoxarifado);

    _CRUDAlmoxarifado = new CRUDAlmoxarifado();
    KoBindings(_CRUDAlmoxarifado, "knockoutCRUDAlmoxarifado");

    _pesquisaAlmoxarifado = new PesquisaAlmoxarifado();
    KoBindings(_pesquisaAlmoxarifado, "knockoutPesquisaAlmoxarifado", false, _pesquisaAlmoxarifado.Pesquisar.id);

    new BuscarEmpresa(_pesquisaAlmoxarifado.Empresa);
    new BuscarEmpresa(_almoxarifado.Empresa);
    new BuscarFuncionario(_almoxarifado.FuncionarioResponsavel);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiNFe) {
        _pesquisaAlmoxarifado.Empresa.visible(false);
        _almoxarifado.Empresa.visible(false);
        _almoxarifado.Empresa.required(false);

        _pesquisaAlmoxarifado.Descricao.cssClass("col col-xs-8 col-md-8");
        _almoxarifado.FuncionarioResponsavel.cssClass("col col-xs-12 col-md-8");
    }

    loadGridAlmoxarifado();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    Salvar(_almoxarifado, "Almoxarifado/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridAlmoxarifado();
                limparCamposAlmoxarifado();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_almoxarifado, "Almoxarifado/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridAlmoxarifado();
                limparCamposAlmoxarifado();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposAlmoxarifado();
}

function editarClick(registroSelecionado) {
    limparCamposAlmoxarifado();

    _almoxarifado.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_almoxarifado, "Almoxarifado/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaAlmoxarifado.ExibirFiltros.visibleFade(false);

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
        ExcluirPorCodigo(_almoxarifado, "Almoxarifado/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridAlmoxarifado();
                    limparCamposAlmoxarifado();
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

function controlarBotoesHabilitados(isEdicao) {
    _CRUDAlmoxarifado.Atualizar.visible(isEdicao);
    _CRUDAlmoxarifado.Excluir.visible(isEdicao);
    _CRUDAlmoxarifado.Cancelar.visible(isEdicao);
    _CRUDAlmoxarifado.Adicionar.visible(!isEdicao);
}

function limparCamposAlmoxarifado() {
    var isEdicao = false;

    controlarBotoesHabilitados(isEdicao);
    LimparCampos(_almoxarifado);
}

function recarregarGridAlmoxarifado() {
    _gridAlmoxarifado.CarregarGrid();
}