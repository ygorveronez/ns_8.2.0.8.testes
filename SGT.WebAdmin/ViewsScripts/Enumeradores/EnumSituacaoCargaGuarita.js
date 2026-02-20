var EnumSituacaoCargaGuaritaHelper = function () {
    this.Todas = "";
    this.AguardandoLiberacao = 0;
    this.Liberada = 1;
    this.AgChegadaVeiculo = 2;
    this.SaidaLiberada = 3;
};

EnumSituacaoCargaGuaritaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando Chegada", value: this.AgChegadaVeiculo },
            { text: "Aguardando Entrada", value: this.AguardandoLiberacao },
            { text: "Aguardando Saída", value: this.Liberada },
            { text: "Veículo Saiu", value: this.SaidaLiberada }
        ];
    },
    obterOpcoesExpedicao: function () {
        return [
            { text: "Veículo não chegou", value: this.AguardandoLiberacao },
            { text: "Veículo liberado", value: this.Liberada }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    },
    obterOpcoesPesquisaExpedicao: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoesExpedicao());
    }
};

var EnumSituacaoCargaGuarita = Object.freeze(new EnumSituacaoCargaGuaritaHelper());
