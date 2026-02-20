var EnumEtapaLiberacaoPagamentoProvedorHelper = function () {
    this.Todos = 0;
    this.DetalhesCarga = 1;
    this.DocumentoProvedor = 2;
    this.Aprovacao = 3;
    this.Liberacao = 4;
    this.Cancelado = 5;
};

EnumEtapaLiberacaoPagamentoProvedorHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Todos", value: this.Todos },
            { text: "Documentos Provedor", value: this.DocumentoProvedor },
            { text: "Aprovação", value: this.Aprovacao },
            { text: "Liberação", value: this.Liberacao },
        ];
    }
};

var EnumEtapaLiberacaoPagamentoProvedor = Object.freeze(new EnumEtapaLiberacaoPagamentoProvedorHelper());