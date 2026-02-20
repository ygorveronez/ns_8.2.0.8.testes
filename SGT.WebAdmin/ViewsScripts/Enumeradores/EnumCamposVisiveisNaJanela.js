var EnumCamposVisiveisNaJanelaHelper = function () {
    this.Carga = 1;
    this.PrevisaoEntrega = 2;
    this.TipoCarga = 3;
    this.Origem = 4;
    this.Operacao = 5;
    this.Transportador = 6;
    this.Disponibilizada = 7;
    this.Veiculo = 8;
    this.Destino = 9;
    this.ObservacaoTransportador = 10;
    this.EnderecoCliente = 11;
    this.ValorFrete = 12;
    this.QuantidadeEntregas = 13;
    this.ValorTarget = 14;
    this.PossuiJanelaDestino = 15;
};

EnumCamposVisiveisNaJanelaHelper.prototype = {
    obterDescricoes: function () {
        var descricoes = [];
        descricoes.push({ text: Localization.Resources.Enumeradores.CampoVisiveisNaJanela.Carga, value: this.Carga });
        descricoes.push({ text: Localization.Resources.Enumeradores.CampoVisiveisNaJanela.PrevisaoEntrega, value: this.PrevisaoEntrega });
        descricoes.push({ text: Localization.Resources.Enumeradores.CampoVisiveisNaJanela.TipoCarga, value: this.TipoCarga });
        descricoes.push({ text: Localization.Resources.Enumeradores.CampoVisiveisNaJanela.Origem, value: this.Origem });
        descricoes.push({ text: Localization.Resources.Enumeradores.CampoVisiveisNaJanela.Operacao, value: this.Operacao });
        descricoes.push({ text: Localization.Resources.Enumeradores.CampoVisiveisNaJanela.Transportador, value: this.Transportador });
        descricoes.push({ text: Localization.Resources.Enumeradores.CampoVisiveisNaJanela.Disponibilizada, value: this.Disponibilizada });
        descricoes.push({ text: Localization.Resources.Enumeradores.CampoVisiveisNaJanela.Veiculo, value: this.Veiculo });
        descricoes.push({ text: Localization.Resources.Enumeradores.CampoVisiveisNaJanela.Destino, value: this.Destino });
        descricoes.push({ text: Localization.Resources.Enumeradores.CampoVisiveisNaJanela.ObservacaoTransportador, value: this.ObservacaoTransportador });
        descricoes.push({ text: Localization.Resources.Enumeradores.CampoVisiveisNaJanela.EnderecoCliente, value: this.EnderecoCliente });
        descricoes.push({ text: Localization.Resources.Enumeradores.CampoVisiveisNaJanela.ValorFrete, value: this.ValorFrete });
        descricoes.push({ text: Localization.Resources.Enumeradores.CampoVisiveisNaJanela.QuantidadeEntregas, value: this.QuantidadeEntregas });
        descricoes.push({ text: Localization.Resources.Enumeradores.CampoVisiveisNaJanela.ValorTarget, value: this.ValorTarget });
        descricoes.push({ text: Localization.Resources.Enumeradores.CampoVisiveisNaJanela.PossuiJanelaDestino, value: this.PossuiJanelaDestino });

        return descricoes;
    }
}

var EnumCamposVisiveisNaJanela = Object.freeze(new EnumCamposVisiveisNaJanelaHelper());
