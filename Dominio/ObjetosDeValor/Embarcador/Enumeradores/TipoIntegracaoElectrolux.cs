namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoIntegracaoElectrolux
    {
        NotfisPendente = 1,
        NotfisDetalhe = 2
    }
    public static class TipoIntegracaoElectroluxHelper
    {
        public static string ObterDescricao(this TipoIntegracaoElectrolux tipo)
        {
            switch (tipo)
            {
                case TipoIntegracaoElectrolux.NotfisPendente: return "Pendentes";
                case TipoIntegracaoElectrolux.NotfisDetalhe: return "Detalhada";
                default: return string.Empty;
            }
        }

    }
}
