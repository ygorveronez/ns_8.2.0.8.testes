var EnumSituacaoFechamentoPalletsHelper = function () {
    this.Aberto = 1;
    this.Finalizado = 2;
}

EnumSituacaoFechamentoPalletsHelper.prototype = {
    obterOpcoes: function (opcoes) {
        var opcoesEnum = [
            { text: "Aberto", value: this.Aberto },
            { text: "Finalizado", value: this.Finalizado },
            { text: "Todas", value: "" }
        ];

        var _opcoesReturn = [];
        opcoes.map(function (addOpt) {
            opcoesEnum.map(function (opt) {
                if (opt.value == addOpt)
                    _opcoesReturn.push(opt);
            });
        });
        return _opcoesReturn;
    }
}

var EnumSituacaoFechamentoPallets = Object.freeze(new EnumSituacaoFechamentoPalletsHelper());