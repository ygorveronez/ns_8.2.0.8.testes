var EnumSituacaoEntregaHelper = function () {
    this.Todos = "";
    this.NaoRealizado = -1;
    this.NaoEntregue = 0;
    this.EmCliente = 1;
    this.Entregue = 2;
    this.Rejeitado = 3;
    this.Revertida = 4;
    this.Reentergue = 5;
    this.AgAtendimento = 6;
    this.EntregarEmOutroCliente = 7;
    this.DescartarMercadoria = 8;
    this.QuebraPeso = 9;
    this.ReentregarMesmaCarga = 10;
    this.EmFinalizacao = 11;
};

EnumSituacaoEntregaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacaoEntrega.NaoEntregue, value: this.NaoEntregue },
            { text: Localization.Resources.Enumeradores.SituacaoEntrega.NoLocal, value: this.EmCliente },
            { text: Localization.Resources.Enumeradores.SituacaoEntrega.Realizado, value: this.Entregue },
            { text: Localization.Resources.Enumeradores.SituacaoEntrega.Rejeitado, value: this.Rejeitado },
            { text: Localization.Resources.Enumeradores.SituacaoEntrega.DevolucaoRevertida, value: this.Revertida },
            { text: Localization.Resources.Enumeradores.SituacaoEntrega.Reentregue, value: this.Reentergue },
            { text: Localization.Resources.Enumeradores.SituacaoEntrega.AgAtendimento, value: this.AgAtendimento },
            { text: Localization.Resources.Enumeradores.SituacaoEntrega.EntregarEmOutroCliente, value: this.EntregarEmOutroCliente },
            { text: Localization.Resources.Enumeradores.SituacaoEntrega.DescartarMercadoria, value: this.DescartarMercadoria },
            { text: Localization.Resources.Enumeradores.SituacaoEntrega.QuebraPeso, value: this.QuebraPeso },
            { text: Localization.Resources.Enumeradores.SituacaoEntrega.ReentregarMesmaCarga, value: this.ReentregarMesmaCarga },
            { text: Localization.Resources.Enumeradores.SituacaoEntrega.EmFinalizacao, value: this.EmFinalizacao },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.SituacaoEntrega.Todos, value: this.Todos }].concat(this.obterOpcoes());
    },
    obterOpcoesPesquisaContainer: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacaoEntrega.Entregue, value: this.Entregue },
            { text: Localization.Resources.Enumeradores.SituacaoEntrega.NaoEntregue, value: this.NaoEntregue }
        ];
    }
};

var EnumSituacaoEntrega = Object.freeze(new EnumSituacaoEntregaHelper());