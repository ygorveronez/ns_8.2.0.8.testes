namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TratativaAutomaticaMonitoramentoEvento
    {
        InicioViagem = 0,
        ConfirmacaoEntrega = 1,
        RetornoSinal = 2, 
        SensorTemperaturaComProblema = 3,
        TendenciaDeEntregaAdiantada = 4
    }

    public static class TratativaAutomaticaMonitoramentoEventoHelper
    {
        public static string ObterDescricao(this TratativaAutomaticaMonitoramentoEvento tipo)
        {
            switch (tipo)
            {
                case TratativaAutomaticaMonitoramentoEvento.InicioViagem: return "Início da Viagem";
                case TratativaAutomaticaMonitoramentoEvento.ConfirmacaoEntrega: return "Confirmação da Entrega";
                case TratativaAutomaticaMonitoramentoEvento.RetornoSinal: return "Retorno de Sinal";
                case TratativaAutomaticaMonitoramentoEvento.SensorTemperaturaComProblema: return "Sensor de Temperatura com Problema";
                case TratativaAutomaticaMonitoramentoEvento.TendenciaDeEntregaAdiantada: return "Tendência de entrega adiantada";
                default: return string.Empty;
            }
        }
    }
}
