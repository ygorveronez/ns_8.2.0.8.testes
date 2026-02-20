var EnumSimNaoHelper = function () {
    this.Nao = 0;
    this.Sim = 1;
};

EnumSimNaoHelper.prototype = {
    obterDescricao: function (valor) {
        switch (valor) {
            case this.Nao: return Localization.Resources.Enumeradores.SimNao.Nao;
            case this.Sim: return Localization.Resources.Enumeradores.SimNao.Sim;
            default: return "";
        }
    },
    obterOpcoes: function () {
        return [
            { text: this.obterDescricao(this.Nao), value: this.Nao },
            { text: this.obterDescricao(this.Sim), value: this.Sim },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [
            { text: this.obterDescricao(this.Sim), value: this.Sim },
            { text: this.obterDescricao(this.Nao), value: this.Nao },
            { text: "Todos", value: null },
        ];
    }
};

var EnumSimNao = Object.freeze(new EnumSimNaoHelper());