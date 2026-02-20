var EnumPrioridadeOrdemServicoHelper = function () {
    this.Urgente = 1;
    this.Alto = 2;
    this.Medio = 3;
    this.Baixo = 4;
};

EnumPrioridadeOrdemServicoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Baixo", value: this.Baixo },
            { text: "Médio", value: this.Medio },
            { text: "Alto", value: this.Alto },
            { text: "Urgente", value: this.Urgente }
        ]
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumPrioridadeOrdemServico = Object.freeze(new EnumPrioridadeOrdemServicoHelper());