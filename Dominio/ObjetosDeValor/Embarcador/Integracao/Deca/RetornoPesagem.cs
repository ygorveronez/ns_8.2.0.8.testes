using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Deca
{
    public class RetornoPesagem
    {
        [JsonProperty(PropertyName = "weight", Required = Required.Default)]
        public string Peso { get; set; }
    }
}
