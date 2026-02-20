var EnumTipoUsuarioHelper = function () {
    this.Todos = 0;
    this.Funcionarios = 1;
    this.Pessoas = 2;
};

EnumTipoUsuarioHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Funcionários", value: this.Funcionarios },
            { text: "Pessoas", value: this.Pessoas }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumTipoUsuario = Object.freeze(new EnumTipoUsuarioHelper());