/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../viewsscripts/consultas/tipobonificacao.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Justificativa.js" />
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

var _gridBonificacaoAcertoViagem, _pesquisaBonificacaoAcertoViagem, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioBonificacaoAcertoViagem;

var PesquisaBonificacaoAcertoViagem = function () {
    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });    

    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: "Motorista:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.NumeroAcerto = PropertyEntity({ text: "Nº Acerto: ", });
    this.DataInicialAcerto = PropertyEntity({ text: "Data Ini. Acerto: ", getType: typesKnockout.date });
    this.DataFinalAcerto = PropertyEntity({ text: "Data Fin. Acerto: ", getType: typesKnockout.date });
    this.DataInicialBonificacao = PropertyEntity({ text: "Data Inicial Bonificação: ", getType: typesKnockout.date });
    this.DataFinalBonificacao = PropertyEntity({ text: "Data Final Bonificação: ", getType: typesKnockout.date });

    this.DataInicialAcerto.dateRangeLimit = this.DataFinalAcerto;
    this.DataFinalAcerto.dateRangeInit = this.DataInicialAcerto;
    this.DataInicialBonificacao.dateRangeLimit = this.DataFinalBonificacao;
    this.DataFinalBonificacao.dateRangeInit = this.DataInicialBonificacao;

    this.TipoBonificacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: "Justificativa da Bonificação:", idBtnSearch: guid(), visible: ko.observable(true) });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridBonificacaoAcertoViagem.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function LoadBonificacaoAcertoViagem() {
    _pesquisaBonificacaoAcertoViagem = new PesquisaBonificacaoAcertoViagem();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridBonificacaoAcertoViagem = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/BonificacaoAcertoViagem/Pesquisa", _pesquisaBonificacaoAcertoViagem);

    _gridBonificacaoAcertoViagem.SetPermitirEdicaoColunas(true);
    _gridBonificacaoAcertoViagem.SetQuantidadeLinhasPorPagina(10);

    _relatorioBonificacaoAcertoViagem = new RelatorioGlobal("Relatorios/BonificacaoAcertoViagem/BuscarDadosRelatorio", _gridBonificacaoAcertoViagem, function () {
        _relatorioBonificacaoAcertoViagem.loadRelatorio(function () {

            KoBindings(_pesquisaBonificacaoAcertoViagem, "knockoutPesquisaBonificacaoAcertoViagem", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaBonificacaoAcertoViagem", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaBonificacaoAcertoViagem", false);

            new BuscarJustificativas(_pesquisaBonificacaoAcertoViagem.TipoBonificacao, null, EnumTipoJustificativa.Acrescimo, [EnumTipoFinalidadeJustificativa.Todas, EnumTipoFinalidadeJustificativa.AcertoViagemMotorista]);
            new BuscarMotoristas(_pesquisaBonificacaoAcertoViagem.Motorista);
            

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaBonificacaoAcertoViagem);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioBonificacaoAcertoViagem.gerarRelatorio("Relatorios/BonificacaoAcertoViagem/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioBonificacaoAcertoViagem.gerarRelatorio("Relatorios/BonificacaoAcertoViagem/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}