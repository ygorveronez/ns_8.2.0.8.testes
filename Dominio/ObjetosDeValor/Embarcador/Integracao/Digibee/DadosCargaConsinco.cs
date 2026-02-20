namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee
{
    public class DadosCargaConsinco
    {
        public int numeroLacre { get; set; }
        public int protocoloIntegracaoCarga { get; set; }
        public long cpfCnpjTransportador { get; set; }
        public long digCpfCnpjTransportador { get; set; }
        public long cpfCnpjMotorista { get; set; }
        public long digCpfCnpjMotorista { get; set; }
        public int codigoOrigem { get; set; }
        public string placaVeiculo { get; set; }
    }
}
