let EnumSituacaoPalletGestaoDevolucaoHelper = function () {
    this.Todos = "";
    this.NoPrazo = 1;
    this.Vencido = 2;
    this.Agendado = 3;   
    this.Permuta = 4;   
};

EnumSituacaoPalletGestaoDevolucaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "No Prazo", value: this.NoPrazo },
            { text: "Vencido", value: this.Vencido },
            { text: "Agendado", value: this.Agendado },
            { text: "Permuta", value: this.Permuta },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.SituacaoPedido.Todos, value: this.Todos }].concat(this.obterOpcoes());
    },
};

let EnumSituacaoPalletGestaoDevolucao = Object.freeze(new EnumSituacaoPalletGestaoDevolucaoHelper());