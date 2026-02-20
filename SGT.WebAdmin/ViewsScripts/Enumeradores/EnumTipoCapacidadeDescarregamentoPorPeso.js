const EnumTipoCapacidadeDescarregamentoPorPesoHelper = function () {
    this.Todos = null;
    this.DiaSemana = 1;
    this.PeriodoDescarregamento = 2;
};

EnumTipoCapacidadeDescarregamentoPorPesoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Dia da Semana", value: this.DiaSemana },
            { text: "Período de Descarregamento", value: this.PeriodoDescarregamento }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

const EnumTipoCapacidadeDescarregamentoPorPeso = Object.freeze(new EnumTipoCapacidadeDescarregamentoPorPesoHelper());
