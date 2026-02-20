using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Unitop
{
    public class Veiculo
    {
        [JsonProperty("Placa")]
        public string Placa { get; set; }

        [JsonProperty("Data_movimento")]
        public string DataMovimento { get; set; }

        [JsonProperty("Hora")]
        public string Hora { get; set; }

        [JsonProperty("Ignicao")]
        public string Ignicao { get; set; }

        [JsonProperty("Latitude")]
        public double? Latitude { get; set; }

        [JsonProperty("Longitude")]
        public double? Longitude { get; set; }

        [JsonProperty("Velocidade")]
        public int Velocidade { get; set; }

        [JsonProperty("Cnpj")]
        public string Cnpj { get; set; }
    }
}
