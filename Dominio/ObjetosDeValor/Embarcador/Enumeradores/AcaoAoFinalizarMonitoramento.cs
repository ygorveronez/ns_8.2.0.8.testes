namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum AcaoAoFinalizarMonitoramento
    {
        Nenhuma = 1,
        IniciarProximoMonitoramentoAgendadoPorDataCriacao = 2,
        IniciarProximoMonitoramentoAgendadoPorDataCarregamentoCarga = 3
    }

    public static class AcaoAoFinalizarMonitoramentoHelper
    {
        public static string ObterDescricao(this AcaoAoFinalizarMonitoramento o)
        {
            switch (o)
            {
                case AcaoAoFinalizarMonitoramento.Nenhuma: return "Nenhuma";
                case AcaoAoFinalizarMonitoramento.IniciarProximoMonitoramentoAgendadoPorDataCriacao: return "Iniciar próximo monitoramento agendado por data de criação";
                case AcaoAoFinalizarMonitoramento.IniciarProximoMonitoramentoAgendadoPorDataCarregamentoCarga: return "Iniciar próximo monitoramento agendado por data de carregamento da carga";
                default: return string.Empty;
            }
        }
    }
}
