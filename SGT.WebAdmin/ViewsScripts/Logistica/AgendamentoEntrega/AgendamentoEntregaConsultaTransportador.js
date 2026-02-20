var _agendamentoConsultaTransportador;
var _gridAgendamentoConsultaTransportador

var AgendamentoConsultaTransportador = function () {
    this.CodigoPedido = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });
    this.CodigoCargaEntrega = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });
    this.GridConsultas = PropertyEntity({ idGrid: guid() });

    this.Pesquisar = PropertyEntity({ eventClick: pesquisarAgendamentoEntregaPedidoConsultaTransportador, type: types.event, text: "Pesquisar", visible: ko.observable(true) });
}

function loadAgendamentoConsultaTransportador() {
    _agendamentoConsultaTransportador = new AgendamentoConsultaTransportador();
    KoBindings(_agendamentoConsultaTransportador, "knockoutConsultaTransportador");

    _gridAgendamentoConsultaTransportador = new GridView(_agendamentoConsultaTransportador.GridConsultas.idGrid, "AgendamentoEntregaPedidoConsulta/PesquisaAuditoria", _agendamentoConsultaTransportador, null, null, 10);
}

function exibirModalAgendamentoConsultaTransportador(registroSelecionado) {
    _agendamentoConsultaTransportador.CodigoPedido.val(registroSelecionado.CodigoPedido);
    _agendamentoConsultaTransportador.CodigoCargaEntrega.val(registroSelecionado?.CodigoCargaEntrega ?? 0);
    _gridAgendamentoConsultaTransportador.CarregarGrid();
    
    $("#divModalConsultaTransportador")
        .modal("show").on("show.bs.modal", function () {
        }).on("hidden.bs.modal", function () {
            LimparCampos(_agendamentoConsultaTransportador);
        });
}

function pesquisarAgendamentoEntregaPedidoConsultaTransportador() {
    _gridAgendamentoConsultaTransportador.CarregarGrid();
}