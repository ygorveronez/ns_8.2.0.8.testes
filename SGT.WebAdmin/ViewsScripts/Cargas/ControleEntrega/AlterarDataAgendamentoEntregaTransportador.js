/*
 * Declaração de Objetos Globais do Arquivo
 */

var _alterarDataAgendamentoEntregaTransportador;

/*
 * Declaração das Classes
 */

var AlterarDataAgendamentoEntregaTransportador = function () {
    this.TituloModal = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.AlterarDataAgendamentoEntregaTransportador });

    this.CodigoCargaEntrega = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.DataAgendamentoEntregaTransportador = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataAgendamentoEntregaTransportador.getFieldDescription(), getType: typesKnockout.dateTime });

    this.Confirmar = PropertyEntity({ eventClick: confirmarAlterarDataAgendamentoEntregaTransportadorClick, type: types.event, text: Localization.Resources.Gerais.Geral.Confirmar, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarAlterarDataAgendamentoEntregaTransportadorClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadAlterarDataAgendamentoEntregaTransportador() {
    _alterarDataAgendamentoEntregaTransportador = new AlterarDataAgendamentoEntregaTransportador();
    KoBindings(_alterarDataAgendamentoEntregaTransportador, "knockouAlterarDataAgendamentoEntregaTransportador");
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function confirmarAlterarDataAgendamentoEntregaTransportadorClick() {
    executarReST("ControleEntregaEntrega/AlterarDataAgendamentoEntregaTransportador", { CodigoCargaEntrega: _entrega.Codigo.val(), DataAgendamentoEntregaTransportador: _alterarDataAgendamentoEntregaTransportador.DataAgendamentoEntregaTransportador.val() }, function (arg) {
        if (arg.Success) {

            BuscarDetalhesEntrega({Codigo:_entrega.Codigo.val()})

            Global.fecharModal("divModalAlterarDataAgendamentoEntregaTransportador");

            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, arg.Msg);
        } else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);

    }, null);
}

function cancelarAlterarDataAgendamentoEntregaTransportadorClick() {
    Global.fecharModal("divModalAlterarDataAgendamentoEntregaTransportador");
}

/*
 * Declaração das Funções
 */

function AbrirModalAlterarDataAgendamentoEntregaTransportador() {
    LimparCampos(_alterarDataAgendamentoEntregaTransportador);
    _alterarDataAgendamentoEntregaTransportador.CodigoCargaEntrega.val(_entrega.Codigo.val());
    _alterarDataAgendamentoEntregaTransportador.DataAgendamentoEntregaTransportador.val(_entrega.DataAgendamentoEntregaTransportador.val());

    Global.abrirModal("divModalAlterarDataAgendamentoEntregaTransportador");
}