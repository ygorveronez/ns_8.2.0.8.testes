var EnumTipoPrazoPagamentoHelper = function () {
    this.Todos = "";
    this.DataPagamento = 1;
    this.DataDocumento = 2;
    this.DataPrevisaoEncerramento = 3;
    this.DataLiberacaoDocumento = 4;
    this.ApartirAprovacaoFatura = 5;
    this.ApartirDataGeracaoFatura = 6;
};

EnumTipoPrazoPagamentoHelper.prototype = {
    obterDescricao: function (tipoPrazoPagamento) {
        switch (tipoPrazoPagamento) {
            case this.DataPagamento: return Localization.Resources.Enumeradores.TipoPrazoPagamento.PartirDaDataDoPagamento;
            case this.DataDocumento: return Localization.Resources.Enumeradores.TipoPrazoPagamento.PartirDaEmissaoDoPrimeiroDocumento;
            case this.DataPrevisaoEncerramento: return Localization.Resources.Enumeradores.TipoPrazoPagamento.PartirDaDataDePrevisaoDeEncerramento;
            case this.DataLiberacaoDocumento: return Localization.Resources.Enumeradores.TipoPrazoPagamento.PartirDataLiberacaoDocumento;
            case this.ApartirAprovacaoFatura: return Localization.Resources.Enumeradores.TipoPrazoFaturamento.ApartirAprovacaoFatura;
            case this.ApartirDataGeracaoFatura: return Localization.Resources.Enumeradores.TipoPrazoFaturamento.ApartirDataGeracaoFatura;
            default: return "";
        }
    },
    obterOpcoes: function () {
        return [
            {text: Localization.Resources.Enumeradores.TipoPrazoPagamento.PartirDaDataDoPagamento, value: this.DataPagamento },
            { text: Localization.Resources.Enumeradores.TipoPrazoPagamento.PartirDaEmissaoDoPrimeiroDocumento, value: this.DataDocumento },
            { text: Localization.Resources.Enumeradores.TipoPrazoPagamento.PartirDaDataDePrevisaoDeEncerramento, value: this.DataPrevisaoEncerramento },
            { text: Localization.Resources.Enumeradores.TipoPrazoPagamento.PartirDataLiberacaoDocumento, value: this.DataLiberacaoDocumento },
            { text: Localization.Resources.Enumeradores.TipoPrazoFaturamento.ApartirAprovacaoFatura, value: this.ApartirAprovacaoFatura },
            { text: Localization.Resources.Enumeradores.TipoPrazoFaturamento.ApartirDataGeracaoFatura, value: this.ApartirDataGeracaoFatura }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoPrazoPagamento.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoPrazoPagamento = Object.freeze(new EnumTipoPrazoPagamentoHelper());