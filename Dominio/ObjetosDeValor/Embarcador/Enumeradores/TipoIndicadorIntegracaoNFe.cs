namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoIndicadorIntegracaoNFe
    {
        Automatico = 1,
        PorEmail = 2
    }

    public static class TipoIndicadorIntegracaoNFeHelper
    {
        public static string ObterDescricao(this TipoIndicadorIntegracaoNFe tipo)
        {
            switch (tipo)
            {
                case TipoIndicadorIntegracaoNFe.Automatico: return "Automático";
                case TipoIndicadorIntegracaoNFe.PorEmail: return "Por e-mail";
                default: return string.Empty;
            }
        }
    }
}
