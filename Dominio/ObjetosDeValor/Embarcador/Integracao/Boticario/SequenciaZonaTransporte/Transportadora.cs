using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.SequenciaZonaTransporte
{
    public class Transportadora
    {
        [JsonProperty(PropertyName = "codigoTransportadora", Order = 1, Required = Required.Always)]
        public string CodigoIntegracao { get; set; }

        [JsonProperty(PropertyName = "cnpj", Order = 2, Required = Required.Always)]
        public string Cnpj { get; set; }
    }
}
