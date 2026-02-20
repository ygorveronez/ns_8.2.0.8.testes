var EnumVerificarStatusViagemHelper = function () {
    this.NaoVerificar = 1;
    this.EstarComStatusViagem = 2;
    this.NaoEstarComStatusViagem = 3;
    this.HaverPeloMenosUmStatusViagem = 4;
    this.HaverTodosStatusViagem = 5;
}

EnumVerificarStatusViagemHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Não verificar", value: this.NaoVerificar },
            { text: "Estar com o status de viagem", value: this.EstarComStatusViagem },
            { text: "Não estar com o status de viagem", value: this.NaoEstarComStatusViagem },
            { text: "Haver pelo menos um dos status de viagem", value: this.HaverPeloMenosUmStatusViagem },
            { text: "Haver todos os status de viagem", value: this.HaverTodosStatusViagem }
        ];
    }
}

var EnumVerificarStatusViagem = Object.freeze(new EnumVerificarStatusViagemHelper());