

const EnumTipoAlertaEtapaChecklistSuperAppHelper = function () {
    this.Error = "ERROR";
    this.Warning = "WARNING";
    this.Info = "INFO";
    this.Success = "SUCCESS";
};

EnumTipoAlertaEtapaChecklistSuperAppHelper.prototype = {
    obterOpcoes: function () {
        const opcoes = [];

        opcoes.push({ text: this.obterDescricao(this.Error), value: this.Error });
        opcoes.push({ text: this.obterDescricao(this.Warning), value: this.Warning });
        opcoes.push({ text: this.obterDescricao(this.Info), value: this.Info });
        opcoes.push({ text: this.obterDescricao(this.Success), value: this.Success });

        return opcoes;
    },
    obterOpcoesCadastroChecklists: function () {
        const opcoes = [];

        opcoes.push({ text: "Nenhum", value: "" });
        opcoes.push({ text: this.obterDescricao(this.Error), value: this.Error });
        opcoes.push({ text: this.obterDescricao(this.Warning), value: this.Warning });
        opcoes.push({ text: this.obterDescricao(this.Info), value: this.Info });
        opcoes.push({ text: this.obterDescricao(this.Success), value: this.Success });

        return opcoes;
    },
    obterDescricao: function (tipo) {
        switch (tipo) {
            case this.Error: return "Perigo";
            case this.Warning: return "Atenção";
            case this.Info: return "Informativo";
            case this.Success: return "Sucesso";
            default: return "";
        }
    }
};

const EnumTipoAlertaEtapaChecklistSuperApp = Object.freeze(new EnumTipoAlertaEtapaChecklistSuperAppHelper());