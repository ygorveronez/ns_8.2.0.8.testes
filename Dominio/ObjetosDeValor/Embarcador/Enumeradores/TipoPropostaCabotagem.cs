namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoPropostaCabotagem
    {
        Outros = 0,
        Cabotagem = 1,
        Feeder = 2,
    }

    public static class TipoPropostaCabotagemHelper
    {
        public static string ObterDescricao(this TipoPropostaCabotagem status)
        {
            switch (status)
            {
                case TipoPropostaCabotagem.Outros: return Localization.Resources.Enumeradores.TipoProposta.Outros;
                case TipoPropostaCabotagem.Cabotagem: return Localization.Resources.Enumeradores.TipoProposta.Cabotagem;
                case TipoPropostaCabotagem.Feeder: return Localization.Resources.Enumeradores.TipoProposta.Feeder;
                default: return string.Empty;
            }
        }
    }
}
