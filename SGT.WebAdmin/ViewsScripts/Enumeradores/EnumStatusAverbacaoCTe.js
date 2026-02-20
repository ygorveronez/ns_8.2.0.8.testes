var EnumStatusAverbacaoCTeHelper = function () {
    this.Pendente = 0;
    this.Sucesso = 1;
    this.Cancelado = 2;
    this.Enviado = 3;
    this.AgEmissao = 4;
    this.AgCancelamento = 5;
    this.Rejeicao = 9;
    this.Todos = 99;
};

EnumStatusAverbacaoCTeHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.StatusAverbacaoCTe.Averbado, value: this.Sucesso },
            { text: Localization.Resources.Enumeradores.StatusAverbacaoCTe.Cancelado, value: this.Cancelado },
            { text: Localization.Resources.Enumeradores.StatusAverbacaoCTe.Enviado, value: this.Enviado },
            { text: Localization.Resources.Enumeradores.StatusAverbacaoCTe.Pendente, value: this.Pendente },
            { text: Localization.Resources.Enumeradores.StatusAverbacaoCTe.Rejeitado, value: this.Rejeicao }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.StatusAverbacaoCTe.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumStatusAverbacaoCTe = Object.freeze(new EnumStatusAverbacaoCTeHelper());