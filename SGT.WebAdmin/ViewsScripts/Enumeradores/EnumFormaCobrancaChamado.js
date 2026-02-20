var EnumFormaCobrancaChamadoHelper = function () {
    this.Todos = 0;
    this.Caixa = 1;
    this.Chapa = 2;
    this.Pallet = 3;
    this.Peso = 4;
    this.ValorFixo = 5;
};

EnumFormaCobrancaChamadoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Caixas (UN)", value: this.Caixa },
            { text: "Chapas (UN)", value: this.Chapa },
            { text: "Pallets (UN)", value: this.Pallet },
            { text: "Peso (TON)", value: this.Peso },
            { text: "Valor Fixo", value: this.ValorFixo }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumFormaCobrancaChamado = Object.freeze(new EnumFormaCobrancaChamadoHelper());