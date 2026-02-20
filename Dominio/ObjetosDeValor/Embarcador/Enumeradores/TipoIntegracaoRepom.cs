namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoIntegracaoRepom
    {
        SOAP = 0,
        REsT = 1
    }

    public static class TipoIntegracaoRepomHelper
    {
        public static string ObterDescricao(this TipoIntegracaoRepom tipo)
        {
            switch (tipo)
            {
                case TipoIntegracaoRepom.SOAP: return "SOAP";
                case TipoIntegracaoRepom.REsT: return "REsT";
                default: return string.Empty;
            }
        }
    }
}
