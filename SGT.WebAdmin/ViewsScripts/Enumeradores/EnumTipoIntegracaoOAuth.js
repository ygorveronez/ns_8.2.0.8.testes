var EnumTipoIntegracaoOAuthHelper = function () {
    this.OAuth1_0 = 1;
    this.OAuth2_0 = 2;
};

EnumTipoIntegracaoOAuthHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "OAuth 1.0", value: this.OAuth1_0 },
            { text: "OAuth 2.0", value: this.OAuth2_0 },
        ];
    },
};

var EnumTipoIntegracaoOAuth = Object.freeze(new EnumTipoIntegracaoOAuthHelper);