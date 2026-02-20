using Newtonsoft.Json;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda
{
    public partial class Rota
    {
        [JsonProperty("sequence")]
        public long Sequencia { get; set; }

        [JsonProperty("type")]
        public long Tipo { get; set; }

        [JsonProperty("scheduledDate")]
        public DateTime? DataAgendada { get; set; }

        [JsonProperty("appointmentRequired")]
        public bool AgendamentoRequerido { get; set; }

        [JsonProperty("observation")]
        public string Observacao { get; set; }

        [JsonProperty("entity")]
        public Entidade Entidade { get; set; }

        [JsonProperty("address")]
        public Endereco Endereco { get; set; }

        [JsonProperty("city")]
        public Cidade Cidade { get; set; }

        [JsonProperty("provinceState")]
        public Estado Estado { get; set; }
    }

}
