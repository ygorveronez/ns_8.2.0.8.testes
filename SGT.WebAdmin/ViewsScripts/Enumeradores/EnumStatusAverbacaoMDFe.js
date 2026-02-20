var EnumStatusAverbacaoMDFeHelper = function () {
    this.Pendente = 0;
    this.Sucesso = 1;
    this.Cancelado = 2;
    this.Encerrado = 3;
    this.AgEmissao = 4;
    this.AgCancelamento = 5;
    this.AgEncerramento = 6;
    this.Rejeicao = 9;
};

EnumStatusAverbacaoMDFeHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.StatusAverbacaoMDFe.Pendente, value: this.Pendente },
            { text: Localization.Resources.Enumeradores.StatusAverbacaoMDFe.Averbado, value: this.Sucesso },
            { text: Localization.Resources.Enumeradores.StatusAverbacaoMDFe.Cancelado, value: this.Cancelado },
            { text: Localization.Resources.Enumeradores.StatusAverbacaoMDFe.Encerrado, value: this.Encerrado },
            { text: Localization.Resources.Enumeradores.StatusAverbacaoMDFe.AguardandoEmissao, value: this.AgEmissao },
            { text: Localization.Resources.Enumeradores.StatusAverbacaoMDFe.AguardandoCancelamento, value: this.AgCancelamento },
            { text: Localization.Resources.Enumeradores.StatusAverbacaoMDFe.AguardandoEncerramento, value: this.AgEncerramento },
            { text: Localization.Resources.Enumeradores.StatusAverbacaoMDFe.Rejeitado, value: this.Rejeicao }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: null}].concat(this.obterOpcoes());
    }
};

var EnumStatusAverbacaoMDFe = Object.freeze(new EnumStatusAverbacaoMDFeHelper());