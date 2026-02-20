var EnumSituacaoControleCarregamentoHelper = function () {
    this.Todas = "";
    this.Aguardando = 1;
    this.EmCarregamento = 2;
    this.Finalizado = 3;
    this.EmDoca = 4;
}

EnumSituacaoControleCarregamentoHelper.prototype = {
    obterListaOpcoesPendentes: function () {
        return [
            this.Aguardando,
            this.EmCarregamento,
            this.EmDoca
        ];
    },
    obterOpcoes: function () {
        return [
            { text: "Aguardando", value: this.Aguardando },
            { text: "Em Carregamento", value: this.EmCarregamento },
            { text: "Em Doca", value: this.EmDoca},
            { text: "Finalizado", value: this.Finalizado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumSituacaoControleCarregamento = Object.freeze(new EnumSituacaoControleCarregamentoHelper());