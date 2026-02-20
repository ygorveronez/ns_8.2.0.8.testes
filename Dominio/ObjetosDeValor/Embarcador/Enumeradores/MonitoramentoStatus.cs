namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum MonitoramentoStatus
    {
        Aguardando = 0,
        Iniciado = 1,
        Finalizado = 2,
        Cancelado = 3,
        Todos = 99
    }

    public static class MonitoramentoStatusHelper
    {
        public static string ObterDescricao(this MonitoramentoStatus monitoramentoStatus)
        {
            switch (monitoramentoStatus)
            {
                case MonitoramentoStatus.Aguardando: return Localization.Resources.Enumeradores.MonitoramentoStatus.Agendado;
                case MonitoramentoStatus.Iniciado: return Localization.Resources.Enumeradores.MonitoramentoStatus.EmMonitoramento;
                case MonitoramentoStatus.Finalizado: return Localization.Resources.Enumeradores.MonitoramentoStatus.Finalizado;
                case MonitoramentoStatus.Cancelado: return Localization.Resources.Enumeradores.MonitoramentoStatus.Cancelado;
                default: return string.Empty;
            }
        }
    }
}
