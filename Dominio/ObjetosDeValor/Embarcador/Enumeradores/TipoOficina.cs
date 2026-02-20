namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoOficina
    {
        Interna = 1,
        Externa = 2
    }

    public static class TipoOficinaHelper
    {
        public static string ObterDescricao(this TipoOficina tipo)
        {
            switch (tipo)
            {
                case TipoOficina.Interna: return "Interna";
                case TipoOficina.Externa: return "Externa";
                default: return string.Empty;
            }
        }
    }
}
