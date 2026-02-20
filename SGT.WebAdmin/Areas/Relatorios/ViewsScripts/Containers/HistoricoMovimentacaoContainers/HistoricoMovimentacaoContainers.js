/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Pedido.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Carga.js" />
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

var _gridHistoricoMovimentacaoContainers, _pesquisaHistoricoMovimentacaoContainers, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioHistoricoMovimentacaoContainers;

var PesquisaHistoricoMovimentacaoContainers = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.Carga = PropertyEntity({ text: "Carga:", val: ko.observable(""), visible: ko.observable(true) });
    this.Container = PropertyEntity({ text: "Container:", val: ko.observable(""), visible: ko.observable(true) });
    this.SituacaoContainer = PropertyEntity({ text: "Situação Container:", val: ko.observable(EnumStatusColetaContainer.Todas), options: EnumStatusColetaContainer.obterOpcoesPesquisa(), def: EnumStatusColetaContainer.Todas });
    this.TipoContainer = PropertyEntity({ text: "Tipo Container:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.LocalEsperaVazio = PropertyEntity({ text: "Local de Espera Vazio:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Filial = PropertyEntity({ text: "Filial:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataInicialColeta = PropertyEntity({ text: "Data inicial Coleta:", getType: typesKnockout.date, visible: ko.observable(true), def: "", val: ko.observable("") });
    this.DataFinalColeta = PropertyEntity({ text: "Data final Coleta:", getType: typesKnockout.date, visible: ko.observable(true), def: Global.DataHoraAtual(), val: ko.observable(Global.DataHoraAtual()) });
    this.DiasPosseInicial = PropertyEntity({ text: "Dias em Posse Inicial:", val: ko.observable(""), visible: ko.observable(true) });
    this.DiasPosseFinal = PropertyEntity({ text: "Dias em Posse Final:", val: ko.observable(""), visible: ko.observable(true) });
    this.NumeroPedido = PropertyEntity({ text: "Número Pedido:", val: ko.observable(""), visible: ko.observable(true) });
    this.NumeroBooking = PropertyEntity({ text: "Número Booking:", val: ko.observable(""), visible: ko.observable(true) });
    this.NumeroEXP = PropertyEntity({ text: "Número EXP:", val: ko.observable(""), visible: ko.observable(true) });
    this.DataMovimentacao = PropertyEntity({ text: "Data movimentação:", val: ko.observable(""), def: "", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataPorto = PropertyEntity({ text: "Data porto:", val: ko.observable(""), def: "", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.LocalColeta = PropertyEntity({ text: "Local de Coleta:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.LocalAtual = PropertyEntity({ text: "Local Atual:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });


    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridHistoricoMovimentacaoContainers.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaHistoricoMovimentacaoContainers.Visible.visibleFade() == true) {
                _pesquisaHistoricoMovimentacaoContainers.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaHistoricoMovimentacaoContainers.Visible.visibleFade(true);
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

function LoadHistoricoMovimentacaoContainers() {
    _pesquisaHistoricoMovimentacaoContainers = new PesquisaHistoricoMovimentacaoContainers();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridHistoricoMovimentacaoContainers = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/HistoricoMovimentacaoContainers/Pesquisa", _pesquisaHistoricoMovimentacaoContainers);

    _gridHistoricoMovimentacaoContainers.SetPermitirEdicaoColunas(true);
    _gridHistoricoMovimentacaoContainers.SetQuantidadeLinhasPorPagina(20);

    _relatorioHistoricoMovimentacaoContainers = new RelatorioGlobal("Relatorios/HistoricoMovimentacaoContainers/BuscarDadosRelatorio", _gridHistoricoMovimentacaoContainers, function () {
        _relatorioHistoricoMovimentacaoContainers.loadRelatorio(function () {
            KoBindings(_pesquisaHistoricoMovimentacaoContainers, "knockoutPesquisaHistoricoMovimentacaoContainers", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaHistoricoMovimentacaoContainers", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaHistoricoMovimentacaoContainers", false);

            new BuscarFilial(_pesquisaHistoricoMovimentacaoContainers.Filial);
            new BuscarTiposContainer(_pesquisaHistoricoMovimentacaoContainers.TipoContainer);
            new BuscarClientes(_pesquisaHistoricoMovimentacaoContainers.LocalEsperaVazio);
            new BuscarClientes(_pesquisaHistoricoMovimentacaoContainers.LocalColeta);
            new BuscarClientes(_pesquisaHistoricoMovimentacaoContainers.LocalAtual);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaHistoricoMovimentacaoContainers);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioHistoricoMovimentacaoContainers.gerarRelatorio("Relatorios/HistoricoMovimentacaoContainers/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioHistoricoMovimentacaoContainers.gerarRelatorio("Relatorios/HistoricoMovimentacaoContainers/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
