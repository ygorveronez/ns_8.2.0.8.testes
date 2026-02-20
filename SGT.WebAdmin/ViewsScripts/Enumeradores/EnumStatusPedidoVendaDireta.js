var EnumStatusPedidoVendaDiretaHelper = function () {
    this.Todos = "";
    this.AgendadoFora = 1;
    this.Aprovado = 2;
    this.Baixado = 3;
    this.FaltaAgendar = 4;
    this.Agendado = 5;
    this.Contato1 = 6;
    this.Contato2 = 7;
    this.Contato3 = 8;
    this.Problema = 9;
    this.Reagendar = 10;
    this.ClienteBaixa = 11;
    this.AguardandoVerificacaoCertisign = 12;
};

EnumStatusPedidoVendaDiretaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Agendado Fora", value: this.AgendadoFora },
            { text: "Aprovado", value: this.Aprovado },
            { text: "Baixado", value: this.Baixado },
            { text: "Falta Agendar", value: this.FaltaAgendar },
            { text: "Agendado", value: this.Agendado },
            { text: "Contato 1", value: this.Contato1 },
            { text: "Contato 2", value: this.Contato2 },
            { text: "Contato 3", value: this.Contato3 },
            { text: "Problema", value: this.Problema },
            { text: "Reagendar", value: this.Reagendar },
            { text: "Cliente Baixa", value: this.ClienteBaixa },
            { text: "Aguardando Verificação Certisign", value: this.AguardandoVerificacaoCertisign }            
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumStatusPedidoVendaDireta = Object.freeze(new EnumStatusPedidoVendaDiretaHelper());