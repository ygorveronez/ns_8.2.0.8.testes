var EnumTipoPagamentoCIOTHelper = function () {
    this.SemPgto = 0;
    this.Cartao = 1;
    this.Deposito = 2;
    this.Transferencia = 3;
    this.PIX = 4;
    this.BBC = 5;
};

EnumTipoPagamentoCIOTHelper.prototype.ObterOpcoes = function () {
    return [
        { value: "", text: Localization.Resources.Enumeradores.TipoPagamentoCIOT.NaoSelecionado },
        { value: this.SemPgto, text: Localization.Resources.Enumeradores.TipoPagamentoCIOT.SemPagamento },
        { value: this.Cartao, text: Localization.Resources.Enumeradores.TipoPagamentoCIOT.Cartao },
        { value: this.Deposito, text: Localization.Resources.Enumeradores.TipoPagamentoCIOT.Deposito },
        { value: this.Transferencia, text: Localization.Resources.Enumeradores.TipoPagamentoCIOT.Transferencia },
        { value: this.PIX, text: Localization.Resources.Enumeradores.TipoPagamentoCIOT.PIX }
    ];
};

EnumTipoPagamentoCIOTHelper.prototype.ObterOpcoesComDadosBancarios = function () {
    return [
        { value: "", text: Localization.Resources.Enumeradores.TipoPagamentoCIOT.NaoSelecionado },
        { value: this.Transferencia, text:  "Dados Bancários" },
        { value: this.PIX, text: Localization.Resources.Enumeradores.TipoPagamentoCIOT.PIX }
    ];
};

EnumTipoPagamentoCIOTHelper.prototype.obterDescricao = function (valor) {
    switch (valor) {
        case "": returnLocalization.Resources.Enumeradores.TipoPagamentoCIOT.NaoSelecionado;
        case this.SemPgto: return Localization.Resources.Enumeradores.TipoPagamentoCIOT.SemPagamento;
        case this.Cartao: return Localization.Resources.Enumeradores.TipoPagamentoCIOT.Cartao;
        case this.Deposito: return Localization.Resources.Enumeradores.TipoPagamentoCIOT.Deposito;
        case this.Transferencia: return Localization.Resources.Enumeradores.TipoPagamentoCIOT.Transferencia;
        case this.PIX: return Localization.Resources.Enumeradores.TipoPagamentoCIOT.PIX;
        case this.BBC: return Localization.Resources.Enumeradores.TipoPagamentoCIOT.BBC;

        default: return "";
    }
};


EnumTipoPagamentoCIOTHelper.prototype.obterOpcoesPorOperadora = function (operadora) {
    switch (operadora) {
        case EnumOperadoraCIOT.TruckPad:
            return [
                { value: "", text: Localization.Resources.Enumeradores.TipoPagamentoCIOT.NaoSelecionado },
                { value: this.SemPgto, text: Localization.Resources.Enumeradores.TipoPagamentoCIOT.SemPagamento },
                { value: this.Deposito, text: Localization.Resources.Enumeradores.TipoPagamentoCIOT.Deposito },
                { value: this.Transferencia, text: Localization.Resources.Enumeradores.TipoPagamentoCIOT.Transferencia },
                { value: this.PIX, text: Localization.Resources.Enumeradores.TipoPagamentoCIOT.PIX },
                { value: this.BBC, text: Localization.Resources.Enumeradores.TipoPagamentoCIOT.BBC }
            ];

        case EnumOperadoraCIOT.Extratta:
            return [
                { value: "", text: Localization.Resources.Enumeradores.TipoPagamentoCIOT.NaoSelecionado },
                { value: this.SemPgto, text: Localization.Resources.Enumeradores.TipoPagamentoCIOT.SemPagamento },
                { value: this.Deposito, text: Localization.Resources.Enumeradores.TipoPagamentoCIOT.Deposito },
                { value: this.Transferencia, text: Localization.Resources.Enumeradores.TipoPagamentoCIOT.Transferencia },
                { value: this.PIX, text: Localization.Resources.Enumeradores.TipoPagamentoCIOT.PIX }
            ];

        default:
            return [
                { value: "", text: Localization.Resources.Enumeradores.TipoPagamentoCIOT.NaoSelecionado },
                { value: this.SemPgto, text: Localization.Resources.Enumeradores.TipoPagamentoCIOT.SemPagamento },
                { value: this.Cartao, text: Localization.Resources.Enumeradores.TipoPagamentoCIOT.Cartao },
                { value: this.Deposito, text: Localization.Resources.Enumeradores.TipoPagamentoCIOT.Deposito },
                { value: this.Transferencia, text: Localization.Resources.Enumeradores.TipoPagamentoCIOT.Transferencia }

            ];
    }
};
EnumTipoPagamentoCIOTHelper.prototype.ObterOpcoesPesquisa = function () {
    return [
        { value: "", text: Localization.Resources.Enumeradores.TipoPagamentoCIOT.Todos },
        { value: this.SemPgto, text: Localization.Resources.Enumeradores.TipoPagamentoCIOT.SemPagamento },
        { value: this.Cartao, text: Localization.Resources.Enumeradores.TipoPagamentoCIOT.Cartao },
        { value: this.Deposito, text: Localization.Resources.Enumeradores.TipoPagamentoCIOT.Deposito },
        { value: this.Transferencia, text: Localization.Resources.Enumeradores.TipoPagamentoCIOT.Transferencia },
        { value: this.PIX, text: Localization.Resources.Enumeradores.TipoPagamentoCIOT.PIX },
        { value: this.BBC, text: Localization.Resources.Enumeradores.TipoPagamentoCIOT.BBC }
    ];
};

var EnumTipoPagamentoCIOT = Object.freeze(new EnumTipoPagamentoCIOTHelper());