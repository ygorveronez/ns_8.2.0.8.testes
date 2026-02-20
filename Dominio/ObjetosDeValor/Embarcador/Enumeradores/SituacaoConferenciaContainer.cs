namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoConferenciaContainer
    {
        AguardandoContainer = 1,
        AguardandoConferencia = 2,
        ConferenciaAprovada = 3
    }

    public static class SituacaoConferenciaContainerHelper
    {
        public static string ObterCorLinha(this SituacaoConferenciaContainer situacao)
        {
            switch (situacao)
            {
                case SituacaoConferenciaContainer.AguardandoContainer: return "#ffffff";
                case SituacaoConferenciaContainer.AguardandoConferencia: return "#e29e23";
                case SituacaoConferenciaContainer.ConferenciaAprovada: return "##85de7b";
                default: return string.Empty;
            }
        }

        public static string ObterDescricao(this SituacaoConferenciaContainer situacao)
        {
            switch (situacao)
            {
                case SituacaoConferenciaContainer.AguardandoContainer: return "Aguardando Container";
                case SituacaoConferenciaContainer.AguardandoConferencia: return "Aguardando Conferência";
                case SituacaoConferenciaContainer.ConferenciaAprovada: return "Conferência Aprovada";
                default: return string.Empty;
            }
        }
    }
}
