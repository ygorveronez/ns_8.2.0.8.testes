/// <reference path="baixarCanhoto.js" />

var _dataEntregaCliente;

var CanhotoDataEntregaCliente = function () {
    this.DataEntregaNotaCliente = PropertyEntity({ text: "Data Entrega da Nota ao cliente", getType: typesKnockout.dateTime, required: ko.observable(true) });
    this.Enviar = PropertyEntity({ type: types.event, eventClick: informarDataEntregaClienteCanhotoClick, text: ko.observable("Enviar") });
    this.DadosBaixa= PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), def: "", visible: ko.observable(true) });
};

_dataEntregaCliente = new CanhotoDataEntregaCliente();
KoBindings(_dataEntregaCliente, "knoutDataEntregaClienteCanhoto");

function informarDataEntregaClienteCanhotoClick() {

    var data = { DadosBaixa: _baixarCanhoto.DadosBaixa.val(), DataEntregaNotaCliente: _dataEntregaCliente.DataEntregaNotaCliente.val() };

    executarReST("Canhoto/EnviarDataEntregaNotaCliente", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                Global.fecharModal('divModalDataEntregaCanhoto');
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.EnviadoComSucesso);
                baixarCanhotoClick();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}