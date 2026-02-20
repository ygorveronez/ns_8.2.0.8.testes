/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _pesquisaPracaPedagioTarifaIntegracao;
var _pesquisaPracaPedagioTarifaIntegracaoHistorico;
var _gridPracaPedagioTarifaIntegracao;
var _gridPracaPedagioTarifaIntegracaoHistorico;

var PesquisaPracaPedagioTarifaIntegracao = function () {
    var dataPadrao = Global.DataAtual();
    this.DataInicial = PropertyEntity({ text: Localization.Resources.Gerais.Geral.DataInicial.getFieldDescription(), val: ko.observable(dataPadrao), def: dataPadrao, getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: Localization.Resources.Gerais.Geral.DataFinal.getFieldDescription(), val: ko.observable(dataPadrao), def: dataPadrao, getType: typesKnockout.date });
    this.SituacaoIntegracao = PropertyEntity({ text: Localization.Resources.Logistica.PracaPedagio.SituacaoIntegracao, val: ko.observable(EnumSituacaoIntegracao.Todas), def: EnumSituacaoIntegracao.Todas, options: EnumSituacaoIntegracao.obterOpcoesPesquisa() });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;
    this.Pesquisar = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Pesquisar, type: types.event, eventClick: pesquisarPracaPedagioTarifaIntegracaoClick, idGrid: guid(), visible: ko.observable(true), visibleFade: ko.observable(false) });
    this.ExibirFiltros = PropertyEntity({ text: Localization.Resources.Gerais.Geral.FiltrosPequisa, type: types.event, eventClick: exibirFiltrosPracaPedagioTarifaIntegracaoClick, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });
    this.ExecutarIntegracao = PropertyEntity({ text: Localization.Resources.Logistica.PracaPedagio.ExecutarIntegracao, type: types.event, eventClick: executarPracaPedagioTarifaIntegracaoClick, idGrid: guid(), visible: ko.observable(true)});
}

var PesquisaPracaPedagioTarifaIntegracaoHistorico = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

function loadGridPracaPedagioTarifaIntegracao() {
    var reenviar = { descricao: Localization.Resources.Gerais.Geral.Reenviar, id: guid(), evento: "onclick", metodo: reenviarPracaPedagioTarifaIntegracaoClick, visibilidade: reenviarPracaPedagioTarifaIntegracaoVisible };
    var historico = { descricao: Localization.Resources.Gerais.Geral.HistoricoDeIntegracao, id: guid(), evento: "onclick", metodo: exibirHistoricoPracaPedagioTarifaIntegracaoClick };
    var menuOpcoes = { tipo: TypeOptionMenu.list, tamanho: 5, opcoes: [historico, reenviar] };
    _gridPracaPedagioTarifaIntegracao = new GridView(_pesquisaPracaPedagioTarifaIntegracao.Pesquisar.idGrid, "PracaPedagio/PesquisaIntegracao", _pesquisaPracaPedagioTarifaIntegracao, menuOpcoes, null, 5);
    _gridPracaPedagioTarifaIntegracao.CarregarGrid();
}

function loadGridPedagioTarifaIntegracaoHistorico() {
    var opcaoDownload = { descricao: Localization.Resources.Gerais.Geral.DowloadArquivos, id: guid(), evento: "onclick", metodo: downloadArquivosPracaPedagioTarifaIntegracaoHistoricoClick, tamanho: "20", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoDownload] };
    _gridPracaPedagioTarifaIntegracaoHistorico = new GridView("grid-praca-pedagio-tarifa-integracao-historico", "PracaPedagio/PesquisaIntegracaoHistorico", _pesquisaPracaPedagioTarifaIntegracaoHistorico, menuOpcoes, { column: 1, dir: orderDir.desc });
}

function loadPedagioTarifaIntegracao() {
    _pesquisaPracaPedagioTarifaIntegracao = new PesquisaPracaPedagioTarifaIntegracao();
    KoBindings(_pesquisaPracaPedagioTarifaIntegracao, "knockoutPesquisaPracaPedagioTarifaIntegracao", _pesquisaPracaPedagioTarifaIntegracao.Pesquisar.id);
    _pesquisaPracaPedagioTarifaIntegracaoHistorico = new PesquisaPracaPedagioTarifaIntegracaoHistorico();
    loadGridPracaPedagioTarifaIntegracao();
    loadGridPedagioTarifaIntegracaoHistorico();
}

function exibirHistoricoPracaPedagioTarifaIntegracaoClick(registroSelecionado) {
    _pesquisaPracaPedagioTarifaIntegracaoHistorico.Codigo.val(registroSelecionado.Codigo);
    _gridPracaPedagioTarifaIntegracaoHistorico.CarregarGrid();
    Global.abrirModal('divModalPracaPedagioTarifaIntegracaoHistorico');
}

function downloadArquivosPracaPedagioTarifaIntegracaoHistoricoClick(row) {
    executarDownload("PracaPedagio/DownloadArquivosIntegracaoHistorico", { Codigo: row.Codigo });
}

function pesquisarPracaPedagioTarifaIntegracaoClick() {
    _gridPracaPedagioTarifaIntegracao.CarregarGrid();
}

function exibirFiltrosPracaPedagioTarifaIntegracaoClick() {
    _pesquisaPracaPedagioTarifaIntegracao.ExibirFiltros.visibleFade(!_pesquisaPracaPedagioTarifaIntegracao.ExibirFiltros.visibleFade());
}

function executarPracaPedagioTarifaIntegracaoClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Logistica.PracaPedagio.DesejaRealmenteIniciarIntegracao, function () {
        executarReST("PracaPedagio/Integrar", null, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Logistica.PracaPedagio.IntegracaoSolicitadaComSucesso );
                    _gridPracaPedagioTarifaIntegracao.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            }
        });
    });
}

function reenviarPracaPedagioTarifaIntegracaoVisible(row) {
    return (row.CodigoSituacaoIntegracao == EnumSituacaoIntegracao.ProblemaIntegracao);
}

function reenviarPracaPedagioTarifaIntegracaoClick(row) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Logistica.PracaPedagio.DesejaRealmenteReenviarIntegracao, function () {
        executarReST("PracaPedagio/Reenviar", { Codigo: row.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Logistica.PracaPedagio.ReenvioSolicitadoComSucesso);
                    _gridPracaPedagioTarifaIntegracao.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            }
        });
    });
}