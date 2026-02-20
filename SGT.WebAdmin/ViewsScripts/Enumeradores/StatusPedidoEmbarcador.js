var StatusPedidoEmbarcadorHelper = function () {
    this.Todos = -1;
    this.NaoDefinido = 0;
    this.Aberto = 1;
    this.BloqueadoPorRegra = 2;
    this.BloqueadoPorVerba = 3;
    this.AguardandoSalesForce = 4;
    this.Liberado = 5;
    this.Encerrado = 6;
}

StatusPedidoEmbarcadorHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Não Definido", value: this.NaoDefinido },
            { text: "Aberto", value: this.Aberto },
            { text: "Bloqueado por Regra", value: this.BloqueadoPorRegra },
            { text: "Bloqueado por Verba", value: this.BloqueadoPorVerba },
            { text: "Aguardando Sales Force", value: this.AguardandoSalesForce },
            { text: "Liberado", value: this.Liberado },
            { text: "Encerrado", value: this.Encerrado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
}

var StatusPedidoEmbarcador = new StatusPedidoEmbarcadorHelper();