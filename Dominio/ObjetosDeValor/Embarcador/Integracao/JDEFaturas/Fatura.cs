using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.JDEFaturas
{
    public class Fatura
    {
        [JsonProperty(PropertyName = "cnpjEmissor")]
        public string CNPJEmissor { get; set; }

        [JsonProperty(PropertyName = "cnpjTransp")]
        public string CNPJTransportador { get; set; }

        [JsonProperty(PropertyName = "dataFatura")]
        public DateTime DataFatura { get; set; }

        [JsonProperty(PropertyName = "dataVencto")]
        public DateTime DataVencimento { get; set; }

        [JsonProperty(PropertyName = "id")]
        public int ID { get; set; }

        [JsonProperty(PropertyName = "noFatura")]
        public string NumeroFatura { get; set; }

        [JsonProperty(PropertyName = "valor")]
        public decimal Valor { get; set; }

        [JsonProperty(PropertyName = "faturaCTe")]
        public List<CTe> FaturaCTes { get; set; }
    }
}
