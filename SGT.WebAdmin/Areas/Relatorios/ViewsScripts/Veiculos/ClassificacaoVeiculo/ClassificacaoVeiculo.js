/// <reference path="../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../ViewsScripts/Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../../../ViewsScripts/Consultas/ModeloCarroceria.js" />
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
/// <reference path="../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../js/app.config.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridClassificacaoVeiculo, _pesquisaClassificacaoVeiculo, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioClassificacaoVeiculo;

var PesquisaClassificacaoVeiculo = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:",issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:",issue: 69, idBtnSearch: guid(), visible: ko.observable(true) });
    this.ModeloVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo do Veículo:",issue: 44, idBtnSearch: guid(), visible: ko.observable(true) });
    this.ModeloCarroceria = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo de Carroceria:",issue: 658, idBtnSearch: guid(),  visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: "Situação:",issue: 556, options: _statusPesquisa, val: ko.observable(0), def: 0, visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridClassificacaoVeiculo.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function LoadClassificacaoVeiculo() {
    _pesquisaClassificacaoVeiculo = new PesquisaClassificacaoVeiculo();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridClassificacaoVeiculo = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ClassificacaoVeiculo/Pesquisa", _pesquisaClassificacaoVeiculo);

    _gridClassificacaoVeiculo.SetPermitirEdicaoColunas(true);
    _gridClassificacaoVeiculo.SetQuantidadeLinhasPorPagina(10);

    _relatorioClassificacaoVeiculo = new RelatorioGlobal("Relatorios/ClassificacaoVeiculo/BuscarDadosRelatorio", _gridClassificacaoVeiculo, function () {
        _relatorioClassificacaoVeiculo.loadRelatorio(function () {
            KoBindings(_pesquisaClassificacaoVeiculo, "knockoutPesquisaClassificacaoVeiculo", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaClassificacaoVeiculo", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaClassificacaoVeiculo", false);

            BuscarModelosCarroceria(_pesquisaClassificacaoVeiculo.ModeloCarroceria);
            BuscarModelosVeicularesCarga(_pesquisaClassificacaoVeiculo.ModeloVeiculo);
            BuscarTransportadores(_pesquisaClassificacaoVeiculo.Transportador);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaClassificacaoVeiculo);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioClassificacaoVeiculo.gerarRelatorio("Relatorios/ClassificacaoVeiculo/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioClassificacaoVeiculo.gerarRelatorio("Relatorios/ClassificacaoVeiculo/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
