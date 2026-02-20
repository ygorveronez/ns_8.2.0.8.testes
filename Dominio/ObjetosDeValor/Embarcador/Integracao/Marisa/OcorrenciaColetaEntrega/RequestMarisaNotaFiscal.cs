using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marisa
{
    public class RequestMarisaNotaFiscal
    {
        [JsonProperty("invoice_number")]
        public string NumeroNotaFiscal;

        [JsonProperty("invoice_key")]
        public string ChaveAcessoNotaFiscal;

        [JsonProperty("invoice_series")]
        public string SerieNotaFiscal;
    }
}