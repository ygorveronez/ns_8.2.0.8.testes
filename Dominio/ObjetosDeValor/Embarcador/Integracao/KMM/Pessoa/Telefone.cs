using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KMM
{
    public class Telefone
    {
        [JsonProperty(PropertyName = "operation", Required = Required.Default)]
        public string Operation { get; set; }

        [JsonProperty(PropertyName = "id_externo", Required = Required.Default)]
        public string IdExterno { get; set; }

        [JsonProperty(PropertyName = "telefone_id", Required = Required.Default)]
        public string TelefoneId { get; set; }

        [JsonProperty(PropertyName = "telefone_padrao", Required = Required.Default)]
        public bool TelefonePadrao { get; set; }

        [JsonProperty(PropertyName = "tipo", Required = Required.Default)]
        public int Tipo { get; set; }

        [JsonProperty(PropertyName = "ddi", Required = Required.Default)]
        public string DDI { get; set; }

        [JsonProperty(PropertyName = "ddd", Required = Required.Default)]
        public string DDD { get; set; }

        [JsonProperty(PropertyName = "prefixo", Required = Required.Default)]
        public string Prefixo { get; set; }

        [JsonProperty(PropertyName = "numero", Required = Required.Default)]
        public string Numero { get; set; }

        [JsonProperty(PropertyName = "ramal", Required = Required.Default)]
        public string Ramal { get; set; }

        [JsonProperty(PropertyName = "contato", Required = Required.Default)]
        public string Contato { get; set; }
    }
}
