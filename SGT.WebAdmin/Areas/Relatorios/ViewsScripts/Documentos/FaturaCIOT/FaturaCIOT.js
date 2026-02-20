
//*******MAPEAMENTO KNOUCKOUT*******

var _gridFaturaCIOT, _pesquisaFaturaCIOT, _CRUDRelatorio, _relatorioFaturaCIOT, _CRUDFiltrosRelatorio;

var PesquisaFaturaCIOT = function () {
    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataVencimentoInicial = PropertyEntity({ text: "Data Vencimento Inicial: ", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.DataVencimentoFinal = PropertyEntity({ text: "Data Vencimento Final: ", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });

    this.DataVencimentoInicial.dateRangeLimit = this.DataVencimentoFinal;
    this.DataVencimentoFinal.dateRangeInit = this.DataVencimentoInicial;
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridFaturaCIOT.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaFaturaCIOT.Visible.visibleFade() == true) {
                _pesquisaFaturaCIOT.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaFaturaCIOT.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.ConsultaFaturas = PropertyEntity({ eventClick: consultarFaturasClick, type: types.event, text: "Consultar Faturas" });
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function LoadFaturaCIOT() {

    _pesquisaFaturaCIOT = new PesquisaFaturaCIOT();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridFaturaCIOT = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "/Relatorios/FaturaCIOT/Pesquisa", _pesquisaFaturaCIOT, null, null, 10, null, null, null, null, 20);
    _gridFaturaCIOT.SetPermitirEdicaoColunas(true);

    _relatorioFaturaCIOT = new RelatorioGlobal("Relatorios/FaturaCIOT/BuscarDadosRelatorio", _gridFaturaCIOT, function () {
        _relatorioFaturaCIOT.loadRelatorio(function () {
            KoBindings(_pesquisaFaturaCIOT, "knockoutPesquisaFaturaCIOT");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaFaturaCIOT");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaFaturaCIOT");

            //new BuscarTransportadores(_pesquisaFaturaCIOT.Transportador, null, null, true);
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaFaturaCIOT);
}
    
function consultarFaturasClick() {
    executarReST("Relatorios/FaturaCIOT/ExecutarConsultaEFrete", null, function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Sucesso");
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioFaturaCIOT.gerarRelatorio("Relatorios/FaturaCIOT/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioFaturaCIOT.gerarRelatorio("Relatorios/FaturaCIOT/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
