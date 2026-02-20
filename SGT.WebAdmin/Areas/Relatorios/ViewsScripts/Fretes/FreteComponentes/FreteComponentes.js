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
/// <reference path="../../../../../ViewsScripts/Consultas/Localidade.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../js/app.config.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridFreteComponente, _pesquisaFreteComponente, _CRUDRelatorio, _relatorioComponente, _CRUDFiltrosRelatorio;

var PesquisaFreteComponente = function () {
    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });

    this.DataInicio = PropertyEntity({ text: "Data início: ", getType: typesKnockout.date });
    this.DataFim = PropertyEntity({ text: "Data limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", issue: 55, idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", issue: 55, idBtnSearch: guid() });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid() });
    this.Expedidor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Expedidor:", idBtnSearch: guid() });
    this.Recebedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Recebedor:", idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ text: "Transportador:", issue: 69, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", issue: 70, idBtnSearch: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridFreteComponente.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaFreteComponente.Visible.visibleFade()) {
                _pesquisaFreteComponente.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaFreteComponente.Visible.visibleFade(true);
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

function loadRelatorioFreteComponente() {

    _pesquisaFreteComponente = new PesquisaFreteComponente();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridFreteComponente = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/FreteComponentes/Pesquisa", _pesquisaFreteComponente, null, null, 10);
    _gridFreteComponente.SetPermitirEdicaoColunas(true);

    _relatorioComponente = new RelatorioGlobal("Relatorios/FreteComponentes/BuscarDadosRelatorio", _gridFreteComponente, function () {
        _relatorioComponente.loadRelatorio(function () {
            KoBindings(_pesquisaFreteComponente, "knockoutPesquisaFreteComponente", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaFreteComponente", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaFreteComponentes", false);

            new BuscarClientes(_pesquisaFreteComponente.Remetente);
            new BuscarClientes(_pesquisaFreteComponente.Destinatario);
            new BuscarClientes(_pesquisaFreteComponente.Recebedor);
            new BuscarClientes(_pesquisaFreteComponente.Expedidor);
            new BuscarClientes(_pesquisaFreteComponente.Tomador);
            new BuscarFilial(_pesquisaFreteComponente.Filial);
            new BuscarTransportadores(_pesquisaFreteComponente.Transportador);

            $("#divConteudoRelatorio").show();
        })
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaFreteComponente);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioComponente.gerarRelatorio("Relatorios/FreteComponentes/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioComponente.gerarRelatorio("Relatorios/FreteComponentes/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}