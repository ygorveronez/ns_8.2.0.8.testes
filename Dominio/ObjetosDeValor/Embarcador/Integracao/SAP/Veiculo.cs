using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.SAP
{
    public class Veiculo
    {
        [JsonProperty("Type")]
        public string Tipo { get; set; }

        [JsonProperty("Plate")]
        public string Placa { get; set; }
    }
}
