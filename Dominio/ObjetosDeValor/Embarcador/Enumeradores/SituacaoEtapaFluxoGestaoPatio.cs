namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoEtapaFluxoGestaoPatio
    {
        Aguardando = 1,
        Aprovado = 2,
        Rejeitado = 3,
        Cancelado = 4
    }

    public static class SituacaoEtapaFluxoGestaoPatioHelper
    {
        public static string ObterDescricao(this SituacaoEtapaFluxoGestaoPatio situacao)
        {
            switch (situacao)
            {
                case SituacaoEtapaFluxoGestaoPatio.Aguardando: return "Aguardando";
                case SituacaoEtapaFluxoGestaoPatio.Aprovado: return "Aprovada";
                case SituacaoEtapaFluxoGestaoPatio.Rejeitado: return "Rejeitada";
                case SituacaoEtapaFluxoGestaoPatio.Cancelado: return "Cancelada";
                default: return "Indefinida";
            }
        }
    }
}
