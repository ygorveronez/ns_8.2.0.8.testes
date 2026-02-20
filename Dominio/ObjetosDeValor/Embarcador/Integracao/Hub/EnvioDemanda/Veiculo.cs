using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda
{
    public partial class Veiculo
    {
        [JsonProperty("trailer")]
        public Veiculo Reboque { get; set; }

        [JsonProperty("VehicleLand")]
        public VeiculoTerrestre VeiculoTerrestre { get; set; }

        [JsonProperty("crew")]
        public List<Tripulacao> Tripulacao { get; set; }
    }

}
