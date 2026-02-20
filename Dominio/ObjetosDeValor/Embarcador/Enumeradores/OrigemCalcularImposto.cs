namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum OrigemCalcularImposto
    {
        ContratoFrete = 1,
        PagamentoMotoristaTMS = 2
    }

    public static class OrigemCalcularImpostoHelper
    {
        public static string ObterDescricao(this OrigemCalcularImposto origemCalcularImposto)
        {
            switch (origemCalcularImposto)
            {
                case OrigemCalcularImposto.ContratoFrete: return "Contrato de Frete";
                case OrigemCalcularImposto.PagamentoMotoristaTMS: return "Pagamento Motorista TMS";
                default: return string.Empty;
            }
        }
    }
}
