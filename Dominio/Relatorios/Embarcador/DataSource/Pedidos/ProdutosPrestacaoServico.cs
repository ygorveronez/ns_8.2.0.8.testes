namespace Dominio.Relatorios.Embarcador.DataSource.Pedidos
{
    public class ProdutosPrestacaoServico
    {
        public string Descricao { get; set; }
        public decimal Peso { get; set; }
        public decimal Palet { get; set; }
        public decimal Quantidade { get; set; }        
        public decimal Altura { get; set; }
        public decimal Largura { get; set; }
        public decimal Comprimento { get; set; }
        public decimal MetroCubico { get; set; }
        public string Observacao { get; set; }
        public int CodigoPedido { get; set; }
    }
}
