using System;
using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Protheus
{
    public class FaturaDados
    {
        [JsonProperty(PropertyName = "transportadora", Required = Required.Always)]
        public string CNPJTransportadora { get; set; }

        [JsonProperty(PropertyName = "dataEmissao", Required = Required.Always)]
        public DateTime DataEmissao { get; set; }

        [JsonProperty(PropertyName = "dataVencimento", Required = Required.Always)]
        public DateTime DataVencimento { get; set; }

        [JsonProperty(PropertyName = "dataVencimentoComDesconto", Required = Required.Always)]
        public DateTime DataVencimentoComDesconto { get; set; }

        [JsonProperty(PropertyName = "numeroFatura", Required = Required.Always)]
        public string NumeroFatura { get; set; }

        [JsonProperty(PropertyName = "valorFatura", Required = Required.Always)]
        public decimal ValorFatura { get; set; }

        [JsonProperty(PropertyName = "cte", Required = Required.Always)]
        public FaturaCTe[] FaturaCTe { get; set; }
    }
}