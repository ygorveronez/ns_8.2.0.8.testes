namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoCarreta
    {
        Lisa = 1,
        Gancheira = 2
    }

    public static class TipoCarretaHelper
    {
        public static string ObterDescricao(this TipoCarreta tipoCarreta)
        {
            switch (tipoCarreta)
            {
                case TipoCarreta.Lisa: return "Lisa";
                case TipoCarreta.Gancheira: return "Gancheira";
                default: return string.Empty;
            }
        }
    }
}
