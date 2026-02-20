var EnumRemarkSpedHelper = function () {
    this.OutrosServicos = 0;
    this.Cabotagem = 1;
    this.Container = 2;
    this.DnD = 3;
    this.FaturamentoMercadoInterno = 4;
    this.Feeder = 5;
    this.NoShow = 6;
    this.Rodo = 7;
    this.TrocaDeEspaco = 8;
}


EnumRemarkSpedHelper.prototype = {
    //obterDescricao: function (valor) {
    //    switch (valor) {
    //        case this.OutrosServicos: return Localization.Resources.Enumeradores.RemarkSped.OutrosServicos;
    //        case this.Cabotagem: return Localization.Resources.Enumeradores.RemarkSped.Cabotagem;
    //        case this.Container: return Localization.Resources.Enumeradores.RemarkSped.Container;
    //        case this.DnD: return Localization.Resources.Enumeradores.RemarkSped.DnD;
    //        case this.FaturamentoMercadoInterno: return Localization.Resources.Enumeradores.RemarkSped.FaturamentoMercadoInterno;
    //        case this.Feeder: return Localization.Resources.Enumeradores.RemarkSped.Feeder;
    //        case this.NoShow: return Localization.Resources.Enumeradores.RemarkSped.NoShow;
    //        case this.Rodo: return Localization.Resources.Enumeradores.RemarkSped.Rodo;
    //        case this.TrocaDeEspaco: return Localization.Resources.Enumeradores.RemarkSped.TrocaDeEspaco;
    //        default: return "";
    //    }
    //},
    obterOpcoes: function () {
        return [
            { text: "Outros Serviços", value: this.OutrosServicos },
            { text: "Cabotagem", value: this.Cabotagem },
            { text: "Container", value: this.Container },
            { text: "DnD", value: this.DnD },
            { text: "Faturamento Mercado Interno", value: this.FaturamentoMercadoInterno },
            { text: "Feeder", value: this.Feeder },
            { text: "No Show", value: this.NoShow },
            { text: "Rodo", value: this.Rodo },
            { text: "Troca de Espaço", value: this.TrocaDeEspaco }
        ];
    }
}

var EnumRemarkSped = Object.freeze(new EnumRemarkSpedHelper());