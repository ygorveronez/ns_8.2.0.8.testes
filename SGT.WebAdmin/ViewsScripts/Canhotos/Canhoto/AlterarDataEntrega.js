/// <reference path="Canhoto.js" />

var _canhotoDataEntregaCliente;

var CanhotoDataEntregaCliente = function () {
    this.DataEntregaNotaCliente = PropertyEntity({ text: "Data Entrega da Nota ao cliente", getType: typesKnockout.dateTime, required: ko.observable(true) });
    this.Enviar = PropertyEntity({ type: types.event, eventClick: alterarDataEntregaClienteCanhotoClick, text: ko.observable("Enviar") });
};

_canhotoDataEntregaCliente = new CanhotoDataEntregaCliente();
KoBindings(_canhotoDataEntregaCliente, "knoutAlterarDataEntregaCanhoto");

function alterarDataEntregaClienteCanhotoClick() {
    let data = { CodigoCanhoto: _knoutArquivo.Codigo.val(), DataEntregaNotaCliente: _canhotoDataEntregaCliente.DataEntregaNotaCliente.val() };

    executarReST("Canhoto/AlterarDataEntrega", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                Global.fecharModal('divModalDataEntregaCanhoto');
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.EnviadoComSucesso);
                Global.fecharModal('divModalAlterarDataEntrega');
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}