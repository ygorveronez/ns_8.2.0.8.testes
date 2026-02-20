namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoDocumentoCreditoDebito
    {
        Credito = 0,
        Debito = 1,
        Todos = 99
    }

    public static class TipoDocumentoCreditoDebitoHelper
    {
        public static string ObterDescricao(this TipoDocumentoCreditoDebito tipo)
        {
            switch (tipo)
            {
                case TipoDocumentoCreditoDebito.Todos: return "Todos";
                case TipoDocumentoCreditoDebito.Debito: return "Débito";
                case TipoDocumentoCreditoDebito.Credito: return "Crédito";
                default: return string.Empty;
            }
        }
    }
}
