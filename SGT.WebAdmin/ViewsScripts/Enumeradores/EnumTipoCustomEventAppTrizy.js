const EnumTipoCustomEventAppTrizyHelper = function () {
    this.Nenhum = 0;
    this.EstouIndo = 1;
    this.SolicitacaoDataeHoraCanhoto = 2;
};

EnumTipoCustomEventAppTrizyHelper.prototype = {
    obterOpcoes: function () {
        const opcoes = [];

        opcoes.push({ text: this.obterDescricao(this.Nenhum), value: this.Nenhum });
        opcoes.push({ text: this.obterDescricao(this.EstouIndo), value: this.EstouIndo });
        opcoes.push({ text: this.obterDescricao(this.SolicitacaoDataeHoraCanhoto), value: this.SolicitacaoDataeHoraCanhoto });

        return opcoes;
    },
    opcoesCadastroEventos: function () {
        const opcoes = [];

        opcoes.push({ text: "", value: "" });
        opcoes.push({ text: this.obterDescricao(this.EstouIndo), value: this.EstouIndo });
        opcoes.push({ text: this.obterDescricao(this.SolicitacaoDataeHoraCanhoto), value: this.SolicitacaoDataeHoraCanhoto });

        return opcoes;
    },
    obterDescricao: function (tipo) {
        switch (tipo) {
            case this.Nenhum: return "Nenhum";
            case this.EstouIndo: return "Estou indo";
            case this.SolicitacaoDataeHoraCanhoto: return "Solicitação de data e hora do canhoto";
            default: return "";
        }
    }
};

const EnumTipoCustomEventAppTrizy = Object.freeze(new EnumTipoCustomEventAppTrizyHelper());
