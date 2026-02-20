namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoDocumentoModeloImpressao
    {
        NotaDeCreditoDebito = 0,
        NotaDePagamento = 1
    }

    public static class TipoDocumentoModeloImpressaoHelper
    {
        public static string ObterDescricao(this TipoDocumentoModeloImpressao tipo)
        {
            switch (tipo)
            {
                case TipoDocumentoModeloImpressao.NotaDeCreditoDebito: return "Nota de Crédito/Débito";
                case TipoDocumentoModeloImpressao.NotaDePagamento: return "Nota de Pagamento";
                default: return string.Empty;
            }
        }
    }
}
