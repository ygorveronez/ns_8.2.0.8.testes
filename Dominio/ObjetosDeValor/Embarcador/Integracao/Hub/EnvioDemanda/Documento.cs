using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda
{
    public partial class Documento
    {
        [JsonProperty("documentNumber")]
        public string NumeroDocumento { get; set; }

        [JsonProperty("type")]
        public Tipo Tipo { get; set; }
    }
}
