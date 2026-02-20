/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Estado.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAgendamentoEntregaPedido.js" />

var _pesquisaAgendamentoEntregaPedidoConsulta;
var _gridAgendamentoEntregaPedidoConsulta;

var PesquisaAgendamentoEntregaPedidoConsulta = function () {
    this.NotaFiscal = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(""), configInt: { precision: 0, allowZero: false, thousands: "" }, text: "NF-e:" });
    this.CTe = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(""), configInt: { precision: 0, allowZero: false, thousands: "" }, text: "CT-e:" });
    this.TipoOperacao = PropertyEntity({ codEntity: ko.observable(0), required: false, type: types.entity, text: "Tipo de Operação:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Cliente = PropertyEntity({ codEntity: ko.observable(0), required: false, type: types.entity, text: "Cliente:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Destino = PropertyEntity({ codEntity: ko.observable(0), required: false, type: types.entity, text: "Destino:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Estado = PropertyEntity({ codEntity: ko.observable(0), required: false, type: types.entity, text: "Estado:", idBtnSearch: guid(), enable: ko.observable(true) });

    this.DataAgendamentoInicial = PropertyEntity({ val: ko.observable(""), def: "", text: "Data Agendamento Inicial:", getType: typesKnockout.date });
    this.DataAgendamentoFinal = PropertyEntity({ val: ko.observable(""), def: "", text: "Data Agendamento Final:", getType: typesKnockout.date });

    this.DataAgendamentoInicial.dateRangeLimit = this.DataAgendamentoFinal;
    this.DataAgendamentoFinal.dateRangeInit = this.DataAgendamentoInicial;
    
    this.Situacao = PropertyEntity({ getType: typesKnockout.options, val: ko.observable(""), options: EnumSituacaoAgendamentoEntregaPedido.obterOpcoesPesquisa(), text: "Situação do Agendamento:" });
    
    this.Pesquisar = PropertyEntity({ eventClick: pesquisarAgendamentoEntregaPedidoConsulta, type: types.event, text: "Pesquisar", visible: ko.observable(true) });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
}

var AgendamentoEntregaPedidoConsulta = function () {
    this.Grid = PropertyEntity({ idGrid: "grid-agendamento-entrega-pedido-consulta" });
}

function loadAgendamentoEntregaPedidoConsulta() {
    _pesquisaAgendamentoEntregaPedidoConsulta = new PesquisaAgendamentoEntregaPedidoConsulta();
    KoBindings(_pesquisaAgendamentoEntregaPedidoConsulta, "knockoutPesquisaAgendamentoEntregaPedidoConsulta");

    _agendamentoEntregaPedidoConsulta = new AgendamentoEntregaPedidoConsulta();
    KoBindings(_agendamentoEntregaPedidoConsulta, "knockoutAgendamentoEntregaPedidoConsulta");

    new BuscarTiposOperacao(_pesquisaAgendamentoEntregaPedidoConsulta.TipoOperacao);
    new BuscarClientes(_pesquisaAgendamentoEntregaPedidoConsulta.Cliente);
    new BuscarLocalidades(_pesquisaAgendamentoEntregaPedidoConsulta.Destino);
    new BuscarEstados(_pesquisaAgendamentoEntregaPedidoConsulta.Estado);

    loadGridAgendamentoEntregaPedidoConsulta();
}

function loadGridAgendamentoEntregaPedidoConsulta() {
    var configExportacao = {
        url: "AgendamentoEntregaPedidoConsulta/ExportarPesquisa",
        titulo: "Agendamento Entrega Pedido"
    };
    
    _gridAgendamentoEntregaPedidoConsulta = new GridViewExportacao(_agendamentoEntregaPedidoConsulta.Grid.idGrid, "AgendamentoEntregaPedidoConsulta/Pesquisa", _pesquisaAgendamentoEntregaPedidoConsulta, null, configExportacao, null, 10);
    _gridAgendamentoEntregaPedidoConsulta.SetPermitirEdicaoColunas(true);
    _gridAgendamentoEntregaPedidoConsulta.SetSalvarPreferenciasGrid(true);

    _gridAgendamentoEntregaPedidoConsulta.CarregarGrid();
}

function pesquisarAgendamentoEntregaPedidoConsulta() {
    _gridAgendamentoEntregaPedidoConsulta.CarregarGrid();
}

function exibirFiltrosClick() {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}