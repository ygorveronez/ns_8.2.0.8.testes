
/*
 * Declaração de Objetos Globais do Arquivo
 */

var _fluxoPatioEnvioNotificacaoSMS;

/*
 * Declaração das Classes
 */
var FluxoPatioEnvioNotificacaoSMS = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Mensagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Notificação:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true) });

    this.Enviar = PropertyEntity({ eventClick: enviarNotificacaoMotoristaSMSClick, type: types.event, text: ko.observable("Enviar"), visible: true });
}

/*
 * Declaração das Funções de Inicialização
 */
function loadFluxoPatioEnvioNotificacaoSMS() {
    _fluxoPatioEnvioNotificacaoSMS = new FluxoPatioEnvioNotificacaoSMS();

    KoBindings(_fluxoPatioEnvioNotificacaoSMS, "knockoutEnvioNotificacaoSMS");

    BuscarConfiguracaoNotificacaoMotoristaSMS(_fluxoPatioEnvioNotificacaoSMS.Mensagem, EnumTipoNotificacaoMotoristaSMS.GestaoDePatioEnvioManual);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function enviarNotificacaoMotoristaSMSClick() {
    executarReST("FluxoPatio/EnviarNotificacaoMotoristaSMS", RetornarObjetoPesquisa(_fluxoPatioEnvioNotificacaoSMS), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Notificação enviada com sucesso.");
                fecharModalFluxoPatioEnvioNotificacaoSMS();
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

function exibirModalFluxoPatioEnvioNotificacaoSMS(codigoFilaSelecionada) {   
    _fluxoPatioEnvioNotificacaoSMS.Codigo.val(codigoFilaSelecionada);

    Global.abrirModal('divModalEnvioNotificacaoSMS');
    $("#divModalEnvioNotificacaoSMS").one('hidden.bs.modal', function () {
        LimparCampos(_fluxoPatioEnvioNotificacaoSMS);
    });
}

function fecharModalFluxoPatioEnvioNotificacaoSMS() {
    Global.fecharModal('divModalEnvioNotificacaoSMS');
}