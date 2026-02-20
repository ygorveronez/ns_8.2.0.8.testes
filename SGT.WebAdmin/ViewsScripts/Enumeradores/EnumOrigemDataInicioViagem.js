let EnumOrigemDataInicioViagemHelper = function () {
    this.DataEnvioIntegracao = 0;
    this.DataCarregamentoCarga = 1;
};

EnumOrigemDataInicioViagemHelper.prototype = {
    obterOpcoes: function () {
        return [
            {
                value: this.DataEnvioIntegracao,
                text: "Data de Envio para Integração"
            }, {
                value: this.DataCarregamentoCarga,
                text: "Data de Carregamento da Carga"
            }
        ];
    }
}

let EnumOrigemDataInicioViagem = Object.freeze(new EnumOrigemDataInicioViagemHelper());