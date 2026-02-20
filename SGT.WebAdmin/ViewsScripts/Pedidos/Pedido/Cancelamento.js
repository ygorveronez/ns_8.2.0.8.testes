var _pedidoCancelamento;

var PedidoCancelamento = function () {

    this.Motivo = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.Motivo.getFieldDescription() });

    this.Cancelar = PropertyEntity({ eventClick: CancelarPedidoClick, type: types.event, text: Localization.Resources.Pedidos.Pedido.CancelaroPedido, visible: ko.observable(true) });
    this.Retornar = PropertyEntity({ eventClick: FecharTelaCancelamentoPedido, type: types.event, text: Localization.Resources.Pedidos.Pedido.Voltar, visible: ko.observable(true) });
};

function LoadPedidoCancelamento() {
    _pedidoCancelamento = new PedidoCancelamento();
    KoBindings(_pedidoCancelamento, "divCancelamentoPedido");
}

function AbrirTelaCancelamentoPedido() {
    LimparCampos(_pedidoCancelamento);    
    Global.abrirModal('divCancelamentoPedido');
}

function FecharTelaCancelamentoPedido() {
    Global.fecharModal('divCancelamentoPedido');
}

function CancelarPedidoClick() {
    executarReST("Pedido/CancelarPorCodigo", { Codigo: _pedido.Codigo.val(), Motivo: _pedidoCancelamento.Motivo.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                FecharTelaCancelamentoPedido();
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Pedidos.Pedido.PedidoCanceladoComSucesso);
                limparCamposPedido();
                _gridPedido.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Remetente.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, null);
}