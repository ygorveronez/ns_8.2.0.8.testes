/// <reference path="../../../../../ViewsScripts/Consultas/TipoOperacao.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoAgendamentoEntregaPedido.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioConsultaPorNotaFiscal, _gridConsultaPorNotaFiscal, _pesquisaConsultaPorNotaFiscal, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaConsultaPorNotaFiscal = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid() });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.Expedidor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Expedidor:", idBtnSearch: guid() });
    this.Recebedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Recebedor:", idBtnSearch: guid() });
    this.TipoTrecho = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: " ", idBtnSearch: guid(), visible: ko.observable(true) });

    this.NumeroCarga = PropertyEntity({ type: types.string, val: ko.observable(""), text: "Número da Carga:" });
    this.NumeroNota = PropertyEntity({ type: types.int, val: ko.observable(""), text: "Número da Nota:" });

    this.DataPrevisaoEntregaInicial = PropertyEntity({ text: "Data Previsão Entrega Inicial: ", getType: typesKnockout.date });
    this.DataPrevisaoEntregaFinal = PropertyEntity({ text: "Data Previsão Entrega Final: ", getType: typesKnockout.date });
    this.DataCarregamentoInicial = PropertyEntity({ text: "Data Carregamento Inicial: ", getType: typesKnockout.date });
    this.DataCarregamentoFinal = PropertyEntity({ text: "Data Carregamento Final: ", getType: typesKnockout.date });
    this.DataAgendamentoInicial = PropertyEntity({ text: "Data Agendamento Inicial: ", getType: typesKnockout.date });
    this.DataAgendamentoFinal = PropertyEntity({ text: "Data Agendamento Final: ", getType: typesKnockout.date });

    this.SituacaoAgendamento = PropertyEntity({ val: ko.observable(EnumSituacaoAgendamentoEntregaPedido.Todas), options: EnumSituacaoAgendamentoEntregaPedido.obterOpcoesPesquisa(), text: "Status: ", def: EnumSituacaoAgendamentoEntregaPedido.Todas });

    this.DataAgendamentoInicial.dateRangeLimit = this.DataAgendamentoFinal;
    this.DataAgendamentoFinal.dateRangeInit = this.DataAgendamentoInicial;

    this.DataCarregamentoInicial.dateRangeLimit = this.DataCarregamentoFinal;
    this.DataCarregamentoFinal.dateRangeInit = this.DataCarregamentoInicial;

    this.DataPrevisaoEntregaInicial.dateRangeLimit = this.DataPrevisaoEntregaFinal;
    this.DataPrevisaoEntregaFinal.dateRangeInit = this.DataPrevisaoEntregaInicial;
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridConsultaPorNotaFiscal.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaConsultaPorNotaFiscal.Visible.visibleFade()) {
                _pesquisaConsultaPorNotaFiscal.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaConsultaPorNotaFiscal.Visible.visibleFade(true);
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

function LoadConsultaPorNotaFiscal() {
    _pesquisaConsultaPorNotaFiscal = new PesquisaConsultaPorNotaFiscal();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridConsultaPorNotaFiscal = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ConsultaPorNotaFiscal/Pesquisa", _pesquisaConsultaPorNotaFiscal, null, null, 10);
    _gridConsultaPorNotaFiscal.SetPermitirEdicaoColunas(true);

    _relatorioConsultaPorNotaFiscal = new RelatorioGlobal("Relatorios/ConsultaPorNotaFiscal/BuscarDadosRelatorio", _gridConsultaPorNotaFiscal, function () {
        _relatorioConsultaPorNotaFiscal.loadRelatorio(function () {
            KoBindings(_pesquisaConsultaPorNotaFiscal, "knockoutPesquisaConsultaPorNotaFiscal", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaConsultaPorNotaFiscal", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaConsultaPorNotaFiscal", false);

            BuscarTransportadores(_pesquisaConsultaPorNotaFiscal.Transportador);
            BuscarTiposOperacao(_pesquisaConsultaPorNotaFiscal.TipoOperacao);
            BuscarClientes(_pesquisaConsultaPorNotaFiscal.Cliente);
            BuscarFilial(_pesquisaConsultaPorNotaFiscal.Filial);
            BuscarClientes(_pesquisaConsultaPorNotaFiscal.Expedidor);
            BuscarClientes(_pesquisaConsultaPorNotaFiscal.Recebedor);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaConsultaPorNotaFiscal);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioConsultaPorNotaFiscal.gerarRelatorio("Relatorios/ConsultaPorNotaFiscal/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioConsultaPorNotaFiscal.gerarRelatorio("Relatorios/ConsultaPorNotaFiscal/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}