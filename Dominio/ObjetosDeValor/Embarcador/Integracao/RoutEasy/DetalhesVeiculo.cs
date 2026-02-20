using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy
{
    public class DetalhesVeiculo
    {
        [JsonProperty(PropertyName = "conditions")]
        public List<CondicoesVeiculo> CondicoesVeiculo { get; set; }
    }
}
