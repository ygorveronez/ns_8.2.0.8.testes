var EnumPamcardParcelaTipoEfetivacaoHelper = function () {
    this.Manual = 1;
    this.Automatica = 2;
    this.Quitacao = 4;
}

EnumPamcardParcelaTipoEfetivacaoHelper.prototype = {
    obterOpcoes: function (defaultOption) {
        var options = [
            { text: "Manual", value: this.Manual },
            { text: "Automática", value: this.Automatica },
            { text: "Quitação", value: this.Quitacao }
        ];

        if (defaultOption != null)
            return [defaultOption].concat(options);
        else
            return options;
    }
}

var EnumPamcardParcelaTipoEfetivacao = Object.freeze(new EnumPamcardParcelaTipoEfetivacaoHelper());