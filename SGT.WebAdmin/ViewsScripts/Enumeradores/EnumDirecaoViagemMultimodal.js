var EnumDirecaoViagemMultimodalHelper = function () {
    this.Todos = 0;
    this.Norte = 1;
    this.Sul = 2;
    this.Leste  = 3;
    this.Oeste  = 4;
}

EnumDirecaoViagemMultimodalHelper.prototype = {
    obterDescricao: function (valor) {
        switch (valor) {
            case this.Norte: return "Norte";
            case this.Sul: return "Sul";
            case this.Leste: return "Leste";
            case this.Oeste: return "Oeste";
            case this.Todos: return "Todos";
            default: return "";
        }
    },
    obterOpcoes: function () {
        return [
            { text: "Norte", value: this.Norte },
            { text: "Sul", value: this.Sul },
            { text: "Leste", value: this.Leste },
            { text: "Oeste", value: this.Oeste }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumDirecaoViagemMultimodal = Object.freeze(new EnumDirecaoViagemMultimodalHelper());