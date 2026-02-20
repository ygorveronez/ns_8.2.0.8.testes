var EnumTipoArquivoGeracaoRelatorioHelper = function () {
    this.Excel = 1;
    this.Pdf = 2;
    this.PdfEExcel = 3;
}

EnumTipoArquivoGeracaoRelatorioHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Excel", value: this.Excel },
            { text: "PDF", value: this.Pdf },
            { text: "PDF e Excel", value: this.PdfEExcel }
        ];
    }
}

var EnumTipoArquivoGeracaoRelatorio = Object.freeze(new EnumTipoArquivoGeracaoRelatorioHelper());
