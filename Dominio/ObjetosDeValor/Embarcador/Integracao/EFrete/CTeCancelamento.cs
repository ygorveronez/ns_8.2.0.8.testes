using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.EFrete
{
    public class CTeCancelamento
    {
        [JsonProperty("justitication")]
        public string Justificativa { get; set; }

        [JsonProperty("receivableIdentifier")]
        public string NumeroCTe { get; set; }
    }
}
