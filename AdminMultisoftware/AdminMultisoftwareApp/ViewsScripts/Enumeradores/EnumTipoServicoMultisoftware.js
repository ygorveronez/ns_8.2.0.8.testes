var EnumTipoServicoMultisoftwareHelper = function () {
    this.Todos = "";
    this.MultiEmbarcador = 1;
    this.MultiTMS = 2;
    this.MultiCTe = 3;
    this.CallCenter = 4;
    this.Terceiros = 5;
    this.MultiNFe = 6;
    this.MultiNFeAdmin = 7;
    this.Fornecedor = 8;
    this.MultiMobile = 9;
    this.MultiBus = 10;
    this.MultiBusTransportador = 11;
    this.TransportadorTerceiro = 12;
};

EnumTipoServicoMultisoftwareHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "MultiEmbarcador", value: this.MultiEmbarcador },
            { text: "MultiTMS", value: this.MultiTMS },
            { text: "MultiCTe", value: this.MultiCTe },
            { text: "Call Center", value: this.CallCenter },
            { text: "Terceiros", value: this.Terceiros },
            { text: "Multi NF-e", value: this.MultiNFe },
            { text: "Multi NF-e Admin", value: this.MultiNFeAdmin },
            { text: "Fornecedor", value: this.Fornecedor },
            { text: "Multi Mobile", value: this.MultiMobile },
            { text: "MultiBus", value: this.MultiBus },
            { text: "MultiBus Transportador", value: this.MultiBusTransportador },
            { text: "Transportador Terceiro", value: this.TransportadorTerceiro }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoServicoMultisoftware = Object.freeze(new EnumTipoServicoMultisoftwareHelper());