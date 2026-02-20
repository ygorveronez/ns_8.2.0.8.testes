let EnumIdentificadorUnicoViagemApiSulLogHelper = function () {
    this.CodIntegracaoCidadeUF = 0;
    this.CodIntegracaoNomeCliente = 1;
};

EnumIdentificadorUnicoViagemApiSulLogHelper.prototype = {
    obterOpcoes: function () {
        return [
            {
                value: this.CodIntegracaoCidadeUF,
                text: "Cód. de Integração - Cidade/UF"
            }, {
                value: this.CodIntegracaoNomeCliente,
                text: "Cód. de Integração - Nome Cliente"
            }
        ];
    }
}

let EnumIdentificadorUnicoViagemApiSulLog = Object.freeze(new EnumIdentificadorUnicoViagemApiSulLogHelper());
