var EnumStatusCCeHelper = function () {
    this.EmDigitacao = 0;
    this.Pendente = 1;
    this.Enviado = 2;
    this.Autorizado = 3;
    this.Rejeicao = 9;
};

EnumStatusCCeHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "Em Digitação", value: this.EmDigitacao },
            { text: "Pendente", value: this.Pendente },
            { text: "Enviado", value: this.Enviado },
            { text: "Autorizado", value: this.Autorizado },
            { text: "Rejeição", value: this.Rejeicao }
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: "" }].concat(this.ObterOpcoes());
    }
};

var EnumStatusCCe = Object.freeze(new EnumStatusCCeHelper());