
var EnumMotivoChamadoTipoGatilhoNaCargaHelper = function () {
    this.Nenhum = "";
    this.AoSalvarPlacaComModeloVeicularDiferenteDoPrevisto = 1;
};

EnumMotivoChamadoTipoGatilhoNaCargaHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "", value: this.Nenhum },
            { text: Localization.Resources.Chamado.MotivoChamado.AoSalvarPlacaComModeloVeicularDiferenteDoPrevisto, value: this.AoSalvarPlacaComModeloVeicularDiferenteDoPrevisto }
        ];
    },
    ObterDescricao: function (valor) {
        switch (valor) {
            case this.AoSalvarPlacaComModeloVeicularDiferenteDoPrevisto: return Localization.Resources.Chamado.MotivoChamado.AoSalvarPlacaComModeloVeicularDiferenteDoPrevisto;
            default: return "";
        }
    }
};

var EnumMotivoChamadoTipoGatilhoNaCarga = Object.freeze(new EnumMotivoChamadoTipoGatilhoNaCargaHelper());