namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoTomadorCabotagem
    {
        Outros = 0,
        Destinatario = 1,
        Remetente = 2,
        Terceiro = 3,
    }

    public static class TipoTomadorCabotagemHelper
    {
        public static string ObterDescricao(this TipoTomadorCabotagem status)
        {
            switch (status)
            {
                case TipoTomadorCabotagem.Outros: return Localization.Resources.Enumeradores.TipoTomador.Outros;
                case TipoTomadorCabotagem.Destinatario: return Localization.Resources.Enumeradores.TipoTomador.Destinatario;
                case TipoTomadorCabotagem.Remetente: return Localization.Resources.Enumeradores.TipoTomador.Remetente;
                case TipoTomadorCabotagem.Terceiro: return Localization.Resources.Enumeradores.TipoTomador.Outros;
                default: return string.Empty;
            }
        }
    }
}
