var EnumStatusControleMaritimoHelper = function () {
    this.Todas = "";
    this.Ativo = 0;
    this.Cancelado = 1;
};

EnumStatusControleMaritimoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Ativo", value: this.Ativo },
            { text: "Cancelado", value: this.Cancelado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todas }].concat(this.obterOpcoes());
    },
    obterDescricao: function (valor) {
        switch (valor) {
            case this.Ativo: return "Ativo";
            case this.Cancelado: return "Cancelado";
            default: return "";
        }
    }
}

var EnumStatusControleMaritimo = Object.freeze(new EnumStatusControleMaritimoHelper());