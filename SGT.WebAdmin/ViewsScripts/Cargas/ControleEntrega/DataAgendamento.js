/*
 * Declaração de Objetos Globais do Arquivo
 */

var _dataAgendamento;

/*
 * Declaração das Classes
 */

var DataAgendamento = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataAgendamento = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.ControleEntrega.DataDeAgendamento.getRequiredFieldDescription()), getType: typesKnockout.dateTime, required: true, issue: 0, enable: ko.observable(false), visible: ko.observable(true) });

    this.Atualizar = PropertyEntity({ eventClick: atualizarDataAgendamentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarDataAgendamentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });

    this.dispose = function () {
        RunObjectDispose(this);
    }
}

/*
 * Declaração das Funções de Inicialização
 */


function exibirModalEventoDataAgendamento() {
    Global.abrirModal("divModalEventosDataAgendamento");

    $("#divModalEventosDataAgendamento").one('hidden.bs.modal', function () {
        _dataAgendamento.dispose();
    });
}

function loadEventosDataAgendamento(carga) {
    _dataAgendamento = new DataAgendamento();
    KoBindings(_dataAgendamento, "knockouEventosModalDataAgendamento");

    _dataAgendamento.Carga.val(carga);

    exibirModalEventoDataAgendamento();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function atualizarDataAgendamentoClick(e, sender) {
    Salvar(_dataAgendamento, "ControleEntrega/AlterarDataAgendamento", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.ControleEntrega.DataAlteradaComSucesso);

                atualizarControleEntrega();

                Global.fecharModal("divModalEventosDataAgendamento");
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    }, sender);
}

function cancelarDataAgendamentoClick() {
    LimparCampos(_dataAgendamentos);
    Global.fecharModal("divModalEventosDataAgendamento");
}


