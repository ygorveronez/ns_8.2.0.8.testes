var EnumTipoReciboHelper = function () {
    this.Padrao = 0;
    this.Completo = 1;
}

EnumTipoReciboHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Padrão", value: this.Padrao },
            { text: "Completo", value: this.Completo }
        ];
    }
}

var EnumTipoRecibo = Object.freeze(new EnumTipoReciboHelper());