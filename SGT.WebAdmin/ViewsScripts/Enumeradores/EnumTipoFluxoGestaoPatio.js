var EnumTipoFluxoGestaoPatioHelper = function () {
    this.Todos = "";
    this.Origem = 1;
    this.Destino = 2;
};

EnumTipoFluxoGestaoPatioHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Destino", value: this.Destino },
            { text: "Origem", value: this.Origem }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoFluxoGestaoPatio = Object.freeze(new EnumTipoFluxoGestaoPatioHelper());
