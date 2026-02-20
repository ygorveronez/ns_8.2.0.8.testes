var EnumTipoOfertaHUBHelper = function () {
    this.Exclusiva = 0;
    this.Adiciona = 1;
};

EnumTipoOfertaHUBHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Exclusiva", value: this.Exclusiva },
            { text: "Adiciona", value: this.Adiciona }
        ];
    }
};

var EnumTipoOfertaHUB = Object.freeze(new EnumTipoOfertaHUBHelper());