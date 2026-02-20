/// <reference path="../../../../../ViewsScripts/Consultas/TipoCarga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />

var _gridAgendaCancelada;
var _pesquisaAgendaCancelada;
var _CRUDFiltrosRelatorio;
var _relatorioAgendaCancelada;

var PesquisaAgendaCancelada = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.DataInicio = PropertyEntity({ text: "Data Inicial Agendamento: ", getType: typesKnockout.dateTime, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.DataFim = PropertyEntity({ text: "Data Final Agendamento: ", getType: typesKnockout.dateTime, val: ko.observable("") });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor:", idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.TipoDeCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo De Carga:", idBtnSearch: guid() });

    this.NumeroCarga = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), text: "Número da carga:" });
    this.Senha = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), text: "Senha:" });
    this.Pedido = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), text: "Pedido:" });
    this.SituacaoAgendamento = PropertyEntity({ getType: typesKnockout.selectMultiple, val: ko.observable(EnumSituacaoAgendamentoAgendaCancelada.Todas), options: EnumSituacaoAgendamentoAgendaCancelada.obterOpcoesPesquisa(), def: EnumSituacaoAgendamentoAgendaCancelada.Todas, text: "Situação Agendamento:", visible: ko.observable(true) });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridAgendaCancelada.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaAgendaCancelada.Visible.visibleFade()) {
                _pesquisaAgendaCancelada.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaAgendaCancelada.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};


function loadRelatorioAgendaCancelada() {
    _pesquisaAgendaCancelada = new PesquisaAgendaCancelada();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridAgendaCancelada = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/AgendaCancelada/Pesquisa", _pesquisaAgendaCancelada, null, null, 10);
    _gridAgendaCancelada.SetPermitirEdicaoColunas(true);

    _relatorioAgendaCancelada = new RelatorioGlobal("Relatorios/AgendaCancelada/BuscarDadosRelatorio", _gridAgendaCancelada, function () {
        _relatorioAgendaCancelada.loadRelatorio(function () {
            KoBindings(_pesquisaAgendaCancelada, "knockoutPesquisaAgendaCancelada", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaAgendaCancelada", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaAgendaCancelada", false);

            new BuscarTiposdeCarga(_pesquisaAgendaCancelada.TipoDeCarga);
            new BuscarClientes(_pesquisaAgendaCancelada.Fornecedor);
            new BuscarClientes(_pesquisaAgendaCancelada.Destinatario);
            new BuscarFilial(_pesquisaAgendaCancelada.Filial);
            
            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaAgendaCancelada);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioAgendaCancelada.gerarRelatorio("Relatorios/AgendaCancelada/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioAgendaCancelada.gerarRelatorio("Relatorios/AgendaCancelada/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}