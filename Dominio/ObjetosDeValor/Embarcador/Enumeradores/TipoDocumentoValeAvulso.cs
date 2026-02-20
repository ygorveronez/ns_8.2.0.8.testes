namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoDocumentoValeAvulso
    {
        Todos = 0,
        ValeAvulso = 1,
        Recibo = 2
    }

    public static class TipoDocumentoValeAvulsoHelper
    {
        public static string ObterDescricao(this TipoDocumentoValeAvulso tipoDocumento)
        {
            switch (tipoDocumento)
            {
                case TipoDocumentoValeAvulso.Todos: return "Todos";
                case TipoDocumentoValeAvulso.ValeAvulso: return "Vale";
                case TipoDocumentoValeAvulso.Recibo: return "Recibo";
                default: return string.Empty;
            }
        }
    }
}

