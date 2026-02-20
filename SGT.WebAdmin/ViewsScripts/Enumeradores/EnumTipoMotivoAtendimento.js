var EnumTipoMotivoAtendimentoHelper = function () {
    this.Todos = "";
    this.Atendimento = 0;
    this.Devolucao = 1;
    this.Reentrega = 2;
    this.Retencao = 3;
    this.RetencaoOrigem = 4;
    this.ReentregarMesmaCarga = 8;
};

EnumTipoMotivoAtendimentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoMotivoAtendimento.Atendimento, value: this.Atendimento },
            { text: Localization.Resources.Enumeradores.TipoMotivoAtendimento.Devolucao, value: this.Devolucao },
            { text: Localization.Resources.Enumeradores.TipoMotivoAtendimento.Reentrega, value: this.Reentrega },
            { text: Localization.Resources.Enumeradores.TipoMotivoAtendimento.Retencao, value: this.Retencao },
            { text: Localization.Resources.Enumeradores.TipoMotivoAtendimento.RetencaoOrigem, value: this.RetencaoOrigem },
            { text: Localization.Resources.Enumeradores.TipoMotivoAtendimento.ReentregarMesmaCarga, value: this.ReentregarMesmaCarga }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoMotivoAtendimento.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoMotivoAtendimento = Object.freeze(new EnumTipoMotivoAtendimentoHelper());