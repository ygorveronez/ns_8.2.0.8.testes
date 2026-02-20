using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Mars
{
    public class Participante
    {
        [JsonProperty("name")]
        public string Nome { get; set; }

        [JsonProperty("address")]
        public string Endereco { get; set; }

        [JsonProperty("cnpj")]
        public string Cnpj { get; set; }

        [JsonProperty("stateRegistration")]
        public string InscricaoEstadual { get; set; }

        [JsonProperty("phone")]
        public string Telefone { get; set; }
    }
}
