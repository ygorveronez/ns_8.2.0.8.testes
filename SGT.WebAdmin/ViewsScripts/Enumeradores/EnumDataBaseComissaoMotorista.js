
var EnumDataBaseComissaoMotoristaHelper = function () {
    this.DataEmissao = 1;
    this.DataCarregamento = 2;
};

EnumDataBaseComissaoMotoristaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Data de Emissão", value: this.DataEmissao },
            { text: "Data de Carregamento", value: this.DataCarregamento },
        ];
    },
};


var EnumDataBaseComissaoMotorista = Object.freeze(new EnumDataBaseComissaoMotoristaHelper());