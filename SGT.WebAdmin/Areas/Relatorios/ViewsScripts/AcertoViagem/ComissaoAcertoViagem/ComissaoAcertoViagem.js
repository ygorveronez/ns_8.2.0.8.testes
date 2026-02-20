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
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/AcertoViagem.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/SegmentoVeiculo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioComissaoAcertoViagem, _gridComissaoAcertoViagem, _pesquisaComissaoAcertoViagem, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaComissaoAcertoViagem = function () {
    this.DataInicial = PropertyEntity({ text: "Período inicial do acerto: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataVencimentoInicial = PropertyEntity({ text: "Período vencimento inicial: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataVencimentoFinal = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.DataVencimentoInicial.dateRangeLimit = this.DataVencimentoFinal;
    this.DataVencimentoFinal.dateRangeInit = this.DataVencimentoInicial;

    this.AcertoViagem = PropertyEntity({ text: "Acerto de Viagem:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ text: "Motorista:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Segmento = PropertyEntity({ text: "Segmento:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.ExibirOcorrencias = PropertyEntity({ text: "Exibir as ocorrências lançadas no PDF/Excel dos filtros gerados?", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridComissaoAcertoViagem.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function loadRelatorioComissaoAcertoViagem() {

    _pesquisaComissaoAcertoViagem = new PesquisaComissaoAcertoViagem();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridComissaoAcertoViagem = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ComissaoAcertoViagem/Pesquisa", _pesquisaComissaoAcertoViagem, null, null, 10);
    _gridComissaoAcertoViagem.SetPermitirEdicaoColunas(true);

    _relatorioComissaoAcertoViagem = new RelatorioGlobal("Relatorios/ComissaoAcertoViagem/BuscarDadosRelatorio", _gridComissaoAcertoViagem, function () {
        _relatorioComissaoAcertoViagem.loadRelatorio(function () {
            KoBindings(_pesquisaComissaoAcertoViagem, "knockoutPesquisaComissaoAcertoViagem");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaComissaoAcertoViagem");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaComissaoAcertoViagem");

            new BuscarMotoristas(_pesquisaComissaoAcertoViagem.Motorista);
            new BuscarSegmentoVeiculo(_pesquisaComissaoAcertoViagem.Segmento);
            new BuscarAcertoViagem(_pesquisaComissaoAcertoViagem.AcertoViagem, RetornoBuscarAcertoViagem);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaComissaoAcertoViagem);

}

function RetornoBuscarAcertoViagem(data) {
    _pesquisaComissaoAcertoViagem.AcertoViagem.codEntity(data.Codigo);
    _pesquisaComissaoAcertoViagem.AcertoViagem.val(data.Numero);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioComissaoAcertoViagem.gerarRelatorio("Relatorios/ComissaoAcertoViagem/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioComissaoAcertoViagem.gerarRelatorio("Relatorios/ComissaoAcertoViagem/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
