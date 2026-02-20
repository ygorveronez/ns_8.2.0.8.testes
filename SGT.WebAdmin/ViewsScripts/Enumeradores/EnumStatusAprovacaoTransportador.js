var EnumStatusAprovacaoTransportadorHelper = function () {
    this.Todos = "";
    this.Rejeitado = 0;
    this.Aprovado = 1;
    this.AguardandoAprovacao = 2;
};

EnumStatusAprovacaoTransportadorHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "Rejeitado", value: this.Rejeitado },
            { text: "Aprovado", value: this.Aprovado },
            { text: "Aguardando Aprovação", value: this.AguardandoAprovacao },
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: "" }].concat(this.ObterOpcoes());
    }
}

var EnumStatusAprovacaoTransportador = Object.freeze(new EnumStatusAprovacaoTransportadorHelper());