var _agendamentoEntregaLegendas;
var _agendamentoRegraCadastroClienteLegendas;

var AgendamentoEntregaLegendas = function () {
    this.ExibirLegendas = PropertyEntity({ eventClick: exibirLegendasClick, type: types.event, text: "Legendas", idFade: guid(), visible: ko.observable(true), visibleFade: ko.observable(false) });

    this.AguardandoTerminoCarregamento = PropertyEntity({ text: "Ag. Término do Carregamento", cssClass: ko.observable("legenda-item") });
    this.AguardandoSugestaoDataEntrega = PropertyEntity({ text: "Ag. Sugestão de Data de Entrega", cssClass: ko.observable("legenda-item") });
    this.PermiteAgendarAposInicioViagem = PropertyEntity({ text: "Permite agendar após início de viagem", cssClass: ko.observable("legenda-item") });
    this.NaoExigeQueEntregasSejamAgendadas = PropertyEntity({ text: "Não exige que as entregas sejam agendadas", cssClass: ko.observable("legenda-item") });
};

var AgendamentoRegraCadastroClienteLegendas = function () {
    this.ExibirLegendas = PropertyEntity({ eventClick: exibirLegendasClick, type: types.event, text: "Legendas", idFade: guid(), visible: ko.observable(true), visibleFade: ko.observable(false) });

    this.Ate5HorasAgendamento = PropertyEntity({ text: "Até 5 horas em agendamento", cssClass: ko.observable("legenda-item") });
    this.De5Ate10HorasAgendamento = PropertyEntity({ text: "De 5 até 10 horas em agendamento", cssClass: ko.observable("legenda-item") });
    this.HaMais10HorasAgendamento = PropertyEntity({ text: "Há mais de 10 horas em agendamento", cssClass: ko.observable("legenda-item") });
};

function loadAgendamentoEntregaLegendas() {
    _agendamentoEntregaLegendas = new AgendamentoEntregaLegendas();
    KoBindings(_agendamentoEntregaLegendas, "knockoutAgendamentoEntregaLegendas");

    _agendamentoRegraCadastroClienteLegendas = new AgendamentoRegraCadastroClienteLegendas();
    KoBindings(_agendamentoRegraCadastroClienteLegendas, "knockoutAgendamentoRegraCadastroClienteLegendas");

    if (_CONFIGURACAO_TMS.PermitirAgendamentoPedidosSemCarga) {
        $("#knockoutAgendamentoRegraCadastroClienteLegendas").show();
        $("#knockoutAgendamentoEntregaLegendas").hide();
    }
    else {
        $("#knockoutAgendamentoEntregaLegendas").show();
        $("#knockoutAgendamentoRegraCadastroClienteLegendas").hide();
    }
}

function exibirLegendasClick(e) {
    e.ExibirLegendas.visibleFade(!e.ExibirLegendas.visibleFade());
}