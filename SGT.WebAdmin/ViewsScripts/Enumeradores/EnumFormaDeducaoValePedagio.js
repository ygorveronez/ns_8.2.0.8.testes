var EnumFormaDeducaoValePedagioHelper = function () {
    this.NaoAplicado = 0;
    this.ReduzirValorFrete = 1;
    this.AcrescentarValorFrete = 2;
};

EnumFormaDeducaoValePedagioHelper.prototype = {
    obterOpcoes: function (opcaoSelecionada) {
        var arrayOpcoes = [
            { text: "Não Aplicado", value: EnumFormaDeducaoValePedagio.NaoAplicado },
            { text: "Reduzir do valor do frete", value: EnumFormaDeducaoValePedagio.ReduzirValorFrete },
            { text: "Acrescentar o valor ao frete", value: EnumFormaDeducaoValePedagio.AcrescentarValorFrete },
        ];

        return arrayOpcoes;
    }
};

var EnumFormaDeducaoValePedagio = Object.freeze(new EnumFormaDeducaoValePedagioHelper());