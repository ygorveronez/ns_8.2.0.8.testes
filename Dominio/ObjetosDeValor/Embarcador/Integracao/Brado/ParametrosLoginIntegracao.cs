using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Brado
{
    public class ParametrosLoginIntegracao
    {
        [JsonProperty("username")]
        public string Usuario { get; set; }

        [JsonProperty("password")]
        public string Senha { get; set; }

        [JsonProperty("cod_gestao")]
        public int CodigoGestao { get; set; }
    }
}
