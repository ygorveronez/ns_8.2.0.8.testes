var EnumPessoaLocalidadeHelper = function () {
    this.Pessoa = 1;
    this.Localidade = 2;
};

EnumPessoaLocalidadeHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Pessoa", value: this.Pessoa },
            { text: "Localidade", value: this.Localidade }
        ];
    }
};

var EnumPessoaLocalidade = Object.freeze(new EnumPessoaLocalidadeHelper());