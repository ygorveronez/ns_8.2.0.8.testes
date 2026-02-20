var EnumSituacaoTermoQuitacaoFinanceiroHelper = function () {
    this.Todas = "";
    this.AguardandoAprovacaoTransportador = 1;
    this.AprovadoTransportador = 2;
    this.RejeitadoTransportador = 3;
    this.AguardandoAprovacaoProvisao = 4;
    this.AprovadoProvisao = 5;
    this.RejeitadoProvisao = 6;
    this.SemRegraProvisao = 7;
    this.Novo = 8;
    this.Finalizada = 9;
}

EnumSituacaoTermoQuitacaoFinanceiroHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando Aprovação Transportador", value: this.AguardandoAprovacaoTransportador },
            { text: "Aprovado Transportador", value: this.AprovadoTransportador },
            { text: "Rejeitado Transportador", value: this.RejeitadoTransportador },
            { text: "Pendente Transportador", value: this.PendenteTransportador },
            { text: "Aguardando Aprovação Provisao", value: this.AguardandoAprovacaoProvisao },
            { text: "Aprovado Provisão", value: this.AprovadoProvisao },
            { text: "Rejeitado Provisão", value: this.RejeitadoProvisao },
            { text: "Pendente Provisão", value: this.PendenteProvisao },
            { text: "Sem Regra Provisão", value: this.SemRegraProvisao },
            { text: "Finalizada", value: this.Finalizada },
            { text: "Novo", value: this.Novo },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos }].concat(this.obterOpcoes());
    },
    obterOpcoesTransportador: function () {
        return [
            { text: "Aguardando Aprovação Transportador", value: this.AguardandoAprovacaoTransportador },
            { text: "Aprovado Transportador", value: this.AprovadoTransportador },
            { text: "Rejeitado Transportador", value: this.RejeitadoTransportador },
            { text: "Pendente Transportador", value: this.PendenteTransportador },
        ];
    },
    obterOpcoesTransportadorPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos }].concat(this.obterOpcoesTransportador());
    },
}

var EnumSituacaoTermoQuitacaoFinanceiro = Object.freeze(new EnumSituacaoTermoQuitacaoFinanceiroHelper());