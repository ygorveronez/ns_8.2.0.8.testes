namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoGeracaoTermo
    {
        Nenhum = 0,
        Automatico = 1,
        Manual = 2,
    }

    public static class TipoGeracaoTermoHelper
    {
        public static string ObterDescricao(this TipoGeracaoTermo tipoGeracaoTermo)
        {
            switch (tipoGeracaoTermo)
            {
                case TipoGeracaoTermo.Automatico: return Localization.Resources.Transportadores.Transportador.Automatico;
                case TipoGeracaoTermo.Manual: return Localization.Resources.Transportadores.Transportador.Manual;
                default: return string.Empty;
            }
        }
    }
}
