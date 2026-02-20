/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CargaEntrega.js" />
/// <reference path="../../Enumeradores/EnumMonitoramentoStatus.js" />
/// <reference path="../../Enumeradores/EnumAlertaMonitorStatus.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridParadas, _pesquisaParadas, _CRUDRelatorio, _relatorioParadas, _CRUDFiltrosRelatorio;

var PesquisaParadas = function () {
    let dataDoDia = Global.DataAtual();
    let dataAmanha = Global.Data(EnumTipoOperacaoDate.Add, 1, EnumTipoOperacaoObjetoDate.Days);
    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });

    this.DataEntregaPlanejadaInicio = PropertyEntity({ text: "Data Entrega Planejada Inicio: ", getType: typesKnockout.dateTime, val: ko.observable() });
    this.DataEntregaPlanejadaFinal = PropertyEntity({ text: "Data Entrega Planejada Final:", getType: typesKnockout.dateTime, val: ko.observable() });
    this.DataInicial = PropertyEntity({ text: "Data Inicio Criação Carga: ", getType: typesKnockout.dateTime, val: ko.observable(dataDoDia), def: dataDoDia });
    this.DataFinal = PropertyEntity({ text: "Data Fim Criação Carga: ", getType: typesKnockout.dateTime, val: ko.observable(dataAmanha), def: dataAmanha });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;
    this.NumeroCarga = PropertyEntity({ text: "Número Carga: " });

    this.TipoCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Carga: ", idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Operação: ", idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Transportador: ", idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial: ", idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Veículo: ", idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Motorista: ", idBtnSearch: guid() });
    this.Remetente = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Remetente: ", idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Destinatário: ", idBtnSearch: guid() });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Origem: ", idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destino: ", idBtnSearch: guid() });
    this.EscritorioVendas = PropertyEntity({ text: "Escritório de Vendas: " });
    this.ProtocoloIntegracaoSM = PropertyEntity({ text: "SM: " });
    this.NumeroPedidoCliente = PropertyEntity({ text: "Número do pedido no cliente: " });
    this.MonitoramentoStatus = PropertyEntity({ text: "Situação do Monitoramento: ", getType: typesKnockout.selectMultiple, options: EnumMonitoramentoStatus.obterOpcoes(), def: EnumMonitoramentoStatus.Todas, visible: ko.observable(true) });
    this.ExibirCargasAgrupadas = PropertyEntity({text: "Visualizar as cargas agrupadas (apenas para cargas que foram agrupadas)", getType: typesKnockout.bool, val: ko.observable(false)});

    this.TipoParada = PropertyEntity({ text: "Tipo Parada: ", options: Global.ObterOpcoesPesquisaBooleano("Coleta", "Entrega"), val: ko.observable(""), def: "" });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridParadas.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaParadas.Visible.visibleFade()) {
                _pesquisaParadas.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaParadas.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function LoadRelatorioParadas() {
    _pesquisaParadas = new PesquisaParadas();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridParadas = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "/Relatorios/Paradas/Pesquisa", _pesquisaParadas, null, null, 10, null, null, null, null, 20);
    _gridParadas.SetPermitirEdicaoColunas(true);

    _relatorioParadas = new RelatorioGlobal("Relatorios/Paradas/BuscarDadosRelatorio", _gridParadas, function () {
        _relatorioParadas.loadRelatorio(function () {
            KoBindings(_pesquisaParadas, "knockoutPesquisaParadas");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaParadas");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaParadas");

            new BuscarTiposdeCarga(_pesquisaParadas.TipoCarga);
            new BuscarTiposOperacao(_pesquisaParadas.TipoOperacao);
            new BuscarTransportadores(_pesquisaParadas.Transportador, null, null, true);
            new BuscarFilial(_pesquisaParadas.Filial);
            new BuscarVeiculos(_pesquisaParadas.Veiculo);
            new BuscarMotoristas(_pesquisaParadas.Motorista);
            new BuscarClientes(_pesquisaParadas.Remetente);
            new BuscarClientes(_pesquisaParadas.Destinatario);
            new BuscarLocalidades(_pesquisaParadas.Origem);
            new BuscarLocalidades(_pesquisaParadas.Destino);

        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaParadas);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioParadas.gerarRelatorio("Relatorios/Paradas/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioParadas.gerarRelatorio("Relatorios/Paradas/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}