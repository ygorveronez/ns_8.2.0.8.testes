/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

// #region Objetos Globais do Arquivo

var _detalhePedidoAlterarPrevisaoEntrega;

// #endregion Objetos Globais do Arquivo

// #region Classes

var DetalhePedidoAlterarPrevisaoEntrega = function () {
    this.PedidoAlterar = undefined;
    this.Pedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataPrevisaoEntrega = PropertyEntity({ text: "Nova previsão de entrega: ", required: true, getType: typesKnockout.dateTime });
    this.Responsavel = PropertyEntity({ text: "Responsável: ", required: ko.observable(false), visible: ko.observable(false), val: ko.observable(EnumResponsavelAlteracaoDataPedido.Embarcador), options: EnumResponsavelAlteracaoDataPedido.obterOpcoes(), def: EnumResponsavelAlteracaoDataPedido.Embarcador });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, required: ko.observable(false), visible: ko.observable(false), val: ko.observable("") });
    this.Alterar = PropertyEntity({ type: types.event, eventClick: alterarDataPrevisaoEntregaDetalhePedidoClick, text: "Alterar", visible: ko.observable(true) });
};

// #endregion Classes

// #region Funções de Inicialização

function loadDetalhePedidoAlterarPrevisaoEntrega() {
    _detalhePedidoAlterarPrevisaoEntrega = new DetalhePedidoAlterarPrevisaoEntrega();
    var habilitado = _CONFIGURACAO_TMS.NecessarioInformarJustificativaAoAlterarDataSaidaOuPrevisaoEntregaPedidoNaCarga;
    _detalhePedidoAlterarPrevisaoEntrega.Responsavel.visible(habilitado);
    _detalhePedidoAlterarPrevisaoEntrega.Responsavel.required(habilitado);
    _detalhePedidoAlterarPrevisaoEntrega.Observacao.visible(habilitado);
    _detalhePedidoAlterarPrevisaoEntrega.Observacao.required(habilitado);
    KoBindings(_detalhePedidoAlterarPrevisaoEntrega, "knoutDetalhesPedidoAlterarPrevisaoEntrega");
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function alterarDataPrevisaoEntregaDetalhePedidoClick() {
    Salvar(_detalhePedidoAlterarPrevisaoEntrega, "Carga/AlterarDataPrevisaoEntregaPedido", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Data alterada com sucesso");
                _detalhePedidoAlterarPrevisaoEntrega.PedidoAlterar.DataPrevisaoEntrega.val(_detalhePedidoAlterarPrevisaoEntrega.DataPrevisaoEntrega.val());
                fecharModalAlterarDataPrevisaoEntregaDetalhePedido();
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

function alterarDataPrevisaoEntregaDetalhePedido(e) {
    _detalhePedidoAlterarPrevisaoEntrega.Pedido.val(e.Codigo.val());
    _detalhePedidoAlterarPrevisaoEntrega.DataPrevisaoEntrega.val(e.DataPrevisaoEntrega.val());
    _detalhePedidoAlterarPrevisaoEntrega.PedidoAlterar = e;

    exibirModalAlterarDataPrevisaoEntregaDetalhePedido();
}

// #endregion Funções Públicas

// #region Funções Privadas

function exibirModalAlterarDataPrevisaoEntregaDetalhePedido() {
    Global.abrirModal('divModalDetalhesPedidoAlterarPrevisaoEntrega');
    $("#divModalDetalhesPedidoAlterarPrevisaoEntrega").one('hidden.bs.modal', function () {
        LimparCampos(_detalhePedidoAlterarPrevisaoEntrega);
        _detalhePedidoAlterarPrevisaoEntrega.PedidoAlterar = undefined;
    });
}

function fecharModalAlterarDataPrevisaoEntregaDetalhePedido() {
    Global.fecharModal('divModalDetalhesPedidoAlterarPrevisaoEntrega');
}

// #endregion Funções Públicas
