/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="Pedido.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _resumoPedido;

var ResumoPedido = function () {

    this.NumeroPedido = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.NumeroPedido.getFieldDescription(), visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.TipoOperacao.getFieldDescription(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.SituacaoComercialPedido = PropertyEntity({ text: Localization.Resources.Consultas.SituacaoComercialPedido.SituacaoComercialDoPedido.getFieldDescription(), getType: typesKnockout.dateTime, visible: ko.observable(_CONFIGURACAO_TMS.ExisteSituacaoComercialPedido) });
    this.SituacaoComercialPedidoBolinha = PropertyEntity({ val: ko.observable("") });
    this.Etapa = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.Etapa.getFieldDescription(), visible: ko.observable(true) });
    this.Remetente = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Remetente.getFieldDescription(), visible: ko.observable(true) });
    this.Motoristas = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.Motoristass.getFieldDescription(), getType: typesKnockout.decimal, visible: ko.observable(true), visible: ko.observable(false) });
    this.Veiculo = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.Veiculo.getFieldDescription(), visible: ko.observable(true) });
    this.DataPrevisao = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.DataPrevisaoSaida.getFieldDescription(), visible: ko.observable(true) });
    this.DataColeta = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.DataColeta.getFieldDescription(), visible: ko.observable(true) });
    this.MotivoCancelamentoWS = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.MotivoCancelamentoWS.getFieldDescription(), visible: ko.observable(true) });
    this.DataAgendamento = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.DataAgendamento.getFieldDescription(), visible: ko.observable(true) });
    this.NumeroTransporte = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.NumeroTransporteDoisPontos.getFieldDescription(), visible: ko.observable(true) })
    this.DataInclusaoPedido = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.DataInclusaoPedido.getFieldDescription(), visible: ko.observable(true) });
    this.DataCriacaoPedidoERP = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.DataCriacaoPedidoERP.getFieldDescription(), visible: ko.observable(_CONFIGURACAO_TMS.PermiteReceberDataCriacaoPedidoERP) });
    this.MotivoCancelamento = PropertyEntity({ text: "Motivo Cancelamento:", enable: ko.observable(false), visible: ko.observable(false), getType: typesKnockout.string, val: ko.observable("") });
};


//*******EVENTOS*******

function loadResumoPedido() {
    _resumoPedido = new ResumoPedido();
    KoBindings(_resumoPedido, "knockoutResumoPedido");
}

//*******MÉTODOS*******

function preecherResumoPedido(dadosResumo) {
    _resumoPedido.NumeroPedido.visible(true);

    PreencherObjetoKnout(_resumoPedido, { Data: dadosResumo });

    _resumoPedido.MotivoCancelamentoWS.visible(_resumoPedido.MotivoCancelamentoWS.val() != "");

    if (_CONFIGURACAO_TMS.TelaPedidosResumido) {
        _resumoPedido.DataPrevisao.visible(false);
        _resumoPedido.DataColeta.visible(false);
    }
    
    document.querySelector('.bolinha').style.backgroundColor = _resumoPedido.SituacaoComercialPedidoBolinha.val();
}

function limparResumoPedido() {
    _resumoPedido.NumeroPedido.visible(false);

    LimparCampos(_resumoPedido);
}