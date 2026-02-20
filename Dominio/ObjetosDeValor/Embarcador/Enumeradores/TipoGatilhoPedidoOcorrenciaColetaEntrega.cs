namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoGatilhoPedidoOcorrenciaColetaEntrega
    {
        CriacaoPedido = 1,
        FinalizacaoEmissaoCarga = 2
    }

    public static class TipoGatilhoPedidoOcorrenciaColetaEntregaHelper
    {
        public static string ObterDescricao(this TipoGatilhoPedidoOcorrenciaColetaEntrega tipo)
        {
            switch (tipo)
            {
                case TipoGatilhoPedidoOcorrenciaColetaEntrega.CriacaoPedido: return "Criação do Pedido";
                case TipoGatilhoPedidoOcorrenciaColetaEntrega.FinalizacaoEmissaoCarga: return "Finalização da Emissão da Carga";
                default: return string.Empty;
            }
        }
    }
}
