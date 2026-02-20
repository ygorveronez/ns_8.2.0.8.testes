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
/// <reference path="../../../../../js/app.config.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTaxaIncidenciaFrete, _pesquisaTaxaIncidenciaFrete, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioTaxaIncidenciaFrete;

var PesquisaTaxaIncidenciaFrete = function () {
    var dataAtual = moment().format("DD/MM/YYYY");

    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Data Final:", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Carregamento:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Rota = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Rota:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.ModeloVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo do Veículo:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Carga:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridTaxaIncidenciaFrete.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaTaxaIncidenciaFrete.Visible.visibleFade()) {
                _pesquisaTaxaIncidenciaFrete.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaTaxaIncidenciaFrete.Visible.visibleFade(true);
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

function LoadTaxaIncidenciaFrete() {
    _pesquisaTaxaIncidenciaFrete = new PesquisaTaxaIncidenciaFrete();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridTaxaIncidenciaFrete = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/TaxaIncidenciaFrete/Pesquisa", _pesquisaTaxaIncidenciaFrete);

    _gridTaxaIncidenciaFrete.SetPermitirEdicaoColunas(true);
    _gridTaxaIncidenciaFrete.SetQuantidadeLinhasPorPagina(10);

    _relatorioTaxaIncidenciaFrete = new RelatorioGlobal("Relatorios/TaxaIncidenciaFrete/BuscarDadosRelatorio", _gridTaxaIncidenciaFrete, function () {
        _relatorioTaxaIncidenciaFrete.loadRelatorio(function () {
            KoBindings(_pesquisaTaxaIncidenciaFrete, "knockoutPesquisaTaxaIncidenciaFrete", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaTaxaIncidenciaFrete", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaTaxaIncidenciaFrete", false);

            new BuscarClientes(_pesquisaTaxaIncidenciaFrete.Destinatario);
            new BuscarTransportadores(_pesquisaTaxaIncidenciaFrete.Transportador);
            new BuscarCentrosCarregamento(_pesquisaTaxaIncidenciaFrete.CentroCarregamento);
            new BuscarTiposdeCarga(_pesquisaTaxaIncidenciaFrete.TipoCarga);
            new BuscarModelosVeicularesCarga(_pesquisaTaxaIncidenciaFrete.ModeloVeiculo);
            new BuscarRotasFrete(_pesquisaTaxaIncidenciaFrete.Rota);
            new BuscarFilial(_pesquisaTaxaIncidenciaFrete.Filial);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaTaxaIncidenciaFrete);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioTaxaIncidenciaFrete.gerarRelatorio("Relatorios/TaxaIncidenciaFrete/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioTaxaIncidenciaFrete.gerarRelatorio("Relatorios/TaxaIncidenciaFrete/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
