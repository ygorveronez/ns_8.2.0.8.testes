/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/TipoOcorrencia.js" />
/// <reference path="../../Consultas/TipoIntegracao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _pesquisaCargaEntregaEventoIntegracao;
var _pesquisaCargaEntregaEventoIntegracaoHistorico;
var _gridCargaEntregaEventoIntegracao;
var _gridCargaEntregaEventoIntegracaoHistorico;
var _detalheCargaEntregaEventoIntegracao;

var PesquisaCargaEntregaEventoIntegracao = function () {
    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Carga: " });
    this.TipoDeOcorrencia = PropertyEntity({ text: ko.observable("Tipo de ocorrência:"), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });

    this.TipoIntegracao = PropertyEntity({ text: "Tipo de integração:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    var dataPadrao = Global.DataAtual();
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", val: ko.observable(dataPadrao), def: dataPadrao, getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final:", val: ko.observable(dataPadrao), def: dataPadrao, getType: typesKnockout.date });
    this.SituacaoIntegracao = PropertyEntity({ text: "Situação da integração:", val: ko.observable(EnumSituacaoIntegracao.Todas), def: EnumSituacaoIntegracao.Todas, options: EnumSituacaoIntegracao.obterOpcoesPesquisa() });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Pesquisar = PropertyEntity({ text: "Pesquisar", type: types.event, eventClick: pesquisarCargaEntregaEventoIntegracaoClick, idGrid: guid(), visible: ko.observable(true), visibleFade: ko.observable(false) });
    this.ReenviarIntegracoesComFalha = PropertyEntity({ text: "Reenviar Integrações com Falha", type: types.event, eventClick: reenviarIntegracoesComFalhaClick, idGrid: guid(), visible: ko.observable(false), visibleFade: ko.observable(false) });
    this.ExibirFiltros = PropertyEntity({ text: "Filtros de Pesquisa", type: types.event, eventClick: exibirFiltrosCargaEntregaEventoIntegracaoClick, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });
};

var PesquisaCargaEntregaEventoIntegracaoHistorico = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

function loadGridCargaEntregaEventoIntegracao() {
    var historico = { descricao: "Histórico de Integração", id: guid(), evento: "onclick", metodo: exibirHistoricoCargaEntregaEventoIntegracaoClick };
    var auditoria = { descricao: "Auditoria", id: guid(), evento: "onclick", metodo: OpcaoAuditoria("CargaEntregaEventoIntegracao"), visibilidade: VisibilidadeOpcaoAuditoria };
    var reenviar = { descricao: "Reenviar", id: guid(), evento: "onclick", metodo: reenviarCargaEntregaEventoIntegracaoClick, visibilidade: reenviarCargaEntregaEventoIntegracaoVisible };
    var menuOpcoes = { tipo: TypeOptionMenu.list, tamanho: 5, opcoes: [historico, auditoria, reenviar] };

    var configuracoesExportacao = { url: "CargaEntregaEventoIntegracao/ExportarPesquisa", titulo: "CargaEntregaEventoIntegracao" };
    _gridCargaEntregaEventoIntegracao = new GridView(_pesquisaCargaEntregaEventoIntegracao.Pesquisar.idGrid, "CargaEntregaEventoIntegracao/Pesquisa", _pesquisaCargaEntregaEventoIntegracao, menuOpcoes, null, 10, null, true, false, undefined, 100, undefined, configuracoesExportacao);
    _gridCargaEntregaEventoIntegracao.CarregarGrid();
}

function loadGridCargaEntregaEventoIntegracaoHistorico() {
    var opcaoDownload = { descricao: "Download Arquivos", id: guid(), evento: "onclick", metodo: downloadArquivosCargaEntregaEventoIntegracaoHistoricoClick, tamanho: "20", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoDownload] };
    _gridCargaEntregaEventoIntegracaoHistorico = new GridView("grid-carga-entrega-evento-integracao-historico", "CargaEntregaEventoIntegracao/PesquisaIntegracaoHistorico", _pesquisaCargaEntregaEventoIntegracaoHistorico, menuOpcoes, { column: 1, dir: orderDir.desc });
}

