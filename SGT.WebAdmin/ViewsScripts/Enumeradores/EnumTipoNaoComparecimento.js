var EnumTipoNaoComparecimentoHelper = function () {
    this.Todos = "";
    this.Compareceu = 0;
    this.NaoCompareceu = 1;
    this.NaoCompareceuComFalha = 2;
};

EnumTipoNaoComparecimentoHelper.prototype = {
    obterDescricao: function (tipo) {
        switch (tipo) {
            case this.Compareceu: return "Compareceu";
            case this.NaoCompareceu: return "No-show";
            case this.NaoCompareceuComFalha: return "Falha";
            default: return "";
        }
    },
    obterOpcoes: function () {
        return [
            { text: this.obterDescricao(this.Compareceu), value: this.Compareceu },
            { text: this.obterDescricao(this.NaoCompareceuComFalha), value: this.NaoCompareceuComFalha },
            { text: this.obterDescricao(this.NaoCompareceu), value: this.NaoCompareceu }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoNaoComparecimento = Object.freeze(new EnumTipoNaoComparecimentoHelper());
