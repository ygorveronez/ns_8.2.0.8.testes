var EnumFormaAverbacaoCTEHelper = function () {
    this.Definitiva = 0;
    this.Provisoria = 1;
};

EnumFormaAverbacaoCTEHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.FormaAverbacaoCTE.Definitiva, value: this.Definitiva },
            { text: Localization.Resources.Enumeradores.FormaAverbacaoCTE.Provisoria, value: this.Provisoria }
        ];
    },
}

var EnumFormaAverbacaoCTE = Object.freeze(new EnumFormaAverbacaoCTEHelper());