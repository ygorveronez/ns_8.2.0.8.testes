const EnumTipoParadaEventoSuperAppHelper = function () {
    this.Coleta = 0;
    this.Entrega = 1;
    this.Ambos = 2;
};

EnumTipoParadaEventoSuperAppHelper.prototype = {
    obterOpcoes: function () {
        const opcoes = [];

        opcoes.push({ text: this.obterDescricao(this.Coleta), value: this.Coleta });
        opcoes.push({ text: this.obterDescricao(this.Entrega), value: this.Entrega });
        opcoes.push({ text: this.obterDescricao(this.Ambos), value: this.Ambos });
        
        return opcoes;
    },
    opcoesCadastroEventos: function () {
        const opcoes = [];

        opcoes.push({ text: "", value: "" });
        opcoes.push({ text: this.obterDescricao(this.Coleta), value: this.Coleta });
        opcoes.push({ text: this.obterDescricao(this.Entrega), value: this.Entrega });
        opcoes.push({ text: this.obterDescricao(this.Ambos), value: this.Ambos });

        return opcoes;
    },
    obterDescricao: function (tipo) {
        switch (tipo) {
            case this.Coleta: return "Coleta";
            case this.Entrega: return "Entrega";
            case this.Ambos: return "Ambos";
            default: return "";
        }
    }
};

const EnumTipoParadaEventoSuperApp = Object.freeze(new EnumTipoParadaEventoSuperAppHelper());
