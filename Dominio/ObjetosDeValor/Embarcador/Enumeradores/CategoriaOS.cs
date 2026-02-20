namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum CategoriaOS
    {
        Trucking = 0,
        Hibrida = 1,
        Negocio = 2,
    }

    public static class EnumCategoriaOSeHelper
    {
        public static string ObterDescricao(this CategoriaOS situacao)
        {
            switch (situacao)
            {
                case CategoriaOS.Trucking: return "Trucking";
                case CategoriaOS.Hibrida: return "Híbrida";
                case CategoriaOS.Negocio: return "Negócio";
                default: return string.Empty;
            }
        }
    }
}
