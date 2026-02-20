var EnumPamcardParcelaStatusHelper = function () {
    this.Pendente = 1;
    this.Liberada = 2;
}

EnumPamcardParcelaStatusHelper.prototype = {
    obterOpcoes: function (defaultOption) {
        var options = [
            { text: "Pendente", value: this.Pendente },
            { text: "Liberada", value: this.Liberada }
        ];

        if (defaultOption != null)
            return [defaultOption].concat(options);
        else
            return options;
    }
}

var EnumPamcardParcelaStatus = Object.freeze(new EnumPamcardParcelaStatusHelper());