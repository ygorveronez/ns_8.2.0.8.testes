namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoLicitacaoParticipacao
    {
        AguardandoOferta = 1,
        AguardandoRetornoOferta = 2,
        Cancelada = 3,
        OfertaAceita = 4,
        OfertaRecusada = 5
    }

    public static class SituacaoLicitacaoParticipacaoHelper
    {
        public static string ObterClasseCor(this SituacaoLicitacaoParticipacao situacao)
        {
            switch (situacao)
            {
                case SituacaoLicitacaoParticipacao.AguardandoRetornoOferta: return "alert-warning";
                case SituacaoLicitacaoParticipacao.OfertaAceita: return "alert-success";
                case SituacaoLicitacaoParticipacao.OfertaRecusada: return "alert-danger";
                default: return "alert-info";
            }
        }

        public static string ObterDescricao(this SituacaoLicitacaoParticipacao situacao)
        {
            switch (situacao)
            {
                case SituacaoLicitacaoParticipacao.AguardandoOferta: return "Aguardando Oferta";
                case SituacaoLicitacaoParticipacao.AguardandoRetornoOferta: return "Aguardando Retorno da Oferta";
                case SituacaoLicitacaoParticipacao.Cancelada: return "Cancelada";
                case SituacaoLicitacaoParticipacao.OfertaAceita: return "Oferta Aceita";
                case SituacaoLicitacaoParticipacao.OfertaRecusada: return "Oferta Recusada";
                default: return string.Empty;
            }
        }
    }
}
