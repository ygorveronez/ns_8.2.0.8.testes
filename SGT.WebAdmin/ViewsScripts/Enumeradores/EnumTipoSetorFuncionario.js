var EnumTipoSetorFuncionarioHelper = function () {
    this.NaoInformado = 0;
    this.Planejamento = 1;
}

EnumTipoSetorFuncionarioHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Não informado", value: this.NaoInformado },
            { text: "Planejamento", value: this.Planejamento },
        ];
    },
}

var EnumTipoSetorFuncionario = Object.freeze(new EnumTipoSetorFuncionarioHelper());