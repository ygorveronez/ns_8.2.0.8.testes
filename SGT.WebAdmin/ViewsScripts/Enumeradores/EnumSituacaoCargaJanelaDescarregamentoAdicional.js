var EnumSituacaoCargaJanelaDescarregamentoAdicionalHelper = function () {
    this.Todas = "";
    this.ForaPeriodoDescarregamento = 1;
};

EnumSituacaoCargaJanelaDescarregamentoAdicionalHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Fora do Período de Descarregamento", value: this.ForaPeriodoDescarregamento }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumSituacaoCargaJanelaDescarregamentoAdicional = Object.freeze(new EnumSituacaoCargaJanelaDescarregamentoAdicionalHelper());
