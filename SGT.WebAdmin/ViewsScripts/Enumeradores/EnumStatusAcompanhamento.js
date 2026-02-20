var EnumStatusAcompanhamentoHelper = function () {
    this.SemMonitoramento = 0;
    this.ComMonitoramentoSemPosicao = 1;
    this.ComMonitoramentoComUmaPosicao = 2;
    this.ComPosicaoRecebidaNoTempoConfigurado = 3;
    this.SemPosicaoRecebidaNoTempoConfigurado = 4;
};

EnumStatusAcompanhamentoHelper.prototype = {
    obterDescricao: function (valor) {
        switch (valor) {
            case this.SemMonitoramento: return "Carga sem monitoramento";
            case this.ComMonitoramentoSemPosicao: return "Carga com monitoramento mas sem posição";
            case this.ComMonitoramentoComUmaPosicao: return "Carga com monitoramento e com pelo menos uma posição";
            case this.ComPosicaoRecebidaNoTempoConfigurado: return "Carga com monitoramento e com pelo menos uma posição e com posição recebida no tempo configurado";
            case this.SemPosicaoRecebidaNoTempoConfigurado: return "Carga com monitoramento e com pelo menos uma posição e sem posição recebida no tempo configurado";
            default: return "";
        }
    }
}

var EnumStatusAcompanhamento = Object.freeze(new EnumStatusAcompanhamentoHelper());