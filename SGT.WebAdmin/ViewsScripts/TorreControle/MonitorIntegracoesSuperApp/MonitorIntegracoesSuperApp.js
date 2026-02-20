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
var _pesquisaMonitorIntegracoesSuperApp;
var _monitorIntegracoesSuperApp;
var _gridMonitorIntegracoesSuperApp;
// #endregion Objetos Globais do Arquivo

// #region Classes
var PesquisaMonitorIntegracoesSuperApp = function () {
    this.Pesquisar = PropertyEntity({ eventClick: loadGridIntegracoesSuperApp, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataInicioRecebimento = PropertyEntity({ text: "Data Início Recebimento: ", getType: typesKnockout.dateTime, def: "", val: ko.observable(Global.DataAtual()) });
    this.DataFimRecebimento = PropertyEntity({ text: "Data Fim Recebimento: ", getType: typesKnockout.dateTime });
    this.SituacaoProcessamento = PropertyEntity({ text: "Situação Processamento:", val: ko.observable(EnumSituacaoProcessamentoIntegracaoSuperApp.Todos), options: EnumSituacaoProcessamentoIntegracaoSuperApp.obterOpcoes(), def: EnumSituacaoProcessamentoIntegracaoSuperApp.Todos });
    this.TipoEventoApp = PropertyEntity({ text: "Tipo de Evento no App:", val: ko.observable(EnumTipoEventoApp.Todas), options: EnumTipoEventoApp.obterOpcoes(), def: EnumTipoEventoApp.Todas });

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

var MonitorIntegracoesSuperApp = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Grid = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(true) });

}
// #endregion Classes

// #region Funções de Inicialização
function loadMonitorIntegracoesSuperApp() {
    _pesquisaMonitorIntegracoesSuperApp = new PesquisaMonitorIntegracoesSuperApp();
    KoBindings(_pesquisaMonitorIntegracoesSuperApp, "knockoutPesquisaMonitorIntegracoesSuperApp");

    _monitorIntegracoesSuperApp = new MonitorIntegracoesSuperApp();
    KoBindings(_monitorIntegracoesSuperApp, "knockoutMonitorIntegracoesSuperApp");

    BuscarTransportadores(_pesquisaMonitorIntegracoesSuperApp.Transportador);
    BuscarCargas(_pesquisaMonitorIntegracoesSuperApp.Carga);
}

function loadGridIntegracoesSuperApp() {
    var reenviar = { descricao: "Reenviar", id: guid(), evento: "onclick", metodo: reenviarIntegracaoSuperAppClick };
    var downloadArquivos = { descricao: "Download Arquivos", id: guid(), evento: "onclick", metodo: downloadArquivosIntegracaoSuperAppHistoricoClick };

    var menuOpcoes = { tipo: TypeOptionMenu.list, tamanho: 5, opcoes: [reenviar, downloadArquivos] };

    var configuracoesExportacao = { url: "MonitorIntegracoesSuperApp/ExportarPesquisa", titulo: "Integrações SuperApp" };
    _gridMonitorIntegracoesSuperApp = new GridView(_monitorIntegracoesSuperApp.Grid.idGrid, "MonitorIntegracoesSuperApp/Pesquisa", _pesquisaMonitorIntegracoesSuperApp, menuOpcoes, null, 10, null, true, false, undefined, 100, undefined, configuracoesExportacao);
    _gridMonitorIntegracoesSuperApp.CarregarGrid();

}
// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos
function downloadArquivosIntegracaoSuperAppHistoricoClick(row) {
    executarDownload("MonitorIntegracoesSuperApp/DownloadArquivosIntegracaoHistorico", { Codigo: row.Codigo });
}
function reenviarIntegracaoSuperAppClick(row) {
    exibirConfirmacao("Atenção!", "Deseja realmente reenviar a integração?", function () {
        executarReST("MonitorIntegracoesSuperApp/Reenviar", { Codigo: row.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio executado com sucesso.");
                    _gridMonitorIntegracoesSuperApp.CarregarGrid();
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