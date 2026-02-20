var EnumSituacaoFilaCarregamentoMotoristaHelper = function () {
    this.Todas = "";
    this.CargaAceita = 1;
    this.CargaAlocada = 2;
    this.CargaCancelada = 3;
    this.CargaRecusada = 4;
    this.Disponivel = 5;
    this.SenhaPerdida = 6;
    this.Finalizada = 7;
    this.Removido = 8;
    this.PreCargaAlocada = 9;
    this.ReboqueAtrelado = 10;
};

var EnumSituacaoFilaCarregamentoMotorista = Object.freeze(new EnumSituacaoFilaCarregamentoMotoristaHelper());