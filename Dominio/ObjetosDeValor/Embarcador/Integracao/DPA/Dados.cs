using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.DPA
{
    public class Dados
    {
        [JsonProperty("codEmpresa")]
        public string CodigoEmpresa { get; set; }

        [JsonProperty("cpfTac")]
        public string CPFCNPJEmpresa { get; set; }

        [JsonProperty("cnpjContratante")]
        public string CNPJContratante { get; set; }

        [JsonProperty("ciotNr")]
        public string NumeroCIOT { get; set; }

        [JsonProperty("ciotData")]
        public string DataAberturaCIOT { get; set; }

        [JsonProperty("nrCarga")]
        public string CodigoCargaEmbarcador { get; set; }

        [JsonProperty("tipoCarga")]
        public string TipoCarga { get; set; }

        [JsonProperty("centro")]
        public string CodigoFilialEmbarcador { get; set; }

        [JsonProperty("moeda")]
        public string Moeda { get; set; }

        [JsonProperty("valorTotalServico")]
        public decimal ValorFreteSubcontratacao { get; set; }

        [JsonProperty("baseCalculoIrCiot")]
        public decimal BaseCalculoIRRF { get; set; }

        [JsonProperty("valorIrCiot")]
        public decimal ValorIRRF { get; set; }

        [JsonProperty("taxaIrrf")]
        public decimal AliquotaIRRF { get; set; }

        [JsonProperty("baseCalculoRets")]
        public decimal BaseCalculoRetencao { get; set; }

        [JsonProperty("numDepIr")]
        public string NumeroDependentesIRRF { get; set; }

        [JsonProperty("valorInss")]
        public decimal ValorINSS { get; set; }

        [JsonProperty("taxaInss")]
        public decimal AliquotaINSS { get; set; }

        [JsonProperty("valorSest")]
        public decimal ValorSEST { get; set; }

        [JsonProperty("taxaSest")]
        public decimal AliquotaSEST { get; set; }

        [JsonProperty("valorSenat")]
        public decimal ValorSENAT { get; set; }

        [JsonProperty("TaxaSenat")]
        public decimal AliquotaSENAT { get; set; }

        [JsonProperty("items")]
        public List<Item> Itens { get; set; }

    }
}
