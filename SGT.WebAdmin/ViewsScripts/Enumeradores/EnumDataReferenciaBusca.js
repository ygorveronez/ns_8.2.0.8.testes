var EnumDataReferenciaBuscaHelper = function () {
    this.DataCriacaoCarga = 1;
    this.DataCarregamentoCarga = 2;
};

EnumDataReferenciaBuscaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Data de Criação da Carga", value: this.DataCriacaoCarga },
            { text: "Data de Carregamento da Carga", value: this.DataCarregamentoCarga },
        ];
    }
};

var EnumDataReferenciaBusca = Object.freeze(new EnumDataReferenciaBuscaHelper());