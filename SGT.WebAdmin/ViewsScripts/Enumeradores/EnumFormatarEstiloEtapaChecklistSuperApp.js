
const EnumFormatarEstiloEtapaChecklistSuperAppHelper = function () {
    this.UpperCase = "UPPERCASE";
    this.LowerCase = "LOWERCASE";
    this.StartCase = "STARTCASE";
};

EnumFormatarEstiloEtapaChecklistSuperAppHelper.prototype = {
    obterOpcoes: function () {
        const opcoes = [];

        opcoes.push({ text: this.obterDescricao(this.UpperCase), value: this.UpperCase });
        opcoes.push({ text: this.obterDescricao(this.LowerCase), value: this.LowerCase });
        opcoes.push({ text: this.obterDescricao(this.StartCase), value: this.StartCase });

        return opcoes;
    },
    obterOpcoesCadastroChecklists: function () {
        const opcoes = [];

        opcoes.push({ text: "Nenhum", value: "" });
        opcoes.push({ text: this.obterDescricao(this.UpperCase), value: this.UpperCase });
        opcoes.push({ text: this.obterDescricao(this.LowerCase), value: this.LowerCase });
        opcoes.push({ text: this.obterDescricao(this.StartCase), value: this.StartCase });

        return opcoes;
    },
    obterDescricao: function (tipo) {
        switch (tipo) {
            case this.UpperCase: return "Caixa alta";
            case this.LowerCase: return "Caixa baixa";
            case this.StartCase: return "Iniciais maiúsculas";
            default: return "";
        }
    }
};

const EnumFormatarEstiloEtapaChecklistSuperApp = Object.freeze(new EnumFormatarEstiloEtapaChecklistSuperAppHelper());
