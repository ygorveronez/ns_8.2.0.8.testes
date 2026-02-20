var EnumSituacaoPreCargaOfertaTransportadorHelper = function () {
    this.Todas = "";
    this.Disponivel = 0;
    this.AguardandoAceite = 1;
    this.AguardandoConfirmacao = 2;
    this.Confirmada = 3;
    this.Rejeitada = 4;
};

EnumSituacaoPreCargaOfertaTransportadorHelper.prototype = {
    obterClasseCor: function (situacao) {
        switch (situacao) {
            case this.AguardandoAceite: return "well-burlywood";
            case this.AguardandoConfirmacao: return "well-orange";
            case this.Confirmada: return "well-green";
            case this.Disponivel: return "well-white";
            case this.Rejeitada: return "well-red";
            default: return "";
        }
    },
    obterOpcoes: function () {
        return [
            { text: Localizations.Resources.Enumeradores.SituacaoPreEntrega.AguardandoAceite, value: this.AguardandoAceite },
            { text: Localizations.Resources.Enumeradores.SituacaoPreEntrega.AguardandoConfirmacao, value: this.AguardandoConfirmacao },
            { text: Localizations.Resources.Enumeradores.SituacaoPreEntrega.Confirmada, value: this.Confirmada },
            { text: Localizations.Resources.Enumeradores.SituacaoPreEntrega.Disponivel, value: this.Disponivel },
            { text: Localizations.Resources.Enumeradores.SituacaoPreEntrega.Rejeitada, value: this.Rejeitada }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localizations.Resources.Enumeradores.SituacaoPreEntrega.Todas, value: this.Todas }].concat(this.obterOpcoes());
    },
}

var EnumSituacaoPreCargaOfertaTransportador = Object.freeze(new EnumSituacaoPreCargaOfertaTransportadorHelper());
