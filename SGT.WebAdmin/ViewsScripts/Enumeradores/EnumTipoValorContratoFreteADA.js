var EnumTipoValorContratoFreteADAHelper = function () {
    this.Todos = "";
    this.Fixo = 1;
    this.Calculado = 2;
};

EnumTipoValorContratoFreteADAHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Fixo", value: this.Fixo },
            { text: "Calculado", value: this.Calculado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoValorContratoFreteADA = Object.freeze(new EnumTipoValorContratoFreteADAHelper());