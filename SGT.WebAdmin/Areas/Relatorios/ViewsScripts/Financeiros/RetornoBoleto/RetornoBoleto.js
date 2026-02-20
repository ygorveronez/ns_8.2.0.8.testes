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
/// <reference path="../../../../../ViewsScripts/Consultas/BoletoConfiguracao.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/BoletoRetornoComando.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioRetornoBoleto, _gridRetornoBoleto, _pesquisaRetornoBoleto, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaRetornoBoleto = function () {
    this.DataInicialImportacao = PropertyEntity({ text: "Data de importação de: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinalImportacao = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataInicialOcorrencia = PropertyEntity({ text: "Data da ocorrência de: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinalOcorrencia = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });

    this.BoletoConfiguracao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Banco:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.BoletoComando = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Comando Banco:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridExtratoConta.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: guid(), visible: ko.observable(true)
    });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridRetornoBoleto.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function loadRelatorioRetornoBoleto() {

    _pesquisaRetornoBoleto = new PesquisaRetornoBoleto();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridRetornoBoleto = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/RetornoBoleto/Pesquisa", _pesquisaRetornoBoleto, null, null, 10);
    _gridRetornoBoleto.SetPermitirEdicaoColunas(true);

    _relatorioRetornoBoleto = new RelatorioGlobal("Relatorios/RetornoBoleto/BuscarDadosRelatorio", _gridRetornoBoleto, function () {
        _relatorioRetornoBoleto.loadRelatorio(function () {
            KoBindings(_pesquisaRetornoBoleto, "knockoutPesquisaRetornoBoleto");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaRetornoBoleto");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaRetornoBoleto");

            new BuscarBoletoConfiguracao(_pesquisaRetornoBoleto.BoletoConfiguracao, RetornoConfiguracaoBanco);
            new BuscarBoletoRetornoComando(_pesquisaRetornoBoleto.BoletoComando, RetornoComandoBanco);

            $("#divConteudoRelatorio").show();
        })
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaRetornoBoleto);
}

function RetornoConfiguracaoBanco(data) {
    _pesquisaRetornoBoleto.BoletoConfiguracao.codEntity(data.Codigo);
    _pesquisaRetornoBoleto.BoletoConfiguracao.val(data.DescricaoBanco);
}

function RetornoComandoBanco(data) {
    _pesquisaRetornoBoleto.BoletoComando.codEntity(data.Codigo);
    _pesquisaRetornoBoleto.BoletoComando.val(data.Descricao);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioRetornoBoleto.gerarRelatorio("Relatorios/RetornoBoleto/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioRetornoBoleto.gerarRelatorio("Relatorios/RetornoBoleto/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
