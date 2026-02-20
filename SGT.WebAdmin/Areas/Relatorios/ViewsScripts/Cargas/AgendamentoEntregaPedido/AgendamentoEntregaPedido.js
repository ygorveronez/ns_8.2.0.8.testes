/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoOperacao.js" />
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

var _gridAgendamentoEntregaPedido, _pesquisaAgendamentoEntregaPedido, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioAgendamentoEntregaPedido;

var _opcoesDataTerminoCarregamento = [
    { text: "Todos", value: "" },
    { text: "Sim", value: true },
    { text: "Não", value: false }
];

var _opcoesDataSugestaoEntrega = [
    { text: "Todos", value: "" },
    { text: "Sim", value: true },
    { text: "Não", value: false }
];

var PesquisaAgendamentoEntregaPedido = function () {
    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Carga = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), text: "Carga:" });
    this.TipoOperacao = PropertyEntity({ codEntity: ko.observable(0), type: types.entity, text: "Tipo de Operação:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Cliente = PropertyEntity({ codEntity: ko.observable(0), type: types.entity, text: "Cliente:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Transportador = PropertyEntity({ codEntity: ko.observable(0), type: types.entity, text: "Transportador:", idBtnSearch: guid(), enable: ko.observable(true) });

    this.DataCarregamentoInicial = PropertyEntity({ val: ko.observable(""), def: "", text: "Data Carregamento Inicial:", getType: typesKnockout.date });
    this.DataCarregamentoFinal = PropertyEntity({ val: ko.observable(""), def: "", text: "Data Carregamento Final:", getType: typesKnockout.date });
    this.DataCarregamentoInicial.dateRangeLimit = this.DataCarregamentoFinal;
    this.DataCarregamentoFinal.dateRangeInit = this.DataCarregamentoInicial;

    this.DataAgendamentoInicial = PropertyEntity({ val: ko.observable(""), def: "", text: "Data Agendamento Inicial:", getType: typesKnockout.date });
    this.DataAgendamentoFinal = PropertyEntity({ val: ko.observable(""), def: "", text: "Data Agendamento Final:", getType: typesKnockout.date });
    this.DataAgendamentoInicial.dateRangeLimit = this.DataAgendamentoFinal;
    this.DataAgendamentoFinal.dateRangeInit = this.DataAgendamentoInicial;

    this.DataPrevisaoEntregaInicial = PropertyEntity({ val: ko.observable(""), def: "", text: "Data Previsão Entrega Inicial:", getType: typesKnockout.date });
    this.DataPrevisaoEntregaFinal = PropertyEntity({ val: ko.observable(""), def: "", text: "Data Previsão Entrega Final:", getType: typesKnockout.date });
    this.DataPrevisaoEntregaInicial.dateRangeLimit = this.DataPrevisaoEntregaFinal;
    this.DataPrevisaoEntregaFinal.dateRangeInit = this.DataPrevisaoEntregaInicial;

    this.DataCriacaoPedidoInicial = PropertyEntity({ val: ko.observable(""), def: "", text: "Data Criação Pedido Inicial:", getType: typesKnockout.date });
    this.DataCriacaoPedidoFinal = PropertyEntity({ val: ko.observable(""), def: "", text: "Data Criação Pedido Final:", getType: typesKnockout.date });
    this.DataCriacaoPedidoInicial.dateRangeLimit = this.DataCriacaoPedidoFinal;
    this.DataCriacaoPedidoFinal.dateRangeInit = this.DataCriacaoPedidoInicial;

    this.DataInicialSugestaoEntrega = PropertyEntity({ val: ko.observable(""), def: "", text: "Data Suges. Ent. Inicial:", getType: typesKnockout.date });
    this.DataFinalSugestaoEntrega = PropertyEntity({ val: ko.observable(""), def: "", text: "Data Suges. Ent. Final:", getType: typesKnockout.date });
    this.DataInicialSugestaoEntrega.dateRangeLimit = this.DataFinalSugestaoEntrega;
    this.DataFinalSugestaoEntrega.dateRangeInit = this.DataInicialSugestaoEntrega;

    this.SituacaoAgendamento = PropertyEntity({ getType: typesKnockout.select, def: "", options: EnumSituacaoAgendamentoEntregaPedido.obterOpcoesPesquisa(), val: ko.observable(""), text: "Situação do Agendamento:" });

    this.PossuiDataTermioCarregamento = PropertyEntity({ text: "Data Término de Carregamento:", options: _opcoesDataTerminoCarregamento, val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.PossuiDataSugestaoEntrega = PropertyEntity({ text: "Data Sugestão de Entrega:", options: _opcoesDataSugestaoEntrega, val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.ExibirCargasAgrupadas = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: "Visualizar apenas cargas agrupadas", visible: ko.observable(false) });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridAgendamentoEntregaPedido.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaAgendamentoEntregaPedido.Visible.visibleFade()) {
                _pesquisaAgendamentoEntregaPedido.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaAgendamentoEntregaPedido.Visible.visibleFade(true);
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

function LoadAgendamentoEntregaPedido() {
    _pesquisaAgendamentoEntregaPedido = new PesquisaAgendamentoEntregaPedido();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridAgendamentoEntregaPedido = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/AgendamentoEntregaPedido/Pesquisa", _pesquisaAgendamentoEntregaPedido);

    _gridAgendamentoEntregaPedido.SetPermitirEdicaoColunas(true);
    _gridAgendamentoEntregaPedido.SetQuantidadeLinhasPorPagina(10);

    _relatorioAgendamentoEntregaPedido = new RelatorioGlobal("Relatorios/AgendamentoEntregaPedido/BuscarDadosRelatorio", _gridAgendamentoEntregaPedido, function () {
        _relatorioAgendamentoEntregaPedido.loadRelatorio(function () {
            KoBindings(_pesquisaAgendamentoEntregaPedido, "knockoutPesquisaAgendamentoEntregaPedido", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaAgendamentoEntregaPedido", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaAgendamentoEntregaPedido", false);

            new BuscarTiposOperacao(_pesquisaAgendamentoEntregaPedido.TipoOperacao);
            new BuscarClientes(_pesquisaAgendamentoEntregaPedido.Cliente);
            new BuscarTransportadores(_pesquisaAgendamentoEntregaPedido.Transportador);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) 
                _pesquisaAgendamentoEntregaPedido.ExibirCargasAgrupadas.visible(true);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaAgendamentoEntregaPedido);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioAgendamentoEntregaPedido.gerarRelatorio("Relatorios/AgendamentoEntregaPedido/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioAgendamentoEntregaPedido.gerarRelatorio("Relatorios/AgendamentoEntregaPedido/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}