namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class RateioPonderacaoDistanciaPeso
    {
        public Dominio.Entidades.Cliente Cliente { get; set; }
        public Dominio.Entidades.Embarcador.Cargas.CargaPedido CargaPedido { get; set; }
        public decimal PesoEntrega { get; set; }
        public int Distancia { get; set; }
        public decimal Ponderacao { get; set; }
        public decimal PercentualAcrescimoPesoCarga { get; set; }
        public decimal PesoTotalCarga { get; set; }
        public decimal SomaTotalPonderacao { get; set; }
        public decimal PercentualRepresentatividade { get; set; }
    }
}
