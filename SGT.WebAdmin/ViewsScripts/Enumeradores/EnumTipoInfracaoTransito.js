var EnumTipoInfracaoTransitoHelper = function () {
    this.Todos = "";
    this.Multa = 0;
    this.Outro = 1;
    this.Sinistro = 2;
    this.Advertencia = 3;
}

EnumTipoInfracaoTransitoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Multa", value: this.Multa },
            { text: "Outro", value: this.Outro },
            { text: "Sinistro", value: this.Sinistro },
            { text: "Advertência", value: this.Advertencia },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumTipoInfracaoTransito = Object.freeze(new EnumTipoInfracaoTransitoHelper());