let EnumStatusCotacaoHelper = function () {
    this.Todos = 0;
    this.AguardandoAprovacao = 1;
    this.Aprovado = 2;
    this.Reprovado = 3;
    this.AguardandoAnalise = 4;
};

EnumStatusCotacaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando Aprovação", value: this.AguardandoAprovacao },
            { text: "Aprovado", value: this.Aprovado },
            { text: "Reprovado", value: this.Reprovado },
            { text: "Aguardando Análise", value: this.AguardandoAnalise },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.SituacaoPedido.Todos, value: this.Todos }].concat(this.obterOpcoes());
    },
};

let EnumStatusCotacao = Object.freeze(new EnumStatusCotacaoHelper());