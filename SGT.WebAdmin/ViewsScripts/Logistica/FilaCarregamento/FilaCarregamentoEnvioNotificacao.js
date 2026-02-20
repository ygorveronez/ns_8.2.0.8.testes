/// <reference path="FilaCarregamento.js" />
/// <reference path="../../Enumeradores/EnumSituacaoFilaCarregamentoVeiculo.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _filaCarregamentoEnvioNotificacao;

/*
 * Declaração das Classes
 */

var FilaCarregamentoEnvioNotificacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Mensagem = PropertyEntity({ text: "*Mensagem:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 100, required: true });

    this.Enviar = PropertyEntity({ eventClick: enviarNotificacaoMotoristaClick, type: types.event, text: ko.observable("Enviar"), visible: true });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadFilaCarregamentoEnvioNotificacao() {
    _filaCarregamentoEnvioNotificacao = new FilaCarregamentoEnvioNotificacao();
    KoBindings(_filaCarregamentoEnvioNotificacao, "knockoutEnvioNotificacao");
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function enviarNotificacaoMotoristaClick() {
    if (ValidarCamposObrigatorios(_filaCarregamentoEnvioNotificacao)) {
        executarReST("FilaCarregamento/EnviarNotificacao", RetornarObjetoPesquisa(_filaCarregamentoEnvioNotificacao), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Notificação enviada com sucesso.");
                    fecharModalFilaCarregamentoEnvioNotificacao();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
    else
        exibirMensagemCamposObrigatorio();
}

/*
 * Declaração das Funções
 */

function ExibirModalFilaCarregamentoEnvioNotificacao(filaSelecionada) {
    _filaCarregamentoEnvioNotificacao.Codigo.val(filaSelecionada.Codigo);

    Global.abrirModal('divModalEnvioNotificacao');
    $("#divModalEnvioNotificacao").one('hidden.bs.modal', function () {
        LimparCampos(_filaCarregamentoEnvioNotificacao);
    });
}

function fecharModalFilaCarregamentoEnvioNotificacao() {
    Global.fecharModal('divModalEnvioNotificacao');
}