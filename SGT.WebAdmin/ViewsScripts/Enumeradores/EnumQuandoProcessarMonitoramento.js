var EnumQuandoProcessarMonitoramentoHelper = function () {
    this.AoCriarMonitoramento = 1;
    this.AoIniciarMonitoramento = 2;
    this.AoIniciarViagem = 3;
    this.AoReceberPosicao = 4;
}

EnumQuandoProcessarMonitoramentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.QuandoProcessarMonitoramento.AoReceberPosicao, value: this.AoReceberPosicao },
            { text: Localization.Resources.Enumeradores.QuandoProcessarMonitoramento.AoCriarMonitoramento, value: this.AoCriarMonitoramento },
            { text: Localization.Resources.Enumeradores.QuandoProcessarMonitoramento.AoIniciarMonitoramento, value: this.AoIniciarMonitoramento },
            { text: Localization.Resources.Enumeradores.QuandoProcessarMonitoramento.AoIniciarViagem, value: this.AoIniciarViagem }
        ];
    }
}

var EnumQuandoProcessarMonitoramento = Object.freeze(new EnumQuandoProcessarMonitoramentoHelper());