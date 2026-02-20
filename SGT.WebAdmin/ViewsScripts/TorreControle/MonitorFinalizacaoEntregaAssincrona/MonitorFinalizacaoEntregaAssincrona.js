/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Enumeradores/EnumTipoEventoApp.js" />
/// <reference path="../../Enumeradores/EnumSituacaoProcessamentoIntegracaoSuperApp.js" />

// #region Objetos Globais do Arquivo
var _pesquisaMonitorFinalizacaoEntregaAssincrona;
var _monitorFinalizacaoEntregaAssincrona;
var _gridMonitorFinalizacaoEntregaAssincrona;
// #endregion Objetos Globais do Arquivo

// #region Classes
var PesquisaMonitorFinalizacaoEntregaAssincrona = function () {
    this.Pesquisar = PropertyEntity({ eventClick: loadGridMonitorFinalizacaoEntregaAssincrona, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataInicialInclusaoProcessamento = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataInicialInclusaoProcessamento.getFieldDescription(), val: ko.observable(Global.DataAtual()), def: "", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataFinalInclusaoProcessamento = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataFinalInclusaoProcessamento.getFieldDescription(), val: ko.observable(Global.DataAtual()), def: "", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataInicialInclusaoProcessamento.dateRangeLimit = this.DataFinalInclusaoProcessamento;
    this.DataFinalInclusaoProcessamento.dateRangeInit = this.DataInicialInclusaoProcessamento;
    this.SituacaoProcessamento = PropertyEntity({ text: "Situação Processamento:", val: ko.observable(EnumSituacaoProcessamentoIntegracaoSuperApp.Todos), options: EnumSituacaoProcessamentoIntegracaoSuperApp.obterOpcoes(), def: EnumSituacaoProcessamentoIntegracaoSuperApp.Todos });
    
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var MonitorFinalizacaoEntregaAssincrona = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Grid = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(true) });

}
// #endregion Classes

// #region Funções de Inicialização
function loadMonitorFinalizacaoEntregaAssincrona() {
    _pesquisaMonitorFinalizacaoEntregaAssincrona = new PesquisaMonitorFinalizacaoEntregaAssincrona();
    KoBindings(_pesquisaMonitorFinalizacaoEntregaAssincrona, "knockoutPesquisaMonitorFinalizacaoEntregaAssincrona");

    _monitorFinalizacaoEntregaAssincrona = new MonitorFinalizacaoEntregaAssincrona();
    KoBindings(_monitorFinalizacaoEntregaAssincrona, "knockoutMonitorFinalizacaoEntregaAssincrona");

    BuscarCargas(_pesquisaMonitorFinalizacaoEntregaAssincrona.Carga);
    BuscarClientes(_pesquisaMonitorFinalizacaoEntregaAssincrona.Cliente);

    loadGridMonitorFinalizacaoEntregaAssincrona();
}
// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos
function loadGridMonitorFinalizacaoEntregaAssincrona() {
    var reenviar = { descricao: "Reenviar", id: guid(), evento: "onclick", metodo: reprocessarFinalizacaoEntregaAssincrona };

    var menuOpcoes = { tipo: TypeOptionMenu.list, tamanho: 5, opcoes: [reenviar] };

    var configuracoesExportacao = { url: "MonitorFinalizacaoEntregaAssincrona/ExportarPesquisa", titulo: "Finalização de Entregas Assíncrona" };
    _gridMonitorFinalizacaoEntregaAssincrona = new GridView(_monitorFinalizacaoEntregaAssincrona.Grid.idGrid, "MonitorFinalizacaoEntregaAssincrona/Pesquisa", _pesquisaMonitorFinalizacaoEntregaAssincrona, menuOpcoes, null, 10, null, true, false, undefined, 50, undefined, configuracoesExportacao);
    _gridMonitorFinalizacaoEntregaAssincrona.CarregarGrid();

}
function reprocessarFinalizacaoEntregaAssincrona(row) {
    exibirConfirmacao("Atenção!", "Realmente deseja reprocessar as entregas?", function () {
        executarReST("MonitorFinalizacaoEntregaAssincrona/Reprocessar", { Codigo: row.DT_RowId }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio executado com sucesso.");
                    _gridMonitorFinalizacaoEntregaAssincrona.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso!", retorno.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
            }
        });
    });
}
// #endregion Funções Associadas a Eventos