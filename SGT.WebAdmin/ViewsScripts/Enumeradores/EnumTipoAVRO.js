var EnumTipoAVROHelper = function () {
    this.v1 = 1;
    this.v2 = 2;
};

EnumTipoAVROHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "V1", value: this.v1 },
            { text: "V2", value: this.v2 },
        ];
    },
};

var EnumTipoAVRO = Object.freeze(new EnumTipoAVROHelper);