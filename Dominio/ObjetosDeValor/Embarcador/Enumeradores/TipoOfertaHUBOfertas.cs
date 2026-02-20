namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoOfertaHUBOfertas
    {
        Exclusiva = 0,
        Adiciona = 1
    }

    public static class TipoOfertaHUBOfertasHelper
    {
        public static string ObterDescricao(this TipoOfertaHUBOfertas tipo)
        {
            switch (tipo)
            {
                case TipoOfertaHUBOfertas.Exclusiva: return "Exclusiva";
                case TipoOfertaHUBOfertas.Adiciona: return "Adiciona";
                default: return string.Empty;
            }
        }
    }
}
