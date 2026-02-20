namespace Dominio.ObjetosDeValor.WebService.Pedido
{
    public sealed class RetornoAlteracaoPedido
    {
        public string MotivoRejeicao { get; set; }

        public int ProtocoloIntegracaoPedido { get; set; }

        public Embarcador.Enumeradores.SituacaoAlteracaoPedido Situacao { get; set; }
    }
}
