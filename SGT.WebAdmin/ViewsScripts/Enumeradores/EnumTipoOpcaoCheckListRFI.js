let EnumTipoOpcaoCheckListRFIHelper = function () {
    this.SimNao = 1;
    this.Opcoes = 2;
};

EnumTipoOpcaoCheckListRFIHelper.prototype = {
    obterOpcoes: function () {
        let opcoes = [];

        opcoes.push({ text: "Sim e Não", value: this.SimNao });
        opcoes.push({ text: "Opções (Múltiplo)", value: this.Opcoes });

        return opcoes;
    },

    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: null }].concat(this.obterOpcoes());
    },

    obterValor: function (valor) {
        switch (valor) {
            case "Sim e Não": return this.SimNao;
            case "Opções (Múltiplo)": return this.Opcoes;
        }
    },

    obterDescricao: function (valor) {
        switch (valor) {
            case this.SimNao: return "Sim e Não";
            case this.Opcoes: return "Opções (Múltiplo)";
        }
    },
};

let EnumTipoOpcaoCheckListRFI = Object.freeze(new EnumTipoOpcaoCheckListRFIHelper());