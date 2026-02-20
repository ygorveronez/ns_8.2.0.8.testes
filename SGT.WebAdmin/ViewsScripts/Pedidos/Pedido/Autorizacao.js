/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumSituacaoPedido.js" />
/// <reference path="Pedido.js" />
/// <reference path="../../Enumeradores/EnumSituacaoSolicitacaoCredito.js" />
/// <reference path="Etapa.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _pedidoAutorizacao;
var _gridAutorizacoes;

var PedidoAutorizacao = function () {

    this.SituacaoSolicitacao = PropertyEntity({ visible: ko.observable(false) });
    this.DescricaoSituacao = PropertyEntity({ visible: ko.observable(true) });
    this.DataEmissao = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.DataEmissao.getFieldDescription(), getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.Solicitado = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.Solicitante.getFieldDescription(), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.Motoristass.getFieldDescription(), visible: ko.observable(true) });
    this.Peso = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.Peso.getFieldDescription(), visible: ko.observable(true) });
    this.QtdEntregas = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.QtdEntregas.getFieldDescription(), visible: ko.observable(true) });
    this.TipoCarga = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.TipoCarga.getFieldDescription(), visible: ko.observable(true) });
    this.ModeloVeicular = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.ModeloVeicular.getFieldDescription(), visible: ko.observable(true) });
    this.Remetente = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Remetente.getFieldDescription(), visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Destinatario.getFieldDescription(), visible: ko.observable(true) });
    this.ValorNegociado = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.ValorNegociado.getFieldDescription(), getType: typesKnockout.decimal, visible: ko.observable(true) });
    this.ValorFrete = PropertyEntity({ text: Localization.Resources.Gerais.Geral.ValorFrete.getFieldDescription(), getType: typesKnockout.decimal, visible: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Observacao.getFieldDescription(), visible: ko.observable(true) });


    this.DataRetorno = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.DataRetorno.getFieldDescription(), getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.Creditor = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.Supervisor.getFieldDescription(), visible: ko.observable(true) });
    this.ValorLiberado = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.ValorAprovado.getFieldDescription(), getType: typesKnockout.decimal, visible: ko.observable(true) });
    this.RetornoSolicitacao = PropertyEntity({ type: types.map, maxlength: 300, text: Localization.Resources.Pedidos.Pedido.Resposta.getFieldDescription(), visible: ko.observable(true) });

    this.Autorizacao = PropertyEntity({ visible: ko.observable(false) });
    
    this.UsuariosAutorizadores = PropertyEntity({ idGrid: guid(), visible: ko.observable(true) });

    // Mensagem da etapa
    this.MensagemEtapaSemRegra = PropertyEntity({ type: types.local, visible: ko.observable(false), title: Localization.Resources.Pedidos.Pedido.AutorizacaoPendente, text: Localization.Resources.Pedidos.Pedido.NenhumaRegraEncontrada});
}

//*******EVENTOS*******

function loadPedidoAutorizacao() {
    _pedidoAutorizacao = new PedidoAutorizacao();
    KoBindings(_pedidoAutorizacao, "knockoutAprovacaoEtapa");

    _gridAutorizacoes = new GridView(_pedidoAutorizacao.UsuariosAutorizadores.idGrid, "Pedido/ConsultarAutorizacoes", _pedido, null, null, null, null, null, null, null);
}

//*******MÉTODOS*******

function preecherPedidoAutorizacaoKnout(knout, grid, dadosAutorizacao) {
    if (dadosAutorizacao != null) {
        knout.UsuariosAutorizadores.visible(false);
        knout.DescricaoSituacao.visible(true)
        knout.SituacaoSolicitacao.visible(true);
        knout.Autorizacao.visible(false);
        var data = { Data: dadosAutorizacao };
        PreencherObjetoKnout(knout, data);

        if (dadosAutorizacao.SituacaoSolicitacao == EnumSituacaoSolicitacaoCredito.AgLiberacao || dadosAutorizacao.SituacaoSolicitacao == EnumSituacaoSolicitacaoCredito.AutorizacaoPendente || !dadosAutorizacao.ComRegraAutorizacao) {
            knout.DataRetorno.visible(false);
            knout.Creditor.visible(false);
            knout.ValorLiberado.visible(false);
            knout.RetornoSolicitacao.visible(false);
        }

        if (dadosAutorizacao.SituacaoSolicitacao == EnumSituacaoSolicitacaoCredito.Liberado) {
            //knout.ConfirmarPagamentoMotorista.visible(false);
        }

        if (dadosAutorizacao.SituacaoSolicitacao == EnumSituacaoSolicitacaoCredito.Utilizado) {
            //knout.ConfirmarPagamentoMotorista.visible(false);
        }

        if (dadosAutorizacao.SituacaoSolicitacao == EnumSituacaoSolicitacaoCredito.Rejeitado) {
            knout.ValorLiberado.visible(false);
        }

        if (dadosAutorizacao.SituacaoSolicitacao == EnumSituacaoSolicitacaoCredito.Todos) {
            knout.DataRetorno.visible(false);
            knout.Creditor.visible(false);
            knout.ValorLiberado.visible(false);
            knout.RetornoSolicitacao.visible(false);
            knout.DescricaoSituacao.visible(false)
        }
    } else {
        knout.UsuariosAutorizadores.visible(false);
        knout.SituacaoSolicitacao.visible(false);
        knout.Autorizacao.visible(true);
    }

    grid.CarregarGrid(function () {
        if (grid.NumeroRegistros() > 0) {
            knout.UsuariosAutorizadores.visible(true);
            knout.Autorizacao.visible(false);
        }
    });
}

function preecherPedidoAutorizacao(dadosAutorizacao) {
    preecherPedidoAutorizacaoKnout(_pedidoAutorizacao, _gridAutorizacoes, dadosAutorizacao);
}

function limparPedidoAutorizacaoKnout(knout) {
    knout.DataRetorno.visible(true);
    knout.Creditor.visible(true);
    knout.RetornoSolicitacao.visible(true);
    knout.ValorLiberado.visible(true);
    knout.SituacaoSolicitacao.visible(false);
}

function limparPedidoAutorizacao() {
    limparPedidoAutorizacaoKnout(_pedidoAutorizacao);
    LimparCampos(_pedidoAutorizacao);
}