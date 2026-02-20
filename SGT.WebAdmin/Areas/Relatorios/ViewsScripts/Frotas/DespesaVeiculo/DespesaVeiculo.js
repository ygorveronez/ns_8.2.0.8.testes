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

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioDespesaVeiculo, _gridDespesaVeiculo, _pesquisaDespesaVeiculo, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaDespesaVeiculo = function () {
    this.DataInicial = PropertyEntity({ text: "Data Emissão Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Emissão Final: ", getType: typesKnockout.date });

    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.GrupoProduto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Produto:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.GrupoProdutoPai = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Produto Pai:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.NaturezaOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Natureza da Operação:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridDespesaVeiculo.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******


function loadRelatorioDespesaVeiculo() {

    _pesquisaDespesaVeiculo = new PesquisaDespesaVeiculo();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridDespesaVeiculo = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/DespesaVeiculo/Pesquisa", _pesquisaDespesaVeiculo, null, null, 10);
    _gridDespesaVeiculo.SetPermitirEdicaoColunas(true);

    _relatorioDespesaVeiculo = new RelatorioGlobal("Relatorios/DespesaVeiculo/BuscarDadosRelatorio", _gridDespesaVeiculo, function () {
        _relatorioDespesaVeiculo.loadRelatorio(function () {
            KoBindings(_pesquisaDespesaVeiculo, "knockoutPesquisaDespesaVeiculo");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaDespesaVeiculo");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaDespesaVeiculo");

            new BuscarEmpresa(_pesquisaDespesaVeiculo.Empresa);
            new BuscarVeiculos(_pesquisaDespesaVeiculo.Veiculo);
            new BuscarClientes(_pesquisaDespesaVeiculo.Fornecedor);
            new BuscarProdutoTMS(_pesquisaDespesaVeiculo.Produto);
            new BuscarGruposProdutosTMS(_pesquisaDespesaVeiculo.GrupoProduto, null);
            new BuscarGruposProdutosTMS(_pesquisaDespesaVeiculo.GrupoProdutoPai, null);
            new BuscarNaturezasOperacoesNotaFiscal(_pesquisaDespesaVeiculo.NaturezaOperacao);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiNFe) {
                _pesquisaDespesaVeiculo.Empresa.visible(true);
            }

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaDespesaVeiculo);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioDespesaVeiculo.gerarRelatorio("Relatorios/DespesaVeiculo/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioDespesaVeiculo.gerarRelatorio("Relatorios/DespesaVeiculo/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}