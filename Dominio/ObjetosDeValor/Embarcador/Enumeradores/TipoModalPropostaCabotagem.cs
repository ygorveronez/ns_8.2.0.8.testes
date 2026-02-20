namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoModalPropostaCabotagem
    {
        Outros = 0,
        PortoPorto = 1,
    }

    public static class TipoModalPropostaCabotagemHelper
    {
        public static string ObterDescricao(this TipoModalPropostaCabotagem status)
        {
            switch (status)
            {
                case TipoModalPropostaCabotagem.Outros: return Localization.Resources.Enumeradores.ModalProposta.Outros;
                case TipoModalPropostaCabotagem.PortoPorto: return Localization.Resources.Enumeradores.ModalProposta.PortoPorto;
                default: return string.Empty;
            }
        }
    }
}
