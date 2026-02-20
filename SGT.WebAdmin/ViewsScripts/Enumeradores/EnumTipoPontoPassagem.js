var EnumTipoPontoPassagemHelper = function () {
    this.Coleta = 1;
    this.Entrega = 2;
    this.Pedagio = 3;
    this.Passagem = 4;
    this.Retorno = 5;
    this.Fronteira = 8;
    this.PostoFiscal = 10;
};

EnumTipoPontoPassagemHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Coleta", value: this.Coleta},
            { text: "Entrega", value: this.Entrega},
            { text: "Pedagio", value: this.Pedagio},
            { text: "Passagem", value: this.Passagem},
            { text: "Retorno", value: this.Retorno},
            { text: "Fronteira", value: this.Fronteira },
            { text: "Posto Fiscal", value: this.PostoFiscal }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumTipoPontoPassagem = Object.freeze(new EnumTipoPontoPassagemHelper());