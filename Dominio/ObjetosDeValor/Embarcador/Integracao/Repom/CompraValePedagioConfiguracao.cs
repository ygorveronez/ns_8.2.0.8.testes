using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Repom
{
    public class CompraValePedagioConfiguracao
    {
        [JsonProperty(PropertyName = "eixosSuspensosIda", Required = Required.AllowNull)]
        public int EixosSuspensosIda { get; set; }

        [JsonProperty(PropertyName = "eixosSuspensosVolta", Required = Required.AllowNull)]
        public int EixosSuspensosVolta { get; set; }

        [JsonProperty(PropertyName = "roteiroPagamentoPedagio", Required = Required.AllowNull)]
        public string RoteiroPagamentoPedagio { get; set; }
    }
}
