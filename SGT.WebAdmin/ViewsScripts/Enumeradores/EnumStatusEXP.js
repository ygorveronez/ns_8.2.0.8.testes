var EnumStatusEXPHelper = function () {
    this.Todos = -1;
    this.NaoDefinido = 0;
    this.AguardandoDistrib = 1;
    this.PEGerado = 2;
    this.EXPCancelada = 3;
    this.PEParcialGerado = 4;
    this.EXPEXPParcialDistrib = 5;
    this.Distrib = 6;
    this.PVGerado = 7;
};

EnumStatusEXPHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Não Definido", value: this.NaoDefinido },
            { text: "Aguardando Distribuição", value: this.AguardandoDistrib },
            { text: "PE Gerado", value: this.PEGerado },
            { text: "EXP Cancelada", value: this.EXPCancelada },
            { text: "PE Parcial Gerado", value: this.PEParcialGerado },
            { text: "EXP Parcial Distribuida", value: this.EXPEXPParcialDistrib },
            { text: "Distribuida", value: this.Distrib },
            { text: "PV Gerado", value: this.PVGerado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumStatusEXP = Object.freeze(new EnumStatusEXPHelper());