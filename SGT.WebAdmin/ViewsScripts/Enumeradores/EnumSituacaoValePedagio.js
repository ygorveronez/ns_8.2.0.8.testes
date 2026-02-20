var EnumSituacaoValePedagioHelper = function () {
    this.Todas = null;
    this.Pendete = 0;
    this.Comprada = 1;
    this.Confirmada = 2;
    this.RotaGerada = 3;
    this.RotaSemCusto = 4;
    this.AguardandoCadastroRota = 5;
    this.Recusada = 7;
    this.Cancelada = 8;
    this.Encerrada = 9;
    this.EmCancelamento = 10;
};

EnumSituacaoValePedagioHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Pendente", value: this.Pendete },
            { text: "Comprada", value: this.Comprada },
            { text: "Confirmada", value: this.Confirmada },
            { text: "Rota gerada", value: this.RotaGerada },
            { text: "Rota sem custo / valor", value: this.RotaSemCusto },
            { text: "Aguardando cadastro rota", value: this.AguardandoCadastroRota },
            { text: "Recusada", value: this.Recusada },
            { text: "Cancelada", value: this.Cancelada },
            { text: "Encerrada", value: this.Encerrada },
            { text: "Em Cancelamento", value: this.EmCancelamento }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoValePedagio = Object.freeze(new EnumSituacaoValePedagioHelper());