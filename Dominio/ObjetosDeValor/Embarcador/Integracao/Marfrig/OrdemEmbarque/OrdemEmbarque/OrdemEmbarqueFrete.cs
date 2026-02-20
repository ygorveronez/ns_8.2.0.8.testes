using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque
{
    public sealed class OrdemEmbarqueFrete
    {
        [JsonProperty(PropertyName = "motoristaCPF", Required = Required.Always)]
        public string MotoristaCpf { get; set; }

        [JsonProperty(PropertyName = "tipoFrete", Required = Required.Always)]
        public int TipoFrete { get; set; }

        [JsonProperty(PropertyName = "transportadoraCNPJ", Required = Required.Always)]
        public string TransportadorCnpj { get; set; }

        [JsonProperty(PropertyName = "veiculoDolly", Required = Required.AllowNull)]
        public string VeiculoDolly { get; set; }

        [JsonProperty(PropertyName = "veiculoTracao", Required = Required.AllowNull)]
        public string VeiculoTracao { get; set; }
    }
}
