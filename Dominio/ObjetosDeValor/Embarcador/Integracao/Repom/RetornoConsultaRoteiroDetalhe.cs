using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Repom
{
    public class RetornoConsultaRoteiroDetalhe
    {
        [JsonProperty(PropertyName = "percursoCodigo", Required = Required.AllowNull)]
        public int CodigoPercurso { get; set; }

        [JsonProperty(PropertyName = "roteiroCodigo", Required = Required.AllowNull)]
        public int CodigoRoteiro { get; set; }

        [JsonProperty(PropertyName = "cidadeOrigem", Required = Required.AllowNull)]
        public string CidadeOrigem { get; set; }

        [JsonProperty(PropertyName = "estadoOrigem", Required = Required.AllowNull)]
        public string EstadoOrigem { get; set; }

        [JsonProperty(PropertyName = "cidadeDestino", Required = Required.AllowNull)]
        public string CidadeDestino { get; set; }

        [JsonProperty(PropertyName = "estadoDestino", Required = Required.AllowNull)]
        public string EstadoDestino { get; set; }

        [JsonProperty(PropertyName = "percursoCodigoCliente", Required = Required.AllowNull)]
        public string CodigoPercursoCliente { get; set; }

        [JsonProperty(PropertyName = "sentido", Required = Required.AllowNull)]
        public int Sentido { get; set; }

        [JsonProperty(PropertyName = "percursoDescricao", Required = Required.AllowNull)]
        public string DescricaoPercurso { get; set; }

        [JsonProperty(PropertyName = "percursoTipoDescricao", Required = Required.AllowNull)]
        public string TipoDescricaoPercurso { get; set; }
    }
}
