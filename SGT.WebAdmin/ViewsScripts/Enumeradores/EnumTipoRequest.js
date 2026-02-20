let EnumTipoRequestHelper = function () {
    this.Todos = "";
    this.AdicionarPedidoEmLote = 0;
    this.GerarCarregamento = 1;
    this.GerarCarregamentoComRedespachos = 2;
    this.EnviarDigitalizacaoCanhotoEmLote = 3;
    this.AdicionarAtendimentoEmLote = 4;
    this.GerarCarregamentoRoteirizacaoEmLote = 5;
};

EnumTipoRequestHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Adicionar Pedido em Lote", value: this.AdicionarPedidoEmLote },
            { text: "Gerar Carregamento", value: this.GerarCarregamento },
            { text: "Gerar Carregamento com Redespachos", value: this.GerarCarregamentoComRedespachos },
            { text: "Enviar Digitalização Canhoto Em Lote", value: this.EnviarDigitalizacaoCanhotoEmLote },
            { text: "Adicionar Atendimento Em Lote", value: this.AdicionarAtendimentoEmLote },
            { text: "Gerar Carregamento Roteirização Em Lote", value: this.GerarCarregamentoRoteirizacaoEmLote },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.SituacaoPedido.Todos, value: this.Todos }].concat(this.obterOpcoes());
    },
};

let EnumTipoRequest = Object.freeze(new EnumTipoRequestHelper());

