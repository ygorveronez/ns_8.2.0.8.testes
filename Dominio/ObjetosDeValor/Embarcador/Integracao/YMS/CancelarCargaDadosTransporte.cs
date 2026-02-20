using Newtonsoft.Json;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.YMS
{
    public class CancelarCargaDadosTransporte
    {
        public int NumeroCarga { get; set; }
        public string Motivo { get; set; }
        [JsonProperty("dateTime")]
        public DateTime? DataCancelamento { get; set; }
        public string? HashCarga { get; set; }
    }
}
