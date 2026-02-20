var EnumTipoServicoCargaHelper = function () {
    this.Normal = 0;
    this.SubContratada = 2;
    this.NormalESubContratada = 3;
    this.Redespacho = 4;
    this.RedespachoIntermediario = 5;
    this.SVMProprio = 6;
    this.SVMTerceiro = 7;
    this.Feeder = 8;
    this.NaoInformado = 99;
};

EnumTipoServicoCargaHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "Normal", value: this.Normal },
            { text: "SubContratada", value: this.SubContratada },
            { text: "Normal e Subcontratada", value: this.NormalESubContratada },
            { text: "Redespacho", value: this.Redespacho },
            { text: "Redespacho Intermediário", value: this.RedespachoIntermediario },
            { text: "SVM Próprio", value: this.SVMProprio },
            { text: "SVM Terceiro", value: this.SVMTerceiro },
            { text: "Feeder", value: this.Feeder },
            { text: "Não Definido", value: this.NaoInformado },
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: "" }].concat(this.ObterOpcoes());
    }
};

var EnumTipoServicoCarga = Object.freeze(new EnumTipoServicoCargaHelper());