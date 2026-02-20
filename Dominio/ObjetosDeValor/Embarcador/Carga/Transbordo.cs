namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class Transbordo
    {
        public int CodigoIntegracao { get; set; }
        public Porto Porto { get; set; }
        public TerminalPorto Terminal { get; set; }
        public Navio Navio { get; set; }
        public int Sequencia { get; set; }
        public Viagem Viagem { get; set; }
    }

    public class VinculoEntreCargaEntregaComCargaEntregaTransbordada
    {
        public int CargaOrigem { get; set; }
        public int CargaTransbordo { get; set; }
        public double CodigoCliente { get; set; }
        public int CodigoCargaEntregaOrigem { get; set; }
    }
}
