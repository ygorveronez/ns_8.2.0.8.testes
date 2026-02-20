var EnumTipoImpressaoFaturaHelper = function () {
    this.Padrao = 0;
    this.Multimodal = 1;
    this.PorDocumentos = 2;
    this.ParcelasSeparadas = 3;
    this.FaturaChaveCTe = 4;
};

EnumTipoImpressaoFaturaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Padrão", value: this.Padrao },
            { text: "Multimodal", value: this.Multimodal },
            { text: "Por Documentos", value: this.PorDocumentos },
            { text: "Parcelas Separadas", value: this.ParcelasSeparadas },
            { text: "Fatura Chave CT-es", value: this.FaturaChaveCTe }
        ];
    }
};

var EnumTipoImpressaoFatura = Object.freeze(new EnumTipoImpressaoFaturaHelper());