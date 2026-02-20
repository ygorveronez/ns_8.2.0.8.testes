var _CamposObrigatoriosUtilizados = [];
var _ObrigatorioInformarProdutoPedido = false;

function SetarCamposObrigatoriosPedido() {

    InformarObrigatoriedadeCamposPedido(false)

    executarReST("PedidoCampoObrigatorio/BuscarParaPedido", { TipoOperacao: _pedido.TipoOperacao.codEntity() }, function (r) {
        if (r.Success && r.Data && r.Data.Campos) {

            _CamposObrigatoriosUtilizados = r.Data.Campos;
            _ObrigatorioInformarProdutoPedido = r.Data.ObrigatorioInformarProdutoPedido;
            InformarObrigatoriedadeCamposPedido(true)
        }
    });
}

function InformarObrigatoriedadeCamposPedido(obrigatorio) {
    if (_CamposObrigatoriosUtilizados == null || _CamposObrigatoriosUtilizados.length <= 0)
        return;

    for (var i = 0; i < _CamposObrigatoriosUtilizados.length; i++) {
        var campo = _CamposObrigatoriosUtilizados[i].Campo;

        var propriedade = null;

        if (_pedido.hasOwnProperty(campo))
            propriedade = _pedido[campo];

        if (propriedade == null && _adicional.hasOwnProperty(campo))
            propriedade = _adicional[campo];

        if (propriedade != null) {
            propriedade.required = obrigatorio;

            if (propriedade.options != null && propriedade.options.length > 0 && _pedido.Codigo.val() <= 0) {
                if (obrigatorio === true)
                    propriedade.val("");
                else
                    propriedade.val(propriedade.options[0].value);
            }
        }
    }
}