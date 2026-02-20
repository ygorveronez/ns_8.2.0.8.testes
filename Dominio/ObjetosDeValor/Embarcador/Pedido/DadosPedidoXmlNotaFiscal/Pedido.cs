namespace Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal
{
    public sealed class Pedido
    {
        public int Codigo { get; set; }

        public string NumeroOrdem { get; set; }

        public string NumeroPedidoEmbarcador { get; set; }

        public Cliente Remetente { get; set; }

        public Cliente Destinatario { get; set; }
    }
}
