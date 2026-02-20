var EnumSituacaoImportacaoPedidoHelper = function () {
    this.Todas = 0;
    this.Pendente = 1;
    this.Processando = 2;
    this.Sucesso = 3;
    this.Erro = 4;
    this.Cancelado = 5;
};

EnumSituacaoImportacaoPedidoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Consultas.SituacaoImportacaoPedido.Pendente, value: this.Pendente },
            { text: Localization.Resources.Consultas.SituacaoImportacaoPedido.Processando, value: this.Processando },
            { text: Localization.Resources.Consultas.SituacaoImportacaoPedido.Sucesso, value: this.Sucesso },
            { text: Localization.Resources.Consultas.SituacaoImportacaoPedido.Erro, value: this.Erro },
            { text: Localization.Resources.Consultas.SituacaoImportacaoPedido.Cancelado, value: this.Cancelado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Consultas.SituacaoImportacaoPedido.Todas, value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoImportacaoPedido = Object.freeze(new EnumSituacaoImportacaoPedidoHelper());