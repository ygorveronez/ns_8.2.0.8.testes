//#region Referências

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
/// <reference path="../../Relatorios/Global/Relatorio.js" />

//#endregion

var _pesquisaCargaComInteresseTransportadorTerceiro;
var _relatorioCargaComInteresseTransportadorTerceiro;
var _gridCargaComInteresseTransportadorTerceiro;
var _CRUDFiltrosRelatorio;
var _CRUDRelatorio;


var PesquisaCargaComInteresseTransportadorTerceiro = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoCargaJanelaCarregamentoTransportador.Todas), options: EnumSituacaoCargaJanelaCarregamentoTransportador.obterOpcoesPesquisa(), def: EnumSituacaoCargaJanelaCarregamentoTransportador.Todas, text: "Situação: " });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Carregamento:", idBtnSearch: guid(), val: ko.observable("") });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), val: ko.observable("") });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), val: ko.observable("") });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridCargaComInteresseTransportadorTerceiro.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorioCargaComInteresseTransportadorTerceiro", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaCargaComInteresseTransportadorTerceiro.Visible.visibleFade()) {
                _pesquisaCargaComInteresseTransportadorTerceiro.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaCargaComInteresseTransportadorTerceiro.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

function LoadCargasComInteresseTransportadorTerceiro() {
    _pesquisaCargaComInteresseTransportadorTerceiro = new PesquisaCargaComInteresseTransportadorTerceiro();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    
    _gridCargaComInteresseTransportadorTerceiro = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/CargasComInteresseTransportadorTerceiro/Pesquisa", _pesquisaCargaComInteresseTransportadorTerceiro, null, null, 10);
    _gridCargaComInteresseTransportadorTerceiro.SetPermitirEdicaoColunas(true);

    _relatorioCargaComInteresseTransportadorTerceiro = new RelatorioGlobal("Relatorios/CargasComInteresseTransportadorTerceiro/BuscarDadosRelatorio", _gridCargaComInteresseTransportadorTerceiro, function () {
        _relatorioCargaComInteresseTransportadorTerceiro.loadRelatorio(function () {
            KoBindings(_pesquisaCargaComInteresseTransportadorTerceiro, "knockoutPesquisaCargaComInteresseTransportadorTerceiro");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaCargaComInteresseTransportadorTerceiro");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaCargaComInteresseTransportadorTerceiro");

            new BuscarVeiculos(_pesquisaCargaComInteresseTransportadorTerceiro.Veiculo);
            new BuscarCargas(_pesquisaCargaComInteresseTransportadorTerceiro.Carga);
            new BuscarCentrosCarregamento(_pesquisaCargaComInteresseTransportadorTerceiro.CentroCarregamento);
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaCargaComInteresseTransportadorTerceiro);
    
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioCargaComInteresseTransportadorTerceiro.gerarRelatorio("Relatorios/CargasComInteresseTransportadorTerceiro/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioCargaComInteresseTransportadorTerceiro.gerarRelatorio("Relatorios/CargasComInteresseTransportadorTerceiro/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
