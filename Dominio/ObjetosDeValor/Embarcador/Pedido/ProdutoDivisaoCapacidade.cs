namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public sealed class ProdutoDivisaoCapacidade
    {
        public string Descricao { get; set; }
        public int? Piso { get; set; }
        public int? Coluna { get; set; }
        public decimal Capacidade { get; set; }
        public decimal Quantidade { get; set; }
        public decimal QuantidadePlanejada { get; set; }
        public Carga.UnidadeDeMedida UnidadeDeMedida { get; set; }
    }
}
