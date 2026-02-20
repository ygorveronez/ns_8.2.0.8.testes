var EnumTipoCalculoCubagemTabelaFreteComponenteFreteHelper = function () {
    this.PorFracao = 0;
    this.PorUnidadeIncompleta = 1;
    this.PorUnidadeCompleta = 2;
};

EnumTipoCalculoCubagemTabelaFreteComponenteFreteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoCalculoCubagemTabelaFrete.PorFracao, value: this.PorFracao },
            { text: Localization.Resources.Enumeradores.TipoCalculoCubagemTabelaFrete.PorUnidadeCompleta, value: this.PorUnidadeCompleta },
            { text: Localization.Resources.Enumeradores.TipoCalculoCubagemTabelaFrete.PorUnidadeIncompleta, value: this.PorUnidadeIncompleta }
        ];
    }
};

var EnumTipoCalculoCubagemTabelaFreteComponenteFrete = Object.freeze(new EnumTipoCalculoCubagemTabelaFreteComponenteFreteHelper());
