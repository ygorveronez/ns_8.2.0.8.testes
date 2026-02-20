var EnumSituacaoOrdemServicoFrotaHelper = function () {
    this.Todas = "";
    this.EmDigitacao = 0;
    this.AgAutorizacao = 1;
    this.Rejeitada = 2;
    this.EmManutencao = 3;
    this.DivergenciaOrcadoRealizado = 4;
    this.Finalizada = 5;
    this.Cancelada = 6;
    this.AgNotaFiscal = 7;
    this.SemRegraAprovacao = 8;
    this.AguardandoAprovacao = 9;
    this.AprovacaoRejeitada = 10;
};

EnumSituacaoOrdemServicoFrotaHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "Em Digitação", value: this.EmDigitacao },
            { text: "Ag. Autorização", value: this.AgAutorizacao },
            { text: "Rejeitada", value: this.Rejeitada },
            { text: "Em Manutenção", value: this.EmManutencao },
            { text: "Divergência Orçado X Realizado", value: this.DivergenciaOrcadoRealizado },
            { text: "Finalizada", value: this.Finalizada },
            { text: "Cancelada", value: this.Cancelada },
            { text: "Aguardando Nota Fiscal", value: this.AgNotaFiscal },
            { text: "Sem Regra Aprovação", value: this.SemRegraAprovacao },
            { text: "Aguardando Aprovação", value: this.AguardandoAprovacao },
            { text: "Aprovação Rejeitada", value: this.AprovacaoRejeitada }
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.ObterOpcoes());
    }
};

var EnumSituacaoOrdemServicoFrota = Object.freeze(new EnumSituacaoOrdemServicoFrotaHelper());