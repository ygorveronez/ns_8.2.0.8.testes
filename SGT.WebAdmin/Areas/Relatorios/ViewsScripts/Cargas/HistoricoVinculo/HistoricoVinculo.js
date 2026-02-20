/// <reference path="../../../../../ViewsScripts/Consultas/Carga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Pedido.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumLocalVinculoHistorico.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
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
/// <reference path="../../../../../js/app.config.js" />
//*******MAPEAMENTO KNOUCKOUT*******

var _gridHistoricoVinculo, _pesquisaHistoricoVinculo, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioHistoricoVinculo;

var PesquisaHistoricoVinculo = function () {
    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
   
    this.DataInicial = PropertyEntity({ val: ko.observable(""), def: "", text: "Data Inicial:", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ val: ko.observable(""), def: "", text: "Data Final:", getType: typesKnockout.date });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Veiculo = PropertyEntity({ codEntity: ko.observable(0), type: types.entity, text: "Veículo:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Motorista = PropertyEntity({ codEntity: ko.observable(0), type: types.entity, text: "Motorista:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Pedido = PropertyEntity({ codEntity: ko.observable(0), type: types.entity, text: "Pedido:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Carga = PropertyEntity({ codEntity: ko.observable(0), type: types.entity, text: "Carga:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.FilaCarregamento = PropertyEntity({ codEntity: ko.observable(0), type: types.entity, text: "Fila de Carregamento:", idBtnSearch: guid(), enable: ko.observable(true) });

    this.LocalVinculoPesquisa = PropertyEntity({ val: ko.observable(EnumLocalVinculoHistorico.Todos), options: EnumLocalVinculoHistorico.obterOpcoesPesquisa(), def: EnumLocalVinculoHistorico.Todos, text: "Local de Vínculo: " });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridHistoricoVinculo.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function LoadHistoricoVinculo() {
    _pesquisaHistoricoVinculo = new PesquisaHistoricoVinculo();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridHistoricoVinculo = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/HistoricoVinculo/Pesquisa", _pesquisaHistoricoVinculo);

    _gridHistoricoVinculo.SetPermitirEdicaoColunas(true);
    _gridHistoricoVinculo.SetQuantidadeLinhasPorPagina(10);

    _relatorioHistoricoVinculo = new RelatorioGlobal("Relatorios/HistoricoVinculo/BuscarDadosRelatorio", _gridHistoricoVinculo, function () {
        _relatorioHistoricoVinculo.loadRelatorio(function () {
            KoBindings(_pesquisaHistoricoVinculo, "knockoutPesquisaHistoricoVinculo", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaHistoricoVinculo", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaHistoricoVinculo", false);

            new BuscarVeiculos(_pesquisaHistoricoVinculo.Veiculo);
            new BuscarMotoristas(_pesquisaHistoricoVinculo.Motorista);
            new BuscarPedidos(_pesquisaHistoricoVinculo.Pedido);
            new BuscarCargas(_pesquisaHistoricoVinculo.Carga);
            new BuscarFilaCarregamentoVeiculo(_pesquisaHistoricoVinculo.FilaCarregamento);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaHistoricoVinculo);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioHistoricoVinculo.gerarRelatorio("Relatorios/HistoricoVinculo/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioHistoricoVinculo.gerarRelatorio("Relatorios/HistoricoVinculo/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}