using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.SequenciaZonaTransporte
{
    public class Coleta
    {
        [JsonProperty(PropertyName = "coletaData", Order = 1, Required = Required.Default)]
        public string Data { get; set; }

        [JsonProperty(PropertyName = "coletaHora", Order = 2, Required = Required.Default)]
        public string Hora { get; set; }
    }
}
