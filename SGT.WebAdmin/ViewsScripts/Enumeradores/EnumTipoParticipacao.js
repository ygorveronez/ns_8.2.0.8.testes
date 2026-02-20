var EnumTipoParticipacao = function() {
    this.Remetente =  1,
    this.Destinatario = 2,
    this.Expedidor = 3,
    this.Recebedor = 4,
    this.Tomador = 5
}

EnumTipoParticipacao.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Remetente", value: this.Remetente },
            { text: "Destinatário", value: this.Destinatario },
            { text: "Expedidor", value: this.Expedidor },
            { text: "Recebedor", value: this.Recebedor }
        ];
    },
    obterOpcoesClienteBuscaAutomatica: function () {
        return [
            { text: "Expedidor", value: this.Expedidor },
            { text: "Tomador", value: this.Tomador }
        ];
    },
    obterOpcoesPesquisa: function () {
        return this.obterOpcoes();
    }
};

var EnumTipoParticipacao = Object.freeze(new EnumTipoParticipacao());


