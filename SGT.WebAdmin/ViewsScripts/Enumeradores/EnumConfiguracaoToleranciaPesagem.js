let EnumConfiguracaoToleranciaPesagemHelper = function () {
    this.Todas = 0;
    this.Ativo = 1;
    this.Inativo = 2;    
};

EnumConfiguracaoToleranciaPesagemHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "Ativo", value: this.Ativo },
            { text: "Inativo", value: this.Inativo },           
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.ObterOpcoes());
    }
};

let EnumConfiguracaoToleranciaPesagem = Object.freeze(new EnumConfiguracaoToleranciaPesagemHelper());