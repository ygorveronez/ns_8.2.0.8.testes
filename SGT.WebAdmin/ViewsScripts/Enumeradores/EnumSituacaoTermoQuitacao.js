var EnumSituacaoTermoQuitacaoHelper = function () {
    this.Todas = "";
    this.AguardandoAceiteTransportador = 1;
    this.AceiteTransportadorRejeitado = 2;
    this.AguardandoAprovacao = 3;
    this.SemRegraAprovacao = 4;
    this.AprovacaoRejeitada = 5;
    this.Finalizado = 6;
}

EnumSituacaoTermoQuitacaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aceite Rejeitado", value: this.AceiteTransportadorRejeitado },
            { text: "Aguardando Aceite", value: this.AguardandoAceiteTransportador },
            { text: "Aguardando Aprovação", value: this.AguardandoAprovacao },
            { text: "Aprovação Rejeitada", value: this.AprovacaoRejeitada },
            { text: "Finalizado", value: this.Finalizado },
            { text: "Sem Regra de Aprovação", value: this.SemRegraAprovacao }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    },
}

var EnumSituacaoTermoQuitacao = Object.freeze(new EnumSituacaoTermoQuitacaoHelper());