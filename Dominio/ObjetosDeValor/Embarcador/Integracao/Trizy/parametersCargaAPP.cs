using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class parametersCargaAPP
    {
        public string externalId { get; set; }
        public Carga external { get; set; }
        public List<Motorista> drivers { get; set; }
        public Filial company { get; set; }
        public OrigemDaCarga origin { get; set; }
        public List<PontoParada> stopovers { get; set; }
        public PontoParada destination { get; set; }
        public string currency { get; set; }
        public string language { get; set; }
        public Veiculo vehicle { get; set; }
        public string observation { get; set; }
        public Ocorrencia occurrence { get; set; }
        public Tracking tracking { get; set; }
        public bool chatAvailable { get; set; }
        public List<InformacaoAdicional> additionalInformation { get; set; }
    }
}
