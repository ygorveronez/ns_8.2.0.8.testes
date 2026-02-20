namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoTermo
    {
        Nenhum = 0,
        Unilateral = 1,
        Bilateral = 2,
    }

    public static class TipoTermoHelper
    {
        public static string ObterDescricao(this TipoTermo tipoTermo)
        {
            switch (tipoTermo)
            {
                case TipoTermo.Unilateral: return Localization.Resources.Transportadores.Transportador.Unilateral;
                case TipoTermo.Bilateral: return Localization.Resources.Transportadores.Transportador.Bilateral;
                default: return string.Empty;
            }
        }
    }
}
