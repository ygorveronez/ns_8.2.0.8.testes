var EnumTipoIntegracaoComproveiHelper = function () {
    this.AgendamentoEntrega = 1;
    this.BaixaDocumentos = 2;
};

EnumTipoIntegracaoComproveiHelper.prototype = {
    obterOpcoes: function (opcaoSelecione) {
        var arrayOpcoes = [
            { text: 'Agendamento de Entrega', value: this.AgendamentoEntrega },
            { text: "Baixa de Documentos", value: this.AgendamentoEntrega },
        ];

        return arrayOpcoes;
    }
};

var EnumTipoIntegracaoComprovei = Object.freeze(new EnumTipoIntegracaoComproveiHelper());