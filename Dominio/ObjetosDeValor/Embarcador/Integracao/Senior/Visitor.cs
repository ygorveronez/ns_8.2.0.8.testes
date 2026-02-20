using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Senior
{
    public class Visitor
    {
        [JsonProperty(PropertyName = "name", Required = Required.Default)]
        public string Nome { get; set; }

        [JsonProperty(PropertyName = "document", Required = Required.Default)]
        public Documento Documento { get; set; }

        [JsonProperty(PropertyName = "birthDate", Required = Required.Default)]
        public string DataNascimento { get; set; }

        [JsonProperty(PropertyName = "email", Required = Required.Default)]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "photo", Required = Required.Default)]
        public string FotoBase64 { get; set; } 

        [JsonProperty(PropertyName = "nationalityCode", Required = Required.Default)]
        public int CodigoNacionalidade { get; set; }

        [JsonProperty(PropertyName = "ddiCode", Required = Required.Default)]
        public string DDI { get; set; }

        [JsonProperty(PropertyName = "dddCode", Required = Required.Default)]
        public string DDD { get; set; }

        [JsonProperty(PropertyName = "phoneNumber", Required = Required.Default)]
        public string NumeroTelefone { get; set; }

        [JsonProperty(PropertyName = "note", Required = Required.Default)]
        public string Anotacao { get; set; }

        [JsonProperty(PropertyName = "company", Required = Required.Default)]
        public Empresa Empresa { get; set; }

        [JsonProperty(PropertyName = "vehicle", Required = Required.Default)]
        public Veiculo Veiculo { get; set; }
    }
}
