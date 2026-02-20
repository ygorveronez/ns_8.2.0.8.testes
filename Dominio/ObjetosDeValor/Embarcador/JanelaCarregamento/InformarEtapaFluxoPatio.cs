using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
namespace Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento
{
    public class InformarEtapaFluxoPatio
    {
        public int ProtocoloCarga { get; set; }
        public EtapaFluxoGestaoPatio EtapaFluxo { get; set; }
        public string DataEtapa { get; set; }
        public decimal Peso { get; set; }
        public string NumeroCarga { get; set; }
        public string CodigoFilial { get; set; }
        public string DataLacre { get; set; }
        public string Doca { get; set; }
        public string Observacao { get; set; }
    }
}
