namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public class RetornoImportacaoPedidoGerarCarga
    {
        public bool Sucesso { get; set; }
        public int TotalPedidos { get; set; }
        public int TotalCargas { get; set; }
        public string Mensagem { get; set; }
    }
}
