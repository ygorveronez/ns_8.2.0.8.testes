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
/// <reference path="CotacaoPedido.js" />
/// <reference path="../../Enumeradores/EnumSituacaoSolicitacaoCredito.js" />
/// <reference path="Etapa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pedidoAutorizacao;
var _gridAutorizacoes;

var CotacaoPedidoAutorizacao = function () {

    this.SituacaoSolicitacao = PropertyEntity({ visible: ko.observable(false) });
    this.DescricaoSituacao = PropertyEntity({ visible: ko.observable(true) });
    this.DataEmissao = PropertyEntity({ text: "Data da Emissão: ", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.Solicitado = PropertyEntity({ text: "Solicitante:", visible: ko.observable(true) });    
    this.Peso = PropertyEntity({ text: "Peso:", visible: ko.observable(true) });
    this.QtdEntregas = PropertyEntity({ text: "Qtd. Entregas:", visible: ko.observable(true) });
    this.TipoCarga = PropertyEntity({ text: "Tipo da Carga:", visible: ko.observable(true) });
    this.ModeloVeicular = PropertyEntity({ text: "Modelo Veícular:", visible: ko.observable(true) });
    this.Cliente = PropertyEntity({ text: "Cliente:", visible: ko.observable(true) });    
    this.ValorFrete = PropertyEntity({ text: "Valor Frete: ", getType: typesKnockout.decimal, visible: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação:", visible: ko.observable(true) });


    this.DataRetorno = PropertyEntity({ text: "Data do Retorno: ", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.Creditor = PropertyEntity({ text: "Supervisor:", visible: ko.observable(true) });
    this.ValorLiberado = PropertyEntity({ text: "Valor Aprovado: ", getType: typesKnockout.decimal, visible: ko.observable(true) });
    this.RetornoSolicitacao = PropertyEntity({ type: types.map, maxlength: 300, text: "Resposta:", visible: ko.observable(true) });

    this.Autorizacao = PropertyEntity({ visible: ko.observable(false) });

    this.UsuariosAutorizadores = PropertyEntity({ idGrid: guid(), visible: ko.observable(true) });

    // Mensagem da etapa
    this.MensagemEtapaSemRegra = PropertyEntity({ type: types.local, visible: ko.observable(false), title: "Autorização Pendente", text: '<b class="margin-bottom-10" style="display:block">Nenhuma regra encontrada.</b><b>Pagamento permanece aguardando autorização.</b>' });
}

//*******EVENTOS*******

function loadCotacaoPedidoAutorizacao() {
    _pedidoAutorizacao = new CotacaoPedidoAutorizacao();
    KoBindings(_pedidoAutorizacao, "knockoutAprovacaoEtapa");

    _gridAutorizacoes = new GridView(_pedidoAutorizacao.UsuariosAutorizadores.idGrid, "CotacaoPedido/ConsultarAutorizacoes", _cotacaoPedido, null, null, null, null, null, null, null);
}

//*******MÉTODOS*******

function preecherCotacaoPedidoAutorizacaoKnout(knout, grid, dadosAutorizacao) {
    if (dadosAutorizacao !== null) {
        knout.UsuariosAutorizadores.visible(false);
        knout.DescricaoSituacao.visible(true);
        knout.SituacaoSolicitacao.visible(true);
        knout.Autorizacao.visible(false);
        var data = { Data: dadosAutorizacao };
        PreencherObjetoKnout(knout, data);

        if (dadosAutorizacao.SituacaoSolicitacao === EnumSituacaoSolicitacaoCredito.AgLiberacao || dadosAutorizacao.SituacaoSolicitacao === EnumSituacaoSolicitacaoCredito.AutorizacaoPendente || !dadosAutorizacao.ComRegraAutorizacao) {
            knout.DataRetorno.visible(false);
            knout.Creditor.visible(false);
            knout.ValorLiberado.visible(false);
            knout.RetornoSolicitacao.visible(false);
        }

        if (dadosAutorizacao.SituacaoSolicitacao === EnumSituacaoSolicitacaoCredito.Rejeitado) {
            knout.ValorLiberado.visible(false);
        }

        if (dadosAutorizacao.SituacaoSolicitacao === EnumSituacaoSolicitacaoCredito.Todos) {
            knout.DataRetorno.visible(false);
            knout.Creditor.visible(false);
            knout.ValorLiberado.visible(false);
            knout.RetornoSolicitacao.visible(false);
            knout.DescricaoSituacao.visible(false);
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

function preecherCotacaoPedidoAutorizacao(dadosAutorizacao) {
    preecherCotacaoPedidoAutorizacaoKnout(_pedidoAutorizacao, _gridAutorizacoes, dadosAutorizacao);
}

function limparCotacaoPedidoAutorizacaoKnout(knout) {
    knout.DataRetorno.visible(true);
    knout.Creditor.visible(true);
    knout.RetornoSolicitacao.visible(true);
    knout.ValorLiberado.visible(true);
    knout.SituacaoSolicitacao.visible(false);
}

function limparCotacaoPedidoAutorizacao() {
    limparCotacaoPedidoAutorizacaoKnout(_pedidoAutorizacao);
    LimparCampos(_pedidoAutorizacao);
}