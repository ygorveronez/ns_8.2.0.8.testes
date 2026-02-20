var EnumSitaucaoTermoQuitacaoHelper = function() {
    this.Todos = "";
    this.AguardandoAprovacaoTransportador = 1;
    this.AprovadoTransportador = 2;
    this.RejeitadoTransportador = 3;
    this.PendenteTransportador = 4;
    this.AguardandoAprovacaoProvisao = 5;
    this.AprovadoProvisao = 6;
    this.RejeitadoProvisao = 7;
    this.PendenteProvisao = 8;
    this.Finalizada = 9;
};

EnumSitaucaoTermoQuitacaoHelper.prototype = {
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
            { text: "Finalizada", value: this.Finalizada },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos }].concat(this.obterOpcoes());
    },

};


var EnumSitaucaoTermoQuitacao = Object.freeze(new EnumSitaucaoTermoQuitacaoHelper());