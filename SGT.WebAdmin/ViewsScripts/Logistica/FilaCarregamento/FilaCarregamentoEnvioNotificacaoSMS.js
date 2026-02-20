/// <reference path="FilaCarregamento.js" />
/// <reference path="../../Enumeradores/EnumSituacaoFilaCarregamentoVeiculo.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _filaCarregamentoEnvioNotificacaoSMS;

/*
 * Declaração das Classes
 */

var FilaCarregamentoEnvioNotificacaoSMS = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Mensagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Notificação:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true) });

    this.Enviar = PropertyEntity({ eventClick: enviarNotificacaoMotoristaSMSClick, type: types.event, text: ko.observable("Enviar"), visible: true });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadFilaCarregamentoEnvioNotificacaoSMS() {
    _filaCarregamentoEnvioNotificacaoSMS = new FilaCarregamentoEnvioNotificacaoSMS();

    KoBindings(_filaCarregamentoEnvioNotificacaoSMS, "knockoutEnvioNotificacaoSMS");

    BuscarConfiguracaoNotificacaoMotoristaSMS(_filaCarregamentoEnvioNotificacaoSMS.Mensagem, EnumTipoNotificacaoMotoristaSMS.FilaCarregamentoEnvioManual);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function enviarNotificacaoMotoristaSMSClick() {
    executarReST("FilaCarregamento/EnviarNotificacaoMotoristaSMS", RetornarObjetoPesquisa(_filaCarregamentoEnvioNotificacaoSMS), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Notificação enviada com sucesso.");
                fecharModalFilaCarregamentoEnvioNotificacaoSMS();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

/*
 * Declaração das Funções
 */

function ExibirModalFilaCarregamentoEnvioNotificacaoSMS(filaSelecionada) {
    _filaCarregamentoEnvioNotificacaoSMS.Codigo.val(filaSelecionada.Codigo);

    Global.abrirModal('divModalEnvioNotificacaoSMS');
    $("#divModalEnvioNotificacaoSMS").one('hidden.bs.modal', function () {
        LimparCampos(_filaCarregamentoEnvioNotificacaoSMS);
    });
}

function fecharModalFilaCarregamentoEnvioNotificacaoSMS() {
    Global.fecharModal('divModalEnvioNotificacaoSMS');
}