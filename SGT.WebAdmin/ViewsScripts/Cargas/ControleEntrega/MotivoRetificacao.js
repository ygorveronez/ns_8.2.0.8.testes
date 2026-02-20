/*
 * Declaração de Objetos Globais do Arquivo
 */

var _motivoRetificacaoColetaEntrega;

/*
 * Declaração das Classes
 */

var MotivoRetificacaoColetaEntrega = function () {
    this.MotivoRetificacao = PropertyEntity({ val: ko.observable(0), options: ko.observable(new Array()), def: 0, text: Localization.Resources.Cargas.ControleEntrega.MotivoRejeicao.getFieldDescription(), visible: ko.observable(true) });

    this.Confirmar = PropertyEntity({ eventClick: confirmarMotivoRetificacaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Confirmar, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarMotivoRetificacaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */


function exibirModalMotivosRetificacao() {
    Global.abrirModal("divModalMotivoRetificacaoColetaEntrega");
}

function loadMotivosRetificacaoColetaEntrega() {
    _motivoRetificacaoColetaEntrega = new MotivoRetificacaoColetaEntrega();
    KoBindings(_motivoRetificacaoColetaEntrega, "knockouMotivoRetificacaoColetaEntrega");
}

function ObterMotivosRetificacao(tipoAplicacaoColetaEntrega) {
    var motivoRetificacaoOption = new Array();
    executarReST("MotivoRetificacaoColeta/BuscarMotivosRetificacao", { TipoAplicacaoColetaEntrega: tipoAplicacaoColetaEntrega }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                var motivos = arg.Data;
                if (motivos.length > 0) {
                    for (var i = 0; i < motivos.length; i++) {
                        motivoRetificacaoOption.push({ text: motivos[i].Descricao, value: motivos[i].Codigo });
                    }
                    _motivoRetificacaoColetaEntrega.MotivoRetificacao.options(motivoRetificacaoOption);
                    _motivoRetificacaoColetaEntrega.MotivoRetificacao.val(motivos[0].Codigo);
                }
                exibirModalMotivosRetificacao();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, arg.Msg, 16000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, null);
}


/*
 * Declaração das Funções Associadas a Eventos
 */

function cancelarMotivoRetificacaoClick() {
    Global.fecharModal("divModalMotivoRetificacaoColetaEntrega");
}

function confirmarMotivoRetificacaoClick() {
   
    executarReST("ControleEntrega/ReverterEntregaZerarData", { CodigoMotivoRetificacao: _motivoRetificacaoColetaEntrega.MotivoRetificacao.val(), CodigoCargaEntrega: _entrega.Codigo.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                atualizarControleEntrega();
                Global.fecharModal("divModalEntrega");
            } else {
                _entrega.RejeitarEntrega.visible(true);
                _entrega.Retificar.visible(false);
                _entrega.InfoMotivoRejeicao.visible(true);
                _entrega.InfoMotivoRejeicao.val($("#" + _motivoRetificacaoColetaEntrega.MotivoRetificacao.id + " option:selected").text());

                if (_entrega.EnumSituacao.val() == EnumSituacaoEntrega.Rejeitado) {
                    _entrega.ConfirmarEntrega.visible(true);
                }
                _entrega.MotivoRejeicao.enable(true)
                _entrega.TipoDevolucao.enable(true)
                _entrega.PermitirEntregarMaisTarde.enable(true)

                _entrega.RejeitarEntrega.visible(true);
                _entrega.AlterarDataEntrega.visible(true);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, null);
    Global.fecharModal("divModalMotivoRetificacaoColetaEntrega");
    


}


