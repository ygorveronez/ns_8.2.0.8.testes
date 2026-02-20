var EnumTipoPercursoHelper = function () {
    this.Todos = 0;
    this.Ida = 1;
    this.Retorno = 2;

};


EnumTipoPercursoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Ida", value: this.Ida },
            { text: "Retorno", value: this.Retorno }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    },

};


var EnumTipoPercurso = Object.freeze(new EnumTipoPercursoHelper());