var EnumTipoGrupoPessoasHelper = function () {
    this.Ambos = 0;
    this.Clientes = 1;
    this.Fornecedores = 2;
};

EnumTipoGrupoPessoasHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoGrupoPessoas.Ambos, value: this.Ambos },
            { text: Localization.Resources.Enumeradores.TipoGrupoPessoas.Clientes, value: this.Clientes },
            { text: Localization.Resources.Enumeradores.TipoGrupoPessoas.Fornecedores, value: this.Fornecedores },
        ];
    },
};

var EnumTipoGrupoPessoas = Object.freeze(new EnumTipoGrupoPessoasHelper());