function loadCargaEntregaEventoIntegracao() {
    _pesquisaCargaEntregaEventoIntegracao = new PesquisaCargaEntregaEventoIntegracao();
    KoBindings(_pesquisaCargaEntregaEventoIntegracao, "knockoutPesquisaCargaEntregaEventoIntegracao", false, _pesquisaCargaEntregaEventoIntegracao.Pesquisar.id);

    new BuscarTipoOcorrencia(_pesquisaCargaEntregaEventoIntegracao.TipoDeOcorrencia, null, null, null, null, null, null, null, true);
    new BuscarTipoIntegracao(_pesquisaCargaEntregaEventoIntegracao.TipoIntegracao, null, null, null, EnumGrupoTipoIntegracao.OcorrenciaEntrega, false);

    _pesquisaCargaEntregaEventoIntegracaoHistorico = new PesquisaCargaEntregaEventoIntegracaoHistorico();
    loadGridCargaEntregaEventoIntegracao();
    loadGridCargaEntregaEventoIntegracaoHistorico();
}

function exibirHistoricoCargaEntregaEventoIntegracaoClick(row) {
    _pesquisaCargaEntregaEventoIntegracaoHistorico.Codigo.val(row.Codigo);
    _gridCargaEntregaEventoIntegracaoHistorico.CarregarGrid();
    Global.abrirModal("divModalCargaEntregaEventoIntegracaoHistorico");
}

function downloadArquivosCargaEntregaEventoIntegracaoHistoricoClick(row) {
    executarDownload("CargaEntregaEventoIntegracao/DownloadArquivosIntegracaoHistorico", { Codigo: row.Codigo });
}

function pesquisarCargaEntregaEventoIntegracaoClick() {
    _pesquisaCargaEntregaEventoIntegracao.ReenviarIntegracoesComFalha.visible(_pesquisaCargaEntregaEventoIntegracao.SituacaoIntegracao.val() == EnumSituacaoIntegracao.ProblemaIntegracao);

    _gridCargaEntregaEventoIntegracao.CarregarGrid();
}

function reenviarIntegracoesComFalhaClick() {
    exibirConfirmacao("Atenção!", "Deseja realmente reenviar as integrações com falha?", function () {
        executarReST("CargaEntregaEventoIntegracao/ReenviarTodasIntegracoesComFalha", {
            DataInicial: _pesquisaCargaEntregaEventoIntegracao.DataInicial.val(),
            DataFinal: _pesquisaCargaEntregaEventoIntegracao.DataFinal.val()
        }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvios realizados com sucesso.");
                    _gridCargaEntregaEventoIntegracao.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso!", retorno.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
            }
        });
    });
}

function exibirFiltrosCargaEntregaEventoIntegracaoClick() {
    _pesquisaCargaEntregaEventoIntegracao.ExibirFiltros.visibleFade(!_pesquisaCargaEntregaEventoIntegracao.ExibirFiltros.visibleFade());
}

function auditoriaCargaEntregaEventoIntegracaoClick(row) {
    OpcaoAuditoria("CargaEntregaEventoIntegracao");
}

function reenviarCargaEntregaEventoIntegracaoVisible(row) {
    return row.CodigoSituacaoIntegracao == EnumSituacaoIntegracao.ProblemaIntegracao;
}

function reenviarCargaEntregaEventoIntegracaoClick(row) {
    exibirConfirmacao("Atenção!", "Deseja realmente reenviar a integração?", function () {
        executarReST("CargaEntregaEventoIntegracao/Reenviar", { Codigo: row.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio executado com sucesso.");
                    _gridCargaEntregaEventoIntegracao.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso!", retorno.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
            }
        });
    });
}