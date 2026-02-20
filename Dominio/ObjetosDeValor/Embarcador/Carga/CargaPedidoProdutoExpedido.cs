namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class CargaPedidoProdutoExpedido
    {
        public virtual string CodigoProduto { get; set; }
        public virtual string Produto { get; set; }
        public virtual decimal QuantidadeAExpedir { get; set; }
        public virtual decimal QuantidadeExpedida { get; set; }
        public virtual decimal QuantidadeTotal { get; set; }

    }
}
