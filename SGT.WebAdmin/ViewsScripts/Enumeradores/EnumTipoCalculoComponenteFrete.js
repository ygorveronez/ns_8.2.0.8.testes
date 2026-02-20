var EnumTipoCalculoComponenteFreteHelper = function () {
    this.Todos = "";
    this.PorFaixaAjudantes = 1;
    this.ValorFixoPorAjudante = 2;

};

EnumTipoCalculoComponenteFreteHelper.prototype = {
    ObterOpcoes: function (opcaoSelecione) {
        var arrayOpcoes = [
            { text: Localization.Resources.Enumeradores.TipoCalculoComponenteFrete.PorFaixaAjudantes, value: this.PorFaixaAjudantes },
            { text: Localization.Resources.Enumeradores.TipoCalculoComponenteFrete.ValorFixoAjudante, value: this.ValorFixoPorAjudante },
        ];

        if (opcaoSelecione) {
            arrayOpcoes.push({ text: Localization.Resources.Enumeradores.TipoCalculoComponenteFrete.Selecione, value: this.Selecione });
        }

        return arrayOpcoes;
    }
};

var EnumTipoCalculoComponenteFrete = Object.freeze(new EnumTipoCalculoComponenteFreteHelper());