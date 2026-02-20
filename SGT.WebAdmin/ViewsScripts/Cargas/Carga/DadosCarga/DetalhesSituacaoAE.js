/// <reference path="Carga.js" />

var _detalhesSituacaoAE;

var DetalhesSituacaoAE = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.MotivoSituacaoAE = PropertyEntity({ text: "Motivo Situação AE:", val: ko.observable(""), def: "" });
}

function LoadDetalhesSituacaoAE() {
    _detalhesSituacaoAE = new DetalhesSituacaoAE();
    KoBindings(_detalhesSituacaoAE, "knockoutMotivoSituacaoAE");
}

function LimparCamposDetalhesSituacaoAE() {
    LimparCampos(_detalhesSituacaoAE);
    Global.fecharModal("divModalMotivoSituacaoAE");
}

function verMotivoSituacaoAEClick(e, sender) {
    LimparCamposDetalhesSituacaoAE();
    executarReST("Carga/BuscarDetalhesAE", { Carga: e.Codigo.val() }, function (arg) {
        if (arg.Data) {
            PreencherObjetoKnout(_detalhesSituacaoAE, arg);
            Global.abrirModal("divModalMotivoSituacaoAE");
        }
    });
}
