namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoMonitoramentoEvento
    {
        DesvioDeRota = 1,
        Velocidade = 2,
        ParadaNaoProgramada = 3,
        Temperatura = 4,
        ParadaEmAreaDeRisco = 5,
        PerdaDeSinal = 6,
        InicioDeViagem = 7,
        FimDeViagem = 8,
        InicioEntrega = 9,
        FimEntrega = 10,
        SensorTemperaturaComProblema = 11,
        SemSinal = 12,
        Pernoite = 13,
        ParadaExcessiva = 14,
        DirecaoContinuaExcessiva = 15,
        Almoco = 16,
        Espera = 17,
        Repouso = 18,
        Abastecimento = 19,
        AtrasoNoCarregamento = 20,
        AtrasoNaLiberacao = 21,
        AtrasoNaEntrega = 23,
        AtrasoNaDescarga = 24,
        DirecaoSemDescanso = 25,
        ChegadaNoRaio = 26,
        ChegadaNoRaioEntrega = 27,
        PermanenciaNoRaio = 28,
        PermanenciaNoRaioEntrega = 29,
        ForaDoPrazo = 30,
        InicioViagemSemDocumentacao = 31,
        SensorDesengate = 32,
        PermanenciaNoPontoApoio = 33,
        AusenciaDeInicioDeViagem = 34,
        PossivelAtrasoNaOrigem = 35,
        ConcentracaoDeVeiculosNoRaio = 36,
        AlertaTendenciaEntregaAdiantada = 37,
        AlertaTendenciaEntregaAtrasada = 38,
        AlertaTendenciaEntregaPoucoAtrasada = 39,

    }

    public static class TipoMonitoramentoEventoHelper
    {
        public static string ObterDescricao(this TipoMonitoramentoEvento tipo)
        {
            switch (tipo)
            {
                case TipoMonitoramentoEvento.DesvioDeRota: return "Desvio de rota";
                case TipoMonitoramentoEvento.Velocidade: return "Velocidade excedida";
                case TipoMonitoramentoEvento.ParadaNaoProgramada: return "Parada não programada";
                case TipoMonitoramentoEvento.Temperatura: return "Temperatura Fora da faixa";
                case TipoMonitoramentoEvento.InicioDeViagem: return "Início da viagem";
                case TipoMonitoramentoEvento.FimDeViagem: return "Fim da viagem";
                case TipoMonitoramentoEvento.InicioEntrega: return "Início da entrega";
                case TipoMonitoramentoEvento.FimEntrega: return "Fim da entrega";
                case TipoMonitoramentoEvento.PerdaDeSinal: return "Perda de sinal";
                case TipoMonitoramentoEvento.SensorTemperaturaComProblema: return "Sensor de tempertura com problema";
                case TipoMonitoramentoEvento.SensorDesengate: return "Sensor de Desengate";
                case TipoMonitoramentoEvento.ParadaEmAreaDeRisco: return "Parada em área de risco";
                case TipoMonitoramentoEvento.SemSinal: return "Sem sinal";
                case TipoMonitoramentoEvento.Pernoite: return "Notificação de pernoite";
                case TipoMonitoramentoEvento.ParadaExcessiva: return "Parada excessiva";
                case TipoMonitoramentoEvento.DirecaoContinuaExcessiva: return "Direção contínua excessiva";
                case TipoMonitoramentoEvento.Almoco: return "Almoço";
                case TipoMonitoramentoEvento.Espera: return "Espera";
                case TipoMonitoramentoEvento.Repouso: return "Repouso";
                case TipoMonitoramentoEvento.Abastecimento: return "Abastecimento";
                case TipoMonitoramentoEvento.AtrasoNoCarregamento: return "Atraso no carregamento";
                case TipoMonitoramentoEvento.AtrasoNaLiberacao: return "Atraso na liberação";
                case TipoMonitoramentoEvento.AtrasoNaEntrega: return "Atraso na entrega";
                case TipoMonitoramentoEvento.AtrasoNaDescarga: return "Atraso na descarga";
                case TipoMonitoramentoEvento.DirecaoSemDescanso: return "Direção sem descanso";
                case TipoMonitoramentoEvento.ChegadaNoRaio: return "Chegada no raio";
                case TipoMonitoramentoEvento.ChegadaNoRaioEntrega: return "Chegada no raio da entrega";
                case TipoMonitoramentoEvento.InicioViagemSemDocumentacao: return "Início Viagem Sem Documentação";
                case TipoMonitoramentoEvento.PermanenciaNoRaio: return "Permanência no raio";
                case TipoMonitoramentoEvento.PermanenciaNoRaioEntrega: return "Permanência no raio da entrega";
                case TipoMonitoramentoEvento.ForaDoPrazo: return "Fora do prazo";
                case TipoMonitoramentoEvento.PermanenciaNoPontoApoio: return "Permanência no ponto de apoio";
                case TipoMonitoramentoEvento.AusenciaDeInicioDeViagem: return "Ausência de inicio de viagem";
                case TipoMonitoramentoEvento.PossivelAtrasoNaOrigem: return "Possível Atraso na Origem";
                case TipoMonitoramentoEvento.ConcentracaoDeVeiculosNoRaio: return "Concentração de Veículos no Raio";
                case TipoMonitoramentoEvento.AlertaTendenciaEntregaAdiantada: return "Alerta de tendência de entrega adiantada";
                case TipoMonitoramentoEvento.AlertaTendenciaEntregaAtrasada: return "Alerta de tendência de entrega atrasada";
                case TipoMonitoramentoEvento.AlertaTendenciaEntregaPoucoAtrasada: return "Alerta de tendência de entrega pouco atrasada";
                default: return string.Empty;
            }
        }

        public static TipoAlerta ObterTipoAlerta(this TipoMonitoramentoEvento tipo)
        {
            switch (tipo)
            {
                case TipoMonitoramentoEvento.DesvioDeRota: return TipoAlerta.DesvioDeRota;
                case TipoMonitoramentoEvento.Velocidade: return TipoAlerta.VelocidadeExcedida;
                case TipoMonitoramentoEvento.ParadaNaoProgramada: return TipoAlerta.ParadaNaoProgramada;
                case TipoMonitoramentoEvento.Temperatura: return TipoAlerta.TemperaturaForaDaFaixa;
                case TipoMonitoramentoEvento.PerdaDeSinal: return TipoAlerta.PerdaDeSinal;
                case TipoMonitoramentoEvento.SensorTemperaturaComProblema: return TipoAlerta.SensorTemperaturaComProblema;
                case TipoMonitoramentoEvento.SensorDesengate: return TipoAlerta.SensorDesengate;
                case TipoMonitoramentoEvento.ParadaEmAreaDeRisco: return TipoAlerta.ParadaEmAreaDeRisco;
                case TipoMonitoramentoEvento.SemSinal: return TipoAlerta.SemSinal;
                case TipoMonitoramentoEvento.Pernoite: return TipoAlerta.Pernoite;
                case TipoMonitoramentoEvento.ParadaExcessiva: return TipoAlerta.ParadaExcessiva;
                case TipoMonitoramentoEvento.DirecaoContinuaExcessiva: return TipoAlerta.DirecaoContinuaExcessiva;
                case TipoMonitoramentoEvento.Almoco: return TipoAlerta.Almoco;
                case TipoMonitoramentoEvento.Espera: return TipoAlerta.Espera;
                case TipoMonitoramentoEvento.Repouso: return TipoAlerta.Repouso;
                case TipoMonitoramentoEvento.InicioViagemSemDocumentacao: return TipoAlerta.InicioViagemSemDocumentacao;
                case TipoMonitoramentoEvento.Abastecimento: return TipoAlerta.Abastecimento;
                case TipoMonitoramentoEvento.AtrasoNoCarregamento: return TipoAlerta.AtrasoNoCarregamento;
                case TipoMonitoramentoEvento.AtrasoNaLiberacao: return TipoAlerta.AtrasoNaLiberacao;
                case TipoMonitoramentoEvento.AtrasoNaEntrega: return TipoAlerta.AtrasoNaEntrega;
                case TipoMonitoramentoEvento.AtrasoNaDescarga: return TipoAlerta.AtrasoNaDescarga;
                case TipoMonitoramentoEvento.ChegadaNoRaio: return TipoAlerta.ChegadaNoRaio;
                case TipoMonitoramentoEvento.ChegadaNoRaioEntrega: return TipoAlerta.ChegadaNoRaioEntrega;
                case TipoMonitoramentoEvento.PermanenciaNoRaio: return TipoAlerta.PermanenciaNoRaio;
                case TipoMonitoramentoEvento.PermanenciaNoRaioEntrega: return TipoAlerta.PermanenciaNoRaioEntrega;
                case TipoMonitoramentoEvento.ForaDoPrazo: return TipoAlerta.ForaDoPrazo;
                case TipoMonitoramentoEvento.PermanenciaNoPontoApoio: return TipoAlerta.PermanenciaNoPontoApoio;
                case TipoMonitoramentoEvento.AusenciaDeInicioDeViagem: return TipoAlerta.AusenciaDeInicioDeViagem;
                case TipoMonitoramentoEvento.PossivelAtrasoNaOrigem: return TipoAlerta.PossivelAtrasoNaOrigem;
                case TipoMonitoramentoEvento.ConcentracaoDeVeiculosNoRaio: return TipoAlerta.ConcentracaoDeVeiculosNoRaio;
                case TipoMonitoramentoEvento.AlertaTendenciaEntregaAdiantada: return TipoAlerta.AlertaTendenciaEntregaAdiantada;
                case TipoMonitoramentoEvento.AlertaTendenciaEntregaAtrasada: return TipoAlerta.AlertaTendenciaEntregaAtrasada;
                case TipoMonitoramentoEvento.AlertaTendenciaEntregaPoucoAtrasada: return TipoAlerta.AlertaTendenciaEntregaPoucoAtrasada;


                default: return TipoAlerta.SemAlerta;
            }
        }
    }
}
