namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class OrdemCompraMercadoria
    {
        public virtual int Codigo { get; set; }
        public virtual OrdemCompra OrdemCompra { get; set; }
        public virtual NotaFiscal.Produto Produto { get; set; }
        public virtual decimal Quantidade { get; set; }
        public virtual decimal QuantidadePendente { get; set; }
        public virtual decimal ValorUnitario { get; set; }
        public virtual Veiculo VeiculoMercadoria { get; set; }
    }
}
