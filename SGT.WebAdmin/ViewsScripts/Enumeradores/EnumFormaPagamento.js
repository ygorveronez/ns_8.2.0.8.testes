var EnumFormaPagamentoHelper = function () {
    this.Nenhum = 0;
    this.Avista = 1;
    this.GerarTituloAutomaticamente = 2;
};

EnumFormaPagamentoHelper.prototype = {
    obterOpcoes: function (opcaoSelecione) {
        var arrayOpcoes = [
            { text: 'Nenhum', value: this.Nenhum },
            { text: "Á vista", value: this.Avista },
            { text: "Gerar Título Automaticamente", value: this.GerarTituloAutomaticamente },
        ];

        return arrayOpcoes;
    }
};

var EnumFormaPagamento = Object.freeze(new EnumFormaPagamentoHelper());