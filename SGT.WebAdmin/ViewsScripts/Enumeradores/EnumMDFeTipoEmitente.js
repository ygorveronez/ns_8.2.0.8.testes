var EnumMDFeTipoEmitenteHelper = function () {
    this.NaoSelecionado = 0;
    this.PrestadorDeServicoDeTransporte = 1;
    this.TransporteCTeGlobalizado = 3;

};

EnumMDFeTipoEmitenteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "2 - Transportador de carga própria", value: this.PrestadorDeServicoDeTransporte },
            { text: "3 - Prestador de serviço de transporte que emitirá CT-e globalizado", value: this.TransporteCTeGlobalizado },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Não Selecionado", value: this.NaoSelecionado }].concat(this.obterOpcoes());
    }
};

var EnumMDFeTipoEmitente = Object.freeze(new EnumMDFeTipoEmitenteHelper());