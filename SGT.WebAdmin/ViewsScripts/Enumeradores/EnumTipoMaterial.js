var EnumTipoMaterialHelper = function () {
    this.Todos = "";
    this.NaoDefinido = 0;
    this.Aco = 1;
    this.Aluminio = 2;
    this.Madeira = 3;
};

EnumTipoMaterialHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Não Definido", value: this.NaoDefinido },
            { text: "Aço", value: this.Aco },
            { text: "Aluminio", value: this.Aluminio },
            { text: "Madeira", value: this.Madeira },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoMaterial = Object.freeze(new EnumTipoMaterialHelper());