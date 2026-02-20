
const EnumTipoProcessamentoImagemEtapaChecklistSuperAppHelper = function () {
    this.GreyScale = "GREY_SCALE";
    this.BlackWhite = "BLACK_WHITE";
};

EnumTipoProcessamentoImagemEtapaChecklistSuperAppHelper.prototype = {
    obterOpcoes: function () {
        const opcoes = [];

        opcoes.push({ text: this.obterDescricao(this.GreyScale), value: this.GreyScale });
        opcoes.push({ text: this.obterDescricao(this.BlackWhite), value: this.BlackWhite });

        return opcoes;
    },
    obterOpcoesCadastroChecklists: function () {
        const opcoes = [];

        opcoes.push({ text: "Nenhum", value: "" });
        opcoes.push({ text: this.obterDescricao(this.GreyScale), value: this.GreyScale });
        opcoes.push({ text: this.obterDescricao(this.BlackWhite), value: this.BlackWhite });

        return opcoes;
    },
    obterDescricao: function (tipo) {
        switch (tipo) {
            case this.GreyScale: return "Escala de Cinza";
            case this.BlackWhite: return "Preto e Branco";
            default: return "";
        }
    }
};

const EnumTipoProcessamentoImagemEtapaChecklistSuperApp = Object.freeze(new EnumTipoProcessamentoImagemEtapaChecklistSuperAppHelper());