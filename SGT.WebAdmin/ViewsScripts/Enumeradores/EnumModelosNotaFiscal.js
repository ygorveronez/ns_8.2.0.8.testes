var EnumModelosNotaFiscalHelper = function () {
    this.NotaFiscal = 01;
    this.NotaFiscalDeProdutor = 04;
};

EnumModelosNotaFiscalHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.ModelosNotaFiscal.NF, value: this.NotaFiscal },
            { text: Localization.Resources.Enumeradores.ModelosNotaFiscal.NotaFiscalDeProdutor, value: this.NotaFiscalDeProdutor }
        ];
    },
};

var EnumModelosNotaFiscal = Object.freeze(new EnumModelosNotaFiscalHelper());