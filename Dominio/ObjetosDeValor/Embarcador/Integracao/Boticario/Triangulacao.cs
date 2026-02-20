using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario
{
    public sealed class Triangulacao
    {
        [JsonProperty(PropertyName = "nfeVenda", Order = 2, Required = Required.Always)]
        public string ChaveRemessa { get; set; }

        [JsonProperty(PropertyName = "nfeRemessa", Order = 1, Required = Required.Always)]
        public string ChaveVenda { get; set; }
    }
}
