var EnumVeiculoMonitoramentoHelper = function () {
    this.Todos = -1;
    this.Todas = 0;
    this.SemViagem = 0;
    this.EmLoja = 1;
    this.EmViagem = 3;
    this.NaCD = 3;
    this.Garagem = 4;
    this.SemPosicao = 5;
    this.Tracao = 6;
    this.Reboque = 7;
    
};

EnumVeiculoMonitoramentoHelper.prototype = {
    obterOpcoesStatusPosicao: function () {
        return [
            { text: Localization.Resources.Enumeradores.VeiculoMonitoramento.SemViagem, value: this.SemViagem },
            { text: Localization.Resources.Enumeradores.VeiculoMonitoramento.EmLoja, value: this.EmLoja },
            { text: Localization.Resources.Enumeradores.VeiculoMonitoramento.EmViagem, value: this.EmViagem },
            { text: Localization.Resources.Enumeradores.VeiculoMonitoramento.NaCD, value: this.NaCD },
            { text: Localization.Resources.Enumeradores.VeiculoMonitoramento.Garagem, value: this.Garagem },
            { text: Localization.Resources.Enumeradores.VeiculoMonitoramento.SemPosicao, value: this.SemPosicao }
        ];
    },
    obterOpcoesPesquisaStatusPosicao: function () {
        return [{ text: Localization.Resources.Enumeradores.VeiculoMonitoramento.Todos, value: this.Todos }].concat(this.obterOpcoesStatusPosicao());
    },


    obterOpcoesStatusViagem: function () {
        return [
            { text: Localization.Resources.Enumeradores.VeiculoMonitoramento.SemViagem , value: this.SemViagem },
            { text: Localization.Resources.Enumeradores.VeiculoMonitoramento.EmViagem, value: this.EmViagem }
        ];
    },
    obterOpcoesPesquisaStatusViagem: function () {
        return [{ text: Localization.Resources.Enumeradores.VeiculoMonitoramento.Todas, value: this.Todas }].concat(this.obterOpcoesStatusViagem());
    },


    obterOpcoesTipoVeiculoPesquisa: function () {
        return [
            { text: Localization.Resources.Enumeradores.VeiculoMonitoramento.Tracao, value: this.Tracao },
            { text: Localization.Resources.Enumeradores.VeiculoMonitoramento.Reboque, value: this.Reboque }
        ];
    },
    obterOpcoesPesquisaTipoVeiculoPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.VeiculoMonitoramento.Todos, value: this.Todos }].concat(this.obterOpcoesTipoVeiculoPesquisa());
    }
}

var EnumVeiculoMonitoramento = Object.freeze(new EnumVeiculoMonitoramentoHelper());

