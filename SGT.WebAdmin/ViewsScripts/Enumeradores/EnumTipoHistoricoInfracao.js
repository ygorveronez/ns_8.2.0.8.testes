var EnumTipoHistoricoInfracaoHelper = function () {
    this.Todos = "";
    this.Pago = 1;
    this.Recurso = 2;
    this.Deferido = 3;
    this.Indeferido = 4;
    this.EmAberto = 5;
    this.EmProcesso = 6;
    this.EnvioRecibo = 7;
    this.ReciboAssinado = 8;
    this.Finalizado = 9;
};

EnumTipoHistoricoInfracaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Deferido", value: this.Deferido },
            { text: "Em Aberto", value: this.EmAberto },
            { text: "Em Processo", value: this.EmProcesso },
            { text: "Indeferido", value: this.Indeferido },
            { text: "Pago", value: this.Pago },
            { text: "Recurso", value: this.Recurso },
            { text: "Envio de Recibo", value: this.EnvioRecibo },
            { text: "Recibo Assinado", value: this.ReciboAssinado }
        ];
    },
    obterOpcoesFluxoSinistro: function () {
        return this.obterOpcoes().concat([{ text: "Finalizado", value: this.Finalizado }]);
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    },
    obterOpcoesFluxoSinistroPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoesFluxoSinistro());
    }
};

var EnumTipoHistoricoInfracao = Object.freeze(new EnumTipoHistoricoInfracaoHelper());