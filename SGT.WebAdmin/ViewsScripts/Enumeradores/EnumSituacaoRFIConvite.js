let EnumSituacaoRFIConviteHelper = function () {
    this.Todas = "";
    this.Aguardando = 0;
    this.Checklist = 1;
    this.AguardandoAprovacao = 2;
    this.AprovacaoRejeitada = 3;
    this.Fechamento = 4;
};

EnumSituacaoRFIConviteHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "Aguardando", value: this.Aguardando },
            { text: "Checklist", value: this.Checklist },
            { text: "Aguardando Aprovação", value: this.AguardandoAprovacao },
            { text: "Aprovação Rejeitada", value: this.AprovacaoRejeitada },
            { text: "Fechamento", value: this.Fechamento },
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.ObterOpcoes());
    }
};

let EnumSituacaoRFIConvite = Object.freeze(new EnumSituacaoRFIConviteHelper());