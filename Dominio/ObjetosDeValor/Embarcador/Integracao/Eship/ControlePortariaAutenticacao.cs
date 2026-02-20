using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Eship
{
    public class ControlePortariaAutenticacao
    {
        [JsonProperty(PropertyName = "apikey", Required = Required.Default)]
        public string ApiKey { get; set; }
    }
}
