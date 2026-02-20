/// <reference path="../../../../../ViewsScripts/Logistica/MonitoramentoHistorico/MonitoramentoHistorico.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Localidade.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Carga.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/Rest.js" />
/// <reference path="../../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../../js/Global/Grid.js" />
/// <reference path="../../../../../js/Global/MapaDraw.js" />
/// <reference path="../../../../../js/Global/MapaGoogle.js" />
/// <reference path="../../../../../js/Global/Mapa.js"/>
/// <reference path="../../../../../js/Global/MapaGoogleSearch.js"/>
/// <reference path="../../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../../js/app.config.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCargaViagemEventos, _pesquisaCargaViagemEventos, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioCargaViagemEventos;


var PesquisaCargaViagemEventos = function () {
    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.CodigoCargaEmbarcador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), issue: 195, visible: ko.observable(true) });
    this.CodigoFilial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.CodigoLocalidadeOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Origem:", issue: 16, idBtnSearch: guid(), visible: ko.observable(true) });
    this.CodigoLocalidadeDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destino:", issue: 16, idBtnSearch: guid(), visible: ko.observable(true) });
    this.CodigoClienteOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente Origem:", issue: 52, idBtnSearch: guid(), visible: ko.observable(true) });
    this.CodigoClienteDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente Destino:", issue: 52, idBtnSearch: guid(), visible: ko.observable(true) });

};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridCargaViagemEventos.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function LoadCargaViagemEventos() {

    _pesquisaCargaViagemEventos = new PesquisaCargaViagemEventos();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    var opcaoHistoricos = { descricao: "Historicos", id: guid(), evento: "onclick", metodo: visualizarHistoricosClick, tamanho: "8", icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", opcoes: [opcaoHistoricos], tamanho: 1, };

    _gridCargaViagemEventos = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/CargaViagemEventos/Pesquisa", _pesquisaCargaViagemEventos);

    _gridCargaViagemEventos.SetPermitirEdicaoColunas(true);
    _gridCargaViagemEventos.SetQuantidadeLinhasPorPagina(10);

    _relatorioCargaViagemEventos = new RelatorioGlobal("Relatorios/CargaViagemEventos/BuscarDadosRelatorio", _gridCargaViagemEventos, function () {
        _relatorioCargaViagemEventos.loadRelatorio(function () {
            KoBindings(_pesquisaCargaViagemEventos, "knockoutCargaViagemEventos", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaCargaViagemEventos", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaCargaViagemEventos", false);
            new BuscarCargas(_pesquisaCargaViagemEventos.CodigoCargaEmbarcador);
            new BuscarLocalidadesBrasil(_pesquisaCargaViagemEventos.CodigoLocalidadeOrigem, "Buscar Origem");
            new BuscarLocalidadesBrasil(_pesquisaCargaViagemEventos.CodigoLocalidadeDestino, "Buscar Destino");
            new BuscarClientes(_pesquisaCargaViagemEventos.CodigoClienteOrigem);
            new BuscarClientes(_pesquisaCargaViagemEventos.CodigoClienteDestino);
            new BuscarFilial(_pesquisaCargaViagemEventos.CodigoFilial);
            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaCargaViagemEventos);

    
    
}

function visualizarHistoricosClick(filaSelecionada) {
    exibirHistoricoMonitoramentoPorCodigo(filaSelecionada.Codigo);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioCargaViagemEventos.gerarRelatorio("Relatorios/CargaViagemEventos/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioCargaViagemEventos.gerarRelatorio("Relatorios/CargaViagemEventos/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}


//*******MÉTODOS*******