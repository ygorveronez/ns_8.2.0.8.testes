var _configuracaoRepomFrete = null;

var ConfiguracaoRepomFrete = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.URLRepomFrete = PropertyEntity({ text: "URL:", maxlength: 400 });
    this.UsuarioRepomFrete = PropertyEntity({ text: "Usuário:", maxlength: 150 });
    this.SenhaRepomFrete = PropertyEntity({ text: "Senha:", maxlength: 150 });
    this.PartnerRepomFrete = PropertyEntity({ text: "Partner:", maxlength: 150 });
    this.UtilizarMetodosValidacaoCadastros = PropertyEntity({ text: "Utilizar Métodos de Validação de Cadastros", val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.RealizarEncerramentoAutorizacaoPagamentoSeparado = PropertyEntity({ text: "Realizar Encerramento e Autorização de Pagamento Separado", val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.RealizarCompraValePedagioIntegracaoCIOT = PropertyEntity({ text: "Realizar compra do vale pedágio na integração do CIOT", val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.EnviarQuantidadesMaioresQueZeroRepomFrete = PropertyEntity({ text: "Enviar quantidades (peso, volumes e valor de mercadoria) sempre maiores que zero", val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.UsarDataPagamentoTransportadorTerceiro = PropertyEntity({ text: "Utilizar a data de autorização de pagamento configurado no cadastro do transportador terceiro", val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.UtilizarDataPrevisaoEntregaPedidoParaExpectativaPagamentoSaldo = PropertyEntity({ text: "Utilizar data de previsão entrega pedido para expectativa pagamento saldo", val: ko.observable(false), def: false, enable: ko.observable(true) });
};

function LoadConfiguracaoRepomFrete() {
    _configuracaoRepomFrete = new ConfiguracaoRepomFrete();
    KoBindings(_configuracaoRepomFrete, "tabRepomFrete");
}

function LimparCamposConfiguracaoRepomFrete() {
    LimparCampos(_configuracaoRepomFrete);
}