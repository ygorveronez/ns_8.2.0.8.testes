namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoModalMDFe
    {
        Todos = 0,
        Rodoviario = 1,        
        Aquaviario = 2
    }

    public static class TipoModalMDFeHelper
    {
        public static string ObterDescricao(this TipoModalMDFe tipo)
        {
            switch (tipo)
            {
                case TipoModalMDFe.Rodoviario: return "Rodoviário";                
                case TipoModalMDFe.Aquaviario: return "Aquaviário";
                default: return string.Empty;
            }
        }
    }
}
