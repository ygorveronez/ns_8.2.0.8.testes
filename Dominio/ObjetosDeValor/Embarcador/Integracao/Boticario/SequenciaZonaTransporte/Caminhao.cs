using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.SequenciaZonaTransporte
{
    public class Caminhao
    {
        [JsonProperty(PropertyName = "placa", Order = 1, Required = Required.Default)]
        public string Placa { get; set; }

        [JsonProperty(PropertyName = "lacre", Order = 2, Required = Required.Default)]
        public string Lacre { get; set; }
    }
}
