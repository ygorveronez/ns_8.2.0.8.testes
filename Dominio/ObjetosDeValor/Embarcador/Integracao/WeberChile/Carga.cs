using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.WeberChile
{
    public class Carga
    {
        [JsonProperty("viaje")]
        public string NumeroCarga { get; set; }

        [JsonProperty("hr1")]
        public string NumeroPedidoEmbarcador { get; set; }

        [JsonProperty("importe")]
        public decimal ValorTotal { get; set; }

        [JsonProperty("incidencias")]
        public List<Dominio.ObjetosDeValor.Embarcador.Integracao.WeberChile.Ocorrencia> Ocorrencias { get; set; }
    }
}
