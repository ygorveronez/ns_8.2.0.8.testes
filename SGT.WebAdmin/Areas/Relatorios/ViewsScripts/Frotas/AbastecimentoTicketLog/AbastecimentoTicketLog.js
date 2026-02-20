//*******MAPEAMENTO KNOUCKOUT*******

var _gridAbastecimentoTicketLog, _pesquisaAbastecimentoTicketLog, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioAbastecimentoTicketLog;

var PesquisaAbastecimentoTicketLog = function () {
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.DataInicial = PropertyEntity({ text: "Data Inicial Transação:", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final Transação:", getType: typesKnockout.date });
    this.Veiculo = PropertyEntity({ text: "Veículo:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid() });
    this.Fornecedor = PropertyEntity({ text: "Fornecedor:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid() });
    this.CodigoTransacao = PropertyEntity({ text: "Código Transação:", val: ko.observable(), getType: typesKnockout.int });

};


var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridAbastecimentoTicketLog.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaAbastecimentoTicketLog.Visible.visibleFade()) {
                _pesquisaAbastecimentoTicketLog.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaAbastecimentoTicketLog.Visible.visibleFade(true);
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

function loadRelatorioAbastecimentoTicketLog() {
    _pesquisaAbastecimentoTicketLog = new PesquisaAbastecimentoTicketLog();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridAbastecimentoTicketLog = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "/Relatorios/AbastecimentoTicketLog/Pesquisa", _pesquisaAbastecimentoTicketLog, null, null, 10);
    _gridAbastecimentoTicketLog.SetPermitirEdicaoColunas(true);

    _relatorioAbastecimentoTicketLog = new RelatorioGlobal("Relatorios/AbastecimentoTicketLog/BuscarDadosRelatorio", _gridAbastecimentoTicketLog, function () {
        _relatorioAbastecimentoTicketLog.loadRelatorio(function () {
            KoBindings(_pesquisaAbastecimentoTicketLog, "knockoutPesquisaAbastecimentoTicketLog");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaAbastecimentoTicketLog");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaAbastecimentoTicketLog");

            new BuscarTracaoManobra(_pesquisaAbastecimentoTicketLog.Veiculo);
            new BuscarClientes(_pesquisaAbastecimentoTicketLog.Fornecedor);
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaAbastecimentoTicketLog);

    
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioAbastecimentoTicketLog.gerarRelatorio("Relatorios/AbastecimentoTicketLog/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}
function GerarRelatorioExcelClick(e, sender) {
    _relatorioAbastecimentoTicketLog.gerarRelatorio("Relatorios/AbastecimentoTicketLog/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
