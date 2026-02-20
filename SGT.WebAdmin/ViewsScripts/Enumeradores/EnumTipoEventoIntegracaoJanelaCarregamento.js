var EnumTipoEventoIntegracaoJanelaCarregamentoHelper = function () {
    this.Todos = null;
    this.RecebimentoLeilao = 1;
    this.RetornoLeilao = 2;
};

EnumTipoEventoIntegracaoJanelaCarregamentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Recebimento Leilão", value: this.RecebimentoLeilao },
            { text: "Retorno Leilão", value: this.RetornoLeilao }
        ]
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(obterOpcoes());
    }
}

var EnumTipoEventoIntegracaoJanelaCarregamento = Object.freeze(new EnumTipoEventoIntegracaoJanelaCarregamentoHelper());