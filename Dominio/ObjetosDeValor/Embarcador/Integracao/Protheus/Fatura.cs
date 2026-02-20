using System;
using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Protheus
{
    public class Fatura
    {
        [JsonProperty(PropertyName = "pagador", Required = Required.Always)]
        public string CNPJPagador { get; set; }

        [JsonProperty(PropertyName = "dataini", Required = Required.Always)]
        public DateTime DataInicialFatura { get; set; }

        [JsonProperty(PropertyName = "datafim", Required = Required.Always)]
        public DateTime DataFinalFatura { get; set; }

        [JsonProperty(PropertyName = "fatura", Required = Required.Always)]
        public FaturaDados FaturaDados { get; set; }
    }
}