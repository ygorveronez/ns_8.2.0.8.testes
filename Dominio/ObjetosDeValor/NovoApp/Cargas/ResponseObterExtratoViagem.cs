using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.NovoApp.Cargas
{
    public partial class ResponseObterExtratoViagem
    {
        [JsonProperty("PreviaDiaria")]
        public decimal PrevisaoDiaria { get; set; }

        [JsonProperty("SaldoReceita")]
        public decimal SaldoReceita { get; set; }

        [JsonProperty("SaldoDespesa")]
        public decimal SaldoDespesa { get; set; }

        [JsonProperty("MotivosDespesa")]
        public List<MotivoDespesa> MotivosDespesa { get; set; }

        [JsonProperty("StatusRetorno")]
        public bool StatusRetorno { get; set; }

        [JsonProperty("MensagemRetorno")]
        public string MensagemRetorno { get; set; }
    }

    public partial class MotivoDespesa
    {
        [JsonProperty("Codigo")]
        public long Codigo { get; set; }

        [JsonProperty("Descricao")]
        public string Descricao { get; set; }
    }
}
