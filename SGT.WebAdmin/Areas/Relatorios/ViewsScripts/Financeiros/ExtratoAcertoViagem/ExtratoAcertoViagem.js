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
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/SegmentoVeiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Justificativa.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CentroResultado.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioExtratoAcertoViagem, _gridExtratoAcertoViagem, _pesquisaExtratoAcertoViagem, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _situacaoAcerto = [
    { text: "Em Andamento", value: 1 },
    { text: "Fechado", value: 2 },
    { text: "Cancelado", value: 3 }
];

var PesquisaExtratoAcertoViagem = function () {

    this.DataInicial = PropertyEntity({ text: "Data inicial: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Motorista = PropertyEntity({ text: "Motorista:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.CentroResultado = PropertyEntity({ text: "Centro de Resultado:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ text: "Veículo:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.SegmentoVeiculo = PropertyEntity({ text: "Segmento Veículo:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.SituacaoAcerto = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: _situacaoAcerto, text: "Situação Acerto:", visible: ko.observable(true) });

    this.TipoLancamento = PropertyEntity({ text: "Tipo Lançamento:", type: types.text, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Justificativa = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Justificativa:", idBtnSearch: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridExtratoAcertoViagem.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaExtratoAcertoViagem.Visible.visibleFade()) {
                _pesquisaExtratoAcertoViagem.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaExtratoAcertoViagem.Visible.visibleFade(true);
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

function loadRelatorioExtratoAcertoViagem() {

    _pesquisaExtratoAcertoViagem = new PesquisaExtratoAcertoViagem();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridExtratoAcertoViagem = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ExtratoAcertoViagem/Pesquisa", _pesquisaExtratoAcertoViagem, null, null, 10);
    _gridExtratoAcertoViagem.SetPermitirEdicaoColunas(true);

    _relatorioExtratoAcertoViagem = new RelatorioGlobal("Relatorios/ExtratoAcertoViagem/BuscarDadosRelatorio", _gridExtratoAcertoViagem, function () {
        _relatorioExtratoAcertoViagem.loadRelatorio(function () {
            KoBindings(_pesquisaExtratoAcertoViagem, "knockoutPesquisaExtratoAcertoViagem");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaExtratoAcertoViagem");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaExtratoAcertoViagem");

            new BuscarMotoristas(_pesquisaExtratoAcertoViagem.Motorista);
            new BuscarVeiculos(_pesquisaExtratoAcertoViagem.Veiculo);
            new BuscarSegmentoVeiculo(_pesquisaExtratoAcertoViagem.SegmentoVeiculo);
            new BuscarJustificativas(_pesquisaExtratoAcertoViagem.Justificativa);
            new BuscarCentroResultado(_pesquisaExtratoAcertoViagem.CentroResultado);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaExtratoAcertoViagem);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioExtratoAcertoViagem.gerarRelatorio("Relatorios/ExtratoAcertoViagem/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioExtratoAcertoViagem.gerarRelatorio("Relatorios/ExtratoAcertoViagem/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}