var EnumTipoFilialHelper = function () {
    this.Filial = 1;
    this.Matriz = 2;
    this.Agencia = 3;
};

EnumTipoFilialHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoFilial.Filial, value: this.Filial },
            { text: Localization.Resources.Enumeradores.TipoFilial.Matriz, value: this.Matriz },
            { text: Localization.Resources.Enumeradores.TipoFilial.Agencia, value: this.Agencia }
        ];
    },
};

var EnumTipoFilial = Object.freeze(new EnumTipoFilialHelper());