var EnumIndicadorPagadorHelper = function () {
    this.Todos = 0;
    this.SeguroProprio = 1;
    this.Empresa = 2;
    this.SeguroTerceiro = 3;
    this.Terceiro = 4;
    this.EmpresaMotorista = 5;
    this.EmpresaNota = 6;
    this.MotoristaFolha = 7;
    this.SeguroTerceiroReembolso = 8;
};

EnumIndicadorPagadorHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Seguro Próprio", value: this.SeguroProprio },
            { text: "Empresa", value: this.Empresa },
            { text: "Seguro Terceiro", value: this.SeguroTerceiro },
            { text: "Terceiro", value: this.Terceiro },
            { text: "Empresa/Motorista", value: this.EmpresaMotorista },
            { text: "Empresa/Nota", value: this.EmpresaNota },
            { text: "Motorista/Folha", value: this.MotoristaFolha },
            { text: "Seguro Terceiro/Reembolso", value: this.SeguroTerceiroReembolso }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumIndicadorPagador = Object.freeze(new EnumIndicadorPagadorHelper());