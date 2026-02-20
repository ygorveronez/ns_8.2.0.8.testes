using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.GrupoMotorista
{
    public class GrupoMotoristaCreateRequest
    {
        public string name { get; set; }
        public string description { get; set; }
        public Driver[] drivers { get; set; }
    }

    public class Driver
    {
        public Document document { get; set; }
        public string fullName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Phone[] phones { get; set; }
        public string workMode { get; set; }
    }

    public class Document
    {
        public string type { get; set; }
        public string value { get; set; }
    }

    public class Phone
    {
        public string type { get; set; }
        public string value { get; set; }
    }

}
