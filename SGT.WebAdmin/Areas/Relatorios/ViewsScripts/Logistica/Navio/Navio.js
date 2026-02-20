/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoEmbarcacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

let _relatorioNavio, _gridNavio, _pesquisaNavio, _CRUDRelatorio, _CRUDFiltrosRelatorio;

const PesquisaNavio = function () {
    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });

    this.Descricao = PropertyEntity({ text: "Descrição:", visible: ko.observable(true), getType: typesKnockout.string });
    this.CodigoIntegracao = PropertyEntity({ text: "Código Integração:", visible: ko.observable(true), getType: typesKnockout.string });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(1), getType: typesKnockout.selectMultiple, options: _statusPesquisa, def: 1 });
    this.CodigoIrin = PropertyEntity({ text: "Código Irin:", visible: ko.observable(true), getType: typesKnockout.string });
    this.CodigoEmbarcacao = PropertyEntity({ text: "Código Embarcação:", visible: ko.observable(true), getType: typesKnockout.string });
    this.TipoEmbarcacao = PropertyEntity({ text: "Tipo Embarcação:", getType: typesKnockout.selectMultiple, val: ko.observable([EnumTipoEmbarcacao.Todas]), options: EnumTipoEmbarcacao.obterOpcoes(), def: ko.observable([]), visible: ko.observable(true) });
    this.CodigoDocumentacao = PropertyEntity({ text: "Código Documentação:", visible: ko.observable(true), getType: typesKnockout.string });
    this.CodigoIMO = PropertyEntity({ text: "Código IMO:", visible: ko.observable(true), getType: typesKnockout.string });
    this.NavioID = PropertyEntity({ text: "Navio ID:", visible: ko.observable(true), getType: typesKnockout.string });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid() });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

const CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridNavio.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaNavio.Visible.visibleFade()) {
                _pesquisaNavio.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaNavio.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
};

const CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function LoadNavio() {
    _pesquisaNavio = new PesquisaNavio();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridNavio = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/Navio/Pesquisa", _pesquisaNavio);

    _gridNavio.SetPermitirEdicaoColunas(true);

    _relatorioNavio = new RelatorioGlobal("Relatorios/Navio/BuscarDadosRelatorio", _gridNavio, function () {
        _relatorioNavio.loadRelatorio(function () {
            KoBindings(_pesquisaNavio, "knockoutPesquisaNavio", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaNavio", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaNavio", false);


            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaNavio);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioNavio.gerarRelatorio("Relatorios/Navio/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioNavio.gerarRelatorio("Relatorios/Navio/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}