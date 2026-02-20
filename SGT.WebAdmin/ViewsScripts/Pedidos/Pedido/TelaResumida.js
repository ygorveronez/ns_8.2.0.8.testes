

function SetarCamposTelaResumida() {
    if (!(_CONFIGURACAO_TMS.TelaPedidosResumido === true))
        return;


    for (var prop in _pedido) {
        if ($.isFunction(_pedido[prop].visible))
            _pedido[prop].visible(false);
    }
    _pedido.NumeroPedidoEmbarcador.enable(false);
    _pedido.PesoTotalCarga.required = false;

    _pedido.Filial.visible(true);
    _pedido.DataCarregamento.visible(true);
    _pedido.Remetente.visible(true);
    _pedido.Destinatario.visible(true);
    _pedido.Veiculo.visible(true);
    _pedido.Empresa.visible(true);
    _pedido.TipoCarga.visible(true);

    _pedido.ObservacaoAbaPedido.visible(true);
    _pedido.DataCarregamento.visible(true);

    _CRUDPedido.Importar.visible(false);


    $("#myTab li").hide();
    $("#myTab li:eq(0)").show();
}