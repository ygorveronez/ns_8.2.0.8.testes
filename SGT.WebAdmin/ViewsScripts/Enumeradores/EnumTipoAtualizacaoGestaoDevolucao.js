var EnumTipoAtualizacaoGestaoDevolucaoHelper = function () {
    this.Nenhum = 0;
    this.AvancarEtapa = 1;
    this.VoltarEtapa = 2;
    this.AtualizarGrid = 3;
    this.AtualizarMesmaEtapa = 4;
};

EnumTipoAtualizacaoGestaoDevolucaoHelper.prototype = {
    obterOpcoesMovimentacao: function () {
        return [
            this.AvancarEtapa,
            this.VoltarEtapa,
        ];
    }
};

var EnumTipoAtualizacaoGestaoDevolucao = Object.freeze(new EnumTipoAtualizacaoGestaoDevolucaoHelper());