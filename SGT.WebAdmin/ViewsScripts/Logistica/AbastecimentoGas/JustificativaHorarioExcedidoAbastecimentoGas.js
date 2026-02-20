var _justificativaHorarioExcedido;

var JustificativaHorarioExcedido = function () {
    this.CodigoJustificativa = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.Justificativa = PropertyEntity({ text: "*Justificativa:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 300, required: true, enable: ko.observable(true) });
    
    this.Salvar = PropertyEntity({ eventClick: salvarJustificativa, type: types.event, text: "Salvar", visible: ko.observable(true), id: guid(), visible: ko.observable(true) });
}

function loadJustificativaHorarioExcedidoAbastecimentoGas() {
    _justificativaHorarioExcedido = new JustificativaHorarioExcedido();
    KoBindings(_justificativaHorarioExcedido, "knockoutJustificativaLancamentoHorarioExcedido");
}

function salvarJustificativa() {
    if (!ValidarCamposObrigatorios(_justificativaHorarioExcedido)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios.");
        return;
    }

    salvarAbastecimento();
}

function carregarJustificativa() {
    _justificativaHorarioExcedido.Salvar.visible(false);

    Global.abrirModal('modalJustificativaHoraExcedida');
}