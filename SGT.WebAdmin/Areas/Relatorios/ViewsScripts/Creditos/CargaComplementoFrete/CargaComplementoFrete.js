/// <reference path="../../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../../../js/Global/Rest.js" />
/// <reference path="../../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../../js/Global/Grid.js" />
/// <reference path="../../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Localidade.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Carga.js" />
/// <reference path="../../../../../js/app.config.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Trasportador.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CentroCarregamento.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoComplementoFrete.js" />
//*******MAPEAMENTO KNOUCKOUT*******

var _gridCargaComplementoFretes, _pesquisaCargaComplementoFretes, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioCargaComplementoFretes;

var _situacaoPesquisa = [
    { text: "Todas", value: EnumSituacaoComplementoFrete.Todas },
    { text: "Utilizado", value: EnumSituacaoComplementoFrete.Utilizada },
    { text: "Ag. Aprovação", value: EnumSituacaoComplementoFrete.AgAprovacao },
    { text: "Ag. Confirmação", value: EnumSituacaoComplementoFrete.AgConfirmacaoUso },
    { text: "Estornado", value: EnumSituacaoComplementoFrete.Estornada },
    { text: "Rejeitado", value: EnumSituacaoComplementoFrete.Rejeitada }
];

var PesquisaCargaComplementoFretes = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.DataInicio = PropertyEntity({ text: "Data Início: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFim = PropertyEntity({ text: "Data Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ text: "Transportador:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), issue: 63, visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), issue: 52, visible: ko.observable(true) });
    this.Operador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.SituacaoComplementoFrete = PropertyEntity({ val: ko.observable(EnumSituacaoComplementoFrete.Utilizada), options: _situacaoPesquisa, def: EnumSituacaoComplementoFrete.Utilizada, text: "*Situação: ", required: false });
    this.MotivoAdicionalFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motivo:", idBtnSearch: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridCargaComplementoFretes.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function loadRelatorioCargaComplementoFretes() {
    _pesquisaCargaComplementoFretes = new PesquisaCargaComplementoFretes();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridCargaComplementoFretes = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/CargaComplementosFrete/Pesquisa", _pesquisaCargaComplementoFretes, null);

    _gridCargaComplementoFretes.SetPermitirEdicaoColunas(true);
    _gridCargaComplementoFretes.SetQuantidadeLinhasPorPagina(10);

    _relatorioCargaComplementoFretes = new RelatorioGlobal("Relatorios/CargaComplementosFrete/BuscarDadosRelatorio", _gridCargaComplementoFretes, function () {
        _relatorioCargaComplementoFretes.loadRelatorio(function () {
            KoBindings(_pesquisaCargaComplementoFretes, "knockoutPesquisaCargaComplementoFretes", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaCargaComplementoFretes", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaCargaComplementoFrete", false);

            new BuscarMotivoAdicionalFrete(_pesquisaCargaComplementoFretes.MotivoAdicionalFrete);
            new BuscarOperador(_pesquisaCargaComplementoFretes.Operador, null, true);
            new BuscarVeiculos(_pesquisaCargaComplementoFretes.Veiculo);
            new BuscarTransportadores(_pesquisaCargaComplementoFretes.Transportador);
            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaCargaComplementoFretes);

}

function baixarDacteClick(e) {
    var data = { CodigoCargaComplementoFrete: e.Codigo };
    executarDownload("Relatorios/CargaComplementoFretes/DownloadDacte", data);
}

function baixarXMLCargaComplementoFreteClick(e) {
    var data = { CodigoCargaComplementoFrete: e.Codigo };
    executarDownload("Relatorios/CargaComplementoFretes/DownloadXML", data);
}


function GerarRelatorioPDFClick(e, sender) {
    _relatorioCargaComplementoFretes.gerarRelatorio("Relatorios/CargaComplementosFrete/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioCargaComplementoFretes.gerarRelatorio("Relatorios/CargaComplementosFrete/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}