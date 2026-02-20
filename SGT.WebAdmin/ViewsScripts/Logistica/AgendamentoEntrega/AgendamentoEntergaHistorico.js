var _agendamentoAgendamentoEntregaHistorico;
var _gridAgendamentoAgendamentoEntregaHistorico

var AgendamentoAgendamentoEntregaHistorico = function () {
    this.CodigoPedido = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });
    this.CodigoCargaEntrega = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });
    this.GridConsultas = PropertyEntity({ idGrid: guid() });

    this.Pesquisar = PropertyEntity({ eventClick: pesquisarAgendamentoEntregaPedidoAgendamentoEntregaHistorico, type: types.event, text: "Pesquisar", visible: ko.observable(true) });
}

function loadAgendamentoAgendamentoEntregaHistorico() {
    _agendamentoAgendamentoEntregaHistorico = new AgendamentoAgendamentoEntregaHistorico();
    KoBindings(_agendamentoAgendamentoEntregaHistorico, "knockoutAgendamentoEntregaHistorico");

    _gridAgendamentoAgendamentoEntregaHistorico = new GridView(_agendamentoAgendamentoEntregaHistorico.GridConsultas.idGrid, "AgendamentoEntregapedido/BuscarHistoricoAgendamentoEntregaPedido", _agendamentoAgendamentoEntregaHistorico, null, null, 10);
}

function exibirModalAgendamentoAgendamentoEntregaHistorico(registroSelecionado) {
    _agendamentoAgendamentoEntregaHistorico.CodigoPedido.val(registroSelecionado.CodigoPedido);
    _agendamentoAgendamentoEntregaHistorico.CodigoCargaEntrega.val(registroSelecionado?.CodigoCargaEntrega ?? 0);
    _gridAgendamentoAgendamentoEntregaHistorico.CarregarGrid();
    
    $("#divModalAgendamentoEntregaHistorico")
        .modal("show").on("show.bs.modal", function () {
        }).on("hidden.bs.modal", function () {
            LimparCampos(_agendamentoAgendamentoEntregaHistorico);
        });
}

function pesquisarAgendamentoEntregaPedidoAgendamentoEntregaHistorico() {
    _gridAgendamentoAgendamentoEntregaHistorico.CarregarGrid();
}