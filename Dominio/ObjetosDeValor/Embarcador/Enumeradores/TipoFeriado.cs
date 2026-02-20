namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoFeriado
    {
        Nacional = 0,
        Estadual = 1,
        Municipal = 2
    }

    public static class TipoFeriadoHelper
    {
        public static string ObterDescricao(this TipoFeriado tipo)
        {
            switch (tipo)
            {
                case TipoFeriado.Estadual: return "Estadual";
                case TipoFeriado.Municipal: return "Municipal";
                case TipoFeriado.Nacional: return "Nacional";
                default: return string.Empty;
            }
        }
    }
}
