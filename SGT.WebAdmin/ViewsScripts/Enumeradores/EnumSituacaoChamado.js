var EnumSituacaoChamadoHelper = function () {
    this.Todas = 0;
    this.Aberto = 1;
    this.Finalizado = 2;
    this.SemRegra = 3;
    this.LiberadaOcorrencia = 4;
    this.Cancelada = 5;
    this.LiberadaValePallet = 6;
    this.EmTratativa = 7;
    this.RecusadoPeloCliente = 8;
    this.AgIntegracao = 9;
    this.FalhaIntegracao = 10;
    this.AgGeracaoLote = 11;
    this.AgAprovacaoLote = 12;
};

EnumSituacaoChamadoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacaoChamado.Aberto, value: this.Aberto },
            { text: Localization.Resources.Enumeradores.SituacaoChamado.Finalizado, value: this.Finalizado },
            { text: Localization.Resources.Enumeradores.SituacaoChamado.Cancelado, value: this.Cancelada },
            { text: Localization.Resources.Enumeradores.SituacaoChamado.SemRegraParaAvaliacao, value: this.SemRegra },
            { text: Localization.Resources.Enumeradores.SituacaoChamado.LiberadoParaOcorrencia, value: this.LiberadaOcorrencia },
            { text: Localization.Resources.Enumeradores.SituacaoChamado.LiberadoParaValePallet, value: this.LiberadaValePallet },
            { text: Localization.Resources.Enumeradores.SituacaoChamado.EmTratativa, value: this.EmTratativa },
            { text: Localization.Resources.Enumeradores.SituacaoChamado.Recusado, value: this.RecusadoPeloCliente },
            { text: Localization.Resources.Enumeradores.SituacaoChamado.AgIntegracao, value: this.AgIntegracao },
            { text: Localization.Resources.Enumeradores.SituacaoChamado.FalhaIntegracao, value: this.FalhaIntegracao },
            { text: Localization.Resources.Enumeradores.SituacaoChamado.AgGeracaoLote, value: this.AgGeracaoLote },
            { text: Localization.Resources.Enumeradores.SituacaoChamado.AgAprovacaoLote, value: this.AgAprovacaoLote }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.SituacaoChamado.Todas, value: this.Todas }].concat(this.obterOpcoes());
    },
    obterOpcoesSituacaoTratativa: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacaoChamado.Aberto, value: this.Aberto },
            { text: Localization.Resources.Enumeradores.SituacaoChamado.EmTratativa, value: this.EmTratativa }
        ];
    },
};

var EnumSituacaoChamado = Object.freeze(new EnumSituacaoChamadoHelper());