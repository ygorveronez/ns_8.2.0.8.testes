/*
 * Declaração de Objetos Globais do Arquivo
 */

var _dataReagendamento;

/*
 * Declaração das Classes
 */

var DataReagendamento = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataReagendamento = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.ControleEntrega.DataReagendamento.getRequiredFieldDescription()), getType: typesKnockout.dateTime, required: true, issue: 0, enable: ko.observable(false), visible: ko.observable(true) });

    this.Atualizar = PropertyEntity({ eventClick: atualizarDataReagendamentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarDataAgendamentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });

    this.dispose = function () {
        RunObjectDispose(this);
    }
}

/*
 * Declaração das Funções de Inicialização
 */


function exibirModalEventoDataReagendamento() {
    Global.abrirModal('divModalEventosDataReagendamento');
    $("#divModalEventosDataReagendamento").one('hidden.bs.modal', function () {
        _dataReagendamento.dispose();
    });
}

function loadEventosDataReagendamento(carga,data) {
    _dataReagendamento = new DataReagendamento();

    KoBindings(_dataReagendamento, "knockouEventosModalDataReagendamento");

    _dataReagendamento.DataReagendamento.val(data);
    _dataReagendamento.Carga.val(carga);

    exibirModalEventoDataReagendamento();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function atualizarDataReagendamentoClick(e, sender) {
    Salvar(_dataReagendamento, "ControleEntrega/AlterarDataReagendamento", function (retorno) {
        if (!retorno.Success)
            return exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);

        if (!retorno.Data)
            return exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);

        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.ControleEntrega.DataAlteradaComSucesso);
        atualizarControleEntrega();
        Global.fecharModal('divModalEventosDataReagendamento');

    }, sender);
}

function cancelarDataReagendamentoClick() {
    LimparCampos(_dataReagendamento);
    Global.fecharModal('divModalEventosDataReagendamento');
}


