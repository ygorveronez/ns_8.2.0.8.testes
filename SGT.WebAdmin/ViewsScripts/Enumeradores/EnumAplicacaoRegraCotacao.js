var EnumAplicacaoRegraCotacaoHelper = function () {
    this.ExcluirTransportador = 1;
    this.UsarTransportador = 2;
    this.ValorPercentualCobrancaCliente = 3;
    this.AdicionarDiasAoFrete = 4;
    this.FixarDiasDeFrete = 5;
    this.FixarValorCotacaoFrete = 6;
    this.ExcluirCotacao = 7;
    this.UtilizarModeloVeicular = 8;
    this.ValorParaCobranca = 9
};

EnumAplicacaoRegraCotacaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Adicionar Dias ao Frete", value: this.AdicionarDiasAoFrete },
            { text: "Excluir Cotação", value: this.ExcluirCotacao },
            { text: "Excluir Transportador", value: this.ExcluirTransportador },
            { text: "Fixar Dias de Frete", value: this.FixarDiasDeFrete },
            { text: "Fixar Valor da Cotação do Frete", value: this.FixarValorCotacaoFrete },
            { text: "Usar Transportador", value: this.UsarTransportador },
            { text: "Valor Percentual para Cobrança", value: this.ValorPercentualCobrancaCliente },
            { text: "Utilizar modelo veicular", value: this.UtilizarModeloVeicular },
            { text: "Valor para Cobrança", value: this.ValorParaCobranca }
        ];
    }
};

var EnumAplicacaoRegraCotacao = Object.freeze(new EnumAplicacaoRegraCotacaoHelper());
