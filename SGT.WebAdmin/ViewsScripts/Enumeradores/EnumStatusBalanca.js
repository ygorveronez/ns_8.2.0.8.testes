var EnumStatusBalancaHelper = function () {
    this.Todos = 0;
    this.AgIntegracao = 1;
    this.TicketCriado = 2;
    this.FalhaIntegracao = 3;
    this.TicketBloqueado = 4;
    this.TicketDesbloqueado = 5;
    this.Encerrado = 6;
};

EnumStatusBalancaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Ag. Integração", value: this.AgIntegracao },
            { text: "Ticket Criado", value: this.TicketCriado },
            { text: "Falha na Integração", value: this.FalhaIntegracao },
            { text: "Ticket Bloqueado", value: this.TicketBloqueado },
            { text: "Ticket Desbloqueado", value: this.TicketDesbloqueado },
            { text: "Encerrado", value: this.Encerrado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumStatusBalanca = Object.freeze(new EnumStatusBalancaHelper());