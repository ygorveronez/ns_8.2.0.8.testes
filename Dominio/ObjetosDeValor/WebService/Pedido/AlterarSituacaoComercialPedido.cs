namespace Dominio.ObjetosDeValor.WebService.Pedido
{
    public sealed class AlterarSituacaoComercialPedido
    {
        public int ProtocoloPedido { get; set; }
        public Embarcador.Pedido.SituacaoComercial SituacaoComercial { get; set; }
        public Embarcador.Pedido.SituacaoComercial SituacaoEstoque { get; set; }
    }
}
