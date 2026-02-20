var EnumSituacaoCarregamentoHelper = function () {
    this.Todas = "";
    this.EmMontagem = 1;
    this.Fechado = 2;
    this.Cancelado = 3;
    this.AguardandoAprovacaoSolicitacao = 4;
    this.SolicitacaoReprovada = 5;
    this.Bloqueado = 6;
    this.GerandoCargaBackground = 7;
    this.FalhaIntegracao = 8;
};

EnumSituacaoCarregamentoHelper.prototype = {
    obterOpcoes: function () {
        var opcoes = [];

        if (_CONFIGURACAO_TMS.UtilizarAlcadaAprovacaoCarregamento)
            opcoes.push({ text: Localization.Resources.Enumeradores.SituacaoCarregamento.AguardandoAprovacaoSolicitacao , value: this.AguardandoAprovacaoSolicitacao });

        opcoes.push({ text: Localization.Resources.Enumeradores.SituacaoCarregamento.Cancelado, value: this.Cancelado });
        opcoes.push({ text: Localization.Resources.Enumeradores.SituacaoCarregamento.EmMontagem, value: this.EmMontagem });
        opcoes.push({ text: Localization.Resources.Enumeradores.SituacaoCarregamento.Fechado, value: this.Fechado });

        if (_CONFIGURACAO_TMS.UtilizarAlcadaAprovacaoCarregamento)
            opcoes.push({ text: Localization.Resources.Enumeradores.SituacaoCarregamento.SolicitacaoReprovada, value: this.SolicitacaoReprovada });

        return opcoes;
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.SituacaoCarregamento.Todas, value: this.Todas }].concat(this.obterOpcoes());
    },
    obterSituacoesEmMontagem: function () {
        var situacoes = [];

        if (_CONFIGURACAO_TMS.UtilizarAlcadaAprovacaoCarregamento)
            situacoes.push(this.AguardandoAprovacaoSolicitacao);

        situacoes.push(this.EmMontagem);
        situacoes.push(this.GerandoCargaBackground);

        if (_CONFIGURACAO_TMS.UtilizarAlcadaAprovacaoCarregamento)
            situacoes.push(this.SolicitacaoReprovada);

        return situacoes;
    },
    permitirCancelarCarregamento: function (situacao) {
        return (situacao == this.EmMontagem) || (situacao == this.AguardandoAprovacaoSolicitacao) || (situacao == this.SolicitacaoReprovada);
    },
    permitirEditarCarregamento: function (situacao) {
        return (situacao == this.EmMontagem) || (situacao == this.SolicitacaoReprovada);
    }
};

var EnumSituacaoCarregamento = Object.freeze(new EnumSituacaoCarregamentoHelper());
