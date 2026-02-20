using Newtonsoft.Json;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.YMS
{
    public class IntegrarCargaEventos
    {
        [JsonProperty("statusName")]
        public string CargaEventos { get; set; }

        [JsonProperty("dateTime")]
        public DateTime? DataEvento { get; set; }

        [JsonProperty("local")]
        public string Local { get; set; }
    }
}
