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
/// <reference path="../../../../../ViewsScripts/Consultas/Localidade.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Produto.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoColaborador.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Licenca.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTempoDeViagem, _pesquisaTempoDeViagem, _CRUDRelatorio, _relatorioTempoDeViagem, _CRUDFiltrosRelatorio;

var _situacaoPesquisa = [{ text: "Todos", value: 0 },
{ text: "Em Andamento", value: 1 },
{ text: "Fechados", value: 2 },
{ text: "Cancelados", value: 3 }];

var PesquisaTempoDeViagem = function () {
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(0), options: _situacaoPesquisa, def: 0, text: "Situação: " });
    
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual(), required: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Data Final:: ", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual(), required: ko.observable(true) });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridTempoDeViagem.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function loadRelatorioTempoDeViagem() {

    _pesquisaTempoDeViagem = new PesquisaTempoDeViagem();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridTempoDeViagem = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "/Relatorios/TempoDeViagem/Pesquisa", _pesquisaTempoDeViagem, null, null, 10);
    _gridTempoDeViagem.SetPermitirEdicaoColunas(true);

    _relatorioTempoDeViagem = new RelatorioGlobal("Relatorios/TempoDeViagem/BuscarDadosRelatorio", _gridTempoDeViagem, function () {
        _relatorioTempoDeViagem.loadRelatorio(function () {
            KoBindings(_pesquisaTempoDeViagem, "knockoutPesquisaTempoDeViagem");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaTempoDeViagem");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaTempoDeViagem");

            new BuscarMotoristas(_pesquisaTempoDeViagem.Motorista);
            new BuscarVeiculos(_pesquisaTempoDeViagem.Veiculo);
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaTempoDeViagem);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioTempoDeViagem.gerarRelatorio("Relatorios/TempoDeViagem/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioTempoDeViagem.gerarRelatorio("Relatorios/TempoDeViagem/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
