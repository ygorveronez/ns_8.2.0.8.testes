var EnumConsultaPorEntregaStatusHelper = function () {
    this.Todos = "";
    this.NovaCarga = "NOVA CARGA";
    this.Faturada = "FATURADA";
    this.EmTransporte = "EM TRANSPORTE";
    this.EmAtendimento = "EM ANTEDIMENTO";
    this.Entregue = "ENTREGUE";
    this.Rejeitada = "REJEITADA";
};

EnumConsultaPorEntregaStatusHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Nova carga", value: this.NovaCarga },
            { text: "Faturada", value: this.Faturada },
            { text: "Em Transporte", value: this.EmTransporte },
            { text: "Em Atendimento", value: this.EmAtendimento },
            { text: "Entregue", value: this.Entregue },
            { text: "Rejeitada", value: this.Rejeitada },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumConsultaPorEntregaStatus = Object.freeze(new EnumConsultaPorEntregaStatusHelper());