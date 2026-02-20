var EnumPaisesHelper = function () {
    this.Brasil = 0;
    this.Exterior = 1;
}

EnumPaisesHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Brasil", value: this.Brasil },
            { text: "Exterior", value: this.Exterior }
        ];
    }
}

var EnumPaises = Object.freeze(new EnumPaisesHelper());