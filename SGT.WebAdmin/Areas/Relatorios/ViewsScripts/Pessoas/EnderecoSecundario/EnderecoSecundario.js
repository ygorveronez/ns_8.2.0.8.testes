/// <reference path="../../../../../ViewsScripts/Consultas/CategoriaPessoa.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoPessoa.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumModalidadePessoa.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoPessoa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridEnderecoSecundario, _pesquisaEnderecoSecundario, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioEnderecoSecundario;

var EnderecoSecundario = function () {
    this.CodigoIntegracao = PropertyEntity({ text: "Código Integração:", val: ko.observable(""), getType: typesKnockout.string });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Cidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cidade: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridEnderecoSecundario.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function LoadEnderecosSecundario() {
    _pesquisaEnderecoSecundario = new EnderecoSecundario();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridEnderecoSecundario = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/EnderecoSecundario/Pesquisa", _pesquisaEnderecoSecundario, null, null, 10);
    _gridEnderecoSecundario.SetPermitirEdicaoColunas(true);

    _relatorioEnderecoSecundario = new RelatorioGlobal("Relatorios/EnderecoSecundario/BuscarDadosRelatorio", _gridEnderecoSecundario, function () {
        _relatorioEnderecoSecundario.loadRelatorio(function () {
            KoBindings(_pesquisaEnderecoSecundario, "knockoutPesquisaEnderecoSecundario", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaEnderecoSecundario", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaEnderecoSecundario", false);

            new BuscarClientes(_pesquisaEnderecoSecundario.Cliente);
            new BuscarLocalidades(_pesquisaEnderecoSecundario.Cidade);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaEnderecoSecundario);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioEnderecoSecundario.gerarRelatorio("Relatorios/EnderecoSecundario/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioEnderecoSecundario.gerarRelatorio("Relatorios/EnderecoSecundario/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
