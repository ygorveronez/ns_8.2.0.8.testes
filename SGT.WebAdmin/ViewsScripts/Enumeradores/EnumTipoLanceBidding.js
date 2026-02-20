let EnumTipoLanceBiddingHelper = function () {
    this.NaoSelecionado = 0;
    this.LancePorEquipamento = 1;
    this.LanceFrotaFixaKmRodado = 2;
    this.LancePorcentagemNota = 3;
    this.LanceViagemAdicional = 4;
    this.LancePorPeso = 5;
    this.LancePorCapacidade = 6;
    this.LancePorFreteViagem = 7;
    this.LanceFrotaFixaFranquia = 8;
    this.LancePorViagemEntregaAjudante = 9;
};

EnumTipoLanceBiddingHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "Não Selecionado", value: this.NaoSelecionado },
            { text: "Lance por Equipamento", value: this.LancePorEquipamento },
            { text: "Lance Frota Fixa Km Rodado", value: this.LanceFrotaFixaKmRodado },
            { text: "Lance Porcentagem Nota", value: this.LancePorcentagemNota },
            { text: "Lance Viagem Adicional", value: this.LanceViagemAdicional },
            { text: "Lance por Peso", value: this.LancePorPeso },
            { text: "Lance por Capacidade", value: this.LancePorCapacidade },
            { text: "Lance por Frete Viagem", value: this.LancePorFreteViagem },
            { text: "Lance Frota Fixa Franquia", value: this.LanceFrotaFixaFranquia },
            { text: "Lance por Viagem Entrega Ajudante", value: this.LancePorViagemEntregaAjudante }
        ];
    }
};

let EnumTipoLanceBidding = Object.freeze(new EnumTipoLanceBiddingHelper());