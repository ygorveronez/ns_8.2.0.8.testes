namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusBiddingConvite
    {
        Aguardando = 0,
        Checklist = 1,
        Ofertas = 2,
        Fechamento = 3,
        SemRegra = 4,
        AguardandoAprovacao = 5,
        AprovacaoRejeitada = 6
    }

    public static  class StatusBiddingConviteHelper
    {
        public static string ObterDescricao(this StatusBiddingConvite status)
        {
            switch (status)
            {
                case StatusBiddingConvite.Aguardando: return "Aguardando Convite";
                case StatusBiddingConvite.Checklist: return "Aguardando Checklist";
                case StatusBiddingConvite.Ofertas: return "Aguardando Ofertas";
                case StatusBiddingConvite.Fechamento: return "Finalizado";
                case StatusBiddingConvite.SemRegra: return "Sem Regra";
                case StatusBiddingConvite.AguardandoAprovacao: return "Aguardando Aprovação";
                case StatusBiddingConvite.AprovacaoRejeitada: return "Aprovação Rejeitada";
                default: return string.Empty;
            }
        }

        public static StatusBiddingConvite ObterProximo(this StatusBiddingConvite status)
        {
            switch (status)
            {
                case StatusBiddingConvite.Aguardando: return StatusBiddingConvite.Checklist;
                case StatusBiddingConvite.Checklist: return StatusBiddingConvite.Ofertas;
                case StatusBiddingConvite.Ofertas: return StatusBiddingConvite.Fechamento;
                default: return StatusBiddingConvite.Fechamento;
            }
        }
    }
}
