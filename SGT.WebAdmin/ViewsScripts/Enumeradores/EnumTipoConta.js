var EnumTipoContaHelper = function () {
    this.Todos = "";
    this.Corrente = 1;
    this.Poupanca = 2;
    this.Salario = 3;
};

EnumTipoContaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoConta.ContaCorrente, value: this.Corrente },
            { text: Localization.Resources.Enumeradores.TipoConta.ContaPoupanca, value: this.Poupanca },
            { text: Localization.Resources.Enumeradores.TipoConta.ContaSalario, value: this.Salario }
        ];
    },
    obterOpcoesBordero: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoConta.ContaCorrente, value: this.Corrente },
            { text: Localization.Resources.Enumeradores.TipoConta.ContaPoupanca, value: this.Poupanca }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoConta.Todos, value: this.Todos }].concat(this.obterOpcoes());
    },
    obterOpcoesPesquisaBordero: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoConta.Todos, value: this.Todos }].concat(this.obterOpcoesBordero());
    }
}

var EnumTipoConta = Object.freeze(new EnumTipoContaHelper());