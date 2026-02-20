var EnumSituacaoVeiculoHelper = function () {
    this.Todos = 0;
    this.Vazio = 1;
    this.AvisoCarregamento = 2;
    this.EmViagem = 3;
    this.EmManutencao = 4;
    this.Disponivel = 5;
    this.EmFila = 6;
    this.Indisponivel = 7;
}

EnumSituacaoVeiculoHelper.prototype = {
    obterTodos: function () {
        return [
            this.Todos,
            this.EmViagem,
            this.EmManutencao,
            this.Disponivel,
            this.EmFila,
            this.Vazio,
            this.AvisoCarregamento,
            this.Indisponivel,
        ];
    },
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacaoVeiculo.Todos, value: this.Todos },
            { text: Localization.Resources.Enumeradores.SituacaoVeiculo.EmViagem, value: this.EmViagem },
            { text: Localization.Resources.Enumeradores.SituacaoVeiculo.EmManutencao, value: this.EmManutencao },
            { text: Localization.Resources.Enumeradores.SituacaoVeiculo.Disponivel, value: this.Disponivel },
            { text: Localization.Resources.Enumeradores.SituacaoVeiculo.Vazio, value: this.Vazio },
            { text: Localization.Resources.Enumeradores.SituacaoVeiculo.AvisoCarregamento, value: this.AvisoCarregamento },
            { text: Localization.Resources.Enumeradores.SituacaoVeiculo.Indisponivel, value: this.Indisponivel },
        ];
    }
}

var EnumSituacaoVeiculo = Object.freeze(new EnumSituacaoVeiculoHelper());