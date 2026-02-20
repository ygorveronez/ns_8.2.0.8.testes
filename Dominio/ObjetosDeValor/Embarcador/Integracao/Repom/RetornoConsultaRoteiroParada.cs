using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Repom
{
    public class RetornoConsultaRoteiroParada
    {
        [JsonProperty(PropertyName = "indice", Required = Required.AllowNull)]
        public int Indice { get; set; }

        [JsonProperty(PropertyName = "cidadeNome", Required = Required.AllowNull)]
        public string CidadeNome { get; set; }

        [JsonProperty(PropertyName = "estadoNome", Required = Required.AllowNull)]
        public string EstadoNome { get; set; }
    }
}
