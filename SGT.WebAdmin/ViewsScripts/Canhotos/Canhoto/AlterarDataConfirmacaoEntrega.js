/// <reference path="../../Consultas/MotivoRejeicaoAuditoria.js" />
/// <reference path="../../../js/Global/Mensagem.js" />

var _alterarDataConfirmacaoEntrega;

var AlterarDataConfirmacaoEntrega = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int });
    this.CodigoCargaEntrega = PropertyEntity({ getType: typesKnockout.int });
    this.DataEntregaConfirmacaoCanhoto = PropertyEntity({ text: "*Data entrega: ", getType: typesKnockout.dateTime, required: true });

    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
    this.Confirmar = PropertyEntity({ eventClick: confirmarClick, type: types.event, text: "Confirmar", visible: ko.observable(true) });
}

//*******EVENTOS*******

function LoadAlterarDataConfirmacaoEntrega() {
    _alterarDataConfirmacaoEntrega = new AlterarDataConfirmacaoEntrega();
    KoBindings(_alterarDataConfirmacaoEntrega, "knoutAlteracaoDataConfirmacaoEntrega");
}

function AbrirModalAlterarDataConfirmacaoEntrega(e, callback) {
    _alterarDataConfirmacaoEntrega.Codigo.val(e.Codigo);
    _alterarDataConfirmacaoEntrega.CodigoCargaEntrega.val(e.CodigoCargaEntrega);

    if (callback != null)
        callback();
}

function confirmarClick() {
    if (_alterarDataConfirmacaoEntrega.DataEntregaConfirmacaoCanhoto.val() === "")
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Canhotos.Canhoto.PreenchaCampoDataEntrega);
    else {
        let dados = {
            CodigoCargaEntrega: _alterarDataConfirmacaoEntrega.CodigoCargaEntrega.val(),
            DataEntrega: _alterarDataConfirmacaoEntrega.DataEntregaConfirmacaoCanhoto.val()
        };
        executarReST("Canhoto/AlterarDataConfirmacaoEntrega", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    Global.fecharModal('divModalAlteracaoDataConfirmaçãoEntrega');
                    LimparCampos(_alterarDataConfirmacaoEntrega);
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, arg.Msg);
                }
                if (arg.Data === false)
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        });
    }
}

function cancelarClick() {
    Global.fecharModal('divModalAlteracaoDataConfirmaçãoEntrega');
}