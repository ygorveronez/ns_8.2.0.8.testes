var EnumLimiteCarregamentosCentroCarregamentoHelper = function () {
    this.Todos = "";
    this.TempoCarregamento = 1;
    this.QuantidadeDocas = 2;
};

EnumLimiteCarregamentosCentroCarregamentoHelper.prototype = {
    obterDescricao: function (tipo) {
        switch (tipo) {
            case this.TempoCarregamento: return Localization.Resources.Enumeradores.LimiteCarregamentosCentroCarregamento.TempoDeCarregamento;
            case this.QuantidadeDocas: return Localization.Resources.Enumeradores.LimiteCarregamentosCentroCarregamento.QuantidadeDeDocas;
            default: return "";
        }
    },
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.LimiteCarregamentosCentroCarregamento.TempoDeCarregamento, value: this.TempoCarregamento },
            { text: Localization.Resources.Enumeradores.LimiteCarregamentosCentroCarregamento.QuantidadeDeDocas, value: this.QuantidadeDocas },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos }].concat(this.obterOpcoes());
    },
};

var EnumLimiteCarregamentosCentroCarregamento = Object.freeze(new EnumLimiteCarregamentosCentroCarregamentoHelper());
