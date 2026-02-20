using Newtonsoft.Json;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy
{
    public class PedidoRestricaoAgendamento
    {
        [JsonProperty(PropertyName = "start_date")]
        public DateTime? DataInicial { get; set; }

        [JsonProperty(PropertyName = "end_date")]
        public DateTime? DataLimite { get; set;}
    }
}
