namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusRFIConvite
    {
        Aguardando = 0,
        Checklist = 1,
        AguardandoAprovacao = 2,
        AprovacaoRejeitada = 3,
        Fechamento = 4,        
    }

    public static class StatusRFIConviteHelper
    {
        public static string ObterDescricao(this StatusRFIConvite status)
        {
            switch (status)
            {
                case StatusRFIConvite.Aguardando: return "Aguardando Convite";
                case StatusRFIConvite.Checklist: return "Aguardando Checklist";
                case StatusRFIConvite.AguardandoAprovacao: return "Aguardando Aprovação";
                case StatusRFIConvite.AprovacaoRejeitada: return "Aprovação Rejeitada";
                case StatusRFIConvite.Fechamento: return "Finalizado";                
                default: return string.Empty;
            }
        }

        public static StatusRFIConvite ObterProximo(this StatusRFIConvite status)
        {
            switch (status)
            {
                case StatusRFIConvite.Aguardando: return StatusRFIConvite.Checklist;
                case StatusRFIConvite.Checklist: return StatusRFIConvite.Fechamento;
                default: return StatusRFIConvite.Fechamento;
            }
        }
    }
}
