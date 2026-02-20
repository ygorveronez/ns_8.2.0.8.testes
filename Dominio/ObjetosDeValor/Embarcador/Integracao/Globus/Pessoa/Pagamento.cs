using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Globus
{

    public class Pagamento
    {
        [JsonProperty(PropertyName = "contaBancaria", Required = Required.Default)]
        public ContaBancaria ContaBancaria { get; set; }

        [JsonProperty(PropertyName = "pix", Required = Required.Default)]
        public Pix Pix { get; set; }
    }
}
