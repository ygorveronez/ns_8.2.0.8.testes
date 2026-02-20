/*
 * Declaração de Objetos Globais do Arquivo
 */

var _alterarDataCarregamentoJanelaCarregamentoTransportador;

/*
 * Declaração das Classes
 */

var AlterarDataCarregamentoJanelaCarregamentoTransportador = function () {
    this.CodigoCargaJanelaCarregamento = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.DataAgendamento = PropertyEntity({ text: "Data de Agendamento", getType: typesKnockout.dateTime });

    this.Confirmar = PropertyEntity({ eventClick: confirmarAlterarDataCarregamentoJanelaCarregamentoTransportadorClick, type: types.event, text: Localization.Resources.Gerais.Geral.Confirmar, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarAlterarDataCarregamentoJanelaCarregamentoTransportadorClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadAlterarDataCarregamentoJanelaCarregamentoTransportador() {
    _alterarDataCarregamentoJanelaCarregamentoTransportador = new AlterarDataCarregamentoJanelaCarregamentoTransportador();
    KoBindings(_alterarDataCarregamentoJanelaCarregamentoTransportador, "knockouAlterarDataCarregamentoJanelaCarregamentoTransportador");
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function confirmarAlterarDataCarregamentoJanelaCarregamentoTransportadorClick() {
    executarReST("JanelaCarregamentoTransportador/AlterarHorarioCarregamento", { CodigoCargaJanelaCarregamento: _alterarDataCarregamentoJanelaCarregamentoTransportador.CodigoCargaJanelaCarregamento.val(), DataCarregamento: _alterarDataCarregamentoJanelaCarregamentoTransportador.DataAgendamento.val() }, function (arg) {
        if (arg.Success) {
            Global.fecharModal("divModalAlterarDataCarregamentoJanelaCarregamentoTransportador");
            Global.fecharModal("divModalDetalhesCarga");

            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, arg.Msg);
        } else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);

    }, null);
}

function cancelarAlterarDataCarregamentoJanelaCarregamentoTransportadorClick() {
    Global.fecharModal("divModalAlterarDataCarregamentoJanelaCarregamentoTransportador");
}

/*
 * Declaração das Funções
 */

function AbrirModalAlterarDataCarregamentoJanelaCarregamentoTransportadorClick(cargaSelecionada) {
    LimparCampos(_alterarDataCarregamentoJanelaCarregamentoTransportador);    
    _alterarDataCarregamentoJanelaCarregamentoTransportador.CodigoCargaJanelaCarregamento.val(cargaSelecionada.Codigo.val());

    Global.abrirModal("divModalAlterarDataCarregamentoJanelaCarregamentoTransportador");
}