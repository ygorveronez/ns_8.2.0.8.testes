namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum EtapaAjusteTabelaFrete
    {
        Todas = 9,
        Criacao = 0,
        AgAprovacao = 1,
        AgIntegracao = 2,
        Finalizada = 3
    }

    public static class EtapaAjusteTabelaFreteHelper
    {
        public static string ObterDescricao(this EtapaAjusteTabelaFrete etapa)
        {
            switch (etapa)
            {
                case EtapaAjusteTabelaFrete.AgAprovacao: return "Ag. Aprovação";
                case EtapaAjusteTabelaFrete.AgIntegracao: return "Ag. Integração";
                case EtapaAjusteTabelaFrete.Criacao: return "Criação";
                case EtapaAjusteTabelaFrete.Finalizada: return "Finalizada";
                default: return string.Empty;
            }
        }
    }
}
