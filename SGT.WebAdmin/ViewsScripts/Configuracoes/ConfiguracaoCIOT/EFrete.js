/// <reference path="../../Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../Enumeradores/EnumTipoPagamentoeFrete.js" />

var _configuracaoEFrete = null;

var ConfiguracaoEFrete = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoIntegradorEFrete = PropertyEntity({ text: "Código do integrador:", maxlength: 200 });
    this.UsuarioEFrete = PropertyEntity({ text: "Usuário:", maxlength: 200 });
    this.SenhaEFrete = PropertyEntity({ text: "Senha:", maxlength: 200 });
    this.CodigoTipoCarga = PropertyEntity({ text: "Código Tipo Carga:", getType: typesKnockout.int });
    this.MatrizEFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Matriz:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.EmissaoGratuita = PropertyEntity({ text: "Emissão Gratuita: ", val: ko.observable(EnumSimNaoPesquisa.Nao), options: EnumSimNaoPesquisa.obterOpcoes(), def: EnumSimNaoPesquisa.Nao });
    this.TipoPagamento = PropertyEntity({ text: "Tipo de Pagamento: ", val: ko.observable(EnumTipoPagamentoeFrete.NaoSelecionado), options: EnumTipoPagamentoeFrete.obterOpcoes(), def: EnumTipoPagamentoeFrete.NaoSelecionado });
}
    function LoadConfiguracaoEFrete() {
    _configuracaoEFrete = new ConfiguracaoEFrete();
    KoBindings(_configuracaoEFrete, "tabEFrete");

    new BuscarTransportadores(_configuracaoEFrete.MatrizEFrete, null, null, true);
}

function LimparCamposConfiguracaoEFrete() {
    LimparCampos(_configuracaoEFrete);
}