using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy
{
    public class PedidoRestricao
    {
        [JsonProperty(PropertyName = "skills", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> CodigoIntegracaoTipoCarga { get; set; }

        [JsonProperty(PropertyName = "route", NullValueHandling = NullValueHandling.Ignore)]
        public string CodigoAgrupamentoCarregamento { get; set; }

        [JsonProperty(PropertyName = "vehicles_filter", NullValueHandling = NullValueHandling.Ignore)]
        public DetalhesVeiculo DetalhesVeiculo { get; set; }

        [JsonProperty(PropertyName = "priority", NullValueHandling = NullValueHandling.Ignore)]
        public int NivelPrioridade { get; set; }

        [JsonProperty(PropertyName = "region", NullValueHandling = NullValueHandling.Ignore)]
        public string Regiao { get; set; }
    }
}
