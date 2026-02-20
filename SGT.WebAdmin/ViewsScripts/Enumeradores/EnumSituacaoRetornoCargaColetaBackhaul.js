var EnumSituacaoRetornoCargaColetaBackhaulHelper = function () {
    this.Todas = "";
    this.AguardandoGerarCarga = 1;
    this.GerandoCarga = 2;
    this.CargaGerada = 3;
    this.RetornoCancelado = 4;
};

EnumSituacaoRetornoCargaColetaBackhaulHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando Gerar a Carga", value: this.AguardandoGerarCarga },
            { text: "Carga Gerada", value: this.CargaGerada },
            { text: "Gerando Carga", value: this.GerandoCarga },
            { text: "Retorno Cancelado", value: this.RetornoCancelado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoRetornoCargaColetaBackhaul = Object.freeze(new EnumSituacaoRetornoCargaColetaBackhaulHelper());
