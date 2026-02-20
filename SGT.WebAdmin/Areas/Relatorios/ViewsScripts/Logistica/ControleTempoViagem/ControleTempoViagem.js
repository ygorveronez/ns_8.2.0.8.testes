/// <reference path="../../../../../ViewsScripts/Consultas/Carga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Localidade.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/NotaFiscal.js" />
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
/// <reference path="../../../../../js/app.config.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridControleTempoViagem, _pesquisaControleTempoViagem, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioControleTempoViagem;

var PesquisaControleTempoViagem = function () {
    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Carga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true), multiplesEntitiesConfig: { propDescricao: "CodigoCargaEmbarcador" } });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Destino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Destino:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.NumeroNota = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Nº NF:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.TempoViagem = PropertyEntity({ text: "Tempo de Viagem: ", getType: typesKnockout.int });
    this.Performance = PropertyEntity({ text: "Performance: ", getType: typesKnockout.int });
    this.DiasRetornoComprovante = PropertyEntity({ text: "Dias Retorno Comprovante: ", getType: typesKnockout.int });
    this.DocumentoVenda = PropertyEntity({ text: "Documento Venda: ", getType: typesKnockout.string });
    this.RazaoSocialDestinatario = PropertyEntity({ text: "Razão Social Destinatário: ", getType: typesKnockout.string });
    this.ValorNotaInicial = PropertyEntity({ text: "Valor Nota Inicial: ", getType: typesKnockout.decimal });
    this.ValorNotaFinal = PropertyEntity({ text: "Valor Nota Final: ", getType: typesKnockout.decimal });

    this.DataFaturaInicial = PropertyEntity({ text: "Data Fatura Inicial: ", getType: typesKnockout.date });
    this.DataFaturaFinal = PropertyEntity({ text: "Data Fatura Final: ", getType: typesKnockout.date });
    this.DataFaturaInicial.dateRangeLimit = this.DataFaturaFinal;
    this.DataFaturaFinal.dateRangeInit = this.DataFaturaInicial;

    this.PrevisaoEntregaInicial = PropertyEntity({ text: "Previsão Entrega Inicial: ", getType: typesKnockout.date });
    this.PrevisaoEntregaFinal = PropertyEntity({ text: "Previsão Entrega Final: ", getType: typesKnockout.date });
    this.PrevisaoEntregaInicial.dateRangeLimit = this.PrevisaoEntregaFinal;
    this.PrevisaoEntregaFinal.dateRangeInit = this.PrevisaoEntregaInicial;

    this.DataEntregaRealInicial = PropertyEntity({ text: "Data Entrega Real Inicial: ", getType: typesKnockout.date });
    this.DataEntregaRealFinal = PropertyEntity({ text: "Data Entrega Real Final: ", getType: typesKnockout.date });
    this.DataEntregaRealInicial.dateRangeLimit = this.DataEntregaRealFinal;
    this.DataEntregaRealFinal.dateRangeInit = this.DataEntregaRealInicial;

    this.DataRetornoComprovanteInicial = PropertyEntity({ text: "Data Retorno Comprovante Inicial: ", getType: typesKnockout.date });
    this.DataRetornoComprovanteFinal = PropertyEntity({ text: "Data Retorno Comprovante Final: ", getType: typesKnockout.date });
    this.DataRetornoComprovanteInicial.dateRangeLimit = this.DataRetornoComprovanteFinal;
    this.DataRetornoComprovanteFinal.dateRangeInit = this.DataRetornoComprovanteInicial;
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridControleTempoViagem.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaControleTempoViagem.Visible.visibleFade()) {
                _pesquisaControleTempoViagem.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaControleTempoViagem.Visible.visibleFade(true);
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

function LoadControleTempoViagem() {
    _pesquisaControleTempoViagem = new PesquisaControleTempoViagem();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridControleTempoViagem = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ControleTempoViagem/Pesquisa", _pesquisaControleTempoViagem);

    _gridControleTempoViagem.SetPermitirEdicaoColunas(true);
    _gridControleTempoViagem.SetQuantidadeLinhasPorPagina(10);

    _relatorioControleTempoViagem = new RelatorioGlobal("Relatorios/ControleTempoViagem/BuscarDadosRelatorio", _gridControleTempoViagem, function () {
        _relatorioControleTempoViagem.loadRelatorio(function () {
            KoBindings(_pesquisaControleTempoViagem, "knockoutPesquisaControleTempoViagem", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaControleTempoViagem", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaControleTempoViagem", false);

            new BuscarCargas(_pesquisaControleTempoViagem.Carga);
            new BuscarTransportadores(_pesquisaControleTempoViagem.Transportador);
            new BuscarLocalidades(_pesquisaControleTempoViagem.Destino);
            new BuscarXMLNotaFiscal(_pesquisaControleTempoViagem.NumeroNota);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaControleTempoViagem);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioControleTempoViagem.gerarRelatorio("Relatorios/ControleTempoViagem/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioControleTempoViagem.gerarRelatorio("Relatorios/ControleTempoViagem/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}