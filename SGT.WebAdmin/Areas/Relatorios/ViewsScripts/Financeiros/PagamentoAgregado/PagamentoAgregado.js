/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/Rest.js" />
/// <reference path="../../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../../js/Global/Grid.js" />
/// <reference path="../../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoPagamentoAgregado.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoOperacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridPagamentoAgregado, _pesquisaPagamentoAgregado, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioPagamentoAgregado;

var _situacaoPagamentoAgregado = [
    { text: "Todas", value: "" },
    { text: "Iniciado", value: EnumSituacaoPagamentoAgregado.Iniciado },
    { text: "Ag. Aprovação", value: EnumSituacaoPagamentoAgregado.AgAprovacao },
    { text: "Finalizado", value: EnumSituacaoPagamentoAgregado.Finalizado },
    { text: "Rejeitada", value: EnumSituacaoPagamentoAgregado.Rejeitada },
    { text: "Sem Regra", value: EnumSituacaoPagamentoAgregado.SemRegra }
];

var PesquisaPagamentoAgregado = function () {
    this.NumeroInicial = PropertyEntity({ text: "Número Inicial:", getType: typesKnockout.int });
    this.NumeroFinal = PropertyEntity({ text: "Número Final:", getType: typesKnockout.int });
    this.DataPagamentoInicial = PropertyEntity({ text: "Data Pagamento De:", getType: typesKnockout.date });
    this.DataPagamentoFinal = PropertyEntity({ text: "Até:", getType: typesKnockout.date });

    this.Situacao = PropertyEntity({ text: "Situação:", options: _situacaoPagamentoAgregado });
    this.Agregado = PropertyEntity({ text: "Agregado:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ text: "Motorista:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid() });

    this.Veiculo = PropertyEntity({ text: "Veículo:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid() });
    this.NumeroCTe = PropertyEntity({ text: "Número CT-e:", getType: typesKnockout.int });
    this.NumeroCarga = PropertyEntity({ text: "Número Carga:", getType: typesKnockout.string });
    this.TipoOperacao = PropertyEntity({ text: "Tipo da Operação:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid() });

    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridPagamentoAgregado.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaPagamentoAgregado.Visible.visibleFade() === true) {
                _pesquisaPagamentoAgregado.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaPagamentoAgregado.Visible.visibleFade(true);
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

function loadRelatorioPagamentoAgregado() {
    _pesquisaPagamentoAgregado = new PesquisaPagamentoAgregado();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridPagamentoAgregado = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/PagamentoAgregado/Pesquisa", _pesquisaPagamentoAgregado);

    _gridPagamentoAgregado.SetPermitirEdicaoColunas(true);
    _gridPagamentoAgregado.SetQuantidadeLinhasPorPagina(10);

    _relatorioPagamentoAgregado = new RelatorioGlobal("Relatorios/PagamentoAgregado/BuscarDadosRelatorio", _gridPagamentoAgregado, function () {
        _relatorioPagamentoAgregado.loadRelatorio(function () {
            KoBindings(_pesquisaPagamentoAgregado, "knockoutPesquisaPagamentoAgregado", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaPagamentoAgregado", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaPagamentoAgregado", false);

            // Buscas
            new BuscarClientes(_pesquisaPagamentoAgregado.Agregado);
            new BuscarMotoristas(_pesquisaPagamentoAgregado.Motorista);
            new BuscarVeiculos(_pesquisaPagamentoAgregado.Veiculo);
            new BuscarTiposOperacao(_pesquisaPagamentoAgregado.TipoOperacao);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaPagamentoAgregado);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioPagamentoAgregado.gerarRelatorio("Relatorios/PagamentoAgregado/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioPagamentoAgregado.gerarRelatorio("Relatorios/PagamentoAgregado/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
