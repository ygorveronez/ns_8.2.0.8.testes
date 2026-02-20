var EnumTipoPropriedadeContainerHelper = function () {
    this.Todos = "";
    this.Proprio = 1;
    this.Soc = 2;
};

EnumTipoPropriedadeContainerHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Próprio", value: this.Proprio },
            { text: "SOC", value: this.Soc }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoPropriedadeContainer = Object.freeze(new EnumTipoPropriedadeContainerHelper());