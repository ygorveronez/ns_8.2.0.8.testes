var EnumTipoEnvolvidoSinistroHelper = function () {
    this.Todos = 0;
    this.Proprio = 1;
    this.Terceiro = 2;
};

EnumTipoEnvolvidoSinistroHelper.prototype = {
    obterOpcoes: function () {
        return [
            {
                value: this.Proprio,
                text: "Próprio"
            },
            {
                value: this.Terceiro,
                text: "Terceiro"
            }
        ];
    },
    obterDescricao: function (tipo) {
        switch (tipo) {
            case this.Proprio:
                return "Próprio";
            case this.Terceiro:
                return "Terceiro";
            default:
                return "";
        };
    }
}

var EnumTipoEnvolvidoSinistro = Object.freeze(new EnumTipoEnvolvidoSinistroHelper());
