namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoIntegracaoUnilever
    {
        OTM = 1,
        None = 2,
        OtmSap = 3,
    }

    public static class TipoIntegracaoUnileverHelper
    {
        public static string ObterDescricao(this TipoIntegracaoUnilever tipoIntegracaoUnilever)
        {
            switch (tipoIntegracaoUnilever)
            {
                case TipoIntegracaoUnilever.OTM: return "OTM";
                case TipoIntegracaoUnilever.None: return "None";
                case TipoIntegracaoUnilever.OtmSap: return "OTM+SAP";
                default: return string.Empty;
            }
        }
    }
}
