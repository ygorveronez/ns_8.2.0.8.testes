using System;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoAlerta
    {
        SemAlerta = 0,
        DesvioDeRota = 1,
        VelocidadeExcedida = 2,
        ParadaNaoProgramada = 3,
        TemperaturaForaDaFaixa = 4,
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

    public static class TipoAlertaHelper
    {

        public static ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta ObterTipoDeAlertaPorDescricao(string Descricao)
        {
            foreach (int i in Enum.GetValues(typeof(TipoAlerta)))
            {
                TipoAlerta tipo = (TipoAlerta)Enum.ToObject(typeof(TipoAlerta), i);
                if (TipoAlertaHelper.ObterDescricao(tipo) == Descricao)
                    return tipo;
            }

            return TipoAlerta.SemAlerta;
        }

        public static string ObterDescricao(this TipoAlerta tipo)
        {
            switch (tipo)
            {
                case TipoAlerta.SemAlerta: return "Sem alerta";
                case TipoAlerta.DesvioDeRota: return "Desvio de rota";
                case TipoAlerta.VelocidadeExcedida: return "Velocidade excedida";
                case TipoAlerta.ParadaNaoProgramada: return "Parada não programada";
                case TipoAlerta.TemperaturaForaDaFaixa: return "Temperatura fora da faixa";
                case TipoAlerta.ParadaEmAreaDeRisco: return "Parada em área de risco";
                case TipoAlerta.PerdaDeSinal: return "Perda de sinal";
                case TipoAlerta.InicioDeViagem: return "Inicio de viagem";
                case TipoAlerta.FimDeViagem: return "Fim de viagem";
                case TipoAlerta.InicioEntrega: return "Inicio de entrega";
                case TipoAlerta.FimEntrega: return "Fim entrega";
                case TipoAlerta.SensorTemperaturaComProblema: return "Sensor de temperatura com problema";
                case TipoAlerta.SensorDesengate: return "Sensor de Desengate";
                case TipoAlerta.SemSinal: return "Sem sinal";
                case TipoAlerta.Pernoite: return "Pernoite";
                case TipoAlerta.ParadaExcessiva: return "Parada excessiva";
                case TipoAlerta.DirecaoContinuaExcessiva: return "Direção continua excessiva";
                case TipoAlerta.Almoco: return "Almoço";
                case TipoAlerta.Espera: return "Espera";
                case TipoAlerta.Repouso: return "Repouso";
                case TipoAlerta.InicioViagemSemDocumentacao: return "Início Viagem Sem Documentação";
                case TipoAlerta.Abastecimento: return "Abastecimento";
                case TipoAlerta.AtrasoNoCarregamento: return "Atraso no carregamento";
                case TipoAlerta.AtrasoNaLiberacao: return "Atraso na liberação";
                case TipoAlerta.AtrasoNaEntrega: return "Atraso na entrega";
                case TipoAlerta.AtrasoNaDescarga: return "Atraso na descarga";
                case TipoAlerta.DirecaoSemDescanso: return "Direção sem descanso";
                case TipoAlerta.ChegadaNoRaio: return "Chegada no raio";
                case TipoAlerta.ChegadaNoRaioEntrega: return "Chegada no raio da entrega";
                case TipoAlerta.PermanenciaNoRaio: return "Permanência no raio";
                case TipoAlerta.PermanenciaNoRaioEntrega: return "Permanência no raio da entrega";
                case TipoAlerta.ForaDoPrazo: return "Fora do prazo";
                case TipoAlerta.PermanenciaNoPontoApoio: return "Permanência no ponto de apoio";
                case TipoAlerta.AusenciaDeInicioDeViagem: return "Ausência de inicio de viagem";
                case TipoAlerta.PossivelAtrasoNaOrigem: return "Possível Atraso na Origem";
                case TipoAlerta.ConcentracaoDeVeiculosNoRaio: return "Concentração de Veículos no Raio";
                case TipoAlerta.AlertaTendenciaEntregaAdiantada: return "Alerta de tendência de entrega adiantada";
                case TipoAlerta.AlertaTendenciaEntregaAtrasada: return "Alerta de tendência de entrega atrasada";
                case TipoAlerta.AlertaTendenciaEntregaPoucoAtrasada: return "Alerta de tendência de entrega pouco atrasada";
                default: return string.Empty;
            }
        }
    }


}
