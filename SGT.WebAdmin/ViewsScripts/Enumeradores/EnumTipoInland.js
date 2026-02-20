var EnumTipoInlandHelper = function () {
    this.Todos = -1;
    this.NaoDefinido = 0;
    this.Rodoviario = 1;
    this.Ferroviario = 2;
    this.Fluvial = 3;
};

EnumTipoInlandHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Não Definido", value: this.NaoDefinido },
            { text: "Rodoviário", value: this.Rodoviario },
            { text: "Ferroviário", value: this.Ferroviario },
            { text: "Fluvial", value: this.Fluvial }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoInland = Object.freeze(new EnumTipoInlandHelper());