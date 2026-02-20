var _cancelamentoCargaCTeAgrupado;

var CancelamentoCargaCTeAgrupado = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.Motivo = PropertyEntity({ getType: typesKnockout.string, text: "*Motivo:", val: ko.observable(""), def: "", required: true, maxlength: 150 });

    this.Cancelar = PropertyEntity({ eventClick: CancelarCargaCTeAgrupadoClick, type: types.event, text: "Cancelar", icon: "fal fa-trash", visible: ko.observable(true) });
    this.Fechar = PropertyEntity({ eventClick: FecharTelaCancelamentoCargaCTeAgrupado, type: types.event, text: "Fechar", icon: "fal fa-window-close", visible: ko.observable(true) });
};

////*******EVENTOS*******

function LoadCancelamentoCargaCTeAgrupado() {
    _cancelamentoCargaCTeAgrupado = new CancelamentoCargaCTeAgrupado();
    KoBindings(_cancelamentoCargaCTeAgrupado, "knockoutCancelamentoCargaCTeAgrupado");
}

function CancelarCargaCTeAgrupadoClick(e, sender) {
    exibirConfirmacao("Confirmação!", "Deseja realmente cancelar o CT-e agrupado?", function () {
        Salvar(_cancelamentoCargaCTeAgrupado, "CargaCTeAgrupado/Cancelar", function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Cancelamento solicitado com sucesso!");
                    BuscarCargaCTeAgrupadoPorCodigo(_cancelamentoCargaCTeAgrupado.Codigo.val());
                    FecharTelaCancelamentoCargaCTeAgrupado();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

////*******METODOS*******

function LimparCamposCancelamentoCargaCTeAgrupado() {
    LimparCampos(_cancelamentoCargaCTeAgrupado);
}

function AbrirTelaCancelamentoCargaCTeAgrupado() {
    LimparCamposCancelamentoCargaCTeAgrupado();
    _cancelamentoCargaCTeAgrupado.Codigo.val(_cargaCTeAgrupado.Codigo.val());
    Global.abrirModal("knockoutCancelamentoCargaCTeAgrupado");
}

function FecharTelaCancelamentoCargaCTeAgrupado() {
    Global.fecharModal('knockoutCancelamentoCargaCTeAgrupado');
    LimparCamposCancelamentoCargaCTeAgrupado();
}