const EnumTipoPacoteHelper = function () {
    this.PorFaixaPacote = 1;
    this.ValorFixoPorPacote = 2;

};

EnumTipoPacoteHelper.prototype = {
    ObterOpcoes: function (opcaoSelecione) {
        let arrayOpcoes = [
            { text: "Por Faixa de Pacote", value: this.PorFaixaPacote },
            { text: "Valor Fixo Por Pacote", value: this.ValorFixoPorPacote },
        ];
        return arrayOpcoes;
    }
};

let EnumTipoPacote = Object.freeze(new EnumTipoPacoteHelper());