using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Senior
{
    public class Veiculo
    {
        [JsonProperty(PropertyName = "licensePlate", Required = Required.Default)]
        public string Placa { get; set; }

        [JsonProperty(PropertyName = "authorizedEntry", Required = Required.Default)]
        public string EntradaAutorizada { get; set; }
    }
}
