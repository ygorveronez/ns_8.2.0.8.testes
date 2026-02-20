var EnumTipoCarregamentoNavioHelper = function () {
    this.Todos = "";
    this.NaoInformado = 0;
    this.Comum = 1;
    this.Reefer = 2;

};


EnumTipoCarregamentoNavioHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Não Informado", value: this.NaoInformado },
            { text: "Comum", value: this.Comum },
            { text: "Reefer", value: this.Reefer }
        ];
    },
 

};


var EnumTipoCarregamentoNavio = Object.freeze(new EnumTipoCarregamentoNavioHelper());