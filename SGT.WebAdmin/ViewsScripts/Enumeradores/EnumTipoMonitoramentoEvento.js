/// <reference path="EnumTipoAlerta.js" />

var EnumTipoMonitoramentoEventoHelper = function () {
    this.Todos = "";
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
};

EnumTipoMonitoramentoEventoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Atraso na descarga", value: this.AtrasoNaDescarga },
            { text: "Atraso na entrega", value: this.AtrasoNaEntrega },
            { text: "Atraso na liberação", value: this.AtrasoNaLiberacao },
            { text: "Atraso no carregamento", value: this.AtrasoNoCarregamento },
            { text: "Ausência de inicio de viagem", value: this.AusenciaDeInicioDeViagem },
            { text: "Chegada no raio", value: this.ChegadaNoRaio },
            { text: "Chegada no raio da entrega", value: this.ChegadaNoRaioEntrega },
            { text: "Concentração de Veículos no Raio", value: this.ConcentracaoDeVeiculosNoRaio },
            { text: "Desvio de rota", value: this.DesvioDeRota },
            { text: "Direção contínua excessiva", value: this.DirecaoContinuaExcessiva },
            { text: "Direção sem descanso", value: this.DirecaoSemDescanso },
            { text: "Fora do prazo", value: this.ForaDoPrazo },
            { text: "Início Viagem Sem Documentação", value: this.InicioViagemSemDocumentacao },
            { text: "Parada em área de risco", value: this.ParadaEmAreaDeRisco },
            { text: "Parada excessiva", value: this.ParadaExcessiva },
            { text: "Parada não programada", value: this.ParadaNaoProgramada },
            { text: "Perda de sinal", value: this.PerdaDeSinal },
            { text: "Permanência no ponto de apoio", value: this.PermanenciaNoPontoApoio },
            { text: "Permanência no raio", value: this.PermanenciaNoRaio },
            { text: "Permanência no raio da entrega", value: this.PermanenciaNoRaioEntrega },
            { text: "Possível Atraso na Origem", value: this.PossivelAtrasoNaOrigem },
            { text: "Sem sinal", value: this.SemSinal },
            { text: "Sensor de tempertura com problema", value: this.SensorTemperaturaComProblema },
            { text: "Temperatura fora da faixa", value: this.TemperaturaForaDaFaixa },
            { text: "Tendência de entrega adiantada", value: this.AlertaTendenciaEntregaAdiantada },
            { text: "Tendência de entrega atrasada", value: this.AlertaTendenciaEntregaAtrasada },
            { text: "Velocidade excedida", value: this.VelocidadeExcedida },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    },
    obterTipoAlerta: function (tipoMonitoramentoEvento) {
        switch (tipoMonitoramentoEvento) {
            case this.AtrasoNaDescarga: return EnumTipoAlerta.AtrasoNaDescarga;
            case this.AtrasoNaEntrega: return EnumTipoAlerta.AtrasoNaEntrega;
            case this.AtrasoNaLiberacao: return EnumTipoAlerta.AtrasoNaLiberacao;
            case this.AtrasoNoCarregamento: return EnumTipoAlerta.AtrasoNoCarregamento;
            case this.AusenciaDeInicioDeViagem: return EnumTipoAlerta.AusenciaDeInicioDeViagem;
            case this.ChegadaNoRaio: return EnumTipoAlerta.ChegadaNoRaio;
            case this.ChegadaNoRaioEntrega: return EnumTipoAlerta.ChegadaNoRaioEntrega;
            case this.ConcentracaoDeVeiculosNoRaio: return EnumTipoAlerta.ConcentracaoDeVeiculosNoRaio;
            case this.DesvioDeRota: return EnumTipoAlerta.DesvioDeRota;
            case this.DirecaoContinuaExcessiva: return EnumTipoAlerta.DirecaoContinuaExcessiva;
            case this.DirecaoSemDescanso: return EnumTipoAlerta.DirecaoSemDescanso;
            case this.ForaDoPrazo: return EnumTipoAlerta.ForaDoPrazo;
            case this.InicioViagemSemDocumentacao: return EnumTipoAlerta.InicioViagemSemDocumentacao;
            case this.ParadaEmAreaDeRisco: return EnumTipoAlerta.ParadaEmAreaDeRisco;
            case this.ParadaExcessiva: return EnumTipoAlerta.ParadaExcessiva;
            case this.ParadaNaoProgramada: return EnumTipoAlerta.ParadaNaoProgramada;
            case this.PerdaDeSinal: return EnumTipoAlerta.PerdaDeSinal;
            case this.PermanenciaNoPontoApoio: return EnumTipoAlerta.PermanenciaNoPontoApoio;
            case this.PermanenciaNoRaio: return EnumTipoAlerta.PermanenciaNoRaio;
            case this.PermanenciaNoRaioEntrega: return EnumTipoAlerta.PermanenciaNoRaioEntrega;
            case this.PossivelAtrasoNaOrigem: return EnumTipoAlerta.PossivelAtrasoNaOrigem;
            case this.SemSinal: return EnumTipoAlerta.SemSinal;
            case this.SensorTemperaturaComProblema: return EnumTipoAlerta.SensorTemperaturaComProblema;
            case this.TemperaturaForaDaFaixa: return EnumTipoAlerta.TemperaturaForaDaFaixa;
            case this.AlertaTendenciaEntregaAdiantada: return EnumTipoAlerta.AlertaTendenciaEntregaAdiantada;
            case this.AlertaTendenciaEntregaAtrasada: return EnumTipoAlerta.AlertaTendenciaEntregaAtrasada;
            case this.VelocidadeExcedida: return EnumTipoAlerta.VelocidadeExcedida;
            default: return EnumTipoAlerta.SemAlerta;
        }
    },
    obterDescricaoPorTipo: function (tipoMonitoramentoEvento) {
        switch (tipoMonitoramentoEvento) {
            case this.AlertaTendenciaEntregaAdiantada:
                return Localization.Resources.Logistica.Monitoramento.DescricaoAlertaAlertaTendenciaEntregaAdiantada;
            case this.AlertaTendenciaEntregaAtrasada:
                return Localization.Resources.Logistica.Monitoramento.DescricaoAlertaAlertaTendenciaEntregaAtrasada;
            case this.AtrasoNaDescarga:
                return Localization.Resources.Logistica.Monitoramento.DescricaoAlertaAtrasoNaDescarga;
            case this.AtrasoNaEntrega:
                return Localization.Resources.Logistica.Monitoramento.DescricaoAlertaAtrasoNaEntrega;
            case this.AtrasoNaLiberacao:
                return Localization.Resources.Logistica.Monitoramento.DescricaoAlertaAtrasoNaLiberacao;
            case this.AtrasoNoCarregamento:
                return Localization.Resources.Logistica.Monitoramento.DescricaoAlertaAtrasoNoCarregamento;
            case this.AusenciaDeInicioDeViagem:
                return Localization.Resources.Logistica.Monitoramento.DescricaoAlertaAusenciaDeInicioDeViagem;
            case this.ChegadaNoRaio:
                return Localization.Resources.Logistica.Monitoramento.DescricaoAlertaChegadaNoRaio;
            case this.ChegadaNoRaioEntrega:
                return Localization.Resources.Logistica.Monitoramento.DescricaoAlertaChegadaNoRaioEntrega;
            case this.ConcentracaoDeVeiculosNoRaio:
                return Localization.Resources.Logistica.Monitoramento.DescricaoAlertaConcentracaoDeVeiculosNoRaio;
            case this.DesvioDeRota:
                return Localization.Resources.Logistica.Monitoramento.DescricaoAlertaDesvioDeRota;
            case this.DirecaoContinuaExcessiva:
                return Localization.Resources.Logistica.Monitoramento.DescricaoAlertaDirecaoContinuaExcessiva;
            case this.DirecaoSemDescanso:
                return Localization.Resources.Logistica.Monitoramento.DescricaoAlertaDirecaoSemDescanso;
            case this.ForaDoPrazo:
                return Localization.Resources.Logistica.Monitoramento.DescricaoAlertaForaDoPrazo;
            case this.InicioViagemSemDocumentacao:
                return Localization.Resources.Logistica.Monitoramento.DescricaoAlertaInicioViagemSemDocumentacao;
            case this.ParadaEmAreaDeRisco:
                return Localization.Resources.Logistica.Monitoramento.DescricaoAlertaParadaEmAreaDeRisco;
            case this.ParadaExcessiva:
                return Localization.Resources.Logistica.Monitoramento.DescricaoAlertaParadaExcessiva;
            case this.ParadaNaoProgramada:
                return Localization.Resources.Logistica.Monitoramento.DescricaoAlertaParadaNaoProgramada;
            case this.PerdaDeSinal:
                return Localization.Resources.Logistica.Monitoramento.DescricaoAlertaPerdaDeSinal;
            case this.PermanenciaNoPontoApoio:
                return Localization.Resources.Logistica.Monitoramento.DescricaoAlertaPermanenciaNoPontoApoio;
            case this.PermanenciaNoRaio:
                return Localization.Resources.Logistica.Monitoramento.DescricaoAlertaPermanenciaNoRaio;
            case this.PermanenciaNoRaioEntrega:
                return Localization.Resources.Logistica.Monitoramento.DescricaoAlertaPermanenciaNoRaioEntrega;
            case this.PossivelAtrasoNaOrigem:
                return Localization.Resources.Logistica.Monitoramento.DescricaoAlertaPossivelAtrasoNaOrigem;
            case this.SemSinal:
                return Localization.Resources.Logistica.Monitoramento.DescricaoAlertaSemSinal;
            case this.SensorTemperaturaComProblema:
                return Localization.Resources.Logistica.Monitoramento.DescricaoAlertaSensorTemperaturaComProblema;
            case this.TemperaturaForaDaFaixa:
                return Localization.Resources.Logistica.Monitoramento.DescricaoAlertaTemperaturaForaDaFaixa;
            case this.VelocidadeExcedida:
                return Localization.Resources.Logistica.Monitoramento.DescricaoAlertaVelocidadeExcedida;
            default:
                return "";
        }
    },
    obterIconePorTipo: function (tipoMonitoramentoEvento) {
        switch (tipoMonitoramentoEvento) {
            case this.AlertaTendenciaEntregaAdiantada:
                return "Content/TorreControle/Icones/alertas/tendencia-adiantamento.svg";
            case this.AlertaTendenciaEntregaAtrasada:
                return "Content/TorreControle/Icones/alertas/tendencia-atraso.svg";
            case this.AtrasoNaDescarga:
                return "Content/TorreControle/Icones/alertas/espera.svg";
            case this.AtrasoNaEntrega:
                return "Content/TorreControle/Icones/alertas/espera.svg";
            case this.AtrasoNaLiberacao:
                return "Content/TorreControle/Icones/alertas/espera.svg";
            case this.AtrasoNoCarregamento:
                return "Content/TorreControle/Icones/alertas/espera.svg";
            case this.AusenciaDeInicioDeViagem:
                return "Content/TorreControle/Icones/alertas/inicio-viagem-problema.svg";
            case this.ChegadaNoRaio:
                return "Content/TorreControle/Icones/alertas/chegada-raio-entrega.svg";
            case this.ChegadaNoRaioEntrega:
                return "Content/TorreControle/Icones/alertas/chegada-raio-entrega.svg";
            case this.ConcentracaoDeVeiculosNoRaio:
                return "Content/TorreControle/Icones/alertas/permanencia-raio.svg";
            case this.DesvioDeRota:
                return "Content/TorreControle/Icones/alertas/desvio-rota.svg";
            case this.DirecaoContinuaExcessiva:
                return "Content/TorreControle/Icones/alertas/direcao-problema.svg";
            case this.DirecaoSemDescanso:
                return "Content/TorreControle/Icones/alertas/direcao-problema.svg";
            case this.ForaDoPrazo:
                return "Content/TorreControle/Icones/alertas/espera.svg";
            case this.InicioViagemSemDocumentacao:
                return "Content/TorreControle/Icones/alertas/inicio-viagem-problema.svg";
            case this.ParadaEmAreaDeRisco:
                return "Content/TorreControle/Icones/alertas/parada-excessiva.svg";
            case this.ParadaExcessiva:
                return "Content/TorreControle/Icones/alertas/parada-excessiva.svg";
            case this.ParadaNaoProgramada:
                return "Content/TorreControle/Icones/alertas/parada-excessiva.svg";
            case this.PerdaDeSinal:
                return "Content/TorreControle/Icones/alertas/sem-sinal.svg";
            case this.PermanenciaNoPontoApoio:
                return "Content/TorreControle/Icones/alertas/permanencia-ponto-apoio.svg";
            case this.PermanenciaNoRaio:
                return "Content/TorreControle/Icones/alertas/permanencia-raio.svg";
            case this.PermanenciaNoRaioEntrega:
                return "Content/TorreControle/Icones/alertas/permanencia-raio.svg";
            case this.PossivelAtrasoNaOrigem:
                return "Content/TorreControle/Icones/alertas/espera.svg";
            case this.SemSinal:
                return "Content/TorreControle/Icones/alertas/sem-sinal.svg";
            case this.SensorTemperaturaComProblema:
                return "Content/TorreControle/Icones/alertas/temperatura.svg";
            case this.TemperaturaForaDaFaixa:
                return "Content/TorreControle/Icones/alertas/temperatura.svg";
            case this.VelocidadeExcedida:
                return "Content/TorreControle/Icones/alertas/velocidade.svg";
            default:
                return "Content/TorreControle/AcompanhamentoCarga/assets/icons/default.png";
        }
    }
};

var EnumTipoMonitoramentoEvento = Object.freeze(new EnumTipoMonitoramentoEventoHelper());