var EnumSituacaoFrotaHelper = function () {
    this.Todos = 0;
    this.Disponivel = 1;
    this.EmViagem = 2;
    this.EmManutencao = 3;
    this.EmCarregamento = 4;
    this.EmDescarregamento = 5;
};

EnumSituacaoFrotaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Disponível", value: this.Disponivel },
            { text: "Em Viagem", value: this.EmViagem },
            { text: "Em Manutenção", value: this.EmManutencao },
            { text: "Em Carregamento", value: this.EmCarregamento },
            { text: "Em Descarregamento", value: this.EmDescarregamento }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoFrota = Object.freeze(new EnumSituacaoFrotaHelper());