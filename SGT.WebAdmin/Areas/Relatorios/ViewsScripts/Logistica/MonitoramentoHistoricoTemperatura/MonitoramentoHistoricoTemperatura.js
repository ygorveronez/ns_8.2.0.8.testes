/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/FaixaTemperatura.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumMonitoramentoStatus.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSimNaoPesquisa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioMonitoramentoHistoricoTemperatura, _gridMonitoramentoHistoricoTemperatura, _pesquisaMonitoramentoHistoricoTemperatura, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaMonitoramentoHistoricoTemperatura = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.dateTime, val: ko.observable(Global.DataAtual()), def: Global.DataAtual(), required: true });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.dateTime, val: ko.observable("") });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.DataCriacaoCargaInicial = PropertyEntity({ text: "Data Criação Carga Inicial: ", getType: typesKnockout.dateTime, val: ko.observable("")});
    this.DataCriacaoCargaFinal = PropertyEntity({ text: "Data Criação Carga Final: ", getType: typesKnockout.dateTime, val: ko.observable("") });
    this.DataCriacaoCargaInicial.dateRangeLimit = this.DataCriacaoCargaFinal;
    this.DataCriacaoCargaFinal.dateRangeInit = this.DataCriacaoCargaInicial;

    this.DuranteMonitoramento = PropertyEntity({ val: ko.observable(true), text: "Durante monitoramento", getType: typesKnockout.bool });
    
    this.NumeroCarga = PropertyEntity({ text: "Nº Carga:" });
    this.ForaFaixa = PropertyEntity({ text: "Fora de Faixa?", val: ko.observable(EnumSimNaoPesquisa.Todos), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), def: EnumSimNaoPesquisa.Todos });
    this.StatusMonitoramento = PropertyEntity({ text: "Status Monitoramento: ", val: ko.observable(EnumMonitoramentoStatus.Todas), options: EnumMonitoramentoStatus.obterOpcoesPesquisa(), def: EnumMonitoramentoStatus.Todas });

    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });
    this.FaixaTemperatura = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Faixa de Temperatura:", idBtnSearch: guid() });
    this.StatusViagem = PropertyEntity({ text: "Status da Viagem", val: ko.observable([]), def: new Array(), getType: typesKnockout.selectMultiple, options: ko.observable([]), visible: ko.observable(true) });

    this.EntregasRealizadas = PropertyEntity({ text: "Entregas realizadas?", val: ko.observable(EnumSimNaoPesquisa.Todos), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), def: EnumSimNaoPesquisa.Todos });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridMonitoramentoHistoricoTemperatura.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function LoadMonitoramentoHistoricoTemperatura() {
    _pesquisaMonitoramentoHistoricoTemperatura = new PesquisaMonitoramentoHistoricoTemperatura();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridMonitoramentoHistoricoTemperatura = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/MonitoramentoHistoricoTemperatura/Pesquisa", _pesquisaMonitoramentoHistoricoTemperatura, null, null, 10);
    _gridMonitoramentoHistoricoTemperatura.SetPermitirEdicaoColunas(true);

    buscaStatusViagem(function () {
        _relatorioMonitoramentoHistoricoTemperatura = new RelatorioGlobal("Relatorios/MonitoramentoHistoricoTemperatura/BuscarDadosRelatorio", _gridMonitoramentoHistoricoTemperatura, function () {
            _relatorioMonitoramentoHistoricoTemperatura.loadRelatorio(function () {
                KoBindings(_pesquisaMonitoramentoHistoricoTemperatura, "knockoutPesquisaMonitoramentoHistoricoTemperatura", false);
                KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaMonitoramentoHistoricoTemperatura", false);
                KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaMonitoramentoHistoricoTemperatura", false);

                new BuscarTransportadores(_pesquisaMonitoramentoHistoricoTemperatura.Transportador);
                new BuscarVeiculos(_pesquisaMonitoramentoHistoricoTemperatura.Veiculo);
                new BuscarFilial(_pesquisaMonitoramentoHistoricoTemperatura.Filial);
                new BuscarFaixaTemperatura(_pesquisaMonitoramentoHistoricoTemperatura.FaixaTemperatura);

                $("#divConteudoRelatorio").show();
            });
        }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaMonitoramentoHistoricoTemperatura);
    }, _pesquisaMonitoramentoHistoricoTemperatura.StatusViagem);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioMonitoramentoHistoricoTemperatura.gerarRelatorio("Relatorios/MonitoramentoHistoricoTemperatura/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioMonitoramentoHistoricoTemperatura.gerarRelatorio("Relatorios/MonitoramentoHistoricoTemperatura/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}

function buscaStatusViagem(callback, statusViagem) {

    executarReST("MonitoramentoStatusViagem/BuscarTodos", null, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {

                statusViagem.options(arg.Data.StatusViagem);

                $("#" + statusViagem.id).selectpicker('refresh');

                callback();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}