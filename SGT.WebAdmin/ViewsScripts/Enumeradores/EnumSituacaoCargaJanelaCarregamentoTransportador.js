var EnumSituacaoCargaJanelaCarregamentoTransportadorHelper = function () {
    this.Todas = "";
    this.Disponivel = 0;
    this.ComInteresse = 1;
    this.AgConfirmacao = 2;
    this.Confirmada = 3;
    this.Rejeitada = 4;
    this.AgAceite = 5;
};

EnumSituacaoCargaJanelaCarregamentoTransportadorHelper.prototype = {
    obterClasseCor: function (situacao) {
        switch (situacao) {
            case this.AgAceite: return "burlywood";
            case this.AgConfirmacao: return "orange";
            case this.ComInteresse: return "yellow";
            case this.Confirmada: return "green";
            case this.Disponivel: return "white";
            case this.Rejeitada: return "red";
            default: return "";
        }
    },
    obterOpcoes: function () {
        var opcoes = [];

        if (_CONFIGURACAO_TMS.PossuiSituacaoAguardandoAceite)
            opcoes.push({ text: "Aguardando Aceite", value: this.AgAceite });

        opcoes.push({ text: "Aguardando Confirmação", value: this.AgConfirmacao });
        opcoes.push({ text: "Com Interesse", value: this.ComInteresse });
        opcoes.push({ text: "Confirmada", value: this.Confirmada });
        opcoes.push({ text: "Disponível", value: this.Disponivel });
        opcoes.push({ text: "Rejeitada", value: this.Rejeitada });

        return opcoes;
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    },
}

var EnumSituacaoCargaJanelaCarregamentoTransportador = Object.freeze(new EnumSituacaoCargaJanelaCarregamentoTransportadorHelper());
