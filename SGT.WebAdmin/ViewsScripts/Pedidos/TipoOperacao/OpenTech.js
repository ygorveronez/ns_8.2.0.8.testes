var _configuracaoOpenTech;

var ConfiguracaOpenTech = function () {
    this.HabilitarOutraConfiguracaoOpenTech = PropertyEntity({ text: "Especificar outra configuração de integração para esse tipo de Operação", val: ko.observable(false), def: false, visible: ko.observable(true), getType: typesKnockout.bool });
    this.UsuarioOpenTech = PropertyEntity({ text: "Usuário:", maxlength: 50, visible: ko.observable(true), required: false });
    this.SenhaOpenTech = PropertyEntity({ text: "Senha:", maxlength: 50, visible: ko.observable(true), required: false });
    this.DominioOpenTech = PropertyEntity({ text: "Domínio:", maxlength: 50, visible: ko.observable(true), required: false });
    this.CodigoClienteOpenTech = PropertyEntity({ text: "Código do cliente:", getType: typesKnockout.int, visible: ko.observable(true), required: false });
    this.CodigoPASOpenTech = PropertyEntity({ text: "Código PAS:", getType: typesKnockout.int, visible: ko.observable(true), required: false });
    this.URLOpenTech = PropertyEntity({ text: "URL Web Service:", maxlength: 250, visible: ko.observable(true), required: false });
    this.CodigoProdutoPadraoOpentech = PropertyEntity({ text: "Código produto padrão (Quando não tiver grupo de produto, na etapa de integração)", getType: typesKnockout.int, visible: ko.observable(true), required: false });
    this.CodigoTransportadorOpenTech = PropertyEntity({ text: "Código do transportador", required: false, getType: typesKnockout.int, val: ko.observable("") });
};
function LoadConfiguracaoOpenTech() {
    _configuracaoOpenTech = new ConfiguracaOpenTech();
    KoBindings(_configuracaoOpenTech, "tabOpenTech");

    _tipoOperacao.HabilitarOutraConfiguracaoOpenTech = _configuracaoOpenTech.HabilitarOutraConfiguracaoOpenTech;
    _tipoOperacao.UsuarioOpenTech = _configuracaoOpenTech.UsuarioOpenTech;
    _tipoOperacao.SenhaOpenTech = _configuracaoOpenTech.SenhaOpenTech;
    _tipoOperacao.DominioOpenTech = _configuracaoOpenTech.DominioOpenTech;
    _tipoOperacao.CodigoClienteOpenTech = _configuracaoOpenTech.CodigoClienteOpenTech;
    _tipoOperacao.CodigoPASOpenTech = _configuracaoOpenTech.CodigoPASOpenTech;
    _tipoOperacao.URLOpenTech = _configuracaoOpenTech.URLOpenTech;
    _tipoOperacao.CodigoProdutoPadraoOpentech = _configuracaoOpenTech.CodigoProdutoPadraoOpentech;
    _tipoOperacao.CodigoTransportadorOpenTech = _configuracaoOpenTech.CodigoTransportadorOpenTech;

}

