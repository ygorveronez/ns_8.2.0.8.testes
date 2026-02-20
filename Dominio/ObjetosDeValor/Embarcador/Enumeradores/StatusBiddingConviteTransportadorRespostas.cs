namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusBiddingConviteTransportadorRespostas
    {
        Aguardando = 0,
        Aprovado = 1,
        Reprovado = 2,
    }

    public static class StatusBiddingConviteTransportadorRespostasHelper
    {
        public static string ObterDescricao(this StatusBiddingConviteTransportadorRespostas status)
        {
            switch (status)
            {
                case StatusBiddingConviteTransportadorRespostas.Aguardando: return "Aguardando Avaliação";
                case StatusBiddingConviteTransportadorRespostas.Aprovado: return "Aprovado";
                case StatusBiddingConviteTransportadorRespostas.Reprovado: return "Reprovado";
                default: return string.Empty;
            }
        }
    }
}
