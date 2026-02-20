var EnumTipoCobrancaVendaDiretaHelper = function () {
    this.NaoInformado = -1;
    this.Todos = 0;
    this.Boleto = 1;
    this.Cartao = 2;
    this.Avista = 3;
    this.PIX = 4;
    this.Bonificado = 5;    
    this.MaqCartao = 6;
    this.Site = 7;
    this.Confirmar = 8;
};

EnumTipoCobrancaVendaDiretaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Não informado", value: this.NaoInformado },
            { text: "Boleto", value: this.Boleto },
            { text: "Cartão", value: this.Cartao },
            { text: "À vista", value: this.Avista },
            { text: "PIX", value: this.PIX },
            { text: "Bonificado", value: this.Bonificado },
            { text: "Maq de Cartão", value: this.MaqCartao },
            { text: "Site", value: this.Site },
            { text: "Confirmar", value: this.Confirmar },
        ];
    },
    obterOpcoesServico: function () {
        return [
            { text: "Boleto", value: this.Boleto },
            { text: "Cartão", value: this.Cartao },            
            { text: "PIX", value: this.PIX },
            { text: "Bonificado", value: this.Bonificado },
            { text: "Maq de Cartão", value: this.MaqCartao },
            { text: "Site", value: this.Site },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoCobrancaVendaDireta = Object.freeze(new EnumTipoCobrancaVendaDiretaHelper());