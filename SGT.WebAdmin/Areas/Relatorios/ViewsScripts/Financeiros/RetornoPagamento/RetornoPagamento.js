/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Conhecimento.js" />
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
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/BoletoConfiguracao.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Banco.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioRetornoPagamento, _gridRetornoPagamento, _pesquisaRetornoPagamento, _CRUDRelatorio, _CRUDFiltrosRelatorio;


var PesquisaRetornoPagamento = function () {
    this.DataInicialImportacao = PropertyEntity({ text: "Data importação de: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinalImportacao = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataInicialPagamento = PropertyEntity({ text: "Data pagamento de: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinalPagamento = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });

    this.Titulo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Título:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.ConfiguracaoBanco = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Configuração Banco:", idBtnSearch: guid() });
    this.Comando = PropertyEntity({ text: "Comando:", getType: typesKnockout.string, def: "" });
    this.Banco = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Banco:", idBtnSearch: guid() });
    this.BancoPessoa = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Banco da Pessoa:", idBtnSearch: guid() });

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
            _gridRetornoPagamento.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function loadRelatorioRetornoPagamento() {

    _pesquisaRetornoPagamento = new PesquisaRetornoPagamento();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridRetornoPagamento = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/RetornoPagamento/Pesquisa", _pesquisaRetornoPagamento, null, null, 10);
    _gridRetornoPagamento.SetPermitirEdicaoColunas(true);

    _relatorioRetornoPagamento = new RelatorioGlobal("Relatorios/RetornoPagamento/BuscarDadosRelatorio", _gridRetornoPagamento, function () {
        _relatorioRetornoPagamento.loadRelatorio(function () {
            KoBindings(_pesquisaRetornoPagamento, "knockoutPesquisaRetornoPagamento");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaRetornoPagamento");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaRetornoPagamento");

            new BuscarClientes(_pesquisaRetornoPagamento.Pessoa);
            new BuscarTitulo(_pesquisaRetornoPagamento.Titulo, null, null, RetornoTitulo);
            new BuscarBoletoConfiguracao(_pesquisaRetornoPagamento.ConfiguracaoBanco);
            new BuscarBanco(_pesquisaRetornoPagamento.Banco);
            new BuscarBanco(_pesquisaRetornoPagamento.BancoPessoa);

            $("#divConteudoRelatorio").show();
        })
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaRetornoPagamento);

}

function RetornoTitulo(data) {
    _pesquisaRetornoPagamento.Titulo.val(data.Codigo);
    _pesquisaRetornoPagamento.Titulo.codEntity(data.Codigo);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioRetornoPagamento.gerarRelatorio("Relatorios/RetornoPagamento/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioRetornoPagamento.gerarRelatorio("Relatorios/RetornoPagamento/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
