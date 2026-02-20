var EnumTipoManutencaoPalletHelper = function () {
    this.Todos = 0;
    this.Disponivel = 1;
    this.Descarte = 2;
    this.Sucata = 3;
};

EnumTipoManutencaoPalletHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Disponível para uso", value: this.Disponivel },
            { text: "Descarte", value: this.Descarte },
            { text: "Sucata", value: this.Sucata }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    },
    obterOpcoesCadastro: function () {
        return [{ text: "", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoManutencaoPallet = Object.freeze(new EnumTipoManutencaoPalletHelper());