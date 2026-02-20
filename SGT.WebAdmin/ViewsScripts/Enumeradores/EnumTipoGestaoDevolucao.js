var EnumTipoGestaoDevolucaoHelper = function () {
    this.NaoDefinido = 0;
    this.Permuta = 1;
    this.Coleta = 2;
    this.Descarte = 3;
    this.Agendamento = 4;
    this.PosEntrega = 5;
    this.PermutaPallet = 6;
    this.Simplificado = 7;
    this.Selecione = 99;
};

EnumTipoGestaoDevolucaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Permuta", value: this.Permuta },
            { text: "Coleta", value: this.Coleta },
            { text: "Agendamento", value: this.Agendamento },
        ];
    },

    obterOpcaoDescarte: function () {
        return [
            { text: "Selecione", value: this.Selecione },
            { text: "Descarte", value: this.Descarte }
        ];
    },

    obterOpcaoDescarteEPermuta: function () {
        return [
            { text: "Selecione", value: this.Selecione },
            { text: "Descarte", value: this.Descarte },
            { text: "Permuta", value: this.Permuta }
        ];
    },

    obterOpcoesPallet: function () {
        return [
            { text: "Permuta", value: this.PermutaPallet },
            { text: "Agendamento", value: this.Agendamento }
        ];
    },
    obterOpcoesCIF: function () {
        return [
            { text: "Coleta", value: this.Coleta }
        ];
    },

    obterOpcoesFOB: function () {
        return [
            { text: "Agendamento", value: this.Agendamento }
        ];
    }
}

var EnumTipoGestaoDevolucao = Object.freeze(new EnumTipoGestaoDevolucaoHelper());