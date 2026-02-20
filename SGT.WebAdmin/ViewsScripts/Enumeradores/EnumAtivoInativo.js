var EnumAtivoInativoHelper = function () {
    this.Todos = null;
    this.Inativo = 0;
    this.Ativo = 1;
};

EnumAtivoInativoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Inativo", value: this.Inativo },
            { text: "Ativo", value: this.Ativo }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumAtivoInativo = Object.freeze(new EnumAtivoInativoHelper());
