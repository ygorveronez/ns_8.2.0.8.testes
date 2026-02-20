var EnumTipoEnvioFaturaHelper = function () {
    this.Todos = 0;
    this.SomenteFatura = 1;
    this.SomenteCTe = 2;
    this.SomenteCTeSemXML = 3;
    this.CTeFaturaSemXML = 4;
    this.PDFCTeFaturaAgrupado = 5;
    this.TodosOsDocumentosDaFatura = 6;
    this.EnviarTodosDocumentosPDFNFSe = 7;
 
   
    
};

EnumTipoEnvioFaturaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoEnvioFatura.Todos, value: this.Todos },
            { text: Localization.Resources.Enumeradores.TipoEnvioFatura.SomenteFatura, value: this.SomenteFatura },
            { text: Localization.Resources.Enumeradores.TipoEnvioFatura.SomenteCTe, value: this.SomenteCTe },
            { text: Localization.Resources.Enumeradores.TipoEnvioFatura.PDFDoCTeSemXmlDoCTe, value: this.SomenteCTeSemXML },
            { text: Localization.Resources.Enumeradores.TipoEnvioFatura.PDFDoCTePDFDaFaturaSemXmlDoCTe, value: this.CTeFaturaSemXML },
            { text: Localization.Resources.Enumeradores.TipoEnvioFatura.PDFDeCTeFaturaAgrupado, value: this.PDFCTeFaturaAgrupado },
            { text: Localization.Resources.Enumeradores.TipoEnvioFatura.TodosOsDocumentosDaFatura, value: this.TodosOsDocumentosDaFatura },
            { text: Localization.Resources.Enumeradores.TipoEnvioFatura.EnviarTodosDocumentosPDFNFSe, value: this.EnviarTodosDocumentosPDFNFSe }
           
        ];
    }
};

var EnumTipoEnvioFatura = Object.freeze(new EnumTipoEnvioFaturaHelper());