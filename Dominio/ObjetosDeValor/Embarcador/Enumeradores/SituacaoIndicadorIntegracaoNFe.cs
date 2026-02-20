namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoIndicadorIntegracaoNFe
    {
        Sucesso = 1,
        Rejeitada = 2
    }

    public static class SituacaoIndicadorIntegracaoNFeHelper
    {
        public static string ObterDescricao(this SituacaoIndicadorIntegracaoNFe situacao)
        {
            switch (situacao)
            {
                case SituacaoIndicadorIntegracaoNFe.Rejeitada: return "Rejeitada";
                case SituacaoIndicadorIntegracaoNFe.Sucesso: return "Sucesso";
                default: return string.Empty;
            }
        }
    }
}
