namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoIntegracaoMercadoLivre
    {
        HandlingUnit = 0,
        Dispatch = 1,
        RotaEFacility = 2
    }

    public static class TipoIntegracaoMercadoLivreHelper
    {
        public static string ObterDescricao(this TipoIntegracaoMercadoLivre tipoIntegracaoMercadoLivre)
        {
            switch (tipoIntegracaoMercadoLivre)
            {
                case TipoIntegracaoMercadoLivre.HandlingUnit: return "Handling Unit";
                case TipoIntegracaoMercadoLivre.Dispatch: return "Dispatch";
                case TipoIntegracaoMercadoLivre.RotaEFacility: return "Rota e Facility";
                default: return "";
            }
        }
    }
}
