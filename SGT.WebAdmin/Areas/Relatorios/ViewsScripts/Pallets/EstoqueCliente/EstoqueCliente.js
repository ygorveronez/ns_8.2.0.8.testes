//*******MAPEAMENTO KNOUCKOUT*******

var _gridEstoqueCliente, _pesquisaEstoqueCliente, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioEstoqueCliente;


var PesquisaEstoqueCliente = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFim = PropertyEntity({ text: "Data Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid(), visible: ko.observable(true) });


    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridEstoqueCliente.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaEstoqueCliente.Visible.visibleFade() == true) {
                _pesquisaEstoqueCliente.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaEstoqueCliente.Visible.visibleFade(true);
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

function LoadEstoqueCliente() {
    _pesquisaEstoqueCliente = new PesquisaEstoqueCliente();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridEstoqueCliente = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/EstoqueCliente/Pesquisa", _pesquisaEstoqueCliente);

    _gridEstoqueCliente.SetPermitirEdicaoColunas(true);
    _gridEstoqueCliente.SetQuantidadeLinhasPorPagina(20);

    _relatorioEstoqueCliente = new RelatorioGlobal("Relatorios/EstoqueCliente/BuscarDadosRelatorio", _gridEstoqueCliente, function () {
        _relatorioEstoqueCliente.loadRelatorio(function () {
            KoBindings(_pesquisaEstoqueCliente, "knockoutPesquisaEstoqueCliente", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaEstoqueCliente", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaEstoqueCliente", false);

            new BuscarClientes(_pesquisaEstoqueCliente.Cliente);
            new BuscarGruposPessoas(_pesquisaEstoqueCliente.GrupoPessoas)

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaEstoqueCliente);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioEstoqueCliente.gerarRelatorio("Relatorios/EstoqueCliente/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioEstoqueCliente.gerarRelatorio("Relatorios/EstoqueCliente/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
