var EnumTipoRotaFreteDBTransHelper = function () {
    this.NaoEspecificado = 0;
    this.Ida = 1;
    this.IdaVolta = 2;
};

EnumTipoRotaFreteDBTransHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Não Especificado (conforme rota frete)", value: this.NaoEspecificado },
            { text: "Somente Ida", value: this.Ida },
            { text: "Ida e Volta", value: this.IdaVolta }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoRotaFreteDBTrans = Object.freeze(new EnumTipoRotaFreteDBTransHelper());