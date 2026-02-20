/// <reference path="../../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/Rest.js" />
/// <reference path="../../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../../js/Global/Grid.js" />
/// <reference path="../../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Porto.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/PedidoViagemNavio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioAFRMMControlMercante, _gridAFRMMControlMercante, _pesquisaAFRMMControlMercante, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaAFRMMControlMercante = function () {
    this.DataEmissaoInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.DataEmissaoFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });

    this.DataEmissaoInicial.dateRangeLimit = this.DataEmissaoFinal;
    this.DataEmissaoFinal.dateRangeInit = this.DataEmissaoInicial;

    this.PortoOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Porto Origem:", idBtnSearch: guid() });
    this.PortoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Porto Destino:", idBtnSearch: guid() });
    this.Viagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Viagem:", idBtnSearch: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridAFRMMControlMercante.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
    this.GerarArquivoMercante = PropertyEntity({ eventClick: GerarArquivoMercanteClick, type: types.event, text: "Gerar Arquivo Mercante", idGrid: guid() });
    
};

//*******EVENTOS*******


function loadRelatorioAFRMMControlMercante() {

    _pesquisaAFRMMControlMercante = new PesquisaAFRMMControlMercante();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridAFRMMControlMercante = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/AFRMMControlMercante/Pesquisa", _pesquisaAFRMMControlMercante, null, null, 10);
    _gridAFRMMControlMercante.SetPermitirEdicaoColunas(true);

    _relatorioAFRMMControlMercante = new RelatorioGlobal("Relatorios/AFRMMControlMercante/BuscarDadosRelatorio", _gridAFRMMControlMercante, function () {
        _relatorioAFRMMControlMercante.loadRelatorio(function () {
            KoBindings(_pesquisaAFRMMControlMercante, "knockoutPesquisaAFRMMControlMercante");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaAFRMMControlMercante");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaAFRMMControlMercante");

            new BuscarPedidoViagemNavio(_pesquisaAFRMMControlMercante.Viagem);
            new BuscarPorto(_pesquisaAFRMMControlMercante.PortoOrigem);
            new BuscarPorto(_pesquisaAFRMMControlMercante.PortoDestino);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaAFRMMControlMercante);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioAFRMMControlMercante.gerarRelatorio("Relatorios/AFRMMControlMercante/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioAFRMMControlMercante.gerarRelatorio("Relatorios/AFRMMControlMercante/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}

function GerarArquivoMercanteClick(e, sender) {
    _relatorioAFRMMControlMercante.gerarRelatorio("Relatorios/AFRMMControlMercante/GerarRelatorio", EnumTipoArquivoRelatorio.DOC);
}