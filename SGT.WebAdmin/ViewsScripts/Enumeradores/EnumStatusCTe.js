var EnumStatusCTeHelper = function () {
    this.TODOS = "";
    this.PENDENTE = "P";
    this.ENVIADO = "E";
    this.REJEICAO = "R";
    this.AUTORIZADO = "A";
    this.CANCELADO = "C";
    this.INUTILIZADO = "I";
    this.DENEGADO = "D";
    this.EMDIGITACAO = "S";
    this.EMCANCELAMENTO = "K";
    this.EMINUTILIZACAO = "L";
    this.ANULADO = "Z";
    this.AGUARDANDOFINALIZARCARGA = "Y";
    this.FSDA = "F";
    this.EMCONTINGENCIA = "Q";
};

EnumStatusCTeHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.StatusCTe.Todos, value: this.TODOS },
            { text: Localization.Resources.Enumeradores.StatusCTe.Autorizado, value: this.AUTORIZADO },
            { text: Localization.Resources.Enumeradores.StatusCTe.Pendente, value: this.PENDENTE },
            { text: Localization.Resources.Enumeradores.StatusCTe.Enviado, value: this.ENVIADO },
            { text: Localization.Resources.Enumeradores.StatusCTe.Rejeitado, value: this.REJEICAO },
            { text: Localization.Resources.Enumeradores.StatusCTe.Cancelado, value: this.CANCELADO },
            { text: Localization.Resources.Enumeradores.StatusCTe.Inutilizado, value: this.INUTILIZADO },
            { text: Localization.Resources.Enumeradores.StatusCTe.Denegado, value: this.DENEGADO },
            { text: Localization.Resources.Enumeradores.StatusCTe.EmDigitacao, value: this.EMDIGITACAO },
            { text: Localization.Resources.Enumeradores.StatusCTe.EmCancelamento, value: this.EMCANCELAMENTO },
            { text: Localization.Resources.Enumeradores.StatusCTe.EmInutilizacao, value: this.EMINUTILIZACAO },
            { text: Localization.Resources.Enumeradores.StatusCTe.EmContingencia, value: this.EMCONTINGENCIA },
        ];
    }
};

var EnumStatusCTe = Object.freeze(new EnumStatusCTeHelper());