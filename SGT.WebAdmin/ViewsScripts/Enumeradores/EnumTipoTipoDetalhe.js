var EnumTipoTipoDetalheHelper = function () {
    this.Todos = 0;
    this.ProcessamentoEspecial = 1;
    this.HorarioEntrega = 2;
    this.ZonaTransporte = 3;
    this.PeriodoEntrega = 4;
    this.DetalheEntrega = 5;
};


EnumTipoTipoDetalheHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Processamento Especial", value: this.ProcessamentoEspecial },
            { text: "Horário Entrega", value: this.HorarioEntrega },
            { text: "Zona de Transporte", value: this.ZonaTransporte },
            { text: "Período de Entrega", value: this.PeriodoEntrega },
            { text: "Detalhe de Entrega", value: this.DetalheEntrega }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    },

};


var EnumTipoTipoDetalhe = Object.freeze(new EnumTipoTipoDetalheHelper());