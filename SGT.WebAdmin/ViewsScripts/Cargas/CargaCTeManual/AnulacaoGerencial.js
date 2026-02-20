var _anulacaoGerencialCTe = null;

var AnulacaoGerencialCTe = function () {
    this.CodigoCTe = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoCarga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    
    this.Justificativa = PropertyEntity({ text: "*Justificativa: ", maxlength: 1000, required: true });    

    this.Confirmar = PropertyEntity({ eventClick: ConfirmarAnulacaoGerencialCTeClick, type: types.event, text: "Confirmar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: FecharTelaAnulacaoGerencialCTe, type: types.event, text: "Cancelar", visible: ko.observable(true), enable: ko.observable(true) });
};

function LoadAnulacaoGerencialCTe() {
    _anulacaoGerencialCTe = new AnulacaoGerencialCTe();
    KoBindings(_anulacaoGerencialCTe, "divModalAnulacaoGerencialCTe");
}

function AbrirTelaAnulacaoGerencialCTe(e) {
    LimparCampos(_anulacaoGerencialCTe);
    _anulacaoGerencialCTe.CodigoCTe.val(e.CodigoCTE);
    _anulacaoGerencialCTe.CodigoCarga.val(_cargaCTe.Carga.val());
    Global.abrirModal("divModalAnulacaoGerencialCTe");
}

function FecharTelaAnulacaoGerencialCTe() {
    LimparCampos(_anulacaoGerencialCTe);
    Global.fecharModal('divModalAnulacaoGerencialCTe');
}

function ConfirmarAnulacaoGerencialCTeClick() {
    Salvar(_anulacaoGerencialCTe, "CargaCTeManual/AnularGerencialCTe", function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "O CT-e foi anulado gerencialmente com sucesso.");

                FecharTelaAnulacaoGerencialCTe();
                _gridCargaCTe.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}