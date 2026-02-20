let EnumSituacaoPalletHelper = function () {
    this.Todos = "";
    this.Pendente = 1;
    this.Concluido = 2;
    this.Cancelado = 3;   
    this.Reserva = 4;   
    this.AguardandoAvaliacao = 5;
};

EnumSituacaoPalletHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Pendente", value: this.Pendente },
            { text: "Concluído", value: this.Concluido },
            { text: "Cancelada", value: this.Cancelado },
            { text: "Reserva", value: this.Reserva },
            { text: "Aguardando Avaliação", value: this.AguardandoAvaliacao },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.SituacaoPedido.Todos, value: this.Todos }].concat(this.obterOpcoes());
    },
};

let EnumSituacaoPallet = Object.freeze(new EnumSituacaoPalletHelper());