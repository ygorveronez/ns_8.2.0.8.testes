var EnumStatusFinalizacaoAtendimentoHelper = function () {
    this.SemStatus = 0;
    this.Finalizar = 1;
    this.Cancelar = 2;
};

EnumStatusFinalizacaoAtendimentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.StatusFinalizacaoAtendimento.SemStatus, value: this.SemStatus },
            { text: Localization.Resources.Enumeradores.StatusFinalizacaoAtendimento.Finalizar, value: this.Finalizar },
            { text: Localization.Resources.Enumeradores.StatusFinalizacaoAtendimento.Cancelar, value: this.Cancelar }
        ];
    },
    obterDescricao: function (codigo) {
        const listaOpcoes = this.obterOpcoes();
        const [status] = listaOpcoes.filter(opcao => opcao.value === codigo)

        if (!status)
            return "";

        return status.text;
    }
};

var EnumStatusFinalizacaoAtendimento = Object.freeze(new EnumStatusFinalizacaoAtendimentoHelper());