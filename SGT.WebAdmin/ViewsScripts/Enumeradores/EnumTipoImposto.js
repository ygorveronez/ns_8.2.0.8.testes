var EnumTipoImpostoHelper = function () {
    this.Nenhum = 0;
    this.CBS = 1;
    this.IBS = 2;
}

EnumTipoImpostoHelper.prototype = {
    obterTodos: function () {
        return [
            this.CBS,
            this.IBS
        ];
    },
    obterOpcoes: function () {
        return [
            { text: "CBS", value: this.CBS },
            { text: "IBS", value: this.IBS }
        ];
    },
}

var EnumTipoImposto = Object.freeze(new EnumTipoImpostoHelper());


