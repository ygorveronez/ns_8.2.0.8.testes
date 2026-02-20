var EnumFormaValorDescargaHelper = function () {
    this.Todos = "";
    this.ValorDestacado = 1;
    this.NenhumValorIncluso = 2;
    this.ValorVariavelNaoDestacado = 3;
    this.ValorFixoNaoDestacado = 4;
    this.IntegralmenteIncluso = 5;
};

EnumFormaValorDescargaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.FormaValorDescarga.ValorDestacadoCTe, value: this.ValorDestacado },
            { text: Localization.Resources.Enumeradores.FormaValorDescarga.NenhumValorIncluso, value: this.NenhumValorIncluso },
            { text: Localization.Resources.Enumeradores.FormaValorDescarga.ValorVariavelIncluso, value: this.ValorVariavelNaoDestacado },
            { text: Localization.Resources.Enumeradores.FormaValorDescarga.ValorFixoInclusoNaoDestacado, value: this.ValorFixoNaoDestacado },
            { text: Localization.Resources.Enumeradores.FormaValorDescarga.DescargaIntegralmenteIncluso, value: this.IntegralmenteIncluso }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.FormaValorDescarga.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumFormaValorDescarga = Object.freeze(new EnumFormaValorDescargaHelper());