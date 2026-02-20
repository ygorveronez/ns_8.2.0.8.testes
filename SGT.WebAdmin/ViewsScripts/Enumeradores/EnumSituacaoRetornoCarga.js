var EnumSituacaoRetornoCargaHelper = function (){
    this.Todas = 1;
    this.AgInformarRetorno = 2;
    this.GerandoCargaRetorno = 3;
    //this.FalhaGerarRetorno = 4;
    this.RetornoGerado = 5;
    this.CanceladoRetorno = 6;
    this.GerandoCargaRetornoColetaBackhaul = 7;
};

EnumSituacaoRetornoCargaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.RetornoCarga.AgInformarRetorno, value: this.AgInformarRetorno },
            //{ text: "Falha ao gerar a carga de Retorno", value: this.FalhaGerarRetorno },
            { text: Localization.Resources.Enumeradores.RetornoCarga.GerandoCargaRetorno, value: this.GerandoCargaRetorno },
            { text: Localization.Resources.Enumeradores.RetornoCarga.GerandoCargaRetornoColeta, value: this.GerandoCargaRetornoColetaBackhaul },
            { text: Localization.Resources.Enumeradores.RetornoCarga.RetornoCancelado, value: this.CanceladoRetorno },
            { text: Localization.Resources.Enumeradores.RetornoCarga.RetornoGerado, value: this.RetornoGerado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.RetornoCarga.Todas, value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoRetornoCarga = Object.freeze(new EnumSituacaoRetornoCargaHelper());