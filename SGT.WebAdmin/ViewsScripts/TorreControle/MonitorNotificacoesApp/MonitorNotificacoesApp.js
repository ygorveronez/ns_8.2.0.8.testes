var _pesquisaMonitorNotificacoes;
var _monitorNotificacoes;
var _gridMonitorNotificacoes;

var PesquisaMonitorNotificacoesApp = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridMonitorNotificacoesApp, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", issue: 69, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", issue: 69, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", issue: 69, idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataInicioEnvio = PropertyEntity({ text: "Data Início Envio: ", getType: typesKnockout.date, def: "", val: ko.observable(Global.DataAtual()) });
    this.DataFimEnvio = PropertyEntity({ text: "Data Fim Envio: ", getType: typesKnockout.date });
    this.SituacaoIntegracao = PropertyEntity({ text: "Situação Integração:", val: ko.observable(EnumSituacaoIntegracao.Todas), options: EnumSituacaoIntegracao.obterOpcoesPesquisa(), def: EnumSituacaoIntegracao.Todas });
    this.TipoNotificacaoApp = PropertyEntity({ text: "Tipo de notificação no App:", val: ko.observable(EnumTipoNotificacaoApp.Todas), options: EnumTipoNotificacaoApp.obterOpcoes(), def: EnumTipoNotificacaoApp.Todas });
    this.Chamado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Chamado:", issue: 69, idBtnSearch: guid(), visible: ko.observable(true) });

    this.DataInicioEnvio.dateRangeLimit = this.DataFimEnvio;
    this.DataFimEnvio.dateRangeInit = this.DataInicioEnvio;

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

var MonitorNotificacoesApp = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Grid = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(true) });

}

function loadGridNotificacoesApp() {
    var reenviar = { descricao: "Reenviar", id: guid(), evento: "onclick", metodo: reenviarNotificacaoSuperAppClick };
    var downloadArquivos = { descricao: "Download Arquivos", id: guid(), evento: "onclick", metodo: downloadArquivosIntegracaoNotificacaoSuperAppHistoricoClick };
    var menuOpcoes = { tipo: TypeOptionMenu.list, tamanho: 5, opcoes: [reenviar, downloadArquivos] };

    _gridMonitorNotificacoes = new GridView(_monitorNotificacoes.Grid.idGrid, "MonitorNotificacoesApp/Pesquisa", _pesquisaMonitorNotificacoes, menuOpcoes, null, 10, null, true, false);
    _gridMonitorNotificacoes.SetPermitirEdicaoColunas(true);
    _gridMonitorNotificacoes.SetSalvarPreferenciasGrid(true);
    _gridMonitorNotificacoes.SetHabilitarScrollHorizontal(true, 100);
    _gridMonitorNotificacoes.CarregarGrid();

}

function loadMonitorNotificacoesApp() {
    _pesquisaMonitorNotificacoes = new PesquisaMonitorNotificacoesApp();
    KoBindings(_pesquisaMonitorNotificacoes, "knockoutPesquisaMonitorNotificacoesApp", false, _pesquisaMonitorNotificacoes.Pesquisar.id);

    _monitorNotificacoes = new MonitorNotificacoesApp();
    KoBindings(_monitorNotificacoes, "knockoutMonitorNotificacoesApp");

    new BuscarTransportadores(_pesquisaMonitorNotificacoes.Transportador);
    new BuscarMotoristas(_pesquisaMonitorNotificacoes.Motorista);
    BuscarCargas(_pesquisaMonitorNotificacoes.Carga);
    BuscarTodosChamadosParaOcorrencia(_pesquisaMonitorNotificacoes.Chamado);

    loadGridNotificacoesApp();
}

function recarregarGridMonitorNotificacoesApp() {
    _gridMonitorNotificacoes.CarregarGrid();
}

function downloadArquivosIntegracaoNotificacaoSuperAppHistoricoClick(row) {
    executarDownload("MonitorNotificacoesApp/DownloadArquivosIntegracaoHistorico", { Codigo: row.Codigo });
}

function reenviarNotificacaoSuperAppClick(row) {
    exibirConfirmacao("Atenção!", "Deseja realmente reenviar a integração?", function () {
        executarReST("MonitorNotificacoesApp/Reenviar", { Codigo: row.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio executado com sucesso.");
                    _gridMonitorNotificacoes.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso!", retorno.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
            }
        });
    });
}