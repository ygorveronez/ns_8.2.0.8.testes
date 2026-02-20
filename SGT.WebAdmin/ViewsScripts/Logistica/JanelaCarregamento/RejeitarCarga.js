/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="RetornoMultiplaSelecao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _rejeitarCargaTransportador;

/*
 * Declaração das Classes
 */

var RejeitarCargaTransportador = function () {
    this.CodigoCarga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.MotivoRejeicaoCarga = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.Carga.MotivoRejeicaoCarga.getFieldDescription(), maxlength: 500, required: true });

    this.Rejeitar = PropertyEntity({ eventClick: rejeitarCargaPortalTransportadorClick, type: types.event, text: Localization.Resources.Gerais.Geral.Rejeitar, idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadRejeitarCargaTransportador() {
    _rejeitarCargaTransportador = new RejeitarCargaTransportador();
    KoBindings(_rejeitarCargaTransportador, "knockoutRejeitarCargaTransportador");
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function rejeitarCargaPortalTransportadorClick() {
    if (!ValidarCamposObrigatorios(_rejeitarCargaTransportador))
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.VoceDeveInformarMotivoDeRejeicaoParaRejeitarCargaParaTrasnportador)
    else {
        var dados = RetornarObjetoPesquisa(_rejeitarCargaTransportador);

        executarReST("JanelaCarregamento/RejeitarCargaJanelaCarregamentoTransportador", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    fecharModalMotivoRejeicaoTransportador();
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, retorno.Msg);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    }
}

/*
 * Declaração das Funções Públicas
 */

function exibirMotivoRejeicaoTransportador(carga) {
    _rejeitarCargaTransportador.CodigoCarga.val(carga.Carga.Codigo);

    exibirModalMotivoRejeicaoTransportador();
}

/*
 * Declaração das Funções Privadas
 */

function exibirModalMotivoRejeicaoTransportador() {
    Global.abrirModal('divModalRejeitarCargaTransportador');
    $("#divModalRejeitarCargaTransportador").one('hidden.bs.modal', function () {
        LimparCampos(_rejeitarCargaTransportador);
    });
}

function fecharModalMotivoRejeicaoTransportador() {
    Global.fecharModal('divModalRejeitarCargaTransportador');
}