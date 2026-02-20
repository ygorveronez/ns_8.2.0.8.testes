using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.SAP
{
    public class Motorista
    {
        [JsonProperty("Name")]
        public string Nome { get; set; }

        [JsonProperty("RG")]
        public string DocumentoIdentidade { get; set; }
    }
}
