using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Repom
{
    public class RetornoConsultaRoteiro
    {
        [JsonProperty(PropertyName = "roteiro", Required = Required.AllowNull)]
        public RetornoConsultaRoteiroDetalhe Roteiro { get; set; }

        [JsonProperty(PropertyName = "paradas", Required = Required.AllowNull)]
        public List<RetornoConsultaRoteiroParada> Paradas { get; set; }

        [JsonProperty(PropertyName = "descricaoErro", Required = Required.AllowNull)]
        public string DescricaoErro { get; set; }

        [JsonProperty(PropertyName = "status", Required = Required.AllowNull)]
        public bool Status { get; set; }
    }
}
