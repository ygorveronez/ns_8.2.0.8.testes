var EnumTratativaDevolucaoHelper = function () {
    this.Todos = "";
    this.Rejeitada = 3;
    this.Revertida = 4;
    this.Reentregue = 5;
    this.EntregarEmOutroCliente = 7;
    this.DescartarMercadoria = 8;
    this.QuebraPeso = 9;
    this.ReentregarMesmaCarga = 10;
};

EnumTratativaDevolucaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TratativaDevolucao.AceitarDevolucao, value: this.Rejeitada },
            { text: Localization.Resources.Enumeradores.TratativaDevolucao.ReverterDevolucao, value: this.Revertida },
            { text: Localization.Resources.Enumeradores.TratativaDevolucao.EntregarEmOutroCliente, value: this.EntregarEmOutroCliente },
            { text: Localization.Resources.Enumeradores.TratativaDevolucao.DescartarMercadoria, value: this.DescartarMercadoria },
            { text: Localization.Resources.Enumeradores.TratativaDevolucao.QuebraPeso, value: this.QuebraPeso },
            { text: Localization.Resources.Enumeradores.TratativaDevolucao.Reentregar, value: this.Reentregue },
            { text: Localization.Resources.Enumeradores.TratativaDevolucao.ReentregarMesmaCarga, value: this.ReentregarMesmaCarga }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.TratativaDevolucao.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTratativaDevolucao = Object.freeze(new EnumTratativaDevolucaoHelper());