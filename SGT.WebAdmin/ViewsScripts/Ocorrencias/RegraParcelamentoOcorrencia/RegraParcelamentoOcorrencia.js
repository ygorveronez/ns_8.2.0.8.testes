/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumPeriodoFechamento.js" />
/// <reference path="Parcelamento.js" />

// #region Objetos Globais do Arquivo

var _CRUDRegraParcelamentoOcorrencia;
var _gridRegraParcelamentoOcorrencia;
var _pesquisaRegraParcelamentoOcorrencia;
var _regraParcelamentoOcorrencia;

// #endregion

// #region Classes

var CRUDRegraParcelamentoOcorrencia = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar" });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var PesquisaRegraParcelamentoOcorrencia = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", maxlength: 200 });
    this.Status = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridRegraParcelamentoOcorrencia, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

var RegraParcelamentoOcorrencia = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, maxlength: 200 });
    this.PeriodoFaturamento = PropertyEntity({ text: "*Período do Faturamento: ", val: ko.observable(EnumPeriodoFechamento.Mensal), options: EnumPeriodoFechamento.obterOpcoes(), def: EnumPeriodoFechamento.Mensal, enable: ko.observable(true) });
    this.QuantidadePeriodos = PropertyEntity({ text: "*Quantidades de Períodos:", getType: typesKnockout.int, required: true, maxlength: 2 });
    this.Status = PropertyEntity({ text: "*Situação: ", val: ko.observable(true), options: _status, def: true });
}

// #endregion

// #region Funções de Inicialização

function loadGridRegraParcelamentoOcorrencia() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "RegraParcelamentoOcorrencia/ExportarPesquisa", titulo: "Regras de Parcelamento de Ocorrência" };

    _gridRegraParcelamentoOcorrencia = new GridViewExportacao(_pesquisaRegraParcelamentoOcorrencia.Pesquisar.idGrid, "RegraParcelamentoOcorrencia/Pesquisa", _pesquisaRegraParcelamentoOcorrencia, menuOpcoes, configuracoesExportacao);
    _gridRegraParcelamentoOcorrencia.CarregarGrid();
}

function loadRegraParcelamentoOcorrencia() {
    _regraParcelamentoOcorrencia = new RegraParcelamentoOcorrencia();
    KoBindings(_regraParcelamentoOcorrencia, "knockoutRegraParcelamentoOcorrencia");

    HeaderAuditoria("RegraParcelamentoOcorrencia", _regraParcelamentoOcorrencia);

    _CRUDRegraParcelamentoOcorrencia = new CRUDRegraParcelamentoOcorrencia();
    KoBindings(_CRUDRegraParcelamentoOcorrencia, "knockoutCRUDRegraParcelamentoOcorrencia");

    _pesquisaRegraParcelamentoOcorrencia = new PesquisaRegraParcelamentoOcorrencia();
    KoBindings(_pesquisaRegraParcelamentoOcorrencia, "knockoutPesquisaRegraParcelamentoOcorrencia", false, _pesquisaRegraParcelamentoOcorrencia.Pesquisar.id);

    loadParcelamento();
    loadGridRegraParcelamentoOcorrencia();
}

// #endregion

// #region Funções Associadas a Eventos

function adicionarClick() {
    if (!ValidarCamposObrigatorios(_regraParcelamentoOcorrencia)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return;
    }

    executarReST("RegraParcelamentoOcorrencia/Adicionar", obterRegraParcelamentoOcorrenciaSalvar(), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso!");
                recarregarGridRegraParcelamentoOcorrencia();
                limparCamposRegraParcelamentoOcorrencia();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function atualizarClick() {
    if (!ValidarCamposObrigatorios(_regraParcelamentoOcorrencia)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return;
    }

    executarReST("RegraParcelamentoOcorrencia/Atualizar", obterRegraParcelamentoOcorrenciaSalvar(), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso!");
                recarregarGridRegraParcelamentoOcorrencia();
                limparCamposRegraParcelamentoOcorrencia();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function cancelarClick() {
    limparCamposRegraParcelamentoOcorrencia();
}

function editarClick(registroSelecionado) {
    limparCamposRegraParcelamentoOcorrencia();

    executarReST("RegraParcelamentoOcorrencia/BuscarPorCodigo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaRegraParcelamentoOcorrencia.ExibirFiltros.visibleFade(false);

                PreencherObjetoKnout(_regraParcelamentoOcorrencia, { Data: retorno.Data.Regra });
                preencherParcelamento(retorno.Data.Parcelamentos);
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
        executarReST("RegraParcelamentoOcorrencia/ExcluirPorCodigo", { Codigo: _regraParcelamentoOcorrencia.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridRegraParcelamentoOcorrencia();
                    limparCamposRegraParcelamentoOcorrencia();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

// #endregion

// #region Funções Privadas

function controlarBotoesHabilitados() {
    var isEdicao = _regraParcelamentoOcorrencia.Codigo.val() > 0;

    _CRUDRegraParcelamentoOcorrencia.Atualizar.visible(isEdicao);
    _CRUDRegraParcelamentoOcorrencia.Excluir.visible(isEdicao);
    _CRUDRegraParcelamentoOcorrencia.Adicionar.visible(!isEdicao);
}

function limparCamposRegraParcelamentoOcorrencia() {
    LimparCampos(_regraParcelamentoOcorrencia);
    limparCamposParcelamento();
    controlarBotoesHabilitados();
}

function obterRegraParcelamentoOcorrenciaSalvar() {
    var regraParcelamentoOcorrencia = RetornarObjetoPesquisa(_regraParcelamentoOcorrencia);

    preencherParcelamentoSalvar(regraParcelamentoOcorrencia);

    return regraParcelamentoOcorrencia;
}

function recarregarGridRegraParcelamentoOcorrencia() {
    _gridRegraParcelamentoOcorrencia.CarregarGrid();
}

// #endregion
