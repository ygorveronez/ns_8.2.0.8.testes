
const EnumTipoEventoSuperAppHelper = function () {
    this.InicioDeViagem = 0;
    this.InicioDeOperacao = 1;
    this.EvidenciasDaEntrega = 2;
    this.Customizado = 3;
    this.FimDeOperacao = 4;
};

EnumTipoEventoSuperAppHelper.prototype = {
    obterOpcoes: function () {
        const opcoes = [];

        opcoes.push({ text: this.obterDescricao(this.InicioDeViagem), value: this.InicioDeViagem });
        opcoes.push({ text: this.obterDescricao(this.InicioDeOperacao), value: this.InicioDeOperacao });
        opcoes.push({ text: this.obterDescricao(this.EvidenciasDaEntrega), value: this.EvidenciasDaEntrega });
        opcoes.push({ text: this.obterDescricao(this.Customizado), value: this.Customizado });
        opcoes.push({ text: this.obterDescricao(this.FimDeOperacao), value: this.FimDeOperacao });

        return opcoes;
    },
    obterOpcoesCadastroEventos: function () {
        const opcoes = [];

        opcoes.push({ text: "", value: "" });
        opcoes.push({ text: this.obterDescricao(this.InicioDeViagem), value: this.InicioDeViagem });
        opcoes.push({ text: this.obterDescricao(this.InicioDeOperacao), value: this.InicioDeOperacao });
        opcoes.push({ text: this.obterDescricao(this.EvidenciasDaEntrega), value: this.EvidenciasDaEntrega });
        opcoes.push({ text: this.obterDescricao(this.Customizado), value: this.Customizado });
        opcoes.push({ text: this.obterDescricao(this.FimDeOperacao), value: this.FimDeOperacao });

        return opcoes;
    },
    obterDescricao: function (tipo) {
        switch (tipo) {
            case this.InicioDeViagem: return "Início de Viagem"
            case this.InicioDeOperacao: return "Início de Carregamento/Descarregamento";
            case this.EvidenciasDaEntrega: return "Evidências da Entrega";
            case this.Customizado: return "Evento Customizado";
            case this.FimDeOperacao: return "Fim de Carregamento/Descarregamento";
            default: return "";
        }
    }
};

const EnumTipoEventoSuperApp = Object.freeze(new EnumTipoEventoSuperAppHelper());
