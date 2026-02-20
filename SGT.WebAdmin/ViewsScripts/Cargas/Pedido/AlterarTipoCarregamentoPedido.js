/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumTipoCarregamentoPedido.js" />

// #region Objetos Globais do Arquivo

var _detalhePedidoAlterarTipoCarregamentoPedido;

// #endregion Objetos Globais do Arquivo

// #region Classes

var DetalhePedidoAlterarTipoCarregamentoPedido = function () {
    this.PedidoAlterar = undefined;
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Pedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoCarregamentoPedido = PropertyEntity({ val: ko.observable(EnumTipoCarregamentoPedido.Normal), options: EnumTipoCarregamentoPedido.obterOpcoes(), def: EnumTipoCarregamentoPedido.Normal, text: "*Tipo do Carregamento: ", required: true });

    this.Alterar = PropertyEntity({ type: types.event, eventClick: alterarTipoCarregamentoPedidoDetalhePedidoClick, text: "Alterar", visible: ko.observable(true) });
};

// #endregion Classes

// #region Funções de Inicialização

function loadDetalhePedidoAlterarTipoCarregamentoPedido() {
    _detalhePedidoAlterarTipoCarregamentoPedido = new DetalhePedidoAlterarTipoCarregamentoPedido();

    KoBindings(_detalhePedidoAlterarTipoCarregamentoPedido, "knoutDetalhesPedidoAlterarTipoCarregamentoPedido");
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function alterarTipoCarregamentoPedidoDetalhePedidoClick() {
    Salvar(_detalhePedidoAlterarTipoCarregamentoPedido, "Carga/AlterarTipoCarregamentoPedido", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Tipo do carregamento do pedido alterado com sucesso");
                _detalhePedidoAlterarTipoCarregamentoPedido.PedidoAlterar.TipoCarregamentoPedido.val(_detalhePedidoAlterarTipoCarregamentoPedido.TipoCarregamentoPedido.val());
                _detalhePedidoAlterarTipoCarregamentoPedido.PedidoAlterar.TipoCarregamentoPedidoDescricao.val(EnumTipoCarregamentoPedido.obterDescricao(_detalhePedidoAlterarTipoCarregamentoPedido.TipoCarregamentoPedido.val()));
                fecharModalAlterarTipoCarregamentoPedidoDetalhePedido();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg, 20000);
        }
        else
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function alterarTipoCarregamentoPedidoDetalhePedido(codigoCarga, pedidoAlterar) {
    _detalhePedidoAlterarTipoCarregamentoPedido.Carga.val(codigoCarga);
    _detalhePedidoAlterarTipoCarregamentoPedido.Pedido.val(pedidoAlterar.Codigo.val());
    _detalhePedidoAlterarTipoCarregamentoPedido.TipoCarregamentoPedido.val(pedidoAlterar.TipoCarregamentoPedido.val());
    _detalhePedidoAlterarTipoCarregamentoPedido.PedidoAlterar = pedidoAlterar;

    exibirModalAlterarTipoCarregamentoPedidoDetalhePedido();
}

// #endregion Funções Públicas

// #region Funções Privadas

function exibirModalAlterarTipoCarregamentoPedidoDetalhePedido() {
    Global.abrirModal('divModalDetalhesPedidoAlterarTipoCarregamentoPedido');
    $("#divModalDetalhesPedidoAlterarTipoCarregamentoPedido").one('hidden.bs.modal', function () {
        LimparCampos(_detalhePedidoAlterarTipoCarregamentoPedido);
        _detalhePedidoAlterarTipoCarregamentoPedido.PedidoAlterar = undefined;
    });
}

function fecharModalAlterarTipoCarregamentoPedidoDetalhePedido() {
    Global.fecharModal('divModalDetalhesPedidoAlterarTipoCarregamentoPedido');
}

// #endregion Funções Públicas
