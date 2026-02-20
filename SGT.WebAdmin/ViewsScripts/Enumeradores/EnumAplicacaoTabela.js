var EnumAplicacaoTabelaHelper = function () {
    this.Todas = 0;
    this.Carga = 1;
    this.Ocorrencia = 2;
    this.CargaEOcorrencia = 3;
};

EnumAplicacaoTabelaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.AplicacaoTabela.Carga, value: this.Carga },
            { text: Localization.Resources.Enumeradores.AplicacaoTabela.Ocorrencia, value: this.Ocorrencia },
            { text: Localization.Resources.Enumeradores.AplicacaoTabela.CargaEOcorrencia, value: this.CargaEOcorrencia }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.AplicacaoTabela.Todas, value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumAplicacaoTabela = Object.freeze(new EnumAplicacaoTabelaHelper());