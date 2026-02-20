namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoConciliacaoTransportador
    {
        Todos = 0,
        Aberta = 1,
        AbertaComOcorrencia = 2,
        DisponivelParaAnuencia = 3,
        Assinada = 4,
    }

    public static class SituacaoConciliacaoTransportadorHelper
    {
        public static string ObterDescricao(this SituacaoConciliacaoTransportador status)
        {
            switch (status)
            {
                case SituacaoConciliacaoTransportador.Aberta: return "Aberta";
                case SituacaoConciliacaoTransportador.AbertaComOcorrencia: return "Aberta com ocorrência";
                case SituacaoConciliacaoTransportador.DisponivelParaAnuencia: return "Disponível para anuência";
                case SituacaoConciliacaoTransportador.Assinada: return "Assinada";
                default: return string.Empty;
            }
        }
    }
}
