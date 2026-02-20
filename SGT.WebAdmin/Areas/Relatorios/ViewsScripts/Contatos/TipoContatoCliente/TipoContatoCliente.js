//*******MAPEAMENTO KNOUCKOUT*******

var _gridTipoContatoCliente, _pesquisaTipoContatoCliente, _CRUDRelatorio, _CRUDFiltrosRelatorio;
var _relatorioTipoContatoCliente;

var PesquisaTipoContatoCliente = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoContato = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, text: "*Tipo de Contato:", options: ko.observable([]), url: "TipoContato/BuscarTodos", visible: ko.observable(true) });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridTipoContatoCliente.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaTipoContatoCliente.Visible.visibleFade() == true) {
                _pesquisaTipoContatoCliente.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaTipoContatoCliente.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(false)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function LoadTipoContatoCliente() {
    _pesquisaTipoContatoCliente = new PesquisaTipoContatoCliente();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridTipoContatoCliente = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/TipoContatoCliente/Pesquisa", _pesquisaTipoContatoCliente);

    _gridTipoContatoCliente.SetPermitirEdicaoColunas(true);
    _gridTipoContatoCliente.SetQuantidadeLinhasPorPagina(10);

    _relatorioTipoContatoCliente = new RelatorioGlobal("Relatorios/TipoContatoCliente/BuscarDadosRelatorio", _gridTipoContatoCliente, function () {
        _relatorioTipoContatoCliente.loadRelatorio(function () {
            KoBindings(_pesquisaTipoContatoCliente, "knockoutPesquisaTipoContatoCliente", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaTipoContatoCliente", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaTipoContatoCliente", false);

            new BuscarClientes(_pesquisaTipoContatoCliente.Pessoa);
            new BuscarGruposPessoas(_pesquisaTipoContatoCliente.GrupoPessoa);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaTipoContatoCliente);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioTipoContatoCliente.gerarRelatorio("Relatorios/TipoContatoCliente/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioTipoContatoCliente.gerarRelatorio("Relatorios/TipoContatoCliente/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
