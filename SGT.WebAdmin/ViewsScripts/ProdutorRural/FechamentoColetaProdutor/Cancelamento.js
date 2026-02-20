
var CancelamentoFechamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.MotivoCancelamento = PropertyEntity({ type: types.map, val: ko.observable(""), text: "*Motivo:", required: true, enable: ko.observable(true) });
    this.ConfirmarCancelamento = PropertyEntity({ eventClick: confirmarCancelamentoFechamentoColetaProdutorClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
}

var _cancelamentoFechamento;

function LoadCancelamentoFechamento() {
    _cancelamentoFechamento = new CancelamentoFechamento();
    KoBindings(_cancelamentoFechamento, "knoutCancelamentoFechamento");
}

function cancelarFechamentoColetaProdutorClick(e, sender) {
    _cancelamentoFechamento.Codigo.val(e.Codigo.val());
    Global.abrirModal('divModalCancelamentoFechamento');
}

function confirmarCancelamentoFechamentoColetaProdutorClick(e, sender) {
    var dados = RetornarObjetoPesquisa(_cancelamentoFechamento);
    Salvar(_cancelamentoFechamento, "FechamentoColetaProdutor/CancelarFechamento", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Fechamento cancelado com sucesso");
                _fechamentoColetaProdutor.Situacao.val(EnumSituacaoFechamentoColetaProdutor.Cancelado);
                _gridFechamentoColetaProdutor.CarregarGrid();
                Global.fecharModal("divModalCancelamentoFechamento");
                LimparCamposFechamentoColetaProdutor();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function limparCamposCancelamentoFechamento() {
    LimparCampos(_cancelamentoFechamento);
}