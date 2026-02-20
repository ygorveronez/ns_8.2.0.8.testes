using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class ViagemV3
    {
        public string externalId { get; set; }
        public List<Driver> drivers { get; set; }
        public Company? company { get; set; }
        public Company? riskManager { get; set; }
        public StoppingPoint origin { get; set; }
        public StoppingPoint destination { get; set; }
        public List<StoppingPoint>? stopovers { get; set; }
        public string? currency { get; set; }
        public string? language { get; set; }
        public Vehicle vehicle { get; set; }
        public InformacaoExterna? external { get; set; }
        public List<Alert> alerts { get; set; }
        public string? status { get; set; }
        public Route? route { get; set; }
        public List<InformacaoAdicional>? additionalInformation { get; set; }
        public Tracking tracking { get; set; }
        public bool? chatAvailable { get; set; }
        public ViagemV3Features features { get; set; }
    }
}