/// <reference path="../../Consultas/MotivoCancelamentoPedido.js" />

////*******MAPEAMENTO KNOUCKOUT*******

var _cancelamentoPedido;

var CancelamentoPedido = function () {
    this.Motivo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motivo:", idBtnSearch: guid(), required: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarCancelamentoPedidoClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
    this.Confirmar = PropertyEntity({ eventClick: salvarCancelamentoPedidoClick, type: types.event, text: "Salvar", visible: ko.observable(true) });
    
}

////*******EVENTOS*******

function loadCancelamentoPedido() {   
    _cancelamentoPedido = new CancelamentoPedido();
    KoBindings(_cancelamentoPedido, "knockoutMotivoCancelamento");

    new BuscarMotivoCancelamentoPedido(_cancelamentoPedido.Motivo);
}

function cancelarCancelamentoPedidoClick() {
   fecharModalCancelamentoPedido();
}

function salvarCancelamentoPedidoClick() {
    _cancelamentoPedido.Motivo.required(true);

    var validaMotivo = ValidarCampoObrigatorioEntity(_cancelamentoPedido.Motivo);

    _cancelamentoPedido.Motivo.required(false);

    if (!validaMotivo) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    execultarExclusao(_cancelamentoPedido.Motivo.codEntity());
}

function execultarExclusao(Motivo){
    iniciarControleManualRequisicao();
    _handleExclusao(Motivo);
}

function _handleExclusao(Motivo) {
    executarReST("Pedido/ExcluirPorCodigo", { Codigo: _pedido.Codigo.val(), Motivo: Motivo}, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {

                if (arg.Data.PossuiDependencias)
                    _handleCancelamento();
                else
                    callbackExclusao();
            }
            else {
                finalizarControleManualRequisicao();
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            finalizarControleManualRequisicao();
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, null);
};

function _handleCancelamento() {
    executarReST("Pedido/CancelarPorCodigo", { Codigo: _pedido.Codigo.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                callbackExclusao();
            }
            else {
                finalizarControleManualRequisicao();
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            finalizarControleManualRequisicao();
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, null);
};

function callbackExclusao() {
    fecharModalCancelamentoPedido();
    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
    _gridPedido.CarregarGrid();
    limparCamposPedido();
    finalizarControleManualRequisicao();
};


function excluirPedidoMotivo(){
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        Global.abrirModal('divModalMotivoCancelamento');
    } else {
        exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Pedidos.Pedido.RealmenteDesejaExcluirOPedido.format(_pedido.NumeroPedidoEmbarcador.val()), function () {

            execultarExclusao(null);

        });
    }
}

function fecharModalCancelamentoPedido() {
    LimparCampoEntity(_cancelamentoPedido.Motivo);
    Global.fecharModal('divModalMotivoCancelamento');    
}

////*******MÉTODOS*******