var EnumBotoesDetalheAcompanhamentoCargaHelper = function () {
    this.AdicionarPedidoReentrega = 1;
    this.DadosDaCarga = 2;
    this.VisualizarNoMapa = 3;
    this.BoletimDeEmbarque = 4;
    this.AdicionarEventos = 5;
    this.RaioXDaCarga = 6;
    this.Assumir = 7;
    this.Anotacoes = 8;
    this.OcorrenciasDeFrete = 9;
    this.HistoricoMonitoramento = 10;
    this.DetalhesPedidos = 11;
    this.BotaoPrimario = "Botão primário";
    this.BotaoSecundario = "Botão secundário";
};

EnumBotoesDetalheAcompanhamentoCargaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Adicionar Pedido Reentrega", value: this.AdicionarPedidoReentrega },
            { text: "Dados da Carga", value: this.DadosDaCarga },
            { text: "Visualizar no Mapa", value: this.VisualizarNoMapa },
            { text: "Boletim de Embarque", value: this.BoletimDeEmbarque },
            { text: "Adicionar Eventos", value: this.AdicionarEventos },
            { text: "Raio X da Carga", value: this.RaioXDaCarga },
            { text: "Assumir", value: this.Assumir },
            { text: "Anotações", value: this.Anotacoes },
            { text: "Ocorrências de Frete", value: this.OcorrenciasDeFrete },
            { text: "Histórico Monitoramento", value: this.HistoricoMonitoramento },
            { text: "Detalhes dos Pedidos", value: this.DetalhesPedidos },
        ]    
    }
}

var EnumBotoesDetalheAcompanhamentoCarga = Object.freeze(new EnumBotoesDetalheAcompanhamentoCargaHelper());