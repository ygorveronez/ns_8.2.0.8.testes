var EnumTipoCarretaHelper = function () {
    this.Todos = "";
    this.NaoDefinido = 0;
    this.Lisa = 1;
    this.Gancheira = 2;
};

EnumTipoCarretaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Não Definido", value: this.NaoDefinido },
            { text: "Lisa", value: this.Lisa },
            { text: "Gancheira", value: this.Gancheira }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoCarreta = Object.freeze(new EnumTipoCarretaHelper());