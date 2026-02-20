var EnumTiposCargaTerceirosHelper = function () {
    this.Todas = 0;
    this.Terceiro = 1;
    this.Proprio = 2;

};

EnumTiposCargaTerceirosHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Todas", value: this.Todas },
            { text: "Terceiro", value: this.Terceiro },
            { text: "Próprio", value: this.Proprio }
        ];
    }
};

var EnumTiposCargaTerceiros = Object.freeze(new EnumTiposCargaTerceirosHelper());