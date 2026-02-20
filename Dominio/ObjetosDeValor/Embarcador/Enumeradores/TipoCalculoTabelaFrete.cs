namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoCalculoTabelaFrete
    {
        PorCarga = 0,
        PorPedido = 1,
        PorDocumentoEmitido = 2,
        PorMaiorValorPedido = 3,
        PorPedidosAgrupados = 4,
        PorMaiorValorPedidoAgrupados = 5,
        PorMaiorDistanciaPedidoAgrupados = 6
    }
}
