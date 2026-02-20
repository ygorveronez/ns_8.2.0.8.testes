namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoEtapaFluxoSinistro
    {
        Aberto = 1,
        Finalizado = 2,
        Cancelado = 3
    }

    public static class SituacaoEtapaFluxoSinistroHelper
    {
        public static string ObterDescricao(this SituacaoEtapaFluxoSinistro situacao)
        {
            switch (situacao)
            {
                case SituacaoEtapaFluxoSinistro.Aberto: return "Aberto";
                case SituacaoEtapaFluxoSinistro.Finalizado: return "Finalizado";
                case SituacaoEtapaFluxoSinistro.Cancelado: return "Cancelado";
                default: return string.Empty;
            }
        }
    }
}
