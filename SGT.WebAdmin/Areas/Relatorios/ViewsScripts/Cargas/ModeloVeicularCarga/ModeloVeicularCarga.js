/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumAtivoInativo.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoModeloVeicularCarga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ModeloVeicularCarga.js" />

//********MAPEAMENTO KNOCKOUT********

var _relatorioModeloVeicularCarga, _gridModeloVeicularCarga, _pesquisaModeloVeicularCarga, _CRUDRelatorio, _CRUDFiltrosRelatorio;


var PesquisaModeloVeicularCarga = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.ModeloVeicular = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.ModeloVeicular.getFieldDescription(), idBtnSearch: guid() });
    this.Ativo = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), val: ko.observable(EnumAtivoInativo.Todos), def: ko.observable(EnumAtivoInativo.Todos), options: EnumAtivoInativo.obterOpcoesPesquisa() });
    this.Tipo = PropertyEntity({ text: Localization.Resources.Cargas.ModeloVeicularCarga.TipoDeVeiculo.getFieldDescription(), val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumTipoModeloVeicularCarga.obterOpcoes() });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridModeloVeicularCarga.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: Localization.Resources.Gerais.Geral.GerarPDF });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: Localization.Resources.Gerais.Geral.GerarPlanilhaExcel });
}

//*********EVENTOS**********

function LoadModeloVeicularCarga() {
    _pesquisaModeloVeicularCarga = new PesquisaModeloVeicularCarga();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridModeloVeicularCarga = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ModeloVeicularCarga/Pesquisa", _pesquisaModeloVeicularCarga, null, null, 10);
    _gridModeloVeicularCarga.SetPermitirEdicaoColunas(true);

    _relatorioModeloVeicularCarga = new RelatorioGlobal("Relatorios/ModeloVeicularCarga/BuscarDadosRelatorio", _gridModeloVeicularCarga, function () {
        _relatorioModeloVeicularCarga.loadRelatorio(function () {
            KoBindings(_pesquisaModeloVeicularCarga, "knockoutPesquisaModeloVeicularCarga", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaModeloVeicularCarga", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaModeloVeicularCarga", false);

            new BuscarModelosVeicularesCarga(_pesquisaModeloVeicularCarga.ModeloVeicular);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
                _pesquisaModeloVeicularCarga.Transportador.text("Empresa/Filial:");

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaModeloVeicularCarga);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioModeloVeicularCarga.gerarRelatorio("Relatorios/ModeloVeicularCarga/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioModeloVeicularCarga.gerarRelatorio("Relatorios/ModeloVeicularCarga/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}