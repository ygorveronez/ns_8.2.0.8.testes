using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.NovoApp.Autenticacao
{
    public partial class ResponseLogin
    {
        [JsonProperty("Codigo")]
        public long Codigo { get; set; }

        [JsonProperty("Sessao")]
        public string Token { get; set; }

        [JsonProperty("Nome")]
        public string Nome { get; set; }

        [JsonProperty("Empresas")]
        public List<Empresa> Empresas { get; set; }
    }

    public partial class Empresa
    {
        [JsonProperty("Codigo")]
        public long Codigo { get; set; }

        [JsonProperty("Descricao")]
        public string Descricao { get; set; }

        [JsonProperty("UrlEmbarcador")]
        public string UrlEmbarcador { get; set; }

        [JsonProperty("UrlMobile")]
        public string UrlMobile { get; set; }
    }
}
