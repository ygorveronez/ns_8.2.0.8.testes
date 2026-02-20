var EnumFormaEnvioDocumentacaoHelper = function () {
    this.PDFCTeNotas = 0;
    this.PDFCTeNotasXMLCTeNotas = 1;
    this.SomenteXMLCTe = 2;
    this.SomenteXMLNotas = 3;
    this.SomentePDFCTe = 4;
    this.SomentePDFNotas = 5;
    this.Padrao = 6;
};

EnumFormaEnvioDocumentacaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.FormaEnvioDocumentacao.PDFDoCTesNotas, value: this.PDFCTeNotas },
            { text: Localization.Resources.Enumeradores.FormaEnvioDocumentacao.PDFDoCTesNotasXMLDosCTesNotas, value: this.PDFCTeNotasXMLCTeNotas }
        ];
    },
    obterOpcoesImpressaoLote: function () {
        return [
            { text: Localization.Resources.Enumeradores.FormaEnvioDocumentacao.UsarDoGrupoDePessoa, value: this.Padrao },
            { text: Localization.Resources.Enumeradores.FormaEnvioDocumentacao.PDFDoCTesNotas, value: this.PDFCTeNotas },
            { text: Localization.Resources.Enumeradores.FormaEnvioDocumentacao.PDFDoCTesNotasXMLDosCTesNotas, value: this.PDFCTeNotasXMLCTeNotas },
            { text: Localization.Resources.Enumeradores.FormaEnvioDocumentacao.SomenteXMLDosCTes, value: this.SomenteXMLCTe },
            { text: Localization.Resources.Enumeradores.FormaEnvioDocumentacao.SomenteXMLDasNotas, value: this.SomenteXMLNotas },
            { text: Localization.Resources.Enumeradores.FormaEnvioDocumentacao.PDFDosCTesApenas, value: this.SomentePDFCTe },
            { text: Localization.Resources.Enumeradores.FormaEnvioDocumentacao.PDFDasNotasApenas, value: this.SomentePDFNotas }
        ];
    }
};

var EnumFormaEnvioDocumentacao = Object.freeze(new EnumFormaEnvioDocumentacaoHelper());