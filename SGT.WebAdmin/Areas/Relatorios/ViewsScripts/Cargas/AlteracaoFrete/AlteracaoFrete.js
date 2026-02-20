/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoOperacao.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoAlteracaoFreteCarga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridAlteracaoFrete, _pesquisaAlteracaoFrete, _CRUDRelatorio, _relatorioAlteracaoFrete, _CRUDFiltrosRelatorio;

var _situacaoCargaEmbarcador = [
    { text: "Com a Logística", value: EnumSituacoesCarga.NaLogistica },
    { text: "Dados da Carga", value: EnumSituacoesCarga.Nova },
    { text: "NF-e", value: EnumSituacoesCarga.AgNFe },
    { text: "Cálculo de Frete", value: EnumSituacoesCarga.CalculoFrete },
    { text: "Transportador", value: EnumSituacoesCarga.AgTransportador },
    { text: "Emissão dos Documentos", value: EnumSituacoesCarga.PendeciaDocumentos },
    { text: "Integração", value: EnumSituacoesCarga.AgIntegracao },
    { text: "Impressão", value: EnumSituacoesCarga.AgImpressaoDocumentos },
    { text: "Em Transporte", value: EnumSituacoesCarga.EmTransporte },
    { text: "Encerrada", value: EnumSituacoesCarga.Encerrada },
    { text: "Pagamento Liberado", value: EnumSituacoesCarga.LiberadoPagamento },
    { text: "Canceladas", value: EnumSituacoesCarga.Cancelada }
];

var _situacaoCargaTMS = [
    { text: "Em Andamento", value: EnumSituacoesCarga.NaLogistica },
    { text: "Etapa 1 (Carga)", value: EnumSituacoesCarga.Nova },
    { text: "Etapa 2 (NF-e)", value: EnumSituacoesCarga.AgNFe },
    { text: "Etapa 3 (Frete)", value: EnumSituacoesCarga.CalculoFrete },
    { text: "Etapa 4 e 5 (Documentos)", value: EnumSituacoesCarga.PendeciaDocumentos },
    { text: "Etapa 6 (Integração)", value: EnumSituacoesCarga.AgIntegracao },
    { text: "Em Transporte", value: EnumSituacoesCarga.EmTransporte },
    { text: "Finalizada", value: EnumSituacoesCarga.Encerrada },
    { text: "Cancelada", value: EnumSituacoesCarga.Cancelada },
    { text: "Anulada", value: EnumSituacoesCarga.Anulada }
];

var PesquisaAlteracaoFrete = function () {
    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });

    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", issue: 2, getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", issue: 2, getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.CodigoCargaEmbarcador = PropertyEntity({ type: types.map, text: "Nº Carga:" });

    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable("Transportador:"), issue: 69, idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial:", issue: 63, idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Carga:", issue: 53, idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Operação:", issue: 121, idBtnSearch: guid() });
    this.ModeloVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo de Veículo:", issue: 144, idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", issue: 143, idBtnSearch: guid() });
    this.Operador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador:", issue: 145, idBtnSearch: guid() });

    var situacaoCargaPesquisa = _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador ? _situacaoCargaEmbarcador : _situacaoCargaTMS;
    this.Situacoes = PropertyEntity({ text: "Situação da Carga: ", val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, params: { Tipo: "", Ativo: situacaoCargaPesquisa.Todos, OpcaoSemGrupo: false }, issue: 533, options: ko.observable(situacaoCargaPesquisa), });
    this.SituacaoAlteracaoFrete = PropertyEntity({ text: "Situação Alteração do Frete: ", val: ko.observable(EnumSituacaoAlteracaoFreteCarga.Todas), options: EnumSituacaoAlteracaoFreteCarga.obterOpcoesPesquisaAutorizacao(), def: EnumSituacaoAlteracaoFreteCarga.Todas });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridAlteracaoFrete.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaAlteracaoFrete.Visible.visibleFade()) {
                _pesquisaAlteracaoFrete.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaAlteracaoFrete.Visible.visibleFade(true);
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

function LoadRelatorioAlteracaoFrete() {
    _pesquisaAlteracaoFrete = new PesquisaAlteracaoFrete();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridAlteracaoFrete = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "/Relatorios/AlteracaoFrete/Pesquisa", _pesquisaAlteracaoFrete, null, null, 10, null, null, null, null, 20);
    _gridAlteracaoFrete.SetPermitirEdicaoColunas(true);

    _relatorioAlteracaoFrete = new RelatorioGlobal("Relatorios/AlteracaoFrete/BuscarDadosRelatorio", _gridAlteracaoFrete, function () {
        _relatorioAlteracaoFrete.loadRelatorio(function () {
            KoBindings(_pesquisaAlteracaoFrete, "knockoutPesquisaAlteracaoFrete");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaAlteracaoFrete");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaAlteracaoFrete");

            new BuscarTransportadores(_pesquisaAlteracaoFrete.Transportador, null, null, true);
            new BuscarVeiculos(_pesquisaAlteracaoFrete.Veiculo);
            new BuscarTiposdeCarga(_pesquisaAlteracaoFrete.TipoCarga);
            new BuscarModelosVeicularesCarga(_pesquisaAlteracaoFrete.ModeloVeiculo);
            new BuscarFilial(_pesquisaAlteracaoFrete.Filial);
            new BuscarOperador(_pesquisaAlteracaoFrete.Operador);
            new BuscarTiposOperacao(_pesquisaAlteracaoFrete.TipoOperacao);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
                _pesquisaAlteracaoFrete.Filial.visible(false);
                _pesquisaAlteracaoFrete.Transportador.text("Empresa/Filial:");
            }

        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaAlteracaoFrete);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioAlteracaoFrete.gerarRelatorio("Relatorios/AlteracaoFrete/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioAlteracaoFrete.gerarRelatorio("Relatorios/AlteracaoFrete/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}