var EnumTipoChegadaGuaritaHelper = function () {
    this.Todos = "";
    this.Carregamento = 1;
    this.Descarregamento = 2;
};

EnumTipoChegadaGuaritaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Carregamento", value: this.Carregamento },
            { text: "Descarregamento", value: this.Descarregamento }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoChegadaGuarita = Object.freeze(new EnumTipoChegadaGuaritaHelper());
