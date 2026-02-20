using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Repom
{
    public class RetornoCadastroRoteiro
    {
        [JsonProperty(PropertyName = "status", Required = Required.AllowNull)]
        public int Status { get; set; }

        [JsonProperty(PropertyName = "codigoRoteiroRepom", Required = Required.AllowNull)]
        public string CodigoRoteiroRepom { get; set; }

        [JsonProperty(PropertyName = "codigoPercurso", Required = Required.AllowNull)]
        public string CodigoPercurso { get; set; }

        [JsonProperty(PropertyName = "codigoRoteiroExterno", Required = Required.AllowNull)]
        public string CodigoRoteiroExterno { get; set; }

        [JsonProperty(PropertyName = "descricaoErro", Required = Required.AllowNull)]
        public string DescricaoErro { get; set; }
    }
}
