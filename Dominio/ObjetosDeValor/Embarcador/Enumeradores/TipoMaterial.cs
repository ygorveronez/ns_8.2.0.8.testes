namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoMaterial
    {
        Aco = 1,
        Aluminio = 2,
        Madeira = 3
    }

    public static class TipoMaterialHelper
    {
        public static string ObterDescricao(this TipoMaterial tipoMaterial)
        {
            switch (tipoMaterial)
            {
                case TipoMaterial.Aco: return "Aço";
                case TipoMaterial.Aluminio: return "Alumínio";
                case TipoMaterial.Madeira: return "Madeira";
                default: return string.Empty;
            }
        }
    }
}
