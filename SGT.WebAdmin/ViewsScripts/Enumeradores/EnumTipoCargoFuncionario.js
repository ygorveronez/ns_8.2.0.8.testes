var EnumTipoCargoFuncionarioHelper = function () {
    this.Todos = 0;
    this.Outros = 1;
    this.Mecanico = 2;
};

EnumTipoCargoFuncionarioHelper.prototype = {
    obterDescricao: function (valor) {
        switch (valor) {
            case this.Todos: return "Todos";
            case this.Outros: return "Outros";
            case this.Mecanico: return "Mecânico";
            default: return "";
        }
    },
    obterOpcoes: function () {
        return [
            { text: "Outros", value: this.Outros },
            { text: "Mecânico", value: this.Mecanico }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [
            { text: "Todos", value: this.Todos },
            { text: "Outros", value: this.Outros },
            { text: "Mecânico", value: this.Mecanico }
        ];
    }
};

var EnumTipoCargoFuncionario = Object.freeze(new EnumTipoCargoFuncionarioHelper());