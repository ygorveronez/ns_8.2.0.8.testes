var EnumSituacaoManobraTracaoHelper = function () {
    this.Todas = "";
    this.EmIntervalo = 1;
    this.EmManobra = 2;
    this.Ocioso = 3;
    this.SemMotorista = 4;
}

EnumSituacaoManobraTracaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Em Intervalo", value: this.EmIntervalo },
            { text: "Em Manobra", value: this.EmManobra },
            { text: "Ocioso", value: this.Ocioso },
            { text: "Sem Motorista", value: this.SemMotorista }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumSituacaoManobraTracao = Object.freeze(new EnumSituacaoManobraTracaoHelper());