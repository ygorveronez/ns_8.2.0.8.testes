var EnumTipoAlertaHelper = function () {
    this.SemAlerta = 0;
    this.DesvioDeRota = 1;
    this.VelocidadeExcedida = 2;
    this.ParadaNaoProgramada = 3;
    this.TemperaturaForaDaFaixa = 4;
    this.ParadaEmAreaDeRisco = 5;
    this.PerdaDeSinal = 6;
    this.SensorTemperaturaComProblema = 11;
    this.SemSinal = 12;
    this.Pernoite = 13;
    this.ParadaExcessiva = 14;
    this.DirecaoContinuaExcessiva = 15;
    this.Almoco = 16;
    this.Espera = 17;
    this.Repouso = 18;
    this.Abastecimento = 19;
    this.AtrasoNoCarregamento = 20;
    this.AtrasoNaLiberacao = 21;
    this.AtrasoNaEntrega = 23;
    this.AtrasoNaDescarga = 24;
    this.DirecaoSemDescanso = 25;
    this.ChegadaNoRaio = 26;
    this.ChegadaNoRaioEntrega = 27;
    this.PermanenciaNoRaio = 28;
    this.PermanenciaNoRaioEntrega = 29;
    this.ForaDoPrazo = 30;
    this.InicioViagemSemDocumentacao = 31;
    this.SensorDesengate = 32;
    this.PermanenciaNoPontoApoio = 33;
    this.AusenciaDeInicioDeViagem = 34;
    this.PossivelAtrasoNaOrigem = 35;
    this.ConcentracaoDeVeiculosNoRaio = 36;
    this.AlertaTendenciaEntregaAdiantada = 37;
    this.AlertaTendenciaEntregaAtrasada = 38;
    this.AlertaTendenciaEntregaPoucoAtrasada = 39;
}

EnumTipoAlertaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Abastecimento", value: this.Abastecimento },
            { text: "Almoço", value: this.Almoco },
            { text: "Atraso no carregamento", value: this.AtrasoNoCarregamento },
            { text: "Atraso na descarga", value: this.AtrasoNaDescarga },
            { text: "Atraso na entrega", value: this.AtrasoNaEntrega },
            { text: "Atraso na liberação", value: this.AtrasoNaLiberacao },
            { text: "Chegada no raio", value: this.ChegadaNoRaio },
            { text: "Chegada no raio da entrega", value: this.ChegadaNoRaioEntrega },
            { text: "Desvio de rota", value: this.DesvioDeRota },
            { text: "Direção continua excessiva", value: this.DirecaoContinuaExcessiva },
            { text: "Direção sem descanso", value: this.DirecaoSemDescanso },
            { text: "Espera", value: this.Espera },
            { text: "Fora do prazo", value: this.ForaDoPrazo },
            { text: "Início Viagem Sem Documentação", value: this.InicioViagemSemDocumentacao },
            { text: "Parada em área de risco", value: this.ParadaEmAreaDeRisco },
            { text: "Parada excessiva", value: this.ParadaExcessiva },
            { text: "Parada não programada", value: this.ParadaNaoProgramada },
            { text: "Perda de sinal", value: this.PerdaDeSinal },
            { text: "Permanência no raio", value: this.PermanenciaNoRaio },
            { text: "Permanência no raio da entrega", value: this.PermanenciaNoRaioEntrega },
            { text: "Pernoite", value: this.Pernoite },
            { text: "Sem sinal", value: this.SemSinal },
            { text: "Sem alerta", value: this.SemAlerta },
            { text: "Sensor de temperatura com problema", value: this.SensorTemperaturaComProblema },
            { text: "Sensor de Desengate", value: this.SensorDesengate },
            { text: "Repouso", value: this.Repouso },
            { text: "Temperatura fora da faixa", value: this.TemperaturaForaDaFaixa },
            { text: "Velocidade excedida", value: this.VelocidadeExcedida },
            { text: "Permanência no ponto de apoio", value: this.PermanenciaNoPontoApoio },
            { text: "Ausência de inicio de viagem", value: this.AusenciaDeInicioDeViagem },
            { text: "Possível Atraso na Origem", value: this.PossivelAtrasoNaOrigem },
            { text: "Concentração de Veículos no Raio", value: this.ConcentracaoDeVeiculosNoRaio },
            { text: "Tendência Entrega Adiantada", value: this.AlertaTendenciaEntregaAdiantada },
            { text: "Tendência Entrega Atrasada", value: this.AlertaTendenciaEntregaAtrasada },
            { text: "Tendência Entrega Pouco Atrasada", value: this.AlertaTendenciaEntregaPoucoAtrasada }

        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumTipoAlerta = Object.freeze(new EnumTipoAlertaHelper());