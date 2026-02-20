var EnumTipoAjusteValorHelper = function () {
    this.Todos = "";
    this.Desconto = 1;
    this.Acrescimo = 2;
};

EnumTipoAjusteValorHelper.prototype = {
    obterDescricao: function (valor) {
        switch (valor) {
            case this.Acrescimo: return "Acréscimo";
            case this.Desconto: return "Desconto";
            default: return "";
        }
    },
    obterOpcoes: function () {
        return [
            { text: this.obterDescricao(this.Acrescimo), value: this.Acrescimo },
            { text: this.obterDescricao(this.Desconto), value: this.Desconto }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoAjusteValor = Object.freeze(new EnumTipoAjusteValorHelper());
