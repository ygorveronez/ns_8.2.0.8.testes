/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Produto.js" />
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
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../js/app.config.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridComissaoProduto, _pesquisaComissaoProduto, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioComissaoProduto;

var PesquisaComissaoProduto = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid(), issue: 52, visible: ko.observable(true) });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ text: "Transportador:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), issue: 63, visible: ko.observable(true) });
    this.ContratoFreteTransportador = PropertyEntity({ text: "Contrato de Frete:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Produto = PropertyEntity({ text: "Produto:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridComissaoProduto.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaComissaoProduto.Visible.visibleFade() == true) {
                _pesquisaComissaoProduto.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaComissaoProduto.Visible.visibleFade(true);
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

function LoadComissaoProduto() {
    _pesquisaComissaoProduto = new PesquisaComissaoProduto();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridComissaoProduto = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ComissaoProduto/Pesquisa", _pesquisaComissaoProduto);

    _gridComissaoProduto.SetPermitirEdicaoColunas(true);
    _gridComissaoProduto.SetQuantidadeLinhasPorPagina(10);

    _relatorioComissaoProduto = new RelatorioGlobal("Relatorios/ComissaoProduto/BuscarDadosRelatorio", _gridComissaoProduto, function () {
        _relatorioComissaoProduto.loadRelatorio(function () {
            KoBindings(_pesquisaComissaoProduto, "knockoutPesquisaComissaoProduto", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaComissaoProduto", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaComissaoProduto", false);

            new BuscarClientes(_pesquisaComissaoProduto.Pessoa);
            new BuscarTransportadores(_pesquisaComissaoProduto.Transportador);
            new BuscarGruposPessoas(_pesquisaComissaoProduto.GrupoPessoas, null, null, null, EnumTipoGrupoPessoas.Clientes);
            new BuscarProdutos(_pesquisaComissaoProduto.Produto);
            new BuscarContratoFreteTransportador(_pesquisaComissaoProduto.ContratoFreteTransportador);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaComissaoProduto);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioComissaoProduto.gerarRelatorio("Relatorios/ComissaoProduto/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioComissaoProduto.gerarRelatorio("Relatorios/ComissaoProduto/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
