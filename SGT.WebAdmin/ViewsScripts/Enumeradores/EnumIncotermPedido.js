var EnumIncotermPedidoHelper = function () {
    this.Nenhum = 0;
    this.EXW = 1;
    this.FCA = 2;
    this.FAS = 3;
    this.FOB = 4;
    this.CFR = 5;
    this.CIF = 6;
    this.CPT = 7;
    this.CIP = 8;
    this.DAP = 9;
    this.DPU = 10;
    this.DDP = 11;
    this.OCV = 12; 
    this.C_F = 13; 
    this.C_I = 14; 

};

EnumIncotermPedidoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Nenhum", value: this.Nenhum },
            { text: "EXW", value: this.EXW },
            { text: "FCA", value: this.FCA },
            { text: "FAS", value: this.FAS },
            { text: "FOB", value: this.FOB },
            { text: "CFR", value: this.CFR },
            { text: "CIF", value: this.CIF },
            { text: "CPT", value: this.CPT },
            { text: "CIP", value: this.CIP },
            { text: "DAP", value: this.DAP },
            { text: "DPU", value: this.DPU },
            { text: "DDP", value: this.DDP },
            { text: "OCV", value: this.OCV },   
            { text: "C_F", value: this.C_F },   
            { text: "C_I", value: this.C_I }    
        ];
    },
}

var EnumIncotermPedido = Object.freeze(new EnumIncotermPedidoHelper());