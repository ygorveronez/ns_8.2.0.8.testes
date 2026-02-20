

const EnumValidacaoAdicionalEtapaTextoSuperAppHelper = function () {
    this.Cpf = 1;
    this.Cnpj = 2;
    this.CpfCnpj = 3;
};

EnumValidacaoAdicionalEtapaTextoSuperAppHelper.prototype = {
    obterOpcoes: function () {
        const opcoes = [];

        opcoes.push({ text: this.obterDescricao(this.Cpf), value: this.Cpf });
        opcoes.push({ text: this.obterDescricao(this.Cnpj), value: this.Cnpj });
        opcoes.push({ text: this.obterDescricao(this.CpfCnpj), value: this.CpfCnpj });
        return opcoes;
    },
    obterOpcoesCadastroChecklist: function () {
        const opcoes = [];

        opcoes.push({ text: "", value: "" });
        opcoes.push({ text: this.obterDescricao(this.Cpf), value: this.Cpf });
        opcoes.push({ text: this.obterDescricao(this.Cnpj), value: this.Cnpj });
        opcoes.push({ text: this.obterDescricao(this.CpfCnpj), value: this.CpfCnpj });
        
        return opcoes;
    },
    obterDescricao: function (tipo) {
        switch (tipo) {
            case this.Cpf: return "CPF";
            case this.Cnpj: return "CNPJ";
            case this.CpfCnpj: return "CPF ou CNPJ";
            default: return "";
        }
    }
};

const EnumValidacaoAdicionalEtapaTextoSuperApp = Object.freeze(new EnumValidacaoAdicionalEtapaTextoSuperAppHelper());