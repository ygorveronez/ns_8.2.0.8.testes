namespace Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega
{
    public class ProdutoDivisaoCapacidade
    {
        public decimal Quantidade { get; set; }

        public decimal QuantidadePlanejada { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Carga.DivisaoCapacidadeModeloVeicular DivisaoCapacidadeModeloVeicular { get; set; }
    }
}
