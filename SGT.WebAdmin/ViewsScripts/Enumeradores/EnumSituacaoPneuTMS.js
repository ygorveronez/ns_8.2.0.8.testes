var EnumSituacaoPneuTMSHelper = function () {
    this.Todos = 0;
    this.Disponivel = 1;
    this.EmUso = 2;
    this.Reforma = 3;
    this.Sucata = 4;
};

EnumSituacaoPneuTMSHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Disponível", value: this.Disponivel },
            { text: "Em Uso", value: this.EmUso },
            { text: "Reforma", value: this.Reforma },
            { text: "Sucata", value: this.Sucata }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoPneuTMS = Object.freeze(new EnumSituacaoPneuTMSHelper());