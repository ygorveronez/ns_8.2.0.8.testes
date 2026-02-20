/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../js/Global/Globais.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoIntegracao.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoPagamento.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacoesCarga.js" />


//*******MAPEAMENTO KNOUCKOUT*******

let _gridIntegracaoLotePagamento, _pesquisaIntegracaoLotePagamento, _CRUDRelatorio, _CRUDFiltrosRelatorio;

let _relatorioIntegracaoLotePagamento;

const PesquisaIntegracaoLotePagamento = function () {
    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.NumeroPagamento = PropertyEntity({ text: "Número do Pagamento:", getType: typesKnockout.string, val: ko.observable(""), def: "" });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), issue: 195, visible: ko.observable(true) });
    this.CTe = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "CT-e:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.SituacaoPagamento = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoPagamento.obterOpcoesPesquisa(), def: EnumSituacaoPagamento.Todos, text: "Situação do Pagamento" });
    this.SituacaoIntegracao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoIntegracao.obterOpcoesPesquisa(), def: EnumSituacaoIntegracao.Todos, text: "Situação da Integração" });
    this.DataInicialEmissaoDocumento = PropertyEntity({ text: "Data Inicial Emissão Documento:", val: ko.observable(""), getType: typesKnockout.date, visible: ko.observable(true)});
    this.DataFinalEmissaoDocumento = PropertyEntity({ text: "Data Final Emissão Documento:", val: ko.observable(""), getType: typesKnockout.date, visible: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.SituacaoCarga = PropertyEntity({ val: ko.observable(""), options: EnumSituacoesCarga.obterOpcoesIntegracaoPesquisa(), def: EnumSituacoesCarga.Todos, text: "Situação da Carga" });
    this.ExibirUltimoRegistroQuandoExistirProtocoloCTeDuplicado = PropertyEntity({ text: "Exibir último registro quando existir protocolo do CTe duplicado", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
};

const CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridIntegracaoLotePagamento.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

const CRUDRelatorio = function () {
    this.GerarRelatorioIntegracaoLotePagamento = PropertyEntity({ eventClick: GerarRelatorioIntegracaoLotePagamentoClick, type: types.event, text: "Gerar Relatório Integração Lote Pagamento" });
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function LoadIntegracaoLotePagamento() {
    _pesquisaIntegracaoLotePagamento = new PesquisaIntegracaoLotePagamento();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridIntegracaoLotePagamento = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/IntegracaoLotePagamento/Pesquisa", _pesquisaIntegracaoLotePagamento);

    _gridIntegracaoLotePagamento.SetPermitirEdicaoColunas(true);
    _gridIntegracaoLotePagamento.SetQuantidadeLinhasPorPagina(10);

    _relatorioIntegracaoLotePagamento = new RelatorioGlobal("Relatorios/IntegracaoLotePagamento/BuscarDadosRelatorio", _gridIntegracaoLotePagamento, function () {
        _relatorioIntegracaoLotePagamento.loadRelatorio(function () {
            KoBindings(_pesquisaIntegracaoLotePagamento, "knockoutPesquisaIntegracaoLotePagamento", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaIntegracaoLotePagamento", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaIntegracaoLotePagamento", false);

            BuscarCargas(_pesquisaIntegracaoLotePagamento.Carga);
            BuscarCTes(_pesquisaIntegracaoLotePagamento.CTe);
            BuscarFilial(_pesquisaIntegracaoLotePagamento.Filial);
            BuscarTransportadores(_pesquisaIntegracaoLotePagamento.Transportador);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaIntegracaoLotePagamento);

}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioIntegracaoLotePagamento.gerarRelatorio("Relatorios/IntegracaoLotePagamento/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioIntegracaoLotePagamento.gerarRelatorio("Relatorios/IntegracaoLotePagamento/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}

function GerarRelatorioIntegracaoLotePagamentoClick(e, sender) {
    executarDownload("Relatorios/IntegracaoLotePagamento/GerarRelatorioIntegracaoLotePagamento", RetornarObjetoPesquisa(_pesquisaIntegracaoLotePagamento));
}

//*******MÉTODOS*******