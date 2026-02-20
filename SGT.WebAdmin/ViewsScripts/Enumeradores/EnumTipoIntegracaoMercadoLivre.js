var EnumTipoIntegracaoMercadoLivreHelper = function () {
    this.HandlingUnit = 0;
    this.Dispatch = 1;
    this.RotaEFacility = 2;
};

EnumTipoIntegracaoMercadoLivreHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Handling Unit", value: this.HandlingUnit },
            { text: "Dispatch", value: this.Dispatch },
            { text: "Rota e Facility", value: this.RotaEFacility }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: "" }].concat(this.obterOpcoes());
    }
};

var EnumTipoIntegracaoMercadoLivre = Object.freeze(new EnumTipoIntegracaoMercadoLivreHelper());