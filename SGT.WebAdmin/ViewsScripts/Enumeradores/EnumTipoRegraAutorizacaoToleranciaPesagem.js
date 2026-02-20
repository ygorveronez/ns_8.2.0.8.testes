let EnumTipoRegraAutorizacaoToleranciaPesagemHelper = function () {   
    this.Peso = 1;
    this.Percentual = 2;
}

EnumTipoRegraAutorizacaoToleranciaPesagemHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Peso", value: this.Peso },
            { text: "Percentual", value: this.Percentual },
        ];
    },    
};

let EnumTipoRegraAutorizacaoToleranciaPesagem = Object.freeze(new EnumTipoRegraAutorizacaoToleranciaPesagemHelper());