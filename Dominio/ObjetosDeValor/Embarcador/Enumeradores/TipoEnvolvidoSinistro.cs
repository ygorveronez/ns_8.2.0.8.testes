namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoEnvolvidoSinistro
    {
        Todos = 0,
        Proprio = 1,
        Terceiro = 2
    }
    public static class TipoEnvolvidoSinistroHelper
    {
        public static string ObterDescricao(this TipoEnvolvidoSinistro tipoEnvolvido)
        {
            switch (tipoEnvolvido)
            {
                case TipoEnvolvidoSinistro.Todos: return "Todos";
                case TipoEnvolvidoSinistro.Proprio: return "Próprio";
                case TipoEnvolvidoSinistro.Terceiro: return "Terceiro";
                default: return string.Empty;
            }
        }
    }
}
