var EnumSituacaoJanelaDescarregamentoHelper = function () {
    this.Todas = "";
    this.AguardandoCarregamento = 1;
    this.EmViagem = 2;
    this.Atrazada = 3;
    this.EmDescarregamento = 4;
    this.Liberada = 5;
};

EnumSituacaoJanelaDescarregamentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando Carregamento", value: this.AguardandoCarregamento },
            { text: "Atrazada", value: this.Atrazada },
            { text: "Em Descarregamento", value: this.EmDescarregamento },
            { text: "Em Viagem", value: this.EmViagem },
            { text: "Liberada", value: this.Liberada }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumSituacaoJanelaDescarregamento = Object.freeze(new EnumSituacaoJanelaDescarregamentoHelper());
