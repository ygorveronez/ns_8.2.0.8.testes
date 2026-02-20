var EnumSituacaoCTeHelper = function () {
    this.Autorizado = "A";
    this.Pendente = "P";
    this.Enviado = "E";
    this.Rejeitado = "R";
    this.Cancelado = "C";
    this.AnuladoGerencialmente = "G";
    this.Inutilizado = "I";
    this.Denegado = "D";
    this.EmDigitacao = "S";
    this.EmCancelamento = "K";
    this.EmInutilizacao = "L";
    this.Anulado = "Z";
}

EnumSituacaoCTeHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Autorizado", value: this.Autorizado },
            { text: "Pendente", value: this.Pendente },
            { text: "Enviado", value: this.Enviado },
            { text: "Rejeitado", value: this.Rejeitado },
            { text: "Cancelado", value: this.Cancelado },
            { text: "Anulado Gerencialmente", value: this.AnuladoGerencialmente },
            { text: "Inutilizado", value: this.Inutilizado },
            { text: "Denegado", value: this.Denegado },
            { text: "Em Digitação", value: this.EmDigitacao },
            { text: "Em Cancelamento", value: this.EmCancelamento },
            { text: "Em Inutilização", value: this.EmInutilizacao },
            { text: "Anulado", value: this.Anulado },
        ];
    }
}

var EnumSituacaoCTe = Object.freeze(new EnumSituacaoCTeHelper());