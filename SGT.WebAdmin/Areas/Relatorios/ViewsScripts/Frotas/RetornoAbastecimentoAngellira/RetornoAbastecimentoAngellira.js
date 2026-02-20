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
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Produto.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/SegmentoVeiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoPessoa.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CentroResultado.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Estado.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridRetornoAbastecimentoAngellira, _pesquisaRetornoAbastecimentoAngellira, _CRUDRelatorio, _relatorioRetornoAbastecimentoAngellira, _CRUDFiltrosRelatorio;

var PesquisaRetornoAbastecimentoAngellira = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.dateTime, val: ko.observable(""), def: "" });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.dateTime, val: ko.observable(""), def: "" });
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridRetornoAbastecimentoAngellira.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaRetornoAbastecimentoAngellira.Visible.visibleFade()) {
                _pesquisaRetornoAbastecimentoAngellira.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaRetornoAbastecimentoAngellira.Visible.visibleFade(true);
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

function loadRelatorioRetornoAbastecimentoAngellira() {

    _pesquisaRetornoAbastecimentoAngellira = new PesquisaRetornoAbastecimentoAngellira();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridRetornoAbastecimentoAngellira = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "/Relatorios/RetornoAbastecimentoAngellira/Pesquisa", _pesquisaRetornoAbastecimentoAngellira, null, null, 30);
    _gridRetornoAbastecimentoAngellira.SetPermitirEdicaoColunas(true);

    _relatorioRetornoAbastecimentoAngellira = new RelatorioGlobal("Relatorios/RetornoAbastecimentoAngellira/BuscarDadosRelatorio", _gridRetornoAbastecimentoAngellira, function () {
        _relatorioRetornoAbastecimentoAngellira.loadRelatorio(function () {
            KoBindings(_pesquisaRetornoAbastecimentoAngellira, "knockoutPesquisaRetornoAbastecimentoAngellira");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaRetornoAbastecimentoAngellira");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaRetornoAbastecimentoAngellira");

            new BuscarClientes(_pesquisaRetornoAbastecimentoAngellira.Fornecedor);
            new BuscarVeiculos(_pesquisaRetornoAbastecimentoAngellira.Veiculo);
            new BuscarMotoristas(_pesquisaRetornoAbastecimentoAngellira.Motorista);
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaRetornoAbastecimentoAngellira);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioRetornoAbastecimentoAngellira.gerarRelatorio("Relatorios/RetornoAbastecimentoAngellira/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioRetornoAbastecimentoAngellira.gerarRelatorio("Relatorios/RetornoAbastecimentoAngellira/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
