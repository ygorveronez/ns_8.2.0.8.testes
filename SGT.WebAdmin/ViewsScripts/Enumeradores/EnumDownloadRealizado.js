var EnumDownloadRealizadoHelper = function () {
    this.Todos = "";
    this.Pendente = 1;
    this.Realizado = 2;
};

EnumDownloadRealizadoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Pendente", value: this.Pendente },
            { text: "Realizado", value: this.Realizado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumDownloadRealizado = Object.freeze(new EnumDownloadRealizadoHelper());