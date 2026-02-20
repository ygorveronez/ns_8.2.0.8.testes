var EnumSituacaoManobraHelper = function () {
    this.Todas = "";
    this.AguardandoManobra = 1;
    this.EmManobra = 2;
    this.Finalizada = 3;
    this.Reservada = 4;
    this.Cancelada = 5;
}

EnumSituacaoManobraHelper.prototype = {
    obterListaOpcoesPendentes: function () {
        return [
            this.AguardandoManobra,
            this.EmManobra,
            this.Reservada
        ];
    },
    obterOpcoes: function () {
        return [
            { text: "Aguardando", value: this.AguardandoManobra },
            { text: "Cancelada", value: this.Cancelada },
            { text: "Em Manobra", value: this.EmManobra },
            { text: "Finalizada", value: this.Finalizada },
            { text: "Reservada", value: this.Reservada }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumSituacaoManobra = Object.freeze(new EnumSituacaoManobraHelper());