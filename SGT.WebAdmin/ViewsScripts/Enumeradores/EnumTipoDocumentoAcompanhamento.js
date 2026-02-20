var EnumTipoDocumentoAcompanhamentoHelper = function () {
    this.SemTipo = 0;
    this.Pagoviacreditoemconta = 1;
    this.PendentesemAberto = 2;
    this.Debitoscompensados = 3;
    this.PagoviaConfirming = 4;
    this.NotasCompensadasXAdiantamento = 5;
    this.Cockpit = 6;
    this.BaixaResultado = 7;
    this.Descontos = 8;
    this.TotaldeAdiantamento = 9;
};

EnumTipoDocumentoAcompanhamentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Sem Tipo", value: this.SemTipo },
            { text: "Pago via cr\u00E9dito em conta", value: this.Pagoviacreditoemconta },
            { text: "Pendentes em aberto", value: this.PendentesemAberto },
            { text: `D\u00E9bitos compensados`, value: this.Debitoscompensados },
            { text: "Pago via confirming", value: this.PagoviaConfirming },
            { text: "Notas compensadas x adiantamento", value: this.NotasCompensadasXAdiantamento },
            { text: "Cockpit", value: this.Cockpit },
            { text: "Baixa resultado", value: this.BaixaResultado },
            //{ text: "Descontos", value: this.Descontos },
            { text: "Total de adiantamento", value: this.TotaldeAdiantamento },
           
        ];
    }
};

var EnumTipoDocumentoAcompanhamento = Object.freeze(new EnumTipoDocumentoAcompanhamentoHelper());