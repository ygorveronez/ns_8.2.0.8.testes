var EnumTipoOcorrenciaPatioHelper = function () {
    this.Todos = "";
    this.NaoInformado = 0;
    this.Checklist = 1;
    this.Higienizacao = 2;
}

EnumTipoOcorrenciaPatioHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Checklist", value: this.Checklist },
            { text: "Higienização", value: this.Higienizacao },
            { text: "Não Informado", value: this.NaoInformado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoOcorrenciaPatio = Object.freeze(new EnumTipoOcorrenciaPatioHelper());