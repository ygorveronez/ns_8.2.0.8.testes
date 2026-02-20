var EnumSituacaoCargaJanelaDescarregamentoCadastradaHelper = function () {
    this.Todas = "";
    this.PendenteColeta = 1;
    this.Programado = 2;
    this.AguardandoCarregamento = 3;
    this.EmTransito = 4;
    this.AguardandoDescarga = 5;
    this.AguardandoVeiculoEncostar = 6;
    this.EmDescarga = 7;
    this.DescarregamentoFinalizado = 8;
    this.BloqueadaParaDescarga = 9;
};

EnumSituacaoCargaJanelaDescarregamentoCadastradaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Pendente de Coleta", value: this.PendenteColeta },
            { text: "Programado", value: this.Programado },
            { text: "Aguardando Carregamento", value: this.AguardandoCarregamento },
            { text: "Em Trânsito", value: this.EmTransito },
            { text: "Aguardando Descarga", value: this.AguardandoDescarga },
            { text: "Aguardando Veíulo Encostar", value: this.AguardandoVeiculoEncostar },
            { text: "Em Descarga", value: this.EmDescarga },
            { text: "Descarregamento Finalizado", value: this.DescarregamentoFinalizado },
            { text: "Bloqueada para Descarga", value: this.BloqueadaParaDescarga }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumSituacaoCargaJanelaDescarregamentoCadastrada = Object.freeze(new EnumSituacaoCargaJanelaDescarregamentoCadastradaHelper());
