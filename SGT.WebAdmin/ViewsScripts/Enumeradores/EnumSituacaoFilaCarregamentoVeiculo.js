var EnumSituacaoFilaCarregamentoVeiculoHelper = function () {
    this.Todas = "";
    this.AguardandoConfirmacao = 1;
    this.AguardandoConjuntos = 2;
    this.CargaCancelada = 3;
    this.Disponivel = 4;
    this.EmTransicao = 5;
    this.EmViagem = 6;
    this.Finalizada = 7;
    this.Removida = 8;
    this.EmRemocao = 9;
    this.AguardandoAceiteCarga = 10;
    this.AceiteCargaRecusado = 11;
    this.EmChecklist = 12;
    this.AguardandoChegadaVeiculo = 13;
    this.AguardandoCarga = 14;
    this.ReboqueAtrelado = 15;
    this.AguardandoAceitePreCarga = 16;
};

EnumSituacaoFilaCarregamentoVeiculoHelper.prototype = {
    obterSituacoesNaFila: function () {
        return [
            this.AceiteCargaRecusado,
            this.AguardandoAceiteCarga,
            this.AguardandoAceitePreCarga,
            this.AguardandoCarga,
            this.AguardandoChegadaVeiculo,
            this.AguardandoConfirmacao,
            this.AguardandoConjuntos,
            this.CargaCancelada,
            this.Disponivel,
            this.EmChecklist,
            this.EmRemocao
        ];
    }
}

var EnumSituacaoFilaCarregamentoVeiculo = Object.freeze(new EnumSituacaoFilaCarregamentoVeiculoHelper());