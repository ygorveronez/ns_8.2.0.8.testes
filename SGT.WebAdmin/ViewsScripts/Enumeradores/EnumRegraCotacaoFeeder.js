var EnumRegraCotacaoFeederHelper = function () {
    this.Todos = 0;
    this.Nenhuma = 1;
    this.TaxaDoDia = 2;
    this.TaxaDoDiaUtilDoETS = 3;
    this.TaxaDoDiaUtilAnterior = 4;
};

EnumRegraCotacaoFeederHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.RegraCotacaoFeeder.NaoUtilizarRegra, value: this.Nenhuma },
            { text: Localization.Resources.Enumeradores.RegraCotacaoFeeder.UtilizarTaxaDia, value: this.TaxaDoDia },
            { text: Localization.Resources.Enumeradores.RegraCotacaoFeeder.UtilizarTaxaDiaETS, value: this.TaxaDoDiaUtilDoETS },
            { text: Localization.Resources.Enumeradores.RegraCotacaoFeeder.UtilizarTaxaDiaCTe, value: this.TaxaDoDiaUtilAnterior }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.RegraCotacaoFeeder.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumRegraCotacaoFeeder = Object.freeze(new EnumRegraCotacaoFeederHelper());