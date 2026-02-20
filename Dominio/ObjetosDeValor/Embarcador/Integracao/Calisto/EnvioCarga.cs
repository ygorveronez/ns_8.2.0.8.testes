using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Calisto
{
    public class EnvioCarga
    {
        [JsonProperty("codigoFilial")]
        public int CodigoFilial { get; set; }

        [JsonProperty("codigoTransportadora")]
        public int CodigoTransportador { get; set; }

        [JsonProperty("numeroCarga")]
        public string NumeroCarga { get; set; }

        [JsonProperty("listaPedidos")]
        public List<ListaPedidos> ListaPedidos { get; set; }
    }
}
