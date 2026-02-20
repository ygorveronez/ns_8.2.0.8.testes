var EnumTipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFreteHelper = function () {
    this.PorNotaFiscal = 1;
    this.PorDocumentoEmitido = 2;
};

EnumTipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFreteHelper.prototype = {
    ObterOpcoes: function (opcaoSelecione) {
        var arrayOpcoes = [
            { text: Localization.Resources.Enumeradores.TipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete.PorNotaFiscal, value: this.PorNotaFiscal },
            { text: Localization.Resources.Enumeradores.TipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete.PorDocumentoEmitido, value: this.PorDocumentoEmitido },
     ];

        return arrayOpcoes;
    }
};

var EnumTipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete = Object.freeze(new EnumTipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFreteHelper());