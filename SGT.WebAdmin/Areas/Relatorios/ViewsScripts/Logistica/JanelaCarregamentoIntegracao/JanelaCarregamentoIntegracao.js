/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Localidade.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoPessoa.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../js/Global/knockout-3.1.0.js" />
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

var _gridJanelaCarregamentoIntegracao, _pesquisaJanelaCarregamentoIntegracao, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioJanelaCarregamentoIntegracao;

var PesquisaJanelaCarregamentoIntegracao = function () {

    var dataAtual = moment().format("DD/MM/YYYY");

    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.Carga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Carga:", issue: 70, idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: "Data inicial:", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Data final:", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true) });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridJanelaCarregamentoIntegracao.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaJanelaCarregamentoIntegracao.Visible.visibleFade() == true) {
                _pesquisaJanelaCarregamentoIntegracao.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaJanelaCarregamentoIntegracao.Visible.visibleFade(true);
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

function LoadJanelaCarregamentoIntegracao() {
    _pesquisaJanelaCarregamentoIntegracao = new PesquisaJanelaCarregamentoIntegracao();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridJanelaCarregamentoIntegracao = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/JanelaCarregamentoIntegracao/Pesquisa", _pesquisaJanelaCarregamentoIntegracao);

    _gridJanelaCarregamentoIntegracao.SetPermitirEdicaoColunas(true);
    _gridJanelaCarregamentoIntegracao.SetQuantidadeLinhasPorPagina(20);

    _relatorioJanelaCarregamentoIntegracao = new RelatorioGlobal("Relatorios/JanelaCarregamentoIntegracao/BuscarDadosRelatorio", _gridJanelaCarregamentoIntegracao, function () {
        _relatorioJanelaCarregamentoIntegracao.loadRelatorio(function () {
            KoBindings(_pesquisaJanelaCarregamentoIntegracao, "knockoutPesquisaJanelaCarregamentoIntegracao", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaJanelaCarregamentoIntegracao", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaJanelaCarregamentoIntegracao", false);

            new BuscarCargas(_pesquisaJanelaCarregamentoIntegracao.Carga)

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaJanelaCarregamentoIntegracao);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioJanelaCarregamentoIntegracao.gerarRelatorio("Relatorios/JanelaCarregamentoIntegracao/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioJanelaCarregamentoIntegracao.gerarRelatorio("Relatorios/JanelaCarregamentoIntegracao/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
