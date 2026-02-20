namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusBiddingRota
    {
        Aguardando = 0,
        EmAnalise = 1,
        Rejeitada = 2,
        Aprovada = 3,
        NovaRodada = 4
    }

    public static class StatusBiddingRotaHelper
    {
        public static string ObterDescricao(this StatusBiddingRota status)
        {
            switch (status)
            {
                case StatusBiddingRota.Aguardando: return "Aguardando Oferta";
                case StatusBiddingRota.EmAnalise: return "Aguardando Avaliação";
                case StatusBiddingRota.Rejeitada: return "Oferta Rejeitada";
                case StatusBiddingRota.Aprovada: return "Aprovada";
                default: return string.Empty;
            }
        }

        public static string ObterCorLinha(this StatusBiddingRota status)
        {
            switch (status)
            {
                case StatusBiddingRota.Aguardando: return "#7dafdb";
                case StatusBiddingRota.EmAnalise: return CorGrid.Amarelo;
                case StatusBiddingRota.Rejeitada: return CorGrid.Vermelho;
                case StatusBiddingRota.Aprovada: return CorGrid.Verde;
                default: return string.Empty;
            }
        }

        public static string ObterCorFonte(this StatusBiddingRota status)
        {
            switch (status)
            {
                case StatusBiddingRota.Aguardando: return CorGrid.Branco;
                case StatusBiddingRota.EmAnalise: return CorGrid.Black;
                case StatusBiddingRota.Rejeitada: return CorGrid.Branco;
                case StatusBiddingRota.Aprovada: return CorGrid.Black;
                default: return string.Empty;
            }
        }
    }
}
