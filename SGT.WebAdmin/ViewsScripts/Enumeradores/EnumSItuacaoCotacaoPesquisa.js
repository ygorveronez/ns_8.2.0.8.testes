// Esse enumerador não fica salvo em nenhum lugar, serve apenas para pesquisar

var EnumSituacaoCotacaoPesquisaHelper = function () {
    this.LeilaoAguardandoAprovacaoDoVencedor = 0;
    this.LeilaoSemLances = 1;
    this.LeilaoFinalizado = 2;
    this.LeilaoAbertoParaLances = 3;
};

EnumSituacaoCotacaoPesquisaHelper.prototype = {
    obterOpcoesPesquisa: function () {
        return [
            { text: "Leilão aguardando aprovação do vencedor", value: this.LeilaoAguardandoAprovacaoDoVencedor },
            { text: "Leilão sem lances", value: this.LeilaoSemLances },
            { text: "Leilão finalizado", value: this.LeilaoFinalizado },
            { text: "Leilão aberto para lances", value: this.LeilaoAbertoParaLances }
        ];
    }
};

var EnumSituacaoCotacaoPesquisa = Object.freeze(new EnumSituacaoCotacaoPesquisaHelper());
