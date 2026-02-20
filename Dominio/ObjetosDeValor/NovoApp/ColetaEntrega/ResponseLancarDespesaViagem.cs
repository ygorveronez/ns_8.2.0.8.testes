using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.NovoApp.ColetaEntrega
{
    public partial class ResponseLancarDespesaViagem
    {
        [JsonProperty("CodigoOcorrencia")]
        public int? CodigoOcorrencia { get; set; }
        public int? NumeroOcorrencia { get; set; }
        public int? CodigoDespesaViagem { get; set; }
    }
}
