var EnumCamposOpcionaisJanelaCarregamentoTransportadorHelper = function () {
    this.DataCarregamento = 1;
    this.TipoCarga = 2;
    this.PrevisaoEntrega = 3;
    this.ModeloVeiculo = 4;
    this.Origem = 5;
    this.Destino = 6;
    this.NumeroEntregas = 7;
    this.Volumes = 8;
    this.Remetente = 9;
    this.Pedido = 10;
    this.Peso = 11;
    this.Placas = 12;
    this.Motorista = 13;
    this.Observacao = 14;
    this.ObservacaoCarregamento = 15;
    this.ObservacaoCliente = 16;
    this.Ordem = 17;
    this.Destinatario = 18;
    this.Rota = 19;
    this.Endereco = 20;
    this.CDDestino = 21;
    this.DivisoriaIntegracaoLeilao = 22;
    this.CargaPerigosaIntegracaoLeilao = 23;
    this.CanalEntrega = 24;
    this.CanalVenda = 25;
    this.NotasEnviadas = 26;
    this.Doca = 27;
    this.ValorOfertado = 28;
    this.ValidacaoConjunto = 29;
    this.FreteSimulado = 30;
    this.CentroDeCarregamento = 31;
    this.DataPrevisaoTerminoCarregamento = 32;
};

EnumCamposOpcionaisJanelaCarregamentoTransportadorHelper.prototype = {
    obterDescricoes: function () {

        const descricoes = [];

        descricoes.push({ text: Localization.Resources.Enumeradores.CamposOpcionaisJanelaCarregamentoTransportador.DataCarregamento, value: this.DataCarregamento });
        descricoes.push({ text: Localization.Resources.Enumeradores.CamposOpcionaisJanelaCarregamentoTransportador.TipoCarga, value: this.TipoCarga });
        descricoes.push({ text: Localization.Resources.Enumeradores.CamposOpcionaisJanelaCarregamentoTransportador.PrevisaoEntrega, value: this.PrevisaoEntrega });
        descricoes.push({ text: Localization.Resources.Enumeradores.CamposOpcionaisJanelaCarregamentoTransportador.ModeloVeiculo, value: this.ModeloVeiculo });
        descricoes.push({ text: Localization.Resources.Enumeradores.CamposOpcionaisJanelaCarregamentoTransportador.Origem, value: this.Origem });
        descricoes.push({ text: Localization.Resources.Enumeradores.CamposOpcionaisJanelaCarregamentoTransportador.Destino, value: this.Destino });
        descricoes.push({ text: Localization.Resources.Enumeradores.CamposOpcionaisJanelaCarregamentoTransportador.NumeroEntregas, value: this.NumeroEntregas });
        descricoes.push({ text: Localization.Resources.Enumeradores.CamposOpcionaisJanelaCarregamentoTransportador.Volumes, value: this.Volumes });
        descricoes.push({ text: Localization.Resources.Enumeradores.CamposOpcionaisJanelaCarregamentoTransportador.Remetente, value: this.Remetente });
        descricoes.push({ text: Localization.Resources.Enumeradores.CamposOpcionaisJanelaCarregamentoTransportador.Pedido, value: this.Pedido });
        descricoes.push({ text: Localization.Resources.Enumeradores.CamposOpcionaisJanelaCarregamentoTransportador.Peso, value: this.Peso });
        descricoes.push({ text: Localization.Resources.Enumeradores.CamposOpcionaisJanelaCarregamentoTransportador.Placas, value: this.Placas });
        descricoes.push({ text: Localization.Resources.Enumeradores.CamposOpcionaisJanelaCarregamentoTransportador.Motorista, value: this.Motorista });
        descricoes.push({ text: Localization.Resources.Enumeradores.CamposOpcionaisJanelaCarregamentoTransportador.Observacao, value: this.Observacao });
        descricoes.push({ text: Localization.Resources.Enumeradores.CamposOpcionaisJanelaCarregamentoTransportador.ObservacaoCarregamento, value: this.ObservacaoCarregamento });
        descricoes.push({ text: Localization.Resources.Enumeradores.CamposOpcionaisJanelaCarregamentoTransportador.ObservacaoCliente, value: this.ObservacaoCliente });
        descricoes.push({ text: Localization.Resources.Enumeradores.CamposOpcionaisJanelaCarregamentoTransportador.Ordem, value: this.Ordem });
        descricoes.push({ text: Localization.Resources.Enumeradores.CamposOpcionaisJanelaCarregamentoTransportador.Destinatario, value: this.Destinatario });
        descricoes.push({ text: Localization.Resources.Enumeradores.CamposOpcionaisJanelaCarregamentoTransportador.Rota, value: this.Rota });
        descricoes.push({ text: Localization.Resources.Enumeradores.CamposOpcionaisJanelaCarregamentoTransportador.Endereco, value: this.Endereco });
        descricoes.push({ text: Localization.Resources.Enumeradores.CamposOpcionaisJanelaCarregamentoTransportador.CDDestino, value: this.CDDestino });
        descricoes.push({ text: Localization.Resources.Enumeradores.CamposOpcionaisJanelaCarregamentoTransportador.Divisoria, value: this.DivisoriaIntegracaoLeilao });
        descricoes.push({ text: Localization.Resources.Enumeradores.CamposOpcionaisJanelaCarregamentoTransportador.ContemCargaPerigosa, value: this.CargaPerigosaIntegracaoLeilao });
        descricoes.push({ text: Localization.Resources.Enumeradores.CamposOpcionaisJanelaCarregamentoTransportador.CanalEntrega, value: this.CanalEntrega });
        descricoes.push({ text: Localization.Resources.Enumeradores.CamposOpcionaisJanelaCarregamentoTransportador.CanalVenda, value: this.CanalVenda });
        descricoes.push({ text: Localization.Resources.Enumeradores.CamposOpcionaisJanelaCarregamentoTransportador.NotasEnviadas, value: this.NotasEnviadas });
        descricoes.push({ text: Localization.Resources.Enumeradores.CamposOpcionaisJanelaCarregamentoTransportador.Doca, value: this.Doca });
        descricoes.push({ text: Localization.Resources.Enumeradores.CamposOpcionaisJanelaCarregamentoTransportador.ValorOfertado, value: this.ValorOfertado });
        descricoes.push({ text: Localization.Resources.Enumeradores.CamposOpcionaisJanelaCarregamentoTransportador.FreteSimulado, value: this.FreteSimulado });
        descricoes.push({ text: Localization.Resources.Enumeradores.CamposOpcionaisJanelaCarregamentoTransportador.CentroDeCarregamento, value: this.CentroDeCarregamento });
        descricoes.push({ text: Localization.Resources.Enumeradores.CamposOpcionaisJanelaCarregamentoTransportador.DataPrevisaoTerminoCarregamento, value: this.DataPrevisaoTerminoCarregamento });

        if (_CONFIGURACAO_TMS.PossuiIntegracaoKlios != null && _CONFIGURACAO_TMS.PossuiIntegracaoKlios)
            descricoes.push({ text: Localization.Resources.Enumeradores.CamposOpcionaisJanelaCarregamentoTransportador.ValidacaoConjunto, value: this.ValidacaoConjunto });

        return descricoes;
    }
}

var EnumCamposOpcionaisJanelaCarregamentoTransportador = Object.freeze(new EnumCamposOpcionaisJanelaCarregamentoTransportadorHelper());
