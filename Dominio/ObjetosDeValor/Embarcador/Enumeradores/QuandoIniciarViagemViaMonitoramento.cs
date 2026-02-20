namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum QuandoIniciarViagemViaMonitoramento
    {
        AoChegarNaOrigem = 1,
        AoSairDaOrigem = 2,
        NoStatusViagemTransito = 3
    }

    public static class QuandoIniciarViagemViaMonitoramentoaHelper
    {
        public static string ObterDescricao(this QuandoIniciarViagemViaMonitoramento o)
        {
            switch (o)
            {
                case QuandoIniciarViagemViaMonitoramento.AoChegarNaOrigem: return "Ao chegar na origem";
                case QuandoIniciarViagemViaMonitoramento.AoSairDaOrigem: return "Ao sair da origem";
                case QuandoIniciarViagemViaMonitoramento.NoStatusViagemTransito: return "No status de viagem \"Trânsito\"";
                default: return string.Empty;
            }
        }
    }
}



