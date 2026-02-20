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
/// <reference path="../../../../../ViewsScripts/Consultas/ServicoVeiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ModeloVeiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/MarcaVeiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Produto.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Equipamento.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoProdutoTMS.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoContratoFinanciamento.js" />


var _relatorioContratoFinanceiro;
var _gridContratoFinanceiro;
var _pesquisaContratoFinanceiro;
var _CRUDRelatorio;
var _CRUDFiltrosRelatorio;

var PesquisaContratoFinanceiro = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.NumeroDocumento = PropertyEntity({ text: "Número Documento:" });
    this.NumeroDocumentoEntrada = PropertyEntity({ text: "Número Documento Entrada:" });
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable([]), def: [], getType: typesKnockout.selectMultiple, options: EnumSituacaoContratoFinanciamento.obterOpcoes(), text: "Situação:", visible: ko.observable(true) });

    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridContratoFinanceiro.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaContratoFinanceiro.Visible.visibleFade()) {
                _pesquisaContratoFinanceiro.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fa fa-plus");
            } else {
                _pesquisaContratoFinanceiro.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fa fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fa fa-plus"), visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

/****** EVENTOS ******/

function loadRelatorioContratoFinanceiro() {
    _pesquisaContratoFinanceiro = new PesquisaContratoFinanceiro();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridContratoFinanceiro = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ContratoFinanceiro/Pesquisa", _pesquisaContratoFinanceiro, null, null, 10);
    _gridContratoFinanceiro.SetPermitirEdicaoColunas(true);

    _relatorioContratoFinanceiro = new RelatorioGlobal("Relatorios/ContratoFinanceiro/BuscarDadosRelatorio", _gridContratoFinanceiro, function () {
        _relatorioContratoFinanceiro.loadRelatorio(function () {
            KoBindings(_pesquisaContratoFinanceiro, "knockoutPesquisaContratoFinanceiro");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaContratoFinanceiro");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaContratoFinanceiro");

            new BuscarVeiculos(_pesquisaContratoFinanceiro.Veiculo);
            new BuscarClientes(_pesquisaContratoFinanceiro.Fornecedor);
            new BuscarClientes(_pesquisaContratoFinanceiro.Empresa)

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaContratoFinanceiro);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioContratoFinanceiro.gerarRelatorio("Relatorios/ContratoFinanceiro/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioContratoFinanceiro.gerarRelatorio("Relatorios/ContratoFinanceiro/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}