var EnumTipoDocumentoValeAvulsoHelper = function () {
    this.Todos = 0;
    this.ValeAvulso = 1;
    this.Recibo = 2;    
};

EnumTipoDocumentoValeAvulsoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Vale", value: this.ValeAvulso },
            { text: "Recibo", value: this.Recibo },            
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoDocumentoValeAvulso = Object.freeze(new EnumTipoDocumentoValeAvulsoHelper());