let EnumResponsavelPalletHelper = function () {
    this.Todos = ""
    this.Cliente = 1,
    this.Transportador = 2,
    this.Filial = 3
};

EnumResponsavelPalletHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Cliente", value: this.Cliente },
            { text: "Transportador", value: this.Transportador },
            { text: "Filial", value: this.Filial },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    },

    obterOpcoesPesquisaAgendamento: function () {
        return [{ text: "Todos", value: this.Todos }].concat(
            this.obterOpcoes().filter(opcao => opcao.value !== this.Filial)
        );
    },
};

let EnumResponsavelPallet = Object.freeze(new EnumResponsavelPalletHelper());