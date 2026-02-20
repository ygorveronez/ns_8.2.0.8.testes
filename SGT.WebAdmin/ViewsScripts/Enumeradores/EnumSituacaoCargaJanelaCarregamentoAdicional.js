var EnumSituacaoCargaJanelaCarregamentoAdicionalHelper = function () {
    this.Todas = "";
    this.DocumentosEmitidos = 1;
    this.DadosTransporteInformados = 2;
    this.SemDadosTransporte = 3;
    this.AtrasoChegadaVeiculo = 4;
    this.ForaPeriodoCarregamento = 5;
    this.NotaFiscalEmitida = 6;
};

EnumSituacaoCargaJanelaCarregamentoAdicionalHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Atraso na Chegada do Veículo", value: this.AtrasoChegadaVeiculo },
            { text: "Dados de Transporte Informados", value: this.DadosTransporteInformados },
            { text: "Documentos Emitidos", value: this.DocumentosEmitidos },
            { text: "Fora do Período de Carregamento", value: this.ForaPeriodoCarregamento },
            { text: "Sem Dados de Transporte", value: this.SemDadosTransporte },
            { text: "Nota Fiscal Emitida", value: this.NotaFiscalEmitida }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumSituacaoCargaJanelaCarregamentoAdicional = Object.freeze(new EnumSituacaoCargaJanelaCarregamentoAdicionalHelper());
