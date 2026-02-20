/// <reference path="C:\Users\Willian\Documents\Visual Studio 2015\Projects\MultiCTe\SGT.WebAdmin\ViewsScripts/Consultas/Filial.js" />
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

var _gridTaxaOcupacaoVeiculo, _pesquisaTaxaOcupacaoVeiculo, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioTaxaOcupacaoVeiculo;

var PesquisaTaxaOcupacaoVeiculo = function () {
    var dataAtual = moment().format("DD/MM/YYYY");

    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.TipoRelatorio.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });

    this.DataCriacaoInicial = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataDeCriacaoInicial.getFieldDescription(), val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataCriacaoFinal = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataDeCriacaoFinal.getFieldDescription(), val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataCriacaoInicial.dateRangeLimit = this.DataCriacaoFinal;
    this.DataCriacaoFinal.dateRangeInit = this.DataCriacaoInicial;

    this.DataJanelaCarregamentoInicial = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataDeJanelaDeCarregamentoInicial.getFieldDescription(), val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataJanelaCarregamentoFinal = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataDeJanelaDeCarregamentoFinal.getFieldDescription(), val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataJanelaCarregamentoInicial.dateRangeLimit = this.DataJanelaCarregamentoFinal;
    this.DataJanelaCarregamentoFinal.dateRangeInit = this.DataJanelaCarregamentoInicial;

    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Filial.getFieldDescription(), idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.CentroCarregamento.getFieldDescription(), idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Destinatario.getFieldDescription(), idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Transportador.getFieldDescription(), idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Rota = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Rota.getFieldDescription(), idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.ModeloVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.ModeloVeiculo.getFieldDescription(), idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.TipoCarga.getFieldDescription(), idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.TipoOperacao.getFieldDescription(), idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridTaxaOcupacaoVeiculo.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaTaxaOcupacaoVeiculo.Visible.visibleFade() == true) {
                _pesquisaTaxaOcupacaoVeiculo.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaTaxaOcupacaoVeiculo.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Avancada, icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: Localization.Resources.Gerais.Geral.GerarPDF });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: Localization.Resources.Gerais.Geral.GerarPlanilhaExcel, idGrid: guid() });
}

//*******EVENTOS*******

function LoadTaxaOcupacaoVeiculo() {
    _pesquisaTaxaOcupacaoVeiculo = new PesquisaTaxaOcupacaoVeiculo();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridTaxaOcupacaoVeiculo = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/TaxaOcupacaoVeiculo/Pesquisa", _pesquisaTaxaOcupacaoVeiculo);

    _gridTaxaOcupacaoVeiculo.SetPermitirEdicaoColunas(true);
    _gridTaxaOcupacaoVeiculo.SetQuantidadeLinhasPorPagina(10);

    _relatorioTaxaOcupacaoVeiculo = new RelatorioGlobal("Relatorios/TaxaOcupacaoVeiculo/BuscarDadosRelatorio", _gridTaxaOcupacaoVeiculo, function () {
        _relatorioTaxaOcupacaoVeiculo.loadRelatorio(function () {
            KoBindings(_pesquisaTaxaOcupacaoVeiculo, "knockoutPesquisaTaxaOcupacaoVeiculo", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaTaxaOcupacaoVeiculo", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaTaxaOcupacaoVeiculo", false);

            new BuscarClientes(_pesquisaTaxaOcupacaoVeiculo.Destinatario);
            new BuscarTransportadores(_pesquisaTaxaOcupacaoVeiculo.Transportador);
            new BuscarCentrosCarregamento(_pesquisaTaxaOcupacaoVeiculo.CentroCarregamento);
            new BuscarTiposdeCarga(_pesquisaTaxaOcupacaoVeiculo.TipoCarga);
            new BuscarTiposOperacao(_pesquisaTaxaOcupacaoVeiculo.TipoOperacao);
            new BuscarModelosVeicularesCarga(_pesquisaTaxaOcupacaoVeiculo.ModeloVeiculo);
            new BuscarRotasFrete(_pesquisaTaxaOcupacaoVeiculo.Rota);
            new BuscarFilial(_pesquisaTaxaOcupacaoVeiculo.Filial);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaTaxaOcupacaoVeiculo);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioTaxaOcupacaoVeiculo.gerarRelatorio("Relatorios/TaxaOcupacaoVeiculo/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioTaxaOcupacaoVeiculo.gerarRelatorio("Relatorios/TaxaOcupacaoVeiculo/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
