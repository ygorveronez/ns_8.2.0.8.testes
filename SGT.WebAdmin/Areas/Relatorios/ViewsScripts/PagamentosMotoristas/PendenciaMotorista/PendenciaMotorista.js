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
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />

//#endregion

var _pesquisaPendenciaMotorista;
var _relatorioPendenciaMotorista;
var _gridPendenciaMotorista;
var _CRUDFiltrosRelatorio;
var _CRUDRelatorio;

var _situacaoPendenciaMotorista = [{ text: "Todos", value: EnumSituacaoPendenciaMotorista.Todos }, { text: "Ativo", value: EnumSituacaoPendenciaMotorista.Ativo }, { text: "Estornado", value: EnumSituacaoPendenciaMotorista.Estornado }];

var PesquisaPendenciaMotorista = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), val: ko.observable("") });
    this.ValorInicial = PropertyEntity({ text: "Valor Inicial:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, val: ko.observable(0.00), maxlength: 10 });
    this.ValorFinal = PropertyEntity({ text: "Valor Final:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, val: ko.observable(0.00), maxlength: 10 });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoPendenciaMotorista.Todas), options: _situacaoPendenciaMotorista, def: EnumSituacaoPendenciaMotorista.Todas, text: "Situação: " });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });


    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridPendenciaMotorista.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorioPendenciaMotorista", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaPendenciaMotorista.Visible.visibleFade()) {
                _pesquisaPendenciaMotorista.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaPendenciaMotorista.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

function LoadPendenciaMotorista() {
    _pesquisaPendenciaMotorista = new PesquisaPendenciaMotorista();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    console.log(_CRUDFiltrosRelatorio.Preview.idGrid);
    _gridPendenciaMotorista = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/PendenciaMotorista/Pesquisa", _pesquisaPendenciaMotorista, null, null, 10);
    _gridPendenciaMotorista.SetPermitirEdicaoColunas(true);

    _relatorioPendenciaMotorista = new RelatorioGlobal("Relatorios/PendenciaMotorista/BuscarDadosRelatorio", _gridPendenciaMotorista, function () {
        _relatorioPendenciaMotorista.loadRelatorio(function () {
            KoBindings(_pesquisaPendenciaMotorista, "knockoutPesquisaPendenciaMotorista");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaPendenciaMotorista");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaPendenciaMotorista");

            new BuscarMotorista(_pesquisaPendenciaMotorista.Motorista);
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaPendenciaMotorista);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioPendenciaMotorista.gerarRelatorio("Relatorios/PendenciaMotorista/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioPendenciaMotorista.gerarRelatorio("Relatorios/PendenciaMotorista/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
