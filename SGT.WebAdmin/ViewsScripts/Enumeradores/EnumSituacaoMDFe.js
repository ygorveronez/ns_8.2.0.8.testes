var EnumSituacaoMDFeHelper = function () {
    this.Todos = -1;
    this.EmDigitacao = 0;
    this.Pendente = 1;
    this.Enviado = 2;
    this.Autorizado = 3;
    this.EmEncerramento = 4;
    this.Encerrado = 5;
    this.EmCancelamento = 6;
    this.Cancelado = 7;
    this.Rejeicao = 9;
    this.EmitidoContingencia = 10;
    this.AguardandoCompraValePedagio = 11;
    this.EventoInclusaoMotoristaEnviado = 12;
};

EnumSituacaoMDFeHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacaoMDFe.AguardandoCompraValePedagio, value: this.AguardandoCompraValePedagio },
            { text: Localization.Resources.Enumeradores.SituacaoMDFe.Autorizado, value: this.Autorizado },
            { text: Localization.Resources.Enumeradores.SituacaoMDFe.Cancelado, value: this.Cancelado },
            { text: Localization.Resources.Enumeradores.SituacaoMDFe.Continencia, value: this.EmitidoContingencia },
            { text: Localization.Resources.Enumeradores.SituacaoMDFe.EmCancelamento, value: this.EmCancelamento },
            { text: Localization.Resources.Enumeradores.SituacaoMDFe.EmDigitacao, value: this.EmDigitacao },
            { text: Localization.Resources.Enumeradores.SituacaoMDFe.EmEncerramento, value: this.EmEncerramento },
            { text: Localization.Resources.Enumeradores.SituacaoMDFe.Encerrado, value: this.Encerrado },
            { text: Localization.Resources.Enumeradores.SituacaoMDFe.Enviado, value: this.Enviado },
            { text: Localization.Resources.Enumeradores.SituacaoMDFe.Pendente, value: this.Pendente },
            { text: Localization.Resources.Enumeradores.SituacaoMDFe.Rejeicao, value: this.Rejeicao },
            { text: Localization.Resources.Enumeradores.SituacaoMDFe.AguardandoInclusaoDeMotorista, value: this.EventoInclusaoMotoristaEnviado },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumSituacaoMDFe = Object.freeze(new EnumSituacaoMDFeHelper());