const EnumTipoFluxoChecklistSuperAppHelper = function () {
    this.Sequencial = 1;
    this.NaoSequencial = 2;
};

EnumTipoFluxoChecklistSuperAppHelper.prototype = {
    obterOpcoes: function () {
        const opcoes = [];

        opcoes.push({ text: "", value: "" });
        opcoes.push({ text: this.obterDescricao(this.Sequencial), value: this.Sequencial });
        opcoes.push({ text: this.obterDescricao(this.NaoSequencial), value: this.NaoSequencial });
        
        return opcoes;
    },
    obterDescricao: function (tipo) {
        switch (tipo) {
            case this.Sequencial: return "Sequencial";
            case this.NaoSequencial: return "Não Sequencial";
            default: return "";
        }
    }
};

const EnumTipoFluxoChecklistSuperApp = Object.freeze(new EnumTipoFluxoChecklistSuperAppHelper());
