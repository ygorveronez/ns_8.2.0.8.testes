/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAgendamentoEntregaPedido.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridConsultaPorNotaFiscal, _pesquisaConsultaPorNotaFiscal, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaConsultaPorNotaFiscal = function () {
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operação:", idBtnSearch: guid() });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });
    this.NumeroCarga = PropertyEntity({ type: types.string, val: ko.observable(""), text: "Numero da Carga:" });
    this.NumeroNota = PropertyEntity({ type: types.int, val: ko.observable(""), text: "Numero da Nota:" });
    this.DataPrevisaoEntregaInicial = PropertyEntity({ text: "Data Previsão Entrega Inicial: ", getType: typesKnockout.date });
    this.DataPrevisaoEntregaFinal = PropertyEntity({ text: "Data Previsão Entrega Final: ", getType: typesKnockout.date });
    this.DataCarregamentoInicial = PropertyEntity({ text: "Data Carregamento Inicial: ", getType: typesKnockout.date });
    this.DataCarregamentoFinal = PropertyEntity({ text: "Data Carregamento Final: ", getType: typesKnockout.date });
    this.DataAgendamentoInicial = PropertyEntity({ text: "Data Agendamento Inicial: ", getType: typesKnockout.date });
    this.DataAgendamentoFinal = PropertyEntity({ text: "Data Agendamento Final: ", getType: typesKnockout.date });
    this.SituacaoAgendamento = PropertyEntity({ val: ko.observable(EnumSituacaoAgendamentoEntregaPedido.Todas), options: EnumSituacaoAgendamentoEntregaPedido.obterOpcoesPesquisa(), text: "Status: ", def: EnumSituacaoAgendamentoEntregaPedido.Todas });

    this.DataAgendamentoInicial.dateRangeLimit = this.DataAgendamentoFinal;
    this.DataAgendamentoFinal.dateRangeInit = this.DataAgendamentoInicial;

    this.DataCarregamentoInicial.dateRangeLimit = this.DataCarregamentoFinal;
    this.DataCarregamentoFinal.dateRangeInit = this.DataCarregamentoInicial;

    this.DataPrevisaoEntregaInicial.dateRangeLimit = this.DataPrevisaoEntregaFinal;
    this.DataPrevisaoEntregaFinal.dateRangeInit = this.DataPrevisaoEntregaInicial;
    
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridConsultaPorNotaFiscal.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: "grid-consulta-por-nota-fiscal", visible: ko.observable(true)
    });
    
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

//*******EVENTOS*******

function loadConsultaPorNotaFiscal() {
    _pesquisaConsultaPorNotaFiscal = new PesquisaConsultaPorNotaFiscal();
    KoBindings(_pesquisaConsultaPorNotaFiscal, "knockoutPesquisaConsultaPorNotaFiscal", false, _pesquisaConsultaPorNotaFiscal.Pesquisar.idGrid);

    loadGridConsultaPorNotaFiscal();

    new BuscarTransportadores(_pesquisaConsultaPorNotaFiscal.Transportador);
    new BuscarTiposOperacao(_pesquisaConsultaPorNotaFiscal.TipoOperacao);
    new BuscarClientes(_pesquisaConsultaPorNotaFiscal.Cliente);
}

function loadGridConsultaPorNotaFiscal() {
    var draggableRows = false;
    var draggableRows = false;
    var limiteRegistros = 50;
    var totalRegistrosPorPagina = 10;

    var configuracaoExportacao = {
        url: "ConsultaPorNotaFiscal/ExportarPesquisa",
        titulo: "Consulta por Nota Fiscal"
    };
    
    _gridConsultaPorNotaFiscal = new GridView(_pesquisaConsultaPorNotaFiscal.Pesquisar.idGrid, "ConsultaPorNotaFiscal/Pesquisa", _pesquisaConsultaPorNotaFiscal, null, null, totalRegistrosPorPagina, null, true, draggableRows, undefined, limiteRegistros, null, configuracaoExportacao);
    _gridConsultaPorNotaFiscal.SetPermitirEdicaoColunas(true);
    _gridConsultaPorNotaFiscal.SetSalvarPreferenciasGrid(true);
    _gridConsultaPorNotaFiscal.SetHabilitarScrollHorizontal(true, 100);
    _gridConsultaPorNotaFiscal.CarregarGrid();
}