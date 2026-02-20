using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.Rastreamento
{

    public class Rastreamento
    {
        [JsonProperty(PropertyName = "solicitante_id", Required = Required.Always)]
        public string ID { get; set; }

        [JsonProperty(PropertyName = "solicitante_token", Required = Required.Always)]
        public string Token { get; set; }

        [JsonProperty(PropertyName = "fornecedor_id", Required = Required.Always)]
        public int Fornecedor { get; set; }

        [JsonProperty(PropertyName = "rastreador_id", Required = Required.Always)]
        public int Rastreador { get; set; }

        [JsonProperty(PropertyName = "usuario", Required = Required.Always)]
        public string Usuario { get; set; }

        [JsonProperty(PropertyName = "senha", Required = Required.Always)]
        public string Senha { get; set; }

        [JsonProperty(PropertyName = "tagsLogin", Required = Required.AllowNull)]
        public TagsLogin TagsLogin { get; set; }

        [JsonProperty(PropertyName = "filtro", Required = Required.AllowNull)]
        public Filtro Filtro { get; set; }

    }

    public class TagsLogin
    {
        [JsonProperty(PropertyName = "dominio", Required = Required.Always)]
        public string Dominio { get; set; }

        [JsonProperty(PropertyName = "ipporta_dns", Required = Required.Always)]
        public string IppPorta { get; set; }
    }

    public class Filtro
    {
        [JsonProperty(PropertyName = "rast_ultima_pos", Required = Required.Always)]
        public string UltimaPosicao { get; set; }

        [JsonProperty(PropertyName = "rast_sequencia_inicial", Required = Required.Always)]
        public long Sequencial { get; set; }

        [JsonProperty(PropertyName = "terminal_associado", Required = Required.Always)]
        public string terminal_associado { get; set; }

    }
}
