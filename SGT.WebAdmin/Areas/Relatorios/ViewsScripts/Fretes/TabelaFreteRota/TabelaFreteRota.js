
/// <reference path="../../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/knockout-3.1.0.js" />
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

var _gridTabelaFreteRota;
var _pesquisaTabelaFreteRota;
var _CRUDRelatorio;
var _CRUDFiltrosRelatorio;
var _relatorioTabelaFreteRota;

var PesquisaTabelaFreteRota = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.Ativo = PropertyEntity({ val: ko.observable(1), options: _statusPesquisa, def: 1, text: "Situação: " });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Origem:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destino:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Carga:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridTabelaFreteRota.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function LoadTabelaFreteRota() {
    _pesquisaTabelaFreteRota = new PesquisaTabelaFreteRota();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridTabelaFreteRota = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/TabelaFreteRota/Pesquisa", _pesquisaTabelaFreteRota);

    _gridTabelaFreteRota.SetPermitirEdicaoColunas(true);
    _gridTabelaFreteRota.SetQuantidadeLinhasPorPagina(10);

    _relatorioTabelaFreteRota = new RelatorioGlobal("Relatorios/TabelaFreteRota/BuscarDadosRelatorio", _gridTabelaFreteRota, function () {
        _relatorioTabelaFreteRota.loadRelatorio(function () {
            KoBindings(_pesquisaTabelaFreteRota, "knockoutPesquisaTabelaFreteRota", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaTabelaFreteRota", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaTabelaFreteRota", false);

            //BUSCA DOS CAMPOS DE PESQUISA E OUTROS BINDINGS
            new BuscarLocalidades(_pesquisaTabelaFreteRota.Origem);
            new BuscarLocalidades(_pesquisaTabelaFreteRota.Destino);
            new BuscarTiposdeCarga(_pesquisaTabelaFreteRota.TipoCarga);
            new BuscarModelosVeicularesCarga(_pesquisaTabelaFreteRota.ModeloVeicularCarga);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaTabelaFreteRota);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioTabelaFreteRota.gerarRelatorio("Relatorios/TabelaFreteRota/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioTabelaFreteRota.gerarRelatorio("Relatorios/TabelaFreteRota/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}

//*******MÉTODOS*******