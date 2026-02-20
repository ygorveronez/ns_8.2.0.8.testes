var EnumTipoRotaFreteRepomHelper = function () {
    this.NaoEspecificado = 0;
    this.Ida = 1;
    this.IdaVolta = 2;
};

EnumTipoRotaFreteRepomHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Não Especificado (irá usar da carga)", value: this.NaoEspecificado },
            { text: "Somente Ida", value: this.Ida },
            { text: "Ida e Volta", value: this.IdaVolta }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoRotaFreteRepom = Object.freeze(new EnumTipoRotaFreteRepomHelper());