var EnumEmailEnviarParaHelper = function () {
    this.Transportador = 1;
    this.Fornecedor = 2;
    this.OperadorME = 3;
};

EnumEmailEnviarParaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Transportador", value: this.Transportador },
            { text: "Fornecedor", value: this.Fornecedor },
            //{ text: "Operador ME", value: this.OperadorME }
        ];
    }
}

var EnumEmailEnviarPara = Object.freeze(new EnumEmailEnviarParaHelper());
