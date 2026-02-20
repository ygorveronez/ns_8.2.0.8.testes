using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Calisto
{
    public class Response
    {
        [JsonProperty("message")]
        public string Menssagem { get; set; }

        [JsonProperty("sucesso")]
        public bool Sucesso { get; set; }
    }
}
