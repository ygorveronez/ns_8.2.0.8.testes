var EnumTipoCapacidadeCarregamentoPorPesoHelper = function () {
    this.Todos = null;
    this.DiaSemana = 1;
    this.PeriodoCarregamento = 2;
};

EnumTipoCapacidadeCarregamentoPorPesoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoCapacidadeCarregamentoPorPeso.DiaDaSemana, value: this.DiaSemana },
            { text: Localization.Resources.Enumeradores.TipoCapacidadeCarregamentoPorPeso.PeriodoCarregamento, value: this.PeriodoCarregamento }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoCapacidadeCarregamentoPorPeso = Object.freeze(new EnumTipoCapacidadeCarregamentoPorPesoHelper());
