using System;
using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Protheus
{
    public class FaturaCTe
    {
        [JsonProperty(PropertyName = "numCTE", Required = Required.Always)]
        public int NumeroCTe { get; set; }

        [JsonProperty(PropertyName = "serieCTE", Required = Required.Always)]
        public int SerieCTe { get; set; }

        [JsonProperty(PropertyName = "dataEmissaoCTE", Required = Required.Always)]
        public DateTime DataEmissaoCTe { get; set; }

        [JsonProperty(PropertyName = "valorFrete", Required = Required.Always)]
        public decimal ValorFrete { get; set; }
    }
}