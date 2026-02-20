var EnumTipoLiberacaoPagamentoHelper = function () {
    this.Todos = "";
    this.Nenhum = 0;
    this.ReceberEscrituracaoNotaProduto = 1;
    this.DigitalizacaoImagemCanhoto = 2;
    this.AprovacaoImagemCanhoto = 3;
};

EnumTipoLiberacaoPagamentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Gerais.Geral.Nenhum, value: this.Nenhum },
            { text: Localization.Resources.Enumeradores.TipoLiberacaoPagamento.ReceberEscrituracaoNotaProduto, value: this.ReceberEscrituracaoNotaProduto },
            { text: Localization.Resources.Enumeradores.TipoLiberacaoPagamento.DigitalizacaoImagemCanhoto, value: this.DigitalizacaoImagemCanhoto },
            { text: Localization.Resources.Enumeradores.TipoLiberacaoPagamento.AprovacaoImagemCanhoto, value: this.AprovacaoImagemCanhoto },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoLiberacaoPagamento = Object.freeze(new EnumTipoLiberacaoPagamentoHelper());