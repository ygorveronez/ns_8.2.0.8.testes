using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom
{
    public class RetornoValePedagioRecibo
    {
        [JsonProperty("nroTransporte")]
        public long NumeroTransporte { get; set; }

        [JsonProperty("status")]
        public Status Status { get; set; }

        [JsonProperty("dadosRetorno")]
        public DadosRetorno DadosRetorno { get; set; }
    }
}
