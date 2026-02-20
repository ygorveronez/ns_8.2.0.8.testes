/*
 * Declaração de Objetos Globais do Arquivo
 */

var _alterarDataAgendamentoEntrega;

/*
 * Declaração das Classes
 */

var AlterarDataAgendamentoEntrega = function () {
    this.CodigoCargaEntrega = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.DataAgendamento = PropertyEntity({ text: "Data de Agendamento", getType: typesKnockout.dateTime });

    this.Confirmar = PropertyEntity({ eventClick: confirmarAlterarDataAgendamentoDeEntregaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Confirmar, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarAlterarDataAgendamentoEntregaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadAlterarDataAgendamentoEntrega() {
    _alterarDataAgendamentoEntrega = new AlterarDataAgendamentoEntrega();
    KoBindings(_alterarDataAgendamentoEntrega, "knockouAlterarDataAgendamentoEntrega");
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function confirmarAlterarDataAgendamentoDeEntregaClick() {
    executarReST("ControleEntregaEntrega/AlterarDataAgendamentoDeTodosPedidos", { CodigoCargaEntrega: _entrega.Codigo.val(), DataAgendamento: _alterarDataAgendamentoEntrega.DataAgendamento.val() }, function (arg) {
        if (arg.Success) {
            Global.fecharModal("divModalAlterarDataAgendamentoEntrega");
            Global.fecharModal("divModalEntrega");

            _entrega.DataAgendamentoDeEntrega.val(arg.Data.DataAgendamentoDeEntrega);
            _entrega.OrigemSituacaoDataAgendamentoEntrega.val(arg.Data.OrigemSituacao);

            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, arg.Msg);
        } else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);

    }, null);
}

function cancelarAlterarDataAgendamentoEntregaClick() {
    Global.fecharModal("divModalAlterarDataAgendamentoEntrega");
}

/*
 * Declaração das Funções
 */

function AbrirModalAlterarDataAgendamentoEntrega() {
    LimparCampos(_alterarDataAgendamentoEntrega);
    _alterarDataAgendamentoEntrega.CodigoCargaEntrega.val(_entrega.Codigo.val());

    Global.abrirModal("divModalAlterarDataAgendamentoEntrega");
}