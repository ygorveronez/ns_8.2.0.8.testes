var EnumPerfilEmpresaHelper = function () {
    this.Todos = "";
    this.A = 0;
    this.B = 1;
    this.C = 2;
};

EnumPerfilEmpresaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Perfil A", value: this.A },
            { text: "Perfil B", value: this.B },
            { text: "Perfil C", value: this.C }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumPerfilEmpresa = Object.freeze(new EnumPerfilEmpresaHelper());