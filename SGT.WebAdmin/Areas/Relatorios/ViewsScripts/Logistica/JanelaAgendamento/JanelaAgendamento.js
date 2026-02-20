/// <reference path="../../../../../ViewsScripts/Consultas/TipoCarga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoAgendamentoColeta.js" />

var _gridJanelaAgendamento;
var _pesquisaJanelaAgendamento;
var _CRUDFiltrosRelatorio;

var PesquisaJanelaAgendamento = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.dateTime, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.dateTime, val: ko.observable("") });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor:", idBtnSearch: guid() });
    this.TipoDeCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo De Carga:", idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.CentroDescarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Descarregamento:", idBtnSearch: guid() });

    this.RaizCnpjFornecedor = PropertyEntity({ getType: typesKnockout.raizCnpj, text: "Raiz CNPJ Fornecedor:" });
    this.Senha = PropertyEntity({ getType: typesKnockout.string, text: "Senha:" });
    this.NumeroCarga = PropertyEntity({ getType: typesKnockout.string, text: "Número Carga:" });
    this.NumeroPedido = PropertyEntity({ getType: typesKnockout.string, text: "Número Pedido:" });

    this.JanelaExcedente = PropertyEntity({ text: "Janela Excedente?", val: ko.observable(EnumSimNaoPesquisa.Todos), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), def: EnumSimNaoPesquisa.Todos });
    this.SituacaoAgendamento = PropertyEntity({ getType: typesKnockout.selectMultiple, val: ko.observable([EnumSituacaoAgendamentoColeta.Todas]), options: EnumSituacaoAgendamentoColeta.obterOpcoesPesquisaComSituacoesCanceladas(), def: ko.observable([]), text: "Situações do Agendamento: ", visible: ko.observable(true) });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridJanelaAgendamento.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};


function loadRelatorioJanelaAgendamento() {
    _pesquisaJanelaAgendamento = new PesquisaJanelaAgendamento();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridJanelaAgendamento = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/JanelaAgendamento/Pesquisa", _pesquisaJanelaAgendamento, null, null, 10);
    _gridJanelaAgendamento.SetPermitirEdicaoColunas(true);

    _relatorioJanelaAgendamento = new RelatorioGlobal("Relatorios/JanelaAgendamento/BuscarDadosRelatorio", _gridJanelaAgendamento, function () {
        _relatorioJanelaAgendamento.loadRelatorio(function () {
            KoBindings(_pesquisaJanelaAgendamento, "knockoutPesquisaJanelaAgendamento", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaJanelaAgendamento", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaJanelaAgendamento", false);

            new BuscarTiposdeCarga(_pesquisaJanelaAgendamento.TipoDeCarga);
            new BuscarClientes(_pesquisaJanelaAgendamento.Fornecedor);
            new BuscarFilial(_pesquisaJanelaAgendamento.Filial);
            new BuscarCentrosDescarregamento(_pesquisaJanelaAgendamento.CentroDescarregamento);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaJanelaAgendamento);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioJanelaAgendamento.gerarRelatorio("Relatorios/JanelaAgendamento/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioJanelaAgendamento.gerarRelatorio("Relatorios/JanelaAgendamento/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}