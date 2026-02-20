/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ContratoFreteTransportador.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoProduto.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoPessoa.js" />
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
/// <reference path="../../../../../ViewsScripts/Consultas/Localidade.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../js/app.config.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridComissaoGrupoProduto, _pesquisaComissaoGrupoProduto, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioComissaoGrupoProduto;

var PesquisaComissaoGrupoProduto = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid(), issue: 52, visible: ko.observable(true) });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ text: "Transportador:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), issue: 63, visible: ko.observable(true) });
    this.ContratoFreteTransportador = PropertyEntity({ text: "Contrato de Frete:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoProduto = PropertyEntity({ text: "Grupo de Produto:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Produto = PropertyEntity({ text: "Produto:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridComissaoGrupoProduto.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaComissaoGrupoProduto.Visible.visibleFade() == true) {
                _pesquisaComissaoGrupoProduto.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaComissaoGrupoProduto.Visible.visibleFade(true);
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

function LoadComissaoGrupoProduto() {
    _pesquisaComissaoGrupoProduto = new PesquisaComissaoGrupoProduto();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridComissaoGrupoProduto = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ComissaoGrupoProduto/Pesquisa", _pesquisaComissaoGrupoProduto);

    _gridComissaoGrupoProduto.SetPermitirEdicaoColunas(true);
    _gridComissaoGrupoProduto.SetQuantidadeLinhasPorPagina(10);

    _relatorioComissaoGrupoProduto = new RelatorioGlobal("Relatorios/ComissaoGrupoProduto/BuscarDadosRelatorio", _gridComissaoGrupoProduto, function () {
        _relatorioComissaoGrupoProduto.loadRelatorio(function () {
            KoBindings(_pesquisaComissaoGrupoProduto, "knockoutPesquisaComissaoGrupoProduto", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaComissaoGrupoProduto", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaComissaoGrupoProduto", false);

            new BuscarClientes(_pesquisaComissaoGrupoProduto.Pessoa);
            new BuscarTransportadores(_pesquisaComissaoGrupoProduto.Transportador);
            new BuscarGruposPessoas(_pesquisaComissaoGrupoProduto.GrupoPessoas, null, null, null, EnumTipoGrupoPessoas.Clientes);
            new BuscarGruposProdutos(_pesquisaComissaoGrupoProduto.GrupoProduto);
            new BuscarContratoFreteTransportador(_pesquisaComissaoGrupoProduto.ContratoFreteTransportador);
            new BuscarProdutos(_pesquisaComissaoGrupoProduto.Produto);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaComissaoGrupoProduto);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioComissaoGrupoProduto.gerarRelatorio("Relatorios/ComissaoGrupoProduto/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioComissaoGrupoProduto.gerarRelatorio("Relatorios/ComissaoGrupoProduto/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}