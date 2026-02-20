var _configuracaoA52;

var ConfiguracaoA52 = function () {
    this.CodigoIntegracaoA52 = PropertyEntity({ text: "Código de Integração:", val: ko.observable(""), def: "", getType: typesKnockout.int, maxlength: 3 });
};

function LoadConfiguracaoA52() {
    _configuracaoA52 = new ConfiguracaoA52();
    KoBindings(_configuracaoA52, "tabA52");

    _tecnologiaRastreador.CodigoIntegracaoA52 = _configuracaoA52.CodigoIntegracaoA52;
}