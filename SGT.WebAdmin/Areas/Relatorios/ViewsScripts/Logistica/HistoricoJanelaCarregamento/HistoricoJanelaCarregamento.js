//#region Referências
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
/// <reference path="../../Relatorios/Global/Relatorio.js" />

//#endregion

var _pesquisaHistoricoJanelaCarregamento;
var _relatorioHistoricoJanelaCarregamento;
var _gridHistoricoJanelaCarregamento;
var _CRUDFiltrosRelatorio;
var _CRUDRelatorio;


var PesquisaHistoricoJanelaCarregamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.MotivoRecusa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motivo de Recusa:", idBtnSearch: guid(), val: ko.observable("") });
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Carregamento:", idBtnSearch: guid(), val: ko.observable("") });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), val: ko.observable("") });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridHistoricoJanelaCarregamento.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorioHistoricoJanelaCarregamento", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaHistoricoJanelaCarregamento.Visible.visibleFade()) {
                _pesquisaHistoricoJanelaCarregamento.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaHistoricoJanelaCarregamento.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

function LoadHistoricoJanelaCarregamento() {
    _pesquisaHistoricoJanelaCarregamento = new PesquisaHistoricoJanelaCarregamento();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();


    _gridHistoricoJanelaCarregamento = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/HistoricoJanelaCarregamento/Pesquisa", _pesquisaHistoricoJanelaCarregamento, null, null, 10);
    _gridHistoricoJanelaCarregamento.SetPermitirEdicaoColunas(true);

    _relatorioHistoricoJanelaCarregamento = new RelatorioGlobal("Relatorios/HistoricoJanelaCarregamento/BuscarDadosRelatorio", _gridHistoricoJanelaCarregamento, function () {
        _relatorioHistoricoJanelaCarregamento.loadRelatorio(function () {
            KoBindings(_pesquisaHistoricoJanelaCarregamento, "knockoutPesquisaHistoricoJanelaCarregamento");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaHistoricoJanelaCarregamento");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaHistoricoJanelaCarregamento");

            new BuscarMotivoRetiradaFilaCarregamento(_pesquisaHistoricoJanelaCarregamento.MotivoRecusa);
            new BuscarCargas(_pesquisaHistoricoJanelaCarregamento.Carga);
            new BuscarCentrosCarregamento(_pesquisaHistoricoJanelaCarregamento.CentroCarregamento);
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaHistoricoJanelaCarregamento);

}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioHistoricoJanelaCarregamento.gerarRelatorio("Relatorios/HistoricoJanelaCarregamento/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioHistoricoJanelaCarregamento.gerarRelatorio("Relatorios/HistoricoJanelaCarregamento/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
