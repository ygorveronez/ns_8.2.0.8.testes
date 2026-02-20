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
/// <reference path="../../../../../ViewsScripts/Consultas/Localidade.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../js/app.config.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridContratoFreteTransportador, _pesquisaContratoFreteTransportador, _CRUDRelatorio, _relatorioContratoFreteTransportador, _CRUDFiltrosRelatorio;

var PesquisaContratoFreteTransportador = function () {
    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });

    this.NumeroEmbarcador = PropertyEntity({ text: "Número: ", getType: typesKnockout.string });

    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable("Transportador:") , idBtnSearch: guid(), visible: ko.observable(true) });
    this.EmVigencia = PropertyEntity({ type: types.map, val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Somente contratos em Vigência" });
    this.Situacao = PropertyEntity({ val: ko.observable(0), options: _statusPesquisa, def: 0, text: "Situação: " });
    this.TipoContratoFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Contrato de Frete:", idBtnSearch: guid(), required: false, enable: ko.observable(true) });
    this.DataInicio = PropertyEntity({ text: "Vigência Inicial: ", getType: typesKnockout.date });
    this.DataFim = PropertyEntity({ text: "Vigência Final: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;
    this.DiasParaVencimento = PropertyEntity({ text: "Dias para vencimento do contrato:", val: ko.observable(""), def: "", getType: typesKnockout.string, getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: '' }, enable: ko.observable(true), maxlength: 3 });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
        this.Transportador.text("Empresa/Filial:");
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridContratoFreteTransportador.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaContratoFreteTransportador.Visible.visibleFade() == true) {
                _pesquisaContratoFreteTransportador.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaContratoFreteTransportador.Visible.visibleFade(true);
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

function loadRelatorioContratoFreteTransportador() {

    _pesquisaContratoFreteTransportador = new PesquisaContratoFreteTransportador();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridContratoFreteTransportador = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ContratoFreteTransportador/Pesquisa", _pesquisaContratoFreteTransportador);
    _gridContratoFreteTransportador.SetPermitirEdicaoColunas(true);
    _gridContratoFreteTransportador.SetQuantidadeLinhasPorPagina(10);

    _relatorioContratoFreteTransportador = new RelatorioGlobal("Relatorios/ContratoFreteTransportador/BuscarDadosRelatorio", _gridContratoFreteTransportador, function () {
        _relatorioContratoFreteTransportador.loadRelatorio(function () {
            KoBindings(_pesquisaContratoFreteTransportador, "knockoutPesquisaContratoFreteTransportador", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaContratoFreteTransportador", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaContratoFreteTransportador", false);
            new BuscarTipoContratoFrete(_pesquisaContratoFreteTransportador.TipoContratoFrete);
            new BuscarTransportadores(_pesquisaContratoFreteTransportador.Transportador);

            $("#divConteudoRelatorio").show();
        })
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaContratoFreteTransportador);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioContratoFreteTransportador.gerarRelatorio("Relatorios/ContratoFreteTransportador/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioContratoFreteTransportador.gerarRelatorio("Relatorios/ContratoFreteTransportador/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}