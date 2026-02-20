var EnumDataBaseCalculoPrevisaoControleEntregaHelper = function () {
    this.DataCriacaoCarga = 1;
    this.DataPrevisaoTerminoCarga = 2;
    this.DataInicioViagemPrevista = 3;
    this.DataCarregamentoCarga = 4;
    this.DataInicioCarregamentoJanela = 5;
}

EnumDataBaseCalculoPrevisaoControleEntregaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Data de criação da carga", value: this.DataCriacaoCarga },
            { text: "Data prevista para término da carga", value: this.DataPrevisaoTerminoCarga },
            { text: "Data de início de viagem prevista", value: this.DataInicioViagemPrevista },
            { text: "Data de carregamento da carga", value: this.DataCarregamentoCarga },
            { text: "Data inicial do carregamento na janela", value: this.DataInicioCarregamentoJanela }
        ];
    }
}

var EnumDataBaseCalculoPrevisaoControleEntrega = Object.freeze(new EnumDataBaseCalculoPrevisaoControleEntregaHelper());