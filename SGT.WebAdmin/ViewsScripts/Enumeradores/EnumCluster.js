var EnumClusterHelper = function () {
    this.Brasil = 1;
};

EnumClusterHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "Brasil", value: this.Brasil },
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: "" }].concat(this.ObterOpcoes());
    }
}

var EnumCluster = Object.freeze(new EnumClusterHelper());