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

var _gridTransportadoresSemContrato, _pesquisaTransportadoresSemContrato, _CRUDRelatorio, _relatorioTransportadoresSemContrato, _CRUDFiltrosRelatorio;

var PesquisaTransportadoresSemContrato = function () {
    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });

    this.DataInicio = PropertyEntity({ text: "Vigência Inicial: ", getType: typesKnockout.date });
    this.DataFim = PropertyEntity({ text: "Vigência Final: ", getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.Situacao = PropertyEntity({ val: ko.observable(1), options: _statusPesquisa, def: 1, text: "Situação do Transportador: " });
    this.SomenteContrato = PropertyEntity({ type: types.bool, val: ko.observable(false), def: false, text: "Somente transportadores que estão com contrato de frete vencido?", visible: ko.observable(true) });
    this.SomenteSemContrato = PropertyEntity({ type: types.bool, val: ko.observable(false), def: false, text: "Somente transportadores que não possuem contrato de frete?", visible: ko.observable(true) });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridTransportadoresSemContrato.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaTransportadoresSemContrato.Visible.visibleFade() == true) {
                _pesquisaTransportadoresSemContrato.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaTransportadoresSemContrato.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function loadRelatorioTransportadoresSemContrato() {

    _pesquisaTransportadoresSemContrato = new PesquisaTransportadoresSemContrato();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridTransportadoresSemContrato = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/TransportadoresSemContrato/Pesquisa", _pesquisaTransportadoresSemContrato);
    _gridTransportadoresSemContrato.SetPermitirEdicaoColunas(true);
    _gridTransportadoresSemContrato.SetQuantidadeLinhasPorPagina(10);

    _relatorioTransportadoresSemContrato = new RelatorioGlobal("Relatorios/TransportadoresSemContrato/BuscarDadosRelatorio", _gridTransportadoresSemContrato, function () {
        _relatorioTransportadoresSemContrato.loadRelatorio(function () {
            KoBindings(_pesquisaTransportadoresSemContrato, "knockoutPesquisaTransportadoresSemContrato", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaTransportadoresSemContrato", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaTransportadoresSemContrato", false);

            $("#divConteudoRelatorio").show();
        })
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaTransportadoresSemContrato);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioTransportadoresSemContrato.gerarRelatorio("Relatorios/TransportadoresSemContrato/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioTransportadoresSemContrato.gerarRelatorio("Relatorios/TransportadoresSemContrato/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}