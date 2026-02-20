namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum EventoRelevanteMonitoramento
    {
        ConfirmarEntrega = 1,
        IniciarViagem = 2,
        ChegadaRaio = 3,
        SaidaRaio = 4,
        FinalizacaoMonitoramento = 5,
    }

    public static class EventoRelevanteMonitoramentoHelper
    {
        public static string ObterDescricao(this EventoRelevanteMonitoramento evento)
        {
            switch (evento)
            {
                case EventoRelevanteMonitoramento.ConfirmarEntrega : return "Confirmação";
                case EventoRelevanteMonitoramento.IniciarViagem : return "Início";
                case EventoRelevanteMonitoramento.ChegadaRaio : return "Chegada Entrega";
                case EventoRelevanteMonitoramento.SaidaRaio: return "Saída Entrega";
                case EventoRelevanteMonitoramento.FinalizacaoMonitoramento: return "Fim tracking";
                default: return string.Empty;
            }
        }
    }
}
