var EnumSituacaoSeparacaoMercadoriaHelper = function () {
    this.Todas = "";
    this.AguardandoSeparacaoMercadoria = 1;
    this.SeparacaoMercadoriaFinalizada = 2;
};

EnumSituacaoSeparacaoMercadoriaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando Separação de Mercadoria", value: this.AguardandoSeparacaoMercadoria },
            { text: "Separação de Mercadoria Finalizada", value: this.SeparacaoMercadoriaFinalizada }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumSituacaoSeparacaoMercadoria = Object.freeze(new EnumSituacaoSeparacaoMercadoriaHelper());
