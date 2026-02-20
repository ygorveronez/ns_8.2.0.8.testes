var EnumTipoQuantidadeCargaHelper = function () {
    this.AgConfirmacaoTransportador = 1;
    this.AguardandoCarregamento = 2;
    this.AgLiberacaoParaTransportadores = 3;
    this.JaCarregada = 4;
    this.EmAtraso = 5;
    this.SemTransportador = 7;
    this.SemValorFrete = 8;
    this.TotalCargaJanelaCarregamento = 9;
    this.ProntaParaCarregar = 10;
    this.Faturada = 11;
};

EnumTipoQuantidadeCargaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Ag. Carregamento", value: this.AguardandoCarregamento },
            { text: "Ag. Confirmação do Transportador", value: this.AgConfirmacaoTransportador },
            { text: "Ag. Liberação para Transportadores", value: this.AgLiberacaoParaTransportadores },
            { text: "Em Atraso", value: this.EmAtraso },
            { text: "Já Carregada", value: this.JaCarregada },
            { text: "Pronta para Carregar", value: this.ProntaParaCarregar },
            { text: "Sem Transportador", value: this.SemTransportador },
            { text: "Sem Valor de Frete", value: this.SemValorFrete },
            { text: "Total de Cargas", value: this.TotalCargaJanelaCarregamento },
            { text: "Faturada", value: this.Faturada }
        ];
    }
};

var EnumTipoQuantidadeCarga = Object.freeze(new EnumTipoQuantidadeCargaHelper());