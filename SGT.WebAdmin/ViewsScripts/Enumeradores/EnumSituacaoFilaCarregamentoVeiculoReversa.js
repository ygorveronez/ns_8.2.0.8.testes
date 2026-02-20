var EnumSituacaoFilaCarregamentoVeiculoReversaHelper = function () {
    this.Todas = "";
    this.AguardandoDescarregamento = 1;
    this.EmDescarregamento = 2;
    this.Finalizada = 3;
    this.Cancelada = 4;
}

EnumSituacaoFilaCarregamentoVeiculoReversaHelper.prototype = {
    obterListaOpcoesPendentes: function () {
        return [
            this.AguardandoDescarregamento,
            this.EmDescarregamento
        ];
    },
    obterOpcoes: function () {
        return [
            { text: "Aguardando Descarregamento", value: this.AguardandoDescarregamento },
            { text: "Cancelada", value: this.Cancelada },
            { text: "Em Descarregamento", value: this.EmDescarregamento },
            { text: "Finalizada", value: this.Finalizada }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    },
}

var EnumSituacaoFilaCarregamentoVeiculoReversa = Object.freeze(new EnumSituacaoFilaCarregamentoVeiculoReversaHelper());