var EnumTipoClienteHelper = function () {
    this.Pessoa = 1;
    this.GrupoPessoa = 2;
    this.CategoriaPessoa = 3;
};

EnumTipoClienteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Pessoa (CNPJ/CPF)", value: this.Pessoa },
            { text: "Grupo Pessoa", value: this.GrupoPessoa },
            { text: "Categoria Pessoa", value: this.CategoriaPessoa }
        ];
    },
    obterOpcoesSemCategoria: function () {
        return [
            { text: "Pessoa (CNPJ/CPF)", value: this.Pessoa },
            { text: "Grupo Pessoa", value: this.GrupoPessoa }
        ];
    }
};

var EnumTipoCliente = Object.freeze(new EnumTipoClienteHelper());