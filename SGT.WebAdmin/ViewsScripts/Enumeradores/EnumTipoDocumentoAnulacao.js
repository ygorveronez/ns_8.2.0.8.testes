var EnumTipoDocumentoAnulacaoHelper = function () {
    this.CTe = 1;
    this.NFe = 2;
    this.CTouNF = 3;
};

EnumTipoDocumentoAnulacaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoDocumentoAnulacao.ChaveDeAcessoDoCTeEmitidoPeloTomador, value: this.CTe },
            { text: Localization.Resources.Enumeradores.TipoDocumentoAnulacao.ChaveDeAcessoDaNFeEmitidaPeloTomador, value: this.NFe },
            { text: Localization.Resources.Enumeradores.TipoDocumentoAnulacao.InformacoesDaNFOuCTEmitidoPeloTomador, value: this.CTouNF }
        ];
    }
};

var EnumTipoDocumentoAnulacao = Object.freeze(new EnumTipoDocumentoAnulacaoHelper());