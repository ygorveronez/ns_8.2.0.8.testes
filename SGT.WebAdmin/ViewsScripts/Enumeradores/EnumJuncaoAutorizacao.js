var EnumJuncaoAutorizacaoHelper = function () {
    this.E = 1;
    this.Ou = 2;
}

EnumJuncaoAutorizacaoHelper.prototype = {
    obterDescricao: function (valor) {
        switch (valor) {
            case this.E: return "E (Todas verdadeiras)";
            case this.Ou: return "Ou (Apenas uma verdadeira)";
            default: return "";
        }
    },
    obterOpcoes: function () {
        return [
            { text: this.obterDescricao(this.E), value: this.E },
            { text: this.obterDescricao(this.Ou), value: this.Ou }
        ];
    }
}

var EnumJuncaoAutorizacao = Object.freeze(new EnumJuncaoAutorizacaoHelper());