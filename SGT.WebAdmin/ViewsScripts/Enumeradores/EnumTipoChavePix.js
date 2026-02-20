var EnumTipoChavePixHelper = function () {
    this.Nenhum = "";
    this.CPFCNPJ = 1;
    this.Email = 2;
    this.Celular = 3;
    this.Aleatoria = 4;
};

EnumTipoChavePixHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoChavePix.CPFCNPJ, value: this.CPFCNPJ },
            { text: Localization.Resources.Enumeradores.TipoChavePix.Email, value: this.Email },
            { text: Localization.Resources.Enumeradores.TipoChavePix.Celular, value: this.Celular },
            { text: Localization.Resources.Enumeradores.TipoChavePix.Aleatoria, value: this.Aleatoria }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resouces.Enumeradores.TipoChavePix.Nenhum, value: this.Nenhum }].concat(this.obterOpcoes());
    },
    obterOpcoesComVazio: function () {
        return [{ text: "", value: this.Nenhum }].concat(this.obterOpcoes());
    }
};

var EnumTipoChavePix = Object.freeze(new EnumTipoChavePixHelper());