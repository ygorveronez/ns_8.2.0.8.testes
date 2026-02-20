var EnumSituacaoSolicitacaoVeiculoHelper = function () {
    this.Todas = "";
    this.AguardandoSolicitacaoVeiculo = 1;
    this.VeiculoSolicitado = 2;
};

EnumSituacaoSolicitacaoVeiculoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando Solicitação do Veículo", value: this.AguardandoSolicitacaoVeiculo },
            { text: "Veículo Solicitado", value: this.VeiculoSolicitado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumSituacaoSolicitacaoVeiculo = Object.freeze(new EnumSituacaoSolicitacaoVeiculoHelper());